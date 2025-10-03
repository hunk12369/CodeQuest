using UnityEngine;

/// <summary>
/// Obstacle component for objects that block robot movement
/// Attach to trees, rocks, walls, and other barriers
/// </summary>
public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private ObstacleType obstacleType = ObstacleType.Rock;
    [SerializeField] private bool isDestructible = false;
    [SerializeField] private int hitsToDestroy = 3;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.2f;
    
    private Renderer obstacleRenderer;
    private Color originalColor;
    private int currentHits = 0;

    public enum ObstacleType
    {
        Rock,
        Tree,
        Wall,
        Barrier
    }

    private void Start()
    {
        obstacleRenderer = GetComponent<Renderer>();
        if (obstacleRenderer != null)
        {
            originalColor = obstacleRenderer.material.color;
        }

        // Make sure we have a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"Obstacle '{gameObject.name}' has no collider! Adding BoxCollider.");
            gameObject.AddComponent<BoxCollider>();
        }

        // Tag this as an obstacle
        if (!gameObject.CompareTag("Obstacle"))
        {
            gameObject.tag = "Obstacle";
        }
    }

    /// <summary>
    /// Called when robot collides with this obstacle
    /// </summary>
    public void OnRobotCollision()
    {
        if (isDestructible)
        {
            currentHits++;
            Debug.Log($"{obstacleType} hit! ({currentHits}/{hitsToDestroy})");

            if (currentHits >= hitsToDestroy)
            {
                DestroyObstacle();
            }
            else
            {
                ShowHitFeedback();
            }
        }
        else
        {
            Debug.Log($"Robot collided with {obstacleType} (indestructible)");
            ShowHitFeedback();
        }
    }

    private void ShowHitFeedback()
    {
        if (obstacleRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashColor());
        }
    }

    private System.Collections.IEnumerator FlashColor()
    {
        obstacleRenderer.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        obstacleRenderer.material.color = originalColor;
    }

    private void DestroyObstacle()
    {
        Debug.Log($"{obstacleType} destroyed!");
        // Add particle effect here if desired
        Destroy(gameObject);
    }

    public ObstacleType GetObstacleType()
    {
        return obstacleType;
    }

    public bool IsDestructible()
    {
        return isDestructible;
    }
}