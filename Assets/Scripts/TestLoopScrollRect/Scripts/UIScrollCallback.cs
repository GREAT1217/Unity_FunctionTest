using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIScrollCallback : MonoBehaviour
{
    public Text _index;
    public Text _time;
    public Text _type;
    public Text _operate;
    public Text _score;
    public Button _detail;
    public Button _playback;
    public ScrollCellData _data;

    //回调函数，规定的方法名不能改
    //void ScrollCellIndex(int index) { }
    void ScrollCellContent(object obj)
    {
        _data = (ScrollCellData)obj;
        _index.text = _data._index.ToString();
        _time.text = _data._time.ToString();
        _type.text = _data._type.ToString();
        _operate.text = _data._operate;
        _score.text = _data._score.ToString();
        _detail.onClick.AddListener(() => _data._detail(_data._index));
        _playback.onClick.AddListener(() => _data._playback(_data._index));
    }

    void OnDisable()
    {
        _detail.onClick.RemoveAllListeners();
        _playback.onClick.RemoveAllListeners();
    }

}


