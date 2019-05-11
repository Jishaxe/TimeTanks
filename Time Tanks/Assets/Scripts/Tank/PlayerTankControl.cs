using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankControl : TankControl
{
    MovementControl prevMovement;

    private void Update()
    {
        MovementControl movement;

        movement.forwards = Input.GetKey(KeyCode.W);
        movement.reverse = Input.GetKey(KeyCode.S);
        movement.left = Input.GetKey(KeyCode.A);
        movement.right = Input.GetKey(KeyCode.D);

        // only send new movement if it's changed since last frame
        if (prevMovement.forwards != movement.forwards || prevMovement.right != movement.right || prevMovement.left != movement.left || prevMovement.reverse || movement.reverse)
        {
            tank.Move(movement);
            prevMovement = movement;
        }
    }
}


