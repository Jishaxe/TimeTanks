using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineSpeedUI : MonoBehaviour
{
    public TankUIManager tankUIManager;
    public Image bar;

    TankMovement tankMovement;
    float maxSpeed;

    public void Awake()
    {
        tankUIManager.OnTargetTankChanged += Init;
    }

    public void Init (GameObject newTank)
    {
        tankMovement = newTank.GetComponent<TankMovement>();
        maxSpeed = tankMovement.GetMaximumEngineSpeed();
    }

    public void Update()
    {
        bar.fillAmount = (tankMovement.GetEngineSpeed() / maxSpeed) * 0.75f;
    }
}
