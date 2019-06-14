using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工具UI数据
/// </summary>
public class ToolUIData
{
    public int _index;
    public Sprite _icon;
    public string _name;
    public string _type;
    public string _subType;

    public ToolUIData(int _index, Sprite _icon, string _name, string _type, string _subType)
    {
        this._index = _index;
        this._icon = _icon;
        this._name = _name;
        this._type = _type;
        this._subType = _subType;
    }
}

public class UIToolsManager : MonoBehaviour {

    public UIToolsBar _leftToolsBar;
    public UIToolsBar _rightToolsBar;

    private void Start()
    {
        _leftToolsBar.Init();
        _rightToolsBar.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _leftToolsBar.IsShow = !_leftToolsBar.IsShow;
            _rightToolsBar.IsShow = !_rightToolsBar.IsShow;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _leftToolsBar.ChangeTool(-1);
            Debug.Log("当前左手Index" + _leftToolsBar.CurToolIndex);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _leftToolsBar.ChangeTool(1);
            Debug.Log("当前左手Index" + _leftToolsBar.CurToolIndex);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _rightToolsBar.ChangeTool(-1);
            Debug.Log("当前右手Index" + _rightToolsBar.CurToolIndex);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _rightToolsBar.ChangeTool(1);
            Debug.Log("当前右手Index" + _rightToolsBar.CurToolIndex);
        }
    }
}
