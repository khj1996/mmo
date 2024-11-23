using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRange = 3f; // 몬스터 스폰 범위
    [SerializeField] private int maxMonsters = 5; // 최대 몬스터 수
    [SerializeField] private float spawnInterval = 5f; // 스폰 간격
    [SerializeField] private GameObject monsterType; // Addressable로 로드할 몬스터 타입

    private int currentMonsterCount = 0; // 현재 스폰된 몬스터 수
    private PoolManager poolManager;
    private bool isWaitingForSpawn = false; // 스폰 대기 중인지 확인

    private void Start()
    {
        poolManager = Managers.PoolManager; // PoolManager 인스턴스 가져오기
        poolManager.PrewarmPools<MonsterController>(monsterType.name); // 몬스터 풀 미리 생성
    }

    private void Update()
    {
        // 최대 몬스터 수 미만이고 스폰 대기 중이 아닐 경우
        if (currentMonsterCount < maxMonsters && !isWaitingForSpawn)
        {
            StartCoroutine(SpawnMonsterWithDelay());
        }
    }

    private IEnumerator SpawnMonsterWithDelay()
    {
        isWaitingForSpawn = true;
        yield return new WaitForSeconds(spawnInterval); // 스폰 대기 시간
        SpawnMonster();
        isWaitingForSpawn = false;
    }

    private void SpawnMonster()
    {
        // 풀에서 몬스터 가져오기
        var monster = poolManager.GetFromPool<MonsterController>(monsterType.name);
        if (monster != null)
        {
            // 랜덤 위치 설정
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0,
                Random.Range(-spawnRange, spawnRange));

            // 몬스터 초기화 및 부모 설정
            monster.transform.SetParent(transform);
            monster.transform.position = randomPosition;
            /*monster.OnMonsterDefeated -= HandleMonsterDefeated; // 중복 등록 방지
            monster.OnMonsterDefeated += HandleMonsterDefeated;*/

            currentMonsterCount++;
        }
    }

    private void HandleMonsterDefeated(MonsterController monster)
    {
        // 몬스터가 처치되었을 때 호출
        poolManager.ReturnToPool(monsterType.name, monster);
        currentMonsterCount--;
    }
}