using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeedbackManager : MonoBehaviour
{
    //public GameObject visualFeedback;
    // public AudioSource auditiveFeedback;
    public SerialController tactileSerialController;
    public SerialController emsSerialController;
    [SerializeField] LightChanger _lightChanger;

    public bool isTactileFeedbackOn = false;
    public bool isEmsFeedbackOn = false;

    private string ClickedButtonName;
    //StudyDesignManager
    //private ConditionDescription currentCondition = new ConditionDescription(false, false, false);
    private bool isInTestMode = false;

    void Start()
    {
        //UpdateFeedback();
        //TestFeedback();
        //SetTactileFeedback(1,true);
    }

    public void UpdateFeedback()
    {
        //if(isInTestMode)
         //   return;
        //values from method
        //var percentage = calculatePercentage(currentValue, maxValue);

        //SetVisualFeedback(percentage, currentCondition.HasVisual);
        //SetTactileFeedback(percentage, currentCondition.HasTactile);
        //SetAuditiveFeedback(percentage, currentCondition.HasAuditive);
        if(isTactileFeedbackOn)
        {
            SetTactileFeedback(1, isTactileFeedbackOn);
        }
        if(isEmsFeedbackOn) {
            SetEMSFeedback(1, isEmsFeedbackOn);
        }        
    }

  /*  public void NextEpoch(ConditionDescription condition)
    {
        currentCondition = condition;
    }*/

    public void TestFeedback()
    {
        if (!isInTestMode)
        {
            isInTestMode = true;
            ClickedButtonName = EventSystem.current.currentSelectedGameObject.name;
            Debug.Log("Button clicked: " + ClickedButtonName);
            StartCoroutine(nameof(TestFeedbackCo));
        }
    }

    private IEnumerator TestFeedbackCo()
    {
        isInTestMode = true;
        SetTactileFeedback(1, true);
        yield return new WaitForSecondsRealtime(3);
        SetTactileFeedback(0, false);
        SetEMSFeedback(1, true);
        yield return new WaitForSecondsRealtime(3);
        SetEMSFeedback(0, false);
        isInTestMode = false;
    }
    IEnumerator startCoroutine()
    {
        //yield on a new YieldInstruction that waits for 1 seconds.
        yield return new WaitForSeconds(1);
    }

    private float calculatePercentage(double currentValue, double maxValue)
    {
        return (float)(currentValue / maxValue);
    }
    private void SetEMSFeedback(float percentage, bool activate)
    {
        /*if (!activate)
        {
            //startCoroutine();
            SetEMSFeedback(0, true);
            return;
        }*/
        if (emsSerialController is null)
        {
            Debug.Log("ems serial controller is null");
            return;
        }

        // Trigger EMS
        if (isInTestMode)
        {
            switch (ClickedButtonName)
            {
                case "EMSBiceps":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 1 : 0) + "\n");
                    break;
                case "EMSTriceps":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 2 : 0) + "\n");
                    break;
                case "EMSUpperLeg":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 4 : 0) + "\n");
                    break;
                case "EMSLowerLeg":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 3 : 0) + "\n");
                    break;
            }
            //Debug.Log("EMS ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? (short)(75 + percentage) : 0));
        }
        else
        {
            switch (_lightChanger.muscle)
            {
                case "biceps":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 1 : 0) + "\n");
                    Debug.Log("EMS ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 1 : 0));
                    break;
                case "triceps":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 2 : 0) + "\n");
                    Debug.Log("EMS ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 2 : 0));
                    break;
                case "upper leg":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 4 : 0) + "\n");
                    Debug.Log("EMS ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 3 : 0));
                    break;
                case "calf":
                    emsSerialController.SendSerialMessage((percentage > 0 ? 3 : 0) + "\n");
                    Debug.Log("EMS ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 4 : 0));
                    break;
                default:

                    break;
            }
        }        
    }

    private void SetTactileFeedback(float percentage, bool activate)
    {
        if(!activate)
        {
            SetTactileFeedback(0, true);
            return;
        }
        //Debug.Log(XInputDotNetPure.GamePad.GetState(XInputDotNetPure.PlayerIndex.One).IsConnected);
        if (tactileSerialController is null)
        {
            Debug.Log("tactile serial controller is null");
            return;
        }
        
        
        // Trigger Vib
        if (isInTestMode)
        {
            switch (ClickedButtonName)
            {
                case "VibBiceps":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 5 : 0) + "\n");
                    break;
                case "VibTriceps":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 6 : 0) + "\n");
                    break;
                case "VibUpperLeg":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 8 : 0) + "\n");
                    break;
                case "VibLowerLeg":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 7 : 0) + "\n");
                    break;
            }
            //Debug.Log("Vibration ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? "all" : 0));          
        }
        else
        {
            switch (_lightChanger.muscle)
            {
                case "biceps":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 5 : 0) + "\n");
                    Debug.Log("Vibration ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 5 : 0));
                    break;
                case "triceps":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 6 : 0) + "\n");
                    Debug.Log("Vibration ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 6 : 0));
                    break;
                case "upper leg":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 8 : 0) + "\n");
                    Debug.Log("Vibration ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 8 : 0));
                    break;
                case "calf":
                    tactileSerialController.SendSerialMessage((percentage > 0 ? 7 : 0) + "\n");
                    Debug.Log("Vibration ausgelöst, percentage: " + percentage + " rechnung: " + (percentage > 0 ? 7 : 0));
                    break;
                default:

                    break;
            }
        } 
    }
}