using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    Tank tank;
    MovementControl currentMovement;

    public Rigidbody rb;


    public float forwardSpeed;
    public float reverseSpeed;
    public float turnSpeed;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        tank.OnTankMovementChanged += OnMovementChanged;
    }

    public void OnMovementChanged(MovementControl oldMovement, MovementControl newMovement)
    {
        currentMovement = newMovement;

        if (currentMovement.forwards)
        {
           rb.AddForce(transform.forward * forwardSpeed * Time.deltaTime, ForceMode.Acceleration);
        }

        if (currentMovement.reverse)
        {
            rb.AddForce(-transform.forward * reverseSpeed * Time.deltaTime, ForceMode.Acceleration);
        }

        if (currentMovement.right)
        {
            rb.AddRelativeTorque(Vector3.up * turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }

        if (currentMovement.left)
        {
            rb.AddRelativeTorque(Vector3.up * -turnSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    public void Update()
    { 

    }
}
