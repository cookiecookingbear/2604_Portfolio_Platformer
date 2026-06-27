using UnityEngine;

public class MissionManager : MonoBehaviour
{
    private PlatformGenerator generator;
    [SerializeField] private SO_MissionInfo info;//미션 스크립터블오브젝트 

    private int floor;                      //현재 PlatformGenerator에서 생성된 가장 높은 층

    //private float missionTime = 0.0f;
    private float missionTimer = 0.0f;
    private SO_MissionInfo.MissionInfo missionInfo;

    private int successCount = 0;
    private int totalTries = 0;

    private bool isOnMission = false;
    public bool IsOnMission => isOnMission;
    private bool missionSuccess = false;


    private void Awake()
    {
        generator = GameObject.Find("PlatformGenerator")?.GetComponent<PlatformGenerator>();
        if (generator is null)
        {
            Debug.LogError("PlatformGenerator가 연결되지 않음", this);
            enabled = false;
        }

        if(info is null)
        {
            Debug.LogError("미션인포 연결안됨", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Update_MissionCountDown();
    }

    public void MissionEnable()             //미션 스위치에서 사용할 메서드?
    {
        if (isOnMission == true) return;    //현재 이미 미션 수행중이라면 리턴.

        missionTimer = missionInfo.missionTime;
        totalTries++;
        isOnMission = true;
    }

    private void Update_MissionCountDown()
    {
        if (isOnMission == false) return;

        missionTimer -= Time.deltaTime;

        if (missionSuccess)
        {
            //reward
            AddSuccessCount();
        }

        if(missionTimer < 0f)
        {
            missionSuccess = false;
            missionTimer = 0f;
            //실패패널티 부과
        }
    }

    private void AddSuccessCount()
    {
        successCount++;
    }

    private void SelectRandomMission()
    {
        int mission = Random.Range(0, (int)SO_MissionInfo.MissionType.Max);

        missionInfo = mission switch
        {
            0 => info.collectCoins,
            1 => info.killMonster,
            _ => info.collectCoins
        };
    }

    //private void GetFloor()
    //{
    //    floor = generator.Floor;
    //}
}
