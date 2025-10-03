using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

/// <summary>
/// Provides real-time syntax highlighting for the command input field
/// Highlights valid commands in green, invalid in red, and comments in gray
/// </summary>
public class SyntaxHighlighter : MonoBehaviour
{
    [Header("Input Field Reference")]
    [SerializeField] private TMP_InputField inputField;

    [Header("Syntax Colors")]
    [SerializeField] private Color validCommandColor = new Color(0.2f, 0.8f, 0.2f); // Green
    [SerializeField] private Color invalidCommandColor = new Color(0.9f, 0.2f, 0.2f); // Red
    [SerializeField] private Color commentColor = new Color(0.5f, 0.5f, 0.5f); // Gray
    [SerializeField] private Color numberColor = new Color(0.3f, 0.6f, 1f); // Blue
    [SerializeField] private Color keywordColor = new Color(1f, 0.8f, 0.2f); // Orange/Yellow

    [Header("Settings")]
    [SerializeField] private bool enableRealTimeHighlighting = true;
    [SerializeField] private float updateDelay = 0.1f; // Delay to avoid highlighting on every keystroke

    private TMP_Text textComponent;
    private string lastText = "";
    private float lastUpdateTime = 0f;

    // Regular expressions for command patterns
    private static readonly Regex moveForwardRegex = new Regex(@"MOVE_FORWARD\s*\(\s*(\d+(?:\.\d+)?)\s*\)", RegexOptions.IgnoreCase);
    private static readonly Regex rotateRegex = new Regex(@"ROTATE\s*\(\s*(-?\d+(?:\.\d+)?)\s*\)", RegexOptions.IgnoreCase);
    private static readonly Regex numberRegex = new Regex(@"\d+(?:\.\d+)?");

    private void Start()
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        if (inputField == null)
        {
            Debug.LogError("SyntaxHighlighter: No TMP_InputField found!");
            enabled = false;
            return;
        }

        // Get the text component
        textComponent = inputField.textComponent;

        // Enable rich text on the input field
        inputField.richText = true;

        // Subscribe to text change events
        inputField.onValueChanged.AddListener(OnTextChanged);

        Debug.Log("SyntaxHighlighter initialized successfully");
    }

    private void OnTextChanged(string newText)
    {
        if (!enableRealTimeHighlighting)
            return;

        // Update highlighting with a small delay to improve performance
        lastUpdateTime = Time.time;
        Invoke(nameof(UpdateHighlighting), updateDelay);
    }

    private void UpdateHighlighting()
    {
        // Only update if enough time has passed since last change
        if (Time.time - lastUpdateTime < updateDelay)
            return;

        string currentText = inputField.text;

        // Avoid unnecessary updates
        if (currentText == lastText)
            return;

        lastText = currentText;

        // Apply syntax highlighting
        string highlightedText = ApplySyntaxHighlighting(currentText);

        // Temporarily disable the listener to avoid infinite loop
        inputField.onValueChanged.RemoveListener(OnTextChanged);

        // Store cursor position
        int caretPosition = inputField.caretPosition;

        // Update the text with highlighting
        inputField.text = highlightedText;

        // Restore cursor position
        inputField.caretPosition = caretPosition;

        // Re-enable the listener
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    /// <summary>
    /// Applies syntax highlighting to the entire text
    /// </summary>
    private string ApplySyntaxHighlighting(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        string[] lines = text.Split('\n');
        string[] highlightedLines = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            highlightedLines[i] = HighlightLine(lines[i]);
        }

        return string.Join("\n", highlightedLines);
    }

    /// <summary>
    /// Highlights a single line of text
    /// </summary>
    private string HighlightLine(string line)
    {
        if (string.IsNullOrEmpty(line))
            return line;

        string trimmedLine = line.Trim();

        // Check if it's a comment
        if (trimmedLine.StartsWith("//"))
        {
            return WrapInColor(line, commentColor);
        }

        // Check if it's a valid MOVE_FORWARD command
        if (moveForwardRegex.IsMatch(trimmedLine))
        {
            return HighlightCommand(line, "MOVE_FORWARD", true);
        }

        // Check if it's a valid ROTATE command
        if (rotateRegex.IsMatch(trimmedLine))
        {
            return HighlightCommand(line, "ROTATE", true);
        }

        // Check if it looks like a command but is invalid
        if (trimmedLine.Contains("MOVE_FORWARD") || trimmedLine.Contains("ROTATE") || 
            trimmedLine.ToLower().Contains("move_forward") || trimmedLine.ToLower().Contains("rotate"))
        {
            return WrapInColor(line, invalidCommandColor);
        }

        // Unknown text - return as-is (could be typing in progress)
        return line;
    }

    /// <summary>
    /// Highlights a command with keyword and number colors
    /// </summary>
    private string HighlightCommand(string line, string commandName, bool isValid)
    {
        Color baseColor = isValid ? validCommandColor : invalidCommandColor;

        // Highlight the command keyword
        string result = Regex.Replace(
            line,
            commandName,
            WrapInColor(commandName, keywordColor),
            RegexOptions.IgnoreCase
        );

        // Highlight numbers
        result = numberRegex.Replace(result, match => WrapInColor(match.Value, numberColor));

        return result;
    }

    /// <summary>
    /// Wraps text in TextMeshPro color tags
    /// </summary>
    private string WrapInColor(string text, Color color)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hexColor}>{text}</color>";
    }

    /// <summary>
    /// Manually trigger highlighting update (can be called from other scripts)
    /// </summary>
    public void ForceUpdateHighlighting()
    {
        lastText = ""; // Force update
        UpdateHighlighting();
    }

    /// <summary>
    /// Enable or disable real-time highlighting
    /// </summary>
    public void SetHighlightingEnabled(bool enabled)
    {
        enableRealTimeHighlighting = enabled;
        
        if (!enabled)
        {
            // Clear highlighting
            inputField.onValueChanged.RemoveListener(OnTextChanged);
            inputField.text = inputField.text; // Reset to plain text
            inputField.onValueChanged.AddListener(OnTextChanged);
        }
    }

    private void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnTextChanged);
        }
    }
}