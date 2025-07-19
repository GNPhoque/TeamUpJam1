using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
	new public string name = "PLAYER";
	[SerializeField] protected FusedPlayerInputManager fusedInput;
	[SerializeField] public GameObject mergeRadius;
	[SerializeField] public EPlayerType attacker;
	[SerializeField] protected float range;

	[Header("MOVEMENT")]
	[SerializeField] private float moveSpeed;

	[Header("INPUTS")]
	public Vector2 inputMovement;
	public bool inputInteract;
	public bool inputSplit;
	public bool inputAction1;
	public bool inputAction2;
	public bool canMove;

	protected bool isForcedToMove;
	protected Vector2 lastDirection;
	protected Rigidbody2D rb;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		fusedInput = GameManager.instance.fusedPlayerInputManager;
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<CheckPoint>() != null)
		{
			GameManager.instance.UpdateCheckPoint(collision.transform.position);
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
		Vector2 tmp = input.Get<Vector2>();

		if (fusedInput.isFused || fusedInput.isFusing)
		{
			fusedInput.OnMoveChanged(tmp, this);
			return;
		}

		OnMoveChanged(tmp);
	}

	public void OnInteract(InputValue input)
	{
		bool tmp = input.Get<float>() == 1f;

		if (fusedInput.isFused || fusedInput.isFusing)
		{
			fusedInput.OnInteractChanged(tmp, this);
			return;
		}

		inputInteract = tmp;
		OnInteractChanged(tmp);
	}

	public void OnSplit(InputValue input)
	{
		inputSplit = input.Get<float>() == 1f;

		fusedInput.OnSplitChanged(inputSplit, this);
	}

	public void OnAction1(InputValue input)
	{
		bool tmp = input.Get<float>() == 1f;

		if (fusedInput.isFused || fusedInput.isFusing)
		{
			fusedInput.OnAction1Changed(tmp, this);
			return;
		}

		inputAction1 = tmp;
		OnAction1Changed(inputAction1);
	}

	public void OnAction2(InputValue input)
	{
		bool tmp = input.Get<float>() == 1f;

		if (fusedInput.isFused || fusedInput.isFusing)
		{
			fusedInput.OnAction2Changed(tmp, this);
			return;
		}

		inputAction2 = tmp;
		OnAction2Changed(inputAction2);
	}
	#endregion

	#region Input Implementation
	private void OnMoveChanged(Vector2 value)
	{
		inputMovement = value;
		print($"MOVE : {inputMovement}");
	}

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

	private void Die()
	{
		print($"{name} DIED");
		StartCoroutine(GameManager.instance.RespawnPlayer(this));
	}

	protected virtual void DieFromDeathZone()
	{
		Die();
	}

	public void ResetInputMovement()
	{
		inputMovement = Vector2.zero;
	}
}
