using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents state of movement
public struct MovementControl
{
    public bool forwards;
    public bool reverse;
    public bool left;
    public bool right;

    public override string ToString()
    {
        string res = "Movement: ";
        if (forwards) res += "forwards ";
        if (reverse) res += "reverse ";
        if (left) res += "left ";
        if (right) res += "right ";

        return res;
    }
}

public class Tank : MonoBehaviour
{
    // current movement
    public MovementControl movement;

    public delegate void TankMoveEvent(MovementControl previousMovement, MovementControl movement);
    public event TankMoveEvent OnTankMovementChanged;

    // Instruct the tank to start moving in a different way
    public void Move(MovementControl newMovement)
    {
        // Trigger the tank move event
        OnTankMovementChanged?.Invoke(this.movement, newMovement);
        this.movement = newMovement;
    }
}

