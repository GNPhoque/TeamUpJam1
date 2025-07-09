using UnityEngine;

public class PlayerPlatypusController : PlayerController
{

	protected override void OnAction1Changed(bool value)
	{
		print($"Triggered ACTION 1 {value} on {name}");
	}

	protected override void OnAction2Changed(bool value)
	{
		print($"Triggered ACTION 2 {value} on {name}");
	}
}
