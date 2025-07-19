using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargingBubble : MonoBehaviour
{
	[SerializeField] private float fillSpeed;
	
	[SerializeField] private Image fillImage;
	[SerializeField] private Image iconImage;
	[SerializeField] private Sprite iconMove;
	[SerializeField] private Sprite iconInteract;
	[SerializeField] private Sprite iconSplit;
	[SerializeField] private Sprite iconAction1;
	[SerializeField] private Sprite iconAction2;

	private bool isActionByDuck;
	private bool isActionByBeaver;
	private EChargingActionType actionType;

	private Coroutine fillCoroutine;

	public Action OnBubbleFilled;
	public Action OnBubbleCancelled;

	public void SetIcon(EChargingActionType type, EPlayerType player)
	{
		if(actionType != EChargingActionType.None && actionType != type)
		{
			Hide();
			OnBubbleCancelled?.Invoke();
			return;
		}

		if(actionType == type && 
			(
				(player == EPlayerType.Duck && isActionByDuck) ||
				(player == EPlayerType.Beaver && isActionByBeaver)
			)
		)
		{
			return;
		}

		actionType = type;

		if(player == EPlayerType.Duck)
		{
			isActionByDuck = true;
		}
		else if(player == EPlayerType.Beaver)
		{
			isActionByBeaver = true;
		}

		switch (type)
		{
			case EChargingActionType.None:
				Debug.LogError("Tried setting bubble icon to NONE");
				return;
			case EChargingActionType.Move:
				iconImage.sprite = iconMove;
				break;
			case EChargingActionType.Interact:
				iconImage.sprite = iconInteract;
				break;
			case EChargingActionType.Split:
				iconImage.sprite = iconSplit;
				break;
			case EChargingActionType.Action1:
				iconImage.sprite = iconAction1;
				break;
			case EChargingActionType.Action2:
				iconImage.sprite = iconAction2;
				break;
			default:
				Debug.LogError($"Tried setting bubble with unmanaged type : {type}");
				return;
		}

		gameObject.SetActive(true);

		fillCoroutine = StartCoroutine(ProgressBubble());
	}

	public void Hide()
	{
		isActionByDuck = false;
		isActionByBeaver = false;
		OnBubbleFilled = null;

		gameObject.SetActive(false);
		fillImage.fillAmount = 0f;
		actionType = EChargingActionType.None;
		if (fillCoroutine != null)
		{
			StopCoroutine(fillCoroutine);
		}
	}

	private IEnumerator ProgressBubble()
	{
		while(actionType != EChargingActionType.None && iconImage.fillAmount <= 1f)
		{
			yield return null;
			fillImage.fillAmount += fillSpeed * Time.deltaTime * (isActionByDuck && isActionByBeaver ? 2f : 1f);
			
			if(fillImage.fillAmount >= 1f)
			{
				OnBubbleFilled?.Invoke();
				Hide();
			}
		}
	}

	public void FillImage()
	{
		fillImage.fillAmount = 1f;
		OnBubbleFilled?.Invoke();
		Hide();
	}
}
