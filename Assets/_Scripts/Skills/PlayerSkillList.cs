using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerSkillList", menuName ="PlayerSkill/PlayerSkillList")]
public class PlayerSkillList : ScriptableObject
{
	public List<PlayerSkill> playerSkills;
}
