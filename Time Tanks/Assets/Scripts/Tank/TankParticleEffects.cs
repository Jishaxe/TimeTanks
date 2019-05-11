using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankParticleEffects : MonoBehaviour
{
    TankMovement tankMovement;

    public Animator[] smokeStacks;

    // speed the tank should be travelling at before playing the "smoke moving" animation
    public float speedBeforeMovingSmoke;

    public void Awake()
    {
        tankMovement = GetComponent<TankMovement>();
    }

    public void FixedUpdate()
    {
        float tankSpeed = tankMovement.GetCurrentSpeed();

        if (tankSpeed < speedBeforeMovingSmoke)
        {
            if (tankMovement.AreWheelsPowered()) SmokeStacksToAccelerating(); // we're accelerating
            else SmokeStacksToIdle(); // we're idling
        } else
        {
            SmokeStacksToMoving(); // we're at cruise speed
        }
    }


    public void SmokeStacksToIdle()
    {
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("idle");
        }
    }

    public void SmokeStacksToAccelerating()
    {
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("accelerating");
        }
    }

    public void SmokeStacksToMoving()
    {
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("moving");
        }
    }
}
