using UnityEngine;
using TMPro;
using System.Collections;

public class AIScanner : MonoBehaviour
{
    [Header("重置")]
    public Transform resetPoint;
    public float resetCooldown = 1.5f;

    [Header("检测")]
    public float detectionRadius = 2.5f;

    [Header("UI")]
    public GameObject detectionWarningPanel;
    public TextMeshProUGUI warningText;
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

        if (toPlayer.magnitude <= detectionRadius)
        {
            OnPlayerDetected(player);
        }
    }

    void OnPlayerDetected(GameObject player)
    {
        lastResetTime = Time.time;

        // 🔥 新增：压力增加（关键）
        if (ExposureSystem.Instance != null)
            ExposureSystem.Instance.AddExposure(35f);

        // reset player
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = resetPoint.position;
        player.transform.rotation = resetPoint.rotation;

        if (cc != null) cc.enabled = true;

        // reset fragments
        if (FragmentManager.Instance != null)
            FragmentManager.Instance.ResetFragments();

        // 隐藏当前显示的碎片句子
        if (FragmentManager.Instance != null)
            FragmentManager.Instance.HideCurrentPopup();

        ShowWarning();
    }

    void ShowWarning()
    {
        if (detectionWarningPanel == null) return;

        if (warningText != null)
        {
            warningText.text = "AI DETECTED YOU! Memory instability increased.";
        }

        if (hideWarningCoroutine != null)
            StopCoroutine(hideWarningCoroutine);

        detectionWarningPanel.SetActive(true);
        hideWarningCoroutine = StartCoroutine(HideWarning());
    }

    IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(warningDisplayTime);

        if (detectionWarningPanel != null)
            detectionWarningPanel.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}