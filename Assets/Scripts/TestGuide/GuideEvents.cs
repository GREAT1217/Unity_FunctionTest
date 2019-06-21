using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuideEvents : MonoBehaviour, IPointerClickHandler
{
    public event Action SingleClick;
    public event Action DoubleClick;

    [Range(0, 1)]
    public float _clickInterval = 0.2f;//双击时间间隔
    private float _time1, _time2;

    /// <summary>
    /// 检测3D对象的单击、双击
    /// </summary>
    public void OnMouseDown()
    {
        SingleClickEvent();
        DoubleClickEvent();
    }

    /// <summary>
    /// 检测UI对象的单击、双击
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SingleClickEvent();
        DoubleClickEvent();
    }

    /// <summary>
    /// 单击事件
    /// </summary>
    private void SingleClickEvent()
    {
        if (SingleClick != null)
        {
            SingleClick();
            //foreach (Delegate d in SingleClick.GetInvocationList())
            //{
            //    SingleClick -= d as Action;
            //}
        }
    }

    /// <summary>
    /// 双击事件
    /// </summary>
    private void DoubleClickEvent()
    {
        _time1 = Time.timeSinceLevelLoad;
        if (_time1 - _time2 < _clickInterval)
        {
            if (DoubleClick != null)
            {
                DoubleClick();
                //foreach (Delegate d in DoubleClick.GetInvocationList())
                //{
                //    DoubleClick -= d as Action;
                //}
            }
        }
        _time2 = _time1;
    }

}
