using System;
using System.Collections;
using UnityEngine;

public class HidingItem : MonoBehaviour
{
	[SerializeField] private float revealDuration;
	[SerializeField] private SpriteRenderer image;

	public bool isRevealed;
	public Coroutine revealCoroutine;

	public void TriggerReveal()
	{
		if (revealCoroutine != null)
		{
			StopCoroutine(revealCoroutine);
		}
		revealCoroutine = StartCoroutine(Reveal());
	}

	private IEnumerator Reveal()
	{
		float currentTimer = 0f;
		isRevealed = true;
		image.enabled = true;

		while(currentTimer < revealDuration)
		{
			yield return null;
			currentTimer += Time.deltaTime;
		}

		isRevealed = false;
		image.enabled = false;
	}
}
