using UnityEngine;

public abstract class PlayerSkill : ScriptableObject
{
	protected PlayerController pc;

	public abstract void Init(PlayerController _pc);
	public abstract void Trigger();
}
