using UnityEngine;

public class BeaverDamageArea : MonoBehaviour
{
	[SerializeField] private float liveTime;

	private void Start()
	{
		Invoke("DestroySelf", liveTime);
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		EnemyAI ai = collision.transform.parent.GetComponent<EnemyAI>();
		if(ai != null)
		{
			ai.Die();
			DestroySelf();
		}

		IBreakable bk = collision.gameObject.GetComponent<IBreakable>();
		if(bk != null)
		{
			bk.Attack(EPlayerType.Beaver);
			DestroySelf();
		}
	}
}
