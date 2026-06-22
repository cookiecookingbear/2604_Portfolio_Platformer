using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    // 몇층까지 초기 생성해놓을건지. 3층? 5층?
    // 층간 간격은 y축 3유닛, x좌표 좌플랫폼 -2, 우플랫폼 2
    // 한 층의 x스케일이 15 => 각 조각의 x스케일이 3 , 모든 조각 y스케일 0.5
    // 어차피 각 조각을 담을 부모 오브젝트가 필요하므로, 시작 단계에서도 빈 부모 오브젝트를 생성하고 그 하위 항목으로 실제 플랫폼 생성하도록 설정
    // 플랫폼 생성 신호는 어떤 방식으로 받을건지?
    // 플랫폼 삭제 신호는 어떻게 받을건지? 어디까지 삭제할건지?
    // 플랫폼을 큐나 배열에 담았다가 삭제할까? 배열은 매번 앞으로 당겨야 할것 같으니 큐가 적당해 보일것 같긴 함

    // 초기 시작시 플랫폼이 없다가 베이스에 발이 닿으면 그때 큐에 담긴 빈 부모 오브젝트의 개수를 받아서 3개 미만이면 일단 세개를 한꺼번에 생성하도록? 아님 5개 다 만들어놔도 되고.

    public enum PieceType
    {
        Normal,
        Hole,
        Reward,
        End
    }

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject floor_NormalPrefab;
    [SerializeField] private GameObject floor_HolePrefab;
    [SerializeField] private GameObject floor_EndPrefab;

    private float xPosition = 2.0f;                         //빈 부모 오브젝트 글로벌 x좌표
    [SerializeField] private float yInterval = 4.8f;        //층간격, 빈 부모 오브젝트에 적용
    private float roll = 2.5f;                              //기울기, locatingSide 계산하여 빈 부모 오브젝트에 적용
                                                             //roll 대신에 우측 플랫폼에 y회전 180을 주면 그대로 좌측 플랫폼 모양이 됨.
    private int locatingSide = 1;                           //플랫폼 배치 위치(좌 = -1, 우 = 1)

    private float lastYPosition = -5.4f;                    // 마지막으로 만들어진 빈 부모 오브젝트의 y좌표

    private float highestPlayerYpos;                        //플레이어 최고높이
    private int floor = 1;

    private Queue<GameObject> emptyParentObjects = new Queue<GameObject>();


    private Player player;
    private PlatformRatioGenerator ratio;

    private void Awake()
    {
        player = GameObject.Find("Player")?.GetComponent<Player>();
        if(player is null)
        {
            Debug.Log("플랫폼 생성기에 player연결 안됐음", this);
            enabled = false;
            return;
        }

        ratio = GetComponent<PlatformRatioGenerator>();
        
    }

    private void Update()
    {
        Update_GetHighestPlayerY();
        Update_Generate_Platform();
        Update_DestroyPlatformParent();
    }


    private void Update_GetHighestPlayerY()
    {
        highestPlayerYpos = player.HighestYPos;
    }
    private void Update_Generate_Platform()
    {
        if(lastYPosition < highestPlayerYpos + yInterval * 2)
        {
            AddPlatform();
        }
    }

    private void AddPlatform()
    {
        locatingSide *= -1;


        Vector3 position = Vector3.zero;
        position.x = xPosition * locatingSide;
        position.y = lastYPosition + yInterval;

        GameObject parent = Instantiate(floorPrefab);
        parent.name = $"floor : {floor}";
        floor++;

        parent.transform.position = position;

        if (locatingSide > 0)
        {
            parent.transform.rotation = Quaternion.Euler(0f, 0f, roll);
        }
        else
        {
            parent.transform.rotation = Quaternion.Euler(0f, 180f, roll);
        }

        lastYPosition = position.y;

        emptyParentObjects.Enqueue(parent);

        PieceType[] platformType = ratio.MakePlatform();

        for (int i = 0; i < platformType.Length; i++)
        {
            Transform slot = parent.transform.GetChild(i);

            GameObject piece = GetPieceType(platformType[i]);

            Instantiate(piece, slot, false);
        }
        
    }

    private GameObject GetPieceType(PlatformGenerator.PieceType pieceType)
    {
        switch (pieceType)
        {
            case PlatformGenerator.PieceType.Normal:
                return floor_NormalPrefab;
            case PlatformGenerator.PieceType.Hole:
                return floor_HolePrefab;
            default:
                return floor_EndPrefab;
        }
    }
    
    private void Update_DestroyPlatformParent()
    {
        if (emptyParentObjects.Count <= 5) return;
       
        GameObject destroy = emptyParentObjects.Dequeue();
        Destroy(destroy);
    }
}
