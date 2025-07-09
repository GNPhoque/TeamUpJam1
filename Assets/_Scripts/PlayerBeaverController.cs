using UnityEngine;

public class PlayerBeaverController : PlayerController
{
	protected override void OnAction1Changed(bool value)
	{
		print($"Triggered ACTION 1 {value} on {name}");

		if (!value)
		{
			return;
		}

		RaycastHit2D rh = RaycastForward();

		if (rh.collider == null)
		{
			print($"No item in attack range");
			return;
		}

		IBreakable breakable = null;
		if((breakable = rh.collider.gameObject.GetComponent<IBreakable>()) == null)
		{
			print($"No breakable item in attack range");
			return;
		}

		breakable.Attack(attacker);
	}

	protected override void OnAction2Changed(bool value)
	{
		print($"Triggered ACTION 2 {value} on {name}");
	}
}
