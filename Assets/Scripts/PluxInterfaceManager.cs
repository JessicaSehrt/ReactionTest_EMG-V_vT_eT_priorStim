using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PluxInterfaceManager : MonoBehaviour
{
    // Class Variables
    private PluxDeviceManager pluxDevManager;
    [SerializeField] LogRawData _logRawData;
    [SerializeField] RadialFillElement _radialFillElement;
    [SerializeField] LightChanger _lightChanger;
    // GUI Objects.
    [Header("Buttons")]
    public Button ScanButton;
    public Button ConnectButton;
    public Button DisconnectButton;
    public Button StartAcqButton;
    public Button StopAcqButton;
    [Header("Dropdown")]
    public Dropdown DeviceDropdown;
    public Dropdown SamplingRateDropdown;
    public Dropdown ResolutionDropdown;
    public Dropdown RedIntensityDropdown;
    public Dropdown InfraredIntensityDropdown;
    [Header("Text")]
    public Text OutputMsgText;
    [Header("Visuals")]
    public RectTransform GraphContainer1;
    public RectTransform GraphContainer2;
    public RectTransform GraphContainer3;
    public RectTransform GraphContainer4;
    public string uniqueIdentifier;
    private bool isBluetoothConnected = false;
    [SerializeField] public Sprite DotSprite;
    public List<List<int>> MultiThreadList = null;
    public int VisualizationChannel = -1;
    public int GraphWindSize = -1;
    public List<double> convertedValues = null;
    public List<int> MultiThreadSubList = null;
    public bool UpdatePlotFlag = false;
    public bool FirstPlot = true;
    public int WindowInMemorySize;
    public List<int> ActiveChannels;
    public bool CH1Toggle;
    public bool CH2Toggle;
    public bool CH3Toggle;
    public bool CH4Toggle;
    public bool CH5Toggle;
    public bool CH6Toggle;
    public bool CH7Toggle;
    public bool CH8Toggle;

    // Class constants (CAN BE EDITED BY IN ACCORDANCE TO THE DESIRED DEVICE CONFIGURATIONS)
    [System.NonSerialized]
    public List<string> domains = new List<string>() { "BTH" };
    public WindowGraph GraphZone1;
    public WindowGraph GraphZone2;
    public WindowGraph GraphZone3;
    public WindowGraph GraphZone4;
    public GameObject MainPanel;
    public GameObject WindowGraph1;
    public GameObject WindowGraph2;
    public GameObject WindowGraph3;
    public GameObject WindowGraph4;

    public int samplingRate = 1000;

    private int Hybrid8PID = 517;
    private int BiosignalspluxPID = 513;
    private int BitalinoPID = 1538;
    private int MuscleBanPID = 1282;
    private int MuscleBanNewPID = 2049;
    private int CardioBanPID = 2050;
    private int BiosignalspluxSoloPID = 532;
    private int MaxLedIntensity = 255;

    private double scanDevicesAfter = 8f;

    void Awake()
    {
        // Find references to graphical objects.
        // User interface zone, where the acquired data will be plotted using the "WindowGraph.cs" script
        GraphContainer1 = transform.Find("Graph/WindowGraph1/GraphContainer").GetComponent<RectTransform>();
        GraphContainer2 = transform.Find("Graph/WindowGraph2/GraphContainer").GetComponent<RectTransform>();
        GraphContainer3 = transform.Find("Graph/WindowGraph3/GraphContainer").GetComponent<RectTransform>();
        GraphContainer4 = transform.Find("Graph/WindowGraph4/GraphContainer").GetComponent<RectTransform>();
        uniqueIdentifier = System.DateTime.Now.Ticks.ToString();
    }
    // Start is called before the first frame update
    private void Start()
    {
        // Initialise object
        pluxDevManager = new PluxDeviceManager(ScanResults, ConnectionDone, AcquisitionStarted, OnDataReceived, OnEventDetected, OnExceptionRaised);

        // Important call for debug purposes by creating a log file in the root directory of the project.
        pluxDevManager.WelcomeFunctionUnity();

        StartCoroutine(startCoroutine());
        //Auto-Scan on startup
        ScanButtonFunction();

        // Initialization of Variables.      
        MultiThreadList = new List<List<int>>();
        ActiveChannels = new List<int>();

        // Initialization of graphical zone.

        WindowGraph.IGraphVisual graphVisual1 = new WindowGraph.LineGraphVisual(GraphContainer1, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
        GraphContainer1 = graphVisual1.GetGraphContainer();
        GraphZone1 = new WindowGraph(GraphContainer1, graphVisual1);
        WindowGraph.IGraphVisual graphVisual2 = new WindowGraph.LineGraphVisual(GraphContainer2, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
        GraphContainer1 = graphVisual2.GetGraphContainer();
        GraphZone2 = new WindowGraph(GraphContainer2, graphVisual2);
        WindowGraph.IGraphVisual graphVisual3 = new WindowGraph.LineGraphVisual(GraphContainer3, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
        GraphContainer1 = graphVisual3.GetGraphContainer();
        GraphZone3 = new WindowGraph(GraphContainer3, graphVisual3);
        WindowGraph.IGraphVisual graphVisual4 = new WindowGraph.LineGraphVisual(GraphContainer4, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
        GraphContainer1 = graphVisual4.GetGraphContainer();
        GraphZone4 = new WindowGraph(GraphContainer4, graphVisual4);

        try
        {
            GraphZone1.ShowGraph(new List<int>() { 0 }, graphVisual1, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");
            GraphZone2.ShowGraph(new List<int>() { 0 }, graphVisual2, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");
            GraphZone3.ShowGraph(new List<int>() { 0 }, graphVisual3, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");
            GraphZone4.ShowGraph(new List<int>() { 0 }, graphVisual4, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");
            isBluetoothConnected = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            isBluetoothConnected = false;
        }
        // Create a timer that controls the update of real-time plot.
        System.Timers.Timer waitForPlotTimer = new System.Timers.Timer();
        waitForPlotTimer.Elapsed += new ElapsedEventHandler(OnWaitingTimeEnds);
        // Update timing of plot and all progress bars
        waitForPlotTimer.Interval = 100; //in milliseconds
        waitForPlotTimer.Enabled = true;
        waitForPlotTimer.AutoReset = true;
    }
    IEnumerator startCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 8 seconds.
        yield return new WaitForSeconds(8);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

        // Update function, being constantly invoked by Unity.
        private void Update() { }

    // Method invoked when the application was closed.
    private void OnApplicationQuit()
    {
        try
        {
            // Disconnect from device.
            if (pluxDevManager != null)
            {
                pluxDevManager.DisconnectPluxDev();
                Console.WriteLine("Application ending after " + Time.time + " seconds");
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine("Device already disconnected when the Application Quit.");
        }
    }

    /**
     * =================================================================================
     * ============================= GUI Events ========================================
     * =================================================================================
     */

    // Method called when the "Scan for Devices" button is pressed.
    public void ScanButtonFunction()
    {
        
        // Search for PLUX devices
        pluxDevManager.GetDetectableDevicesUnity(domains);
        
        // Disable the "Scan for Devices" button.
        ScanButton.interactable = false;
    }

    // Method called when the "Connect to Device" button is pressed.
    public void ConnectButtonFunction()
    {
        // Disable Connect button.
        ConnectButton.interactable = false;

        // Connect to the device selected in the Dropdown list.
        pluxDevManager.PluxDev(DeviceDropdown.options[DeviceDropdown.value].text);
    }

    // Method called when the "Disconnect Device" button is pressed.
    public void DisconnectButtonFunction()
    {
        // Disconnect from the device.
        pluxDevManager.DisconnectPluxDev();

        // Reboot GUI elements state.
        RebootGUI();
    }

    // Method called when the "Start Acquisition" button is pressed.
    public void StartButtonFunction()
    {
        /*  private int Hybrid8PID = 517;
            private int BiosignalspluxPID = 513;
            private int BitalinoPID = 1538;
            private int MuscleBanPID = 1282;
            private int MuscleBanNewPID = 2049;
            private int CardioBanPID = 2050;
            private int BiosignalspluxSoloPID = 532;
            private int MaxLedIntensity = 255;*/
        string deviceName = "";
        int deviceID = pluxDevManager.GetProductIdUnity();
        switch (deviceID)
        {
            case 517:
                deviceName = "Hybrid8";
                break;
            case 513:
                deviceName = "Biosignalsplux";
                break;
            case 1282:
                deviceName = "MuscleBan";
                break;
            case 2049:
                deviceName = "MuscleBanNew";
                break;
            case 2050:
                deviceName = "CardioBan";
                break;
            case 532:
                deviceName = "BiosignalspluxSolo";
                break;
            case 1538:
                deviceName = "Bitalino";
                break;
        }

        // Get the Sampling Rate and Resolution values.
        samplingRate = int.Parse(SamplingRateDropdown.options[SamplingRateDropdown.value].text);
        int resolution = int.Parse(ResolutionDropdown.options[ResolutionDropdown.value].text);

        // Initializing the sources array.
        List<PluxDeviceManager.PluxSource> pluxSources = new List<PluxDeviceManager.PluxSource>();
       
        // biosignalsplux Hybrid-8 device (3 sensors >>> 1 Analog + 2 Digital SpO2/fNIRS)
        if (pluxDevManager.GetProductIdUnity() == Hybrid8PID)
        {
            // Add the sources of the digital channels (CH1 and CH2).
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x03));
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x03));

            // Define the LED Intensities of both sensors (CH1 and CH2) as: {RED, INFRARED}
            int redLedIntensity = (int)(int.Parse(RedIntensityDropdown.options[RedIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int infraredLedIntensity = (int)(int.Parse(InfraredIntensityDropdown.options[InfraredIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            pluxDevManager.SetParameter(1, 0x03, ledIntensities);
            pluxDevManager.SetParameter(2, 0x03, ledIntensities);
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();

            // Add the source of the analog channel (CH8).
            pluxSources.Add(new PluxDeviceManager.PluxSource(8, 1, resolution, 0x01));

            // Add the sources of the internal IMU channels (CH11 with 9 derivations [3xACC | 3xGYRO | 3xMAG] defined by the 0x01FF chMask).
            int imuPort = 11;
            pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01FF));

            // Alternatively only some of the derivations can be activated.
            // >>> 3xACC (channel mask 0x0007)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0007));
            // >>> 3xGYR (channel mask 0x0038)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x0038));
            // >>> 3xMAG (channel mask 0x01C0)
            // pluxSources.Add(new PluxDeviceManager.PluxSource(imuPort, 1, resolution, 0x01C0));
        }
        // biosignalsplux (2 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux [CH1 and CH2 active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01)); // CH1 | EDA
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01)); // CH2 | ECG
            pluxSources.Add(new PluxDeviceManager.PluxSource(3, 1, resolution, 0x01)); // CH3 | ECG
            pluxSources.Add(new PluxDeviceManager.PluxSource(4, 1, resolution, 0x01)); // CH4 | ECG
            CH1Toggle = true;
            CH2Toggle = true;
            CH3Toggle = true;
            CH4Toggle = true;
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
            pluxDevManager.StartAcquisitionBySourcesUnity(1000, pluxSources.ToArray());
            // Add the sources of the digital channel (CH9 | fNIRS/SpO2).
            // pluxSources.Add(new PluxDeviceManager.PluxSource(9, 1, resolution, 0x03));

            // Define the LED Intensities of the CH9 sensor as: {RED, INFRARED}
            // int redLedIntensity = (int)(int.Parse(RedIntensityDropdown.options[RedIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int infraredLedIntensity = (int)(int.Parse(InfraredIntensityDropdown.options[InfraredIntensityDropdown.value].text) * (MaxLedIntensity / 100f)); // A 8-bit value (0-255)
            // int[] ledIntensities = new int[2] { redLedIntensity, infraredLedIntensity };
            // pluxDevManager.SetParameter(9, 0x03, ledIntensities);
        }
        // muscleBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x3F));
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
        }
        // muscleBAN v2 (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == MuscleBanNewPID)
        {
            // Starting a real-time acquisition from:
            // >>> muscleBAN [CH1 > EMG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> muscleBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
        }
        // cardioBAN (7 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == CardioBanPID)
        {
            // Starting a real-time acquisition from:
            // >>> cardioBAN [CH1 > ECG]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> cardioBAN Virtual Port [CH2-CH4 > ACC | CH5-CH7 > MAG active]
            pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
        }
        // biosignalsplux Solo (8 Analog sensors)
        else if (pluxDevManager.GetProductIdUnity() == BiosignalspluxSoloPID)
        {
            // Starting a real-time acquisition from:
            // >>> biosignalsplux Solo [CH1 > MICRO]
            pluxSources.Add(new PluxDeviceManager.PluxSource(1, 1, resolution, 0x01));
            // >>> biosignalsplux Solo [CH2 > CUSTOM]
            pluxSources.Add(new PluxDeviceManager.PluxSource(2, 1, resolution, 0x01));
            // >>> biosignalsplux Solo Virtual Port [CH3-CH5 > ACC | CH6-CH8 > MAG]
            //pluxSources.Add(new PluxDeviceManager.PluxSource(11, 1, resolution, 0x3F));
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
        }

        // BITalino (2 Analog sensors)
        if (pluxDevManager.GetProductIdUnity() == BitalinoPID)
        {
            // Starting a real-time acquisition from:
            // >>> BITalino [Channels A2 and A5 active]
            pluxDevManager.StartAcquisitionUnity(samplingRate, new List<int> { 2, 5 }, 10);
            Debug.Log("Used Device: " + deviceName + ", ID:" + pluxDevManager.GetProductIdUnity());
            GetChannel();
        }
        else
        {
            // Start a real-time acquisition with the created sources.
            pluxDevManager.StartAcquisitionBySourcesUnity(samplingRate, pluxSources.ToArray());
        }

        try
        {
            //int resolution = Int32.Parse(ResolutionDropDownOptions[ResolutionDropdown.value]);
            resolution = 16;

            // Update graphical window size variable (the plotting zone should contain 10 seconds of data).
            GraphWindSize = samplingRate * 10;
            WindowInMemorySize = Convert.ToInt32(1.1 * GraphWindSize);

            // Number of Active Channels.
            int nbrChannels = 0;

            bool[] toggleArray = new bool[]
                {CH1Toggle, CH2Toggle, CH3Toggle, CH4Toggle, CH5Toggle, CH6Toggle, CH7Toggle, CH8Toggle};
            MultiThreadList.Add(new List<int>(Enumerable.Repeat(0, GraphWindSize).ToList()));
            for (int i = 0; i < pluxDevManager.GetNbrChannelsUnity(); i++)
            {
                if (toggleArray[i] == true)
                {
                    // Preparation of a string that will be communicated to our .dll
                    // This string will be formed by "1" or "0" characters, identifying sequentially which channels are active or not.
                    ActiveChannels.Add(i + 1);

                    // Definition of the first active channel.
                    if (VisualizationChannel == -1)
                    {
                        VisualizationChannel = i + 1;
                    }

                    nbrChannels++;
                }

                // Dictionary that stores all the data received from .dll API.
                MultiThreadList.Add(new List<int>(Enumerable.Repeat(0, GraphWindSize).ToList()));
            }

            // Check if at least one channel is active.
            if (ActiveChannels.Count != 0)
            {
                // Start of Acquisition.
                //Thread.CurrentThread.Name = "MAIN_THREAD";
                if (pluxDevManager.GetDeviceTypeUnity() != "MuscleBAN BE Plux")
                {
                    pluxDevManager.StartAcquisitionUnity(samplingRate, ActiveChannels, resolution);
                }
                else
                {
                    // Definition of the frequency divisor (subsampling ratio).
                    int freqDivisor = 10;
                    pluxDevManager.StartAcquisitionMuscleBanUnity(samplingRate, ActiveChannels, resolution,
                        freqDivisor);
                }
            }
        }
        catch (Exception exc)
        {
            // Exception info.
            Debug.Log("Exception: " + exc.Message + "\n" + exc.StackTrace);
        }
    }
   

    // Method called when the "Stop Acquisition" button is pressed.
    public void StopButtonFunction()
    {
        // Stop the real-time acquisition.
        pluxDevManager.StopAcquisitionUnity();

        // Enable the "Start Acquisition" button and disable the "Stop Acquisition" button.
        StartAcqButton.interactable = true;
        StopAcqButton.interactable = false;
    }

    /**
     * =================================================================================
     * ============================= Callbacks =========================================
     * =================================================================================
     */

    // Callback that receives the list of PLUX devices found during the Bluetooth scan.
    public void ScanResults(List<string> listDevices)
    {
        // Enable the "Scan for Devices" button.
        ScanButton.interactable = true;
        Debug.Log("Devices " + listDevices);
        
        if (listDevices.Count > 0)
        {
            // Update list of devices.
            DeviceDropdown.ClearOptions();
            DeviceDropdown.AddOptions(listDevices);

            // Enable the Dropdown and the Connect button.
            DeviceDropdown.interactable = true;
            ConnectButton.interactable = true;

            // Show an informative message about the number of detected devices.
            OutputMsgText.text = "Scan completed.\nNumber of devices found: " + listDevices.Count;
            ConnectButtonFunction();
        }
        else
        {
            // Show an informative message stating the none devices were found.
            OutputMsgText.text = "Bluetooth device scan didn't found any valid devices.";
        }
    }

    // Callback invoked once the connection with a PLUX device was established.
    // connectionStatus -> A boolean flag stating if the connection was established with success (true) or not (false).
    public void ConnectionDone(bool connectionStatus)
    {
        if (connectionStatus)
        {
            // Disable some GUI elements.
            ScanButton.interactable = false;
            DeviceDropdown.interactable = false;
            ConnectButton.interactable = false;

            // Enable some generic GUI elements.
            if (pluxDevManager.GetProductIdUnity() != BitalinoPID)
            {
                ResolutionDropdown.interactable = true;
            }

            SamplingRateDropdown.interactable = true;
            StartAcqButton.interactable = true;
            DisconnectButton.interactable = true;

            // Enable some biosignalsplux Hybrid-8 specific GUI elements.
            if (pluxDevManager.GetProductIdUnity() == Hybrid8PID || pluxDevManager.GetProductIdUnity() == BiosignalspluxPID)
            {
                RedIntensityDropdown.interactable = true;
                InfraredIntensityDropdown.interactable = true;
            }
            //StartButtonFunction();
        }
        else
        {
            // Enable Connect button.
            ConnectButton.interactable = true;

            // Show an informative message stating the connection with the device was not established with success.
            OutputMsgText.text = "It was not possible to establish a connection with the device. Please, try to repeat the connection procedure.";
        }
    }

    // Callback invoked once the data streaming between the PLUX device and the computer is started.
    // acquisitionStatus -> A boolean flag stating if the acquisition was started with success (true) or not (false).
    // exceptionRaised -> A boolean flag that identifies if an exception was raised and should be presented in the GUI (true) or not (false).
    public void AcquisitionStarted(bool acquisitionStatus, bool exceptionRaised = false, string exceptionMessage = "")
    {
        if (acquisitionStatus)
        {
            // Enable the "Stop Acquisition" button and disable the "Start Acquisition" button.
            StartAcqButton.interactable = false;
            StopAcqButton.interactable = true;
        }
        else
        {
            // Present an informative message about the error.
            OutputMsgText.text = !exceptionRaised ? "It was not possible to start a real-time data acquisition. Please, try to repeat the scan/connect/start workflow." : exceptionMessage;

            // Reboot GUI.
            RebootGUI();
        }
    }

    // Callback invoked every time an exception is raised in the PLUX API Plugin.
    // exceptionCode -> ID number of the exception to be raised.
    // exceptionDescription -> Descriptive message about the exception.
    public void OnExceptionRaised(int exceptionCode, string exceptionDescription)
    {
        if (pluxDevManager.IsAcquisitionInProgress())
        {
            // Present an informative message about the error.
            OutputMsgText.text = exceptionDescription;

            // Reboot GUI.
            RebootGUI();
        }
    }

    // Callback that receives the data acquired from the PLUX devices that are streaming real-time data.
    // nSeq -> Number of sequence identifying the number of the current package of data.
    // data -> Package of data containing the RAW data samples collected from each active channel ([sample_first_active_channel, sample_second_active_channel,...]).
    public void OnDataReceived(int nSeq, int[] data)
    {
        // Show samples with a 1s interval.
        //if (nSeq % samplingRate == 0)
        //{
        if (_lightChanger.studyRunning)
        {
            // Show the current package of data.
            string outputString = "";
            for (int j = 0; j < data.Length; j++)
            {
                outputString += data[j] + ";";
            }

            _logRawData.CreateCSVRawData(outputString);
            if (nSeq % samplingRate == 0)
            {
                _radialFillElement.setData(outputString);
            }
            // Creation of the first graphical representation of the results.
            if (MultiThreadList[VisualizationChannel].Count >= 0)
            {
                if (FirstPlot)
                {
                    // Update flag (after this step we won't enter again on this statement).
                    FirstPlot = false;

                    // Plot first set of data.
                    // Subsampling if sampling rate is bigger than 1000 Hz.
                    // 0 is Ch1, 1 is Ch2 aso.
                    List<int> subSamplingList1 = GetSubSampleList(new int[GraphWindSize], 1000, GraphWindSize, 0);
                    List<int> subSamplingList2 = GetSubSampleList(new int[GraphWindSize], 1000, GraphWindSize, 1);
                    List<int> subSamplingList3 = GetSubSampleList(new int[GraphWindSize], 1000, GraphWindSize, 2);
                    List<int> subSamplingList4 = GetSubSampleList(new int[GraphWindSize], 1000, GraphWindSize, 3);
                    GraphZone1.ShowGraph(subSamplingList1, null, -1, _i => "-" + (GraphWindSize - _i),
                        _f => Mathf.RoundToInt(_f / 1000) + "k");
                    GraphZone2.ShowGraph(subSamplingList1, null, -1, _i => "-" + (GraphWindSize - _i),
                        _f => Mathf.RoundToInt(_f / 1000) + "k");
                    GraphZone3.ShowGraph(subSamplingList1, null, -1, _i => "-" + (GraphWindSize - _i),
                        _f => Mathf.RoundToInt(_f / 1000) + "k");
                    GraphZone4.ShowGraph(subSamplingList1, null, -1, _i => "-" + (GraphWindSize - _i),
                        _f => Mathf.RoundToInt(_f / 1000) + "k");
                }
                // Update plot.
                else if (FirstPlot == false)
                {
                    // This if clause ensures that the real-time plot will only be updated every 1 second (Memory Restrictions).
                    if (UpdatePlotFlag)
                    {
                        // Get the values linked with the last 10 seconds of information.
                        MultiThreadSubList = GetSubSampleList(data, 1000, GraphWindSize, 0);
                        GraphZone1.UpdateValue(MultiThreadSubList);
                        MultiThreadSubList = GetSubSampleList(data, 1000, GraphWindSize, 1);
                        GraphZone2.UpdateValue(MultiThreadSubList);
                        MultiThreadSubList = GetSubSampleList(data, 1000, GraphWindSize, 2);
                        GraphZone3.UpdateValue(MultiThreadSubList);
                        MultiThreadSubList = GetSubSampleList(data, 1000, GraphWindSize, 3);
                        GraphZone4.UpdateValue(MultiThreadSubList);
                        //convertedValues = convert_to_mv_TKEO(MultiThreadSubList);
                        // Reboot flag.
                        UpdatePlotFlag = false;
                        // Get the values linked with the last 10 seconds of information.
                    }
                }
            }
            // Show the values in the GUI.
            OutputMsgText.text = outputString;
        }
    }

    // Function used to subsample acquired data.
    public List<int> GetSubSampleList(int[] originalArray, int samplingRate, int graphWindowSize, int channel)
    {
        // Subsampling if sampling rate is bigger than 100 Hz.
        List<int> subSamplingList = new List<int>();
        int subSamplingLevel = 1;
        if (samplingRate > 100)
        {
            // Subsampling Level.
            subSamplingLevel = samplingRate / 100;
            for (int i = 0; i < originalArray.Length; i++)
            {
                if (i % subSamplingLevel == 0)
                {
                    subSamplingList.Add(originalArray[channel]);
                }
            }
        }
        else
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                subSamplingList.Add(originalArray[i]);
            }
        }

        return subSamplingList;
    }

    // Callback that receives the events raised from the PLUX devices that are streaming real-time data.
    // pluxEvent -> Event object raised by the PLUX API.
    public void OnEventDetected(PluxDeviceManager.PluxEvent pluxEvent)
    {
        if (pluxEvent is PluxDeviceManager.PluxDisconnectEvent)
        {
            // Present an error message.
            OutputMsgText.text =
                "The connection between the computer and the PLUX device was interrupted due to the following event: " +
                (pluxEvent as PluxDeviceManager.PluxDisconnectEvent).reason;

            // Securely stop the real-time acquisition.
            pluxDevManager.StopAcquisitionUnity(-1);

            // Reboot GUI.
            RebootGUI();
        }
        else if (pluxEvent is PluxDeviceManager.PluxDigInUpdateEvent)
        {
            PluxDeviceManager.PluxDigInUpdateEvent digInEvent = (pluxEvent as PluxDeviceManager.PluxDigInUpdateEvent);
            Console.WriteLine("Digital Input Update Event Detected on channel " + digInEvent.channel + ". Current state: " + digInEvent.state);
        }
    }

    public void GetChannel()
    {
        Debug.Log("Number of active Channel: " + pluxDevManager.GetNbrChannelsUnity());
    }

    /**
     * =================================================================================
     * ========================== Auxiliary Methods ====================================
     * =================================================================================
     */

    // Auxiliary method used to reboot the GUI elements.
    public void RebootGUI()
    {
        ScanButton.interactable = true;
        ConnectButton.interactable = false;
        DisconnectButton.interactable = false;
        StartAcqButton.interactable = false;
        StopAcqButton.interactable = false;
        DeviceDropdown.interactable = false;
        SamplingRateDropdown.interactable = false;
        ResolutionDropdown.interactable = false;
        RedIntensityDropdown.interactable = false;
        InfraredIntensityDropdown.interactable = false;
    }
    /*
    public List<double> convert_to_mv_TKEO(List<int> values)
    {
        mv_values.Clear();
        bp_filtered_values.Clear();
        TKEO_values.Clear();
        //conversion to millivolts according to https://plux.info/signal-processing/458-emg-sensor-unit-conversion.html
        foreach (int i in values)
        {
            converted_value = ((3 * (float)i / 65536) - 1.5); // Faktoren für Gain und Vcc in Formel eingerechnet
            mv_values.Add(converted_value);
        }

        bp_filtered_values = mv_values;

        //TKEO conversion and absolute value according to https://plux.info/signal-processing/460-event-detection-muscular-activations-emg.html
        int lc = 0;
        foreach (double j in bp_filtered_values)
        {
            if (lc == 0 || lc == (bp_filtered_values.Count - 1))
            {
                TKEO_values.Add(Math.Abs(bp_filtered_values[lc]));
            }
            else
            {
                TKEO_values.Add(Math.Abs((bp_filtered_values[lc] * bp_filtered_values[lc]) - (bp_filtered_values[lc + 1] * bp_filtered_values[lc - 1])));
            }
            lc++;

        }

        return TKEO_values;
    }*/
    public void RebootVariables()
    {
        MultiThreadList = new List<List<int>>();
        ActiveChannels = new List<int>();
        MultiThreadSubList = null;
        GraphWindSize = -1;
        VisualizationChannel = -1;
        UpdatePlotFlag = false;
    }

    public void OnWaitingTimeEnds(object source, ElapsedEventArgs e)
    {
        // Update flag, which will trigger the update of real-time plot.
        UpdatePlotFlag = true;
    }
}