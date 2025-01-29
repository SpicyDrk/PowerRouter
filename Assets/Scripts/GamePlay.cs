using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    public List<GameObject> powerLineInstances = new();

    public List<GameObject> powerPoleInstances = new();

    public GameObject powerStart;
    public GameObject powerEnd;
    [SerializeField] private Light2D winningLight;
    
    public List<Scene> scenes; 
    
    private SoundManager _soundManager;
    private bool _wonLevel = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winningLight.intensity = 0;
        if(powerStart == null || powerEnd == null)
        {
            Debug.LogError("Power start or end not set");
        }
        else
        {
            powerPoleInstances.Add(powerStart);
            powerPoleInstances.Add(powerEnd);
        }
        _soundManager = SoundManager.instance;
        if (_soundManager == null)
        {
            Debug.LogError("No SoundManager found in scene");
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_wonLevel)
        {
            if (winningLight.intensity < 1)
            {
                winningLight.intensity += 0.01f;
            }
            else
            {
                //var currentScene = SceneManager.GetActiveScene();
                //var currentSceneIndex = scenes.IndexOf(currentScene);
                //if (currentSceneIndex == scenes.Count - 1)
                //{
                //   // SceneManager.LoadScene(scenes[0].name);
                //}
                //else
                //{
                //    //SceneManager.LoadScene(scenes[currentSceneIndex + 1].name);
                //}
            }
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
        List<PowerPole> powerPoleHasPower = new();
        foreach (var pole in powerPoleInstances)
        {
            powerPoleHasPower.Add(pole.GetComponent<PowerPole>());
        }
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

        if (powerEnd.GetComponent<PowerPole>().hasPower)
        {
            LevelComplete();
        }
    }
    
    public void RemovePowerPoleInstance(GameObject powerPoleInstance)
    {
        var ppInstance = powerPoleInstance.GetComponent<PowerPole>();
        if (ppInstance.isStart)
        {
            return;
        }
        if(ppInstance.powerLineIn != null)
        {
            powerLineInstances.Remove(ppInstance.powerLineIn);
            Destroy(ppInstance.powerLineIn);
        }
        if(ppInstance.powerLineOut != null)
        {
            powerLineInstances.Remove(ppInstance.powerLineOut);
            Destroy(ppInstance.powerLineOut);
        }
        powerPoleInstances.Remove(powerPoleInstance);
        Destroy(powerPoleInstance);
        Invoke(nameof(CalculatePower),0.1f);
    }
    
    private void LevelComplete()
    {
        _wonLevel = true;
        _soundManager.PlaySound("Win");
        
    }
}


