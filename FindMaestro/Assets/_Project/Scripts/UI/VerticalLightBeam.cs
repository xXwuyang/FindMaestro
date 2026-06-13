using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VerticalLightBeam : MonoBehaviour
{
    [Header("光束设置")]
    public float beamLength = 5.0f;       // 光束的长度
    public float beamWidth = 0.2f;        // 光束的宽度

    private LineRenderer lineRenderer;
    private Vector3 startPoint;
    private Vector3 endPoint;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // 设置线的宽度
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;

        // 设置线的顶点数量（2个点，起点和终点）
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // 起点: 扫描仪本身的位置 (世界坐标)
        startPoint = transform.position;
        // 终点: 从起点位置，向下垂直移动 beamLength 米
        endPoint = startPoint + Vector3.down * beamLength;

        // 更新线段的两个端点
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}