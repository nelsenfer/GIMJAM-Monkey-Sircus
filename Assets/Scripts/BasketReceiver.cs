using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    private StackManager stackManager;

    void Start()
    {
        // Cari StackManager otomatis
        stackManager = FindAnyObjectByType<StackManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang masuk adalah Item Jatuh
        FallingItem item = other.GetComponent<FallingItem>();

        if (item != null)
        {
            // JANGAN PANGGIL GAMEMANAGER DI SINI! (Nanti double score)
            // Cukup oper ke StackManager, biar dia yang urus semuanya.

            if (stackManager != null)
            {
                stackManager.AddToStack(other.gameObject);
            }
        }
    }
}