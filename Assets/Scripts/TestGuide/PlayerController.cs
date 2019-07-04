using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float _speed = 30f;
    public Rigidbody _rigid;
    public Transform _camera;

    void Update()
    {
        Vector3 targetPos = transform.position;
        targetPos += transform.right * Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        targetPos += transform.forward * Input.GetAxis("Vertical") * _speed * Time.deltaTime;
        _rigid.MovePosition(targetPos);
        _rigid.MoveRotation(Quaternion.Euler(Vector3.up * _camera.eulerAngles.y));
    }
}
