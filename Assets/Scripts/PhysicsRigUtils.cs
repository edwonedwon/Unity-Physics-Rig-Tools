
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

public class PhysicsRigUtils
{
    public static void ScaleStart(Transform parent, ResetableRigidbody[] resetables, Transform scalePivot = null)
    {
        PhysicsRigUtils.ToggleRigKinematic(resetables, true);
        if (scalePivot)
            PhysicsRigUtils.MoveParentToCenterOfTransform(parent, scalePivot);
        else
            PhysicsRigUtils.MoveParentToCenterOfChildren(parent);
        PhysicsRigUtils.CacheRig(resetables);
    }

    public static void ScaleUpdate(Transform parent, Vector3 scale)
    {
        parent.localScale = scale;
    }

    public static void ScaleEnd(Transform parent, ResetableRigidbody[] resetables)
    {
        PhysicsRigUtils.ResetRigTransform(resetables, false);
        PhysicsRigUtils.FixChildJointsAfterScalingParent(parent);
        PhysicsRigUtils.ResetRigTransform(resetables, false, true);
        PhysicsRigUtils.ToggleRigKinematic(resetables, false);
    }

    public static void FullResetRig(Transform parent, ResetableRigidbody[] resetables, Transform scalePivot = null)
    {
        ToggleRigKinematic(resetables, true);
        if (scalePivot)
            MoveParentToCenterOfTransform(parent, scalePivot);
        else
            MoveParentToCenterOfChildren(parent);
        ResetRigTransform(resetables, false);
        FixChildJointsAfterScalingParent(parent);
        ToggleRigKinematic(resetables, false);
    }

    public static void ToggleRigKinematic(Transform parent, bool toggle)
    {
        ResetableRigidbody[] resetables = parent.GetComponentsInChildren<ResetableRigidbody>();
        ToggleRigKinematicPrivate(resetables, toggle);
    }

    public static void ToggleRigKinematic(ResetableRigidbody[] resetables, bool toggle)
    {
        ToggleRigKinematicPrivate(resetables, toggle);
    }

    static void ToggleRigKinematicPrivate(ResetableRigidbody[] resetables, bool toggle)
    {
        if (resetables == null || resetables.Length == 0)
            return;

        foreach (ResetableRigidbody resetable in resetables)
        {
            resetable.ToggleKinematic(toggle);
        }
    }

    public static void ResetRigTransform(Transform parent,bool useMove = true, bool useCached = false)
    {
        ResetableRigidbody[] resetables = parent.GetComponentsInChildren<ResetableRigidbody>();
        ResetRigTransformPrivate(resetables, useMove, useCached);
    }

    public static void ResetRigTransform(ResetableRigidbody[] resetables, bool useMove, bool useCached = false)
    {
        ResetRigTransformPrivate(resetables, useMove, useCached);
    }

    static void ResetRigTransformPrivate(ResetableRigidbody[] resetables, bool useMove = true, bool useCached = false)
    {
        if (resetables == null || resetables.Length == 0)
            return;

        foreach (ResetableRigidbody resetable in resetables)
            resetable.ResetRigTransform(useMove, useCached);
    }

    public static void ResetRigPhysics(Transform parent)
    {
        ResetableRigidbody[] resetables = parent.GetComponentsInChildren<ResetableRigidbody>();
        ResetRigPhysicsPrivate(resetables);
    }

    public static void ResetRigPhysics(ResetableRigidbody[] resetables)
    {
        ResetRigPhysicsPrivate(resetables);
    }

    static void ResetRigPhysicsPrivate(ResetableRigidbody[] resetables)
    {
        if (resetables == null || resetables.Length == 0)
            return;

        foreach (ResetableRigidbody resetable in resetables)
            resetable.ResetRigPhysics();
    }

    // fixes child joints of a parent that has been scaled
    public static void FixChildJointsAfterScalingParent(Transform parent)
    {
        Joint[] joints = parent.GetComponentsInChildren<Joint>();
        if (joints == null || joints.Length == 0)
            return; 
        
        FixChildJointsAfterScalingParentPrivate(parent, joints);
    }

    // fixes child joints of a parent that has been scaled
    public static void FixChildJointsAfterScalingParent(Transform parent, Joint[] joints)
    {
        FixChildJointsAfterScalingParentPrivate(parent, joints);
    }

    static void FixChildJointsAfterScalingParentPrivate(Transform parent, Joint[] joints)
    {
        if (joints == null || joints.Length == 0)
            return;

        foreach (Joint j in joints)
        {
            j.autoConfigureConnectedAnchor = false;
            Rigidbody connectedBody = j.connectedBody;
            j.connectedBody = null;
            Vector3 newConnectedAnchor = parent.TransformPoint(j.connectedAnchor);
            newConnectedAnchor = parent.InverseTransformPoint(newConnectedAnchor);
            j.connectedAnchor = newConnectedAnchor;
            j.connectedBody = connectedBody;
        }
    }

    public static void CacheRig(Transform parent)
    {
        ResetableRigidbody[] resetables = parent.GetComponentsInChildren<ResetableRigidbody>();
        CacheRigPrivate(resetables);
    }

    public static void CacheRig(ResetableRigidbody[] resetables)
    {
        CacheRigPrivate(resetables);
    }

    static void CacheRigPrivate(ResetableRigidbody[] resetables)
    {
        if (resetables == null || resetables.Length == 0)
            return;

        foreach (ResetableRigidbody resetable in resetables)
        {
            resetable.CacheTransform();
        }
    }

    public static void MoveParentToCenterOfChildren(Transform parent)
    {
        Vector3 average = Vector3.zero;
        int childCount = parent.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        for (int i = 0; i < childCount; i++)
        {
            average += children[i].position;
            children[i].parent = null;
        }
        Vector3 center = average/childCount;
        parent.position = center;
        for (int i = 0; i < childCount; i++)
        {
            children[i].parent = parent;
        }
    }

    public static void MoveParentToCenterOfTransform(Transform parent, Transform scalePivot)
    {
        int childCount = parent.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        for (int i = 0; i < childCount; i++)
        {
            children[i].parent = null;
        }
        parent.position = scalePivot.position;
        parent.rotation = scalePivot.rotation;
        for (int i = 0; i < childCount; i++)
        {
            children[i].parent = parent;
        }
    }
}