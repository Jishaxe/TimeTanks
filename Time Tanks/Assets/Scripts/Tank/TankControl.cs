using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    protected Tank tank;

    private void Awake()
    {
        tank = GetComponent<Tank>();
    }
}
