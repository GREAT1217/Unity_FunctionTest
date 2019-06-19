using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ECellType
{
    A, B, C
}

public class ScrollCellData
{
    public int _index;
    public DateTime _time;
    public ECellType _type;
    public string _operate;
    public int _score;
    public ButtonHandler _detail;
    public ButtonHandler _playback;

    public ScrollCellData(int index, DateTime time, ECellType type, string operate, int score, ButtonHandler detail, ButtonHandler playback)
    {
        _index = index;
        _time = time;
        _type = type;
        _operate = operate;
        _score = score;
        _detail = detail;
        _playback = playback;
    }
}

public delegate void ButtonHandler(int index);
public class UIScrollRectManager : MonoBehaviour
{
    public int _totalCount;
    public string _prefabPath;
    public LoopScrollRect _loopScrollRect;
    [HideInInspector]
    public List<ScrollCellData> _cellList;

    private void Start()
    {
        InitData();
        InitScrollRect();
    }

    private void InitData()
    {
        _cellList = new List<ScrollCellData>();
        for (int i = 0; i < _totalCount; i++)
        {
            _cellList.Add(new ScrollCellData(i + 1, DateTime.Now, (ECellType)Enum.Parse(typeof(ECellType), Random.Range(0, 3).ToString()), "描述" + i.ToString(), Random.Range(0, 100), DetialClick, PlayBackClick));
        }
    }

    private void InitScrollRect()
    {
        _loopScrollRect.prefabSource.prefabName = _prefabPath;
        _loopScrollRect.objectsToFill = _cellList.ToArray();
        _loopScrollRect.totalCount = _totalCount;
        _loopScrollRect.RefillCells();
    }

    public void DetialClick(int index)
    {
        Debug.Log("细节" + index);
    }

    public void PlayBackClick(int index)
    {
        Debug.Log("回放" + index);
    }
}
