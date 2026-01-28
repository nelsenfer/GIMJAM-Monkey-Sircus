using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject warning;
    private int peringatan = 0;
    public static int sudahTutorial = 0; 
    public static Tutorial Instance;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null) Instance = this;
        
        sudahTutorial = PlayerPrefs.GetInt("TutorialSelesai", 0);
    }

    public void OpenMennu()
    {
        if (menu != null) menu.SetActive(!menu.activeSelf);
        peringatan = 0;
        warning.SetActive(false);
    }

    public void SkipTutorial()
    {
        if (peringatan == 0)
        {
            peringatan += 1;
            warning.SetActive(true);
        } else
        {
            tutorialSelesai();
        }
    }

    public void tutorialSelesai()
    {
        simpanTutorial();
        SceneManager.LoadScene("prototype");
    }
    public void simpanTutorial()
    {
        sudahTutorial = 1;
        PlayerPrefs.SetInt("TutorialSelesai", 1);
        PlayerPrefs.Save();
    }

}
