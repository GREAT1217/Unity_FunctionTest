using UnityEngine;

public class TestOctree : MonoBehaviour
{
    public GameObject[] m_SceneObjects;
    public int m_TreeMaxDepth = 5;
    public Camera m_Camera;
    private Octree m_Octree;

    void Start()
    {
        m_Octree = new Octree(m_SceneObjects, m_TreeMaxDepth);
    }

    private void Update()
    {
        if (m_Camera != null && m_Octree != null)
        {
            m_Octree.OnCameraUpdate(m_Camera);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            m_Octree.RootNode.Draw();
        }
    }
}
