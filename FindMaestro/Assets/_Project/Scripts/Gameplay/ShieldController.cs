using UnityEngine;
using System.Collections;

public class ShieldController : MonoBehaviour
{
    public GameObject[] fragments;   // 拖入5个金豆模型
    public float rotateSpeed = 90f;
    public float floatAmplitude = 0.2f;
    public float floatSpeed = 2f;

    private Vector3[] startPositions;

    void Start()
    {
        if (fragments.Length == 0) return;
        startPositions = new Vector3[fragments.Length];
        for (int i = 0; i < fragments.Length; i++)
        {
            startPositions[i] = fragments[i].transform.position;
            fragments[i].SetActive(false);
        }
    }

    public void ActivateShield()
    {
        for (int i = 0; i < fragments.Length; i++)
        {
            fragments[i].SetActive(true);
        }
        StartCoroutine(AnimateShield());
    }

    IEnumerator AnimateShield()
    {
        float time = 0;
        while (true)
        {
            time += Time.unscaledDeltaTime;
            // 围绕Y轴旋转
            for (int i = 0; i < fragments.Length; i++)
            {
                float angle = time * rotateSpeed + (i * 360f / fragments.Length);
                float rad = angle * Mathf.Deg2Rad;
                // 原位置为基础，增加旋转偏移（如果希望它们围成圈，可以改变位置）
                // 这里简单让它们围绕中心点旋转：假设中心点是玩家位置或原点
                Vector3 center = Camera.main.transform.position; // 或者指定一个空物体
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad * 0.5f), Mathf.Sin(rad)) * 2f;
                fragments[i].transform.position = center + offset + Vector3.up * Mathf.Sin(time * floatSpeed) * floatAmplitude;
            }
            yield return null;
        }
    }
}