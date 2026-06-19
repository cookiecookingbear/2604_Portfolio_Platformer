using UnityEngine;



/// <summary>
/// 플레이어의 y위치를 받아 플레이어가 플랫폼 밖으로 나갈 수 없도록 제한하는 투명 콜라이더 벽 생성
/// 6/19일 작업 기준으로 플랫폼 양 끝이 +-9.43정도이므로, 투명 콜라이더 벽면이 +-9.4정도에 오게끔 하면 될 것으로 보임
/// 다만, 콜라이더 벽의 위치 조정은 별도 스크립트로 조정하는게 아닌 부모자식관계에 의해 자동으로 따라오게끔 설계
/// 
/// 문제점:
/// 콜라이더를 자식에 그냥 적용하니까 이게 플레이어의 일부가 되어버려서 공중부양해버리고 플랫폼에 걸림..
/// 콜라이더 레이어에서 플레이어만 충돌하게 두면 될것같음-해결
/// 
/// 벽 위치를 스크립팅하지 않았더니 플레이어와 간격을 유지하면서 플레이어와 같이 움직임
/// 
/// </summary>
public class XLimitWalls : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject LWall;
    [SerializeField] private GameObject RWall;


    private float currentPlayerYPos;

    private void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        Update_GetPlayerYPos();
        Update_ParentWallPosition();
    }


    private void Update_GetPlayerYPos()
    {
        currentPlayerYPos = player.transform.position.y;
    }

    private void Update_ParentWallPosition()
    {
        transform.position = new Vector2(0f, currentPlayerYPos);
    }

}
