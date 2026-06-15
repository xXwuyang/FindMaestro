using UnityEngine;
using UnityEngine.UI;

public class ExposureSystem : MonoBehaviour
{
    public static ExposureSystem Instance;

    [Header("Exposure Value")]
    public float exposure = 0f;
    public float maxExposure = 100f;

    [Header("Rates")]
    public float decayRate = 1.5f;
    public bool freezeDecay = false;

    [Header("UI")]
    public Image exposureBar;

    [Header("Visual Pressure")]
    public CanvasGroup blurPanel;
    public CanvasGroup pressurePanel;
    public float pulseSpeed = 2f;
    

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleExposureDecay();

        exposure = Mathf.Clamp(exposure, 0, maxExposure);

        UpdateUI();
        UpdateVisualPressure();
        UpdateBlurEffect();
        UpdateCameraFOV();
    }

    // =====================
    // EXPOSURE LOGIC
    // =====================
    void HandleExposureDecay()
    {
        if (!freezeDecay)
        {
            exposure -= decayRate * Time.deltaTime;

            if (exposure < 2f)
            {
                exposure = Mathf.Lerp(exposure, 0, Time.deltaTime * 0.5f);
            }
        }
    }

    public void AddExposure(float value)
    {
        exposure += value;
        exposure = Mathf.Clamp(exposure, 0, maxExposure);
    }

    public void ReduceExposure(float value)
    {
        exposure -= value;
        exposure = Mathf.Clamp(exposure, 0, maxExposure);
    }

    public bool IsHigh()
    {
        return exposure > maxExposure * 0.7f;
    }

    // =====================
    // UI
    // =====================
    void UpdateUI()
    {
        if (exposureBar != null)
        {
            float value = exposure / maxExposure;
            exposureBar.rectTransform.localScale =
                new Vector3(value, 1f, 1f);
        }
    }

    // =====================
    // VISUAL PRESSURE
    // =====================
    void UpdateVisualPressure()
    {
        if (pressurePanel == null) return;

        float t = exposure / maxExposure;

        // 基础红色压迫
        float baseAlpha = Mathf.Lerp(0f, 0.45f, t);

        // 呼吸闪烁
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.05f;

        pressurePanel.alpha = Mathf.Clamp01(baseAlpha + pulse * t);
    }

    void UpdateBlurEffect()
    {
        if (blurPanel == null) return;

        float t = exposure / maxExposure;

        // 模糊强度
        blurPanel.alpha = Mathf.Lerp(0f, 0.3f, t);
    }

    // =====================
    // CAMERA PRESSURE
    // =====================
    void UpdateCameraFOV()
    {
        if (Camera.main == null) return;

        float t = exposure / maxExposure;

        Camera.main.fieldOfView =
            Mathf.Lerp(60f, 75f, t);
    }
}