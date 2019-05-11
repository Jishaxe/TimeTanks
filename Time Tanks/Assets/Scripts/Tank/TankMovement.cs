using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    Tank tank;
    public MovementControl currentMovement;

    public Rigidbody rb;

    // Keep turnSpeed pretty close to forwardSpeed otherwise turning while going forwards slows tank down
    public float forwardSpeed;
    public float reverseSpeed;
    public float brakeTorque;
    public float turnSpeed;

    // Sideways friction changes when tank is at speed.
    public float velocityBeforeConsideringToBeTravelling;
    public float sidewaysStiffnessAtTravel;
    public float sidewaysStiffnessAtRest;

    public WheelCollider[] leftWheels;
    public WheelCollider[] rightWheels;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        tank.OnTankMovementChanged += OnMovementChanged;
    }

    public void OnMovementChanged(MovementControl oldMovement, MovementControl newMovement)
    {
        // this is then acted on in update
        currentMovement = newMovement;
    }

    // Set the torque for the whole left row of wheels
    public void SetLeftTorque(float torque)
    {
        foreach (WheelCollider collider in leftWheels) collider.motorTorque = torque;
    }

    // Set the torque for whole right row of wheels
    public void SetRightTorque(float torque)
    {
        foreach (WheelCollider collider in rightWheels) collider.motorTorque = torque;
    }

    // update sideways friction stiffness for all wheels
    public void SetSidewaysStiffness(float newStiffness)
    {
        foreach (WheelCollider collider in leftWheels)
        {
            WheelFrictionCurve friction = collider.sidewaysFriction;
            friction.stiffness = newStiffness;
            collider.sidewaysFriction = friction;
        }

        foreach (WheelCollider collider in rightWheels)
        {
            WheelFrictionCurve friction = collider.sidewaysFriction;
            friction.stiffness = newStiffness;
            collider.sidewaysFriction = friction;
        }
    }

    public void Update()
    {
        // Reset the motor speed from last update
        SetLeftTorque(0);
        SetRightTorque(0);


        if (currentMovement.forwards)
        {
            // both treads at same speed to go forwards
            SetLeftTorque(forwardSpeed);
            SetRightTorque(forwardSpeed);
        }

        if (currentMovement.reverse)
        {
            // and both on reverse for backwards
            SetLeftTorque(-reverseSpeed);
            SetRightTorque(-reverseSpeed);
        }

        if (currentMovement.left)
        {
            // right forwards, left backwards to turn left
            SetRightTorque(turnSpeed);
            SetLeftTorque(-turnSpeed);
        }

        if (currentMovement.right)
        {
            // left forwards, right backwards to turn right
            SetLeftTorque(turnSpeed);
            SetRightTorque(-turnSpeed);
        }


        // We adjust the sideways friction of the wheels depending on whether the tank is being moved or not.
        // This is important because we can reduce it when the tank is at rest and that allows it to swivel in place
        // but when it is travelling at speed we need to increase that friction so the tank doesn't spin out
        if (rb.velocity.magnitude <= velocityBeforeConsideringToBeTravelling)
        {
            SetSidewaysStiffness(sidewaysStiffnessAtRest);
        } else
        {
            SetSidewaysStiffness(sidewaysStiffnessAtTravel);
        }
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    // returns true if any of the wheels have power
    public bool AreWheelsPowered()
    {
        return (currentMovement.forwards || currentMovement.reverse || currentMovement.left || currentMovement.right);
    }
}
