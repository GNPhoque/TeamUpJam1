using UnityEngine;

public class PlatypusSpit : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private float lifeTime;

	private float currentLifeTime;
	private Vector2 shootDirection;
	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			return;
		}

		collision.transform.parent.GetComponent<EnemyAI>()?.Die();

		Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + shootDirection.normalized * moveSpeed * Time.deltaTime);
		currentLifeTime += Time.deltaTime;

		if(currentLifeTime > lifeTime)
		{
			Destroy(gameObject);
		}
	}

	public void SetDirection(Vector2 direction)
	{
		shootDirection = direction;
	}
}
