using UnityEngine;
using TMPro;          // 引入 TextMeshPro 命名空间
using System.Collections;

public class AIScanner : MonoBehaviour
{
    [Header("惩罚设置")]
    public Transform resetPoint;
    public float resetCooldown = 1.5f;

    [Header("检测参数")]
    public float detectionRadius = 2.5f;

    [Header("UI 提示")]
    public GameObject detectionWarningPanel;        // 提示面板（默认禁用）
    public TextMeshProUGUI warningText;             // 面板内的文字组件（拖入）
    public float warningDisplayTime = 2.5f;

    private float lastResetTime = -10f;
    private Coroutine hideWarningCoroutine;

    void Update()
    {
        if (Time.time - lastResetTime < resetCooldown) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        float horizontalDist = toPlayer.magnitude;

        if (horizontalDist <= detectionRadius)
        {
            OnPlayerDetected(player);
        }
    }

    void OnPlayerDetected(GameObject player)
    {
        lastResetTime = Time.time;

        if (resetPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = resetPoint.position;
            player.transform.rotation = resetPoint.rotation;
            if (cc != null) cc.enabled = true;
        }

        if (FragmentManager.Instance != null)
            FragmentManager.Instance.ResetFragments();

        ShowWarning();

        Debug.Log("玩家被 AI 扫描到！位置重置，碎片清零。");
    }

    void ShowWarning()
    {
        if (detectionWarningPanel == null) return;

        // 设置警告文字
        if (warningText != null)
        {
            warningText.text = "You have been detected by AI! Your memory has been partially erased. Collect the fragments again.";
        }

        if (hideWarningCoroutine != null)
            StopCoroutine(hideWarningCoroutine);

        detectionWarningPanel.SetActive(true);
        hideWarningCoroutine = StartCoroutine(HideWarningAfterDelay());
    }

    IEnumerator HideWarningAfterDelay()
    {
        yield return new WaitForSeconds(warningDisplayTime);
        if (detectionWarningPanel != null)
            detectionWarningPanel.SetActive(false);
        hideWarningCoroutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}