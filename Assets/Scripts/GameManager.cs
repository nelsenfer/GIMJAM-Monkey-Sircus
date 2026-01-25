using UnityEngine;
using UnityEngine.UI; // WAJIB ADA: Biar bisa akses Slider

public class GameManager : MonoBehaviour
{
    // Singleton: Biar script lain gampang akses script ini
    public static GameManager instance;

    [Header("Settings")]
    public Slider funBarSlider; // Drag Slider kamu kesini
    public float maxFun = 100f;
    public float currentFun = 50f; // Mulai dari setengah?

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Setup awal Slider
        funBarSlider.maxValue = maxFun;
        funBarSlider.value = currentFun;
    }

    public void AddFun(float amount)
    {
        currentFun += amount;

        // Batasi biar gak kurang dari 0 atau lebih dari Max
        currentFun = Mathf.Clamp(currentFun, 0, maxFun);

        // Update tampilan Slider
        funBarSlider.value = currentFun;

        // Cek Menang / Kalah
        if (currentFun >= maxFun)
        {
            Debug.Log("MENANG! PENONTON SENANG!");
            // Nanti disini panggil layar WIN
        }
        else if (currentFun <= 0)
        {
            Debug.Log("GAME OVER! PENONTON BOSAN!");
            // Nanti disini panggil layar LOSE
        }
    }
}