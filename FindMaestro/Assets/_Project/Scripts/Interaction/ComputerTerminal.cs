using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class ComputerTerminal : MonoBehaviour
{
    [Header("VCam")]
    public CinemachineVirtualCamera endVCam;

    [Header("AI 对话")]
    [TextArea] public string[] dialogueLines;
    public float lineDelay = 1.5f;
    public float typeSpeed = 0.05f;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("传送")]
    public Transform teleportTarget;
    public GameObject fadePanel;
    public float fadeDuration = 0.5f;

    [Header("第二层对话触发器")]
    public GameObject secondLevelDialogueTrigger;   // 拖入 SecondLevelDialogueTrigger 物体（带 DialogueManager）

    private GameObject player;
    private StarterAssets.FirstPersonController playerController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<StarterAssets.FirstPersonController>();
        if (endVCam != null) endVCam.Priority = 0;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (fadePanel != null) fadePanel.SetActive(false);
    }

    public void StartTransition()
    {
        StartCoroutine(TransitionSequence());
    }

    IEnumerator TransitionSequence()
    {
        // 锁定玩家
        if (playerController != null) playerController.enabled = false;

        // 切换到电脑 VCam
        if (endVCam != null) endVCam.Priority = 10;
        yield return new WaitForSeconds(0.8f);

        // AI 对话
        dialoguePanel.SetActive(true);
        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(lineDelay);
        }
        dialoguePanel.SetActive(false);

        // 淡出
        yield return StartCoroutine(Fade(1f));

        // 强制切回相机
        if (endVCam != null) endVCam.gameObject.SetActive(false);

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null) brain.enabled = false;

        Camera mainCam = Camera.main;
        mainCam.transform.position = teleportTarget.position;
        mainCam.transform.rotation = teleportTarget.rotation;

        player.transform.position = teleportTarget.position;
        player.transform.rotation = teleportTarget.rotation;

        if (brain != null) brain.enabled = true;

        // 淡入
        yield return StartCoroutine(Fade(0f));

        // ========== 激活第二层对话（增强版） ==========
        if (secondLevelDialogueTrigger != null)
        {
            // 确保物体处于激活状态
            if (!secondLevelDialogueTrigger.activeSelf)
            {
                secondLevelDialogueTrigger.SetActive(true);
                Debug.Log("已激活 SecondLevelDialogueTrigger");
            }

            // 等待一帧，确保物体完全激活（防止时序问题）
            yield return null;

            DialogueManager dm = secondLevelDialogueTrigger.GetComponent<DialogueManager>();
            if (dm != null)
            {
                dm.StartDialogue();
                Debug.Log("已调用 DialogueManager.StartDialogue()");
            }
            else
            {
                Debug.LogError("SecondLevelDialogueTrigger 上没有找到 DialogueManager 组件！");
            }
        }
        else
        {
            Debug.LogError("secondLevelDialogueTrigger 未在 Inspector 中拖拽！");
        }

        // 解锁玩家（对话管理器会再次锁定，无影响）
        if (playerController != null) playerController.enabled = true;
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
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