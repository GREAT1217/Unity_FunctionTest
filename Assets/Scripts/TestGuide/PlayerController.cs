using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody _rigid;
    float h, v;
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        _rigid.MovePosition(transform.position + new Vector3(h, 0, v));
    }
}
