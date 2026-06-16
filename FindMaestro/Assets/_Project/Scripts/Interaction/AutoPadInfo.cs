using UnityEngine;
using TMPro;   // 关键：引入 TextMeshPro 命名空间

public class AutoPadInfo : MonoBehaviour
{
    [Header("消息面板（UI Panel）")]
    public GameObject messagePanel;

    [Header("消息文本（TMP）")]
    public TextMeshProUGUI messageText;   // 改为 TMP 类型

    [Header("消息内容")]
    [TextArea(3, 5)]
    public string message = "Friend: 'Hey! Try this AI – it writes games in seconds!\nNo more all‑night debugging!'";

    [Header("显示时间（秒）")]
    public float displayTime = 3f;

    [Header("激活的电脑UI")]
    public GameObject computerUI;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            if (messageText != null) messageText.text = message;
            if (messagePanel != null) messagePanel.SetActive(true);
            Invoke("ActivateComputerUI", displayTime);
        }
    }

    void ActivateComputerUI()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
        if (computerUI != null) computerUI.SetActive(true);
    }
}