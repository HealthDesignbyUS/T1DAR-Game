using System.Linq;
using UnityEngine;
using NUnit.Framework;

public class GlucodynTest {


    [Test]
    public void GlucodynFirstTest() {
        var glucodyn = new Glucodyn();
        glucodyn.AddInsulin(0f, 15f);

        glucodyn.AddCarbs(5f, 10f);
        glucodyn.AddCarbs(10f, 10f);
        glucodyn.AddCarbs(15f, 10f);

        var glucoseEvents = glucodyn.GetGlucoseEvents();

        Assert.AreEqual(150, glucoseEvents.Count);
        Assert.AreEqual(0, glucoseEvents.Keys.First());
        Assert.AreEqual(-0.2399d, glucoseEvents.Values.First(), 0.001f);

        foreach (var glucoseEvent in glucoseEvents) {
            Debug.Log(glucoseEvent);
        }
    }

    [Test]
    public void GlucodynSecondTest() {
        var glucodyn = new Glucodyn();
        var glucoseEvents = glucodyn.GetGlucoseEvents();
        Assert.AreEqual(150, glucoseEvents.Count);
        Assert.AreEqual(0, glucoseEvents.Keys.First());
        Assert.AreEqual(0, glucoseEvents.Values.First(), 0.001f);

        foreach (var glucoseEvent in glucoseEvents) {
            Debug.Log(glucoseEvent);
        }
    }
}
