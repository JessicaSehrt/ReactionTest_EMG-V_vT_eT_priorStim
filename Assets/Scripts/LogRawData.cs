using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEditor;

public class LogRawData : MonoBehaviour
{
	public GameObject startPanel;
    [SerializeField] LightChanger _lightChanger;

    string filename;
    string filename2;
    string filename3;
    string filename4;

    private string condition;
    private long systemTime;
    private string rawDataSample;
    private int userID;
    void Start()
    {
        systemTime = 0;
		Directory.CreateDirectory(Application.dataPath + "/_RawData/");
		filename = _lightChanger.subjectID + "_RawData" + "_" + System.DateTime.Now.Ticks;
        Directory.CreateDirectory(Application.dataPath + "/_TestArray/");
        filename2 = _lightChanger.subjectID + "_TestArray" + "_" + System.DateTime.Now.Ticks;
        Directory.CreateDirectory(Application.dataPath + "/_FrameBasedData/");
        filename3 = _lightChanger.subjectID + "_FrameBasedData" + "_" + System.DateTime.Now.Ticks;
        Directory.CreateDirectory(Application.dataPath + "/_FrameBasedData/");
        filename4 = _lightChanger.subjectID + "_RawDataAlot" + "_" + System.DateTime.Now.Ticks;
    }
    private void Update()
    {
        if (_lightChanger.startStudy)
        {
            if (_lightChanger.startTimerTime < 0)
            {
                if (systemTime != System.DateTime.Now.Ticks)
                {
                    string path = (Application.dataPath + "/_FrameBasedData/" + filename3 + ".csv");
                    //Time;Ch1;Ch2;Ch3;Ch4;ID;condition;lightSwitch ch1 = biceps ch2 = tri 
                    File.AppendAllText(path, System.DateTime.Now.Ticks + ";" + rawDataSample.ToString() + _lightChanger.subjectID + ";" + condition + ";" + _lightChanger.lightActiveText + "\n");
                }
                systemTime = System.DateTime.Now.Ticks;
            }
        }
    }
    public void FirstCondition(string muscle, string feedback, int timings, int repetition, int subjectID)
    {
        if (muscle == "upper leg")
        {
            muscle = "upperleg";
        }
        userID = subjectID;
        condition = muscle + "-" + feedback + "-" + timings + "-" + repetition;
    }
    public void CreateCSVRawData(String rawData)
	{
        rawDataSample = rawData;
        if (_lightChanger.startStudy)
        {
            if (_lightChanger.startTimerTime < 0)
            {
                string path = (Application.dataPath + "/_RawData/" + filename + ".csv");
                //Time;Ch1;Ch2;Ch3;Ch4;ID;condition;lightSwitch ch1 = biceps ch2 = tri 
                File.AppendAllText(path, System.DateTime.Now.Ticks + ";" + rawDataSample.ToString() + _lightChanger.subjectID + ";" + condition + ";" + _lightChanger.lightActiveText + "\n");
                systemTime = System.DateTime.Now.Ticks;
            }
        }
        
	}
    public void CreateCSVTestArrary(string muscle, string feedback, int timings, int repetition, int index, string light, int duration, int restDuration, string muscleGroup1, string muscleGroup2, string muscleGroup3, string muscleGroup4)
    {
        if (_lightChanger.startStudy)
        {
            if (muscle == "upper leg")
            {
                muscle = "upperleg";
            }
            condition = muscle + "-" + feedback + "-" + timings + "-" + repetition;
            //Debug.Log("arrayValues " + condition);
            string path2 = (Application.dataPath + "/_TestArray/" + filename2 + ".csv");
            File.AppendAllText(path2, System.DateTime.Now.Ticks + ";" + _lightChanger.subjectID + ";" + condition + ";" + index + ";" + light + ";" + duration + ";" + muscle + ";" + restDuration + ";" + muscleGroup1 + ";" + muscleGroup2 + ";" + muscleGroup3 + ";" + muscleGroup4 + "\n");
        }
    }
}
