using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls smoke and tread FX
public class TankFX : MonoBehaviour
{
    TankMovement tankMovement;

    [Header("Smoke FX")]
    public Animator[] smokeStacks;

    [Header("Tread FX")]
    public MeshRenderer treadRenderer;

    // speed the treads scroll at
    public float treadScrollSpeed;
    float treadOffset;

    [Header("Backfiring FX")]
    public float backfireProbability; // chance of backfiring 0 .. 1, 1 being 100% chance
    public float backfireProbabilityWhenAccelerating;
    public Vector2 backfireFor; // how long backfiring should last. x: min, y: max
    public float backfireBoostMultiplier; // how much to boost the tank when backfiring
    bool isBackfiring = false;

    [Header("FoV with speed FX")]
    public Camera cameraToApplyFXTo; // camera to apply FOV FX to, leave undefined if you don't want this to happen
    public float maxFovIncrease; // the maximum FoV increase
    public float fovMultiplier; // multiplier for scaling the fov FX with speed
    float normalCameraFov; // the camera FoV with no increase applied

    [Header("Thresholds")]
    // speed the tank should be travelling at before playing the "smoke moving" animation
    public float cruiseSpeed;

    public void Awake()
    {
        tankMovement = GetComponent<TankMovement>();
        if (cameraToApplyFXTo != null) normalCameraFov = cameraToApplyFXTo.fieldOfView;
    }

    public void FixedUpdate()
    {
        float tankSpeed = tankMovement.GetCurrentSpeed();

        if (tankSpeed < cruiseSpeed)
        {
            if (tankMovement.AreWheelsPowered())
            {
                // test to see if we will backfire
                WillBackfire(backfireProbabilityWhenAccelerating); // more chance to backfire when accelerating
                SmokeStacksToAccelerating(); // we're accelerating
            }
            else
            {
                SmokeStacksToIdle(); // we're idling
            }
        } else
        {
            WillBackfire(backfireProbability);
            SmokeStacksToMoving(); // we're at cruise speed
        }

        AnimateTreads(tankSpeed);
        if (cameraToApplyFXTo != null) AnimateFOV(tankSpeed);
    }

    void AnimateFOV(float tankSpeed)
    {
        float fovIncrease = Mathf.Clamp(tankSpeed * fovMultiplier, 0, maxFovIncrease);
        cameraToApplyFXTo.fieldOfView = normalCameraFov + fovIncrease;
    }

    void AnimateTreads(float tankSpeed)
    {
        float treadOffsetAmount = treadScrollSpeed * -tankSpeed;

        // if we're reversing then reverse the offset
        if (tankMovement.currentMovement.reverse) treadOffsetAmount = -treadOffsetAmount;
        treadOffset += treadOffsetAmount;

        // keep offset betweeen 0 and 1
        treadOffset = Mathf.Repeat(treadOffset, 1);

        // offset the materials for the tank treads to simulate moving 
        treadRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, treadOffset));
    }

    public void WillBackfire(float probability)
    {
        // only check every 10 frames
        if (Time.frameCount % 10 == 0)
        {
            if (Random.Range(0f, 1f) < probability) Backfire();
        }
    }

    public void Backfire()
    {
        if (isBackfiring) return;

        foreach (Animator stack in smokeStacks) stack.Play("backfire");
        isBackfiring = true;
        float backfireDuration = Random.Range(backfireFor.x, backfireFor.y);
        tankMovement.Boost(backfireBoostMultiplier, backfireDuration);
        StartCoroutine(StopBackfiringAfter(backfireDuration));
    }

    public IEnumerator StopBackfiringAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isBackfiring = false;
    }

    public void SmokeStacksToIdle()
    {
        if (isBackfiring) return;
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("idle");
        }
    }

    public void SmokeStacksToAccelerating()
    {
        if (isBackfiring) return;
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("accelerating");
        }
    }

    public void SmokeStacksToMoving()
    {
        if (isBackfiring) return;
        foreach (Animator stack in smokeStacks)
        {
            stack.Play("moving");
        }
    }
}
