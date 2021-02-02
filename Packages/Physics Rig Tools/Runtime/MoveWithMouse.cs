using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edwon.PhysicsRigTools
{
    public class MoveWithMouse : MonoBehaviour
    {
        public Camera cam;
        Rigidbody rb;
        public float distance;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (MouseInputUIBlocker.BlockedByUI)
                return;

            if (Input.GetMouseButton(0))
            {
                rb.isKinematic = true;
                Move();
            }
            if (Input.GetMouseButtonUp(0))
            {
                rb.isKinematic = false;
            }
        }

        void Move()
        {
            Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint);
            rb.MovePosition(worldPoint);
            rb.MoveRotation(Quaternion.identity);
        }
    }
}