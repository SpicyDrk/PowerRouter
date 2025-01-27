using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public List<GameObject> powerLineInstances = new();

    public List<GameObject> powerPoleInstances = new();

    public GameObject powerStart;
    public GameObject powerEnd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(powerStart == null || powerEnd == null)
        {
            Debug.LogError("Power start or end not set");
        }
        else
        {
            powerPoleInstances.Add(powerStart);
            powerPoleInstances.Add(powerEnd);
        }
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void CalculatePower()
    {
        foreach (var powerPoleGo in powerPoleInstances)
        {
            powerPoleGo.GetComponent<PowerPole>().hasPower = false;
        }

        foreach (var powerLineGo in powerLineInstances)
        {
            powerLineGo.GetComponent<RopeCreator>().hasPower = false;
        }
        powerStart.GetComponent<PowerPole>().hasPower = true;
        var currentPowerPole = powerStart;
        var currentPowerLine = powerStart.GetComponent<PowerPole>()?.powerLineOut;
        var parsingPowerPole = true; 
        var powerLineBroken = false;
        var maxDepth = 100;
        while (!powerLineBroken  && maxDepth > 0)
        {
            maxDepth--;
            if (parsingPowerPole)
            {
                var powerPole = currentPowerPole.GetComponent<PowerPole>();
                currentPowerLine = powerPole?.powerLineOut;
                powerPole.hasPower = true;
                if (currentPowerLine == null)
                {
                    powerLineBroken = true;
                }
                else
                {
                    powerPole.hasPower = true; 
                }
            }
            else
            {
                var powerLine = currentPowerLine.GetComponent<RopeCreator>();
                currentPowerPole = powerLine?.powerPoleIn;
                if (currentPowerPole == null)
                {
                    powerLineBroken = true;
                }
                else
                {
                    powerLine.hasPower = true;
                }
            }
            parsingPowerPole = !parsingPowerPole;
        }
    }
    
    public void RemovePowerPoleInstance(GameObject powerPoleInstance)
    {
        powerPoleInstances.Remove(powerPoleInstance);
    }
}


