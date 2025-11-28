using UnityEngine;
using System.Collections;

public class CustomerTable : MonoBehaviour, IInteractable
{
    [Header("Configuración Base")]
    public int dishPrice = 10;
    public int maxTipAmount = 15;

    public GameObject moneyPilePrefab;
    public Transform moneySpawnPoint;

    [Header("Configuración Visual del Cliente")]
    public SpriteRenderer customerVisualRenderer;
    public Sprite[] customerSprites;

    [Header("Impaciencia del Cliente")]
    public float baseWaitTime = 40f;
    public float waitTimeVariance = 15f;

    [Header("Tiempo de Comida")]
    public float baseEatTime = 10f;
    public float eatTimeVariance = 5f;

    [Header("UI del Cliente")]
    public Transform customerUIPositionPoint;

    private bool isOccupied = false;
    private string requiredDish = "";
    private Sprite requiredDishSprite;
    private bool hasOrdered = false;

    private float maxWaitTime;
    private float currentWaitTime;
    private float currentEatTime;
    private bool isEating = false;
    private bool madeMistake = false;

    private UIManager.CustomerUIElements myCustomerUI;

    void Awake()
    {
        if (customerUIPositionPoint == null) customerUIPositionPoint = transform;

        if (customerVisualRenderer != null)
        {
            customerVisualRenderer.gameObject.SetActive(false);
        }
    }

    public void OccupyTable(DishData dishData)
    {
        isOccupied = true;
        requiredDish = dishData.dishName;
        requiredDishSprite = dishData.dishSprite;
        dishPrice = dishData.basePrice;

        madeMistake = false;
        hasOrdered = false;

        if (customerVisualRenderer != null && customerSprites.Length > 0)
        {
            Sprite randomCustomer = customerSprites[Random.Range(0, customerSprites.Length)];
            customerVisualRenderer.sprite = randomCustomer;
            customerVisualRenderer.gameObject.SetActive(true);
        }

        float randomWaitOffset = Random.Range(-waitTimeVariance, waitTimeVariance);
        maxWaitTime = baseWaitTime + randomWaitOffset;
        currentWaitTime = maxWaitTime;

        if (UIManager.Instance != null)
        {
            myCustomerUI = UIManager.Instance.GetOrCreateCustomerUI(this);
            if (myCustomerUI != null)
            {
                if (myCustomerUI.orderDialogImage != null)
                {
                    myCustomerUI.orderDialogImage.gameObject.SetActive(false);
                }
                UIManager.Instance.UpdateCustomerPatience(myCustomerUI, currentWaitTime, maxWaitTime, customerUIPositionPoint.position);
            }
        }

        StartCoroutine(CustomerWaitingRoutine());
    }

    public bool IsTableEmpty()
    {
        return !isOccupied;
    }

    IEnumerator CustomerWaitingRoutine()
    {
        while (currentWaitTime > 0 && !isEating)
        {
            currentWaitTime -= Time.deltaTime;

            if (UIManager.Instance != null && myCustomerUI != null)
            {
                UIManager.Instance.UpdateCustomerPatience(myCustomerUI, currentWaitTime, maxWaitTime, customerUIPositionPoint.position);
            }
            yield return null;
        }

        if (currentWaitTime <= 0 && isOccupied && !isEating)
        {
            GameManager.Instance.RecordCustomerAnnoyed();
            CustomerLeavesAngry();
        }
    }

    IEnumerator CustomerEatingRoutine()
    {
        float randomEatOffset = Random.Range(-eatTimeVariance, eatTimeVariance);
        currentEatTime = baseEatTime + randomEatOffset;

        if (UIManager.Instance != null && myCustomerUI != null)
        {
            if (myCustomerUI.orderDialogImage != null)
            {
                myCustomerUI.orderDialogImage.gameObject.SetActive(false);
            }
        }

        while (currentEatTime > 0)
        {
            currentEatTime -= Time.deltaTime;

            if (UIManager.Instance != null && myCustomerUI != null)
            {
                UIManager.Instance.UpdateCustomerEating(myCustomerUI, customerUIPositionPoint.position);
            }

            yield return null;
        }

        if (isOccupied)
        {
            GenerateMoneyPileWithTip();
            GameManager.Instance.CustomerServed(this);
            TableFreed();
        }

        isEating = false;
    }

    void CheckDish(string deliveredDish)
    {
        if (deliveredDish == requiredDish)
        {
            GameManager.Instance.RecordDishDelivery(true);
            isEating = true;
            StopCoroutine(CustomerWaitingRoutine());
            StartCoroutine(CustomerEatingRoutine());
        }
        else
        {
            GameManager.Instance.RecordDishDelivery(false);
            madeMistake = true;
            Debug.Log("Plato incorrecto.");

            currentWaitTime -= 5f;
            if (currentWaitTime < 0) currentWaitTime = 0;
            if (UIManager.Instance != null && myCustomerUI != null)
            {
                UIManager.Instance.UpdateCustomerPatience(myCustomerUI, currentWaitTime, maxWaitTime, customerUIPositionPoint.position);
            }
        }
    }

    void GenerateMoneyPileWithTip()
    {
        if (moneyPilePrefab != null)
        {
            float patiencePercentage = Mathf.Clamp01(currentWaitTime / maxWaitTime);
            int tip = Mathf.RoundToInt(maxTipAmount * patiencePercentage);

            if (madeMistake) tip = 0;
            else if (patiencePercentage < 0.2f) tip = 0;

            int totalValue = dishPrice + tip;

            Vector3 spawnPos = moneySpawnPoint != null ? moneySpawnPoint.position : transform.position;
            GameObject moneyObject = Instantiate(moneyPilePrefab, spawnPos, Quaternion.identity);
            moneyObject.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

            MoneyPile moneyPileScript = moneyObject.GetComponent<MoneyPile>();
            if (moneyPileScript != null)
            {
                moneyPileScript.SetValue(totalValue);
            }
        }
    }

    void CustomerLeavesAngry()
    {
        StopAllCoroutines();
        if (UIManager.Instance != null && myCustomerUI != null)
        {
            if (myCustomerUI.patienceText != null)
            {
                myCustomerUI.patienceText.text = "¡Enojado!";
                myCustomerUI.patienceText.color = Color.black;
            }
            if (myCustomerUI.orderDialogImage != null)
            {
                myCustomerUI.orderDialogImage.gameObject.SetActive(false);
            }
        }
        TableFreed();
    }

    public void Interact(GameObject interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        //Asignar mesa
        if (!isOccupied && inventory.currentFollower != null)
        {
            WaitingCustomer customer = inventory.currentFollower;
            OccupyTable(customer.assignedDishData);
            inventory.ReleaseFollower();
            Destroy(customer.gameObject);
            return;
        }

        //Atender mesa
        if (isOccupied && !isEating)
        {
            if (!hasOrdered)
            {
                hasOrdered = true;
                currentWaitTime = maxWaitTime;
                if (UIManager.Instance != null && myCustomerUI != null)
                {
                    UIManager.Instance.SetCustomerOrderImage(myCustomerUI, requiredDishSprite, customerUIPositionPoint.position);
                }
            }
            else
            {
                if (inventory.HasDish())
                {
                    CheckDish(inventory.GetDishName());
                    inventory.RemoveItem();
                }
            }
        }
    }

    void TableFreed()
    {
        if (UIManager.Instance != null && myCustomerUI != null)
        {
            UIManager.Instance.HideCustomerUI(myCustomerUI);
            myCustomerUI = null;
        }

        if (customerVisualRenderer != null)
        {
            customerVisualRenderer.gameObject.SetActive(false);
        }

        isOccupied = false;
        requiredDish = "";
        hasOrdered = false;
        isEating = false;
        GameManager.Instance.TableFreed(this);
    }
}