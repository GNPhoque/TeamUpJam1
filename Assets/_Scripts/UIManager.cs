using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public GameObject signPostContainer;
	public TextMeshProUGUI signPostText;

	public static UIManager instance;

	private void Awake()
	{
		if (instance)
		{
			Destroy(instance);
		}

		instance = this;
	}

}
