using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 工具栏
/// </summary>
public class UIToolsBar : MonoBehaviour
{
    public UIToolsCell _cellPrefab;//工具按钮模板
    public RectTransform _bg;
    public RectTransform _title;
    public RectTransform _scrollView;
    public RectTransform _content;
    public Scrollbar _scorllBar;
    public float _bottomSpace;//底部空白
    public float _moveSpeed = 5f;//移动速度
    public int _showCount;//显示数量
    [HideInInspector]
    public UIToolsCell _curTool;//当前工具按钮
    [HideInInspector]
    public List<ToolUIData> _toolsDataList;//工具UI数据列表
    private List<UIToolsCell> _toolsUIList;//工具按钮列表
    private int _totalCount;//总数量
    private float _showX;//显示时位置X
    private float _hideX;//隐藏时位置X
    private bool _isShow;//是否已显示
    private int _curToolIndex;//当前工具索引

    /// <summary>
    /// 当前工具索引
    /// </summary>
    public int CurToolIndex
    {
        get
        {
            return _curToolIndex;
        }

        set
        {
            if (_curTool != null) _curTool.BeSelected(false);
            _curToolIndex = value;
            if (_curToolIndex == _toolsDataList.Count) _curToolIndex = 0;
            else if (_curToolIndex == -1) _curToolIndex = _toolsDataList.Count - 1;
            _curTool = _toolsUIList[_curToolIndex];
            _curTool.BeSelected(true);
        }
    }
    /// <summary>
    /// 当前显示中
    /// </summary>
    public bool IsShow
    {
        get
        {
            return _isShow;
        }

        set
        {
            _isShow = value;
            ToMove(_isShow);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        //数据
        InitToolsData();
        //实体
        InitToolsUI();
        CurToolIndex = 0;
        //标题栏
        _title.GetComponent<Button>().onClick.AddListener(() => IsShow = false);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void InitToolsData()
    {
        _toolsDataList = new List<ToolUIData>();
        for (int i = 0; i < 10; i++)
        {
            _toolsDataList.Add(new ToolUIData(i, null, "工具" + i, "a", "b"));
        }
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitToolsUI()
    {
        _toolsUIList = new List<UIToolsCell>();
        for (int i = 0; i < _toolsDataList.Count; i++)
        {
            UIToolsCell tool = Instantiate(_cellPrefab, _content);
            tool.gameObject.SetActive(true);
            tool.Init(_toolsDataList[i],this);
            _toolsUIList.Add(tool);
        }
        //调整 scrollView bg
        _totalCount = _toolsDataList.Count;
        float cellHeight = _cellPrefab.GetComponent<RectTransform>().sizeDelta.y;
        _scrollView.sizeDelta = new Vector2(_scrollView.sizeDelta.x, cellHeight * Mathf.Min(_totalCount, _showCount));
        _bg.sizeDelta = _title.sizeDelta + _scrollView.sizeDelta + new Vector2(0, _bottomSpace);
        _hideX = _bg.anchoredPosition.x;
        _showX = -_hideX;
    }

    /// <summary>
    /// 移动显示或隐藏
    /// </summary>
    /// <param name="show"></param>
    public void ToMove(bool show)
    {
        StopAllCoroutines();
        StartCoroutine(WaitToMove(show));
    }

    /// <summary>
    /// 移动动画
    /// </summary>
    /// <param name="show"></param>
    /// <returns></returns>
    private IEnumerator WaitToMove(bool show)
    {
        float targetX = show ? _showX : _hideX;
        float dir = targetX - _bg.anchoredPosition.x;
        if (Mathf.Max(targetX, _bg.anchoredPosition.x) == targetX)
        {
            while (targetX - _bg.anchoredPosition.x > 0.01f)
            {
                yield return null;
                _bg.anchoredPosition += Vector2.right * dir * Time.deltaTime * _moveSpeed;
            }
        }
        else
        {
            while (_bg.anchoredPosition.x - targetX > 0.01f)
            {
                yield return null;
                _bg.anchoredPosition += Vector2.right * dir * Time.deltaTime * _moveSpeed;
            }
        }
        _bg.anchoredPosition = new Vector2(targetX, _bg.anchoredPosition.y);
    }

    /// <summary>
    /// 切换工具
    /// </summary>
    /// <param name="dir">方向，1为向下</param>
    public void ChangeTool(int dir = 1)
    {
        if (!IsShow) return;
        CurToolIndex += dir;
        ChangeScrollBar();
    }

    /// <summary>
    /// 更改滑动条
    /// </summary>
    private void ChangeScrollBar()
    {
        if (_toolsDataList.Count <= _showCount) return;
        if (CurToolIndex >= _showCount)
        {
            _scorllBar.value = 1 - (float)(CurToolIndex + 1) / _totalCount;
        }
        else
        {
            _scorllBar.value = 1;
        }
    }

}
