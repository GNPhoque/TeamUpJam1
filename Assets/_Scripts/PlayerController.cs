using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
	new public string name = "PLAYER";
	[SerializeField] protected EAttacker attacker;
	[SerializeField] protected float range;
	[SerializeField] protected float deathDuration;

	[Header("MOVEMENT")]
	[SerializeField] private float moveSpeed;

	[Header("INPUTS")]
	public Vector2 inputMovement;
	public bool inputInteract;
	public bool inputSplit;
	public bool inputAction1;
	public bool inputAction2;

	protected bool canMove;
	protected bool isForcedToMove;
	protected Vector2 lastDirection;
	protected Rigidbody2D rb;

	public Action<Vector2> OnMovementInputChanged;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		GameManager.instance.AddPlayer(this);
	}

	private void Start()
	{
		canMove = true;
	}

	private void Update()
	{
		Debug.DrawLine(transform.position + (Vector3)lastDirection, transform.position + (Vector3)lastDirection + (Vector3)lastDirection * range);
	}

	private void FixedUpdate()
	{
		Move();
	}

	protected virtual void Move()
	{
		if (!canMove)
		{
			return;
		}

		rb.MovePosition(rb.position + inputMovement * Time.deltaTime * moveSpeed);
		if (inputMovement != Vector2.zero)
		{
			lastDirection = inputMovement.normalized;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("DeathZone"))
		{
			DieFromDeathZone();
		}
	}
	#endregion

	#region INPUTS
	public void OnMove(InputValue input)
	{
		inputMovement = input.Get<Vector2>();
		OnMovementInputChanged?.Invoke(inputMovement);
		print($"MOVE : {inputMovement}");
	}

	public void OnInteract(InputValue input)
	{
		inputInteract = input.Get<float>() == 1f;
		OnInteractChanged(inputInteract);
	}

	public void OnSplit(InputValue input)
	{
		inputSplit = input.Get<float>() == 1f;
		OnSplitChanged(inputSplit);
	}

	public void OnAction1(InputValue input)
	{
		inputAction1 = input.Get<float>() == 1f;
		OnAction1Changed(inputAction1);
	}

	public void OnAction2(InputValue input)
	{
		inputAction2 = input.Get<float>() == 1f;
		OnAction2Changed(inputAction2);
	}
	#endregion

	#region Input Implementation
	private void OnInteractChanged(bool value)
	{
		print($"Triggered INTERACT {value} on {name}");
		
		//If menu / dialog => next/ok

		RaycastHit2D rh = RaycastForward();
		if (rh.collider == null)
		{
			print($"No item in interact range");
			return;
		}

		IInteractable interactable = null;
		if ((interactable = rh.collider.gameObject.GetComponent<IInteractable>()) == null)
		{
			print($"No interactable item in attack range");
			return;
		}

		interactable.Interact();
	}

	protected RaycastHit2D RaycastForward()
	{
		Vector2 raycastStart = transform.position + (Vector3)lastDirection;
		return Physics2D.Raycast(raycastStart, lastDirection, range);
	}

	protected virtual void OnSplitChanged(bool value)
	{
		print($"Triggered SPLIT {value} on {name}");
	}

	protected abstract void OnAction1Changed(bool value);

	protected abstract void OnAction2Changed(bool value);
	#endregion

	private IEnumerator Die()
	{
		print($"{name} DIED");
		inputMovement = Vector2.zero;
		//TODO  : despawn, anim, damage, respawn
		gameObject.SetActive(false); //fade out
		yield return new WaitForSeconds(deathDuration);
		GameManager.instance.RespawnPlayer(this);
		yield return new WaitForSeconds(deathDuration);
		gameObject.SetActive(true);
	}

	protected virtual void DieFromDeathZone()
	{
		GameManager.instance.StartCoroutine(Die());
	}
}
