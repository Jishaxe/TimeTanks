using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankControl : TankControl
{
    private void Update()
    {
        MovementControl movement;

        movement.forwards = Input.GetKey(KeyCode.W);
        movement.reverse = Input.GetKey(KeyCode.S);
        movement.left = Input.GetKey(KeyCode.A);
        movement.right = Input.GetKey(KeyCode.D);

        tank.Move(movement);
    }
}


