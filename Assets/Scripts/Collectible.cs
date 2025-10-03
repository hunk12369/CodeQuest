using UnityEngine;

/// <summary>
/// Simple collectible behavior - makes object glow and disappear when collected
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private float bobHeight = 0.2f;
    [SerializeField] private float bobSpeed = 2f;
    
    private Vector3 startPosition;
    private Renderer objectRenderer;

    void Start()
    {
        startPosition = transform.position;
        objectRenderer = GetComponent<Renderer>();
        
        // Make collectible slightly glowing
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.yellow;
        }

        // Ensure it's set as trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Rotate the collectible
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}