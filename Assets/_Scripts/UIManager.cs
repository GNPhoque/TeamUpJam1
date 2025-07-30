using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public GameObject signPostContainer;
	public TextMeshProUGUI signPostText;
	public TextMeshProUGUI collectibles;

	private int collected;

	public static UIManager instance;

	private void Awake()
	{
		if (instance)
		{
			Destroy(instance);
		}

		instance = this;
	}

	public void OnCollectibleFetched()
	{
		collected++;
		collectibles.text = $"Fetched : {collected}/5";
		if(collected >= 5)
		{
			//You win
		}
	}
}
