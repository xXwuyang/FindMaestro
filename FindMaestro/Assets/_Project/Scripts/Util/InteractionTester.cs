using UnityEngine;

public class InteractionTester : MonoBehaviour
{
    void Update()
    {
        // 测试1：E 键是否被 Unity 检测到
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("【测试1】E 键被按下了！");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 测试2：是否触发了碰撞检测
        Debug.Log("【测试2】触发检测中，物体: " + other.name + "，Tag: " + other.tag);

        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("【测试3】玩家在触发器中按下了 E 键！");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("【测试4】进入触发器，物体: " + other.name);
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("【测试5】离开触发器，物体: " + other.name);
    }
}