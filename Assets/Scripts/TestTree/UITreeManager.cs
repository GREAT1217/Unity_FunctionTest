using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    /// <summary>
    /// 当前节点
    /// </summary>
    public Transform _curNode;
    /// <summary>
    /// 当前节点等级
    /// </summary>
    public int _grade;
    /// <summary>
    /// 当前节点路径
    /// 作为子节点父对象
    /// </summary>
    public string _path;
    /// <summary>
    /// 是否有子节点
    /// </summary>
    public bool _hasNodes;
    /// <summary>
    /// 子节点
    /// 是拥有子节点的子节点
    /// </summary>
    public Tree[] _subNodes;
    /// <summary>
    /// 当前节点对象组件
    /// </summary>
    public Component _component;

    public Tree() { }
}

public class Tree<T> : Tree where T : Component
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="curNode">当前节点</param>
    /// <param name="grade">当前节点等级</param>
    /// <param name="path">当前节点路径</param>
    public Tree(Transform curNode, int grade, string path = "")
    {
        _curNode = curNode;
        _grade = grade;
        _path = path + "/" + curNode.name;
        _component = _curNode.GetComponent<T>();
        _hasNodes = (_component != null);
        SubNodes();
    }

    private void SubNodes()
    {
        //初始化所有子节点
        _subNodes = new Tree<T>[_curNode.childCount];
        for (int i = 0; i < _subNodes.Length; i++)
        {
            _subNodes[i] = new Tree<T>(_curNode.GetChild(i), _grade + 1, _path);
        }
        //统计没有子节点的子节点
        int length = 0;
        for (int i = 0; i < _subNodes.Length; i++)
        {
            if (!_subNodes[i]._hasNodes) length++;
        }
        if (length == 0) return;
        Tree<T>[] tempNodes = new Tree<T>[_subNodes.Length - length];
        //整理有子节点的子节点
        int index = 0;
        for (int i = 0; i < _subNodes.Length; i++)
        {
            if (_subNodes[i]._hasNodes)
            {
                tempNodes[index] = _subNodes[i] as Tree<T>;
                index++;
            }
        }
        _subNodes = tempNodes;
    }
}

public class UITreeManager : MonoBehaviour
{
    public static GameObject _prefab;

    public static List<GameObject> TreeNodes { get; set; }

    public static GameObject InitNode(Transform parent, Tree tree, int space)
    {
        GameObject obj = Instantiate(_prefab, parent);
        obj.transform.localPosition = Vector2.up * space;
        obj.GetComponent<UITreeNode>().InitData(tree);
        return obj;
    }

    public static void InsertRange(GameObject obj, GameObject[] subObjs)
    {
        int index = TreeNodes.IndexOf(obj) + 1;
        TreeNodes.InsertRange(index, subObjs);
        ChangePosition(index);
    }

    public static void RemoveRange(GameObject obj, int count)
    {
        int index = TreeNodes.IndexOf(obj) + 1;
        TreeNodes.RemoveRange(index, count);
        ChangePosition(index);
    }

    private static void ChangePosition(int startIndex)
    {
        RectTransform rectTrans = _prefab.GetComponent<RectTransform>();
        for (int i = startIndex; i < TreeNodes.Count; i++)
        {
            TreeNodes[i].transform.position = TreeNodes[0].transform.position - new Vector3(0, i * rectTrans.rect.height, 0);
        }
    }

    public GameObject root;
    public GameObject firstNode;
    private void Start()
    {
        TreeNodes = new List<GameObject>();
        Tree<Transform> tree = new Tree<Transform>(root.transform, 0);
        firstNode.GetComponent<UITreeNode>().InitData(tree);
        TreeNodes.Add(firstNode);
        _prefab = Resources.Load<GameObject>("TestTree/Node");
    }
}