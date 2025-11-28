using UnityEngine;

public class SnackStation : MonoBehaviour, IInteractable
{
    public SnackSelectionUI selectionUI;

    public void Interact(GameObject interactor)
    {
        if (selectionUI == null)
        {
            Debug.LogError("ERROR: No has arrastrado el 'SnackPanel' a la casilla 'Selection UI' en la Vitrina.");
            return;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory != null && !inventory.HasItem())
        {
            selectionUI.ShowSelectionPanel(inventory);
        }
        else
        {
            Debug.Log("Ya llevas algo en las manos.");
        }
    }
}   