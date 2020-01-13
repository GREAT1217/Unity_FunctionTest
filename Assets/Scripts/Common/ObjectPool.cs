using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    private Dictionary<string, List<GameObject>> _objectDict;
    private Dictionary<string, GameObject> _prefabDict;
    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<string, List<GameObject>> ObjPool
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
    /// <param name="objName"></param>
    /// <param name="obj"></param>
    public void SetPrefab(string objName, GameObject obj)
    {
        if (PrefabDict.ContainsKey(objName)) return;
        PrefabDict.Add(objName, obj);
    }

    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public GameObject GetObject(string poolName, Transform parent = null)
    {
        if (parent == null) parent = transform;
        GameObject result;
        if (ObjPool.ContainsKey(poolName))
        {
            if (ObjPool[poolName].Count > 0)
            {
                result = ObjPool[poolName][0];
                result.transform.SetParent(parent);
                ObjPool[poolName].RemoveAt(0);
                return result;
            }
        }
        if (PrefabDict.ContainsKey(poolName))
        {
            result = Instantiate(PrefabDict[poolName], parent);
            result.name = poolName;
            return result;
        }
        else
        {
            Debug.LogError("[ObjectPool]:   prefab is null");
            return null;
            //prefab = Resources.Load<GameObject>("Prefabs/" + objName);
            //_prefabDict.Add(objName, prefab);
        }
    }

    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="obj"></param>
    public void RecycleObj(string poolName, GameObject obj)
    {
        obj.SetActive(false);
        if (ObjPool.ContainsKey(poolName))
        {
            ObjPool[poolName].Add(obj);
        }
        else
        {
            ObjPool.Add(poolName, new List<GameObject>() { obj });
        }
    }

    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="objName"></param>
    public void RecycleObj(GameObject obj, Transform parent = null)
    {
        //TODO  统一
        if (parent == null) parent = transform;
        obj.SetActive(false);
        obj.transform.SetParent(parent);
        if (ObjPool.ContainsKey(obj.name))
        {
            ObjPool[obj.name].Add(obj);
        }
        else
        {
            ObjPool.Add(obj.name, new List<GameObject>() { obj });
        }
    }

    /// <summary>
    /// 删除对象池
    /// </summary>
    /// <param name="poolName"></param>
    public void DestoryPool(string poolName)
    {
        if (!_objectDict.ContainsKey(poolName)) return;
        List<GameObject> objs = _objectDict[poolName];
        for (int i = 0; i < objs.Count;)
        {
            Destroy(objs[i]);
            objs.RemoveAt(i);
        }
        _objectDict.Remove(poolName);
    }

}
