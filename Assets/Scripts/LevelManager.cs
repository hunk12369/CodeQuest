using UnityEngine;
using UnityEngine.SceneManagement; // Imprescindible para cambiar de escena

public class LevelManager : MonoBehaviour
{
    public void OpenLevel(int levelIndex)
    {
        // Carga la escena por su número en el Build Settings
        SceneManager.LoadScene(levelIndex);
    }

    public void OpenLevelByName(string levelName)
    {
        // Carga la escena por su nombre exacto
        SceneManager.LoadScene(levelName);
    }
}
