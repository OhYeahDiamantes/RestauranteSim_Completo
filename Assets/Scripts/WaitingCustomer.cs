using UnityEngine;

public class WaitingCustomer : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public float moveSpeed = 3f;
    public float maxWaitTime = 60f;
    private float currentWaitTime;

    private UIManager.WaitingUIElements myUI;

    private Transform playerTransform;
    private bool isFollowing = false;

    //Datos del plato
    public DishData assignedDishData;

    void Start()
    {
        currentWaitTime = maxWaitTime;

        if (UIManager.Instance != null)
        {
            myUI = UIManager.Instance.GetOrCreateWaitingUI(this);
        }
    }

    void Update()
    {
        //Lógica de Paciencia
        currentWaitTime -= Time.deltaTime;

        if (UIManager.Instance != null && myUI != null)
        {

            UIManager.Instance.UpdateWaitingPatience(myUI, currentWaitTime, maxWaitTime, transform.position);
        }

        if (currentWaitTime <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CustomerLeftQueue();
            }
            RemoveUI();
            Destroy(gameObject);
        }

        //Lógica de Movimiento
        if (isFollowing && playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > 2.5f)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        //Dar Snack
        if (inventory.HasSnack())
        {
            string snack = inventory.GetItemName();
            inventory.RemoveItem();

            currentWaitTime += 20f;
            if (currentWaitTime > maxWaitTime) currentWaitTime = maxWaitTime;

            Debug.Log($"Cliente comió {snack}. Paciencia restaurada.");
            return;
        }

        //Seguir
        if (!isFollowing && inventory.currentFollower == null)
        {
            isFollowing = true;
            playerTransform = interactor.transform;
            inventory.SetFollower(this);
            Debug.Log("Cliente: Te sigo.");
        }
    }

    void RemoveUI()
    {
        if (UIManager.Instance != null && myUI != null)
        {
            UIManager.Instance.HideWaitingUI(myUI);
            myUI = null;
        }
    }

    void OnDestroy()
    {
        RemoveUI();
    }

    public void SetDishData(DishData data)
    {
        assignedDishData = data;
    }
}