using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the robot's movement and command execution
/// This script should be attached to the Robot GameObject
/// </summary>
public class RobotController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second
    
    [Header("References")]
    private Rigidbody robotRigidbody;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    
    // Command execution state
    private bool isExecutingCommands = false;
    private bool isBlocked = false;

    private List<string> currentCommands = new List<string>();

    // Coroutin execution state
    private Coroutine currentMoveCoroutine; // Declara una variable de tipo Coroutine

    // Events for UI feedback
    public System.Action OnCommandExecutionStarted;
    public System.Action OnCommandExecutionFinished;
    public System.Action OnCollectibleFound;

    void Start()
    {
        // Get the Rigidbody component
        robotRigidbody = GetComponent<Rigidbody>();
        
        // Store starting position and rotation for reset functionality
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        
        // Configure Rigidbody for stable movement
        if (robotRigidbody != null)
        {
            robotRigidbody.linearDamping = 5f; // Adds resistance to movement
            robotRigidbody.angularDamping = 5f; // Adds resistance to rotation
            robotRigidbody.freezeRotation = true; // Prevent physics from rotating the robot unexpectedly
        }
        
        Debug.Log("Robot initialized at position: " + startingPosition);
    }

    /// <summary>
    /// Main function to execute a list of commands
    /// Called by the UI when the "Run" button is pressed
    /// </summary>
    /// <param name="commands">List of command strings to execute</param>
    public void ExecuteCommands(List<string> commands)
    {
        if (isExecutingCommands)
        {
            Debug.LogWarning("Robot is already executing commands!");
            return;
        }

        if (commands == null || commands.Count == 0)
        {
            Debug.LogWarning("No commands to execute!");
            return;
        }

        currentCommands = new List<string>(commands); // Create a copy
        StartCoroutine(ExecuteCommandSequence());
    }

    /// <summary>
    /// Coroutine that executes commands one by one
    /// </summary>
    private IEnumerator ExecuteCommandSequence()
    {
        isExecutingCommands = true;
        OnCommandExecutionStarted?.Invoke();
        
        Debug.Log($"Starting execution of {currentCommands.Count} commands");

        for (int i = 0; i < currentCommands.Count; i++)
        {
            string command = currentCommands[i].Trim();
            Debug.Log($"Executing command {i + 1}/{currentCommands.Count}: {command}");
            
            yield return StartCoroutine(ExecuteSingleCommand(command));
            
            // Small pause between commands for better visual feedback
            yield return new WaitForSeconds(0.2f);
        }

        isExecutingCommands = false;
        OnCommandExecutionFinished?.Invoke();
        Debug.Log("Command execution completed!");
    }

    /// <summary>
    /// Executes a single command
    /// </summary>
    /// <param name="command">Command string to execute</param>
    private IEnumerator ExecuteSingleCommand(string command)
    {
        // Parse the command and extract parameters
        if (command.StartsWith("MOVE_FORWARD"))
        {
            float distance = ParseFloatParameter(command, "MOVE_FORWARD");
            if (currentMoveCoroutine != null)
            {
              StopCoroutine(currentMoveCoroutine); // Detén la anterior si está activa
            }
            yield return currentMoveCoroutine = StartCoroutine(MoveForward(distance));
            //yield return StartCoroutine(MoveForward(distance));
        }
        else if (command.StartsWith("ROTATE"))
        {
            float degrees = ParseFloatParameter(command, "ROTATE");
            yield return StartCoroutine(Rotate(degrees));
        }
        else
        {
            Debug.LogError($"Unknown command: {command}");
        }
    }

    /// <summary>
    /// Moves the robot forward by the specified distance
    /// </summary>
    /// <param name="distance">Distance to move in units</param>
    private IEnumerator MoveForward(float distance)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * distance;
        
        float elapsedTime = 0f;
        float duration = distance / moveSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }
        
        // Ensure we reach the exact target position
        transform.position = targetPos;
        Debug.Log($"Moved forward {distance} units to position: {transform.position}");
    }

    /// <summary>
    /// Rotates the robot by the specified degrees
    /// </summary>
    /// <param name="degrees">Degrees to rotate (positive = right, negative = left)</param>
    private IEnumerator Rotate(float degrees)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, degrees, 0);
        
        float elapsedTime = 0f;
        float duration = Mathf.Abs(degrees) / rotationSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
            yield return null;
        }
        
        // Ensure we reach the exact target rotation
        transform.rotation = targetRotation;
        Debug.Log($"Rotated {degrees} degrees. New rotation: {transform.rotation.eulerAngles.y}");
    }

    /// <summary>
    /// Parses a float parameter from a command string
    /// Example: "MOVE_FORWARD(5)" returns 5.0f
    /// </summary>
    /// <param name="command">The full command string</param>
    /// <param name="commandName">The command name (e.g., "MOVE_FORWARD")</param>
    /// <returns>The parsed float value, or 0 if parsing fails</returns>
    private float ParseFloatParameter(string command, string commandName)
    {
        try
        {
            int startIndex = command.IndexOf('(') + 1;
            int endIndex = command.IndexOf(')', startIndex);
            
            if (startIndex > 0 && endIndex > startIndex)
            {
                string paramStr = command.Substring(startIndex, endIndex - startIndex);
                return float.Parse(paramStr);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing parameter from command '{command}': {e.Message}");
        }
        
        Debug.LogWarning($"Could not parse parameter from command: {command}");
        return 0f;
    }

    /// <summary>
    /// Resets the robot to its starting position and rotation
    /// Called by the UI when the "Reset" button is pressed
    /// </summary>
    public void ResetPosition()
    {
        if (isExecutingCommands)
        {
            StopAllCoroutines();
            isExecutingCommands = false;
        }

        transform.position = startingPosition;
        transform.rotation = startingRotation;
        
        // Reset velocity if using Rigidbody
        if (robotRigidbody != null)
        {
            robotRigidbody.linearVelocity = Vector3.zero;
            robotRigidbody.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Robot reset to starting position: " + startingPosition);
    }

    /// <summary>
    /// Handles collision detection for collectibles
    /// </summary>
    /// <param name="other">The collider that was triggered</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Debug.Log("Collectible found!");
            OnCollectibleFound?.Invoke();
            
            // Destroy the collectible object
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// Public getter to check if robot is currently executing commands
    /// </summary>
    public bool IsExecutingCommands
    {
        get { return isExecutingCommands; }
    }

    /// <summary>
    /// Checks if the path is clear of obstacles
    /// </summary>
    private bool CheckPathClear(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        
        if (distance < 0.01f) return true;
        
        RaycastHit hit;
        if (Physics.Raycast(from, direction.normalized, out hit, distance + 0.5f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.OnRobotCollision();
                }
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Handles physical collisions with obstacles
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
          if (currentMoveCoroutine != null)
          {
            // 1. **DETIENE LA CORRUTINA:** Esto interrumpe inmediatamente el bucle 'while'
            StopCoroutine(currentMoveCoroutine);
            //Debug.Log("Robot collided with obstacle!");
            isBlocked = true;
            
            if (robotRigidbody != null)
            {
                Debug.Log("Robot Stop");
                robotRigidbody.linearVelocity = Vector3.zero;
                robotRigidbody.linearVelocity = Vector3.zero;
            }
            
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.OnRobotCollision();
            }
            Debug.Log("Robot Stop: Corrutina de movimiento detenida por colisión.");
            currentMoveCoroutine = null; // Limpia la referencia

            isExecutingCommands = false;
            OnCommandExecutionFinished?.Invoke();
            Debug.Log("Command execution completed!");
          }
        }
    }

}
