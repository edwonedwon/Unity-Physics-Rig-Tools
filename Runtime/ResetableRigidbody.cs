using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Edwon.PhysicsRigTools
{
    [RequireComponent(typeof(Rigidbody))]
    public class ResetableRigidbody : MonoBehaviour 
    {
        Rigidbody rb;
        Vector3 localPositionStart;
        Quaternion localRotationStart;
        Vector3 centerOfMassStart;
        Vector3 inertiaTensorStart;

        [NonSerialized]
        Vector3 localPositionCached;
        [NonSerialized]
        Quaternion localRotationCached;

        ConfigurableJoint joint;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            centerOfMassStart = rb.centerOfMass;
            inertiaTensorStart = rb.inertiaTensor;
            SetStartLocalPositionRotation();
        }

        public void ResetRigPhysics() 
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // rb.inertiaTensor = inertiaTensorStart;
            // rb.centerOfMass = centerOfMassStart;
            // rb.ResetCenterOfMass();
            // rb.ResetInertiaTensor();
        }

        // ONLY CALL THIS WHILE KINEMATIC
        public void ResetRigTransform(bool useMovePositionRotation = true, bool useCached = false)
        {
            if (useMovePositionRotation)
            {
                Vector3 localPosition = useCached ? localPositionCached : localPositionStart;
                Quaternion localRotation = useCached ? localRotationCached : localRotationStart;
                Quaternion worldRotation = transform.parent.rotation * localRotation;
                rb.MovePosition(transform.parent.TransformPoint(localPosition));
                rb.MoveRotation(worldRotation);
            }
            else
            {
                transform.localPosition = useCached ? localPositionCached : localPositionStart;
                transform.localRotation = useCached ? localRotationCached : localRotationStart;
            }
        }

        public void ToggleKinematic(bool toggle)
        {
            rb.isKinematic = toggle;
        }

        void SetStartLocalPositionRotation()
        {
            localPositionStart = transform.localPosition;
            localRotationStart = transform.localRotation;
        }

        public void CacheTransform() 
        {
            localPositionCached = transform.localPosition;
            localRotationCached = transform.localRotation;
        }
    }
}