using System;
using System.Collections;
using Tango;
using UnityEngine;

public class TangoTouchProjector : ITangoDepth, ITangoLifecycle
{
    private bool _findPlaneWaitingForDepth;
    private readonly TangoApplication _tangoApplication;
    private readonly TangoPointCloud _pointCloud;

    public TangoTouchProjector (TangoApplication tangoApplication, TangoPointCloud pointCloud)
    {
        _tangoApplication = tangoApplication;
        _pointCloud = pointCloud;
        _tangoApplication.Register (this);
    }

    public void OnDestroy ()
    {
        _tangoApplication.Unregister (this);
    }

    public IEnumerator Touch (Vector3 touchPosition, Action<Vector3, Quaternion, int> action)
    {
        _findPlaneWaitingForDepth = true;
        _tangoApplication.SetDepthCameraRate (TangoEnums.TangoDepthCameraRate.MAXIMUM);
        while (_findPlaneWaitingForDepth) {
//            action (Vector3.zero, Quaternion.identity, false);
            yield return null;
        }
        _tangoApplication.SetDepthCameraRate (TangoEnums.TangoDepthCameraRate.DISABLED);

        Vector3 planeCenter;
        Plane plane;
        if (_pointCloud.FindPlane (Camera.main, touchPosition, out planeCenter, out plane)) {
            if (Vector3.Angle (plane.normal, Vector3.up) < 30.0f) {
                var forward = CameraForwardOnPlane (plane);
                var lookToCamera = Quaternion.LookRotation (-forward, plane.normal);
                action (planeCenter, lookToCamera, 1);
            } else {
                action (Vector3.zero, Quaternion.identity, 0);
            }
        } else {
            action (Vector3.zero, Quaternion.identity, 2);
        }
    }

    private static Vector3 CameraForwardOnPlane (Plane plane)
    {
        Camera cam = Camera.main;
        Vector3 forward;
        if (Vector3.Angle (plane.normal, cam.transform.forward) < 175) {
            Vector3 right = Vector3.Cross (plane.normal, cam.transform.forward).normalized;
            forward = Vector3.Cross (right, plane.normal).normalized;
        } else {
            // Normal is nearly parallel to camera look direction, the cross product would have too much
            // floating point error in it.
            forward = Vector3.Cross (plane.normal, cam.transform.right);
        }
        return forward;
    }

    public void OnTangoDepthAvailable (TangoUnityDepth tangoDepth)
    {
        // Don't handle depth here because the PointCloud may not have been updated yet.  Just
        // tell the coroutine it can continue.
        _findPlaneWaitingForDepth = false;
    }

    public void OnTangoPermissions (bool permissionsGranted)
    {
    }

    public void OnTangoServiceConnected ()
    {
        _tangoApplication.SetDepthCameraRate (TangoEnums.TangoDepthCameraRate.DISABLED);
    }

    public void OnTangoServiceDisconnected ()
    {
    }
}