﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Edwon.PhysicsRigTools
{
    public class RigScaler : MonoBehaviour
    {
        public bool setScaleOnAwake;
        public float scaleOnAwake = 1;
        ResetableRigidbody[] resetableRigidbodies;
        Joint[] joints;
        public float tweenDuration = 1f;
        public Ease tweenEase;
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
                SetScale(scaleOnAwake);
        }

        public void SetScale(float toScale)
        {
            OnScaleStart();
            OnScaleUpdate(toScale);
            PhysicsRigUtils.ScaleEnd(transform, resetableRigidbodies);
        }

        public void SetScaleWhileKinematic(float toScale)
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
            TweenScaleTo(debugTweenScaleTo);
        }

        public void TweenScaleTo(float toScale)
        {
            tweening = true;
            OnScaleStart();
            float currentScale = transform.lossyScale.x;
            DOTween.To(OnScaleUpdate, currentScale, toScale, tweenDuration)
                .OnComplete(OnTweenScaleEnd)
                .SetEase(tweenEase);
        }

        public void SetTweenScaleToSetValue(float value)
        {
            tweenScaleToSetValue = value;
        }

        public void TweenScaleToSetValue()
        {
            TweenScaleTo(tweenScaleToSetValue);
        }

        void OnTweenScaleEnd()
        {
            tweening = false;
            PhysicsRigUtils.ScaleEnd(transform, resetableRigidbodies);
            onTweenScaleEnd.Invoke();
        }
    }
}