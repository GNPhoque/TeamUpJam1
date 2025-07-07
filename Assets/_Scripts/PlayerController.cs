using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("MOVEMENT")]
	[SerializeField] private float moveSpeedLimit;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float slideValue;

	[Header("JUMP")]
	[SerializeField] private float jumpForce;
	[SerializeField] private float fallSpeed;
	[SerializeField] private float maxJumpTime;

	[Header("GROUND RAYCAST")]
	[SerializeField] public LayerMask groundMask;
	[SerializeField] public float playerHeight;
	[SerializeField] public float groundRaycastSideOffset;

	[Header("SKILLS")]
	public PlayerSkill skill1;

	private Rigidbody2D rb;

	public Vector2 inputMovement;
	public bool canMove;
	public bool canJump;
	public bool canDestroyBlocks;
	private bool isInputSkill1;
	private bool isInJump;
	private bool isInputJump;
	private bool hasDroppedJump;
	private bool mustTriggerJump;
	private float currentJumpTime;

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
		canJump = true;
	}

	private void FixedUpdate()
	{
		UpdateMovement();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (canDestroyBlocks && (collision.collider?.CompareTag("Destructible") ?? false))
		{
			Destroy(collision.gameObject);
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

	public void OnJump(InputValue input)
	{
		isInputJump = input.Get<float>() == 1;
		print($"JUMP : {input.Get<float>()}");

		if (isInputJump)
		{
			if (IsGrounded())
			{
				isInJump = true;
				mustTriggerJump = true;
			}
		}

		if (!isInputJump && isInJump)
		{
			hasDroppedJump = true;
		}
	} 

	public void OnSkill1(InputValue input)
	{
		isInputSkill1 = input.Get<float>() == 1;
		print($"SKILL1 : {input.Get<float>()}");
		skill1?.Trigger();
	}
	#endregion

	private bool IsGrounded()
	{
		Debug.DrawLine(transform.position, transform.position + Vector3.down * playerHeight / 2f, Color.red);
		Debug.DrawLine(transform.position - groundRaycastSideOffset * Vector3.right, transform.position - groundRaycastSideOffset * Vector3.right + Vector3.down * playerHeight / 2f, Color.red);
		Debug.DrawLine(transform.position + groundRaycastSideOffset * Vector3.right, transform.position + groundRaycastSideOffset * Vector3.right + Vector3.down * playerHeight / 2f, Color.red);

		if (Physics2D.Raycast(transform.position, Vector2.down, playerHeight / 2f, groundMask).collider == null
		&& Physics2D.Raycast(transform.position - groundRaycastSideOffset * Vector3.right, Vector2.down, playerHeight / 2f, groundMask).collider == null
		&& Physics2D.Raycast(transform.position + groundRaycastSideOffset * Vector3.right, Vector2.down, playerHeight / 2f, groundMask).collider == null)
		{
			print("NOT ON GROUND");
			isInJump = true;
			if (!isInputJump)
			{
				hasDroppedJump = true;
			}
			return false;
		}

		//List<ContactPoint2D> collider2Ds = new List<ContactPoint2D>();
		//GetComponent<Collider2D>().GetContacts(collider2Ds);
		//foreach (var item in collider2Ds)
		//{
			
		//}



		//isInJump = rb.linearVelocityY < -0.1f && rb.linearVelocityY > 0.1f;
		isInJump = !Mathf.Approximately(rb.linearVelocityY, 0f);

		if (!isInJump)
		{
			hasDroppedJump = false;
		}

		return !isInJump;
	}

	private void UpdateMovement()
	{
		//Apply movement from inputMovement
		if (canMove)
		{
			rb.AddForce(inputMovement * moveSpeed * Vector2.right, ForceMode2D.Force);
			rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -moveSpeedLimit, moveSpeedLimit);
		}

		//Apply jump
		if (canJump)
		{
			if (mustTriggerJump)
			{
				currentJumpTime = 0f;
				mustTriggerJump = false;
				rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
			}
			else if (!IsGrounded())
			{
				currentJumpTime += Time.fixedDeltaTime;
				if (rb.linearVelocityY < 0 || hasDroppedJump || currentJumpTime > maxJumpTime)
				{
					rb.AddForce(Vector2.down * fallSpeed, ForceMode2D.Force);
				}
			}
		}

		if (inputMovement.magnitude == 0f && IsGrounded())
		{
			if (rb.linearVelocityX > slideValue)
			{
				rb.linearVelocityX = slideValue;
			}
			else if (rb.linearVelocityX < -slideValue)
			{
				rb.linearVelocityX = -slideValue;
			}
		}
	}
}
