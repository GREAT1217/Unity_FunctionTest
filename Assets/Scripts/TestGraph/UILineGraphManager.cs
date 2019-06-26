using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public struct GraphData
{
    public string _desc;
    [Range(0, 100)]
    public float _value;

    public float Rate
    {
        get
        {
            return _value / 100;
        }
    }

    public GraphData(string desc, float value)
    {
        _desc = desc;
        _value = value;
    }
}

/// <summary>
/// 折线图
/// </summary>
public class UILineGraphManager : MonoBehaviour
{
    public GraphData[] _datas;//数据
    public float _lineWidth = 3;//线宽
    public float _dotRadius = 2;//点半径
    public Transform _leftSide;//左侧描述
    public RectTransform _descContent;//描述Content
    public RectTransform _dotContent;//点Content
    public RectTransform _lineContent;//线Content
    public RectTransform _descPrefab;//描述Prefab
    public RectTransform _dotPrefab;//点Prefab
    public RectTransform _linePrefab;//线Prefab
    public float _tweenTime = 1f;

    //描述、点、线 管理
    private RectTransform[] _descs;
    private RectTransform[] _dots;
    private RectTransform[] _lines;
    //描述、点、线 对象池
    private List<RectTransform> _descPool;
    private List<RectTransform> _dotPool;
    private List<RectTransform> _linePool;

    /// <summary>
    /// 初始化折线图
    /// </summary>
    public void InitLineGraph(GraphData[] data)
    {
        //leftSide
        for (int i = 0; i < _leftSide.childCount; i++)
        {
            _leftSide.GetChild(i).GetComponent<Text>().text = (100 - i * 10).ToString();
        }
        RefeshLineGraph(data);
    }

    /// <summary>
    /// 刷新折线图
    /// </summary>
    public void RefeshLineGraph(GraphData[] data)
    {
        _datas = data;
        ClearTransform(_descs, _descPool);
        ClearTransform(_dots, _dotPool);
        ClearTransform(_lines, _linePool);
        DrawDesc();
        DrawDot();
        DrawLines();
    }

    /// <summary>
    /// 底部描述
    /// </summary>
    private void DrawDesc()
    {
        _descs = new RectTransform[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            RectTransform desc = GetTransform(_descPrefab, _descContent, ref _descPool);
            desc.GetComponent<Text>().text = _datas[i]._desc;
            desc.SetAsLastSibling();//使用对象池和自动布局组件会调乱顺序，要重置
            desc.gameObject.SetActive(true);
            _descs[i] = desc;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_descContent);//使用自动布局组件要刷新UI，刷新位置
    }

    /// <summary>
    /// 画点
    /// </summary>
    private void DrawDot()
    {
        float height = _dotContent.rect.height;
        _dots = new RectTransform[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            RectTransform dot = GetTransform(_dotPrefab, _dotContent, ref _dotPool);
            dot.localPosition = new Vector3(_descs[i].localPosition.x, height * (_datas[i].Rate - 0.5f), 0);//锚点在中心
            dot.sizeDelta = Vector2.one * _lineWidth * 2;
            dot.gameObject.SetActive(true);
            _dots[i] = dot;
        }
    }

    /// <summary>
    /// 画线
    /// </summary>
    private void DrawLines()
    {
        _lines = new RectTransform[_datas.Length - 1];
        DrawLine();
    }

    /// <summary>
    /// 画线
    /// </summary>
    /// <param name="index"></param>
    private void DrawLine(int index = 0)
    {
        if (index >= _lines.Length) return;
        Vector2 curPos = _dots[index].localPosition;
        Vector2 nextPos = _dots[index + 1].localPosition;
        float length = Vector2.Distance(curPos, nextPos);
        Vector3 dir = (nextPos.y > curPos.y) ? nextPos - curPos : curPos - nextPos;
        float angle = Vector3.Angle(dir.normalized, Vector3.right);
        Vector2 center = (curPos + nextPos) / 2;
        RectTransform line = GetTransform(_linePrefab, _lineContent, ref _linePool);
        line.localEulerAngles = Vector3.forward * (angle + 90);
        line.localPosition = center;
        line.sizeDelta = new Vector2(_lineWidth, length);
        line.gameObject.SetActive(true);
        _lines[index] = line;
        Image img = line.GetComponent<Image>();
        img.fillAmount = 0;
        img.fillOrigin = dir.x > 0 ? 1 : 0;
        img.DOFillAmount(1, _tweenTime / _lines.Length).OnComplete(() => DrawLine(index + 1));
    }

    /// <summary>
    /// 入池
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="pool"></param>
    private void ClearTransform(RectTransform[] trans, List<RectTransform> pool)
    {
        if (trans == null) return;
        for (int i = 0; i < trans.Length; i++)
        {
            trans[i].gameObject.SetActive(false);
            pool.Add(trans[i]);
        }
    }

    /// <summary>
    /// Instantiate
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <param name="pool">对象池</param>
    /// <returns></returns>
    private RectTransform GetTransform(RectTransform prefab, RectTransform parent, ref List<RectTransform> pool)
    {
        if (pool == null) pool = new List<RectTransform>();//这里不用ref的话，相当于没有new空间，可能是直接被回收了
        if (pool.Count <= 0)
        {
            return Instantiate(prefab, parent);
        }
        RectTransform temp = pool[0];
        pool.RemoveAt(0);
        return temp;
    }
}