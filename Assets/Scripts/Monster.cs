using UnityEngine;

public class Monster : MonoBehaviour
{


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Vector2 normal = -collision.GetContact(0).normal;

        float upDot = Vector2.Dot(normal, Vector3.up);
        float rightDot = Vector2.Dot(normal, Vector3.right);

        if(upDot > rightDot)
        {
            //몹 밟히고 1초뒤 삭제 or 풀 반환
            print("잘 밟혔음");
        }
    }


}
