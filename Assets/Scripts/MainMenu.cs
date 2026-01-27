using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject SettingUi;
    public GameObject CreditUi;
    public Image ui;

    [Header("Ganti Gambar Settings")]
    [SerializeField] private Sprite gambarBaru;
    private bool sudahGanti = false;

    [Header("Hover Settings")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color defaultColor = Color.black;

    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions(); // Ditambah 's'
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) // Perbaikan Length
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options); // Harus memasukkan list 'options'
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue(); // Perbaikan nama fungsi
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    private void Update()
    {
        if (global::ChangeUi.selesai == 1 && !sudahGanti)
        {
            if (ui != null && gambarBaru != null)
            {
                ui.sprite = gambarBaru;
                ui.color = Color.white;
                sudahGanti = true;
            }
        }
    }

    // FUNGSI HOVER (Tampilan di Event Trigger)
    public void OnHoverEnter(GameObject buttonObj)
    {
        ApplyColor(buttonObj, hoverColor);
    }

    public void OnHoverExit(GameObject buttonObj)
    {
        ApplyColor(buttonObj, defaultColor);
    }

    private void ApplyColor(GameObject buttonObj, Color targetColor)
    {
        TextMeshProUGUI txt = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Transform otlTransform = buttonObj.transform.Find("Outline");

        if (txt != null) txt.color = targetColor;
        if (otlTransform != null)
        {
            Image img = otlTransform.GetComponent<Image>();
            if (img != null) img.color = targetColor;
        }
    }

    // FUNGSI BUTTONS
    public void StartGameButton() => SceneManager.LoadScene("testing");
    public void OptionsButton()
    {
        if (SettingUi != null) SettingUi.SetActive(!SettingUi.activeSelf);
    }
    public void CreditButton()
    {
        if (CreditUi != null) CreditUi.SetActive(!CreditUi.activeSelf);
    }

    public void QuitGameButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Fullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }


}
