using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Edwon.Tools;

namespace Edwon.PhysicsRigTools
{
    public enum PlacementType { Touch, Raycast }

    [RequireComponent(typeof(RigScaler))]
    public class DraggableRagdoll : MonoBehaviour, IDraggable
    {
        [Header("Setup")]
        public Rigidbody rigidbodyToDrag;
        [Header("the parent transform, will be moved while whole ragdoll is kinematic")]
        public Transform ragdollParent;

        [Header("Lerp")]
        public PlacementType placementType;
        public float distanceFromCamera = 1f;
        public LayerMask raycastLayerMask;
        public float moveTime;  
        public float moveMaxSpeed;
        public float rotateTime;

        [Header("Options")]
        public bool setRotation = false;

        [Header("Debug")]
        public bool debugLog;
        public bool debugDraw;
        [SerializeField]
        [ReadOnly]
        bool isDragged;
        public bool IsDragged {get{ return isDragged;}}
        [ReadOnly]
        [SerializeField]
        bool draggingEnabled;

        Camera playerCamera;
        RigScaler rigScaler;
        Vector3 targetPosition;
        Quaternion targetRotation;
        ResetableRigidbody[] resetableRigidbodies;
        List<Rigidbody> rigidbodies;
        Vector3 velocity = Vector3.zero;
        
        void Awake()
        {
            draggingEnabled = true;
            rigScaler = GetComponent<RigScaler>();
            playerCamera = Camera.main;
            resetableRigidbodies = transform.GetComponentsInChildren<ResetableRigidbody>();
            rigidbodies = transform.GetComponentsInChildren<Rigidbody>().ToList();
        }

        [InspectorButton("DisableDragging")]
        public bool disableDragging;
        public void DisableDragging()
        {
            OnDragEnd(Vector2.zero);
            draggingEnabled = false;
        }

        // need to call this from some touch plugin
        public void OnDragBegin(Vector2 screenPos)
        {
            if (!draggingEnabled)
                return;

            if (debugLog)
                Debug.Log(gameObject.name + " OnDragBegin");

            isDragged = true;

            rigidbodyToDrag.isKinematic = true;
        }

        // need to call this from some touch plugin
        public void OnDragUpdate(Vector2 screenPos)
        {
            if (!draggingEnabled)
                return;

            if (debugLog)
                Debug.Log(gameObject.name + " OnDragUpdate");

            // RAYCAST
            Ray ray = playerCamera.ScreenPointToRay(screenPos);

            if (debugDraw)
                Debug.DrawRay(ray.origin, ray.direction, Color.red, .001f);

            if (placementType == PlacementType.Touch)
            {
                targetPosition = playerCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
            }
            else if (placementType == PlacementType.Raycast)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 10000000, raycastLayerMask))
                {
                    // UPDATE POSITION
                    targetPosition = new Vector3(
                        hit.point.x, 
                        hit.point.y, 
                        hit.point.z);
            
                    if (debugDraw)
                        Debug.DrawLine(hit.point, targetPosition, Color.magenta);
                }

                // UPDATE ROTATION
                Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.position - hit.point, Vector3.up);
                targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }

        // need to call this from some touch plugin
        public void OnDragEnd(Vector2 screenPos)
        {
            if (!draggingEnabled)
                return;
                
            if (debugLog)
                Debug.Log(gameObject.name + " OnDragEnd");

            isDragged = false;

            rigidbodyToDrag.isKinematic = false;
        }

        void LateUpdate()
        {
            if (!isDragged)
                return;

            if (rigScaler.tweening)
                UpdateMoveParent();
            else
                UpdateMoveRigidbodyToDrag();
        }

        void UpdateMoveParent()
        {
            Vector3 targetPositionSmooth = Vector3.SmoothDamp(ragdollParent.position, targetPosition, ref velocity, moveTime, moveMaxSpeed);
            Quaternion targetRotationSmooth = Quaternion.RotateTowards(ragdollParent.transform.rotation, targetRotation, rotateTime);

            ragdollParent.position = targetPositionSmooth;
            if (setRotation)
                ragdollParent.transform.rotation = targetRotationSmooth;
        }

        void UpdateMoveRigidbodyToDrag()
        {
            Vector3 targetPositionSmooth = Vector3.SmoothDamp(rigidbodyToDrag.transform.position, targetPosition, ref velocity, moveTime, moveMaxSpeed);
            Quaternion targetRotationSmooth = Quaternion.RotateTowards(rigidbodyToDrag.transform.rotation, targetRotation, rotateTime);

            rigidbodyToDrag.MovePosition(targetPositionSmooth);
            if (setRotation)
                rigidbodyToDrag.MoveRotation(targetRotationSmooth);
        }
    }
}