using UnityEngine;

public class PushArea : MonoBehaviour
{
	[SerializeField] private float liveTime;
	
	private Vector2 from;

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
		IPushable bk = collision.gameObject.GetComponent<IPushable>();
		if (bk != null)
		{
			bk.Push(from);
			DestroySelf();
		}
	}

	public void Init(Vector2 _from)
	{
		from = _from;
	}
}