using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UITreeManager : MonoBehaviour
{
    public RectTransform _root;
    public GameObject _prefab;
    private List<UITreeNode> _nodeList;//当前显示的节点
    public List<UITreeNode> NodeList
    {
        get
        {
            if (_nodeList == null) _nodeList = new List<UITreeNode>();
            return _nodeList;
        }
    }

    public FileNode[] GetFile(string path, int grade)
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

    public UITreeNode InitNode(Transform parent, FileNode node)
    {
        GameObject obj = Instantiate(_prefab, parent);
        //obj.transform.localPosition = Vector2.up * space;
        UITreeNode treeNode = obj.GetComponent<UITreeNode>();
        treeNode.InitData(node, this);
        obj.SetActive(true);
        return treeNode;
    }

    public void InsertRange(UITreeNode obj, UITreeNode[] subObjs)
    {
        int index = NodeList.IndexOf(obj) + 1;
        NodeList.InsertRange(index, subObjs);
        ChangePosition(index);
    }

    public void RemoveRange(UITreeNode obj, int count)
    {
        //Debug.Log(count);
        int index = NodeList.IndexOf(obj) + 1;
        NodeList.RemoveRange(index, count);
        ChangePosition(index);
    }

    private void ChangePosition(int startIndex = 0)
    {
        RectTransform rectTrans = _prefab.GetComponent<RectTransform>();
        if (startIndex == 0)
        {
            NodeList[0].transform.localPosition = new Vector3(0, (_root.sizeDelta.y - rectTrans.sizeDelta.y) / 2, 0);
            startIndex = 1;
        }
        for (int i = startIndex; i < NodeList.Count; i++)
        {
            //Debug.Log(NodeList[i].GetComponent<UITreeNode>()._node._fileName);
            //NodeList[i].transform.localPosition = NodeList[0].transform.localPosition - new Vector3(0, i * rectTrans.rect.height, 0);//设置在根目录
            NodeList[i].transform.position = NodeList[0].transform.position - new Vector3(0, i * rectTrans.rect.height, 0);
        }
    }

    private void Start()
    {
        FileNode[] firstGrade = GetFile(Application.dataPath, 0);
        for (int i = 0; i < firstGrade.Length; i++)
        {
            UITreeNode node = InitNode(transform, firstGrade[i]);
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