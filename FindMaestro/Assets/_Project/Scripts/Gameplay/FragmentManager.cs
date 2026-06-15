using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI fragmentCounterText;
    public int totalFragmentsToCollect = 5;

    [Header("碎片列表")]
    public List<CreativityFragment> allFragments;

    [Header("总结文本")]
    public GameObject finalMessagePanel;
    public TextMeshProUGUI finalMessageText;
    public float finalDisplayDuration = 5f;
    public string finalMessage = "These things… they crash, they're full of bugs… but they remind me: when a human creates, the heart is warm. The AI tried to erase this, but creativity never dies.";

    [Header("共享弹窗文本")]
    public TextMeshProUGUI popupText;   // 所有碎片共用的 UI 文本

    [Header("传送设置")]
    public Transform teleportDestination;
    public string nextSceneName = "Level3_EarlyAI";
    public GameObject fadePanel;
    public float fadeDuration = 1f;

    private int currentFragments = 0;
    private GameObject player;
    private StarterAssets.FirstPersonController playerController;
    private bool finalSequenceStarted = false;  // 防止重复触发

    public int CurrentFragments => currentFragments;
    public int TotalFragments => totalFragmentsToCollect;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<StarterAssets.FirstPersonController>();

        if (allFragments == null || allFragments.Count == 0)
            allFragments = new List<CreativityFragment>(FindObjectsOfType<CreativityFragment>());

        UpdateUI();
        if (finalMessagePanel != null)
            finalMessagePanel.SetActive(false);
        if (popupText != null)
            popupText.gameObject.SetActive(false);
        if (fadePanel != null)
            fadePanel.SetActive(false);
    }

    public void AddFragment(int amount)
    {
        currentFragments += amount;
        UpdateUI();

        if (ExposureSystem.Instance != null)
            ExposureSystem.Instance.ReduceExposure(10f);

        // 集齐所有碎片，延迟触发总结（等待当前碎片文本显示完）
        if (currentFragments == totalFragmentsToCollect)
        {
            StartCoroutine(DelayedFinalSequence());
        }
    }

    private IEnumerator DelayedFinalSequence()
    {
        // 等待当前碎片文本的显示时长（假设所有碎片 displayDuration 一致，或可配置）
        float delay = 3f; // 可改为从第一个碎片获取，简单起见用3
        yield return new WaitForSeconds(delay);
        StartFinalSequence();
    }

    public void ResetFragments()
    {
        currentFragments = 0;
        UpdateUI();
        foreach (var f in allFragments)
            if (f != null) f.ResetFragment();
    }

    void UpdateUI()
    {
        if (fragmentCounterText != null)
            fragmentCounterText.text = $"Fragments: {currentFragments}/{totalFragmentsToCollect}";
    }

    // 由最后一个碎片在文本消失后调用
    public void StartFinalSequence()
    {
        if (finalSequenceStarted) return;
        finalSequenceStarted = true;
        StartCoroutine(ShowFinalMessageAndTransition());
    }

    IEnumerator ShowFinalMessageAndTransition()
    {
        // 确保当前碎片文本已隐藏（已由碎片脚本处理）
        if (popupText != null)
            popupText.gameObject.SetActive(false);

        // 显示总结面板
        if (finalMessagePanel != null)
        {
            finalMessageText.text = finalMessage;
            finalMessagePanel.SetActive(true);
            yield return new WaitForSeconds(finalDisplayDuration);
            finalMessagePanel.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        AIFuryController fury = FindObjectOfType<AIFuryController>();
        if (fury != null)
            fury.StartFury();
        else
            Debug.LogError("未找到 AIFuryController，请确保场景中有该组件");

        //// 传送或加载场景
        //yield return StartCoroutine(TransitionToNextLayer());
    }

    IEnumerator TransitionToNextLayer()
    {
        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return StartCoroutine(Fade(1f));

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

        yield return StartCoroutine(Fade(0f));

        if (playerController != null)
            playerController.enabled = true;
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

    public void HideCurrentPopup()
    {
        if (popupText != null)
            popupText.gameObject.SetActive(false);
    }
}