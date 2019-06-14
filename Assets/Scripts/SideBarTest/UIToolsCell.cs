using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工具按钮
/// </summary>
public class UIToolsCell : MonoBehaviour
{
    public Image _bg;//背景
    public Image _icon;//图标
    public Text _name;//名称
    public GameObject _division;//分割线
    public Sprite _select;//选中
    public Sprite _unSelect;//未选中
    public ToolUIData _data;//数据
    [HideInInspector]
    public UIToolsBar _toolsBar;//管理器引用

    public void Init(ToolUIData data, UIToolsBar toolsBar)
    {
        _data = data;
        _icon.sprite = data._icon;
        _name.text = data._name;
        _toolsBar = toolsBar;
        GetComponent<Button>().onClick.AddListener(BeClick);
    }

    public void BeSelected(bool value)
    {
        _bg.sprite = value ? _select : _unSelect;
    }

    public void BeClick()
    {
        _toolsBar.CurToolIndex = _data._index;
    }

}