using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;  // 需要引用 UI 命名空间

public class AIFuryController : MonoBehaviour
{
    [Header("红光闪烁")]
    public Image redFlashImage;           // 全屏红色 Image（UI）
    public float flashDuration = 0.1f;
    public int flashCount = 4;

    [Header("警报音效")]
    public AudioClip alarmSound;
    public AudioSource audioSource;

    [Header("报错文字")]
    public TextMeshProUGUI errorText;     // 屏幕中央的报错文字（多行）
    public float errorDisplayTime = 2f;

    [Header("传送效果")]
    public GameObject fadePanel;          // 全屏白色/黑色面板（用于淡入淡出）
    public float whiteOutDuration = 0.5f;
    public Transform teleportDestination;
    public string nextSceneName = "";     // 如果留空，则使用 teleportDestination

    private bool isFuryActive = false;
    private GameObject player;
    private StarterAssets.FirstPersonController playerController;

    void Awake()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (redFlashImage != null)
            redFlashImage.gameObject.SetActive(false);

        if (errorText != null)
            errorText.gameObject.SetActive(false);

        if (fadePanel != null)
            fadePanel.SetActive(false);
    }

    public void StartFury()
    {
        if (isFuryActive) return;
        isFuryActive = true;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();

        StartCoroutine(FurySequence());
    }

    IEnumerator FurySequence()
    {
        // 1. 禁用玩家控制
        if (playerController != null)
            playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 红光闪烁 + 警报音效
        if (alarmSound != null)
            audioSource.PlayOneShot(alarmSound);

        for (int i = 0; i < flashCount; i++)
        {
            if (redFlashImage != null)
            {
                redFlashImage.gameObject.SetActive(true);
                redFlashImage.color = new Color(1, 0, 0, 0.7f);
                yield return new WaitForSeconds(flashDuration);
                redFlashImage.gameObject.SetActive(false);
                yield return new WaitForSeconds(flashDuration);
            }
            else
            {
                yield return new WaitForSeconds(flashDuration * 2);
            }
        }

        // 3. 显示报错文字（带闪烁效果）
        if (errorText != null)
        {
            errorText.text = "ERROR: Emotion not quantifiable\n" +
                             "DELETE FAILED\n" +
                             "Creativity resistance > threshold\n" +
                             "System conflict...";
            errorText.gameObject.SetActive(true);

            // 闪烁两下（每次闪烁间隔0.2秒）
            float blinkInterval = 0.2f;
            int blinkCount = 2;
            for (int i = 0; i < blinkCount; i++)
            {
                errorText.alpha = 0f;
                yield return new WaitForSeconds(blinkInterval);
                errorText.alpha = 1f;
                yield return new WaitForSeconds(blinkInterval);
            }

            // 再停留额外时间（总显示时长减去闪烁占用的时间）
            float remainingTime = errorDisplayTime - (blinkCount * blinkInterval * 2);
            if (remainingTime > 0)
                yield return new WaitForSeconds(remainingTime);

            errorText.gameObject.SetActive(false);
        }

        // 4. 白光淡出（或黑屏，根据你的 fadePanel 颜色）
        yield return StartCoroutine(FadeToWhite(whiteOutDuration));

        // 5. 传送或加载场景
        if (teleportDestination != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = teleportDestination.position;
            player.transform.rotation = teleportDestination.rotation;
            if (cc != null) cc.enabled = true;
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        // 6. 淡入
        yield return StartCoroutine(FadeToClear(whiteOutDuration));

        // 7. 恢复玩家控制
        if (playerController != null)
            playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isFuryActive = false;
    }

    IEnumerator FadeToWhite(float duration)
    {
        if (fadePanel == null) yield break;
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = fadePanel.AddComponent<CanvasGroup>();
        fadePanel.SetActive(true);
        cg.alpha = 0f;
        // 如果希望是白色面板，确保 fadePanel 的 Image 颜色为白色
        Image img = fadePanel.GetComponent<Image>();
        if (img != null) img.color = Color.white;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    IEnumerator FadeToClear(float duration)
    {
        if (fadePanel == null) yield break;
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();
        float startAlpha = cg.alpha;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, timer / duration);
            yield return null;
        }
        cg.alpha = 0f;
        fadePanel.SetActive(false);
    }
}