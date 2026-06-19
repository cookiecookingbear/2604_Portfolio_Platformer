using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private Player player;

    private float highestPlayerY;
    [SerializeField] private float offset = 10.0f;

    private void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        Update_GetPlayerYPos();
        Update_SetPosition();
    }

    private void Update_GetPlayerYPos()
    {
        float y = player.transform.position.y;

        if (highestPlayerY< y)
        {
            highestPlayerY = y;

            return;
        }
    }

    private void Update_SetPosition()
    {
        transform.position = new Vector2(0f, highestPlayerY - offset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject);

            return;
        }

        /*if (collision.gameObject.CompareTag("Player"))
        {
            //게임오버
        }*/
    }
}
