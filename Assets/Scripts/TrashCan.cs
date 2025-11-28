using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            if (inventory.HasItem())
            {
                string itemToTrash = inventory.GetItemName();
                inventory.RemoveItem();

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayTrashSound();
                }

                Debug.Log($"Basurero: Has tirado {itemToTrash} a la basura.");
            }
            else
            {
                Debug.Log("Basurero: No tienes nada que tirar.");
            }
        }
    }
}