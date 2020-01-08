using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAutoCenterItem : MonoBehaviour
{
    private UIAutoCenterScrollView _scrollView;
    private int _index;
    public Text _text;
    public Image _image;
    public Button _btn;

    void Start()
    {
        _btn.onClick.AddListener(() => _scrollView.SetCenterItem(_index));
    }

    public virtual void Init(UIAutoCenterScrollView scrollView,int index,string text)
    {
        _text.text = text;
        _index = index;
        _scrollView = scrollView;
        gameObject.SetActive(true);
    }

    public virtual void OnCenter()
    {
        Debug.Log("居中：" + _text.text);
    }
}