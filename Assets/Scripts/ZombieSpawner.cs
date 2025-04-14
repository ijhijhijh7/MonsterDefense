using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaneType { Lane1, Lane2, Lane3 }

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform spawnPoint;
    public LaneType laneType;
    public float minSpawnInterval = 1.0f; // 최소 스폰 간격
    public float maxSpawnInterval = 2.0f; // 최대 스폰 간격

    void Start()
    {
        float initialDelay = Random.Range(0f, 1f); // 시작 시점 차이를 위한 랜덤 딜레이
        StartCoroutine(SpawnRoutine(initialDelay));
    }

    IEnumerator SpawnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            SpawnZombie();

            // 매번 랜덤한 시간으로 스폰 간격을 설정
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    void SpawnZombie()
    {
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
        zombie.GetComponent<ZombieController>().SetLane(laneType);

        // 레이어 설정
        switch (laneType)
        {
            case LaneType.Lane1:
                zombie.layer = LayerMask.NameToLayer("Lane1");
                break;
            case LaneType.Lane2:
                zombie.layer = LayerMask.NameToLayer("Lane2");
                break;
            case LaneType.Lane3:
                zombie.layer = LayerMask.NameToLayer("Lane3");
                break;
        }
    }
}
