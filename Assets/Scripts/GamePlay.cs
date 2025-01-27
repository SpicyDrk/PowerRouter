using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public List<GameObject> powerLineInstances = new();

    public List<GameObject> powerPoleInstances = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void AddPowerPoleInstance(GameObject powerPoleInstance)
    {
        powerPoleInstances.Add(powerPoleInstance);
    }
    public void RemovePowerPoleInstance(GameObject powerPoleInstance)
    {
        powerPoleInstances.Remove(powerPoleInstance);
    }
}


