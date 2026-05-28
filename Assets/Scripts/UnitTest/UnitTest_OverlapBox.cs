using UnityEngine;

public class UnitTest_OverlapBox : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 size = new Vector2(0.1f, 0.1f);

    private bool isGrounded = false;

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            groundCheck.position,
            size,
            0f,
            groundMask);
        print(hit);
        isGrounded = hit != null;

        DrawOverlapBoxDebug();
    }

    private void DrawOverlapBoxDebug()
    {
        Color resultColor = isGrounded ? Color.green : Color.red;

        DrawDebugBox(groundCheck.position, size, resultColor);
    }

    private void DrawDebugBox(Vector2 center, Vector2 boxSize, Color color)
    {
        Vector2 halfSize = boxSize * 0.5f;

        Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }
}
