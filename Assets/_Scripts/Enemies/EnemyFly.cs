using System.Collections;
using UnityEngine;

public class EnemyFly : EnemyAI
{
	public int hp;
	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		animator = transform.GetChild(0).GetComponent<Animator>();
	}


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
		if (!canMove)
		{
			return;
		}

		hp--;

		if (hp <= 0)
		{
			Destroy(gameObject);
		}
		else
		{
			StartCoroutine(Invincibility());
		}
	}

	private IEnumerator Invincibility()
	{
		canMove = false;
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 1f);
		yield return new WaitForSeconds(.2f);
		spriteRenderer.material.SetFloat("_FlashAmount", 0f);
		yield return new WaitForSeconds(.2f);

		canMove = true;
	}
}
