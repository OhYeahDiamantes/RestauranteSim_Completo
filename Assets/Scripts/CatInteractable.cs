using UnityEngine;

public class CatInteractable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        //Reproducir sonido
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCatSound();
        }

        Debug.Log("Gato: *Miau* (Sigue durmiendo...)");

    }
}