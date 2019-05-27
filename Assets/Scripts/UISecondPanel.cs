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
        float upY = 0, downY = 0;
        TweenCallback action = null;
        if (show)
        {
            upY = Screen.height / 2 + _upPart.rect.y;
            downY = -(Screen.height / 2 + _downPart.rect.y);
        }
        else
        {
            upY = Screen.height / 2 - _upPart.rect.y;
            downY = -(Screen.height / 2 - _downPart.rect.y);
            action = () => gameObject.SetActive(false);
        }
        _upPart.DOLocalMoveY(upY, _useTime);
        _downPart.DOLocalMoveY(downY, _useTime).OnComplete(action);
    }

}
