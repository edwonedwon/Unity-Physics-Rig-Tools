using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct ConfigurableJointSettings
{
    [Header("Connected To")]
    public Rigidbody connected;
    public bool useConnectedMassScale;
    public float connectedMassScale;
    [Header("Rigidbody")]
    public bool rbGravity;
    public float rbMass;
    public float rbDrag;
    public float rbAngularDrag;
    public RigidbodyInterpolation interpolation;
    [Header("Motion")]
    public bool useMotionSettings;
    public ConfigurableJointMotion positionMotion;
    public ConfigurableJointMotion rotationMotion;
    [Header("Joint Position Spring")]
    public float cjPosSpring;
    public float cjPosSpringDamp;
    [Header("Joint Rotation Spring")]
    public RotationDriveMode cjRotDriveMode;
    public float cjRotSpring;
    public float cjRotSpringDamp;
    [Header("Max Forces")]
    public float cjPosSpringMaxForce;
    public float cjRotSpringMaxForce;
}

[RequireComponent(typeof(Rigidbody), typeof(ConfigurableJoint), typeof(ResetableRigidbody))]
public class ConfigurableJointControl : MonoBehaviour
{
    [InspectorButton("ApplyConnected")]
    public bool applyConnected;
    public void ApplyConnected()
    {
        InitComponents();
        cj.connectedBody = settings.connected;
    }

    Rigidbody rb;
    ConfigurableJoint cj;

    public ConfigurableJointSettings settings;
    ConfigurableJointSettings settingsLast;
    JointDrive cjPosSpringDrive;
    JointDrive cjRotSpringDrive;

    void Awake()
    {
        InitComponents();

        cjPosSpringDrive = new JointDrive();

        settingsLast = settings;
        ApplySettings();
    }

    void InitComponents()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (cj == null)
            cj = GetComponent<ConfigurableJoint>();
    }

    void FixedUpdate()
    {
        if (!settings.Equals(settingsLast))
            ApplySettings();

        settingsLast = settings;
    }

    void ApplySettings()
    {
        if (settings.connected != null)
            cj.connectedBody = settings.connected;
        cj.connectedMassScale = settings.connectedMassScale;

        // rigidbody settings
        rb.useGravity = settings.rbGravity;
        rb.mass = settings.rbMass;
        rb.drag = settings.rbDrag;
        rb.angularDrag = settings.rbAngularDrag;
        rb.interpolation = settings.interpolation;

        // motion settings
        if (settings.useMotionSettings)
        {
            cj.xMotion = settings.positionMotion;
            cj.yMotion = settings.positionMotion;
            cj.zMotion = settings.positionMotion;
            cj.angularXMotion = settings.rotationMotion;
            cj.angularYMotion = settings.rotationMotion;
            cj.angularZMotion = settings.rotationMotion;
        }

        // position spring
        cjPosSpringDrive.positionSpring = settings.cjPosSpring;
        cjPosSpringDrive.positionDamper = settings.cjPosSpringDamp;
        cjPosSpringDrive.maximumForce = settings.cjPosSpringMaxForce;
        // apply position spring
        cj.xDrive = cjPosSpringDrive;
        cj.yDrive = cjPosSpringDrive;
        cj.zDrive = cjPosSpringDrive;

        // rotation spring
        cj.rotationDriveMode = settings.cjRotDriveMode;
        cjRotSpringDrive.positionSpring = settings.cjRotSpring;
        cjRotSpringDrive.positionDamper = settings.cjRotSpringDamp;
        cjRotSpringDrive.maximumForce = settings.cjRotSpringMaxForce;
        // apply rotation spring
        cj.slerpDrive = cjRotSpringDrive;
        cj.angularXDrive = cjRotSpringDrive;
        cj.angularYZDrive = cjRotSpringDrive;
    }

    [InspectorButton("ApplyDefaultSettings")]
    public bool applyDefaultSettings;
    public void ApplyDefaultSettings()
    {
        settings.useConnectedMassScale = false;
        settings.connectedMassScale = 1;

        // rigidbody settings
        settings.rbGravity = true;
        settings.rbMass = 1;
        settings.rbDrag = 5;
        settings.rbAngularDrag = 0.5f;
        settings.interpolation = RigidbodyInterpolation.Interpolate;

        // motion settings
        settings.useMotionSettings = true;
        settings.positionMotion = ConfigurableJointMotion.Free;
        settings.rotationMotion = ConfigurableJointMotion.Free;

        // position spring
        settings.cjPosSpring = 500;
        settings.cjPosSpringDamp = 5;
        settings.cjPosSpringMaxForce = 100000;

        // rotation spring
        settings.cjRotDriveMode = RotationDriveMode.Slerp;
        settings.cjRotSpring = 2;
        settings.cjRotSpringDamp = 0;
        settings.cjRotSpringMaxForce = 100000;
    }
}