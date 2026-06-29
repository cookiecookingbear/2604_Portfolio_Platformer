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

    private bool interactionable = true;
    private int floor;
    private float distance;
    private float showingDistance = 1.75f;

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

    private void Start()
    {
        floor = GetRootFloor();
        if (floor % 2 != 0)
        {
            interactionableStat.rectTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    private void Update()
    {
        Update_GetDistanceToPlayer();
        Update_ShowStatUI();
        Update_EnableInteraction();
    }

    private void Update_GetDistanceToPlayer()
    {
        distance = Vector2.Distance(this.transform.position, player.transform.position);
    }

    private void Update_EnableInteraction()
    {
        if (distance > showingDistance) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            missionManager.MissionEnable(floor);      
            off.SetActive(false);
            on.SetActive(true);
            interactionable = false;
        }
    }

    private int GetRootFloor()
    {
        FloorPrefab root = transform.root.gameObject.GetComponent<FloorPrefab>();

        floor = root.Floor;
        return floor;
    }

    private void Update_ShowStatUI()
    {
        if(distance > showingDistance
            ||missionManager.IsOnMission == true)
        {
            interactionableStat.enabled = false;
            return;
        }

        interactionableStat.enabled = true;
    }

}
