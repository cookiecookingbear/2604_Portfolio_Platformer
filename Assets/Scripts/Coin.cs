using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") == false) return;

        Player_Mission player = collision.gameObject.GetComponent<Player_Mission>();
        if (player is null)
        {
            Debug.LogError("플레이어와 부딪혔으나 컴포넌트 연결 안됨", this);
            return;
        }

        player.TargetSecured();
        ConsumeCoin();

    }

    private void ConsumeCoin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        anim.SetTrigger("Consumed");

        Invoke("DestroyCoin", 1.0f);
    }

    private void DestroyCoin()
    {
        Destroy(gameObject);
    }
}
