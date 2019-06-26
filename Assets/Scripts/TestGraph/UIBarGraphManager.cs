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
    public RectTransform _descPrefab;//描述Prefab
    public RectTransform _barPrefab;//条Prefab
    public float _tweenTime = 1f;

    //描述、条 管理
    private RectTransform[] _descs;
    private RectTransform[] _bars;
    //描述、条 对象池
    private List<RectTransform> _descPool;
    private List<RectTransform> _barPool;

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
        ClearTransform(_descs, _descPool);
        ClearTransform(_bars, _barPool);
        DrawDesc();
        DrawBar();
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
    /// 画条
    /// </summary>
    private void DrawBar()
    {
        _bars = new RectTransform[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            RectTransform bar = GetTransform(_barPrefab, _barContent, ref _barPool);
            bar.sizeDelta = new Vector2(_barWidth, bar.sizeDelta.y);
            bar.localPosition = new Vector3(_descs[i].localPosition.x, bar.localPosition.y, 0);//锚点在中心
            bar.GetComponent<Image>().fillAmount = 0;
            bar.gameObject.SetActive(true);
            bar.GetComponent<Image>().DOFillAmount(_datas[i].Rate, _tweenTime);
            _bars[i] = bar;
        }
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
