using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRadarGraphManager : MaskableGraphic
{
    public GraphData[] _datas;
    public int _rulingCount = 4;//刻度数
    public float _lineWidth = 1f;//背景线宽度
    public float _radarLineWidth = 1f;//雷达边框宽度
    public Color _lineColor = Color.gray;//背景线颜色
    public Color _radarLineColor = Color.blue;//雷达边框颜色
    public UIRadarImage _radarImage;//雷达图
    public Text _descPrefab;//描述Prefab
    public Transform _descContent;//描述Content
    public float _tweenTime = 1f;//动画事件

    private Vector2[] _vertexs;//顶点
    private float _radius;//半径
    private float _perRadian;//弧度
    private float _descSpace;//描述间隔
    private const string DESCPOOL = "RDescPool";

    protected override void Awake()
    {
        base.Awake();
        _radius = Mathf.Min(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y) / 2;
        _descSpace = Mathf.Max(_descPrefab.rectTransform.sizeDelta.x, _descPrefab.rectTransform.sizeDelta.y) / 2;
        ObjectPool.Instance.SetPrefab(DESCPOOL, _descPrefab.gameObject);
    }

    /// <summary>
    /// 刷新雷达图
    /// </summary>
    /// <param name="datas"></param>
    public void RefeshRadarGraph(GraphData[] datas)
    {
        _datas = datas;
        ClearTransform(_descContent);
        DrawDesc();
        SetAllDirty();//设置Layout布局、Vertices顶点和Material材质为Dirty；当一个Canvas被标记为包含需要被rebatch的几何图形，那这个Canvas被认为dirty。
        _radarImage.transform.localScale = Vector3.zero;
        _radarImage.Init(datas, _radius, _radarLineWidth, _radarLineColor);
        _radarImage.transform.DOScale(Vector3.one, _tweenTime);
    }

    /// <summary>
    /// UI生成顶点时调用
    /// </summary>
    /// <param name="vh"></param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (_datas == null || _datas.Length <= 2)//不可能存在边树小于三的多边形
        {
            base.OnPopulateMesh(vh);
            return;
        }
        vh.Clear();
        DrawAxis(vh);
        DrawRuling(vh);
    }

    /// <summary>
    /// 画坐标轴
    /// </summary>
    /// <param name="vh"></param>
    private void DrawAxis(VertexHelper vh)
    {
        GetVertexs();
        for (int i = 0; i < _vertexs.Length; i++)
        {
            vh.AddUIVertexQuad(GetQuad(Vector2.zero, _vertexs[i], _lineColor, _lineWidth));
        }
    }

    /// <summary>
    /// 画刻度
    /// </summary>
    private void DrawRuling(VertexHelper vh)
    {
        float perRadius = _radius / (_rulingCount - 1);//原点不需要画
        for (int i = 1; i < _rulingCount; i++)
        {
            for (int j = 0; j < _datas.Length; j++)
            {
                float startRadian = _perRadian * j + 90 * Mathf.Deg2Rad;
                float endRadian = _perRadian * (j + 1) + 90 * Mathf.Deg2Rad;
                Vector2 startPos = new Vector2(Mathf.Cos(startRadian), Mathf.Sin(startRadian)) * perRadius * i;
                Vector2 endPos = new Vector2(Mathf.Cos(endRadian), Mathf.Sin(endRadian)) * perRadius * i;
                UIVertex[] newVertexs = GetQuad(startPos, endPos, _lineColor, _lineWidth);
                vh.AddUIVertexQuad(newVertexs);
            }
        }
    }

    /// <summary>
    /// 描述
    /// </summary>
    private void DrawDesc()
    {
        GetVertexs();
        for (int i = 0; i < _vertexs.Length; i++)
        {
            Text desc = ObjectPool.Instance.GetObject(DESCPOOL, _descContent).GetComponent<Text>();
            desc.text = _datas[i]._desc;
            Vector2 pos = _vertexs[i];
            if (Mathf.Abs(pos.x) >= 0.1f)
                pos.x += _descSpace * (pos.x > 0 ? 1 : -1);
            if (Mathf.Abs(pos.y) >= 0.1f)
                pos.y += _descSpace * (pos.y > 0 ? 1 : -1);
            desc.rectTransform.localPosition = pos;
            desc.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 获取顶点
    /// </summary>
    /// <returns></returns>
    private void GetVertexs()
    {
        _perRadian = Mathf.PI * 2 / _datas.Length;
        _vertexs = new Vector2[_datas.Length];
        for (int i = 0; i < _datas.Length; i++)
        {
            float radian = _perRadian * i + 90 * Mathf.Deg2Rad;
            Vector2 endPos = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * _radius;
            _vertexs[i] = endPos;
        }
    }

    /// <summary>
    /// 获取一条线的四个顶点
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private UIVertex[] GetQuad(Vector2 startPos, Vector2 endPos, Color color, float width)
    {
        float dis = Vector2.Distance(startPos, endPos);
        float x = width / 2 * (endPos.y - startPos.y) / dis;//sin
        float y = width / 2 * (endPos.x - startPos.x) / dis;//cos
        if (y <= 0) y = -y;
        else x = -x;
        UIVertex[] vertex = new UIVertex[4];
        vertex[0].position = new Vector3(startPos.x + x, startPos.y + y);
        vertex[1].position = new Vector3(endPos.x + x, endPos.y + y);
        vertex[2].position = new Vector3(endPos.x - x, endPos.y - y);
        vertex[3].position = new Vector3(startPos.x - x, startPos.y - y);
        for (int i = 0; i < vertex.Length; i++)
            vertex[i].color = color;
        return vertex;
    }

    /// <summary>
    /// 入池
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="pool"></param>
    private void ClearTransform(Transform parent)
    {
        for (int i = 1; i < parent.childCount; i++)
        {
            ObjectPool.Instance.RecycleObj(parent.GetChild(i).gameObject, parent);
        }
    }

}
