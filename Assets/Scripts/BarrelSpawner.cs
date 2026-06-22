using System.Collections;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    private GameObject playerObj;
    private Vector3 playerPosition;

    private float currentPlayerX;
    private float currentPlayerY;

    private float instatiateYInterval = 14.0f;
    [SerializeField] private float instantiateTimeInterval = 3.5f;

    [SerializeField] private GameObject barrelPrefab;
    

    private Coroutine spawnCoroutine;



    private void Awake()
    {
        playerObj = GameObject.Find("Player");

        if(playerObj == null)
        {
            Debug.LogError("플레이어 오브젝트 연결안됨", this);

            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        spawnCoroutine = StartCoroutine(InstantiateBarrel());
    }

    private void Update()
    {
        Update_GetPlayerPosition();
    }

    private void OnDisable()
    {
        StopCoroutine(spawnCoroutine);
    }

    private void Update_GetPlayerPosition()
    {
        playerPosition = playerObj.transform.position;

        currentPlayerX = playerPosition.x;
        currentPlayerY = playerPosition.y;
    }

    private IEnumerator InstantiateBarrel()
    {
        while (true)
        {
            yield return new WaitForSeconds(instantiateTimeInterval);

            SpawnBarrel();
        }
    }

    private void SpawnBarrel()
    {
        Instantiate(barrelPrefab, new Vector3(currentPlayerX, currentPlayerY + instatiateYInterval, 0), Quaternion.identity);
    }
}
