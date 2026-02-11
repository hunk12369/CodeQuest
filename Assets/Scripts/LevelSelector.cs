using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Configuración de Niveles")]
    public Button[] levelButtons; // Arrastra tus botones de nivel aquí
    public string mainMenuSceneName = "MainMenu"; // Para el botón de "Volver"

    void Start()
    {
        // Miramos cuál es el nivel máximo alcanzado (por defecto el 1)
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            // El índice del botón + 1 representa el nivel (Nivel 1, 2, 3...)
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    // Método para el botón de "Volver al Menú"
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
	Debug.Log("Panel de Configuración: " );

    }

    // Método que llamarán los botones de nivel
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
