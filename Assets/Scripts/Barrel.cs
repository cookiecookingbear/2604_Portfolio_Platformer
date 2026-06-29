using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class Barrel : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;

    private float randomGravity;
    private float minGravity = 3.0f;
    private float maxGravity = 5.0f;

    private void Awake()
    {
        //player = GameObject.Find("Player")?.GetComponent<Player>();
        //if (player == null)
        //{
        //    Debug.LogError("player연결안됨", this);
        //    enabled = false;
        //}

        rb = GetComponent<Rigidbody2D>();

        randomGravity = Random.Range(minGravity, maxGravity);
    }

    private void OnEnable()
    {
        rb.gravityScale = randomGravity;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player is null) return;
            //플레이어 튕겨내기
            Vector2 normal = -collision.contacts[0].normal;
            player.HitbyBarrel(normal);
            Destroy(gameObject);

            //print(normal);  
            

            return;
        }

    }


}
