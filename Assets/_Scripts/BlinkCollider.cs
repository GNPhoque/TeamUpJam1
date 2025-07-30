using System.Collections;
using UnityEngine;

public class BlinkCollider : MonoBehaviour
{
	float targetScale = 5f;
	float duration = 1f;

	private IEnumerator Start()
	{
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<CircleCollider2D>().enabled = true;

		float currentDuration = 0f;
		transform.localScale = Vector2.zero;
		while (currentDuration < duration)
		{
			yield return null;
			currentDuration += Time.deltaTime;
			transform.localScale = Vector3.one * targetScale * (currentDuration / duration);
		}
		transform.localScale = Vector3.one * targetScale;


		currentDuration = 0f;
		while (currentDuration < duration)
		{
			yield return null;
			currentDuration += Time.deltaTime;
			transform.localScale = Vector3.one * (1 - (currentDuration / duration));
		}

		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (transform.localScale == Vector3.zero)
		{
			return;
		}

		HidingItem item = collision.GetComponent<HidingItem>();
		if (item == null)
		{
			return;
		}

		item.TriggerReveal();
	}
}
