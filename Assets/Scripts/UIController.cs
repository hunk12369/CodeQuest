using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all UI interactions for the robot programming game
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField commandInputField;
    [SerializeField] private Button runButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private GameObject helpPanel;    [SerializeField] private SyntaxHighlighter syntaxHighlighter;


    [Header("Robot Reference")]
    [SerializeField] private RobotController robotController;

    [Header("UI Settings")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
        
        if (robotController == null)
            robotController = FindObjectOfType<RobotController>();

        if (robotController != null)
        {
            robotController.OnCommandExecutionStarted += OnCommandExecutionStarted;
            robotController.OnCommandExecutionFinished += OnCommandExecutionFinished;
            robotController.OnCollectibleFound += OnCollectibleFound;
        }
    }

    private void InitializeUI()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "Enter commands and press Run!";
            feedbackText.color = normalColor;
        }

        if (helpPanel != null)
            helpPanel.SetActive(false);

        if (helpText != null)

        if (syntaxHighlighter == null)
            syntaxHighlighter = commandInputField?.GetComponent<SyntaxHighlighter>();

            helpText.text = CommandParser.GetCommandHelp();
    }

    private void SetupEventListeners()
    {
        if (runButton != null)
            runButton.onClick.AddListener(OnRunButtonClicked);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);
        
        if (helpButton != null)
            helpButton.onClick.AddListener(OnHelpButtonClicked);
    }

    private void OnRunButtonClicked()
    {
        if (robotController == null || robotController.IsExecutingCommands)
            return;

        string inputText = commandInputField?.text ?? "";
        if (string.IsNullOrEmpty(inputText))
        {
            ShowFeedback("Enter commands first!", errorColor);
            return;
        }

        List<string> commands = CommandParser.ParseCommands(inputText);
        if (commands.Count == 0)
        {
            ShowFeedback("No valid commands found!", errorColor);
            return;
        }

        robotController.ExecuteCommands(commands);
    }

    private void OnResetButtonClicked()
    {
        robotController?.ResetPosition();
        ShowFeedback("Robot reset!", normalColor);
    }

    private void OnHelpButtonClicked()
    {
        if (helpPanel != null)
            helpPanel.SetActive(!helpPanel.activeSelf);
    }

    private void OnCommandExecutionStarted()
    {
        ShowFeedback("Executing commands...", normalColor);
        SetButtonsEnabled(false);
    }

    private void OnCommandExecutionFinished()
    {
        ShowFeedback("Execution complete!", successColor);
        SetButtonsEnabled(true);
    }

    private void OnCollectibleFound()
    {
        ShowFeedback("Collectible found! Level complete!", successColor);
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }
        Debug.Log($"UI Feedback: {message}");
    }

    private void SetButtonsEnabled(bool enabled)
    {
        if (runButton != null) runButton.interactable = enabled;
        if (resetButton != null) resetButton.interactable = enabled;
    }

    private void OnDestroy()
    {
        if (robotController != null)
        {
            robotController.OnCommandExecutionStarted -= OnCommandExecutionStarted;
            robotController.OnCommandExecutionFinished -= OnCommandExecutionFinished;
            robotController.OnCollectibleFound -= OnCollectibleFound;
        }
    }
}