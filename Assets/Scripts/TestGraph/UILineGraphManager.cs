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
    public Text _descPrefab;//描述Prefab
    public RectTransform _dotPrefab;//点Prefab
    public Image _linePrefab;//线Prefab
    public float _tweenTime = 1f;//动画时间

    //描述、点、线 管理
    private Text[] _descs;
    private RectTransform[] _dots;
    private Image[] _lines;
    private const string DESCPOOL = "LDescPool";
    private const string DOTPOOL = "DotPool";
    private const string LINEPOOL = "LinePool";

    private void Awake()
    {
        ObjectPool.Instance.SetPrefab(DESCPOOL, _descPrefab.gameObject);
        ObjectPool.Instance.SetPrefab(DOTPOOL, _dotPrefab.gameObject);
        ObjectPool.Instance.SetPrefab(LINEPOOL, _linePrefab.gameObject);
    }

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
        ClearTransform(_descContent);
        ClearTransform(_dotContent);
        ClearTransform(_lineContent);
        DrawDesc();
        DrawDot();
        DrawLines();
    }

    /// <summary>
    /// 底部描述
    /// </summary>
    private void DrawDesc()
    {
        _descs = new Text[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            Text desc = ObjectPool.Instance.GetObject(DESCPOOL, _descContent).GetComponent<Text>();
            desc.text = _datas[i]._desc;
            desc.transform.SetAsLastSibling();//使用对象池和自动布局组件会调乱顺序，要重置
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
            RectTransform dot = ObjectPool.Instance.GetObject(DOTPOOL, _dotContent).GetComponent<RectTransform>();
            dot.localPosition = new Vector3(_descs[i].transform.localPosition.x, height * (_datas[i].Rate - 0.5f), 0);//锚点在中心
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
        _lines = new Image[_datas.Length - 1];
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
        Vector3 dir = curPos - nextPos;
        float angle = Vector3.Angle(Vector3.up, dir);
        Vector2 center = (curPos + nextPos) / 2;
        Image line = ObjectPool.Instance.GetObject(LINEPOOL, _lineContent).GetComponent<Image>();
        line.rectTransform.localEulerAngles = Vector3.forward * angle;
        line.rectTransform.localPosition = center;
        line.rectTransform.sizeDelta = new Vector2(_lineWidth, length);
        line.gameObject.SetActive(true);
        line.fillAmount = 0;
        line.fillOrigin = dir.x > 0 ? 0 : 1;
        line.DOFillAmount(1, _tweenTime / _lines.Length).OnComplete(() => DrawLine(index + 1));
        _lines[index] = line;
    }

    /// <summary>
    /// 入池
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="pool"></param>
    private void ClearTransform(Transform parent)
    {
        for (int i = 1; i < parent.childCount; i++)
        {
            ObjectPool.Instance.RecycleObj(parent.GetChild(i).gameObject, parent);
        }
    }

}