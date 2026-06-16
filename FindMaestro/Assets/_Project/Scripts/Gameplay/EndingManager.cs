using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("UI 组件")]
    public Image fadeImage;
    public TextMeshProUGUI displayText;

    [Header("光源（安装结局闪回）")]
    public Light sunLight;
    public Light warmLight;
    public int flashCount = 2;
    public float flashDuration = 0.3f;

    [Header("碎片护盾（金豆）")]
    public GameObject[] shieldFragments;

    [Header("电脑面板及按钮")]
    public GameObject computerPopup;
    public GameObject installButton;
    public GameObject rejectButton;
    public GameObject skipEggButton;
    public GameObject showEggButton;

    private bool hasOldKit = false;
    private bool waitingForEgg = false;

    void Start()
    {
        if (computerPopup != null) computerPopup.SetActive(false);
        if (displayText != null) displayText.gameObject.SetActive(false);
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);
        if (skipEggButton != null) skipEggButton.SetActive(false);
        if (showEggButton != null) showEggButton.SetActive(false);
    }

    public void ShowComputerPopup()
    {
        if (computerPopup != null) computerPopup.SetActive(true);
    }

    public void OnInstall()
    {
        StartCoroutine(InstallEnding());
    }

    public void OnReject()
    {
        if (installButton != null) installButton.SetActive(false);
        if (rejectButton != null) rejectButton.SetActive(false);
        if (skipEggButton != null) skipEggButton.SetActive(true);
        if (showEggButton != null) showEggButton.SetActive(true);
    }

    public void OnSkipEgg()
    {
        // 清理按钮
        if (skipEggButton != null) skipEggButton.SetActive(false);
        if (showEggButton != null) showEggButton.SetActive(false);
        // 关闭电脑面板
        if (computerPopup != null) computerPopup.SetActive(false);
        // 启动拒绝结局协程（普通结局）
        StartCoroutine(RejectEndingSequence(false));
    }

    public void OnShowEgg()
    {
        if (skipEggButton != null) skipEggButton.SetActive(false);
        if (showEggButton != null) showEggButton.SetActive(false);
        if (computerPopup != null) computerPopup.SetActive(false);
        displayText.gameObject.SetActive(true);
        displayText.text = "Easter egg is hidden near the stair corner!\nGo find it – the ending will trigger automatically.";
        waitingForEgg = true;
    }

    public void OnEggFound()
    {
        if (!waitingForEgg) return;
        waitingForEgg = false;
        StopAllCoroutines();
        StartCoroutine(RejectEndingSequence(true));
    }

    IEnumerator RejectEndingSequence(bool foundEgg)
    {
        // 关闭电脑面板（再次确保）
        if (computerPopup != null) computerPopup.SetActive(false);
        // 淡入黑屏
        fadeImage.gameObject.SetActive(true);
        float t = 0;
        float fadeDuration = 0.5f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);

        // 构造结局文字
        string endingMsg;
        if (foundEgg)
            endingMsg = "You refused AI and found the programmer's old dev kit.\nLine by line, bug by bug, you code your own world.\n\nAchievement: Pure Creator.\n\n";
        else
            endingMsg = "You refused AI, but you haven't found the old tools yet.\nMaybe there is another way...\n\n";

        endingMsg += "Remember: You are a true Maestro yourself.\nSee you in the future!";
        displayText.text = endingMsg;
        displayText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        // 退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator InstallEnding()
    {
        if (computerPopup != null) computerPopup.SetActive(false);
        displayText.gameObject.SetActive(true);
        displayText.text = "Ruins flash before your eyes...";
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < flashCount; i++)
        {
            if (sunLight != null) sunLight.enabled = false;
            if (warmLight != null) warmLight.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            if (sunLight != null) sunLight.enabled = true;
            if (warmLight != null) warmLight.enabled = true;
            yield return new WaitForSeconds(flashDuration);
        }

        foreach (var frag in shieldFragments)
            if (frag != null) frag.SetActive(true);
        displayText.text = "The 5 fragments glow, forming a shield around you.";
        yield return new WaitForSeconds(2f);

        displayText.text = "\"Tools are neither good nor evil - it's the human heart that matters.\"";
        yield return new WaitForSeconds(2.5f);

        fadeImage.gameObject.SetActive(true);
        float t = 0;
        float fadeDuration = 0.5f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);

        string finalMsg = "You walk into a new world, where AI assists but humans create.\n\nAchievement: Symbiotic Maestro.\n\nRemember: You are a true Maestro yourself.\nSee you in the future!";
        displayText.text = finalMsg;
        yield return new WaitForSeconds(5f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetHasOldKit(bool value)
    {
        hasOldKit = value;
    }
}