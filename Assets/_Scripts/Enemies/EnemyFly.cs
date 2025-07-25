using UnityEngine;

public class EnemyFly : EnemyAI
{

	protected override void MoveTowardPlayer()
	{
		rb.MovePosition(transform.position + (playerTarget.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed);
	}

	protected override bool CanAttackPlayer()
	{
		return Vector2.Distance(playerTarget.transform.position, transform.position) <= attackRangeMax && currentAttackDelay <= 0f;
	}

	protected override void AttackPlayer()
	{
		currentAttackDelay = attackDelay;

		//TODO : Spawn an object that kills on collision?
		playerTarget.Die();
	}

	public override void Die()
	{
		Destroy(gameObject);
	}
}
