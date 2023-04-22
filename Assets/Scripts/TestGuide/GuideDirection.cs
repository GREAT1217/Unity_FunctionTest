using System;
using UnityEngine;

public class GuideDirection : MonoBehaviour
{
    public Transform _target;
    public Transform _uiPoint;
    public Transform _uiDirection;
    public Camera _camera;
    public float _ovalAreaRate = 0.8f;

    public void ShowDirection(Transform target)
    {
        _target = target;
        gameObject.SetActive(true);
    }

    public void HideDirection()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_target == null) return;

        Vector2 screenPoint = _camera.WorldToScreenPoint(_target.position);
        var ovalRadiusX = Screen.width * 0.5f * _ovalAreaRate;
        var ovalRadiusY = Screen.height * 0.5f * _ovalAreaRate;
        var ovalCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        // Debug.DrawLine(screenPoint, ovalCenter, Color.red, 1);

        // 椭圆区域限制。
        var inArea = IsPointInOval(screenPoint, ovalCenter, ovalRadiusX, ovalRadiusY);
        if (inArea)
        {
            _uiDirection.localScale = Vector3.zero;
            _uiPoint.position = screenPoint;
        }
        else
        {
            var angle = SingedAngle(Vector2.right, ovalCenter - screenPoint);
            _uiDirection.eulerAngles = new Vector3(0, 0, angle);
            _uiDirection.localScale = Vector3.one;
            angle = SingedAngle(Vector2.right, screenPoint - ovalCenter);
            var point = GetPointInOval(angle, ovalCenter, ovalRadiusX, ovalRadiusY);
            _uiPoint.position = point;
        }

        // 矩形区域限制。
        // var inArea = GetPointInRect(ref screenPoint, ovalCenter, ovalRadiusX, ovalRadiusY);
        // if (inArea)
        // {
        //     _uiPoint.position = screenPoint;
        //     _uiDirection.localScale = Vector3.zero;
        // }
        // else
        // {
        //     var angle = SingedAngle(Vector2.right, ovalCenter - screenPoint);
        //     _uiDirection.eulerAngles = new Vector3(0, 0, angle);
        //     _uiDirection.localScale = Vector3.one;
        //     _uiPoint.position = screenPoint;
        // }
    }

    /// <summary>
    /// 获取点是否在椭圆内。
    /// </summary>
    /// <param name="point">检测的点。</param>
    /// <param name="centerPoint">椭圆中心点。</param>
    /// <param name="ovalRadiusX">椭圆横向半径。</param>
    /// <param name="ovalRadiusY">椭圆纵向半径。</param>
    /// <returns></returns>
    private bool IsPointInOval(Vector2 point, Vector2 centerPoint, float ovalRadiusX, float ovalRadiusY)
    {
        var v = Mathf.Pow(centerPoint.x - point.x, 2) / Mathf.Pow(ovalRadiusX, 2) + Mathf.Pow(centerPoint.y - point.y, 2) / Mathf.Pow(ovalRadiusY, 2);
        return v < 1f;
    }

    /// <summary>
    /// 获取椭圆上的点。
    /// </summary>
    /// <param name="angle">相对X轴正方向的角度（有符号）。</param>
    /// <param name="centerPoint">椭圆中心点。</param>
    /// <param name="ovalRadiusX">椭圆横向半径。</param>
    /// <param name="ovalRadiusY">椭圆纵向半径。</param>
    /// <returns>椭圆上的点。</returns>
    private Vector2 GetPointInOval(float angle, Vector2 centerPoint, float ovalRadiusX, float ovalRadiusY)
    {
        var x = centerPoint.x + ovalRadiusX * Mathf.Cos(angle * Mathf.Deg2Rad);
        var y = centerPoint.y + ovalRadiusY * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }

    /// <summary>
    /// 获取矩形内的点。
    /// </summary>
    /// <param name="point">检测的点</param>
    /// <param name="centerPoint">矩形中心点。</param>
    /// <param name="halfX">矩形横向半径。</param>
    /// <param name="halfY">矩形纵向半径。</param>
    /// <returns>是否在矩形内。</returns>
    private bool GetPointInRect(ref Vector2 point, Vector2 centerPoint, float halfX, float halfY)
    {
        var x = Mathf.Clamp(point.x, centerPoint.x - halfX, centerPoint.x + halfX);
        var y = Mathf.Clamp(point.y, centerPoint.y - halfY, centerPoint.y + halfY);
        if (Math.Abs(x - point.x) < 0.01f && Math.Abs(y - point.y) < 0.01f) // float 精度太高了，不用 == 对比
        {
            return true;
        }

        point = new Vector2(x, y);
        return false;
    }

    private float SingedAngle(Vector2 from, Vector2 to)
    {
        var angle = Vector2.Angle(from, to);
        var cross = Vector3.Cross(from, to);
        angle *= (cross.z > 0 ? 1 : -1);
        return angle;
    }
}
