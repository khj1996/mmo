using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Skill/SkillDataContainer", fileName = "SkillDataContainer")]
public class SkillDataContainer : ScriptableObject
{
    public List<SkillData> skillDataList;
}