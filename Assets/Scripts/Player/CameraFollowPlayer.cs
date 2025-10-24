using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollowPlayer : MonoBehaviour
{

    #region Variables

    private Vector3 offSet = new Vector3(0f, 0f, -10f); // based on target
    private Vector3 velocitySpeed = Vector3.zero; // speed of camera

    public float smoothTime = 0.25f; // the smoothing time for the camera - changed in inspector

    [SerializeField] private Transform target; // what the camera follows

    [Space]
    [Header("Tether")]
    [SerializeField] private bool tetherToggle; // true = will tether, false = wont tether - changed in inspector
    [SerializeField] private int tetherLimit; // how far the object can be from the target before teleporting to it

    [Tooltip("Distance from target")]
    [SerializeField] private float tetherDistance; // the distance

    #endregion

    private void Update()
    {
        Vector3 targetPosition = target.position + offSet; // location the camera should go

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocitySpeed, smoothTime); // move camera to target on a smooth damp
    }

    private void FixedUpdate()
    {
        TetherDistanceCheck();
    }

    public void TetherDistanceCheck() // only if tether toggled check if too far from target
    {
        if (tetherToggle == false)
        {
            return;
        }

        tetherDistance = Vector3.Distance(transform.position, target.position);

        if (tetherDistance > tetherLimit)
        {
            transform.position = target.position;
        }
    }

    public void SetTargetObject(Transform objectTransform) // if need to change target in play
    {
        target = objectTransform;
    }


}