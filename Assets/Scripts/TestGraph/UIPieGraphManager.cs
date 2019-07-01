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
    public RectTransform _noteContent;//注解Content
    public RectTransform _pieContent;//扇形Content
    public Text _notePrefab;//注解Prefab
    public UIPieImage _piePrefab;//扇形Prefab
    public float _tweenTime = 1f;//动画时间

    //注解、扇形、颜色 管理
    private Text[] _notes;
    private UIPieImage[] _pies;
    private Color[] _colors;

    private void Awake()
    {
        ObjectPool.Instance.SetPrefab(_notePrefab.gameObject);
        ObjectPool.Instance.SetPrefab(_piePrefab.gameObject);
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
        ClearTransform(_noteContent);
        ClearTransform(_pieContent);
        DrawNote();
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
    /// 颜色注解
    /// </summary>
    private void DrawNote()
    {
        _notes = new Text[_datas.Length];
        _colors = new Color[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            Text note = ObjectPool.Instance.GetObject(_notePrefab.name, _noteContent).GetComponent<Text>();
            _colors[i] = new Color(0, (float)i / _datas.Length, 0);
            note.text = _datas[i]._desc;
            note.GetComponentInChildren<Image>().color = _colors[i];
            note.gameObject.SetActive(true);
            _notes[i] = note;
        }
    }

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
        _pies = new UIPieImage[_datas.Length];
        Vector3 _curAngle = Vector3.zero;
        for (int i = 0; i < _datas.Length; i++)
        {
            UIPieImage pie = ObjectPool.Instance.GetObject(_piePrefab.name, _pieContent).GetComponent<UIPieImage>();
            pie.rectTransform.sizeDelta = Vector2.one * _pieRadius * 2;
            float rate = _datas[i]._value / sum;
            pie.fillAmount = 0;
            pie.color = _colors[i];
            _curAngle += Vector3.forward * 360 * rate;
            pie.rectTransform.localEulerAngles = _curAngle;
            //pie.rectTransform.localScale = Vector3.zero;
            pie.gameObject.SetActive(true);
            //pie.rectTransform.DOScale(Vector3.one, _tweenTime);
            pie.DOFillAmount(rate, _tweenTime).OnComplete(() => pie.ResetCollider());
            _pies[i] = pie;
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
