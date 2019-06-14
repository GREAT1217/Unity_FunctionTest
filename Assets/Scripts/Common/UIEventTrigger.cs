using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour ,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public UnityAction onPointerClick;
    public UnityAction onPointerEnter;
    public UnityAction onPointerExit;
    public UnityAction onPointerDown;
    public UnityAction onPointerUp;
    public UnityAction onBeginDrag;
    public UnityAction onDrag;
    public UnityAction onEndDrag;

    private static UIEventTrigger trigger;

    public static UIEventTrigger Add(GameObject go)
    {
        trigger = go.GetComponent<UIEventTrigger>();
        if (trigger == null) trigger = go.AddComponent<UIEventTrigger>();
        return trigger;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null) onPointerClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) onPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) onPointerExit();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) onPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) onPointerUp();
    }

}
