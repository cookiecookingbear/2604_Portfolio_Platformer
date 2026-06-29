using UnityEngine;
using MissionType = SO_MissionInfo.MissionType;
using MissionInfo = SO_MissionInfo.MissionInfo;
using System.Collections;

public class MissionObjectSpawner : MonoBehaviour
{
    private PlatformGenerator generator;
    private MissionManager missionManager;
    private GameObject player;

    private Vector2 playerPos;
    private float instatiateYInterval = 14.0f;
    private float coinTimeInterval = 4.0f;

    private Coroutine objectSpawn;

    [SerializeField] private GameObject coinPrefab;

    private void Awake()
    {
        generator = transform.root.GetComponent<PlatformGenerator>();
        if (generator is null)
        {
            Debug.LogError("플랫폼제너레이터 연결안됨", this);
            enabled = false;
        }

        missionManager = GameObject.FindAnyObjectByType<MissionManager>();

        player = GameObject.Find("Player");

    }

    private void Start()
    {
        missionManager.MissionStarted += MissionStart_CollectCoins;
        missionManager.MissionStarted += MissionStart_KillMonster;
    }

    private void Update()
    {
        StopSpawning();
    }

    private void MissionStart_CollectCoins(MissionInfo missionInfo, int startingFloor)
    {
        if (missionInfo.missionType != MissionType.CollectCoins) return;

        print("CollectCoins시작");

        objectSpawn = StartCoroutine(Co_CollectCoins());
    }

    private void MissionStart_KillMonster(MissionInfo missionInfo, int startingFloor)
    {
        if (missionInfo.missionType != MissionType.KillMonster) return;

        print("KillMonster시작");
    }

    private IEnumerator Co_CollectCoins()
    {   
        while(true)
        {
            if (missionManager.IsOnMission == false) break;

            GetPlayerPos();

            Instantiate(coinPrefab, new Vector2(playerPos.x, playerPos.y + instatiateYInterval), Quaternion.identity);

            yield return new WaitForSeconds(coinTimeInterval);
        }
    }

    private void StopSpawning()
    {
        if (missionManager.IsOnMission == true) return;
        if (objectSpawn is null) return;

        StopCoroutine(objectSpawn);
    }

    private void GetPlayerPos()
    {
        playerPos = player.transform.position;
    }
}
