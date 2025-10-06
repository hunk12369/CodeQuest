using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelSelectorButton;
    [SerializeField] private Button exitButton;
 
    // Nombre de la escena a la que se debe ir al presionar "Play"
    public string gameSceneName = "SampleScene"; 

    // Nombre de la escena del menú de niveles (si tienes una)
    public string levelsSceneName = "LevelsScene"; 

    // Referencia al Panel de Configuración para activarlo/desactivarlo
    public GameObject configurationPanel; 

    private void Start()
    {
        // Al iniciar, nos aseguramos de que el Panel de Configuración esté oculto.
        if (configurationPanel != null)
        {
            configurationPanel.SetActive(false);
        }
	SetupEventListeners();
    }

    private void SetupEventListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (levelSelectorButton != null)
            levelSelectorButton.onClick.AddListener(OnLevelSelectorButtonClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
        
    }


    // --- FUNCIONES ASIGNABLES A LOS BOTONES ---

    private void OnPlayButtonClicked()
    {
        Debug.Log("Iniciando Juego...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLevelSelectorButtonClicked()
    {
        Debug.Log("Abriendo Menú de Niveles...");
        // Si no tienes una escena separada, podrías cargar la misma escena
        // y simplemente mostrar otro panel. Aquí asumimos una escena nueva.
        SceneManager.LoadScene(levelsSceneName);
    }

    public void ToggleConfigurationPanel()
    {
        if (configurationPanel != null)
        {
            // Invierte el estado: si está activo, se desactiva; si está inactivo, se activa.
            bool isCurrentlyActive = configurationPanel.activeSelf;
            configurationPanel.SetActive(!isCurrentlyActive);
            
            Debug.Log("Panel de Configuración: " + (!isCurrentlyActive ? "Abierto" : "Cerrado"));
        }
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Saliendo del Juego...");
        
        #if UNITY_EDITOR
            // Si estamos en el editor, detenemos la reproducción
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si es un build, cerramos la aplicación
            Application.Quit();
        #endif
    }

    private void SetButtonsEnabled(bool enabled)
    {
        if (playButton != null) playButton.interactable = enabled;
        if (levelSelectorButton != null) levelSelectorButton.interactable = enabled;
        if (exitButton != null) exitButton.interactable = enabled;
    }

}
