using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 1.5f;
    [SerializeField] private int maxMonsters = 5;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private GameObject monsterType;

    private int currentMonsterCount = 0;
    private PoolManager poolManager;
    private bool isWaitingForSpawn = false;

    private void Start()
    {
        poolManager = Managers.PoolManager;
        poolManager.PrewarmPools<MonsterController>(monsterType.name, transform, maxMonsters);
        poolManager.PrewarmPools<CircleRange>("CircleRange");
    }

    private void Update()
    {
        if (currentMonsterCount < maxMonsters && !isWaitingForSpawn)
        {
            StartCoroutine(SpawnMonsterWithDelay());
        }
    }

    private IEnumerator SpawnMonsterWithDelay()
    {
        isWaitingForSpawn = true;
        try
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster();
        }
        finally
        {
            isWaitingForSpawn = false;
        }
    }

    private void SpawnMonster()
    {
        var monster = poolManager.GetFromPool<MonsterController>(monsterType.name);
        monster.SetData();
        if (monster)
        {
            monster.SetPos(GetRandomSpawnPosition());


            monster.OnReturnToPoolAction -= HandleMonsterDefeated;
            monster.OnReturnToPoolAction += HandleMonsterDefeated;

            currentMonsterCount++;
        }
    }


    private Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );
    }

    private void HandleMonsterDefeated()
    {
        if (currentMonsterCount > 0)
            currentMonsterCount--;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Gizmos.DrawSphere(transform.position, spawnRadius * 2);
    }
#endif
}