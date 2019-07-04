using UnityEngine;
using UnityEngine.UI;

public class UIRadarImage : MaskableGraphic
{
    [HideInInspector]
    public GraphData[] _datas;
    [HideInInspector]
    public float _radius;
    [HideInInspector]
    public float _lineWidth;
    [HideInInspector]
    public Color _lineColor;

    private float _perRadian;//弧度

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="datas"></param>
    /// <param name="radius"></param>
    /// <param name="lineWidth"></param>
    /// <param name="lineColor"></param>
    public void Init(GraphData[] datas, float radius, float lineWidth, Color lineColor)
    {
        _datas = datas;
        _radius = radius;
        _lineWidth = lineWidth;
        _lineColor = lineColor;
        SetAllDirty();//设置Layout布局、Vertices顶点和Material材质为Dirty；我认为是重新绘制
    }

    /// <summary>
    /// 填充网格
    /// </summary>
    /// <param name="vh"></param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (_datas == null || _datas.Length <= 2)//不可能存在边数小于三的多边形
        {
            base.OnPopulateMesh(vh);
            return;
        }
        vh.Clear();
        _perRadian = Mathf.PI * 2 / _datas.Length;
        DrawRadar(vh);
        DrawLine(vh);
    }

    /// <summary>
    /// 画雷达图
    /// </summary>
    /// <param name="vh"></param>
    private void DrawRadar(VertexHelper vh)
    {
        int edgeCount = _datas.Length;//边数量
        //画雷达三角面
        for (int i = 0; i < edgeCount; i++)
        {
            DrawTriangle(vh, GetVertex(i), i);
        }
    }

    /// <summary>
    /// 画雷达图边框
    /// </summary>
    /// <param name="vh"></param>
    private void DrawLine(VertexHelper vh)
    {
        int edgeCount = _datas.Length;//边数量
        //画雷达三角面
        for (int i = 0; i < edgeCount; i++)
        {
            DrawLine(vh, GetVertex(i));
        }
    }

    /// <summary>
    /// 画三角面
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="index"></param>
    /// <param name="deltaAngle"></param>
    private void DrawTriangle(VertexHelper vh, Vector3[] poses, int index)
    {
        Color color = _lineColor;
        color.a = 0.5f;

        vh.AddVert(Vector3.zero, color, Vector2.zero);//中心点
        vh.AddVert(poses[0], color, Vector2.zero);
        vh.AddVert(poses[1], color, Vector2.zero);//UI的法线可以随便设置
        vh.AddTriangle(index * 3, index * 3 + 1, index * 3 + 2);//将三角面加入UI绘制缓冲区。参数是三角面的三个顶点索引//所以绘制n边形需要绘制3n的顶点
    }

    /// <summary>
    /// 画线
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="index"></param>
    private void DrawLine(VertexHelper vh, Vector3[] poses)
    {
        //画线
        UIVertex[] newVertexs = GetQuad(poses[0], poses[1], _lineColor, _lineWidth);
        vh.AddUIVertexQuad(newVertexs);
    }

    /// <summary>
    /// 获取一个弧度的两个顶点
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3[] GetVertex(int index)
    {
        int nextIndex = index + 1 >= _datas.Length ? 0 : index + 1;
        float radian1 = index * _perRadian + 90 * Mathf.Deg2Rad;
        float radian2 = nextIndex * _perRadian + 90 * Mathf.Deg2Rad;
        float radius1 = _datas[index].Rate * _radius;
        float radius2 = _datas[nextIndex].Rate * _radius;
        //两边顶点
        Vector3 p1 = new Vector3(radius1 * Mathf.Cos(radian1), radius1 * Mathf.Sin(radian1));
        Vector3 p2 = new Vector3(radius2 * Mathf.Cos(radian2), radius2 * Mathf.Sin(radian2));
        return new Vector3[] { p1, p2 };
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
