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
        player = GameObject.Find("Player")?.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("player연결안됨", this);
            enabled = false;
        }

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
            //플레이어 튕겨내기
            Vector2 normal = -collision.contacts[0].normal;
            player.HitbyBarrel(normal);
            Destroy(gameObject);

            //print(normal);  
            

            return;
        }

        //if (collision.gameObject.CompareTag("Destroyer"))
        //{
        //    //플레이어 주위를 감싸는 플레이어 이동방지막(좌우) + 낙사판정판(하단)중 낙사판정판에 닿으면 배럴 삭제
        //    //TODO

        //    return;
        //}
    }


}
