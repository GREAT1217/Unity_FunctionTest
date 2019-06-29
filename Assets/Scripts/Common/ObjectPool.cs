using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    private Dictionary<string, List<GameObject>> _objectDict;
    private Dictionary<string, GameObject> _prefabDict;
    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<string, List<GameObject>> ObjectDict
    {
        get
        {
            if (_objectDict == null) _objectDict = new Dictionary<string, List<GameObject>>();
            return _objectDict;
        }

        set
        {
            _objectDict = value;
        }
    }
    /// <summary>
    /// 预设体字典
    /// </summary>
    private Dictionary<string, GameObject> PrefabDict
    {
        get
        {
            if (_prefabDict == null) _prefabDict = new Dictionary<string, GameObject>();
            return _prefabDict;
        }

        set
        {
            _prefabDict = value;
        }
    }

    /// <summary>
    /// 记录预设体字典
    /// </summary>
    /// <param name="obj"></param>
    public void SetPrefab(GameObject obj)
    {
        if (PrefabDict.ContainsKey(obj.name)) return;
        PrefabDict.Add(obj.name, obj);
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject GetObject(string objName, Transform parent = null)
    {
        if (parent == null) parent = transform;
        GameObject result = null;
        if (ObjectDict.ContainsKey(objName))
        {
            if (ObjectDict[objName].Count > 0)
            {
                result = ObjectDict[objName][0];
                result.transform.SetParent(parent);
                ObjectDict[objName].RemoveAt(0);
                return result;
            }
        }
        GameObject prefab = null;
        if (PrefabDict.ContainsKey(objName))
        {
            prefab = PrefabDict[objName];
        }
        else
        {
            Debug.LogError("[ObjectPool]:   prefab is null");
            return null;
            //prefab = Resources.Load<GameObject>("Prefabs/" + objName);
            //_prefabDict.Add(objName, prefab);
        }
        result = Instantiate(prefab, parent);
        result.name = objName;
        return result;
    }

    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="objName"></param>
    public void RecycleObj(GameObject obj, Transform parent = null)
    {
        if (parent == null) parent = transform;
        obj.SetActive(false);
        obj.transform.SetParent(parent);
        if (ObjectDict.ContainsKey(obj.name))
        {
            ObjectDict[obj.name].Add(obj);
        }
        else
        {
            ObjectDict.Add(obj.name, new List<GameObject>() { obj });
        }
    }

}
