using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectionHighlighter : MonoBehaviour
{
    [Header("Configuración de Marcos")]
    public Sprite defaultFrameSprite; //Marco para menús normales
    public Sprite foodFrameSprite;    //Marco para Platos/Snacks
    public Vector2 defaultPadding = new Vector2(10, 10);
    public Vector2 foodPadding = new Vector2(20, 20);

    [Header("Movimiento")]
    public float moveSpeed = 15f;

    private RectTransform targetRect;
    private RectTransform myRect;
    private Image myImage;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();
    }

    void Update()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        if (selectedObj != null)
        {
            myImage.enabled = true;
            targetRect = selectedObj.GetComponent<RectTransform>();

            if (targetRect != null)
            {

                if (selectedObj.CompareTag("FoodButton"))
                {
                    if (foodFrameSprite != null) myImage.sprite = foodFrameSprite;
                    Vector2 targetSize = targetRect.sizeDelta + foodPadding;
                    myRect.sizeDelta = Vector3.Lerp(myRect.sizeDelta, targetSize, moveSpeed * Time.unscaledDeltaTime);
                }
                else
                {
                    if (defaultFrameSprite != null) myImage.sprite = defaultFrameSprite;
                    Vector2 targetSize = targetRect.sizeDelta + defaultPadding;
                    myRect.sizeDelta = Vector3.Lerp(myRect.sizeDelta, targetSize, moveSpeed * Time.unscaledDeltaTime);
                }

                transform.position = Vector3.Lerp(transform.position, targetRect.position, moveSpeed * Time.unscaledDeltaTime);
            }
        }
        else
        {
            myImage.enabled = false;
        }
    }
}