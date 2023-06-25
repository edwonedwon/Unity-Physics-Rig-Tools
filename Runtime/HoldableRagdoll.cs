using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.Tools;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Edwon.PhysicsRigTools
{
    public class HoldableRagdoll : MonoBehaviour, IHoldable
    {
        public bool debugLog;
        public GameObject GameObject { get {return gameObject;} }
        public Holder holder { get; set; }
        [SerializeField]
        [HideInInspector]
        bool isHeld;
        public bool IsHeld {get{ return isHeld;}}
        public Holder holderLast {get; set;}
        public Rigidbody rigidbodyToHold;
        public enum IsKinematicOption {None, RigidbodyToHold, All}
        public IsKinematicOption isKinematicOption;
        public Transform holdPoint;
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
            if (holdPoint == null)
                Debug.Log("hold point is null on " + gameObject.name);
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

            if (isKinematicOption == IsKinematicOption.RigidbodyToHold)
            {
                Utils.ToggleColliders(colliders, false, collidersDontToggle);
                rigidbodyToHold.transform.position = holder.transform.position;
                rigidbodyToHold.transform.rotation = holder.transform.rotation;
            }
            else if (isKinematicOption == IsKinematicOption.All)
            {
                PhysicsRigUtils.ToggleRigKinematic(transform, true);
                Utils.ToggleColliders(colliders, false, collidersDontToggle);
                transform.position = ParentPositionWithHoldPointOffset(holder.transform.position);
                transform.rotation = holder.transform.rotation;
            }
            else if (isKinematicOption == IsKinematicOption.None)
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

            if (isKinematicOption == IsKinematicOption.RigidbodyToHold || isKinematicOption == IsKinematicOption.All)
            {
                targetPosition = SmoothPosition(rigidbodyToHold.transform.position, holder.transform.position);
                targetRotation = SmoothRotation(holder.transform.rotation, holder.transform.rotation);
            }
            else if (isKinematicOption == IsKinematicOption.None)
            {
                targetPosition = SmoothPosition(rigidbodyToHold.transform.position, rigidbodyToHold.transform.position);
                targetRotation = SmoothRotation(rigidbodyToHold.transform.rotation, holder.transform.rotation);
            }

            if (isKinematicOption == IsKinematicOption.RigidbodyToHold || isKinematicOption == IsKinematicOption.All)
            {
                transform.position = ParentPositionWithHoldPointOffset(targetPosition);
                transform.rotation = targetRotation;        
            }
            else if (isKinematicOption == IsKinematicOption.None)
            {
                rigidbodyToHold.MovePosition(targetPosition);
                rigidbodyToHold.MoveRotation(targetRotation);
            }
        }

        Vector3 SmoothPosition(Vector3 current, Vector3 target)
        {
            if (SmoothMovement)
                return Vector3.SmoothDamp(current, target, ref velocity, moveTime, moveMaxSpeed);
            else
                return target;
        }

        Quaternion SmoothRotation(Quaternion current, Quaternion target)
        {
            if (SmoothMovement)
                return Quaternion.RotateTowards(current, target, rotateTime);
            else 
                return target;
        }

        Vector3 ParentPositionWithHoldPointOffset(Vector3 targetPosition)
        {
            Vector3 difference = transform.position - holdPoint.position;
            return targetPosition + difference;
        }

        public void OnRelease()
        {
            if (debugLog)
                Debug.Log("OnRelease " + gameObject.name);

            if (isKinematicOption == IsKinematicOption.RigidbodyToHold || isKinematicOption == IsKinematicOption.All)    
            {
                PhysicsRigUtils.ToggleRigKinematic(transform, false);
                Utils.ToggleColliders(colliders, true, collidersDontToggle);
            }
            else if (isKinematicOption == IsKinematicOption.None)    
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