using UnityEngine;
using UnityEngine.SceneManagement;

public class Testing : MonoBehaviour
{
    public static int selesai = 0;

    private void Awake()
    {
        selesai = PlayerPrefs.GetInt("GameSelesai", 0);
    }

    public void buttonSelesai()
    {
        selesai = 1;
        PlayerPrefs.SetInt("GameSelesai", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainMenu");
    }
}
