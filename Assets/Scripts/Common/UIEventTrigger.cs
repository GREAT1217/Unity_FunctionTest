using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIEventTrigger :EventTrigger
{
    public UnityAction PointerClick;
    public UnityAction PointerEnter;
    public UnityAction PointerExit;
    public UnityAction PointerDown;
    public UnityAction PointerUp;
    public UnityAction BeginDrag;
    public UnityAction Drag;
    public UnityAction EndDrag;

    private static UIEventTrigger trigger;

    public static UIEventTrigger Add(GameObject go)
    {
        trigger = go.GetComponent<UIEventTrigger>();
        if (trigger == null) trigger = go.AddComponent<UIEventTrigger>();
        return trigger;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (PointerClick != null) PointerClick();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (PointerEnter != null) PointerEnter();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (PointerExit != null) PointerExit();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (PointerDown != null) PointerDown();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (PointerUp != null) PointerUp();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (BeginDrag != null) BeginDrag();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (Drag != null) Drag();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (EndDrag != null) EndDrag();
    }
}
