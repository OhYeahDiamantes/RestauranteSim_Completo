using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerTimerUI : MonoBehaviour
{

    [Header("UI de Paciencia")]
    public TextMeshProUGUI patienceText;

    [Header("UI de Diálogo de Pedido")]
    public GameObject orderDialogPrefab;
    private GameObject currentOrderDialog;

    //Configura y muestra el cuadro de diálogo con el sprite del plato
    public void SetupOrderDialog(Sprite dishSprite)
    {
        if (orderDialogPrefab == null)
        {
            Debug.LogError("CustomerTimerUI: Order Dialog Prefab no asignado.");
            return;
        }

        if (currentOrderDialog != null)
        {
            Destroy(currentOrderDialog);
        }

        currentOrderDialog = Instantiate(orderDialogPrefab, transform);

        //Buscar el componente para asignar el sprite
        Image imageComponent = currentOrderDialog.GetComponent<Image>();
        if (imageComponent == null)
        {

            imageComponent = currentOrderDialog.GetComponentInChildren<Image>();
        }

        if (imageComponent != null)
        {
            imageComponent.sprite = dishSprite;
            imageComponent.SetNativeSize();
        }
        else
        {
            Debug.LogError("CustomerTimerUI: Image component no encontrado en Order Dialog Prefab o sus hijos.");
        }
    }

    //Actualiza el texto de paciencia
    public void UpdatePatience(float currentPatience, float maxPatience)
    {
        if (patienceText != null)
        {
            int percentage = Mathf.RoundToInt((currentPatience / maxPatience) * 100f);
            patienceText.text = $"Paciencia: {percentage}%";

            //Cambiar color del texto según el nivel de paciencia
            if (percentage > 50) patienceText.color = Color.green;
            else if (percentage > 20) patienceText.color = Color.yellow;
            else patienceText.color = Color.red;
        }
    }

    public void HideUI()
    {
        if (currentOrderDialog != null)
        {
            Destroy(currentOrderDialog);
            currentOrderDialog = null;
        }
        if (patienceText != null) patienceText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        if (patienceText != null) patienceText.gameObject.SetActive(true);

    }
}