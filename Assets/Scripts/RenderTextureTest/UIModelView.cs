using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIModelView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RaycastHit _hit;
    private Ray _ray;
    public CameraFollow _modelCamera;

    void Update()
    {
        RaycashTest();
    }

    private void RaycashTest()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _modelCamera._camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray,out _hit))
            {
                Debug.DrawLine(_hit.point, _ray.origin,Color.green);
                Debug.Log(_hit.transform.name);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _modelCamera._pointOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _modelCamera._pointOn = false;
    }

}
