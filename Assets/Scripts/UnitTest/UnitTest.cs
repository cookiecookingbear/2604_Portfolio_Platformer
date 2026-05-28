using UnityEngine;

public class UnitTest : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 size = new Vector2(0.1f, 0.1f);

    private bool isGrounded = false;

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        RaycastHit2D hit = Physics2D.BoxCast(
            groundCheck.position,
            size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundMask);

        isGrounded = hit.collider != null;

        DrawBoxCastDebug();
    }

    private void DrawBoxCastDebug()
    {
        Vector2 startCenter = groundCheck.position;
        Vector2 endCenter = startCenter + Vector2.down * groundCheckDistance;
        Color resultColor = isGrounded ? Color.green : Color.red;

        DrawDebugBox(startCenter, size, Color.yellow);
        DrawDebugBox(endCenter, size, resultColor);

        Debug.DrawLine(startCenter, endCenter, Color.cyan);
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
