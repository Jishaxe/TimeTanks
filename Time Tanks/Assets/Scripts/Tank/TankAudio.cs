using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAudio : MonoBehaviour
{
    public AudioClip[] enginePops;

    public float timeBetweenPops;
    public float rpmScaler;
    public Vector2 pitchMinMax;
    float timeSinceLastPop = 0;

    public AudioSource enginePopSource;

    TankMovement tankMovement;

    private void Awake()
    {
        tankMovement = GetComponent<TankMovement>();
    }

    public void Update()
    {
        timeSinceLastPop += Time.deltaTime;

        // if it's time for the next engine pop sound
        if (timeSinceLastPop > timeBetweenPops)
        {
            timeSinceLastPop = 0;

            enginePopSource.Stop();

            float rpm = tankMovement.GetCurrentRPM();
            Debug.Log(rpm);
            // clamp the rpm to pitch between a min and a max
            enginePopSource.pitch = Mathf.Clamp(rpm * rpmScaler, pitchMinMax.x, pitchMinMax.y);

            // pick a new random clip
            enginePopSource.clip = enginePops[Random.Range(0, enginePops.Length)];
            
            enginePopSource.Play();
        }
    }
}
