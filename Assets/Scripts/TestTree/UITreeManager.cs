using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UITreeManager : MonoBehaviour
{
    private static RectTransform _root;
    private static GameObject _prefab;
    private static List<GameObject> _nodeList;//当前显示的节点

    public static List<GameObject> NodeList
    {
        get
        {
            if (_nodeList == null) _nodeList = new List<GameObject>();
            return _nodeList;
        }
    }

    public static FileNode[] GetFile(string path, int grade)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] dirs = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
            FileInfo[] files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            List<FileNode> nodes = new List<FileNode>();
            for (int i = 0; i < dirs.Length; i++)
            {
                nodes.Add(new FileNode(grade, dirs[i].Name, dirs[i].FullName, true));
            }
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                nodes.Add(new FileNode(grade, files[i].Name, files[i].FullName, false));
            }
            return nodes.ToArray();
        }
        return null;
    }

    public static GameObject InitNode(Transform parent, FileNode node, int space)
    {
        GameObject obj = Instantiate(_prefab, parent);
        obj.transform.localPosition = Vector2.up * space;
        obj.GetComponent<UITreeNode>().InitData(node);
        obj.SetActive(true);
        return obj;
    }

    public static void InsertRange(GameObject obj, GameObject[] subObjs)
    {
        int index = NodeList.IndexOf(obj) + 1;
        NodeList.InsertRange(index, subObjs);
        ChangePosition(index);
    }

    public static void RemoveRange(GameObject obj, int count)
    {
        int index = NodeList.IndexOf(obj) + 1;
        NodeList.RemoveRange(index, count);
        ChangePosition(index);
    }

    private static void ChangePosition(int startIndex = 0)
    {
        RectTransform rectTrans = _prefab.GetComponent<RectTransform>();
        if (startIndex == 0)
        {
            NodeList[0].transform.position = new Vector3(0, (_root.sizeDelta.y - rectTrans.sizeDelta.y) / 2, 0);
            startIndex = 1;
        }
        for (int i = startIndex; i < NodeList.Count; i++)
        {
            Debug.Log(NodeList[i].GetComponent<UITreeNode>()._node._fileName);
            NodeList[i].transform.position = NodeList[0].transform.position - new Vector3(0, i * rectTrans.rect.height, 0);
        }
    }

    private void Start()
    {
        _root = GetComponent<RectTransform>();
        _prefab = transform.Find("Node").gameObject;
        FileNode[] firstGrade = GetFile(Application.dataPath, 0);
        for (int i = 0; i < firstGrade.Length; i++)
        {
            GameObject node = InitNode(transform, firstGrade[i], 0);
            NodeList.Add(node);
        }
        ChangePosition();
    }
}

public class FileNode
{
    public int _grade;
    public string _fileName;
    public string _filePath;
    public bool _hasChild;
    public FileNode[] _childNodes;

    public FileNode(int grade, string fileName, string filePath, bool hasChild, FileNode[] childNodes = null)
    {
        _grade = grade;
        _fileName = fileName;
        _filePath = filePath;
        _hasChild = hasChild;
        _childNodes = childNodes;
    }
}