using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using TMPro;
using SFB;
using System.IO;



/// <summary>
/// Handles all UI interactions for the robot programming game
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField commandInputField;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button runButton;
    [SerializeField] private Button importArduinoButton;
    [SerializeField] private Button compileButton;
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

    public string gameSceneName = "MainMenu"; 
    private const string executableName = "arduino-cli.exe";
    //public string argumentos = "compile --fqbn arduino:avr:uno -v Example1";
    public string argumentos1 = "sketch new Example1";
    private string BaseDirectory => Application.streamingAssetsPath;

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
        
        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnButtonClicked);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetButtonClicked);

        if (importArduinoButton != null)
            importArduinoButton.onClick.AddListener(OnImportArduinoButtonClicked);
        
        if (compileButton != null)
            compileButton.onClick.AddListener(OnCompileButtonClicked);
        
        if (helpButton != null)
            helpButton.onClick.AddListener(OnHelpButtonClicked);
    }

    private void OnReturnButtonClicked()
    {
        UnityEngine.Debug.Log("Iniciando Juego...");
        SceneManager.LoadScene(gameSceneName);
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

    private void OnImportArduinoButtonClicked()				
    {
	    string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecciona el Sketch de Arduino (.ino)", "", "ino", false);
	    if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
	    {
		    string filePath = paths[0];
		    ReadInoFile(filePath);
	    }
	    else
	    {
		    UnityEngine.Debug.Log("Importacion cancelada.");
	    }
    }

    private void OnCompileButtonClicked()				
    {
      string inputText = commandInputField?.text ?? "";
      string sketchFolder = Path.Combine(@"C:\Users\usuario\My project (1)", "Example1");
      string filePath = Path.Combine(sketchFolder,"Example1" + ".ino");
      try
        {
            // 4. Escribir todo el texto en el archivo
            File.WriteAllText(filePath, inputText);
            
            UnityEngine.Debug.Log($"Sketch guardado exitosamente en: {filePath}");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Error al guardar el archivo: {ex.Message}");
        }

      ProcessStartInfo startInfo = new ProcessStartInfo();
        
      // 1. Define el archivo a ejecutar y los argumentos
      string cliPath = Application.streamingAssetsPath + "/" + executableName;
      if (!System.IO.File.Exists(cliPath))
        {
            UnityEngine.Debug.LogError("CLI no encontrado. Asegúrate de que 'arduino-cli.exe' esté en la carpeta StreamingAssets. Ruta esperada: " + cliPath);
            return;
        }
      //UnityEngine.Debug.Log(cliPath);
      startInfo.FileName = cliPath;
      startInfo.Arguments = "compile --fqbn arduino:avr:uno -v Example1";

      // 2. Configura el comportamiento (opcional pero recomendado)
      startInfo.UseShellExecute = false; // Permite redirigir la entrada/salida
      startInfo.RedirectStandardOutput = true; // Si quieres leer la salida del programa
      startInfo.RedirectStandardError = true;
      startInfo.CreateNoWindow = true; // No crea una ventana de consola visible

      try
      {
        // 3. Inicia el proceso
        Process process = Process.Start(startInfo);

        // 4. (Opcional) Leer la salida si RedirectStandardOutput es true
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit(12000);

        //UnityEngine.Debug.Log("Salida del programa de consola: " + output);
        if (process.ExitCode != 0)
        {
          // El proceso terminó con un error
                UnityEngine.Debug.LogError($"--- FALLO DE COMPILACIÓN ---\nSalida de Error: {error}\nSalida Estándar: {output}");
        }
        else
        {
          // El proceso terminó exitosamente
          UnityEngine.Debug.Log($"--- COMPILACIÓN EXITOSA ---\nSalida: {output}");
        }
        // 5. Esperar a que el proceso termine (usar con cuidado para no colgar Unity)

        process.Close();
      }
      catch (System.Exception ex)
      {
        UnityEngine.Debug.LogError("Error al ejecutar el programa: " + ex.Message);
      }
    }

    private void ReadInoFile(string path)
    {
	    try
	    {
		    string fileContent = File.ReadAllText(path);
		    if (true)
		    //if (codeDisplay != null)
		    {
			    commandInputField.text = fileContent;
			    ShowFeedback("Archivo cargado:\n" + Path.GetFileName(path), successColor);
		    //codeDisplay.text = "Archivo cargado:\n" + Path.GetFileName(path) + "\n\n" + fileContent;
		    }
		    UnityEngine.Debug.Log("Archivo .ino cargado con exito. Longitud: " + fileContent.Length);
	    }
	    catch (FileNotFoundException)
	    {
		    UnityEngine.Debug.LogError("Error: Archivo no encontrado en la ruta: " + path);
	    }
	    catch (System.Exception e)
	    {
		    UnityEngine.Debug.LogError("Error al leer el archivo: " + e.Message);
	    }
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
        UnityEngine.Debug.Log($"UI Feedback: {message}");
    }

    private void SetButtonsEnabled(bool enabled)
    {
        if (runButton != null) runButton.interactable = enabled;
        if (returnButton != null) returnButton.interactable = enabled;
        if (resetButton != null) resetButton.interactable = enabled;
        if (importArduinoButton != null) resetButton.interactable = enabled;
        if (compileButton != null) compileButton.interactable = enabled;
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
