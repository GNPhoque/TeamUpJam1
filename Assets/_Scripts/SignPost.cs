using UnityEngine;
using TMPro;

public class SignPost : MonoBehaviour, IInteractable
{
	[SerializeField] [TextArea] private string description;

	public void Interact()
	{
		bool showing = UIManager.instance.signPostContainer.activeSelf;

		if (showing)
		{
			UIManager.instance.signPostContainer.SetActive(false);
		}
		else
		{
			UIManager.instance.signPostText.text = description;
			UIManager.instance.signPostContainer.SetActive(true);
		}
	}
}
