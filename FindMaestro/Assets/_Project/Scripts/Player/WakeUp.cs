using UnityEngine;
using System.Collections;

public class WakeUp : MonoBehaviour
{
    [Header("苏醒效果")]
    public float headShakeAmount = 45f;      // 摇头幅度（度）
    public float lookDuration = 1.2f;        // 每看一侧的时长

    [Header("苏醒眨眼")]
    public GameObject blinkPanel;            // 眨眼面板
    public float blinkSlowDuration = 0.3f;   // 缓慢闭眼/睁眼的时长
    public float initialCloseDuration = 0.5f; // 一开始闭眼的时间

    private MonoBehaviour fpsController;
    private Transform playerCamera;
    private Vector3 originalCameraPos;
    private Quaternion originalPlayerRot;

    void Start()
    {
        // 禁用第一人称控制器
        fpsController = GetComponent<StarterAssets.FirstPersonController>();
        if (fpsController != null)
            fpsController.enabled = false;

        playerCamera = Camera.main.transform;
        originalCameraPos = playerCamera.localPosition;
        originalPlayerRot = transform.rotation;

        // 低头
        transform.rotation = Quaternion.Euler(30f, originalPlayerRot.eulerAngles.y, 0);

        StartCoroutine(WakeUpSequence());
    }

    IEnumerator WakeUpSequence()
    {
        // ========== 第一阶段：一开始闭着眼 ==========
        if (blinkPanel != null)
        {
            blinkPanel.SetActive(true);
            CanvasGroup cg = blinkPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = blinkPanel.AddComponent<CanvasGroup>();
            cg.alpha = 1f;  // 完全黑屏（闭眼）
        }

        // 闭眼状态持续一小段时间，模拟刚醒还没睁眼
        yield return new WaitForSeconds(initialCloseDuration);

        // ========== 缓慢睁眼 ==========
        if (blinkPanel != null)
        {
            CanvasGroup cg = blinkPanel.GetComponent<CanvasGroup>();
            float timer = 0f;
            while (timer < blinkSlowDuration)
            {
                timer += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, timer / blinkSlowDuration);
                yield return null;
            }
            cg.alpha = 0f;
            blinkPanel.SetActive(false);
        }

        yield return new WaitForSeconds(0.2f);

        // ========== 第二阶段：摇头张望 ==========
        float shakeTimer = 0f;
        float shakeDuration = 6f;
        float maxAngle = headShakeAmount;
        Quaternion startShakeRot = transform.rotation;
        float totalCycles = 1.5f;

        float lastAngle = 0f;

        while (shakeTimer < shakeDuration)
        {
            shakeTimer += Time.deltaTime;
            float t = shakeTimer / shakeDuration;
            float angle = maxAngle * Mathf.Sin(t * Mathf.PI * totalCycles);

            Quaternion targetRot = startShakeRot * Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);

            // 每经过中间点就眨一下眼（频率更高）
            if (lastAngle * angle <= 0 && Mathf.Abs(angle) < 5f && shakeTimer > 0.5f)
            {
                if (blinkPanel != null)
                    StartCoroutine(QuickBlink());
            }
            lastAngle = angle;

            yield return null;
        }

        // 回到中间
        float snapBackTimer = 0f;
        float snapBackDuration = 1.2f;
        Quaternion currentRot = transform.rotation;
        Quaternion centerRot = startShakeRot;

        while (snapBackTimer < snapBackDuration)
        {
            snapBackTimer += Time.deltaTime;
            float t = snapBackTimer / snapBackDuration;
            transform.rotation = Quaternion.Slerp(currentRot, centerRot, t);
            yield return null;
        }
        transform.rotation = centerRot;

        // ========== 第三阶段：抬头 ==========
        float raiseTimer = 0f;
        float raiseDuration = 2f;
        Quaternion startRaiseRot = transform.rotation;
        Quaternion targetRaiseRot = originalPlayerRot;

        while (raiseTimer < raiseDuration)
        {
            raiseTimer += Time.deltaTime;
            float t = raiseTimer / raiseDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            transform.rotation = Quaternion.Slerp(startRaiseRot, targetRaiseRot, t);

            float staggerX = Mathf.Sin(raiseTimer * 12f) * 0.02f * (1f - t);
            playerCamera.localPosition = originalCameraPos + new Vector3(0, 0, staggerX);

            yield return null;
        }

        transform.rotation = targetRaiseRot;
        playerCamera.localPosition = originalCameraPos;

        // 恢复控制器
        if (fpsController != null)
            fpsController.enabled = true;

        Debug.Log("苏醒完成");

        // 激活教程
        TutorialManager tm = FindObjectOfType<TutorialManager>();
        if (tm != null) tm.StartTutorial();
    }

    // 快速眨眼协程（用于张望过程中）
    IEnumerator QuickBlink()
    {
        if (blinkPanel == null) yield break;

        blinkPanel.SetActive(true);
        CanvasGroup cg = blinkPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = blinkPanel.AddComponent<CanvasGroup>();

        // 快速闭眼
        float timer = 0f;
        float quickBlinkDuration = 0.1f;
        while (timer < quickBlinkDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / quickBlinkDuration);
            yield return null;
        }
        cg.alpha = 1f;

        // 快速睁眼
        timer = 0f;
        while (timer < quickBlinkDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, timer / quickBlinkDuration);
            yield return null;
        }
        cg.alpha = 0f;
        blinkPanel.SetActive(false);
    }

    // 辅助协程：平滑旋转
    IEnumerator RotateOverTime(Quaternion fromRot, Quaternion toRot, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = 1 - Mathf.Pow(1 - t, 2);
            transform.rotation = Quaternion.Slerp(fromRot, toRot, t);
            yield return null;
        }
        transform.rotation = toRot;
    }
}