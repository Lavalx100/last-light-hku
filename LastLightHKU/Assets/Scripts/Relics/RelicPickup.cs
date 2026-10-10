using UnityEngine;

public class RelicPickup : MonoBehaviour
{
    public RelicId relicId;

    [Header("Pickup settings")]
    public bool destroyOnCollect = true;

    [Header("UI")]
    public GameObject promptText; // Drag your "Click 'E' to Collect" text here
    
    private bool playerInRange = false;

    void Reset()
    {
        // Helps avoid missed triggers
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        // If already collected, hide this pickup immediately
        if (RelicManager.I != null && RelicManager.I.IsCollected(relicId))
        {
            gameObject.SetActive(false);
        }
        
        // Make sure prompt is hidden at start
        if (promptText != null)
            promptText.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            CollectRelic();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerInRange = true;
        if (promptText != null)
            promptText.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        playerInRange = false;
        if (promptText != null)
            promptText.SetActive(false);
    }

    void CollectRelic()
    {
        if (RelicManager.I == null) return;

        bool didCollect = RelicManager.I.TryCollect(relicId);
        if (!didCollect) return;

        // Hide prompt
        if (promptText != null)
            promptText.SetActive(false);

        if (destroyOnCollect)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
