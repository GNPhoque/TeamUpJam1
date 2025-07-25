using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerPlatypusController : APlayer
{
	[SerializeField] private FusedPlayerInputManager fusedInput;
	[SerializeField] new private CinemachineCamera camera;
	[SerializeField] private CinemachineTargetGroup playersCameraTargetGroup;
	[SerializeField] private CinemachineTargetGroup platypusCameraTargetGroup;
	[SerializeField] private PlatypusSpit spitPrefab;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float moveTime;
	
	private bool isFacingRight;
	private Rigidbody2D rb;
	private SpriteRenderer sprite;
	private Animator animator;
	private Vector2 inputMovement;
	private float currentMovementTimer;

	public bool IsWalking { get => currentMovementTimer < moveTime; }

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
		animator = transform.GetChild(0).GetComponent<Animator>();
		currentMovementTimer = moveTime;
		isFacingRight = true;
		sprite.flipX = isFacingRight;
	}

	private void FixedUpdate()
	{
		Move();
	}

	protected virtual void Move()
	{
		if (currentMovementTimer >= moveTime)
		{
			return;
		}

		currentMovementTimer += Time.deltaTime;
		animator.SetBool("Walk", currentMovementTimer < moveTime);

		rb.MovePosition(rb.position + inputMovement * Time.deltaTime * moveSpeed);

		if (inputMovement.x > 0.01f)
		{
			isFacingRight = true;
		}
		else if (inputMovement.x < -0.01f)
		{
			isFacingRight = false;
		}
		sprite.flipX = isFacingRight;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<CheckPoint>() != null)
		{
			GameManager.instance.UpdateCheckPoint(collision.transform.position);
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("DeathZone"))
		{
			Die();
		}
	}

	public void StopMovement()
	{
		currentMovementTimer = moveTime;
	}

	public void OnMoveChanged(Vector2 value)
	{
		print($"Triggered MOVE {value} on {name}");
		inputMovement = value;
		currentMovementTimer = 0f;
	}

	public void OnInteractionChanged(bool value)
	{
		print($"Triggered INTERACTION {value} on {name}");
	}

	public void OnSplitChanged()
	{
		if (fusedInput.isFused)
		{
			print($"Triggered SPLIT on {name}");
			camera.Follow = playersCameraTargetGroup.transform;
		}
		else
		{
			print($"Triggered MERGE on {name}");
			camera.Follow = platypusCameraTargetGroup.transform;
		}
	}

	public void OnAction1Changed()
	{
		print($"Triggered ACTION 1 on {name}");

		if(inputMovement == Vector2.zero)
		{
			return;
		}

		//Shoot
		TriggerAction1Anim();

		Instantiate(spitPrefab, transform.position, Quaternion.identity).SetDirection(inputMovement);
	}

	public void OnAction2Changed()
	{
		print($"Triggered ACTION 2 on {name}");
		TriggerAction2Anim();
	}

	#region ANIMATION
	public void TriggerSplitAnim()
	{
		animator.SetTrigger("Split");
	}
	public void TriggerMergeAnim()
	{
		animator.SetTrigger("Merge");
	}

	public void TriggerAction1Anim()
	{
		animator.SetTrigger("Action1");
	}

	public void TriggerAction2Anim()
	{
		animator.SetTrigger("Action2");
	}

	public void TriggerDeathAnim()
	{
		animator.SetTrigger("Death");
	}

	public override void Die()
	{
		print($"{name} DIED");

		//Disable movement
		currentMovementTimer = moveTime;
		inputMovement = Vector2.zero;

		TriggerDeathAnim();
		fusedInput.OnPlatypusDeath();
	}
	#endregion
}
