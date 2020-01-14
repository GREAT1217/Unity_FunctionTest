using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICircularScrollView : MonoBehaviour
{
    public UIDir _dir;
    public int _crNum = 1;//行或列
    public float _spacing = 5f;//间隔
    public GameObject _cell;

    protected const string CELLPOOL = "CellPool";
    protected ScrollRect _scrollRect;
    protected RectTransform _rectTrans;
    protected RectTransform _content;
    protected float _contentWight;
    protected float _contentHeight;
    protected float _cellWight;
    protected float _cellHeight;
    /// <summary>
    /// 子元素数据
    /// </summary>
    protected struct CellInfo
    {
        public Vector2 pos;
        public GameObject obj;
    }
    protected CellInfo[] _cellInfos;
    protected int _curCount = -1;//当前初始化时Cell最大数量
    protected bool _isInited;//记录首次初始化
    protected bool _clearList;//重新初始化时是否清理Cell数据
    protected UnityAction<GameObject, int> OnCellShow;//Cell显示回调

    /// <summary>
    /// 初始化子元素显示回调函数
    /// </summary>
    /// <param name="onCellShow"></param>
    public virtual void Init(UnityAction<GameObject, int> onCellShow)
    {
        //cell
        if (_cell == null)
        {
            Debug.LogError("没有子元素");
            return;
        }
        RectTransform cellRectTrans = _cell.GetComponent<RectTransform>();
        SetTopLeftAnchor(cellRectTrans);
        _cellHeight = cellRectTrans.rect.height;
        _cellWight = cellRectTrans.rect.width;
        ObjectPool.Instance.SetPrefab(CELLPOOL, _cell);
        ObjectPool.Instance.RecycleObj(CELLPOOL, _cell);
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
        OnCellShow = onCellShow;
    }

    /// <summary>
    /// 显示列表
    /// </summary>
    /// <param name="count"></param>
    public virtual void ShowList(int count)
    {
        SetContentSize(count);
        int lastCount = RecycleCellInfos(count);//上次初始化的cell数量
        CellInfo[] oldCellInfos = _cellInfos;//记录上次初始化的cell
        _cellInfos = new CellInfo[count];//记录本次初始化的cell
        for (int i = 0; i < count; i++)
        {
            CellInfo cellInfo;
            if (_curCount != -1 && i < lastCount)//如果初始化过，复用原来的cell数据存储
            {
                cellInfo = oldCellInfos[i];
            }
            else//第一次初始化或原来的cell数据的不够
            {
                cellInfo = new CellInfo();
                SetCellPos(ref cellInfo, i);
            }
            if (SetCellState(ref cellInfo))
            {
                OnCellShow(cellInfo.obj, i);//cell显示回调
            }
            _cellInfos[i] = cellInfo;
        }
        _curCount = count;
        _isInited = true;
    }

    /// <summary>
    /// 监听滑动区域
    /// </summary>
    /// <param name="value"></param>
    protected virtual void ScrollRectListener(Vector2 value)
    {
        if (_cellInfos == null) return;
        for (int i = 0; i < _cellInfos.Length; i++)
        {
            if (SetCellState(ref _cellInfos[i]))
            {
                OnCellShow(_cellInfos[i].obj, i);
            }
        }
    }

    /// <summary>
    /// 设置Content尺寸
    /// </summary>
    /// <param name="count"></param>
    protected virtual void SetContentSize(int count)
    {
        if (_dir == UIDir.Vertical)
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
    }

    /// <summary>
    /// 设置RectTransform左上角锚点
    /// </summary>
    /// <param name="rectTrans"></param>
    protected void SetTopLeftAnchor(RectTransform rectTrans)
    {
        rectTrans.pivot = Vector2.up;
        rectTrans.anchorMin = Vector2.up;
        rectTrans.anchorMax = Vector2.up;
    }

    /// <summary>
    /// 回收CellInfos
    /// </summary>
    /// <returns></returns>
    protected int RecycleCellInfos(int count)
    {
        if (!_isInited) return 0;
        if (count > _curCount) return _curCount;
        int overIndex = Mathf.Min(count, _curCount);//如果上次的多了，将多出来的回收；如果上次的不足，不需要回收
        overIndex = _clearList ? 0 : overIndex;//如果要全清，从0开始，否则只回收多余的
        for (int i = overIndex; i < _curCount; i++)
        {
            if (_cellInfos[i].obj != null)
            {
                Debug.Log("回收");
                ObjectPool.Instance.RecycleObj(CELLPOOL, _cellInfos[i].obj);
                _cellInfos[i].obj = null;
            }
        }
        return overIndex;
    }

    /// <summary>
    /// 设置Cell位置信息
    /// </summary>
    /// <param name="cellInfo"></param>
    /// <param name="index"></param>
    protected void SetCellPos(ref CellInfo cellInfo, int index)
    {
        if (_dir == UIDir.Vertical)
        {
            float posY = -(_cellHeight * Mathf.CeilToInt(index / _crNum) + _spacing * Mathf.CeilToInt(index / _crNum));
            float posX = _cellWight * (index % _crNum) + _spacing * (index % _crNum);
            cellInfo.pos = new Vector2(posX, posY);
        }
        else
        {
            float posX = _cellWight * Mathf.CeilToInt(index / _crNum) + _spacing * Mathf.CeilToInt(index / _crNum);
            float PosY = -(_cellHeight * (index % _crNum) + _spacing * (index % _crNum));
            cellInfo.pos = new Vector2(posX, PosY);
        }
    }

    /// <summary>
    /// 设置Cell显示状态
    /// </summary>
    /// <param name="cellInfo"></param>
    /// <returns></returns>
    protected bool SetCellState(ref CellInfo cellInfo)
    {
        if (IsOutRange(cellInfo))
        {
            if (cellInfo.obj != null)
            {
                ObjectPool.Instance.RecycleObj(CELLPOOL, cellInfo.obj);
            }
            cellInfo.obj = null;
            return false;
        }
        else
        {
            if (cellInfo.obj == null)
            {
                cellInfo.obj = ObjectPool.Instance.GetObject(CELLPOOL, _content);
            }
            //cellInfo.entity.gameObject.GetComponent<RectTransform>().anchoredPosition = cellInfo.pos;
            cellInfo.obj.transform.localPosition = cellInfo.pos;
            cellInfo.obj.SetActive(true);
            return true;
        }
    }

    /// <summary>
    /// 检测超出显示区域或整个区域
    /// </summary>
    /// <param name="cellInfo"></param>
    /// <returns></returns>
    protected bool IsOutRange(CellInfo cellInfo)
    {
        float pos = _dir == UIDir.Vertical ? cellInfo.pos.y : cellInfo.pos.x;
        Vector3 contentPos = _content.anchoredPosition;
        if (_dir == UIDir.Vertical)
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
}