using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edwon.PhysicsRigTools
{
    public class DebugControls : MonoBehaviour
    {    
        public void ToggleKinematic(bool toggle)
        {
            PhysicsRigUtils.ToggleRigKinematic(transform, toggle);
        }

        public void ResetRigPhysics()
        {
            PhysicsRigUtils.ResetRigPhysics(transform);
        }

        public void ResetRigTransformMove()
        {
            PhysicsRigUtils.ResetRigTransform(transform, true);
        }

        public void ResetRigTransform()
        {
            PhysicsRigUtils.ResetRigTransform(transform, false);
        }

        public void ResetRigTransformCached()
        {
            PhysicsRigUtils.ResetRigTransform(transform, false, true);
        }

        public void ResetRigTransformMoveCached()
        {
            PhysicsRigUtils.ResetRigTransform(transform, true, true);
        }

        public void FixChildJoints()
        {
            PhysicsRigUtils.FixChildJointsAfterScalingParent(transform);
        }

        public void MoveParentToCenterOfChild()
        {
            PhysicsRigUtils.MoveParentToCenterOfChildren(transform);
        }
    }
}