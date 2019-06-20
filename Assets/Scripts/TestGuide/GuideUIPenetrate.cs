using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI事件渗透
/// </summary>
public class GuideUIPenetrate : MonoBehaviour, ICanvasRaycastFilter
{
    private RectTransform _target;

    public void SetTargetImage(RectTransform target)
    {
        _target = target;
    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_target == null)
            return true;

        return !RectTransformUtility.RectangleContainsScreenPoint(_target, sp, eventCamera);
    }
}
