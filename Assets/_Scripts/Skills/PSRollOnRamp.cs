using UnityEngine;

[CreateAssetMenu(fileName = "RollOnRamp", menuName = "PlayerSkill/RollOnRamp")]
public class PSRollOnRamp : PlayerSkill
{
	public override void Init(PlayerController _pc)
	{
		pc = _pc;
	}

	public override void Trigger()
    {
	}
}
