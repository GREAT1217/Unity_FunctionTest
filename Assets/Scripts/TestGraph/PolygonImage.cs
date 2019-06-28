using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PolygonImage : MaskableGraphic
{
    //[SerializeField]
    //Texture _texture;
    //public override Texture mainTexture
    //{
    //    get
    //    {
    //        if (_texture == null)
    //        {
    //            if (material != null && material.mainTexture != null)
    //            {
    //                return material.mainTexture;
    //            }
    //            return s_WhiteTexture;
    //        }

    //        return _texture;
    //    }
    //}
    //public Texture Texture
    //{
    //    get
    //    {
    //        return _texture;
    //    }
    //    set
    //    {
    //        if (_texture == value)
    //            return;

    //        _texture = value;
    //        SetVerticesDirty();
    //        SetMaterialDirty();//重绘 参考：https://www.jianshu.com/p/25df7841ae1e
    //    }
    //}
    public GraphData[] _datas;
    public Color _lineColor;
    public float _lineWidth;

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
        int edgeCount = _datas.Length;//边数量
        float deltaAngle = 360f / edgeCount;//角度

        vh.Clear();

        //画雷达三角面
        for (int i = 0; i < edgeCount; i++)
        {
            DrawTriangle(vh, i, deltaAngle);
        }
    }

    private void DrawBG(VertexHelper vh)
    {
        //环

        //对角线

    }

    private void DrawAxis(VertexHelper vh)
    {
        Rect rect = rectTransform.rect;
        Vector2 startPosX = Vector2.zero - Vector2.up * rect.x / 2;
        Vector2 endPosX = startPosX + Vector2.up * rect.x;
        Vector2 startPosY = Vector2.zero - Vector2.right * rect.y / 2;
        Vector2 endPosY = startPosY - Vector2.right * rect.y;
        //vh.AddUIVertexQuad()
    }


    private void DrawQuad(Vector2 startPos,Vector2 endPos)
    {

    }

    /// <summary>
    /// 画三角面
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="index"></param>
    /// <param name="deltaAngle"></param>
    private void DrawTriangle(VertexHelper vh, int index, float deltaAngle)
    {
        float angle1 = 90 + (index + 1) * deltaAngle;//+90是为了把起始位置方向从Vector2.Right转到Vector2.Up
        float angle2 = 90 + (index) * deltaAngle;
        float radius = Mathf.Min(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y) / 2;

        //两边顶点
        Vector3 p1 = new Vector3(radius * Mathf.Cos(angle1 * Mathf.Deg2Rad), radius * Mathf.Sin(angle1 * Mathf.Deg2Rad));
        Vector3 p2 = new Vector3(radius * Mathf.Cos(angle2 * Mathf.Deg2Rad), radius * Mathf.Sin(angle2 * Mathf.Deg2Rad));

        vh.AddVert(Vector3.zero, color, Vector2.zero);//中心点
        vh.AddVert(p1, color, Vector2.zero);
        vh.AddVert(p2, color, Vector2.zero);//UI的法线可以随便设置

        vh.AddTriangle(index * 3, index * 3 + 1, index * 3 + 2);//将三角面加入UI绘制缓冲区。参数是三角面的三个顶点索引//所以绘制n边形需要绘制3n的顶点

    }

}
