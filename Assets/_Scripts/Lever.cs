using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent OnInteract;

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}
