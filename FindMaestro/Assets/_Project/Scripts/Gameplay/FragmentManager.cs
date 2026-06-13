using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FragmentManager : MonoBehaviour
{
    public static FragmentManager Instance { get; private set; }

    [Header("UI 显示")]
    public TextMeshProUGUI fragmentCounterText;
    public int totalFragmentsToCollect = 5;

    [Header("碎片列表（自动收集或手动拖拽）")]
    public List<CreativityFragment> allFragments;

    private int currentFragments = 0;

    // ===== 添加这两个公共属性 =====
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
        if (allFragments == null || allFragments.Count == 0)
        {
            allFragments = new List<CreativityFragment>(FindObjectsOfType<CreativityFragment>());
        }
        UpdateUI();
    }

    public void AddFragment(int amount)
    {
        currentFragments += amount;
        UpdateUI();

        if (currentFragments >= totalFragmentsToCollect)
        {
            OnAllFragmentsCollected();
        }
    }

    public void ResetFragments()
    {
        currentFragments = 0;
        UpdateUI();

        foreach (var frag in allFragments)
        {
            if (frag != null)
                frag.ResetFragment();
        }

        Debug.Log("所有碎片已重置，计数归零");
    }

    void UpdateUI()
    {
        if (fragmentCounterText != null)
            fragmentCounterText.text = $"Fragments: {currentFragments}/{totalFragmentsToCollect}";
    }

    void OnAllFragmentsCollected()
    {
        Debug.Log("所有碎片收集完毕！可以打开大门或触发结局。");
    }
}