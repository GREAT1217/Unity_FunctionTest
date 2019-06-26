using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 扇形图
/// </summary>
public class UIPieGraphManager : MonoBehaviour
{
    public GraphData[] _datas;//数据
    public bool _sort;//排序
    public float _pieRadius = 100;//扇形半径
    public RectTransform _descContent;//描述Content
    public RectTransform _pieContent;//扇形Content
    public RectTransform _descPrefab;//描述Prefab
    public RectTransform _piePrefab;//扇形Prefab
    public float _tweenTime = 1f;

    //描述、扇形、颜色 管理
    private RectTransform[] _descs;
    private RectTransform[] _pies;
    private Color[] _colors;
    //描述、扇形 对象池
    private List<RectTransform> _descPool;
    private List<RectTransform> _piePool;

    private void Start()
    {

    }

    /// <summary>
    /// 初始化扇形图
    /// </summary>
    public void InitPieGraph(GraphData[] data)
    {
        RefeshPieGraph(data);
    }

    /// <summary>
    /// 刷新扇形图
    /// </summary>
    public void RefeshPieGraph(GraphData[] data)
    {
        _datas = data;
        if (_sort) Sort();
        ClearTransform(_descs, _descPool);
        ClearTransform(_pies, _piePool);
        DrawDesc();
        DrawPie();
    }

    private GraphData _tempData;
    /// <summary>
    /// 排序
    /// </summary>
    private void Sort()
    {
        for (int i = 0; i < _datas.Length; i++)
        {
            for (int j = 0; j < _datas.Length - i - 1; j++)
            {
                if (_datas[j]._value > _datas[j + 1]._value)
                {
                    _tempData = _datas[j];
                    _datas[j] = _datas[j + 1];
                    _datas[j + 1] = _tempData;
                }
            }
        }
    }

    /// <summary>
    /// 描述
    /// </summary>
    private void DrawDesc()
    {
        _descs = new RectTransform[_datas.Length];
        _colors = new Color[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            RectTransform desc = GetTransform(_descPrefab, _descContent, ref _descPool);
            _colors[i] = new Color(0, (float)i / _datas.Length, 0);
            desc.GetComponent<Text>().text = _datas[i]._desc;
            desc.GetComponentInChildren<Image>().color = _colors[i];
            desc.gameObject.SetActive(true);
            _descs[i] = desc;
        }
    }

    private Vector3 _curAngle;
    /// <summary>
    /// 画扇形
    /// </summary>
    private void DrawPie()
    {
        float sum = 0;
        for (int i = 0; i < _datas.Length; i++)
        {
            sum += _datas[i]._value;
        }
        _pies = new RectTransform[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            RectTransform pie = GetTransform(_piePrefab, _pieContent, ref _piePool);
            pie.sizeDelta = Vector2.one * _pieRadius * 2;
            float rate = _datas[i]._value / sum;
            pie.GetComponent<Image>().fillAmount = 0;
            pie.GetComponent<Image>().color = _colors[i];
            _curAngle += Vector3.forward * 360 * rate;
            pie.localEulerAngles = _curAngle;
            pie.gameObject.SetActive(true);
            UIPieImage pieImg = pie.GetComponent<UIPieImage>();
            pieImg.DOFillAmount(rate, _tweenTime).OnComplete(() => pieImg.ResetCollider());
            _pies[i] = pie;
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
