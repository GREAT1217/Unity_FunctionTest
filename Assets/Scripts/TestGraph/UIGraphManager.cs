using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGraphManager : MonoBehaviour
{

    public UILineGraphManager _line;
    public UIBarGraphManager _bar;
    private GraphData[] _datas;

    private void Start()
    {
        RefeshData();
        _line.RefeshLineGraph(_datas);
        _bar.RefeshBarGraph(_datas);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefeshData();
            _line.RefeshLineGraph(_datas);
            _bar.RefeshBarGraph(_datas);
        }
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void RefeshData()
    {
        _datas = new GraphData[10];
        for (int i = 0; i < _datas.Length; i++)
        {
            _datas[i] = new GraphData(i.ToString(), Random.Range(0, 100));
        }
    }
}
