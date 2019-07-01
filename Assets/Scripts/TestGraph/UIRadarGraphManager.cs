using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRadarGraphManager : MaskableGraphic
{
    public GraphData[] _datas;
    public int _rulingCount = 3;//刻度数
    public float _lineWidth = 1f;//背景线宽度
    public float _radarLineWidth = 1f;//雷达边框宽度
    public Color _lineColor = Color.gray;//背景线颜色
    public Color _radarLineColor = Color.blue;//雷达边框颜色
    public UIRadarImage _radarImage;//雷达图
    public float _tweenTime = 1f;//动画事件

    private float _radius;//半径
    private float _perRadian;//弧度

    protected override void Awake()
    {
        base.Awake();
        _radius = Mathf.Min(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y) / 2;
    }

    public void InitRadarGraph(GraphData[] datas)
    {
        //描述
        RefeshRadarGraph(datas);
    }

    /// <summary>
    /// 刷新雷达图
    /// </summary>
    /// <param name="datas"></param>
    public void RefeshRadarGraph(GraphData[] datas)
    {
        _datas = datas;
        _radarImage.transform.localScale = Vector3.zero;
        _radarImage.Init(datas, _radius, _radarLineWidth, _radarLineColor);
        _radarImage.transform.DOScale(Vector3.one, _tweenTime);
    }

    /// <summary>
    /// 填充网格
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
        _perRadian = Mathf.PI * 2 / _datas.Length;
        DrawAxis(vh);
        DrawRuling(vh);
    }

    /// <summary>
    /// 画刻度
    /// </summary>
    private void DrawRuling(VertexHelper vh)
    {
        float perRadius = _radius / _rulingCount;
        for (int i = 0; i < _rulingCount; i++)
        {
            for (int j = 0; j < _datas.Length; j++)
            {
                float startRadian = _perRadian * j;
                float endRadian = _perRadian * (j + 1);
                Vector2 startPos = new Vector2(Mathf.Cos(startRadian), Mathf.Sin(startRadian)) * perRadius * (i + 1);
                Vector2 endPos = new Vector2(Mathf.Cos(endRadian), Mathf.Sin(endRadian)) * perRadius * (i + 1);
                UIVertex[] newVertexs = GetQuad(startPos, endPos, _lineColor, _lineWidth);
                vh.AddUIVertexQuad(newVertexs);
            }
        }
    }

    /// <summary>
    /// 画坐标轴
    /// </summary>
    /// <param name="vh"></param>
    private void DrawAxis(VertexHelper vh)
    {
        for (int i = 0; i < _datas.Length; i++)
        {
            float radian = _perRadian * i;
            Vector2 endPos = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * _radius;
            vh.AddUIVertexQuad(GetQuad(Vector2.zero, endPos, _lineColor, _lineWidth));
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

}
