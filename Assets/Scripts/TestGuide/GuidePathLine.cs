using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 引导线
/// </summary>
public class GuidePathLine : MonoBehaviour
{
    public float _flowSpeed = 10;//流动速度
    private LineRenderer _line;
    private Material _lineMat;
    private bool _isShow;
    private NavMeshAgent _curNavAgent;//当前引导寻路对象
    private NavMeshPath _curNavPath;//临时引导路径
    private GameObject _curGuideTarget;//当前引导目标

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _lineMat = _line.material;
        _curNavPath = new NavMeshPath();
    }

    void Update()
    {
        if (_isShow)
        {
            UpdateOffset();
            UpdateLine();
        }
    }

    private void UpdateOffset()
    {
        _lineMat.mainTextureOffset += Vector2.left * Time.deltaTime * _flowSpeed;//方向根据贴图调整
        if (_lineMat.mainTextureOffset.x % 1 == 0)
        {
            _lineMat.mainTextureOffset = Vector2.zero;
        }
    }

    private void UpdateLine()
    {
        bool canNav = _curNavAgent.CalculatePath(_curGuideTarget.transform.position, _curNavPath);
        if (canNav)
        {
            _line.positionCount = _curNavPath.corners.Length;
            _line.SetPositions(_curNavPath.corners);
        }
    }

    public void ShowLine(GameObject player, GameObject target)
    {
        _curNavAgent = player.GetComponent<NavMeshAgent>();
        if (_curNavAgent == null)
        {
            _curNavAgent = player.AddComponent<NavMeshAgent>();
        }
        _curGuideTarget = target;
        gameObject.SetActive(true);
        _isShow = true;
    }

    public void HideLine()
    {
        _isShow = false;
        gameObject.SetActive(false);
    }
}
