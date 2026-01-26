using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeUi : MonoBehaviour
{
    public static int selesai = 0;

    private void Awake()
    {
        selesai = PlayerPrefs.GetInt("GameSelesai", 0);
    }

    public void gameSelesai()
    {
        selesai = 1;
        PlayerPrefs.SetInt("GameSelesai", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainMenu");
    }
}
