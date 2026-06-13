using UnityEngine;
using System.Collections;
using TMPro;

public class Level2Exit : MonoBehaviour
{
    [Header("触发条件")]
    public bool requireAllFragments = true;     // 是否需要收集全部碎片

    [Header("对话设置")]
    public GameObject dialoguePanel;            // 对话面板（UI）
    public TextMeshProUGUI dialogueText;        // 对话文字组件
    public float lineDelay = 2.5f;              // 每句话显示间隔（秒）

    [Header("传送/切场景")]
    public Transform teleportDestination;       // 第三层起始点（如果同一场景）
    public string nextSceneName = "Level3_EarlyAI"; // 如果要切场景，填入场景名
    public GameObject fadePanel;                // 黑屏面板（带 CanvasGroup）
    public float fadeDuration = 1f;

    private bool triggered = false;
    private StarterAssets.FirstPersonController playerController;
    private GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (requireAllFragments && FragmentManager.Instance != null)
        {
            if (FragmentManager.Instance.CurrentFragments < FragmentManager.Instance.TotalFragments)
            {
                Debug.Log("需要收集所有碎片才能离开！");
                return;
            }
        }

        triggered = true;
        player = other.gameObject;
        playerController = player.GetComponent<StarterAssets.FirstPersonController>();
        StartCoroutine(ExitSequence());
    }

    IEnumerator ExitSequence()
    {
        // 1. 禁用玩家移动和视角
        if (playerController != null) playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 显示对话（玩家内心独白）
        string[] lines = new string[]
        {
            "I've gathered all five fragments...",
            "A programmer's messy code, a friend's game design sketch, an unfinished sheet music, a crumpled poem, an old palette.",
            "All of them are whispers from the past – when humans created for joy, not for perfection.",
            "The AI rays tried to wipe this away, to turn me into a machine that only follows automated rules.",
            "But now I see: creativity is not about being flawless. It's about the courage to try, to fail, to laugh.",
            "This corrupted future can still be changed. I must go back to the beginning – to the time when AI was just a tool.",
            "The age where humans used AI to assist, not to replace. I will prevent this decay."
        };

        dialoguePanel.SetActive(true);
        foreach (string line in lines)
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(lineDelay);
        }
        dialoguePanel.SetActive(false);

        // 3. 黑屏淡出
        yield return StartCoroutine(Fade(1f));

        // 4. 传送或加载场景
        if (teleportDestination != null)
        {
            // 同一场景内传送
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = teleportDestination.position;
            player.transform.rotation = teleportDestination.rotation;
            if (cc != null) cc.enabled = true;
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }

        // 5. 淡入（如果是加载场景，淡入会在新场景开始时做，这里可省略）
        yield return StartCoroutine(Fade(0f));

        // 恢复玩家控制（如果是同一场景内传送，需要重新启用）
        if (playerController != null) playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadePanel == null) yield break;
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = fadePanel.AddComponent<CanvasGroup>();
        fadePanel.SetActive(true);
        float startAlpha = cg.alpha;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }
        cg.alpha = targetAlpha;
        if (targetAlpha == 0) fadePanel.SetActive(false);
    }
}