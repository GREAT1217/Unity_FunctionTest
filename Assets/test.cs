using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject connectedObj = null;
    Component jointComponent = null;

    void OnGUI()
    {
        if (GUILayout.Button("添加链条关节"))
        {
            ResetJoint();
            jointComponent = gameObject.AddComponent<HingeJoint>();
            HingeJoint hjoint = (HingeJoint)jointComponent;
            connectedObj.GetComponent<Rigidbody>().useGravity = true;
            hjoint.connectedBody = connectedObj.GetComponent<Rigidbody>();
        }
        if (GUILayout.Button("添加固定关节"))
        {
            ResetJoint();
            jointComponent = gameObject.AddComponent<FixedJoint>();
            FixedJoint fjoint = (FixedJoint)jointComponent;
            connectedObj.GetComponent<Rigidbody>().useGravity = true;
            fjoint.connectedBody = connectedObj.GetComponent<Rigidbody>();
        }
        if (GUILayout.Button("添加弹簧关节"))
        {
            ResetJoint();
            jointComponent = gameObject.AddComponent<SpringJoint>();
            SpringJoint sjoint = (SpringJoint)jointComponent;
            connectedObj.GetComponent<Rigidbody>().useGravity = true;
            sjoint.connectedBody = connectedObj.GetComponent<Rigidbody>();
        }
        if (GUILayout.Button("添加角色关节"))
        {
            ResetJoint();
            jointComponent = gameObject.AddComponent<CharacterJoint>();
            CharacterJoint cjoint = (CharacterJoint)jointComponent;
            connectedObj.GetComponent<Rigidbody>().useGravity = true;
            cjoint.connectedBody = connectedObj.GetComponent<Rigidbody>();
        }
        if (GUILayout.Button("添加可配置关节"))
        {
            ResetJoint();
            jointComponent = gameObject.AddComponent<ConfigurableJoint>();
            ConfigurableJoint cojoint = (ConfigurableJoint)jointComponent;
            connectedObj.GetComponent<Rigidbody>().useGravity = true;
            cojoint.connectedBody = connectedObj.GetComponent<Rigidbody>();
        }
    }

    void ResetJoint()
    {
        Destroy(jointComponent);
        connectedObj.gameObject.transform.position = Vector3.zero;
        connectedObj.GetComponent<Rigidbody>().useGravity = true;
    }

}