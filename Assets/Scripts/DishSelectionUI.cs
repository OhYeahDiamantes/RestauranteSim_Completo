using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DishSelectionUI : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject selectionPanel;
    public Transform buttonsContainer;
    public GameObject dishButtonPrefab;

    private PlayerInventory currentInteractorInventory;
    private KitchenPickup currentKitchenPickup;

    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("DishSelectionUI: GameManager no encontrado.");
            return;
        }

        selectionPanel.SetActive(false);
        PopulateDishButtons();
    }

    void Update()
    {
        //Cerrar con "Cancel" (Escape o Triángulo/Y)
        if (selectionPanel.activeSelf && Input.GetButtonDown("Cancel"))
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();
            ClosePanel();
        }
    }

    void PopulateDishButtons()
    {
        foreach (Transform child in buttonsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DishData dishData in gameManager.availableDishes)
        {
            GameObject buttonObject = Instantiate(dishButtonPrefab, buttonsContainer);
            Button button = buttonObject.GetComponent<Button>();

            Image dishImage = buttonObject.GetComponent<Image>();
            if (dishImage != null && dishData.dishSprite != null)
            {
                dishImage.sprite = dishData.dishSprite;
                dishImage.color = Color.white;
            }

            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = dishData.dishName;
            }

            button.onClick.AddListener(() => SelectDish(dishData.dishName));
        }
    }

    public void ShowSelectionPanel(PlayerInventory inventory, KitchenPickup kitchenPickup)
    {
        currentInteractorInventory = inventory;
        currentKitchenPickup = kitchenPickup;
        selectionPanel.SetActive(true);

        Time.timeScale = 0f;

        //Navegación: Seleccionar el primer plato automáticamente
        if (buttonsContainer.childCount > 0)
        {
            GameObject firstBtn = buttonsContainer.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstBtn);
        }
    }

    public void SelectDish(string dishName)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();

        if (currentKitchenPickup != null)
        {
            currentKitchenPickup.StartCooking(dishName);
            Debug.Log($"Pedido de {dishName} enviado a Xiao.");
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        currentInteractorInventory = null;
        currentKitchenPickup = null;
        selectionPanel.SetActive(false);

        //Reanudar el juego
        Time.timeScale = 1f;

        EventSystem.current.SetSelectedGameObject(null);
    }
}