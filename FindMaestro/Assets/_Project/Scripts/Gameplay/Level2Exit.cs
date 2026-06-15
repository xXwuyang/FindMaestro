using UnityEngine;
using System.Collections;

public class Level2Exit : MonoBehaviour
{
    [Header("触发条件")]
    public bool requireAllFragments = true;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (requireAllFragments && FragmentManager.Instance != null)
        {
            if (FragmentManager.Instance.CurrentFragments < FragmentManager.Instance.TotalFragments)
            {
                Debug.Log("需要收集所有碎片才能离开！");
                // 可以加一个提示UI
                return;
            }
        }

        // 如果已经集齐，不做任何事（因为 FragmentManager 会自动传送）
        Debug.Log("碎片已集齐，等待 FragmentManager 自动传送...");
        triggered = true; // 避免重复触发
    }
}