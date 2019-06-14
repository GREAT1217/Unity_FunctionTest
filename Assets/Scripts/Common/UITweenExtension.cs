using UnityEngine;
using DG.Tweening;

public static class UITweenException
{

    /// <summary>
    /// 纵向移动到显示
    /// </summary>
    /// <param name="rectTran"></param>
    /// <param name="duration">持续时间</param>
    /// <param name="toTop">向上移动</param>
    /// <param name="snapping">If TRUE the tween will smoothly snap all values to integers</param>
    /// <returns></returns>
    public static Tweener VerticalMoveToShow(this RectTransform rectTran, float duration, bool toTop = true, bool snapping = false)
    {
        Vector2 pos = rectTran.localPosition;
        rectTran.localPosition = new Vector2(pos.x, Screen.height / 2 + rectTran.rect.height / 2) * (toTop ? -1 : 1);
        Vector2 targetPos = new Vector2(pos.x, Screen.height / 2 - rectTran.rect.height / 2) * (toTop ? -1 : 1);
        return rectTran.DOLocalMove(targetPos, duration, snapping);
    }

    /// <summary>
    /// 纵向移动到隐藏
    /// </summary>
    /// <param name="rectTran"></param>
    /// <param name="duration">持续时间</param>
    /// <param name="toTop">向上移动</param>
    /// <param name="snapping">If TRUE the tween will smoothly snap all values to integers</param>
    /// <returns></returns>
    public static Tweener VerticalMoveToHide(this RectTransform rectTran, float duration, bool toTop = true, bool snapping = false)
    {
        Vector2 targetPos = new Vector2(rectTran.localPosition.x, Screen.height / 2 + rectTran.rect.height / 2) * (toTop ? 1 : -1);
        return rectTran.DOLocalMove(targetPos, duration, snapping);
    }

    /// <summary>
    /// 横向移动到显示
    /// </summary>
    /// <param name="rectTran"></param>
    /// <param name="duration">持续时间</param>
    /// <param name="toLeft">向左移动</param>
    /// <param name="snapping">If TRUE the tween will smoothly snap all values to integers</param>
    /// <returns></returns>
    public static Tweener HorizontalMoveToShow(this RectTransform rectTran, float duration, bool toLeft = true, bool snapping = false)
    {
        Vector2 pos = rectTran.localPosition;
        rectTran.localPosition = new Vector2(Screen.width / 2 + rectTran.rect.width / 2, pos.y) * (toLeft ? -1 : 1);
        Vector2 targetPos = new Vector2(Screen.width / 2 - rectTran.rect.width / 2, pos.y) * (toLeft ? -1 : 1);
        return rectTran.DOLocalMove(targetPos, duration, snapping);
    }

    /// <summary>
    /// 横向移动到隐藏
    /// </summary>
    /// <param name="rectTran"></param>
    /// <param name="duration">持续时间</param>
    /// <param name="toLeft">向左移动</param>
    /// <param name="snapping">If TRUE the tween will smoothly snap all values to integers</param>
    /// <returns></returns>
    public static Tweener HorizontalMoveToHide(this RectTransform rectTran, float duration, bool toLeft = true, bool snapping = false)
    {
        Vector2 targetPos = new Vector2(Screen.width / 2 - rectTran.rect.width / 2, rectTran.localPosition.y) * (toLeft ? 1 : -1);
        return rectTran.DOLocalMove(targetPos, duration, snapping);
    }

}
