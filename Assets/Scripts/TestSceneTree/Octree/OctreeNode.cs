using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    /// <summary>
    /// 树。
    /// </summary>
    private Octree m_Octree;
    /// <summary>
    /// 节点深度。
    /// </summary>
    private int m_Depth;
    /// <summary>
    /// 节点范围。
    /// </summary>
    private Bounds m_Bounds;
    /// <summary>
    /// 缓存对象。
    /// </summary>
    private List<GameObject> m_CacheObjects;
    /// <summary>
    /// 子节点。
    /// </summary>
    private OctreeNode[] m_ChildNodes;

    public OctreeNode(Octree octree, int depth, Bounds bounds)
    {
        m_Octree = octree;
        m_Depth = depth;
        m_Bounds = bounds;
        m_CacheObjects = new List<GameObject>();
        // 子节点动态初始化：如果 m_Bounds.size.x <= m_MinSize， 则不再划分子节点，无需初始化 m_ChildNodes
    }

    /// <summary>
    /// 划分节点并缓存游戏对象。
    /// </summary>
    /// <param name="gameObject"></param>
    public void DivideAndCacheObject(GameObject gameObject)
    {
        // 当前节点已经达到最大深度，无法向子节点划分，缓存对象到当前节点 
        if (m_Depth > m_Octree.MaxDepth)
        {
            m_CacheObjects.Add(gameObject);
            return;
        }

        // 当前节点未达到最大深度，可以向子节点划分
        if (m_ChildNodes == null)
        {
            DivideChildNodes();
        }

        bool canDivide = false;
        OctreeNode divideNode = null;
        if (m_ChildNodes != null)
        {
            // 遍历检查子节点，检查子节点范围是否与对象包围盒相交
            foreach (var childNode in m_ChildNodes)
            {
                if (childNode.m_Bounds.Intersects(gameObject.GetComponent<Renderer>().bounds))
                {
                    if (divideNode != null)
                    {
                        canDivide = false;
                        break;
                    }
                    divideNode = childNode;
                    canDivide = true;
                }
            }
        }

        if (canDivide)
        {
            // 只有一个子节点包含此物体，继续向子节点划分
            divideNode.DivideAndCacheObject(gameObject);
        }
        else
        {
            // 有多个子节点包含此物体，将物体缓存至此节点
            m_CacheObjects.Add(gameObject);
        }
    }

    /// <summary>
    /// 划分子节点。
    /// </summary>
    private void DivideChildNodes()
    {
        m_ChildNodes = new OctreeNode[8];
        var childLength = m_Bounds.size.x * 0.5f; // 子级范围尺寸 = 当前范围尺寸 * 0.5f
        var childSize = new Vector3(childLength, childLength, childLength);
        var index = 0;
        for (var x = -1; x <= 1; x += 2)
        {
            for (var y = -1; y <= 1; y += 2)
            {
                for (var z = -1; z <= 1; z += 2)
                {
                    var centerOffset = new Vector3(m_Bounds.size.x / 4f * x, m_Bounds.size.y / 4f * y, m_Bounds.size.z / 4f * z); // 在三个维度上，按 -0.25，0.25的中心偏移，划分子级范围。
                    var childBounds = new Bounds(m_Bounds.center + centerOffset, childSize);
                    m_ChildNodes[index++] = new OctreeNode(m_Octree, m_Depth + 1, childBounds);
                }
            }
        }
    }

    public void OnCameraUpdate(Camera camera)
    {
        bool active = m_Bounds.IsInViewport(camera);
        ActiveNode(active);

        // 显示时，继续检查子节点
        if (active && m_ChildNodes != null)
        {
            foreach (var childNode in m_ChildNodes)
            {
                childNode.OnCameraUpdate(camera);
            }
        }
    }

    public void ActiveNode(bool active)
    {
        foreach (var cacheObject in m_CacheObjects)
        {
            cacheObject.SetActive(active);
        }

        // 隐藏时，子节点全部隐藏
        if (active == false && m_ChildNodes != null)
        {
            foreach (var childNode in m_ChildNodes)
            {
                childNode.ActiveNode(false);
            }
        }
    }

    public void Draw()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);
        if (m_ChildNodes != null)
        {
            for (var i = 0; i < 8; i++)
            {
                if (m_ChildNodes[i] != null)
                {
                    m_ChildNodes[i].Draw();
                }
            }
        }
    }
}
