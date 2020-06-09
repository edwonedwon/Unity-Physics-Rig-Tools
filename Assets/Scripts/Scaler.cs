using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scaler : MonoBehaviour
{
    public Slider slider;
    float startScale;
    public Transform rootChild;

    void Awake()
    {
        startScale = transform.lossyScale.x;
        slider.value = startScale;
    }

    void OnScaleBegin()
    {
        // Utils.MoveParentToCenterOfChild(transform, rootChild);
        // Utils.ToggleRigKinematic(true, transform);
    }

    void OnScaleUpdate(float scale)
    {
        transform.localScale = new Vector3(scale,scale,scale);
    }

    void OnScaleEnd()
    {
        // Utils.FixChildJointsAfterScalingParent(transform);
        // Utils.ResetRigTransform(transform, false, false);
        // Utils.ToggleRigKinematic(false, transform);
        // Utils.ResetRigPhysics(transform);
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
