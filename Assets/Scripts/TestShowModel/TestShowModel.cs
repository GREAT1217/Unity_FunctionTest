using UnityEngine;
using System.Collections;

public class TestShowModel : MonoBehaviour
{
    public UIAutoCenterScrollView _scrollViewH;
    public UIAutoCenterScrollView _scrollViewV;

    public string[] _datas = new string[] { "壹", "贰", "叁", "肆", "伍", "陆" };
    void Start()
    {
        _scrollViewH.InitItem(ShowHItem, _datas.Length, 3);
        _scrollViewV.InitItem(ShowVItem, _datas.Length);
    }

    void ShowHItem(UIAutoCenterItem item, int index)
    {
        item.Init(_scrollViewH, index, _datas[index]);
    }

    void ShowVItem(UIAutoCenterItem item, int index)
    {
        item.Init(_scrollViewV, index, _datas[index]);
    }
}