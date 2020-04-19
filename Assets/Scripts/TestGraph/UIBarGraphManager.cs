using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 条形图
/// </summary>
public class UIBarGraphManager : MonoBehaviour
{
    public GraphData[] _datas;//数据
    public float _barWidth = 20;//条宽
    public Transform _leftSide;//左侧描述
    public RectTransform _descContent;//描述Content
    public RectTransform _barContent;//条Content
    public Text _descPrefab;//描述Prefab
    public Image _barPrefab;//条Prefab
    public float _tweenTime = 1f;//动画时间

    //描述、条 管理
    private Text[] _descs;
    private Image[] _bars;
    private const string DESCPOOL = "BDescPool";
    private const string BARPOOL = "BarPool";

    private void Awake()
    {
        ObjectPool.Instance.SetPrefab(DESCPOOL, _descPrefab.gameObject);
        ObjectPool.Instance.SetPrefab(BARPOOL, _barPrefab.gameObject);
    }

    /// <summary>
    /// 初始化条形图
    /// </summary>
    public void InitBarGraph(GraphData[] data)
    {
        //leftSide
        for (int i = 0; i < _leftSide.childCount; i++)
        {
            _leftSide.GetChild(i).GetComponent<Text>().text = (100 - i * 10).ToString();
        }
        RefeshBarGraph(data);
    }

    /// <summary>
    /// 刷新条形图
    /// </summary>
    public void RefeshBarGraph(GraphData[] data)
    {
        _datas = data;
        ClearTransform(_descContent);
        ClearTransform(_barContent);
        DrawDesc();
        DrawBar();
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
    /// 画条
    /// </summary>
    private void DrawBar()
    {
        _bars = new Image[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            Image bar = ObjectPool.Instance.GetObject(BARPOOL, _barContent).GetComponent<Image>();
            bar.rectTransform.sizeDelta = new Vector2(_barWidth, bar.rectTransform.sizeDelta.y);
            bar.rectTransform.localPosition = new Vector3(_descs[i].rectTransform.localPosition.x, bar.rectTransform.localPosition.y, 0);//锚点在中心
            bar.fillAmount = 0;
            bar.gameObject.SetActive(true);
            bar.DOFillAmount(_datas[i].Rate, _tweenTime);
            _bars[i] = bar;
        }
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
