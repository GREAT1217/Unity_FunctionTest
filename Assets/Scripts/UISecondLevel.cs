using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISecondLevel : MonoBehaviour
{
    public UISecondPanel _panelPrefab;

    public void ShowPanel(string text)
    {
        _panelPrefab.InitUIPath(text);
        _panelPrefab.MovePanel(true);
    }
}
