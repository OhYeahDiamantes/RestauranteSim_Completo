using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnackSelectionUI : MonoBehaviour
{
    [Header("Configuración de UI")]
    public GameObject selectionPanel;

    [Header("Navegación")]
    public GameObject firstSnackButton;

    private PlayerInventory currentInteractor;

    void Start()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }


    public void ShowSelectionPanel(PlayerInventory inventory)
    {
        currentInteractor = inventory;
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);

            Time.timeScale = 0f;

            //Seleccionar el primer botón automáticamente para el mando
            if (firstSnackButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSnackButton);
            }
        }
    }

    public void SelectSnack(string snackName)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound();

        if (currentInteractor != null)
        {
            currentInteractor.PickUpSnack(snackName);
            Debug.Log($"Snack seleccionado: {snackName}");
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        //Reanudar el tiempo al salir
        Time.timeScale = 1f;

        currentInteractor = null;

        EventSystem.current.SetSelectedGameObject(null);
    }
}