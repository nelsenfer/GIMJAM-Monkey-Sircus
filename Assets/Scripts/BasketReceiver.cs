using UnityEngine;

public class BasketReceiver : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem catchEffect;
    private StackManager stackManager;

    void Start() { stackManager = GetComponentInParent<StackManager>(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            FallingItem itemData = other.GetComponent<FallingItem>();

            if (itemData != null)
            {
                // A. ITEM BAGUS
                if (itemData.funValueAmount > 0)
                {
                    if (GameManager.instance != null)
                    {
                        // CUKUP PANGGIL INI SAJA
                        // GameManager akan: Hitung Skor x1 -> Tampilkan UI x1 -> Naikkan Index ke x2
                        GameManager.instance.AddFun(itemData.funValueAmount, transform.position);
                    }

                    if (stackManager != null) stackManager.AddToStack(other.gameObject);
                    if (catchEffect != null) catchEffect.Play();
                }
                // B. BOM
                else
                {
                    Destroy(other.gameObject);
                    if (GameManager.instance != null) GameManager.instance.ReduceLife();
                }
            }
        }
    }
}