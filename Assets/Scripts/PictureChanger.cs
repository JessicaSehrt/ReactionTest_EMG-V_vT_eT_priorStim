using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureChanger : MonoBehaviour
{
    [SerializeField] LightChanger _lightChanger;
    [SerializeField] DecideSide _decideSide;
    public GameObject bicepsGreen;
    public GameObject tricepsGreen;
    public GameObject upperlegGreen;
    public GameObject lowerlegGreen;
    public GameObject bicepsRed;
    public GameObject tricepsRed;
    public GameObject upperlegRed;
    public GameObject lowerlegRed;
    public GameObject bicepsLeft;
    public GameObject tricepsLeft;
    public GameObject upperlegLeft;
    public GameObject lowerlegLeft;
    public GameObject muscle;

    void Update()
    {
        if (_lightChanger.showStudy)
        {
            if (_lightChanger.startPicture)
            {
                muscle.SetActive(true);
            }
            else
            {
                muscle.SetActive(false);
            }
        }
        else
        {
            if (_lightChanger.startPicture)
            {
                if (_decideSide.rightSide)
                {
                    if (_lightChanger.lightActive)
                    {
                        switch (_lightChanger.muscle)
                        {
                            case "biceps":
                                bicepsGreen.SetActive(true);
                                tricepsGreen.SetActive(false);
                                upperlegGreen.SetActive(false);
                                lowerlegGreen.SetActive(false);
                                bicepsRed.SetActive(false);
                                tricepsRed.SetActive(false);
                                lowerlegRed.SetActive(false);
                                upperlegRed.SetActive(false);
                                muscle.SetActive(false);
                                break;
                            case "triceps":
                                bicepsGreen.SetActive(false);
                                tricepsGreen.SetActive(true);
                                upperlegGreen.SetActive(false);
                                lowerlegGreen.SetActive(false);
                                bicepsRed.SetActive(false);
                                tricepsRed.SetActive(false);
                                lowerlegRed.SetActive(false);
                                upperlegRed.SetActive(false);
                                muscle.SetActive(false);
                                break;
                            case "upper leg":
                                bicepsGreen.SetActive(false);
                                tricepsGreen.SetActive(false);
                                upperlegGreen.SetActive(true);
                                lowerlegGreen.SetActive(false);
                                bicepsRed.SetActive(false);
                                tricepsRed.SetActive(false);
                                lowerlegRed.SetActive(false);
                                upperlegRed.SetActive(false);
                                muscle.SetActive(false);
                                break;
                            case "calf":
                                bicepsGreen.SetActive(false);
                                tricepsGreen.SetActive(false);
                                upperlegGreen.SetActive(false);
                                lowerlegGreen.SetActive(true);
                                bicepsRed.SetActive(false);
                                tricepsRed.SetActive(false);
                                lowerlegRed.SetActive(false);
                                upperlegRed.SetActive(false);
                                muscle.SetActive(false);
                                break;
                            default:
                                bicepsGreen.SetActive(false);
                                tricepsGreen.SetActive(false);
                                upperlegGreen.SetActive(false);
                                lowerlegGreen.SetActive(false);
                                bicepsRed.SetActive(false);
                                tricepsRed.SetActive(false);
                                lowerlegRed.SetActive(false);
                                upperlegRed.SetActive(false);
                                muscle.SetActive(false);
                                break;
                        }
                    }
                    else
                    {
                        if (_lightChanger.visualsActive)
                        {
                            switch (_lightChanger.muscle)
                            {
                                case "biceps":
                                    bicepsGreen.SetActive(false);
                                    tricepsGreen.SetActive(false);
                                    upperlegGreen.SetActive(false);
                                    lowerlegGreen.SetActive(false);
                                    bicepsRed.SetActive(true);
                                    tricepsRed.SetActive(false);
                                    lowerlegRed.SetActive(false);
                                    upperlegRed.SetActive(false);
                                    muscle.SetActive(false);
                                    break;
                                case "triceps":
                                    bicepsGreen.SetActive(false);
                                    tricepsGreen.SetActive(false);
                                    upperlegGreen.SetActive(false);
                                    lowerlegGreen.SetActive(false);
                                    bicepsRed.SetActive(false);
                                    tricepsRed.SetActive(true);
                                    lowerlegRed.SetActive(false);
                                    upperlegRed.SetActive(false);
                                    muscle.SetActive(false);
                                    break;
                                case "upper leg":
                                    bicepsGreen.SetActive(false);
                                    tricepsGreen.SetActive(false);
                                    upperlegGreen.SetActive(false);
                                    lowerlegGreen.SetActive(false);
                                    bicepsRed.SetActive(false);
                                    tricepsRed.SetActive(false);
                                    lowerlegRed.SetActive(false);
                                    upperlegRed.SetActive(true);
                                    muscle.SetActive(false);
                                    break;
                                case "calf":
                                    bicepsGreen.SetActive(false);
                                    tricepsGreen.SetActive(false);
                                    upperlegGreen.SetActive(false);
                                    lowerlegGreen.SetActive(false);
                                    bicepsRed.SetActive(false);
                                    tricepsRed.SetActive(false);
                                    lowerlegRed.SetActive(true);
                                    upperlegRed.SetActive(false);
                                    muscle.SetActive(false);
                                    break;
                                default:
                                    bicepsGreen.SetActive(false);
                                    tricepsGreen.SetActive(false);
                                    upperlegGreen.SetActive(false);
                                    lowerlegGreen.SetActive(false);
                                    bicepsRed.SetActive(false);
                                    tricepsRed.SetActive(false);
                                    lowerlegRed.SetActive(false);
                                    upperlegRed.SetActive(false);
                                    muscle.SetActive(false);
                                    break;
                            }
                        }
                        else
                        {
                            bicepsGreen.SetActive(false);
                            tricepsGreen.SetActive(false);
                            upperlegGreen.SetActive(false);
                            lowerlegGreen.SetActive(false);
                            bicepsRed.SetActive(false);
                            tricepsRed.SetActive(false);
                            lowerlegRed.SetActive(false);
                            upperlegRed.SetActive(false);
                            muscle.SetActive(true);
                        }
                    }

                }
                else if (_decideSide.leftSide)
                {
                    switch (_lightChanger.muscle)
                    {
                        case "biceps":
                            bicepsLeft.SetActive(true);
                            tricepsLeft.SetActive(false);
                            upperlegLeft.SetActive(false);
                            lowerlegLeft.SetActive(false);
                            break;
                        case "triceps":
                            bicepsLeft.SetActive(false);
                            tricepsLeft.SetActive(true);
                            upperlegLeft.SetActive(false);
                            lowerlegLeft.SetActive(false);
                            break;
                        case "upper leg":
                            bicepsLeft.SetActive(false);
                            tricepsLeft.SetActive(false);
                            upperlegLeft.SetActive(true);
                            lowerlegLeft.SetActive(false);
                            break;
                        case "calf":
                            bicepsLeft.SetActive(false);
                            tricepsLeft.SetActive(false);
                            upperlegLeft.SetActive(false);
                            lowerlegLeft.SetActive(true);
                            break;
                        default:
                            bicepsLeft.SetActive(false);
                            tricepsLeft.SetActive(false);
                            upperlegLeft.SetActive(false);
                            lowerlegLeft.SetActive(false);
                            break;
                    }
                }
            }
            else
            {
                bicepsLeft.SetActive(false);
                tricepsLeft.SetActive(false);
                upperlegLeft.SetActive(false);
                lowerlegLeft.SetActive(false);
                bicepsGreen.SetActive(false);
                tricepsGreen.SetActive(false);
                upperlegGreen.SetActive(false);
                lowerlegGreen.SetActive(false);
            }
        }
    }
}
