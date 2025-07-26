using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class PlayerController : APlayer
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

	private bool isFacingRight;
	protected bool isForcedToMove;
	protected Vector2 lastDirection;
	protected Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Animator animator;

	#region MONOBEHAVIOUR
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		fusedInput = GameManager.instance.fusedPlayerInputManager;
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		animator = transform.GetChild(0).GetComponent<Animator>();
		GameManager.instance.AddPlayer(this);
	}

	private void Start()
	{
		canMove = true;
		isFacingRight = true;
		sprite.flipX = !isFacingRight;
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
			animator.SetBool("Walk", false);
			return;
		}

		rb.MovePosition(rb.position + inputMovement * Time.deltaTime * moveSpeed);
		if (inputMovement != Vector2.zero)
		{
			lastDirection = inputMovement.normalized;
			if (lastDirection.x > 0.01f)
			{
				isFacingRight = true;
			}
			else if(lastDirection.x < -0.01f)
			{
				isFacingRight = false;
			}

			sprite.flipX = !isFacingRight;
			animator.SetBool("Walk", true);
		}
		else
		{
			animator.SetBool("Walk", false);
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
		animator.SetTrigger("Action1");
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
		animator.SetTrigger("Action2");
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

		if(value == false)
		{
			return;
		}
		
		//If menu / dialog => next/ok
		List<RaycastHit2D> hits = new List<RaycastHit2D>();

		RaycastForward(ref hits);
		if (hits.All(x => x.collider == null))
		{
			print($"No item in interact range");
			return;
		}

		IInteractable interactable = null;
		foreach (var item in hits)
		{
			interactable = item.collider.gameObject.GetComponent<IInteractable>();
			if(interactable != null)
			{
				break;
			}
		}
		if (interactable == null)
		{
			print($"No interactable item in attack range");
			return;
		}

		interactable.Interact();
	}

	protected void RaycastForward( ref List<RaycastHit2D> hits)
	{
		Vector2 raycastStart = transform.position + (Vector3)lastDirection;
		Physics2D.Raycast(raycastStart, lastDirection, new ContactFilter2D(), hits, distance:range);
	}

	protected virtual void OnSplitChanged(bool value)
	{
		print($"Triggered SPLIT {value} on {name}");
	}

	protected abstract void OnAction1Changed(bool value);

	protected abstract void OnAction2Changed(bool value);
	#endregion

	public override void Die()
	{
		print($"{name} DIED");
		StartCoroutine(GameManager.instance.RespawnPlayer(this));
	}

	protected virtual void DieFromDeathZone()
	{
		animator.SetTrigger("Death");
		Die();
	}

	public void ResetInputMovement()
	{
		inputMovement = Vector2.zero;
	}

	public void TriggerMergeAnim()
	{
		animator.SetTrigger("Merge");
	}

	public void TriggerSplitAnim()
	{
		animator.SetTrigger("Split");
	}
}
