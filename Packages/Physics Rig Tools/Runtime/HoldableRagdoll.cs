using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.Tools;
using UnityEngine.Events;

namespace Edwon.PhysicsRigTools
{
    public class HoldableRagdoll : MonoBehaviour, IHoldable
    {
        public bool debugLog;
        public GameObject GameObject { get {return gameObject;} }
        public Holder holder { get; set; }
        [SerializeField]
        [HideInInspector]
        [ReadOnly]
        bool isHeld;
        public bool IsHeld {get{ return isHeld;}}
        public Holder holderLast {get; set;}
        public Transform ragdollParent;
        public Rigidbody rigidbodyToHold;
        public bool isKinematicWhileHeld = false;
        [Header("Smooth Move Settings")]
        public float moveTime = 0.05f;
        public float moveMaxSpeed = 40f;
        public float rotateTime = 1f;
        Vector3 velocity = Vector3.zero;
        Collider[] colliders;
        public LayerMask collidersDontToggle;
        public UnityEvent onHold;
        public UnityEvent onRelease;
        IDestroyable destroyable;
        [Header("Smooth Movement")]
        public float lerpTime = 0.95f;
        bool smoothMovement;
        public bool SmoothMovement 
        {
            get
            {
                return smoothMovement;
            }
            set
            {
                smoothMovement = value;
            }
        }

        void Awake()
        {
            destroyable = GetComponent<IDestroyable>();
            colliders = GetComponentsInChildren<Collider>();
            SetInterpolation();
            SetIsHeld();
        }

        void SetInterpolation()
        {
            Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
            foreach(Rigidbody r in rbs)
                r.interpolation = RigidbodyInterpolation.Interpolate;
        }

        void SetIsHeld()
        {
            if (holder == null)
                isHeld = false;
            else
                isHeld = true;
        }

        public void OnHold(Holder _holder)
        {
            if (debugLog)
                Debug.Log(gameObject.name + "OnHold " + gameObject.name + " To:" + _holder.name);

            if (isKinematicWhileHeld)
            {
                Utils.ToggleColliders(colliders, false, collidersDontToggle);
                ragdollParent.position = holder.transform.position;
                ragdollParent.rotation = holder.transform.rotation;
            }
            else
            {
                rigidbodyToHold.isKinematic = true;
                rigidbodyToHold.MovePosition(holder.transform.position);
                rigidbodyToHold.MoveRotation(holder.transform.rotation);
            }
            onHold.Invoke();
        }

        void LateUpdate()
        {
            SetIsHeld();

            if (holder == null)
                return;

            Vector3 targetPosition = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;

            if (isKinematicWhileHeld)
            {
                targetRotation = GetTargetRotation(holder.transform.rotation, holder.transform.rotation);
                targetPosition = GetTargetPosition(ragdollParent.transform.position, holder.transform.position);
            }
            else
            {
                targetPosition = GetTargetPosition(ragdollParent.transform.position, rigidbodyToHold.transform.position);
                targetRotation = GetTargetRotation(rigidbodyToHold.transform.rotation, holder.transform.rotation);
            }

            if (isKinematicWhileHeld)
            {
                ragdollParent.transform.position = targetPosition;
                ragdollParent.transform.rotation = targetRotation;        
            }
            else
            {
                rigidbodyToHold.MovePosition(targetPosition);
                rigidbodyToHold.MoveRotation(targetRotation);
            }
        }

        Vector3 GetTargetPosition(Vector3 current, Vector3 target)
        {
            if (SmoothMovement)
                return Vector3.SmoothDamp(current, target, ref velocity, moveTime, moveMaxSpeed);
            else
                return target;
        }

        Quaternion GetTargetRotation(Quaternion current, Quaternion target)
        {
            if (SmoothMovement)
                return Quaternion.RotateTowards(current, target, rotateTime);
            else 
                return target;
        }

        public void OnRelease()
        {
            if (debugLog)
                Debug.Log("OnRelease " + gameObject.name);

            if (isKinematicWhileHeld)    
            {
                PhysicsRigUtils.ToggleRigKinematic(transform, false);
                Utils.ToggleColliders(colliders, true, collidersDontToggle);
            }
            else
            {
                rigidbodyToHold.isKinematic = false;
            }

            onRelease.Invoke();
        }

        public void Release(bool andDestroy)
        {
            if (holder != null)
                holder.Release();
            if (andDestroy)
            {
                destroyable.DestroySelf();
            }
        }
    }
}