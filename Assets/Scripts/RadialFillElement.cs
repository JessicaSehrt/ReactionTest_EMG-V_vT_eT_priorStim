using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class RadialFillElement : MonoBehaviour
{
    [SerializeField] private Image radialIndicatorUI = null;
    [SerializeField] public LightChanger _lightChanger;

    public GameObject muscleStrength;

    private int channel1;
    private int channel2;
    private int channel3;
    private int channel4;

    private float setUICh1;
    private float setUICh2;
    private float setUICh3;
    private float setUICh4;

    private void Update()
    {
        if (_lightChanger.startStudy)
        {
            if (_lightChanger.lightActive)
            {
                radialIndicatorUI.enabled = true;
                muscleStrength.SetActive(true);
                switch (_lightChanger.muscle)
                {
                    case "biceps":
                        radialIndicatorUI.fillAmount = setUICh1;
                        break;
                    case "triceps":
                        radialIndicatorUI.fillAmount = setUICh2;
                        break;
                    case "upper leg":
                        radialIndicatorUI.fillAmount = setUICh3;
                        break;
                    case "calf":
                        radialIndicatorUI.fillAmount = setUICh4;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                radialIndicatorUI.enabled = false;
                muscleStrength.SetActive(false);
            }
        }
        else
        {
            radialIndicatorUI.enabled = false;
            muscleStrength.SetActive(false);
        }
    }

    public void setData(String onDataRecieved)
    {
        splitConditions(onDataRecieved);
        setUICh1 = Range(channel1, 15000, 65535);
        setUICh2 = Range(channel2, 15000, 65535);
        setUICh3 = Range(channel3, 15000, 65535);
        setUICh4 = Range(channel4, 15000, 65535); 
    }
    private void splitConditions(String onDataRecieved)
    {
        string[] conditionList = onDataRecieved.Split(";");
        channel1 = int.Parse(conditionList[0]);
        channel2 = int.Parse(conditionList[1]);
        channel3 = int.Parse(conditionList[2]);
        channel4 = int.Parse(conditionList[3]);
    }

    public float Range(float val, int min, int max)
    {
        return (val - min) / (max - min);
    }
        
}
