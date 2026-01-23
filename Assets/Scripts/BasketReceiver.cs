using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    [Header("Visual Effects")]
    [Tooltip("Masukkan Particle System VFX_Catch ke sini")]
    public ParticleSystem catchEffect; // Variabel untuk menyimpan efek

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            FallingItem itemData = other.GetComponent<FallingItem>();

            if (itemData != null)
            {
                // Debug.Log("Menangkap Item! Poin: " + itemData.funValueAmount);

                // --- BARU: NYALAKAN EFEK ---
                if (catchEffect != null)
                {
                    catchEffect.Play(); // Perintah untuk menyalakan partikel
                }
                // ---------------------------

                // Hancurkan itemnya
                Destroy(other.gameObject);
            }
        }
    }
}