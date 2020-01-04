using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 引导触发检测
/// </summary>
public class GuideTrigger : MonoBehaviour
{
    public UnityAction<Collider> TriggerEnter;
    public UnityAction<Collider, float> TriggerStay;

    private float _tempTime;

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerEnter != null) TriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _tempTime += Time.deltaTime;
        if (TriggerStay != null) TriggerStay(other, _tempTime);
    }

    private void OnTriggerExit(Collider other)
    {
        _tempTime = 0;
    }
}
