using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{

    public RectTransform ui1, ui2;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //float angle = Vector3.Dot(ui1.localPosition-ui2.localPosition,Vector3.right);
        //Vector3 angle = Vector3.Cross(ui1.localPosition - ui2.localPosition, Vector3.right);
        //float angle = Vector3.Angle(ui1.localPosition - ui2.localPosition, Vector3.right);
        float angle = Vector3.Angle(ui2.localPosition - ui1.localPosition, Vector3.right);

        //transform.localEulerAngles = angle / 2;
        transform.localEulerAngles = Vector3.forward * angle;


        transform.localPosition = (ui1.localPosition + ui2.localPosition) / 2;

    }
}
