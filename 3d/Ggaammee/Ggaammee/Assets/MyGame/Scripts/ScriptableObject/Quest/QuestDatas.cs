using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest/QuestDatas")]
public class QuestDatas : ScriptableObject
{
    [SerializeField] public List<QuestData> questDatas;

    public QuestData GetData(string questId)
    {
        var questData = questDatas.FirstOrDefault(x => x.id == questId);

        return questData;
    }
}