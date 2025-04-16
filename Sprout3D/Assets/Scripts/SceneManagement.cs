using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagement : MonoBehaviour
{
    [SerializeField] public string gameSceneName = "GameScene";
    [SerializeField] public string creditSceneName = "CreditScene";
    [SerializeField] public string menuSceneName = "MenuScene";

    public void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
    public void LoadCreditsScene()
    {
        if (!string.IsNullOrEmpty(creditSceneName))
        {
            SceneManager.LoadScene(creditSceneName);
        }
    }

    public void LoadMenuScene()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}