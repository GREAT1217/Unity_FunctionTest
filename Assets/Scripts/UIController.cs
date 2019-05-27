using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public UIFirstLevel _firstLevel;
    public UISecondLevel _secondLevel;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null) Instance = this;
    }



}
