using UnityEngine;
using TMPro;
using System.Collections;

public class CreativityFragment : MonoBehaviour
{
    public int fragmentValue = 1;
    [TextArea(2, 4)]
    public string fragmentText = "A jumping game...";
    public float displayDuration = 3f;

    private bool isCollected = false;
    private Coroutine hideCoroutine = null;

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (other.CompareTag("Player"))
        {
            isCollected = true;
            FragmentManager.Instance.AddFragment(fragmentValue);
            ShowPopup();
            gameObject.SetActive(false);
        }
    }

    void ShowPopup()
    {
        if (FragmentManager.Instance == null || FragmentManager.Instance.popupText == null)
            return;

        var popupText = FragmentManager.Instance.popupText;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        popupText.text = fragmentText;
        popupText.gameObject.SetActive(true);

        hideCoroutine = StartCoroutine(HideAfterDelay(popupText));
    }

    IEnumerator HideAfterDelay(TextMeshProUGUI popupText)
    {
        yield return new WaitForSeconds(displayDuration);
        if (popupText != null)
            popupText.gameObject.SetActive(false);
        hideCoroutine = null;

        // 检查是否收集满了所有碎片，如果是，则触发总结（延迟一点点，确保UI稳定）
        if (FragmentManager.Instance != null &&
            FragmentManager.Instance.CurrentFragments >= FragmentManager.Instance.TotalFragments)
        {
            FragmentManager.Instance.StartFinalSequence();
        }
    }

    public void ResetFragment()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}