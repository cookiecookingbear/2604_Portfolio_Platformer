using UnityEditor;
using UnityEngine;

public class Player_CameraPositionObject : MonoBehaviour
{

    [SerializeField, Tooltip("캐릭터와의 거리 인셋")] private float inset = 15.0f;
    [SerializeField, Tooltip("카메라용 오브젝트 이동속도")] private float moveSpeed =35.0f;

    private int facingDirection;
    private Player player;
    //private Vector3 position;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void LateUpdate()
    {
        if (player is not null)
        {
            facingDirection = player.FacingDirection;
        }
        else print("플레이어 스크립트 연결 실패");

        LateUpdate_LocalPosition();
    }

    private void LateUpdate_LocalPosition()
    {
        Vector3 localPosition = transform.localPosition;

        localPosition.x = Mathf.MoveTowards(localPosition.x, inset * facingDirection, moveSpeed * Time.deltaTime);
        transform.localPosition = localPosition;
    }

    

    //private void OnDrawGizmos()
    //{
    //    if (player is null) return;

    //    GUIStyle style = GUIStyle.none;
    //    style.normal.textColor = Color.black;
    //    style.alignment = TextAnchor.MiddleCenter;


    //    string str = "";
    //    str += "direction" + facingDirection;

    //    Handles.Label((Vector2)transform.position + new Vector2(0, 1.25f), str, style);
    //}
}
