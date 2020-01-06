using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAutoCenterItem : MonoBehaviour
{
    public UIAutoCenterScrollView _scrollView;
    public int _index;
    public Text _text;
    public Image _image;
    public Button _btn;

    void Start()
    {
        _btn.onClick.AddListener(() => _scrollView.SetCenterItem(_index));
    }

    public void Init(int index,UIAutoCenterScrollView scrollView)
    {
        _index = index;
        _scrollView = scrollView;
    }
}