using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDuckController : PlayerController
{
	[Header("FLY")]
	[SerializeField] GameObject flyShadow;
	[SerializeField] float flyWidening;

	[Header("DASH")]
	[SerializeField] float dashSpeed;
	[SerializeField] float dashDuration;

	private bool isFlying;
	private bool isDashing;

	#region INPUT IMPLEMENTATION
	//DASH
	protected override void OnAction1Changed(bool value)
	{
		print($"Triggered ACTION 1 {value} on {name}");

		if (!value)
		{
			return;
		}

		if (isDashing)
		{
			return;
		}

		StartCoroutine(DashCooldown());
	}

	private IEnumerator DashCooldown()
	{
		canMove = false;
		isDashing = true;

		float currentDuration = 0f;
		while (currentDuration < dashDuration)
		{
			yield return null;
			currentDuration += Time.deltaTime;
		}

		canMove = true;
		isDashing = false;
	}

	protected override void Move()
	{
		if (!isDashing)
		{
			base.Move();
		}
		else
		{
			rb.MovePosition(rb.position + lastDirection * Time.deltaTime * dashSpeed);
		}
	}

	//FLY
	protected override void OnAction2Changed(bool value)
	{
		print($"Triggered ACTION 2 {value} on {name}");

		isFlying = value;
		flyShadow.SetActive(value);
		Physics2D.IgnoreLayerCollision(8, 9, value);
		transform.localScale = Vector3.one * (value ? flyWidening : 1f);
	}
	#endregion

	protected override void DieFromDeathZone()
	{
		if (isFlying)
		{
			return;
		}

		base.DieFromDeathZone();
	}
} 
