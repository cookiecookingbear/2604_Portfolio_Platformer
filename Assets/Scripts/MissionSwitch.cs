using TMPro;
using UnityEngine;

public class MissionSwitch : MonoBehaviour
{
    private MissionManager missionManager;
    private PlatformGenerator platformGenerator;
    private Transform player;

    private GameObject off;
    private GameObject on;
    [SerializeField] private TextMeshProUGUI interactionableStat;

    private bool interactionable = false;

    private void Awake()
    {
        missionManager = GameObject.FindAnyObjectByType<MissionManager>();
        if(missionManager is null)
        {
            Debug.LogError("미션매니저 연결안됨", this);
            enabled = false;
        }

        platformGenerator = GameObject.FindAnyObjectByType<PlatformGenerator>();
        if (platformGenerator is null)
        {
            Debug.LogError("플랫폼생성기 연결안됨", this);
            enabled = false;
        }

        player = GameObject.Find("Player").transform;
        if(player is null)
        {
            Debug.LogError("플레이어 연결안됨", this);
            enabled = false;
        }

        on = transform.GetChild(0).gameObject;
        off = transform.GetChild(1).gameObject;

        interactionableStat.enabled = false;
    }

    private void Update()
    {
        if (missionManager.IsOnMission == true) return;
        GetDistanceToPlayer();
    }

    private void GetDistanceToPlayer()
    {
        float distance = Vector2.Distance(this.transform.position, player.transform.position);

        if(distance < 1.75f)
        {
            EnableInteraction();
            interactionable = true;
        }
        else
        {
            DisableInteraction();
        }
    }

    private void EnableInteraction()
    {
        interactionableStat.enabled = true;
        int floor = GetRootFloor();

        if (floor % 2 != 0)
        {
            interactionableStat.rectTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //missionStart
            //missionManager.MissionEnable();       //TODO
            //print("스위치 활성화");
            off.SetActive(false);
            on.SetActive(true);
        }
    }

    private void DisableInteraction()
    {
        interactionableStat.enabled = false;
        interactionable = false;
    }

    private int GetRootFloor()
    {
        FloorPrefab root = transform.root.gameObject.GetComponent<FloorPrefab>();

        int floor = root.Floor;
        return floor;
    }

}
