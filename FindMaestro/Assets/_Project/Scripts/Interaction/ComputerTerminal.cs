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
    public float lineDelay = 1.5f;          // 每行显示完后的额外停顿
    public float typeSpeed = 0.05f;         // 每个字符出现的间隔（秒）
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("传送")]
    public Transform teleportTarget;
    public GameObject fadePanel;
    public float fadeDuration = 0.5f;

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

        // 逐行显示对话（逐字打印）
        dialoguePanel.SetActive(true);
        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(lineDelay);
        }
        dialoguePanel.SetActive(false);

        // 淡出
        yield return StartCoroutine(Fade(1f));

        // 强制切回相机（禁用 VCam 物体，避免回切动画）
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

        // 解锁玩家
        if (playerController != null) playerController.enabled = true;
    }

    // 逐字打印协程
    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            // 可选：在这里播放打字音效
            // AudioSource.PlayClipAtPoint(typingSound, Camera.main.transform.position);
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