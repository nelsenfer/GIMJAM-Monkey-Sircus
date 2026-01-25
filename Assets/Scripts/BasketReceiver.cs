using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem catchEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            FallingItem itemData = other.GetComponent<FallingItem>();

            if (itemData != null)
            {
                // 1. TAMBAH/KURANG SKOR KE BAR
                // Mengambil nilai funValueAmount dari item (bisa +10 atau -10)
                GameManager.instance.AddFun(itemData.funValueAmount);

                // 2. Mainkan Efek Visual (Hanya kalau item positif/bagus)
                if (catchEffect != null && itemData.funValueAmount > 0)
                {
                    catchEffect.Play();
                }

                // 3. Hapus Item
                Destroy(other.gameObject);
            }
        }
    }
}