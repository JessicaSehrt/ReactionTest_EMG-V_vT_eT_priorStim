//activate Traffic Light Complex - Object in Unity
//#define COMPLEX_TRAFFIC_LIGHT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.CodeDom;
using System.Security.Policy;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LightChanger : MonoBehaviour
{
    [Header("Object Settings")]
    [SerializeField] LogRawData _logRawData;
    [SerializeField] FeedbackManager _feedbackManager;
    [SerializeField] DecideSide _decideSide;

    [Header("Participent Settings")]
    public int subjectID = 3;
    public string muscle;
    public bool startPicture = false;

    [Header("Light Settings")]
    public Material green;
    public Material black;
    public Material red;
    public Material yellow;
    [Header("Traffic Light Simple Settings")]
    public GameObject greenLightSimple;
    public GameObject redLightSimple;
    public GameObject lightSourceGreenSimple;
    public GameObject lightSourceRedSimple;
    public TextMeshProUGUI textField;
    [Header("Traffic Light Complex Settings")]
    public GameObject greenLightComplex;
    public GameObject redLightComplex;
    public GameObject yellowLightComplex;
    public GameObject lightSourceGreenComplex;
    public GameObject lightSourceRedComplex;
    public GameObject lightSourceYellowComplex;
    public TextMeshProUGUI textFieldComplex;
    [Header("Time Settings")]
    public double startTimerTime = 10f;
    public double restTime = 2f;
    public double feedbackTimer = 3f;
    public double intervallTimer = 21f;
    public double greenLightTimer = 10f;
    public double durationOfTest;
    [Header("Visuals")]
    public GameObject UI;
    public GameObject wall;
    public GameObject text;
    public GameObject EMGInterface;
    public GameObject EMGMainPanel;

    //Default values
    public string[] muscles = new string[] { "biceps", "triceps", "calf", "upper leg" };
    public string[] feedbacks = new string[] { "EMS", "Vibration", "nothing", "Visual" };
    public string[] timings = new string[] { "6", "9", "12", "15", "19", "21" };
    public string[] repetitions = new string[] { "1", "2" };
    public string[] conditions;

    public string side;
    public string lightActiveText = "off";

    public bool lightActive = false;
    public bool visualsActive = false;
    public bool studyRunning = true;
    public string levelName;
    public bool startStudy = false;
    public bool showStudy = false;

    private double feebackT;
    private double greenLightT;
    private double durationOfTestTimer;

    private bool startTimer = false;
    private bool checkIfFeedbackIsInUse = true;

   // private bool muscleIsInUse = true;
    private bool isRandomTimeactive = true;
    private List<String> muscleTypeList = new List<String>();
    private int setMuscle = 0;
    private int numberOfList = 0;
    private float randomTime = 0f;
    private double restingTime;
    private double restStudyTime;
    // private string muscle = string.Empty;
    private string feedback;
    private int timeing;
    private int repetition;
    private int indexOfCondition = 0;
    private int counter = 0;
    private int setLightOff = 1;
    private bool setStudy = true;
    private bool isStudyStillRunning = true;
    private bool isLightAlreadyOn = true;
    private bool isFeedbackUsed = false;
    private bool yellowWasActive = true;
    private bool firstRun = true;

    private AudioSource _source;

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        int index = muscles.Length * feedbacks.Length * timings.Length * repetitions.Length;
        
        conditions = new String [index];
        for (int i = 0; i < muscles.Length; i++)
        {
            for (int x = 0; x < feedbacks.Length; x++)
            {
                for (int y = 0; y < timings.Length; y++)
                {
                    for (int e = 0; e < repetitions.Length; e++) 
                    {
                        conditions[counter] = muscles[i] + "-" + feedbacks[x] + "-" + timings[y] + "-" + repetitions[e];
                        counter++;
                    }
                }
            }
        }
        lightActiveText = "off";
        //resttime = 2, intervallTimer = 21, greenLightTimer = 10, feedbackTimer = 3, durationOfTest = 36
        durationOfTest = (int) (restTime + intervallTimer + greenLightTimer + feedbackTimer);
        Debug.Log(durationOfTest + "sec of test");
        restStudyTime = durationOfTest;
        restingTime = restTime;
        conditions = NCDShuffle(conditions, subjectID);
        counter = 0;    
        splitConditions(counter);
        _logRawData.FirstCondition(muscle, feedback, timeing, repetition, subjectID);
        setTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (startStudy)
        {
            TimerLogic();
        }
        if (showStudy)
        {
            SimpleTimerLogic();
        }
    }

    private void splitConditions(int i)
    {
        string[] conditionList = conditions[i].Split("-");
        muscle = conditionList[0];
        feedback = conditionList[1];
        timeing = int.Parse(conditionList[2]);
        repetition = int.Parse(conditionList[3]);
    }
    //Manages the times
    public void TimerLogic()
    {
        //checks if the timer is active, if it isn't active it starts the countdown
        if (startTimer)
        {
            textField.text = "Study starts in <br>" + startTimerTime.ToString("0");
            textFieldComplex.text = "Study starts in <br>" + startTimerTime.ToString("0");
            startTimerTime -= Time.deltaTime;
        }
        //after the countdown reaches 0 the test starts

        if (startTimerTime < 0)
        {
            if (counter >= conditions.Length)
            {
                textField.text = "End of Study";
                studyRunning = false;
                //show questionare
            }
            else
            {
                restStudyTime -= Time.deltaTime;
                restTime -= Time.deltaTime;
                if (_decideSide.rightSide)
                {
                    if (lightActive)
                    {
                        textField.text = "Tense your right <color=#2FC92F><b>" + muscle + "</b></color>!";
                        textFieldComplex.text = "Tense your right <color=#2FC92F><b>" + muscle + "</b></color>!";
                    }
                    else
                    {
                        textField.text = "Please wait!";
                    }
                }
                if (_decideSide.leftSide)
                {
                    if (lightActive)
                    {
                        textField.text = "Tense your left <color=#2FC92F><b>" + muscle + "</b></color>!";
                        textFieldComplex.text = "Tense your left <color=#2FC92F><b>" + muscle + "</b></color>!";
                    }
                    else
                    {
                        textField.text = "Please wait!";
                        textFieldComplex.text = "Please wait!";
                    }
                }

                startTimer = false;
                if (firstRun)
                {
                    startPicture = true;
                    firstRun = false;
                }
                if (restTime < 0)
                {
                    if (!isFeedbackUsed)
                    {
#if COMPLEX_TRAFFIC_LIGHT
                        yellowLightComplex.GetComponent<MeshRenderer>().material = yellow;
                        redLightComplex.GetComponent<MeshRenderer>().material = black;
                        lightSourceYellowComplex.SetActive(true);
                        lightSourceRedComplex.SetActive(false);
#endif
                        //Check feedback to set true
                        switch (feedback)
                        {
                            case "EMS":
                                _feedbackManager.isTactileFeedbackOn = false;
                                _feedbackManager.isEmsFeedbackOn = true;
                                visualsActive = false;
                                break;
                            case "Vibration":
                                _feedbackManager.isTactileFeedbackOn = true;
                                _feedbackManager.isEmsFeedbackOn = false;
                                visualsActive = false;
                                break;
                            case "nothing":
                                _feedbackManager.isTactileFeedbackOn = false;
                                _feedbackManager.isEmsFeedbackOn = false;
                                visualsActive = false;
                                break;
                            case "Visual":
                                visualsActive = true;
                                break;
                        }
                        _feedbackManager.UpdateFeedback();
                        isFeedbackUsed = true;
                    }
                    if (checkIfFeedbackIsInUse)
                    {
                        feedbackTimer -= Time.deltaTime;
                    }
                    if (feedbackTimer < 0)
                    {
#if COMPLEX_TRAFFIC_LIGHT
                        if (yellowWasActive)
                        {
                            yellowLightComplex.GetComponent<MeshRenderer>().material = black;
                            redLightComplex.GetComponent<MeshRenderer>().material = red;
                            lightSourceYellowComplex.SetActive(false);
                            lightSourceRedComplex.SetActive(true); yellowWasActive = false;
                        }
#endif
                        checkIfFeedbackIsInUse = false;
                        if (isRandomTimeactive)
                        {
                            randomTime = timeing;
                            isRandomTimeactive = false;
                        }
                        //Debug.Log("test after feedbacktimer " + randomTime.ToString("F"));
                        randomTime -= Time.deltaTime;
                        if (randomTime < 0)
                        {
                            greenLightTimer -= Time.deltaTime;
                            
                            if (restStudyTime < 0)
                            {
                                GreenLightOff();
                                setStudy = true;
                                resetValues();
                                counter++;
                                if (counter < conditions.Length)
                                {
                                    splitConditions(indexOfCondition += 1);
                                }
                            }
                            else if(setStudy && isLightAlreadyOn)
                            {
                                GreenLightOn();
                                isLightAlreadyOn = false;
                            }
                            
                            if (greenLightTimer < 0 && setLightOff == 0 && setStudy)
                            {
                                Debug.Log("muscle: " + muscle + " feedback: " + feedback + " time: " + timeing + " repetition: " + repetition);
                                GreenLightOff();
                            }
                            else if (setLightOff == 1)
                            {
                                setLightOff = 0;
                            }
                        }
                    }
                }
            }
        }
    }
    public void SimpleTimerLogic()
    {
        //checks if the timer is active, if it isn't active it starts the countdown
        if (startTimer)
        {
            textField.text = "Study starts in <br>" + startTimerTime.ToString("0");
            textFieldComplex.text = "Study starts in <br>" + startTimerTime.ToString("0");
            startTimerTime -= Time.deltaTime;
        }
        //after the countdown reaches 0 the test starts
        if (startTimerTime < 0)
        {
            if (counter >= 2)
            {
                textField.text = "End of Test";
                BackToMenu();
            }
            else
            {
                restStudyTime -= Time.deltaTime;
                restTime -= Time.deltaTime;
                if (_decideSide.rightSide)
                {
                    if (lightActive)
                    {
                        textField.text = "Tense your right ...";
                        textFieldComplex.text = "Tense your right ...";
                    }
                    else
                    {
                        textField.text = "Please wait!";
                    }
                }
                if (_decideSide.leftSide)
                {
                    if (lightActive)
                    {
                        textField.text = "Tense your left ...";
                        textFieldComplex.text = "Tense your left ...";
                    }
                    else
                    {
                        textField.text = "Please wait!";
                        textFieldComplex.text = "Please wait!";
                    }
                }

                startTimer = false;
                if (firstRun)
                {
                    startPicture = true;
                    firstRun = false;
                }
                if (restTime < 0)
                {
                    if (checkIfFeedbackIsInUse)
                    {
                        feedbackTimer -= Time.deltaTime;
                    }
                    if (feedbackTimer < 0)
                    {
#if COMPLEX_TRAFFIC_LIGHT
                        if (yellowWasActive)
                        {
                            yellowLightComplex.GetComponent<MeshRenderer>().material = black;
                            redLightComplex.GetComponent<MeshRenderer>().material = red;
                            lightSourceYellowComplex.SetActive(false);
                            lightSourceRedComplex.SetActive(true); yellowWasActive = false;
                        }
#endif
                        checkIfFeedbackIsInUse = false;
                        if (isRandomTimeactive)
                        {
                            randomTime = timeing;
                            isRandomTimeactive = false;
                        }
                        //Debug.Log("test after feedbacktimer " + randomTime.ToString("F"));
                        randomTime -= Time.deltaTime;
                        if (randomTime < 0)
                        {
                            greenLightTimer -= Time.deltaTime;

                            if (restStudyTime < 0)
                            {
                                GreenLightOff();
                                setStudy = true;
                                resetValues();
                                counter++;
                                if (counter < conditions.Length)
                                {
                                    splitConditions(indexOfCondition += 1);
                                }
                            }
                            else if (setStudy && isLightAlreadyOn)
                            {
                                GreenLightOn();
                                isLightAlreadyOn = false;
                            }

                            if (greenLightTimer < 0 && setLightOff == 0 && setStudy)
                            {
                                GreenLightOff();
                            }
                            else if (setLightOff == 1)
                            {
                                setLightOff = 0;
                            }
                        }
                    }
                }
            }
        }
    }   
    //Sets Lightbulb to green
    private void GreenLightOn()
    {
#if COMPLEX_TRAFFIC_LIGHT
        greenLightComplex.GetComponent<MeshRenderer>().material = green;
        redLightComplex.GetComponent<MeshRenderer>().material = black;
        lightSourceGreenComplex.SetActive(true);
        lightSourceRedComplex.SetActive(false);
#else
        greenLightSimple.GetComponent<MeshRenderer>().material = green;
        redLightSimple.GetComponent<MeshRenderer>().material = black;
        lightSourceGreenSimple.SetActive(true);
        lightSourceRedSimple.SetActive(false);
#endif
        lightActiveText = "on";
        lightActive = true;
        Debug.Log("counter: " + counter);
        if(startStudy)
        {
            _logRawData.CreateCSVTestArrary(muscle, feedback, timeing, repetition, counter, lightActiveText, (int)durationOfTestTimer, (int)(durationOfTest - restingTime - timeing - feebackT - greenLightT), muscles[0], muscles[1], muscles[2], muscles[3]);
        }
        _source.Play();
        
    }
    //Sets Lightbulb to black
    private void GreenLightOff()
    {
#if COMPLEX_TRAFFIC_LIGHT
        greenLightComplex.GetComponent<MeshRenderer>().material = black;
        redLightComplex.GetComponent<MeshRenderer>().material = red;
        lightSourceGreenComplex.SetActive(false);
        lightSourceRedComplex.SetActive(true);
#else
        greenLightSimple.GetComponent<MeshRenderer>().material = black;
        redLightSimple.GetComponent<MeshRenderer>().material = red;
        lightSourceGreenSimple.SetActive(false);
        lightSourceRedSimple.SetActive(true);
#endif
        lightActiveText = "off";
        lightActive = false;
        startPicture = false;
        visualsActive= false;
        if (startStudy)
        {
            _logRawData.CreateCSVTestArrary(muscle, feedback, timeing, repetition, counter, lightActiveText, (int)durationOfTestTimer, (int)(durationOfTest - restingTime - timeing - feebackT - greenLightT), muscles[0], muscles[1], muscles[2], muscles[3]);
        }
        setLightOff = 1;
        //set Feedback to false
        _feedbackManager.isTactileFeedbackOn = false;
        _feedbackManager.isEmsFeedbackOn = false;
        setStudy = false;
        isLightAlreadyOn = true;
    }
    public void setTimer()
    {
        feebackT = feedbackTimer;
        greenLightT = greenLightTimer;
        durationOfTestTimer = durationOfTest;
    }
    public void resetValues()
    {
        feedbackTimer = feebackT;
        greenLightTimer = greenLightT;
        durationOfTest = durationOfTestTimer;
        restTime = restingTime;
        restStudyTime = durationOfTest;
        checkIfFeedbackIsInUse = true;
        isRandomTimeactive = true;
        isFeedbackUsed = false;
        firstRun = true;
        yellowWasActive = true;
    }
    public void StartStudy()
    {
        startStudy = true;
        startTimer = true;
        UI.SetActive(false);
        wall.SetActive(false);
        text.SetActive(false);
        EMGInterface.SetActive(false);
        EMGMainPanel.transform.position = new Vector3(30, 250, 0);
    }

    public void ShowStudy()
    {
        showStudy = true;
        startTimer = true;
        UI.SetActive(false);
        wall.SetActive(false);
        text.SetActive(false);
        EMGInterface.SetActive(false);
        EMGMainPanel.transform.position = new Vector3(30, 250, 0);
    }

    public void BackToMenu()
    {
        showStudy = false;
        UI.SetActive(true);
        wall.SetActive(true);
        text.SetActive(true);
        EMGInterface.SetActive(true);
        counter = 0;
        indexOfCondition = 0;
        splitConditions(counter);
        startTimerTime = 10f;
        EMGMainPanel.transform.position = new Vector3(150, 350, 0);
    }
    // ###################### Shuffler ###################### //
    public static T[] Shuffle<T>(T[] array, int seed)
    {

        UnityEngine.Random.InitState(seed);

        int n = array.Length;
        for (int i = 0; i < n; i++)
        {

            int r = i + (int)(UnityEngine.Random.value * (n - i));
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
        return array;
    }

    public static T[] GetLatinSquare<T>(T[] array, int participant)
    {
        if (array.Length <= 1) return array;
        // Init Square
        int[,] latinSquare = new int[array.Length, array.Length];

        latinSquare[0, 0] = 1;
        latinSquare[0, 1] = 2;

        // Fill 1st row
        for (int i = 2, j = 3, k = 0; i < array.Length; i++)
        {
            if (i % 2 == 1)
                latinSquare[0, i] = j++;
            else
                latinSquare[0, i] = array.Length - (k++);
        }

        // Fill first column
        for (int i = 1; i <= array.Length; i++)
        {
            latinSquare[i - 1, 0] = i;
        }

        // The rest of the square
        for (int row = 1; row < array.Length; row++)
        {
            for (int col = 1; col < array.Length; col++)
            {
                latinSquare[row, col] = (latinSquare[row - 1, col] + 1) % array.Length;

                if (latinSquare[row, col] == 0)
                    latinSquare[row, col] = array.Length;
            }
        }

        int squareItem = (((participant - 1) % array.Length));
        // Debug.Log("participant no. " + (squareItem + 1));

        // Return only the Participants' Latin Square Item 
        T[] newArray = new T[array.Length];

        for (int col = 0; col < array.Length; col++)
        {
            newArray[col] = array[latinSquare[squareItem, col] - 1];
        }
        return newArray;
    }

    public static T[] GetAllPermutations<T>(T[] array, int participant)
    {
        List<List<T>> results = GeneratePermutations<T>(array.ToList());
        T[] newArray = new T[array.Length];
        int row = (participant + 1) % (results.Count);
        for (int i = 0; i < results[row].Count; i++)
        {
            newArray[i] = results[row][i];
        }
        return newArray;
    }

    private static List<List<T>> GeneratePermutations<T>(List<T> items)
    {
        T[] current_permutation = new T[items.Count];
        bool[] in_selection = new bool[items.Count];
        List<List<T>> results = new List<List<T>>();
        PermuteItems<T>(items, in_selection, current_permutation, results, 0);
        return results;
    }

    private static void PermuteItems<T>(List<T> items, bool[] in_selection, T[] current_permutation, List<List<T>> results, int next_position)
    {
        if (next_position == items.Count)
        {
            results.Add(current_permutation.ToList());
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!in_selection[i])
                {
                    in_selection[i] = true;
                    current_permutation[next_position] = items[i];
                    PermuteItems<T>(items, in_selection, current_permutation, results, next_position + 1);
                    in_selection[i] = false;
                }
            }
        }
    }

    //Heuristic Non-Consecutive Duplicate (NCD) Shuffler
    public static T[] NCDShuffle<T>(T[] array, int seed)
    {
        if (array == null || array.Length <= 1) return null;
        int MAX_RETRIES = 100; //it's heuristic
        bool found;
        int retries = 1;
        do
        {
            array = Shuffle(array, seed);
            found = true;
            for (int i = 0; i < array.Length - 1; i++)
            {
                T cur = array[i];
                T next = array[i + 1];
                if (EqualityComparer<T>.Default.Equals(cur, next))
                {
                    //choose between front and back with some probability based on the size of sublists
                    int r = (int)(UnityEngine.Random.value * array.Length);
                    if (i < r)
                    {
                        if (!swapFront(i + 1, next, array, true))
                        {
                            found = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!swapBack(i + 1, next, array, true))
                        {
                            found = false;
                            break;
                        }
                    }
                }
            }
            retries++;
        } while (retries <= MAX_RETRIES && !found);
        return array;
    }

    private static bool swapFront<T>(int index, T t, T[] array, bool first)
    {
        if (index == array.Length - 1) return first ? swapBack(index, t, array, false) : false;
        int n = array.Length - index - 1;
        int r = (int)(UnityEngine.Random.value * n) + index + 1;
        int counter = 0;
        while (counter < n)
        {
            T t2 = array[r];
            if (!EqualityComparer<T>.Default.Equals(t, t2))
            {
                array = swap(array, index, r);
                //swaps++;
                return true;
            }
            r++;
            if (r == array.Length) r = index + 1;
            counter++;
        }

        //can't move it front, try back
        return first ? swapBack(index, t, array, false) : false;
    }

    private static T[] swap<T>(T[] array, int index, int r)
    {
        T tmp = array[r];
        array[r] = array[index];
        array[index] = tmp;
        return array;
    }

    //try to swap it with an element in a random "previous" position
    private static bool swapBack<T>(int index, T t, T[] array, bool first)
    {
        if (index <= 1) return first ? swapFront(index, t, array, false) : false;
        int n = index - 1;
        int r = (int)(UnityEngine.Random.value * n);
        int counter = 0;
        while (counter < n)
        {
            T t2 = array[r];
            if (!EqualityComparer<T>.Default.Equals(t, t2) && !hasEqualNeighbours(r, t, array))
            {
                array = swap(array, index, r);
                //swaps++;
                return true;
            }
            r++;
            if (r == index) r = 0;
            counter++;
        }
        return first ? swapFront(index, t, array, false) : false;
    }

    //check if an element t can fit in position i
    public static bool hasEqualNeighbours<T>(int i, T t, T[] array)
    {
        if (array.Length == 1)
            return false;
        else if (i == 0)
        {
            if (EqualityComparer<T>.Default.Equals(t, array[i + 1]))
                return true;
            return false;
        }
        else
        {
            if (EqualityComparer<T>.Default.Equals(t, array[i - 1]) || EqualityComparer<T>.Default.Equals(t, array[i + 1]))
                return true;
            return false;
        }
    }

    //check if shuffled with no consecutive duplicates
    public static bool isShuffledOK<T>(T[] array)
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], array[i - 1]))
                return false;
        }
        return true;
    }
    //count consecutive duplicates, the smaller the better; We need ZERO
    public static int getFitness<T>(T[] array)
    {
        int sum = 0;
        for (int i = 1; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], array[i - 1]))
                sum++;
        }
        return sum;
    }
    public static bool Compare<T>(T x, T y) where T : class
    {
        return x == y;
    }
}
