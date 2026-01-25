using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    // Cari script StackManager di induk (Player)
    private StackManager stackManager;

    [Header("Visual Effects")]
    public ParticleSystem catchEffect;

    void Start()
    {
        // Mencari script StackManager di object Parent (Monyet)
        stackManager = GetComponentInParent<StackManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            FallingItem itemData = other.GetComponent<FallingItem>();

            if (itemData != null)
            {
                // BUAH BAGUS (+Fun)
                if (itemData.funValueAmount > 0)
                {
                    if (stackManager != null) stackManager.AddToStack(other.gameObject);

                    // Mainkan efek partikel happy
                    if (catchEffect != null) catchEffect.Play();
                }
                // BOM / SAMPAH (-Fun)
                else
                {
                    // Hancurkan Bom-nya
                    Destroy(other.gameObject);

                    // Hukum Player: Hilangkan 3 buah teratas!
                    if (stackManager != null)
                    {
                        stackManager.RemoveTopItems(3);

                        // TODO: Tambah efek ledakan disini nanti
                        Debug.Log("DUAR! Tumpukan hancur 3 biji!");
                    }
                }
            }
        }
    }
}