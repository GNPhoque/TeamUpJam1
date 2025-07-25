using UnityEngine;

public class PlayerBeaverController : PlayerController
{
	[SerializeField] private float action2Cooldown;
	[SerializeField] private BeaverDamageArea damageAreaPrefab;
	[SerializeField] private PushArea pushAreaPrefab;

	private bool isAction2CooldownUp = true;
	protected override void OnAction1Changed(bool value)
	{
		print($"Triggered ACTION 1 {value} on {name}");

		if (!value)
		{
			return;
		}

		Instantiate(damageAreaPrefab, (Vector2)transform.position + lastDirection.normalized, Quaternion.identity);
	}

	protected override void OnAction2Changed(bool value)
	{
		print($"Triggered ACTION 2 {value} on {name}");

		if (!value || !isAction2CooldownUp)
		{
			return;
		}

		isAction2CooldownUp = false;
		Invoke("ResetAction2Cooldown", action2Cooldown);

		Instantiate(pushAreaPrefab, (Vector2)transform.position + lastDirection.normalized, Quaternion.identity).Init(transform.position);
	}

	private void ResetAction2Cooldown() 
	{
		isAction2CooldownUp = true;
	}
}
