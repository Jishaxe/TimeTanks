using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    Tank tank;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        tank.OnTankMovementChanged += OnMovementChanged;
    }

    public void OnMovementChanged(MovementControl oldMovement, MovementControl newMovement)
    {
        Debug.Log(newMovement);
    }
}
