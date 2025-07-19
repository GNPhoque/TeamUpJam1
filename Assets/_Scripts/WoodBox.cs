using UnityEngine;

public class WoodBox : MonoBehaviour, IBreakable
{
	public void Attack(EPlayerType attacker)
	{
		if (attacker != EPlayerType.Beaver)
		{
			print($"{attacker} cannot break {name}");
			return;
		}

		Break();
	}

	public void Break()
	{
		print($"{name} broke!");
		Destroy(gameObject);
	}
}
