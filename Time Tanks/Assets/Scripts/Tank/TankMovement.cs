using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankMovement : MonoBehaviour
{
    Tank tank;
    public MovementControl currentMovement;
    public Rigidbody rb;

    // This are the maximum speeds the engine can reach
    // Keep turnSpeed pretty close to forwardSpeed otherwise turning while going forwards slows tank down
    public float forwardSpeed;
    public float reverseSpeed;
    public float turnSpeed;

    float engineSpeed = 0;
    public float engineAcceleration = 0.1f; // the forward/reverse/turnSpeed is multiplied by this number then added to engineSpeed
    public float engineDeceleration = 0.95f; // multiplies engineSpeed by this every update

    // Sideways friction changes when tank is at speed.
    public float velocityBeforeConsideringToBeTravelling;
    public float sidewaysStiffnessAtTravel;
    public float sidewaysStiffnessAtRest;

    public WheelCollider[] leftWheels;
    public WheelCollider[] rightWheels;

    // how much to boost by. multiplies the torque values. 0 = no boost
    float boostMultiplier = 1;

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

    // apply the maximum * acceleration to the engine speed, then cap it at maximum
    public void AccelerateEngine(float maximum)
    {
        engineSpeed = Mathf.Clamp(engineSpeed + (maximum * engineAcceleration), 0, maximum);
    }

    public void Update()
    {
        // Reset the motor speed from last update
        SetLeftTorque(engineSpeed);
        SetRightTorque(engineSpeed);


        if (currentMovement.forwards)
        {
            AccelerateEngine(forwardSpeed * boostMultiplier);

            // both treads at same speed to go forwards
            SetLeftTorque(engineSpeed);
            SetRightTorque(engineSpeed);
        }

        if (currentMovement.reverse)
        {
            AccelerateEngine(reverseSpeed * boostMultiplier);

            // and both on reverse for backwards
            SetLeftTorque(-reverseSpeed);
            SetRightTorque(-reverseSpeed);
        }

        int flipLeftAndRightBecauseOfReversing = currentMovement.reverse ? -1 : 1; // set to -1 when reversing

        if (currentMovement.left)
        {
            AccelerateEngine(turnSpeed * boostMultiplier);

            // right forwards, left backwards to turn left
            SetRightTorque(engineSpeed * flipLeftAndRightBecauseOfReversing);
            SetLeftTorque(-engineSpeed * flipLeftAndRightBecauseOfReversing);
        }

        if (currentMovement.right)
        {
            AccelerateEngine(turnSpeed * boostMultiplier);

            // left forwards, right backwards to turn right
            SetLeftTorque(engineSpeed * flipLeftAndRightBecauseOfReversing * boostMultiplier);
            SetRightTorque(-engineSpeed * flipLeftAndRightBecauseOfReversing * boostMultiplier);
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

        if (!this.AreWheelsPowered())
        {
            engineSpeed *= engineDeceleration; // decelerate the engine if there is no power running to it
            if (engineSpeed < 0.01f) engineSpeed = 0;
        }
    }

    // Multiply the torques by a certain amount for a certain amount of seconds
    public void Boost(float boostBy, float boostBySeconds)
    {
        // don't restart the coroutine if we're already boosting, just update the boost amount
        if (boostMultiplier == 1) StartCoroutine(StopBoostingAfter(boostBySeconds));

        boostMultiplier = boostBy;
    }

    public IEnumerator StopBoostingAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        boostMultiplier = 1;
    }

    // return the highest engine speed it can reach without boost
    public float GetMaximumEngineSpeed()
    {
        return Mathf.Max(new float[] { forwardSpeed, turnSpeed, reverseSpeed });
    }

    public float GetEngineSpeed()
    {
        return engineSpeed;
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    public float GetCurrentRPM()
    {
        // Return the averaged RPM across all the wheels, always positive

        float total = 0;
        foreach (WheelCollider wheel in leftWheels) total += Mathf.Abs(wheel.rpm);
        foreach (WheelCollider wheel in rightWheels) total += Mathf.Abs(wheel.rpm);

        return total / (leftWheels.Length + rightWheels.Length);
    }

    // returns true if any of the wheels have power
    public bool AreWheelsPowered()
    {
        return (currentMovement.forwards || currentMovement.reverse || currentMovement.left || currentMovement.right);
    }
}
