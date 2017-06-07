using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Glucodyn {
    GlucodynLib lib = new GlucodynLib();
    public void Reset() {
        lib.resetEvents();
    }

    public void AddInsulin(float t1 = 0, float t2 = 30, float umin = 0.05f) {
        lib.addTempBasalToModel(t1, t2, umin);
    }

    public void AddCarbs(float time = 0, float grams = 10) {
        lib.addCarbsToModel(time, grams);
    }

    public Dictionary<float, double> GetGlucoseEvents() {
        return lib.reloadGraphData().Select((v, i) => new {glucoseValue = v, index = i}).ToDictionary(kv => kv.index * lib.dt, kv => kv.glucoseValue);
    }

    //This is pulled from glucodyn's algorithm.js
    private class GlucodynLib {
        private class UserData {
            public float cratio;
            public float sensf;
            public int idur;
            public int bginitial;
            public int stats;
            public int simlength; //  hours
            public int inputeffect;

            public UserData() {
                cratio = 10f;
                sensf = 100f;
                idur = 3;
                bginitial = 120;
                stats = 0;
                simlength = 18;
                inputeffect = 1;
            }
        }
        private static UserData userdata = new UserData();

        private struct GlucodynEvent {
            public string etype;
            public float time;
            public float ctype;
            public float t1;
            public float t2;
            public float grams;
            public float units;
            public float dbdt;
            public int id;
        }
        private List<GlucodynEvent> uevent = new List<GlucodynEvent>();

        private int uevent_counter = 0;
        private int n; //points in simulation, default 75
        private float simt;
        internal float dt;
        private double[] simbgc;
        private double[] simbgi;
        private double[] simbg;
        internal GlucodynLib(int n = 150) {
            this.n = n;
            simt = userdata.simlength * 60;
            dt = simt / n;
            simbgc = new double[n];
            simbgi = new double[n];
            simbg = new double[n];
        }

        //g is time in minutes from bolus event, idur=insulin duration
        //walsh iob curves
        private double iob(float g, int idur) {
            double tot = 0;

            if (g <= 0.0) {
                tot = 100.0f;
            }
            else if (g >= idur * 60.0) {
                tot = 0.0;
            }
            else {
                if (idur == 3) {
                    tot = -3.203e-7 * Mathf.Pow(g, 4) + 1.354e-4 * Mathf.Pow(g, 3) - 1.759e-2 * Mathf.Pow(g, 2) + 9.255e-2 * g + 99.951;
                }
                else if (idur == 4) {
                    tot = -3.31e-8 * Mathf.Pow(g, 4) + 2.53e-5 * Mathf.Pow(g, 3) - 5.51e-3 * Mathf.Pow(g, 2) - 9.086e-2 * g + 99.95;
                }
                else if (idur == 5) {
                    tot = -2.95e-8 * Mathf.Pow(g, 4) + 2.32e-5 * Mathf.Pow(g, 3) - 5.55e-3 * Mathf.Pow(g, 2) + 4.49e-2 * g + 99.3;
                }
                else if (idur == 6) {
                    tot = -1.493e-8 * Mathf.Pow(g, 4) + 1.413e-5 * Mathf.Pow(g, 3) - 4.095e-3 * Mathf.Pow(g, 2) + 6.365e-2 * g + 99.7;
                }
            }
            return (tot);
        }


        //simpsons rule to integrate IOB - can include sf and dbdt as functions of tstar later - assume constants for now
        //integrating over flux time tstar
        private double intIOB(float x1, float x2, int idur, float g) {
            double integral;
            float dx;
            var nn = 50; //nn needs to be even
            var ii = 1;

            //initialize with first and last terms of simpson series
            dx = (x2 - x1) / nn;
            integral = iob((g - x1), idur) + iob(g - (x1 + nn * dx), idur);

            while (ii < nn - 2) {
                integral = integral + 4 * iob(g - (x1 + ii * dx), idur) + 2 * iob(g - (x1 + (ii + 1) * dx), idur);
                ii = ii + 2;
            }

            integral = integral * dx / 3.0;
            return (integral);
        }

        //scheiner gi curves fig 7-8 from Think Like a Pancreas, fit with a triangle shaped absorbtion rate curve
        //see basic math pdf on repo for details
        //g is time in minutes,gt is carb type
        private double cob(float g, float ct) {
            double tot;

            if (g <= 0) {
                tot = 0.0;
            }
            else if (g >= ct) {
                tot = 1.0;
            }
            else if ((g > 0) && (g <= ct / 2.0)) {
                tot = 2.0 / Mathf.Pow(ct, 2) * Mathf.Pow(g, 2);
            }
            else
                tot = -1.0 + 4.0 / ct * (g - Mathf.Pow(g, 2) / (2.0 * ct));
            return (tot);
        }


        private  double deltatempBGI(float g, float dbdt, float sensf, int idur, float t1, float t2) {
            return -dbdt * sensf * ((t2 - t1) - 1f / 100f * intIOB(t1, t2, idur, g));
        }

        private  double deltaBGC(float g, float sensf, float cratio, float camount, float ct) {
            return sensf / cratio * camount * cob(g, ct);
        }

        private  double deltaBGI(float g, float bolus, float sensf, int idur) {
            return -bolus * sensf * (1 - iob(g, idur) / 100.0);
        }

        private  double deltaBG(float g, float sensf, float cratio, float camount, float ct, float bolus, int idur) {
            return deltaBGI(g, bolus, sensf, idur) + deltaBGC(g, sensf, cratio, camount, ct);
        }


        private  float[] GlucodynStats(float[] bg) {
            var min = 1000f;
            var max = 0f;
            var sum = 0f;
            var averagebg = 0f;

            //calc average
            for (var ii = 0; ii < bg.Length; ii++) {
                sum = sum + bg[ii];
                averagebg = sum / bg.Length;
                //find min and max
                if (bg[ii] < min) {
                    min = bg[ii];
                }
                if (bg[ii] > max) {
                    max = bg[ii];
                }
            }

            //calc square of differences
            var dsq = 0f;
            for (int ii = 0; ii < bg.Length; ii++) {
                dsq = dsq += Mathf.Pow((bg[ii] - averagebg), 2); //lol?
            }
            //calc sd
            var sd = Mathf.Pow((dsq / bg.Length), 0.5f);

            float[] result = new float[4];
            result[0] = averagebg;
            result[1] = sd;
            result[2] = min;
            result[3] = max;

            return result;
        }


        // Max Sim Time
        private  void RecommendedMaxSimTime() {
            var maxsimtime = 0f;
            for (var ii = 0; ii < uevent.Count; ii++) {
                var etime = 0f;

                if (uevent[ii].etype == "bolus") {
                    etime = uevent[ii].time + userdata.idur * 60f;
                }
                if (uevent[ii].etype == "carb") {
                    etime = uevent[ii].time + uevent[ii].ctype;
                }
                if (uevent[ii].etype == "tempbasal") {
                    etime = uevent[ii].t2 + userdata.idur * 60;
                }
                if (etime > maxsimtime) {
                    maxsimtime = etime;
                }
            }
        }

        internal double[] reloadGraphData() {
            for (int i = 0; i < n; i++) {
                simbg[i] = userdata.bginitial;
                simbgc[i] = 0.0f;
                simbgi[i] = 0.0f;
            }

            for (int j = 0; j < uevent.Count; j++) {
//          if (uevent[j] && uevent.etype != "") { //Original code includes 'uevent.etype != ""' which looks like a typo...

                for (int i = 0; i < n; i++) {
                    if (uevent[j].etype == "carb") {
                        simbgc[i] = simbgc[i] + deltaBGC(i * dt - uevent[j].time, userdata.sensf, userdata.cratio, uevent[j].grams, uevent[j].ctype);
                    }
                    else if (uevent[j].etype == "bolus") {
                        simbgi[i] = simbgi[i] + deltaBGI(i * dt - uevent[j].time, uevent[j].units, userdata.sensf, userdata.idur);
                    }
                    else {
                        simbgi[i] = simbgi[i] + deltatempBGI((i * dt), uevent[j].dbdt, userdata.sensf, userdata.idur, uevent[j].t1, uevent[j].t2);
                    }
                }
            }

            for (int i = 0; i < n; i++) {
                // console.log(i +": " + simbgc[i] + " - " + simbgi[i]);
                simbg[i] = userdata.bginitial + simbgc[i] + simbgi[i];
            }
            return simbg;
        }


        internal  void addCarbsToModel(float time, float grams) {
            uevent_counter = uevent_counter + 1;

            // id: uevent_counter,
            // time: $("#carb_time_slider").slider("option", "value"),
            // etype: "carb",
            // grams: $("#carb_amount_slider").slider("option", "value"),
            // ctype: $("#carb_type_slider").slider("option", "value")
            GlucodynEvent newEvent = new GlucodynEvent();
            newEvent.id = uevent_counter;
            newEvent.time = time;
            newEvent.etype = "carb";
            newEvent.grams = grams;
            newEvent.ctype = 180;
            uevent.Add(newEvent);
        }


        ///////////////////////////////////////
        internal  void addBolisToModel(float time, float units) {
            uevent_counter = uevent_counter + 1;

            GlucodynEvent newEvent = new GlucodynEvent();
            newEvent.id = uevent_counter;
            newEvent.time = time;
            newEvent.etype = "bolus";
            newEvent.units = units;
            uevent.Add(newEvent);
        }


        internal  void addTempBasalToModel(float t1, float t2, float umin) {
            uevent_counter = uevent_counter + 1;

            GlucodynEvent newEvent = new GlucodynEvent();

            newEvent.id = uevent_counter;
            newEvent.time = t1;
            newEvent.etype = "tempbasal";
            newEvent.t1 = t1;
            newEvent.t2 = t2;
            newEvent.dbdt = umin; //   U/min temp basal input

            uevent.Add(newEvent);
        }

        internal  void resetEvents() {
            uevent = new List<GlucodynEvent>();
            uevent_counter = 0;
        }
    }
}