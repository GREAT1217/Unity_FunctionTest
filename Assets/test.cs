using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class test : MonoBehaviour
{

    //public RectTransform ui1, ui2;

    //void Update()
    //{
    //    //float angle = Vector3.Dot(ui1.localPosition-ui2.localPosition,Vector3.right);
    //    //Vector3 angle = Vector3.Cross(ui1.localPosition - ui2.localPosition, Vector3.right);
    //    //float angle = Vector3.Angle(ui1.localPosition - ui2.localPosition, Vector3.right);
    //    float angle = Vector3.Angle(ui2.localPosition - ui1.localPosition, Vector3.right);

    //    //transform.localEulerAngles = angle / 2;
    //    transform.localEulerAngles = Vector3.forward * angle;


    //    transform.localPosition = (ui1.localPosition + ui2.localPosition) / 2;

    //}

    //public GuideEvents events;

    //private void Start()
    //{
    //    events.SingleClick += () => Debug.Log("单击");
    //    events.DoubleClick += () => Debug.Log("双击");
    //}

    public Image _image;

    void Start()
    {
        _image.DOFillAmount(0, 5);
        _image.rectTransform.DORotate(Vector3.forward*180, 5);
    }
}
