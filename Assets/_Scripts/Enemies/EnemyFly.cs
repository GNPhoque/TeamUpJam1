using UnityEngine;

public class EnemyFly : EnemyAI
{

	protected override void MoveTowardPlayer()
	{
		rb.MovePosition(transform.position + (playerTarget.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed);
	}

	protected override bool CanAttackPlayer()
	{
		return Vector2.Distance(playerTarget.transform.position, transform.position) <= attackRange;
	}

	protected override void AttackPlayer()
	{
		playerTarget.Die();
	}

	protected override void Die()
	{
		Destroy(gameObject);
	}
}
