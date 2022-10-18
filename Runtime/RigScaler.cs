using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if EDWON_DOTWEEN
using DG.Tweening;
#endif
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace Edwon.PhysicsRigTools
{
    public class RigScaler : MonoBehaviour
    {
        public bool setScaleOnAwake;
        public enum ScaleOnAwakeType{Instant, Tweened};
        public ScaleOnAwakeType setScaleOnAwakeType;
        public float scaleOnAwake = 1;
        public bool scaleOnAwakeIsKinematicAfter = false;
        ResetableRigidbody[] resetableRigidbodies;
        Joint[] joints;
        public float tweenDuration = 1f;
        #if EDWON_DOTWEEN
        public Ease tweenEase;
        #endif
        [Header("if TweenScaleToSetValue is called, this value will be used")]
        public float tweenScaleToSetValue = 1f;
        public float kinematicScaleRatio = 1f;

        public UnityEvent onTweenScaleEnd;
        [ReadOnly]
        public bool tweening = false;
        public float debugTweenScaleTo = 1f;

        void Awake()
        {
            resetableRigidbodies = GetComponentsInChildren<ResetableRigidbody>();
            joints = GetComponentsInChildren<Joint>();

            if (setScaleOnAwake)
            {
                if (setScaleOnAwakeType == ScaleOnAwakeType.Instant)
                    ScaleTo(scaleOnAwake, scaleOnAwakeIsKinematicAfter);
                else if (setScaleOnAwakeType == ScaleOnAwakeType.Tweened)
                    TweenScaleTo(scaleOnAwake, scaleOnAwakeIsKinematicAfter);
            }
        }

        public void ScaleTo(float toScale, bool isKinematicAfter = false)
        {
            OnScaleStart();
            OnScaleUpdate(toScale);
            PhysicsRigUtils.ScaleEnd(transform, resetableRigidbodies, isKinematicAfter);
        }

        public void ScaleToWhileKinematic(float toScale)
        {
            float ratioScale = toScale * kinematicScaleRatio;
            transform.localScale = new Vector3(ratioScale, ratioScale, ratioScale);
        }

        void OnScaleStart()
        {
            PhysicsRigUtils.ScaleStart(transform, resetableRigidbodies);
        }

        void OnScaleUpdate(float toScale)
        {
            PhysicsRigUtils.ScaleUpdate(transform, new Vector3(toScale, toScale, toScale));
        }

        [InspectorButton("DebugSetScaleWithTween")]
        public bool debugSetScaleWithTween;
        public void DebugSetScaleWithTween()
        {
            TweenScaleTo(debugTweenScaleTo, false);
        }

        public void TweenScaleTo(float toScale, bool isKinematicAfter = false)
        {
            tweening = true;
            OnScaleStart();
            float currentScale = transform.lossyScale.x;
            #if EDWON_DOTWEEN
            DOTween.To(OnScaleUpdate, currentScale, toScale, tweenDuration)
                .OnComplete(()=> OnTweenScaleEnd(isKinematicAfter))
                .SetEase(tweenEase);
            #endif
        }

        public void SetTweenScaleToSetValue(float value)
        {
            tweenScaleToSetValue = value;
        }

        public void TweenScaleToSetValue()
        {
            TweenScaleTo(tweenScaleToSetValue, false);
        }

        void OnTweenScaleEnd(bool isKinematicAfter)
        {
            tweening = false;
            PhysicsRigUtils.ScaleEnd(transform, resetableRigidbodies, isKinematicAfter);
            onTweenScaleEnd.Invoke();
        }
    }
}