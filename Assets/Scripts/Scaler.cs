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
        Utils.ToggleRigKinematic(resetables, true);
        Utils.MoveParentToCenterOfChild(transform, physicsRootChild);
    }

    void OnScaleUpdate(float scale)
    {
        transform.localScale = new Vector3(scale,scale,scale);
    }

    void OnScaleEnd()
    {
        Utils.ResetRigTransform(resetables, false);
        Utils.FixChildJointsAfterScalingParent(transform);
        Utils.ToggleRigKinematic(resetables, false);
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
