using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;

    [Header("In-Game UI")]
    public Image[] manaBars;
    public Image[] seedIcons;

    public SceneManagement sceneManager;

    public GameObject keyboardControlsUI;

    private int maxMana = 5;
    public int currentMana { get; private set; } //read only from other scripts
    private int maxSeeds = 3;
    public int currentSeeds { get; private set; } //read only from other scripts
    private float manaRegenTime = 2f;

    void Start()
    {
        currentMana = maxMana;
        currentSeeds = maxSeeds;
        UpdateUI();
    }

    public void PlayGame()
    {
        if (sceneManager != null)
        {
            sceneManager.LoadGameScene();
        }
    }

    public void PlayCredits()
    {
        if (sceneManager != null)
        {
            sceneManager.LoadCreditsScene();
        }
    }

    public void PlayMenu()
    {
        if (sceneManager != null)
        {
            sceneManager.LoadMenuScene();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UseMana()
    {
        if (currentMana > 0)
        {
            currentMana--;
            UpdateUI();
            //Invoke(nameof(RegenMana), manaRegenTime);
        }
    }

    public void RegenMana()
    {
        if (currentMana < maxMana)
        {
            currentMana++;
            UpdateUI();
            Debug.Log("mana Regen");
        }
    }

    public bool CanPlantSeed()
    {
        foreach (var icon in seedIcons)
        {
            if (!icon.rectTransform.GetComponent<SeedIcon>().hasBeenPlanted)
            {
                return true;
            }
        }

        return false;
    }

    public void AssignSeedIcon(GrowableSproutBaseClass seed)
    {
        foreach (var icon in seedIcons)
        {
            if(!icon.rectTransform.GetComponent<SeedIcon>().hasBeenPlanted)
            {
                icon.rectTransform.GetComponent<SeedIcon>().AssignSeedToIcon(seed);
                return;
            }
        }

        //if (currentSeeds > 0)
        //{
        //    currentSeeds--;
        //    UpdateUI();
        //}
    }

    public void AddSeed()
    {
        //if (currentSeeds < maxSeeds)
        //{
        //    currentSeeds++;
        //    UpdateUI();
        //}
    }

    public Vector3 GetUILocation(int index)
    {
        //mana bar rect transform
        RectTransform manaBar = manaBars[index].rectTransform;

        //get mana bar world location in the canvas
        Vector3 canvasWorld = manaBar.TransformPoint(manaBar.rect.center);
        
        //UI depth
        canvasWorld.z = 2f;

        //convert canvas world to camera screen world
        Vector3 cameraWorld = Camera.main.ScreenToWorldPoint(canvasWorld); 
        return cameraWorld;
    }

    void UpdateUI()
    {
        // Update Mana Bar UI
        for (int i = 0; i < manaBars.Length; i++)
        {
            manaBars[i].enabled = i < currentMana;
        }

        // Update Seed Icons UI
        for (int i = 0; i < seedIcons.Length; i++)
        {
            seedIcons[i].enabled = i < currentSeeds;
        }
    }

    public void DisplayControls()
    {
        keyboardControlsUI.SetActive(true);
        Debug.Log("Display");
    }

    public void StopDisplayingControls()
    {
        keyboardControlsUI?.SetActive(false);
        Debug.Log("Stop Displying");
    }
}