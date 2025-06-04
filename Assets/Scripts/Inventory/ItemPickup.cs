using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int itemId;
    public int count;
    private Inventory inventory;
    private Transform player;
    public float pickupRange = 1.5f; // Range for auto-pickup

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupRange)
        {
            PickUp();
        }
    }
    void PickUp()
    {
        // Add item to inventory
        inventory.SeachForSameItem(inventory.data.items[itemId], count);
        Destroy(gameObject); // Remove item after pickup
    }

    // Optional: Visualize pickup range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}