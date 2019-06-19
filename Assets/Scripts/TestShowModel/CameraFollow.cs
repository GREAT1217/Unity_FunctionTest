using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Range
{
    public float _min;
    public float _max;
    public float Value
    {
        get { return _max - _min; }
    }

    public Range(float min, float max)
    {
        _min = min;
        _max = max;
    }
}

public class CameraFollow : MonoBehaviour
{
    public Transform _target;
    public Camera _camera;
    public float _zoomSensitivity = 10f;
    public float _rotateSensitivity = 10f;
    public float _translateSensitivity = 1f;
    public Range _distanceRange = new Range(1f, 5f);
    public Range _angleXRange = new Range(-90f, 90f);
    public Range _areaRange = new Range(1f, 1f);
    [Range(1f, 10f)]
    public float _damper = 5f;
    [HideInInspector]
    public bool _pointOn;

    private Vector2 _currentAngle;
    private Vector2 _targetAngle;
    private Vector2 _defaultAngle;
    private float _currentDistance;
    private float _targetDistance;
    private float _defaultDistance;

    void Start()
    {
        _defaultAngle = _currentAngle = _targetAngle = transform.eulerAngles;
        _defaultDistance = _currentDistance = _targetDistance = Vector3.Distance(_camera.transform.position, _target.position);
    }

    void Update()
    {
        if (_pointOn)
        {
            if (Input.touchCount == 0) MouseControl();
            else if (Input.touchCount == 1) SingleTouch();
            else if (Input.touchCount > 1) DoubleTouch();
        }
    }

    void LateUpdate()
    {
        UpdateValue();
    }

    /// <summary>
    /// 单指触屏
    /// </summary>
    private void SingleTouch()
    {
        Touch newTouch = Input.GetTouch(0);
        if (newTouch.phase == TouchPhase.Moved)
        {
            Vector2 deltaPos = newTouch.deltaPosition;
            _targetAngle.y += deltaPos.x * Time.deltaTime * _rotateSensitivity;
            _targetAngle.x -= deltaPos.y * Time.deltaTime * _rotateSensitivity;
        }
    }

    private Vector2 oldPos0, oldPos1;
    /// <summary>
    /// 双指触屏
    /// </summary>
    private void DoubleTouch()
    {
        Touch newTouch0 = Input.GetTouch(0);
        Touch newTouch1 = Input.GetTouch(1);
        Vector2 newPos0 = newTouch0.position;
        Vector2 newPos1 = newTouch1.position;
        if (newTouch0.phase == TouchPhase.Moved || newTouch1.phase == TouchPhase.Moved)
        {
            if (IsEnlarge(oldPos0, oldPos1, newPos0, newPos1))
            {
                _targetDistance -= Time.deltaTime * _zoomSensitivity;
            }
            else
            {
                _targetDistance += Time.deltaTime * _zoomSensitivity;
            }
        }
        oldPos0 = newPos0;
        oldPos1 = newPos1;
    }

    /// <summary>
    /// 返回放大或缩小手势
    /// </summary>
    /// <param name="op0"></param>
    /// <param name="op1"></param>
    /// <param name="np0"></param>
    /// <param name="np1"></param>
    /// <returns>true为放大</returns>
    private bool IsEnlarge(Vector2 op0, Vector2 op1, Vector2 np0, Vector2 np1)
    {
        float length1 = Vector2.Distance(op0, op1);
        float length2 = Vector2.Distance(np0, np1);
        if (length1 < length2) return true;
        else return false;
    }

    /// <summary>
    /// 鼠标控制
    /// </summary>
    private void MouseControl()
    {
        if (Input.GetMouseButton(0))
        {
            _targetAngle.y += Input.GetAxis("Mouse X") * _rotateSensitivity;
            _targetAngle.x -= Input.GetAxis("Mouse Y") * _rotateSensitivity;
        }
        _targetDistance -= Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
    }

    /// <summary>
    /// 刷新相机
    /// </summary>
    private void UpdateValue()
    {
        _targetDistance = Mathf.Clamp(_targetDistance, _distanceRange._min, _distanceRange._max);//限制缩放

        _currentAngle = Vector2.Lerp(_currentAngle, _targetAngle, _damper * Time.deltaTime);
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, _damper * Time.deltaTime);

        _camera.transform.rotation = Quaternion.Euler(_currentAngle);//旋转
        _camera.transform.position = _target.position - _currentDistance * _camera.transform.forward;//缩放

    }

    /// <summary>
    /// 重置参数
    /// </summary>
    public void ResetCamera()
    {
        _targetAngle = _defaultAngle;
        _targetDistance = _defaultDistance;
    }

}

