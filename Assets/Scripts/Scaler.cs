using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scaler : MonoBehaviour
{
    public Slider slider;
    float startScale;
    public Transform physicsRootChild;
    ResetableRigidbody[] resetables;

    void Awake()
    {
        resetables = GetComponentsInChildren<ResetableRigidbody>();
        startScale = transform.lossyScale.x;
        slider.value = startScale;
    }

    void OnScaleBegin()
    {
        PhysicsRigUtils.ToggleRigKinematic(resetables, true);
        PhysicsRigUtils.MoveParentToCenterOfChild(transform, physicsRootChild);
        PhysicsRigUtils.CacheRig(resetables);
    }

    void OnScaleUpdate(float scale)
    {
        transform.localScale = new Vector3(scale,scale,scale);
    }

    void OnScaleEnd()
    {
        PhysicsRigUtils.ResetRigTransform(resetables, false);
        PhysicsRigUtils.FixChildJointsAfterScalingParent(transform);
        PhysicsRigUtils.ResetRigTransform(resetables, false, true);
        PhysicsRigUtils.ToggleRigKinematic(resetables, false);
    }

    public void OnSliderBegin()
    {
        OnScaleBegin();
    }

    public void OnSliderValueChanged(float toValue)
    {
        OnScaleUpdate(toValue);
    }

    public void OnSliderEnd()
    {
        OnScaleEnd();
    }
}
