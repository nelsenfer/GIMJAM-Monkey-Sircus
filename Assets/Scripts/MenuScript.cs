using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MenuScript : MonoBehaviour
{
    public GameObject SettingUi;
    public TMP_Dropdown resolutionDropdown;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    Resolution[] resolutions;
    public GameObject warning;
    public static bool GameIsPaused = false;
    private bool GoMainMenu = false;
    public static MenuScript Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        // Ambil resolusi layar yang tersedia
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void PauseButton()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
        OptionsButton();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        if (normalSnapshot != null)
        {
            // Tetap 1f atau samakan dengan Pause (0.5f) agar konsisten
            normalSnapshot.TransitionTo(0.5f);
            Debug.Log("Berpindah ke Snapshot Normal (Smooth)");
        }
        AudioListener.pause = false;
    }

    public void Pause()
    {
        if (pausedSnapshot != null)
        {
            // Pindahkan transisi ke baris paling atas
            pausedSnapshot.TransitionTo(0.005f); 
            Debug.Log("Berpindah ke Snapshot Paused (Smooth)");
        }
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MainMenuButton()
    {
        if (GoMainMenu)
        {
            // Pastikan waktu kembali normal sebelum pindah scene
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            GoMainMenu = true;
            if (warning != null) warning.SetActive(true);
        }
    }

    public void Peringatan()
    {
        GoMainMenu = false;
        if (warning != null) warning.SetActive(false);
    }

    public void OptionsButton()
    {
        if (SettingUi != null) SettingUi.SetActive(!SettingUi.activeSelf);
    }

    public void Fullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}