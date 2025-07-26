using DG.Tweening;
using UnityEngine;

public class PushableBox : MonoBehaviour, IPushable
{
	[SerializeField] private float movementRange;
	[SerializeField] private float movementDuration;

	private bool _isBeingPushed;
	private Vector2 pushEndPosition;
	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		rb.linearVelocity = Vector2.zero;
	}

	public void Push(Vector2 from)
	{
		if (_isBeingPushed)
		{
			return;
		}

		Vector2 startPosition = rb.position;

		rb.bodyType = RigidbodyType2D.Dynamic;
		Vector2 fromDirection = (from - rb.position).normalized;

		Vector2 cardinal;
		if (Mathf.Abs(fromDirection.x) >= Mathf.Abs(fromDirection.y))
		{
			cardinal = fromDirection.x < 0f ? Vector2.left : Vector2.right;
		}
		else
		{
			cardinal = fromDirection.y < 0f ? Vector2.down : Vector2.up;
		}

		pushEndPosition = rb.position - cardinal * movementRange;
		rb.DOMove(pushEndPosition, movementDuration).OnKill(() => 
		{ 
			_isBeingPushed = false; 
			rb.bodyType = RigidbodyType2D.Kinematic;
			if (Vector2.Distance(rb.position, pushEndPosition) < .1f)
			{
				rb.position = pushEndPosition;
			}
			else
			{
				rb.position = startPosition;
			}
		});
	}
}
