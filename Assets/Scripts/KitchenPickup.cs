using UnityEngine;
using System.Collections;
using TMPro;

public class KitchenPickup : MonoBehaviour, IInteractable
{
    [Header("Configuración Visual del Chef Xiao")]
    public Sprite idleSprite;
    public Sprite activeSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Configuración de Cocina")]
    public float baseCookingTime = 5f;
    private string dishBeingCooked = null;
    private bool isCooking = false;
    private bool dishIsReady = false;

    [Header("UI del Chef")]
    public Transform chefUIPositionPoint;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("KitchenPickup: SpriteRenderer no encontrado en Xiao.");
        }

        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    public void Interact(GameObject interactor)
    {
        PlayerInventory playerInventory = interactor.GetComponent<PlayerInventory>();
        if (playerInventory == null) return;

        //Recoger (Plato listo + Manos vacías)
        if (dishIsReady && !playerInventory.HasItem())
        {
            playerInventory.PickUpDish(dishBeingCooked);
            Debug.Log($"Plato recogido: {dishBeingCooked}. Mesa de cocina libre.");

            ResetKitchenState();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideChefStatus();
            }
        }
        //Esperando (Está cocinando)
        else if (isCooking)
        {
            if (UIManager.Instance != null && chefUIPositionPoint != null)
            {
                UIManager.Instance.ShowChefStatus("Cocinando...", chefUIPositionPoint.position);
            }
            Debug.Log($"Xiao: El plato {dishBeingCooked} está cocinando.");
        }
        //Avisar al jugador
        else if (dishIsReady && playerInventory.HasItem())
        {
            if (UIManager.Instance != null && chefUIPositionPoint != null)
            {
                UIManager.Instance.ShowChefStatus("¡Manos Llenas!", chefUIPositionPoint.position);
            }
            Debug.Log($"Xiao: ¡El plato está listo!");
        }
        //Pedir nuevo plato (Chef Libre)
        else
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideChefStatus();
            }

            DishSelectionUI dishSelectionUI = Object.FindFirstObjectByType<DishSelectionUI>();
            if (dishSelectionUI != null)
            {
                //Abrir el menú aunque las manos estén llenas
                dishSelectionUI.ShowSelectionPanel(playerInventory, this);
            }
        }
    }

    public void StartCooking(string dishName)
    {
        if (isCooking || dishIsReady) return;

        dishBeingCooked = dishName;
        isCooking = true;
        dishIsReady = false;

        if (UIManager.Instance != null && chefUIPositionPoint != null)
        {
            UIManager.Instance.ShowChefStatus("Cocinando...", chefUIPositionPoint.position);
        }

        StartCoroutine(CookingRoutine());
        Debug.Log($"Xiao comienza a cocinar {dishName}.");
    }

    IEnumerator CookingRoutine()
    {
        float timer = baseCookingTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isCooking = false;
        dishIsReady = true;
        Debug.Log($"¡Plato {dishBeingCooked} listo para recoger!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayChefFinishSound();
        }

        if (UIManager.Instance != null && chefUIPositionPoint != null)
        {
            UIManager.Instance.ShowChefStatus("Listo!", chefUIPositionPoint.position);
        }
    }

    void ResetKitchenState()
    {
        dishBeingCooked = null;
        isCooking = false;
        dishIsReady = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideChefStatus();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetSprite(activeSprite);
            if (dishIsReady && UIManager.Instance != null && chefUIPositionPoint != null)
            {
                UIManager.Instance.ShowChefStatus("Listo!", chefUIPositionPoint.position);
            }
            else if (isCooking && UIManager.Instance != null && chefUIPositionPoint != null)
            {
                UIManager.Instance.ShowChefStatus("Cocinando...", chefUIPositionPoint.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetSprite(idleSprite);
            if (UIManager.Instance != null)
            {
                if (!dishIsReady && !isCooking)
                {
                    UIManager.Instance.HideChefStatus();
                }
            }
        }
    }

    private void SetSprite(Sprite targetSprite)
    {
        if (spriteRenderer != null && targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }
}