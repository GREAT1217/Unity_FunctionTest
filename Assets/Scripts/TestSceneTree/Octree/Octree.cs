using UnityEngine;

public class Octree
{
    public float MaxDepth { get; private set; }

    public OctreeNode RootNode { get; private set; }

    public Octree(GameObject[] gameObjects, float maxDepth)
    {
        MaxDepth = maxDepth;
        // 根据所有游戏对象，计算树的根节点范围。
        Bounds bounds = new Bounds();
        foreach (var sceneObject in gameObjects)
        {
            bounds.Encapsulate(sceneObject.GetComponent<Renderer>().bounds);
        }
        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 0.5f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        RootNode = new OctreeNode(this, 0, bounds);
        // 划分游戏对象，缓存在树的节点中。
        foreach (var sceneObject in gameObjects)
        {
            RootNode.DivideAndCacheObject(sceneObject);
        }
    }

    public void OnCameraUpdate(Camera camera)
    {
        RootNode.OnCameraUpdate(camera);
    }
}
