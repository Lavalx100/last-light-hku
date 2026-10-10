using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsButtons : MonoBehaviour
{
    public void RestartScene()
    {
        Time.timeScale = 1f;
        
        // Reset pause state
        if (PauseMenuController.IsPaused)
            PauseMenuController.ResetPauseState();
        
        // Reset all relics (clears PlayerPrefs for relics)
        if (RelicManager.I != null)
        {
            RelicManager.I.ResetAll();
        }
        
        // Reload the scene (this will reset tasks and respawn the player)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitPlayMode()
    {
        Time.timeScale = 1f;
        
        // Reset pause state
        if (PauseMenuController.IsPaused)
            PauseMenuController.ResetPauseState();
        
        // Reset all relics (clears PlayerPrefs for relics)
        if (RelicManager.I != null)
        {
            RelicManager.I.ResetAll();
        }
        
        SceneManager.LoadScene("MainMenu");
    }
}
