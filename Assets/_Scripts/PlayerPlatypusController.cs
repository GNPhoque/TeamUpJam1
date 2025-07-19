using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerPlatypusController : MonoBehaviour
{
	[SerializeField] private FusedPlayerInputManager fusedInput;
	[SerializeField] new private CinemachineCamera camera;
	[SerializeField] private CinemachineTargetGroup playersCameraTargetGroup;
	[SerializeField] private CinemachineTargetGroup platypusCameraTargetGroup;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float moveTime;

	private Rigidbody2D rb;
	private Vector2 inputMovement;
	private float currentMovementTimer;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		currentMovementTimer = moveTime;
	}

	private void FixedUpdate()
	{
		Move();
	}

	protected virtual void Move()
	{
		if(currentMovementTimer >= moveTime)
		{
			return;
		}

		rb.MovePosition(rb.position + inputMovement * Time.deltaTime * moveSpeed);
		currentMovementTimer += Time.deltaTime;
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
			print($"{name} DIED");

			//Disable movement
			currentMovementTimer += moveTime;
			inputMovement = Vector2.zero;

			fusedInput.OnPlatypusDeath();
		}
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
	}

	public void OnAction2Changed()
	{
		print($"Triggered ACTION 2 on {name}");
	}
}
