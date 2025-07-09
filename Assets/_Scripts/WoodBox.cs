using UnityEngine;

public class WoodBox : MonoBehaviour, IBreakable
{
	public void Attack(EAttacker attacker)
	{
		if (attacker != EAttacker.Beaver)
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
