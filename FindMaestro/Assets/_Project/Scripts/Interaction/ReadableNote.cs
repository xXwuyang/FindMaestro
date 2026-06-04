using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class ReadableNote : MonoBehaviour
{
    [Header("Cinemachine 相机")]
    public CinemachineVirtualCamera vcam;   // 拖入 VCam_Notepad

    [Header("文字内容")]
    [TextArea(5, 10)]
    public string infoText = "笔记内容...";

    [Header("UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoTextUI;

    private CinemachineBrain brain;
    private int previousPriority;
    private bool isViewing = false;
    private bool isPlayerNear = false;

    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        if (vcam != null)
            vcam.Priority = 0;
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isViewing)
        {
            StartViewing();
        }
        else if (isViewing && Input.GetKeyDown(KeyCode.E))
        {
            StopViewing();
        }
    }

    void StartViewing()
    {
        isViewing = true;

        // 启用 VCam
        if (vcam != null)
        {
            previousPriority = vcam.Priority;
            vcam.Priority = 10;
        }

        // 显示 UI
        if (infoPanel != null)
        {
            infoTextUI.text = infoText;
            infoPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StopViewing()
    {
        isViewing = false;

        // 恢复原来的相机（把 Priority 设回 0）
        if (vcam != null)
            vcam.Priority = 0;

        if (infoPanel != null)
            infoPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}