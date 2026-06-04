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

    private enum PieceType
    {
        Normal,
        Hole,
        Reward,
        End
    }
    [SerializeField] private GameObject platformPrefab;
    private float xPosition = 2.0f;     //빈 부모 오브젝트 글로벌 x좌표
    [SerializeField] private float yInterval = 4.0f;     //층간격, 빈 부모 오브젝트에 적용
    private float roll = 2.5f;          //기울기, locatingSide 계산하여 빈 부모 오브젝트에 적용
    //roll 대신에 우측 플랫폼에 y회전 180을 주면 그대로 좌측 플랫폼 모양이 됨.
    private int locatingSide = 1;       //플랫폼 배치 위치(좌 = -1, 우 = 1)

    private float lastYPosition = -5.4f;    // 마지막으로 만들어진 빈 부모 오브젝트의 y좌표

    private float playerYPos;
    private float highestPlayerYpos;          //플레이어 최고높이
    private int floor = 1;

    private GameObject parent;


    private Queue<GameObject> emptyParentObjects = new Queue<GameObject>();//이거 근데 인덱스 접근 안되는데 디큐 하고서 디스트로이 어케할..가 아니라 디큐를 변수로 받아서 그걸 부수면 되겠구나 넣을때도 변수로 만들고 담는게 맞을듯


    private Player player;

    private void Awake()
    {
        player = GameObject.Find("Player")?.GetComponent<Player>();
        if(player is null)
        {
            Debug.Log("플랫폼 생성기에 player연결 안됐음", this);
            enabled = false;
            return;
        }

        if(platformPrefab is null)
        {
            Debug.Log("플랫폼 생성기에 플랫폼 프리팹 연결 안됐음", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        Update_Get_Player_YPos();
        Update_Generate_Platform();
        Update_DestroyPlatformParent();
    }

    private void Update_Get_Player_YPos()
    {
        playerYPos = player.YPos;
        
        if(highestPlayerYpos < playerYPos)
        {
            highestPlayerYpos = playerYPos;
        }

    }

    private void Update_Generate_Platform()
    {
        if(lastYPosition < highestPlayerYpos + yInterval * 2)
        {
            AddParent();
            InsertPlatform();
        }
    }

    private void AddParent()
    {
        locatingSide *= -1;


        Vector3 position = Vector3.zero;
        position.x = xPosition * locatingSide;
        position.y = lastYPosition + yInterval;

        parent = new GameObject($"floor{floor}");

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
    }

    private void InsertPlatform()
    {
        Instantiate(platformPrefab, parent.transform, false);
        floor++;

        print($"{floor}층의 부모에 플랫폼 삽입됨");

    }

    private void Update_DestroyPlatformParent()
    {
        if (emptyParentObjects.Count <= 7) return;
       
        GameObject destroy = emptyParentObjects.Dequeue();
        Destroy(destroy);
    }
}
