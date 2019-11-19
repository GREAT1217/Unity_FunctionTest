using System.IO;
using UnityEngine;

public class FindDirectory : MonoBehaviour
{

    public string _path;

    private void Start()
    {
        _path = Application.dataPath;
        GetFile();
    }

    public void GetFile()
    {
        if (Directory.Exists(_path))
        {
            DirectoryInfo directory = new DirectoryInfo(_path);
            FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
            Debug.Log(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i].Name);
            }
        }
    }
}
