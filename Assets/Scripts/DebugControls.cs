using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugControls : MonoBehaviour
{    
    public void ToggleKinematic(bool toggle)
    {
        Utils.ToggleRigKinematic(transform, toggle);
    }

    public void ResetRigPhysics()
    {
        Utils.ResetRigPhysics(transform);
    }

    public void ResetRigTransformMove()
    {
        Utils.ResetRigTransform(transform, true);
    }

    public void ResetRigTransform()
    {
        Utils.ResetRigTransform(transform, false);
    }

    public void FixChildJoints()
    {
        Utils.FixChildJointsAfterScalingParent(transform);
    }

    public void MoveParentToCenterOfChild()
    {
        Utils.MoveParentToCenterOfChildren(transform);
    }
}
