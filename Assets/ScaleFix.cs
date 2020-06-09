using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFix : MonoBehaviour
{
    Joint[] joints;

    public bool debugDraw;
    float debugDrawSize = 0.08f;

    void OnDrawGizmos()
    {
        if (joints == null)
            return;

        foreach(Joint j in joints)
        {
            Color color = new Color((int)Random.Range(0,255), (int)Random.Range(0,255), (int)Random.Range(0,255), 1);
            Gizmos.color = color;
            Vector3 anchor = j.transform.TransformPoint(j.anchor);
            Gizmos.DrawSphere(anchor, debugDrawSize);
            Vector2 connectedAnchor = j.connectedBody.transform.TransformPoint(j.connectedAnchor);
            Gizmos.DrawCube(connectedAnchor, new Vector3(debugDrawSize, debugDrawSize, debugDrawSize));
        }
    }

    void Awake()
    {
        joints = transform.GetComponentsInChildren<Joint>();
    }

    [InspectorButton("FixScale")]
    public bool fixScale;
    public void FixScale()
    {
        foreach(Joint j in joints)
        {
            j.autoConfigureConnectedAnchor = false;
            Rigidbody connectedBody = j.connectedBody;
            j.connectedBody = null;
            Vector3 newConnectedAnchor = transform.TransformPoint(j.connectedAnchor);
            newConnectedAnchor = transform.InverseTransformPoint(newConnectedAnchor);
            j.connectedAnchor = newConnectedAnchor;
            j.connectedBody = connectedBody;
        }
    }
}
