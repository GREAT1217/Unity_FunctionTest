using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PolygonCollider2D))]
public class UIPieImage : Image
{
    private PolygonCollider2D _polygon = null;
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
        float radius = rectTransform.sizeDelta.y / 2;
        float angle = fillAmount * 360;
        SetColliderPath(radius, angle);
    }

    /// <summary>
    /// 设置Collider路径
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="radian"></param>
    private void SetColliderPath(float radius, float angle)
    {
        Polygon.SetPath(0, GetPathes(radius, angle));
    }

    /// <summary>
    /// 计算路径点
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector2[] GetPathes(float radius, float angle)
    {
        //注意数组的顺序
        Vector2[] pathes = null;
        float rate = angle / 60;
        float radian = angle * Mathf.Deg2Rad;
        #region 找规律
        //if (1 >= rate)
        //{
        //    pathes = new Vector2[3];
        //}
        //else if (1 < rate && 2 >= rate)
        //{
        //    pathes = new Vector2[4];
        //    pathes[2] = new Vector2(radius * Mathf.Sin(radian / 2), radius * Mathf.Cos(radian / 2));
        //}
        //else if (2 < rate && 3 >= rate)
        //{
        //    pathes = new Vector2[5];
        //    pathes[2] = new Vector2(radius * Mathf.Sin(radian / 3), radius * Mathf.Cos(radian / 3));
        //    pathes[3] = new Vector2(radius * Mathf.Sin(radian / 3 * 2), radius * Mathf.Cos(radian / 3 * 2));
        //}
        //else if (3 < rate && 4 >= rate)
        //{
        //    pathes = new Vector2[6];
        //    pathes[2] = new Vector2(radius * Mathf.Sin(radian / 4), radius * Mathf.Cos(radian / 4));
        //    pathes[3] = new Vector2(radius * Mathf.Sin(radian / 2), radius * Mathf.Cos(radian / 2));
        //    pathes[4] = new Vector2(radius * Mathf.Sin(radian / 4 * 3), radius * Mathf.Cos(radian / 4 * 3));
        //}
        //else if (4 < rate && 5 >= rate)
        //{
        //    pathes = new Vector2[7];
        //    pathes[2] = new Vector2(radius * Mathf.Sin(radian / 5), radius * Mathf.Cos(radian / 5));
        //    pathes[3] = new Vector2(radius * Mathf.Sin(radian / 5 * 2), radius * Mathf.Cos(radian / 5 * 2));
        //    pathes[4] = new Vector2(radius * Mathf.Sin(radian / 5 * 3), radius * Mathf.Cos(radian / 5 * 3));
        //    pathes[5] = new Vector2(radius * Mathf.Sin(radian / 5 * 4), radius * Mathf.Cos(radian / 5 * 4));
        //}
        //else
        //{
        //    pathes = new Vector2[8];
        //    pathes[2] = new Vector2(radius * Mathf.Sin(angle / 6), radius * Mathf.Cos(angle / 6));
        //    pathes[3] = new Vector2(radius * Mathf.Sin(angle / 6 * 2), radius * Mathf.Cos(angle / 6 * 2));
        //    pathes[4] = new Vector2(radius * Mathf.Sin(angle / 6 * 3), radius * Mathf.Cos(angle / 6 * 3));
        //    pathes[5] = new Vector2(radius * Mathf.Sin(angle / 6 * 4), radius * Mathf.Cos(angle / 6 * 4));
        //    pathes[6] = new Vector2(radius * Mathf.Sin(angle / 6 * 5), radius * Mathf.Cos(angle / 6 * 5));
        //}
        #endregion
        for (int i = 1; i <= 6; i++)
        {
            if (i - 1 < rate && i >= rate)
            {
                pathes = new Vector2[i + 2];
                Debug.Log(i);
                for (int j = 2; j < pathes.Length - 1; j++)
                {
                    pathes[j] = new Vector2(radius * Mathf.Sin(radian / i * (j - 1)), radius * Mathf.Cos(radian / i * (j - 1)));
                }
                break;
            }
        }
        pathes[0] = Vector2.zero;
        pathes[1] = new Vector2(0, radius);
        pathes[pathes.Length - 1] = new Vector2(radius * Mathf.Sin(radian), radius * Mathf.Cos(radian));
        return pathes;
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
