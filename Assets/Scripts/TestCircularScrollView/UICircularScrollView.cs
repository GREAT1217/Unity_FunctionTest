using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Horizontal,
    Vertical
}

public class UICircularScrollView : MonoBehaviour
{
    public Direction _dir;
    public int _crNum = 1;//行或列
    public float _spacing = 5f;//间隔
    public GameObject _cell;

    protected RectTransform _rectTrans;
    protected RectTransform _content;
    protected float _contentWight;
    protected float _contentHeight;
    protected float _cellWight;
    protected float _cellHeight;

    protected bool _isInited;//

    protected struct CellInfo
    {
        public Vector3 pos;
        public GameObject obj;
    }
    protected CellInfo[] _cellInfos;
    protected ScrollRect _scrollRect;
    protected int _maxCount;
    protected int _headIndex;//首index
    protected int _tailIndex;//尾index
    protected bool _clearList;

    protected Action<GameObject, int> ShowCallback;//元素显示回调
    protected Action<GameObject, int> ClickCallback;//元素点击回调
    protected Action<int, bool, GameObject> OnButtonClickCallback;
    protected Stack<GameObject> poolsObj = new Stack<GameObject>();

    void Start()
    {
        Init(NormalCallBack);
        ShowList(50);
    }

    void Update()
    {

    }

    private void NormalCallBack(GameObject cell, int index)
    {
        cell.transform.Find("Text").GetComponent<Text>().text = index.ToString();
    }

    public virtual void Init(Action<GameObject, int> showCallback)
    {
        Init(showCallback, null);
    }

    public virtual void Init(Action<GameObject, int> showCallBack, Action<GameObject, int> clickCallback)
    {
        //cell
        if (_cell == null)
        {
            Debug.LogError("无元素");
            return;
        }
        SetPoolsObj(_cell);
        RectTransform cellRectTrans = _cell.GetComponent<RectTransform>();
        cellRectTrans.pivot = Vector2.up;
        SetTopLeftAnchor(cellRectTrans);
        _cellHeight = cellRectTrans.rect.height;
        _cellWight = cellRectTrans.rect.width;
        //scrollRect
        _scrollRect = GetComponent<ScrollRect>();
        //content
        _content = _scrollRect.content;
        _contentHeight = _content.rect.height;
        _contentWight = _content.rect.width;
        //rectTransform
        _rectTrans = _scrollRect.GetComponent<RectTransform>();
        //event
        _scrollRect.onValueChanged.RemoveAllListeners();
        _scrollRect.onValueChanged.AddListener(ScrollRectListener);
        ShowCallback = showCallBack;
        ClickCallback = clickCallback;
    }

    public virtual void Init(Action<GameObject, int> callback, Action<GameObject, int> onClickCallback, Action<int, bool, GameObject> onButtonClickCallback)
    {
        if (onButtonClickCallback != null)
        {
            OnButtonClickCallback = onButtonClickCallback;
        }
        Init(callback, onClickCallback);
    }

    public virtual void ShowList(int count)
    {
        _headIndex = _tailIndex = -1;
        //计算content尺寸
        if (_dir == Direction.Vertical)
        {
            float contentSize = (_spacing + _cellHeight) * Mathf.CeilToInt((float)count / _crNum);
            _contentHeight = contentSize;
            _contentWight = _content.sizeDelta.x;
            contentSize = Mathf.Max(contentSize, _rectTrans.rect.height);
            _content.sizeDelta = new Vector2(_contentWight, contentSize);
        }
        else
        {
            float contentSize = (_spacing + _cellWight) * Mathf.CeilToInt((float)count / _crNum);
            _contentWight = contentSize;
            _contentHeight = _content.sizeDelta.x;
            contentSize = Mathf.Max(contentSize, _rectTrans.rect.width);
            _content.sizeDelta = new Vector2(contentSize, _contentHeight);
        }
        //已经生成过的结束索引
        int lastEndIndex = 0;
        //如果已经初始化过，过多cell的保存对象池
        if (_isInited)
        {
            lastEndIndex = Mathf.Max(count, _maxCount);
            lastEndIndex = _clearList ? 0 : lastEndIndex;
            int cellCount = _clearList ? _cellInfos.Length : _maxCount;
            for (int i = lastEndIndex; i < cellCount; i++)
            {
                if (_cellInfos[i].obj != null)
                {
                    SetPoolsObj(_cellInfos[i].obj);
                    _cellInfos[i].obj = null;
                }
            }
        }
        //生成并记录元素
        CellInfo[] oldCellInfos = _cellInfos;//记录原来的元素
        _cellInfos = new CellInfo[count];
        for (int i = 0; i < count; i++)
        {
            if (_maxCount != -1 && i < lastEndIndex)//如果原来已生成过
            {
                Debug.Log("olddddd");
                CellInfo newCellInfo = oldCellInfos[i];
                float cellPos = _dir == Direction.Vertical ? newCellInfo.pos.y : newCellInfo.pos.x;
                if (!IsOutRange(cellPos))
                {
                    _headIndex = _headIndex == -1 ? i : _headIndex;
                    _tailIndex = i;
                    if (newCellInfo.obj == null)
                    {
                        newCellInfo.obj = GetPoolsObj();
                    }
                    newCellInfo.obj.GetComponent<RectTransform>().anchoredPosition = newCellInfo.pos;
                    newCellInfo.obj.name = i.ToString();
                    newCellInfo.obj.SetActive(true);
                    ShowCallback(newCellInfo.obj, i);
                }
                else
                {
                    SetPoolsObj(newCellInfo.obj);
                    newCellInfo.obj = null;
                }
                _cellInfos[i] = newCellInfo;
            }
            else//第一次生成或原来生成的不够
            {
                CellInfo cellInfo = new CellInfo();
                //计算每个cell的坐标
                if (_dir == Direction.Vertical)
                {
                    float posY = -(_cellHeight * Mathf.FloorToInt(i / _crNum) + _spacing * Mathf.FloorToInt(i / _crNum));
                    float posX = _cellWight * (i % _crNum) + _spacing * (i % _crNum);
                    cellInfo.pos = new Vector3(posX, posY, 0);
                }
                else
                {
                    float posX = _cellWight * Mathf.FloorToInt(i / _crNum) + _spacing * Mathf.FloorToInt(i / _crNum);
                    float PosY = -(_cellHeight * (i % _crNum) + _spacing * (i % _crNum));
                    cellInfo.pos = new Vector3(posX, PosY, 0);
                }
                //计算是否超出范围，显示或隐藏
                float cellPos = _dir == Direction.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.obj = null;
                }
                else
                {
                    GameObject cell = GetPoolsObj();
                    cell.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
                    cell.gameObject.name = i.ToString();
                    cellInfo.obj = cell;
                    //记录显示范围的首尾index
                    _headIndex = _headIndex == -1 ? i : _headIndex;
                    _tailIndex = i;
                    //回调
                    ShowCallback(cell, i);
                }
                _cellInfos[i] = cellInfo;
            }
        }
        _maxCount = count;
        _isInited = true;
    }

    protected virtual void ScrollRectListener(Vector2 value)
    {
        UpdateCheck();
    }

    /// <summary>
    /// 设置左上角锚点
    /// </summary>
    /// <param name="rectTrans"></param>
    protected void SetTopLeftAnchor(RectTransform rectTrans)
    {
        rectTrans.anchorMin = new Vector2(0, 1);
        rectTrans.anchorMax = new Vector2(0, 1);
    }

    /// <summary>
    /// 实时检测
    /// 检测显示隐藏
    /// </summary>
    protected void UpdateCheck()
    {
        if (_cellInfos == null) return;
        Debug.Log(_cellInfos.Length);
        for (int i = 0; i < _cellInfos.Length; i++)
        {
            CellInfo cell = _cellInfos[i];
            GameObject obj = cell.obj;
            Vector3 pos = cell.pos;
            float cellPos = _dir == Direction.Vertical ? pos.y : pos.x;
            if (IsOutRange(cellPos))
            {
                SetPoolsObj(obj);
                _cellInfos[i].obj = null;
            }
            else
            {
                if (obj == null)
                {
                    GameObject newCell = GetPoolsObj();
                    newCell.transform.localPosition = pos;
                    newCell.name = i.ToString();
                    _cellInfos[i].obj = newCell;
                    ShowCallback(newCell, i);
                }
            }
        }
    }

    /// <summary>
    /// 检测超出显示区域或整个区域
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    protected bool IsOutRange(float pos)
    {
        Vector3 contentPos = _content.anchoredPosition;
        if (_dir == Direction.Vertical)
        {
            if (pos + contentPos.y > _cellHeight || pos + contentPos.y < -_rectTrans.rect.height)//超出显示区域或超出整个区域
            {
                return true;
            }
        }
        else
        {
            if (pos + contentPos.x < -_cellWight || pos + contentPos.x > _rectTrans.rect.width)//超出显示区域或超出整个区域
            {
                return true;
            }
        }
        return false;
    }

    #region 对象池

    protected virtual void SetPoolsObj(GameObject obj)
    {
        if (obj != null)
        {
            poolsObj.Push(obj);
            obj.SetActive(false);
        }
    }

    protected virtual GameObject GetPoolsObj()
    {
        GameObject cell = null;
        if (poolsObj.Count > 0)
        {
            cell = poolsObj.Pop();
        }
        if (cell == null)
        {
            cell = Instantiate(_cell);
        }
        cell.transform.SetParent(_content);
        cell.gameObject.SetActive(true);
        return cell;
    }

    #endregion

    public void DisposeAll()
    {
        ShowCallback = null;
        ClickCallback = null;
    }

    void OnDestory()
    {
        DisposeAll();
    }
}