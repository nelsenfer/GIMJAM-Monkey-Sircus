using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem catchEffect;

    private StackManager stackManager;

    void Start()
    {
        stackManager = GetComponentInParent<StackManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            FallingItem itemData = other.GetComponent<FallingItem>();

            if (itemData != null)
            {
                // A. ITEM BAGUS (Buah)
                if (itemData.funValueAmount > 0)
                {
                    // Update Skor
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.AddFun(itemData.funValueAmount);
                    }

                    // Tumpuk Item
                    if (stackManager != null)
                    {
                        stackManager.AddToStack(other.gameObject);
                    }

                    if (catchEffect != null) catchEffect.Play();
                }
                // B. BOM (Item Jahat)
                else
                {
                    // 1. Hancurkan Object Bom
                    Destroy(other.gameObject);

                    // 2. KURANGI NYAWA ❤️
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.ReduceLife();
                    }

                    // (Opsional) Efek ledakan visual bisa ditaruh sini nanti
                    Debug.Log("DUAR! Bom Meledak! Nyawa berkurang.");
                }
            }
        }
    }
}