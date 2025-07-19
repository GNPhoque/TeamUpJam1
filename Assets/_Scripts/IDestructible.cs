using UnityEngine;

public interface IBreakable
{
	public void Attack(EPlayerType attacker);
	public void Break();
}