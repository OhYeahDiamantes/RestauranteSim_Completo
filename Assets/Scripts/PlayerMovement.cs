using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Sprites de Aether (Manos Vacías)")]
    public Sprite idleDownSprite;
    public Sprite idleUpSprite;
    public Sprite idleRightSprite;
    public Sprite idleLeftSprite;

    [Header("Sprites de Aether (Con Bandeja)")]
    public Sprite dishDownSprite;
    public Sprite dishUpSprite;
    public Sprite dishRightSprite;
    public Sprite dishLeftSprite;

    private SpriteRenderer spriteRenderer;
    private PlayerInventory inventory;
    private Vector2 lastDirection = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventory = GetComponent<PlayerInventory>();

        if (rb == null || spriteRenderer == null || inventory == null)
        {
            Debug.LogError("PlayerMovement: Faltan componentes.");
            enabled = false;
        }
    }

    void Update()
    {
        //Si el juego está en pausa (Menús abiertos), no mover
        if (Time.timeScale == 0f) return;

        //Detecta Stick y Flechas/WASD automáticamente
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        UpdateSpriteDirection();
    }

    void FixedUpdate()
    {

        if (Time.timeScale == 0f) return;

        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void UpdateSpriteDirection()
    {
        if (spriteRenderer == null) return;

        bool isMoving = movement.x != 0 || movement.y != 0;
        bool hasDish = inventory.HasItem();

        if (isMoving)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y)) lastDirection = new Vector2(movement.x, 0);
            else lastDirection = new Vector2(0, movement.y);
        }

        Sprite targetSprite = null;

        if (hasDish)
        {
            if (lastDirection.y > 0) targetSprite = dishUpSprite;
            else if (lastDirection.y < 0) targetSprite = dishDownSprite;
            else if (lastDirection.x > 0) targetSprite = dishRightSprite;
            else if (lastDirection.x < 0) targetSprite = dishLeftSprite;
        }
        else
        {
            if (lastDirection.y > 0) targetSprite = idleUpSprite;
            else if (lastDirection.y < 0) targetSprite = idleDownSprite;
            else if (lastDirection.x > 0) targetSprite = idleRightSprite;
            else if (lastDirection.x < 0) targetSprite = idleLeftSprite;
        }

        if (targetSprite != null) spriteRenderer.sprite = targetSprite;
        else spriteRenderer.sprite = idleDownSprite;
    }
}