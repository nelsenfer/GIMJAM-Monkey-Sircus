using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject SettingUi;
    public GameObject CreditUi;
    public void StartGameButton()
    {
        Debug.Log("Mulai Game...");
        SceneManager.LoadScene("prologue");
    }

    public void OptionsButton()
    {
        if (SettingUi.activeSelf == false)
        {
            SettingUi.SetActive(true);
        }
        else
        {
            SettingUi.SetActive(false);
        }
    }

    public void CreditButton()
    {
        if (CreditUi.activeSelf == false)
        {
            CreditUi.SetActive(true);
        }
        else
        {
            CreditUi.SetActive(false);
        }
    }

    public void QuitGameButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
