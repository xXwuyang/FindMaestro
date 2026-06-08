using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DoorLock : MonoBehaviour
{
    [Header("UI")]
    public GameObject passwordPanel;
    public TMP_InputField inputField;
    public Button submitButton;

    [Header("密码")]
    public string correctPassword = "17";

    private bool isPlayerNear = false;

    void Start()
    {
        passwordPanel.SetActive(false);
        submitButton.onClick.AddListener(CheckPassword);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !passwordPanel.activeSelf)
        {
            passwordPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CheckPassword()
    {
        if (inputField.text == correctPassword)
        {
            passwordPanel.SetActive(false);
            // 找到场景中的电脑终端，触发过场动画
            ComputerTerminal terminal = FindObjectOfType<ComputerTerminal>();
            if (terminal != null)
                terminal.StartTransition();
            else
                Debug.LogError("未找到 ComputerTerminal，请确保场景中有挂载 ComputerTerminal 脚本的物体");
        }
        else
        {
            inputField.text = "";
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