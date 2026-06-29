using System;
using UnityEngine;
using MissionInfo = SO_MissionInfo.MissionInfo;

public class MissionManager : MonoBehaviour
{
    private PlatformGenerator generator;
    [SerializeField] private SO_MissionInfo info;//미션 스크립터블오브젝트 
    private Player_Mission playerMission;

    private int floor;                      //미션스위치측에서 전달받은 층(활성화한 미션스위치의 층)
    //private float missionTime = 0.0f;
    private float missionTimer = 0.0f;
    private MissionInfo missionInfo;

    private int successCount = 0;
    private int totalTries = 0;
    private int targetCount = 0;

    private bool isOnMission = false;
    public bool IsOnMission => isOnMission;
    private bool missionSuccess = false;

    public event Action<MissionInfo, int> MissionStarted;       //뒤쪽의 int에는 미션스위치를 누른 층 입력


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

        playerMission = GameObject.FindAnyObjectByType<Player_Mission>();
        if (playerMission is null)
        {
            Debug.LogError("PlayerMission연결안됨", this);
            enabled = false;
        }
    }

    private void Start()
    {
        playerMission.MissionTargetCount += AddTargetCount;
    }

    private void Update()
    {
        Update_MissionCountDown();
    }

    public void MissionEnable(int floor)             //미션 스위치에서 사용할 메서드?
    {
        if (isOnMission == true) return;    //현재 이미 미션 수행중이라면 리턴.

        totalTries++;
        isOnMission = true;

        SelectRandomMission();
        //스위치가 켜지는 동시에 미션을 고르고, 유관 변수에 값이 입력되는 등의 작업을 거치면, 짧은 프레임 안에 너무 많은 일들이 일어나니까, 미리 미션정보를 로드하는건 어떨까?
        missionTimer = missionInfo.missionTime;
        MissionStarted?.Invoke(missionInfo, floor);
    }

    private void Update_MissionCountDown()
    {
        if (isOnMission == false) return;

        missionTimer -= Time.deltaTime;

        if (missionSuccess)
        {
            //TODO : reward
            AddSuccessCount();
            return;
        }

        if(missionTimer < 0f)
        {
            MissionFail();
            return;
        }
    }

    private void AddSuccessCount()
    {
        successCount++;
        isOnMission = false;
    }

    private void SelectRandomMission()
    {
        int mission = UnityEngine.Random.Range(0, (int)SO_MissionInfo.MissionType.Max);

        missionInfo = mission switch
        {
            0 => info.collectCoins,
            1 => info.killMonster,
            _ => info.collectCoins
        };
    }

    private void MissionFail()
    {
        missionSuccess = false;
        missionTimer = 0f;
        isOnMission = false;
        print("미션실패");
        //TODO : penalty
    }

    private void AddTargetCount()
    {
        targetCount++;

        if(targetCount >= missionInfo.targetCount)
        {
            missionSuccess = true;
        }
    }

    private void OnGUI()
    {
        if (missionTimer < 0f) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.green;
                
        string text = $"{missionTimer}";

        GUI.Label(new Rect(5, 525, Screen.width, 20), text, style);
    }
}
