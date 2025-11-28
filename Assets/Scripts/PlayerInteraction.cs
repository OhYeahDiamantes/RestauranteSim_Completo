using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRadius = 1f;
    public LayerMask interactableLayer;

    private PlayerInventory inventory;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {

        if (Time.timeScale == 0f) return;

        //"Submit" (E / Enter / Botón de Acción en Mando)
        if (Input.GetButtonDown("Submit"))
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

            //Si tiene un seguidor (cliente), buscar mesa vacía
            if (inventory != null && inventory.currentFollower != null)
            {
                foreach (Collider2D hit in hitColliders)
                {
                    CustomerTable table = hit.GetComponent<CustomerTable>();
                    if (table != null && table.IsTableEmpty())
                    {
                        table.Interact(this.gameObject);
                        return;
                    }
                }
            }

            //Interacción normal (Mesas ocupadas, Vitrina, Basura, Gatos, etc.)
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.gameObject == gameObject) continue;

                //Evitar interactuar con el cliente que está siguiendo al jugador
                if (inventory != null && inventory.currentFollower != null && hit.gameObject == inventory.currentFollower.gameObject) continue;

                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(this.gameObject);
                    break; //Interactuar solo con el objeto más cercano/relevante
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}