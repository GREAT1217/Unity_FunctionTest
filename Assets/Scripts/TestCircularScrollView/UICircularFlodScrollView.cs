using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICircularFlodScrollView : UICircularScrollView
{
    public GameObject _flodBtn;//折叠按钮
    public bool _defaultFlod = true;//默认折叠

    private const string FLODBTNPOOL = "FlodBtnPool";
    private float _flodBtnX;
    private float _flodBtnY;
    private float _flodBtnWidth;
    private float _flodBtnHeight;
    /// <summary>
    /// 折叠按钮信息
    /// </summary>
    private struct FlodBtnInfo
    {
        public int index;
        public GameObject obj;
        public bool isFlod;
        public CellInfo[] cellInfos;
        public float cellsSize;//子元素横向或纵尺寸
        public int cellCount;
    }
    private FlodBtnInfo[] _flodBtnInfos;

    private UnityAction<GameObject, int> OnFlodBtnShow;
    private UnityAction<GameObject, int, int> OnFlodCellShow;
    private UnityAction<int, bool, GameObject> OnFlodBtnClick;

    public void Init(UnityAction<GameObject, int> onFlodBtnShow, UnityAction<GameObject, int, int> onFlodCellShow)
    {
        if (_flodBtn == null)
        {
            Debug.LogError("没有折叠按钮");
            return;
        }
        OnFlodBtnShow = onFlodBtnShow;
        OnFlodCellShow = onFlodCellShow;
        RectTransform expandRectTrans = _flodBtn.GetComponent<RectTransform>();
        SetTopLeftAnchor(expandRectTrans);
        _flodBtnX = expandRectTrans.anchoredPosition.x;
        _flodBtnY = expandRectTrans.anchoredPosition.y;
        _flodBtnWidth = expandRectTrans.rect.width;
        _flodBtnHeight = expandRectTrans.rect.height;
        ObjectPool.Instance.SetPrefab(FLODBTNPOOL, _flodBtn);
        ObjectPool.Instance.RecycleObj(FLODBTNPOOL, _flodBtn);
        base.Init(null);
    }

    public void ShowList(params int[] num)
    {
        RecycleItems();
        int curCellCount = 0;//当前元素数量
        int cellCount = 0;//元素总数量
        int flodBtnCount = num.Length;//折叠按钮总数量
        _flodBtnInfos = new FlodBtnInfo[flodBtnCount];
        for (int k = 0; k < flodBtnCount; k++)
        {
            //====生成折叠按钮
            GameObject flodObj = ObjectPool.Instance.GetObject(FLODBTNPOOL, _content);
            flodObj.SetActive(true);
            flodObj.name = k.ToString();
            int count = num[k];
            cellCount += count;
            //存储按钮数据
            FlodBtnInfo flodBtnInfo = new FlodBtnInfo
            {
                index = k,
                obj = flodObj,
                cellCount = count,
                cellInfos = new CellInfo[count],
                isFlod = _defaultFlod,
                cellsSize = _dir == UIDir.Vertical ? (_cellHeight + _spacing) * Mathf.CeilToInt((float)count / _crNum) : (_cellWight + _spacing) * Mathf.CeilToInt((float)count / _crNum)
            };
            _flodBtnInfos[k] = flodBtnInfo;
            Button btn = flodObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnFlodClick(flodBtnInfo));
            SetFlodBtnPos(flodBtnInfo, curCellCount);
            //=====生成cell
            for (int i = 0; i < count; i++)
            {
                if (flodBtnInfo.isFlod) break;
                CellInfo cellInfo = new CellInfo();
                SetCellPos(ref cellInfo, k, i, curCellCount);
                if (SetCellState(ref cellInfo))
                {
                    cellInfo.obj.name = k + "-" + i;
                    OnFlodCellShow(cellInfo.obj, i, k);//cell显示回调
                }
                flodBtnInfo.cellInfos[i] = cellInfo;
            }
            curCellCount += flodBtnInfo.isFlod ? 0 : count;
            OnFlodBtnShow(flodObj, k);//flodBtn显示回调
        }
        SetContentSize(flodBtnCount, cellCount);
    }

    protected override void ScrollRectListener(Vector2 value)
    {
        if (_flodBtnInfos == null) return;
        for (int i = 0; i < _flodBtnInfos.Length; i++)
        {
            FlodBtnInfo flodBtnInfo = _flodBtnInfos[i];
            if (flodBtnInfo.isFlod) continue;
            for (int k = 0; k < flodBtnInfo.cellCount; k++)
            {
                CellInfo cellInfo = flodBtnInfo.cellInfos[k];
                if (SetCellState(ref cellInfo))
                {
                    OnFlodCellShow(cellInfo.obj, i, k);//cell显示回调
                    cellInfo.obj.name = i + "-" + k;
                }
                flodBtnInfo.cellInfos[k] = cellInfo;
            }
        }
    }

    private void RecycleItems()
    {
        if (!_isInited) return;
        for (int i = 0; i < _flodBtnInfos.Length; i++)
        {
            if (_flodBtnInfos[i].obj != null)
            {
                ObjectPool.Instance.RecycleObj(FLODBTNPOOL, _flodBtnInfos[i].obj);
                _flodBtnInfos[i].obj = null;
            }
            for (int j = 0; j < _flodBtnInfos[i].cellInfos.Length; j++)
            {
                if (_flodBtnInfos[i].cellInfos[j].obj != null)
                {
                    ObjectPool.Instance.RecycleObj(CELLPOOL, _flodBtnInfos[i].cellInfos[j].obj);
                    _flodBtnInfos[i].cellInfos[j].obj = null;
                }
            }
        }
    }

    private void SetFlodBtnPos(FlodBtnInfo flod, int curCellCount)
    {
        if (_dir == UIDir.Vertical)
        {
            float pos =  _spacing;
            if (flod.index > 0)
            {
                pos += _cellHeight;
                pos += Mathf.Abs(_flodBtnInfos[flod.index - 1].obj.transform.localPosition.y);
                pos += _flodBtnInfos[flod.index - 1].isFlod ? 0 : _flodBtnInfos[flod.index - 1].cellsSize;
            }
            flod.obj.transform.localPosition = new Vector2(_flodBtnX, -pos);
        }
        else
        {
            float pos = _flodBtnWidth * flod.index + _spacing * (flod.index + 1);
            pos += flod.index > 0 ? (_cellWight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum) : 0;
            flod.obj.transform.localPosition = new Vector2(pos, _flodBtnY);
        }
    }

    private void SetCellPos(ref CellInfo cellInfo, int flodIndex, int cellIndex, int curCellCount)
    {
        if (_dir == UIDir.Vertical)
        {
            float posY = (_cellHeight + _spacing) * (Mathf.CeilToInt(cellIndex / _crNum) + 1) + Mathf.Abs(_flodBtnInfos[flodIndex].obj.transform.localPosition.y);
            //posY += (_flodBtnHeight + _spacing) * (flodIndex + 1);
            //posY += (_cellHeight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum);
            //posY += Mathf.Abs(_flodBtnInfos[flodIndex].obj.transform.localPosition.y) + _cellHeight;
            float posX = (_cellWight + _spacing) * (cellIndex % _crNum);
            cellInfo.pos = new Vector2(posX, -posY);
        }
        else
        {
            float pos = _cellWight * Mathf.CeilToInt(cellIndex / _crNum) + _spacing * (Mathf.CeilToInt(cellIndex / _crNum) + 1);
            pos += (_flodBtnWidth + _spacing) * (flodIndex + 1);
            pos += (_cellHeight + _spacing) * Mathf.CeilToInt((float)curCellCount / _crNum);
            float posY = _cellHeight * (cellIndex % _crNum) + _spacing * (cellIndex % _crNum);
            cellInfo.pos = new Vector2(pos, -posY);
        }
    }

    private void SetContentSize(int flodBtnCount, int cellCount)
    {
        if (_dir == UIDir.Vertical)
        {
            float contentSize = Mathf.Abs(_flodBtnInfos[flodBtnCount - 1].obj.transform.localPosition.y) + _flodBtnHeight + _spacing;
            //  float contentSize = (_spacing + _flodBtnHeight) * flodBtnCount;
            contentSize += _defaultFlod ? 0 : _flodBtnInfos[flodBtnCount - 1].cellsSize;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, contentSize);
        }
        else
        {
            float contentSize = _defaultFlod ? 0 : (_spacing + _cellWight) * Mathf.CeilToInt((float)cellCount / _crNum);
            contentSize += (_spacing + _flodBtnWidth) * flodBtnCount;
            _content.sizeDelta = new Vector2(contentSize, _content.sizeDelta.y);
        }
    }

    private void SetContentSize(FlodBtnInfo flod)
    {
        Vector2 contentSize = _content.sizeDelta;
        if (_dir == UIDir.Vertical)
        {
            float height = flod.isFlod ? contentSize.y - flod.cellsSize : contentSize.y + flod.cellsSize;
            _content.sizeDelta = new Vector2(contentSize.x, height);
        }
        else
        {
            float width = flod.isFlod ? contentSize.x - flod.cellsSize : contentSize.x + flod.cellsSize;
            _content.sizeDelta = new Vector2(width, contentSize.y);
        }
    }

    private void OnFlodClick(FlodBtnInfo flod)
    {
        OnFlodClick(flod.index);
        if (OnFlodBtnClick != null)
        {
            OnFlodBtnClick(flod.index, flod.isFlod, flod.obj);
        }
    }

    private void OnFlodClick(int index)
    {
        _flodBtnInfos[index].isFlod = !_flodBtnInfos[index].isFlod;
        //重置Content大小
        SetContentSize(_flodBtnInfos[index]);
        int curCellCount = 0;
        //下面的折叠按钮，重新计算坐标显示处理
        for (int i = 0; i < _flodBtnInfos.Length; i++)
        {
            FlodBtnInfo flodBtnInfo = _flodBtnInfos[i];
            int count = flodBtnInfo.cellCount;
            if (i >= index)
            {
                SetFlodBtnPos(_flodBtnInfos[i], curCellCount);
                for (int k = 0; k < count; k++)
                {
                    if (flodBtnInfo.isFlod)
                    {
                        if (flodBtnInfo.cellInfos[k].obj != null)
                        {
                            ObjectPool.Instance.RecycleObj(CELLPOOL, flodBtnInfo.cellInfos[k].obj);
                            _flodBtnInfos[i].cellInfos[k].obj = null;
                        }
                    }
                    else
                    {
                        CellInfo cellInfo = flodBtnInfo.cellInfos[k];
                        SetCellPos(ref cellInfo, i, k, curCellCount);
                        if (SetCellState(ref cellInfo))
                        {
                            OnFlodCellShow(cellInfo.obj, i, k);//cell显示回调
                            cellInfo.obj.name = i + "-" + k;
                        }
                        _flodBtnInfos[i].cellInfos[k] = cellInfo;
                    }
                }
            }
            if (!_flodBtnInfos[i].isFlod)
            {
                curCellCount += count;
            }
        }
    }
}