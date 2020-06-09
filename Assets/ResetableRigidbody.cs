using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Puppetoon
{
	[RequireComponent(typeof(Rigidbody))]
	public class ResetableRigidbody : MonoBehaviour 
	{
		Rigidbody rb;
		Vector3 localStartPosition;
		Quaternion localStartRotation;
        // Vector3 connectedAnchor;

        [NonSerialized]
        Vector3 localEndPosition;
        [NonSerialized]
        Quaternion localEndRotation;

        List<JointAndConnectedAnchor> joints;
        Joint[] jointsTemp;
        public class JointAndConnectedAnchor 
        {
            public Joint joint;
            public Vector3 connectedAnchor;
            public Rigidbody connectedBody;
        }

        void Awake()
		{
			rb = GetComponent<Rigidbody>();
            joints = new List<JointAndConnectedAnchor>();
            jointsTemp = new Joint[10];
            if (transform.GetComponent<Joint>())
            {
                jointsTemp = transform.GetComponents<Joint>();
                foreach(Joint j in jointsTemp)
                {
                    JointAndConnectedAnchor newJoint = new JointAndConnectedAnchor();
                    newJoint.joint = j;
                    newJoint.connectedAnchor = j.connectedAnchor;
                    joints.Add(newJoint);
                }
            }
            SetStartPositionRotation();
		}

        public void ResetRigidbodyPositionRotation(bool resetToEndPositionRotation = false) 
        {
            if (!rb.isKinematic) 
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            transform.localPosition = resetToEndPositionRotation ? localEndPosition : localStartPosition;
            transform.localRotation = resetToEndPositionRotation ? localEndRotation : localStartRotation;
        }

        public void DisconnectJoints()
        {
            if (joints.Count ==  0)
                return; 

            foreach(JointAndConnectedAnchor j in joints)
            {
                j.joint.autoConfigureConnectedAnchor = false;
                j.connectedBody = j.joint.connectedBody;
                j.joint.connectedBody = null;
            }
        }

        public void ReconnectJoints()
        {
            if (joints.Count ==  0)
                return; 

            foreach(JointAndConnectedAnchor j in joints)
            {
                j.joint.connectedBody = j.connectedBody;
                j.joint.connectedAnchor = j.connectedAnchor;
            }
        }

        public void DisableRigidbody()
        {
            rb.isKinematic = true;
        }

        public void EnableRigidbody()
        {
            rb.isKinematic = false;
        }

        void SetStartPositionRotation()
        {
			localStartPosition = transform.localPosition;
			localStartRotation = transform.localRotation;
        }

        public void SetEndPositionRotation() 
        {
            localEndPosition = transform.position;
            localEndRotation = transform.rotation;
        }
    }
}