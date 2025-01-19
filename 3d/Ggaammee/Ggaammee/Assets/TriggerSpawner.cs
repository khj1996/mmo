using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerSpawner : MonoBehaviour
{
    [SerializeField] private string needContdition; // 필요 조건
    [SerializeField] private GameObject spawnObject; // 스폰할 오브젝트
    [SerializeField] private Vector3 spawnPos; // 스폰 위치
    [SerializeField] private DialogueAction beforeAction; // 연출 시작 전 실행
    [SerializeField] private DialogueAction afterAction; // 연출 끝난 뒤 실행
    [SerializeField] private PlayableDirector director; // 플레이어블 디렉터
    [SerializeField] private PlayableAsset playableAsset; // 플레이어블 에셋

    private void OnTriggerEnter(Collider other)
    {
        if (Managers.QuestManager.activeQuests.ContainsKey(needContdition))
        {
            StartCoroutine(TestCoroutine());
        }
    }

    IEnumerator TestCoroutine()
    {
        beforeAction?.Execute();

        GameObject spawnObj = Instantiate(spawnObject, spawnPos, Quaternion.identity);

        spawnObj.transform.SetParent(transform);
        spawnObj.GetComponent<MonsterController>().SetData();

        yield return new WaitForSeconds(2.5f);

        afterAction?.Execute();
    }
}