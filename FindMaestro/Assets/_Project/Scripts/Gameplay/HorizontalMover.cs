using UnityEngine;

public class HorizontalMover : MonoBehaviour
{
    public float leftBound = -5f;    // 左边界 X
    public float rightBound = 5f;    // 右边界 X
    public float speed = 2f;         // 移动速度

    private float direction = 1f;

    void Update()
    {
        float newX = transform.position.x + direction * speed * Time.deltaTime;
        if (newX > rightBound) { newX = rightBound; direction = -1f; }
        if (newX < leftBound) { newX = leftBound; direction = 1f; }
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}