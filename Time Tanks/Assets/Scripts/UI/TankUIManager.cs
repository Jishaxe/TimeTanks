using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUIManager : MonoBehaviour
{
    public delegate void TargetTankChangedEvent(GameObject newTank);
    public event TargetTankChangedEvent OnTargetTankChanged;
    public GameObject tank;

    public void ChangeTargetTank(GameObject tank)
    {
        OnTargetTankChanged(tank);
    }

    public void Start()
    {
        if (tank != null) OnTargetTankChanged?.Invoke(tank);
    }
}
