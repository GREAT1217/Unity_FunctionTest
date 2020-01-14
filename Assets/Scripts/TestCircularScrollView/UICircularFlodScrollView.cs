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
        public GameObject obj;
        public bool isFlod;
        public CellInfo[] cellInfos;
        public float cellsSize;//子元素横向或纵尺寸
    }
    private FlodBtnInfo[] _flodBtnInfos;

    private UnityAction<GameObject, int> OnFlodBtnShow;
    private UnityAction<GameObject, int, int> OnFlodCellShow;
    private UnityAction<GameObject, int, bool> OnFlodBtnClick;

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
        int flodBtnCount = num.Length;
        _flodBtnInfos = new FlodBtnInfo[flodBtnCount];
        for (int i = 0; i < flodBtnCount; i++)
        {
            //====生成折叠按钮
            GameObject flodObj = ObjectPool.Instance.GetObject(FLODBTNPOOL, _content);
            flodObj.SetActive(true);
            flodObj.name = i.ToString();
            int count = num[i];
            //存储按钮数据
            FlodBtnInfo flodBtnInfo = new FlodBtnInfo
            {
                obj = flodObj,
                cellInfos = new CellInfo[count],
                isFlod = _defaultFlod,
                cellsSize = _dir == UIDir.Vertical ? (_cellHeight + _spacing) * Mathf.CeilToInt((float)count / _crNum) : (_cellWight + _spacing) * Mathf.CeilToInt((float)count / _crNum)
            };
            _flodBtnInfos[i] = flodBtnInfo;
            int flodIndex = i;
            flodObj.GetComponent<Button>().onClick.AddListener(() => OnFlodClick(flodBtnInfo, flodIndex));
            SetFlodBtnPos(i);
            //=====生成cell
            for (int k = 0; k < count; k++)
            {
                if (flodBtnInfo.isFlod) break;
                CellInfo cellInfo = new CellInfo();
                SetCellPos(ref cellInfo, i, k);
                if (SetCellState(ref cellInfo))
                {
                    cellInfo.obj.name = i + "-" + k;
                    OnFlodCellShow(cellInfo.obj, k, i);//cell显示回调
                }
                flodBtnInfo.cellInfos[k] = cellInfo;
            }
            OnFlodBtnShow(flodObj, i);//flodBtn显示回调
        }
        SetContentSize(flodBtnCount);
    }

    protected override void ScrollRectListener(Vector2 value)
    {
        if (_flodBtnInfos == null) return;
        for (int i = 0; i < _flodBtnInfos.Length; i++)
        {
            FlodBtnInfo flodBtnInfo = _flodBtnInfos[i];
            if (flodBtnInfo.isFlod) continue;
            for (int k = 0; k < flodBtnInfo.cellInfos.Length; k++)
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

    protected override void SetContentSize(int flodBtnCount)
    {
        int lastIndex = flodBtnCount - 1;
        if (_dir == UIDir.Vertical)
        {
            float sizeY = -_flodBtnInfos[lastIndex].obj.transform.localPosition.y + _flodBtnHeight + _spacing;
            sizeY += _defaultFlod ? 0 : _flodBtnInfos[lastIndex].cellsSize;
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, sizeY);
        }
        else
        {
            float sizeX = _flodBtnInfos[lastIndex].obj.transform.localPosition.x + _flodBtnWidth + _spacing;
            sizeX += _defaultFlod ? 0 : _flodBtnInfos[lastIndex].cellsSize;
            _content.sizeDelta = new Vector2(sizeX, _content.sizeDelta.y);
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

    private void SetFlodBtnPos(int index)
    {
        if (_dir == UIDir.Vertical)
        {
            float posY = -_spacing;
            if (index > 0)
            {
                posY -= _cellHeight;
                posY += _flodBtnInfos[index - 1].obj.transform.localPosition.y;
                posY -= _flodBtnInfos[index - 1].isFlod ? 0 : _flodBtnInfos[index - 1].cellsSize;
            }
            _flodBtnInfos[index].obj.transform.localPosition = new Vector2(_flodBtnX, posY);
        }
        else
        {
            float posX = _spacing;
            if (index > 0)
            {
                posX += _cellWight;
                posX += _flodBtnInfos[index - 1].obj.transform.localPosition.x;
                posX += _flodBtnInfos[index - 1].isFlod ? 0 : _flodBtnInfos[index - 1].cellsSize;
            }
            _flodBtnInfos[index].obj.transform.localPosition = new Vector2(posX, _flodBtnY);
        }
    }

    private void SetCellPos(ref CellInfo cellInfo, int flodIndex, int cellIndex)
    {
        if (_dir == UIDir.Vertical)
        {
            float posX = (_cellWight + _spacing) * (cellIndex % _crNum);
            float posY = -(_cellHeight + _spacing) * (Mathf.CeilToInt(cellIndex / _crNum) + 1);
            posY += _flodBtnInfos[flodIndex].obj.transform.localPosition.y;
            cellInfo.pos = new Vector2(posX, posY);
        }
        else
        {
            float posX = (_cellWight + _spacing) * (Mathf.CeilToInt(cellIndex / _crNum) + 1);
            posX += _flodBtnInfos[flodIndex].obj.transform.localPosition.x;
            float posY = -(_cellHeight + _spacing) * (cellIndex % _crNum);
            cellInfo.pos = new Vector2(posX, posY);
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

    private void OnFlodClick(FlodBtnInfo flod,int index)
    {
        OnFlodClick(index);
        if (OnFlodBtnClick != null)
        {
            OnFlodBtnClick(flod.obj, index, flod.isFlod);
        }
    }

    private void OnFlodClick(int index)
    {
        _flodBtnInfos[index].isFlod = !_flodBtnInfos[index].isFlod;
        //重置Content大小
        SetContentSize(_flodBtnInfos[index]);
        //下面的折叠按钮，重新计算坐标显示处理
        for (int i = 0; i < _flodBtnInfos.Length; i++)
        {
            FlodBtnInfo flodBtnInfo = _flodBtnInfos[i];
            int count = flodBtnInfo.cellInfos.Length;
            if (i >= index)
            {
                SetFlodBtnPos(i);
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
                        SetCellPos(ref cellInfo, i, k);
                        if (SetCellState(ref cellInfo))
                        {
                            OnFlodCellShow(cellInfo.obj, i, k);//cell显示回调
                            cellInfo.obj.name = i + "-" + k;
                        }
                        _flodBtnInfos[i].cellInfos[k] = cellInfo;
                    }
                }
            }
        }
    }
}