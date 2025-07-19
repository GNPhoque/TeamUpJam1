using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
	[SerializeField] protected float moveSpeed;
	[SerializeField] protected float attackRange;
	[SerializeField] protected float detectionRange;

	protected Rigidbody2D rb;
	protected APlayer playerTarget;
	protected Animator animator;

	protected abstract void MoveTowardPlayer();
	protected abstract bool CanAttackPlayer();
	protected abstract void AttackPlayer();
	protected abstract void Die();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = transform.GetChild(0).GetComponent<Animator>();
	}

	private void Start()
	{
		
	}

	private void FixedUpdate()
	{
		DetectPlayerTarget();

		if (playerTarget == null)
		{
			return;
		}

		if(Vector2.Distance(playerTarget.transform.position, transform.position) > attackRange)
		{
			MoveTowardPlayer();
		}

		if (!CanAttackPlayer())
		{
			return;
		}

		AttackPlayer();
	}

	protected virtual void DetectPlayerTarget()
	{
		playerTarget = null;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);

		List<Collider2D> players;
		if ((players = colliders.Where(x => x.CompareTag("Player")).ToList()).Count == 0)
		{
			playerTarget = null;
			return;
		}

		players = players.OrderBy(x => Vector2.Distance(x.transform.position, transform.position)).ToList();
		playerTarget = players.First().transform.parent.GetComponent<APlayer>();
	}

	protected void TriggerAttackAnim()
	{
		animator.SetTrigger("Attack");
	}

	protected void TriggerWalkAnim()
	{
		animator.SetTrigger("Walk");
	}

	protected void TriggerDeathAnim()
	{
		animator.SetTrigger("Death");
	}
}
