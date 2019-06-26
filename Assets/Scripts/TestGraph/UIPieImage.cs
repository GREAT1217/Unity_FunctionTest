using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]
public class UIPieImage : Image
{
    private RectTransform _rectTrans = null;
    private PolygonCollider2D _polygon = null;
    private RectTransform RectTrans
    {
        get
        {
            if (_rectTrans == null) _rectTrans = GetComponent<RectTransform>();
            return _rectTrans;
        }
    }
    private PolygonCollider2D Polygon
    {
        get
        {
            if (_polygon == null) _polygon = GetComponent<PolygonCollider2D>();
            return _polygon;
        }
    }

    /// <summary>
    /// 重置Collider
    /// </summary>
    public void ResetCollider()
    {
        float radius = RectTrans.sizeDelta.y / 2;
        float radian = fillAmount * 360 * Mathf.Deg2Rad;
        SetColliderPath(radius, radian);
    }

    /// <summary>
    /// 设置Collider路径
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="radian"></param>
    private void SetColliderPath(float radius, float radian)
    {
        Vector2[] pathes = null;
        //Debug.Log(radian * Mathf.Rad2Deg);
        if (radian < 1)//大约60°
        {
            pathes = new Vector2[3];
            pathes[0] = Vector2.zero;
            pathes[1] = new Vector2(0, radius);
            pathes[2] = new Vector2(radius * Mathf.Sin(radian), radius * Mathf.Cos(radian));
        }
        else
        {
            pathes = new Vector2[4];
            //注意顺序
            pathes[0] = Vector2.zero;
            pathes[1] = new Vector2(0, radius);
            pathes[2] = new Vector2(radius * Mathf.Sin(radian / 2), radius * Mathf.Cos(radian / 2));
            pathes[3] = new Vector2(radius * Mathf.Sin(radian), radius * Mathf.Cos(radian));
        }
        Polygon.SetPath(0, pathes);
    }

    /// <summary>
    /// 检测Recast是否有效
    /// ICanvasRaycastFilter接口函数
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        bool inCollider = Polygon.OverlapPoint(Input.mousePosition);
        if (inCollider) transform.localScale = Vector3.one * 1.2f;
        else transform.localScale = Vector3.one;
        return inCollider;
    }
}
