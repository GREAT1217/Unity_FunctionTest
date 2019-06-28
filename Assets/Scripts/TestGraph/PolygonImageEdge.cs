using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PolygonImageEdge
{
    public int EdgeCount
    {
        get
        {
            if (m_Weights == null)
                return 0;
            return m_Weights.Count;
        }
    }

    public List<float> Weights
    {
        get { return m_Weights; }
    }

    [SerializeField] private List<float> m_Weights;

}
