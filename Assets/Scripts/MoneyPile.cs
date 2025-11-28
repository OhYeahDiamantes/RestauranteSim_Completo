using UnityEngine;

public class MoneyPile : MonoBehaviour, IInteractable
{
    private int value = 0;

    public void SetValue(int amount)
    {
        value = amount;
    }

    public void Interact(GameObject interactor)
    {
        if (value > 0)
        {
            GameManager.Instance.AddMoney(value);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMoneySound();
            }

            Debug.Log($"Dinero recogido: {value}");
            Destroy(gameObject);
        }
    }
}