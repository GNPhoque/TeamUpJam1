using System.Collections;
using UnityEngine;

public class FusedPlayerInputManager : MonoBehaviour
{
	[SerializeField] private PlayerPlatypusController platypus;
	[SerializeField] private ChargingBubble chargingBubble;
	[SerializeField] private float moveAngleMaxDifference;
	[SerializeField] private float mergeMaxRadius;
	[SerializeField] private GameObject platypusVisuals;
	[SerializeField] private GameObject beaverVisuals;
	[SerializeField] private GameObject duckVisuals;

	public bool isFused;
	public bool isFusing;

	private bool isTriggered;
	[SerializeField] private bool didDuckInputMove;
	[SerializeField] private bool didBeaverInputMove;
	private Vector2 moveDirection = Vector2.zero;

	private void Start()
	{
		chargingBubble.OnBubbleCancelled += OnBubbleCancelled;
	}

	public void SetDuckVisuals(GameObject duck)
	{
		duckVisuals = duck;
	}

	public void SetBeaverVisuals(GameObject beaver)
	{
		beaverVisuals = beaver;
	}

	private void OnBubbleCancelled()
	{
		isFusing = false;
		didDuckInputMove = false;
		didBeaverInputMove = false;
	}

	public void OnPlatypusDeath()
	{
		StopPlatypus();

		//TODO  : despawn, anim, damage, respawn
		isFused = !isFused;
		platypusVisuals.SetActive(false); //fade out
		GameManager.instance.RespawnPlayers();
	}

	#region Input Implementation
	public void OnMoveChanged(Vector2 value, PlayerController pc)
	{
		if (pc.attacker == EPlayerType.Duck && didDuckInputMove)
		{
			return;
		}
		if (pc.attacker == EPlayerType.Beaver && didBeaverInputMove)
		{
			return;
		}
		if (value.magnitude < .5f)
		{
			return;
		}

		if (moveDirection == Vector2.zero)
		{
			moveDirection = value.normalized;
		}
		else
		{
			float angle = Vector2.Angle(value, moveDirection);
			print(angle);
			if (angle > moveAngleMaxDifference)
			{
				OnBubbleCancelled();
				chargingBubble.Hide();
				moveDirection = Vector2.zero;
				return;
			}
		}

		StopPlatypus();

		isTriggered = false;
		if (pc.attacker == EPlayerType.Duck)
		{
			didDuckInputMove = true;
		}
		else if (pc.attacker == EPlayerType.Beaver)
		{
			didBeaverInputMove = true;
		}

		chargingBubble.SetIcon(EChargingActionType.Move, pc.attacker);
		chargingBubble.OnBubbleFilled += TriggerMove;
	}

	private void StopPlatypus()
	{
		if (platypus.IsWalking)
		{
			chargingBubble.Hide();
			platypus.StopMovement();
			didDuckInputMove = false;
			didBeaverInputMove = false;
		}
	}

	private void TriggerMove()
	{
		if (!isTriggered)
		{
			StopPlatypus();

			platypus.OnMoveChanged(moveDirection);
			moveDirection = Vector2.zero;
			isTriggered = true;
			didDuckInputMove = false;
			didBeaverInputMove = false;
		}
	}

	public void OnInteractChanged(bool value, PlayerController pc)
	{
		StopPlatypus();

		platypus.OnInteractionChanged(value);
	}

	public void OnSplitChanged(bool value, PlayerController pc)
	{
		pc.mergeRadius.SetActive(value);

		if (value)
		{
			StopPlatypus();


			if (Vector2.Distance(duckVisuals.transform.position, beaverVisuals.transform.position) > mergeMaxRadius)
			{
				return;
			}

			if (isFused)
			{
				platypus.TriggerSplitAnim();
			}
			else
			{
				pc.TriggerSplitAnim();
			}

			StopPlatypus();

			isTriggered = false;
			isFusing = true;
			chargingBubble.SetIcon(EChargingActionType.Split, pc.attacker);
			chargingBubble.OnBubbleFilled += TriggerSplit;
		}
	}

	private void TriggerSplit()
	{
		if (!isTriggered)
		{
			StopPlatypus();

			platypus.OnSplitChanged();
			isFused = !isFused;
			isTriggered = true;
			isFusing = false;

			beaverVisuals.SetActive(!isFused);
			duckVisuals.SetActive(!isFused);
			platypusVisuals.SetActive(isFused);

			if (isFused)
			{
				platypus.transform.position = (duckVisuals.transform.position + beaverVisuals.transform.position) / 2f;
				platypus.TriggerMergeAnim();
			}
			else
			{
				duckVisuals.transform.parent.position = platypus.transform.position;
				beaverVisuals.transform.parent.position = platypus.transform.position;
				GameManager.instance.GetBeaver().TriggerMergeAnim();
				GameManager.instance.GetDuck().TriggerMergeAnim();
			}
		}
	}

	public void OnAction1Changed(bool value, PlayerController pc)
	{
		if (value)
		{
			StopPlatypus();

			isTriggered = false;
			chargingBubble.SetIcon(EChargingActionType.Action1, pc.attacker);
			chargingBubble.OnBubbleFilled += TriggerAction1;
		}
	}

	private void TriggerAction1()
	{
		if (!isTriggered)
		{
			StopPlatypus();

			platypus.OnAction1Changed();
			isTriggered = true;
		}
	}

	public void OnAction2Changed(bool value, PlayerController pc)
	{
		if (value)
		{
			StopPlatypus();

			isTriggered = false;
			chargingBubble.SetIcon(EChargingActionType.Action2, pc.attacker);
			chargingBubble.OnBubbleFilled += TriggerAction2;
		}
	}

	private void TriggerAction2()
	{
		if (!isTriggered)
		{
			StopPlatypus();

			platypus.OnAction2Changed();
			isTriggered = true;
		}
	}
	#endregion
}
