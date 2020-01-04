using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExpandCircularScrollView : UICircularScrollView
{
    public GameObject _expandBtn;
    public float _backgroundMargin;
    public bool _isExpand = false;

    private float _expandBtnX;
    private float _expandBtnY;
    private float _expandBtnWidth;
    private float _expandBtnHeight;

    private struct ExpandInfo
    {
        public GameObject btn;
        public bool isExpand;
        public CellInfo[] cellInfos;
        public float size;//横向或纵向占用长度大小
        public int cellCount;
    }
    private ExpandInfo[] _expandInfos;

    private Action<GameObject, GameObject, int, int> _funcCallBackFunc;
    protected Action<GameObject, GameObject, int, int> _funcOnClickCallBack;

    void Start()
    {
        Init(ExpandCallback);
        ShowList(2, 3, 4, 5);
    }

    void ExpandCallback(GameObject cell, GameObject childCell, int index, int childIndex)
    {
        cell.GetComponentInChildren<Text>().text = "ClickME:" + index.ToString();
        if (childCell != null)
        {
            childCell.transform.Find("Text").GetComponent<Text>().text = childIndex.ToString();
        }
    }

    public void Init(Action<GameObject, GameObject, int, int> callback)
    {
        base.Init(null, null);
        _funcCallBackFunc = callback;
        RectTransform expandRectTrans = _expandBtn.GetComponent<RectTransform>();
        _expandBtnX = expandRectTrans.anchoredPosition.x;
        _expandBtnY = expandRectTrans.anchoredPosition.y;
        _expandBtnWidth = expandRectTrans.rect.width;
        _expandBtnHeight = expandRectTrans.rect.width;
        SetPoolsButtonObj(_expandBtn);
    }

    public void ShowList(params int[] num)
    {
        ClearCell();
        int curCellCount = 0;//当前元素数量
        int cellCount = 0;//元素总数量
        int expandCount = num.Length;//扩展按钮总数量
        bool isReset;//重新生成
        if (_isInited && _expandInfos.Length == expandCount)
        {
            isReset = false;
        }
        else
        {
            _expandInfos = new ExpandInfo[expandCount];
            isReset = true;
        }
        //生成扩展按钮
        for (int i = 0; i < expandCount; i++)
        {
            GameObject button = GetPoolsButtonObj();
            button.name = i.ToString();
            Button btn = button.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnClickExpand(button));
                btn.transform.SetSiblingIndex(0);
            }
            //设置位置
            SetExpandPos(button.GetComponent<RectTransform>(), i, curCellCount);
            int count = num[i];
            cellCount += count;
            //存储数据
            ExpandInfo expandInfo = isReset ? new ExpandInfo() : _expandInfos[i];
            expandInfo.btn = button;
            expandInfo.cellCount = count;
            expandInfo.cellInfos = new CellInfo[count];
            expandInfo.isExpand = isReset ? _isExpand : expandInfo.isExpand;
            expandInfo.size = _dir == Direction.Vertical ? (_cellHeight + _spacing) * Mathf.CeilToInt((float)count / _crNum) : (_cellWight + _spacing) * Mathf.CeilToInt((float)count / _crNum);
            //生成cell
            for (int k = 0; k < count; k++)
            {
                if (!expandInfo.isExpand) break;
                CellInfo cellInfo = new CellInfo();
                //计算Cell位置并显示或隐藏
                SetCellPos(cellInfo, i, k, curCellCount);
                float cellPos = _dir == Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                }
                else
                {
                    GameObject cell = GetPoolsObj();
                    cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                    cell.name = i + "-" + k;
                    Button btnCell = cell.GetComponent<Button>();
                    if (btnCell != null)
                    {
                        btnCell.onClick.AddListener(() => _funcOnClickCallBack(button, cell, i, k));
                    }
                    cellInfo.obj = cell;
                    //cell显示回调
                    _funcCallBackFunc(button, cell, i, k);
                }
                expandInfo.cellInfos[i] = cellInfo;
            }
            curCellCount += expandInfo.isExpand ? count : 0;
            _expandInfos[i] = expandInfo;
            if (!_isInited)
            {
                //计算content尺寸
                SetContentSize(expandCount, cellCount);
            }
            _isInited = true;
        }
    }

    private void SetExpandPos(RectTransform rectTrans, int expandIndex, int curCellCount)
    {
        if (_dir == Direction.Vertical)
        {
            float pos = _expandBtnHeight * expandIndex + _spacing * (expandIndex + 1);
            pos += expandIndex > 0 ? (_cellHeight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum) : 0;
            rectTrans.anchoredPosition = new Vector2(_expandBtnX, -pos);
        }
        else
        {
            float pos = _expandBtnWidth * expandIndex + _spacing * (expandIndex + 1);
            pos += expandIndex > 0 ? (_cellWight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum) : 0;
            rectTrans.anchoredPosition = new Vector2(pos, _expandBtnY);
        }
    }

    private void SetCellPos(CellInfo cellInfo, int expandIndex, int cellIndex, int curCellCount)
    {
        if (_dir == Direction.Vertical)
        {
            float pos = _cellHeight * Mathf.FloorToInt(cellIndex / _crNum) + _spacing * (Mathf.FloorToInt(cellIndex / _crNum) + 1);
            pos += (_expandBtnHeight + _spacing) * (expandIndex + 1);
            pos += (_cellHeight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum);
            float posX = _cellWight * (cellIndex % _crNum) + _spacing * (cellIndex % _crNum);
            cellInfo.pos = new Vector2(posX, -pos);
        }
        else
        {
            float pos = _cellWight * Mathf.FloorToInt(cellIndex / _crNum) + _spacing * (Mathf.FloorToInt(cellIndex / _crNum) + 1);
            pos += (_expandBtnWidth + _spacing) * (expandIndex + 1);
            pos += (_cellHeight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum);
            float posY = _cellHeight * (cellIndex % _crNum) + _spacing * (cellIndex % _crNum);
            cellInfo.pos = new Vector2(pos, -posY);
        }
    }

    private void SetContentSize(int expandCount, int cellCount)
    {
        if (_dir == Direction.Vertical)
        {
            float contentSize = _isExpand ? (_spacing + _cellHeight) * Mathf.CeilToInt((float)cellCount / _crNum) : 0;
            contentSize += (_spacing + _expandBtnHeight) * expandCount;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentSize);
        }
        else
        {
            float contentSize = _isExpand ? (_spacing + _cellWight) * Mathf.CeilToInt((float)cellCount / _crNum) : 0;
            contentSize += (_spacing + _expandBtnWidth) * expandCount;
            _content.sizeDelta = new Vector2(contentSize, _content.sizeDelta.y);
        }
    }

    private void ClearCell()
    {
        if (_isInited)
        {
            for (int i = 0; i < _expandInfos.Length; i++)
            {
                if (_expandInfos[i].btn != null)
                {
                    SetPoolsButtonObj(_expandInfos[i].btn);
                    _expandInfos[i].btn = null;
                }
                for (int j = 0; j < _expandInfos[i].cellInfos.Length; j++)
                {
                    if (_expandInfos[i].cellInfos[j].obj != null)
                    {
                        SetPoolsObj(_expandInfos[i].cellInfos[j].obj);
                        _expandInfos[i].cellInfos[j].obj = null;
                    }
                }
            }
        }
    }

    private void OnClickExpand(GameObject button)
    {
        int index = int.Parse(button.name);
        OnClickExpand(index);
        if (OnButtonClickCallback != null)
        {
            OnButtonClickCallback(index, _expandInfos[index - 1].isExpand, button);
        }
    }

    private void OnClickExpand(int index)
    {
        _expandInfos[index].isExpand = !_expandInfos[index].isExpand;
        //重置Content大小
        Vector2 size = _content.sizeDelta;
        if (_dir == Direction.Vertical)
        {
            float height = _expandInfos[index].isExpand ? size.y + _expandInfos[index].size : size.y - _expandInfos[index].size;
            _content.sizeDelta = new Vector2(size.x, height);
        }
        else
        {
            float width = _expandInfos[index].isExpand ? size.x + _expandInfos[index].size : size.x - _expandInfos[index].size;
            _content.sizeDelta = new Vector2(width, size.y);
        }
        int beforeCellCount = 0;
        float pos;
        float rowPos;
        //重新计算坐标显示处理
        for (int i = 0; i < _expandInfos.Length; i++)
        {
            int count = _expandInfos[i].cellCount;
            if (i >= index)
            {
                GameObject button = _expandInfos[i].btn;
                if (_dir == Direction.Vertical)
                {
                    pos = _expandBtnHeight * i + _spacing * (i + 1);
                    pos += (_cellHeight + _spacing) * Mathf.CeilToInt((float)beforeCellCount / _crNum);
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(_expandBtnX, -pos);
                }
                else
                {
                    pos = _expandBtnWidth * i + _spacing * (i + 1);
                    pos += (_cellWight + _spacing) * Mathf.CeilToInt((float)beforeCellCount / _crNum);
                    button.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos, _expandBtnY);
                }
                ExpandInfo expandInfo = _expandInfos[i];
                for (int k = 0; k < count; k++)
                {
                    if (!expandInfo.isExpand)
                    {
                        if (expandInfo.cellInfos[k].obj != null)
                        {
                            SetPoolsObj(expandInfo.cellInfos[k].obj);
                            _expandInfos[i].cellInfos[k].obj = null;
                        }
                        continue;
                    }
                    CellInfo cellInfo = expandInfo.cellInfos[k];
                    if (_dir == Direction.Vertical)
                    {
                        pos = _cellHeight * Mathf.FloorToInt(k / _crNum) + _spacing * (Mathf.FloorToInt(k / _crNum) + 1);
                        pos += (_expandBtnHeight + _spacing) * (i + 1);
                        pos += (_cellHeight + _spacing) * Mathf.CeilToInt((float)beforeCellCount / _crNum);
                        rowPos = _cellWight * (k % _crNum) + _spacing * (k % _crNum);
                        cellInfo.pos = new Vector3(rowPos, -pos, 0);
                    }
                    else
                    {
                        pos = _cellWight * Mathf.FloorToInt(k / _crNum) + _spacing * (Mathf.FloorToInt(k / _crNum) + 1);
                        pos += (_expandBtnWidth + _spacing) * (i + 1);
                        pos += (_cellWight + _spacing) * Mathf.CeilToInt((float)beforeCellCount / _crNum);
                        rowPos = _cellHeight * (k % _crNum) + _spacing * (k % _crNum);
                        cellInfo.pos = new Vector3(pos, -rowPos, 0);
                    }
                    //计算是否超出范围
                    float cellPos = _dir == Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                    if (IsOutRange(cellPos))
                    {
                        SetPoolsObj(cellInfo.obj);
                        cellInfo.obj = null;
                        _expandInfos[i].cellInfos[k] = cellInfo;
                        continue;
                    }
                    GameObject cell = cellInfo.obj != null ? cellInfo.obj : GetPoolsObj();
                    cell.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                    cell.gameObject.name = i + "-" + k;
                    //回调
                    if (cellInfo.obj == null)
                    {
                        _funcCallBackFunc(button, cell, i, k);
                    }
                    //添加按钮
                    Button cellButton = cell.GetComponent<Button>();
                    if (cellButton)
                    {
                        //
                        cellButton.onClick.AddListener(() => _funcOnClickCallBack(button, cell, i, k));
                    }
                    //存数据
                    cellInfo.obj = cell;
                    _expandInfos[i].cellInfos[k] = cellInfo;
                }
            }
            if (_expandInfos[i].isExpand)
            {
                beforeCellCount += count;
            }
        }
    }

    #region 对象池

    private Stack<GameObject> buttonPoolsObj = new Stack<GameObject>();
    //取出 button
    private GameObject GetPoolsButtonObj()
    {
        GameObject button = null;
        if (buttonPoolsObj.Count > 0)
        {
            button = buttonPoolsObj.Pop();
        }
        if (button == null)
        {
            button = Instantiate(_expandBtn) as GameObject;
        }
        button.transform.SetParent(_content);
        button.transform.localScale = Vector3.one;
        button.SetActive(true);
        return button;
    }
    //存入 button
    private void SetPoolsButtonObj(GameObject button)
    {
        if (button != null)
        {
            buttonPoolsObj.Push(button);
            button.SetActive(false);
        }
    }

    #endregion
}