using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DoorLock : MonoBehaviour
{
    [Header("UI")]
    public GameObject passwordPanel;
    public TMP_InputField inputField;
    public Button submitButton;

    [Header("密码")]
    public string correctPassword = "20260608";

    private bool isPlayerNear = false;
    private bool isUIOpen = false;

    void Start()
    {
        passwordPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(CheckPassword);
    }

    void Update()
    {
        // 打开UI
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isUIOpen)
        {
            OpenUI();
        }
        // UI已打开
        else if (isUIOpen)
        {
            // 回车提交
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                CheckPassword();
            }

            // E关闭UI
            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseUI();
            }
        }
    }

    void OpenUI()
    {
        isUIOpen = true;
        passwordPanel.SetActive(true);

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inputField.text = "";

        // 延迟一帧再聚焦（UI稳定关键）
        StartCoroutine(FocusInput());
    }

    void CloseUI()
    {
        isUIOpen = false;
        passwordPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputField.text = "";
    }

    IEnumerator FocusInput()
    {
        yield return null;

        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    void CheckPassword()
    {
        string input = inputField.text.Trim();

        if (input == correctPassword)
        {
            CloseUI();

            ComputerTerminal terminal = FindObjectOfType<ComputerTerminal>();
            if (terminal != null)
            {
                terminal.StartTransition();
            }
            else
            {
                Debug.LogError("未找到 ComputerTerminal");
            }
        }
        else
        {
            inputField.text = "";

            // 重新聚焦（失败后继续输入）
            StartCoroutine(FocusInput());

            Debug.Log("密码错误");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}