using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISecondPanel : MonoBehaviour
{
    public RectTransform _upPart;
    public RectTransform _downPart;
    public float _useTime = 0.5f;

    public void InitUIPath(string path)
    {
        gameObject.SetActive(true);
        _upPart.GetComponentInChildren<Text>().text = path;
        _upPart.GetComponentInChildren<Button>().onClick.AddListener(() => MovePanel(false));
    }

    //tween
    public void MovePanel(bool show)
    {
        if (show)
        {
            _upPart.VerticalMoveToShow(_useTime, false);
            _downPart.VerticalMoveToShow(_useTime);
        }
        else
        {
            TweenCallback action = () => gameObject.SetActive(false);
            _upPart.VerticalMoveToHide(_useTime);
            _downPart.VerticalMoveToHide(_useTime,false).OnComplete(action);
        }
    }

}
