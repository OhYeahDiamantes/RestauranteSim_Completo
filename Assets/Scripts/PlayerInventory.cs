using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private string heldItem = null;
    private bool isSnack = false; //Diferenciar plato de snack

    public WaitingCustomer currentFollower = null;

    //Funciones de estado
    public bool HasDish() { return heldItem != null && !isSnack; }
    public bool HasSnack() { return heldItem != null && isSnack; }
    public bool HasItem() { return heldItem != null; }

    //Recoger objeto
    public void PickUpDish(string dishName)
    {
        if (!HasItem())
        {
            heldItem = dishName;
            isSnack = false;
            Debug.Log($"Inventario: Recogido plato {heldItem}");
        }
        else
        {
            Debug.LogWarning("Inventario: Ya llevas algo.");
        }
    }

    public void PickUpSnack(string snackName)
    {
        if (!HasItem())
        {
            heldItem = snackName;
            isSnack = true;
            Debug.Log($"Inventario: Recogido snack {heldItem}");
        }
    }

    //Gestión de seguidores
    public void SetFollower(WaitingCustomer customer)
    {
        currentFollower = customer;
    }

    public void ReleaseFollower()
    {
        currentFollower = null;
    }

    //Funciones de recuperación y limpieza

    public string GetItemName()
    {
        return heldItem;
    }

    public void RemoveItem()
    {
        heldItem = null;
        isSnack = false;
        Debug.Log("Inventario: Objeto entregado/removido.");
    }

    public string GetDishName()
    {
        return GetItemName();
    }

    public void RemoveDish()
    {
        RemoveItem();
    }
}