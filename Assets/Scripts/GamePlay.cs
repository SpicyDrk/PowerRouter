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
    
    [SerializeField] List<Scene> scenes; 
    
    private SoundManager _soundManager;
    private bool _wonLevel = false;
    private LevelLoader _levelLoader;

    private List<Transform> allPowerLineSegments = new();
    private Camera _camera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
        _levelLoader = GetComponent<LevelLoader>();
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
        //loop over this scene and get all objects named "PowerSwitch"
        foreach (var gameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (gameObject.name.Contains("PowerSwitch"))
            {
                powerPoleInstances.Add(gameObject);
            }
        }
        
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_wonLevel)
        {
            if (winningLight.intensity < 0.6)
            {
                winningLight.intensity += 0.02f;
            }
            if(allPowerLineSegments.Count == 0)
            {
                _levelLoader.LoadLevel();
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
        _levelLoader.LoadLevel();
    }
}


