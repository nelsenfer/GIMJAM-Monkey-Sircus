using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Components")]
    public Transform basketHolder;

    [Header("Settings")]
    public float headYOffset = 2.5f; // Tinggi start tumpukan (di atas kepala)
    public float maxStackY = 8.0f;   // Batas langit-langit

    [Header("Spacing Settings")]
    public float normalSpacing = 0.5f; // Jarak normal antar buah

    // Variabel pressedSpacing & maxFreshStack DIHAPUS karena kita pakai sistem rata

    [Header("🎭 Physics Sway (Inersia)")]
    [Range(0f, 0.5f)] public float swayMultiplier = 0.15f;
    public float swaySmoothness = 5f;
    public float maxSwayOffset = 1.0f;

    private List<Transform> stackedItems = new List<Transform>();

    private float lastXPosition;
    private float currentSwayValue;

    void Start()
    {
        lastXPosition = transform.position.x;
    }

    public float GetCurrentHeight()
    {
        if (basketHolder != null) return basketHolder.position.y;
        return transform.position.y + headYOffset;
    }

    public void AddToStack(GameObject fruit)
    {
        // Bersihkan komponen fisik
        Destroy(fruit.GetComponent<Rigidbody2D>());
        Destroy(fruit.GetComponent<Collider2D>());
        Destroy(fruit.GetComponent<FallingItem>());

        Transform fruitTransform = fruit.transform;
        stackedItems.Add(fruitTransform);
        fruitTransform.SetParent(transform);

        // Update visual sekalian
        UpdateStackY();
    }

    public void RemoveTopItems(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (stackedItems.Count > 0)
            {
                int lastIndex = stackedItems.Count - 1;
                Transform itemToRemove = stackedItems[lastIndex];
                stackedItems.RemoveAt(lastIndex);
                Destroy(itemToRemove.gameObject);
            }
        }
        UpdateStackY();
    }

    void LateUpdate()
    {
        CalculateSwayPhysics();
        UpdateStackY();
    }

    void CalculateSwayPhysics()
    {
        float currentX = transform.position.x;
        float velocity = (currentX - lastXPosition) / Time.deltaTime;
        lastXPosition = currentX;

        float targetSway = -velocity * swayMultiplier;
        targetSway = Mathf.Clamp(targetSway, -maxSwayOffset, maxSwayOffset);

        currentSwayValue = Mathf.Lerp(currentSwayValue, targetSway, Time.deltaTime * swaySmoothness);
    }

    void UpdateStackY()
    {
        int totalCount = stackedItems.Count;
        if (totalCount == 0) return;

        // --- LOGIKA BARU: KOMPRESI RATA (UNIFORM) ---

        // 1. Hitung ruang yang tersedia dari kepala sampai langit-langit
        float totalAvailableHeight = maxStackY - headYOffset;

        // 2. Hitung kalau pakai jarak normal butuh berapa meter?
        float requiredHeight = totalCount * normalSpacing;

        // 3. Tentukan Jarak (Spacing) Final
        float currentSpacing = normalSpacing;

        // Kalau ternyata butuh ruang lebih besar dari yang tersedia...
        if (requiredHeight > totalAvailableHeight)
        {
            // ...maka kita bagi rata ruang yang ada untuk SEMUA item
            // Hasilnya: Semua item akan mengecil jaraknya barengan
            currentSpacing = totalAvailableHeight / totalCount;

            // Jaga-jaga biar gak terlalu gepeng (negatif/nol)
            currentSpacing = Mathf.Max(0.05f, currentSpacing);
        }

        // --- PENERAPAN POSISI ---
        float currentLocalY = headYOffset;

        for (int i = 0; i < totalCount; i++)
        {
            Transform item = stackedItems[i];

            // Posisi Y
            float yPos = currentLocalY;

            // Posisi X (Miring/Sway)
            float xOffset = i * currentSwayValue;

            item.localPosition = new Vector3(xOffset, yPos, 0);

            // Sorting
            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10 + i;

            // Tambahkan jarak (Spacingnya sudah sama rata untuk semua item)
            currentLocalY += currentSpacing;
        }

        // --- UPDATE KERANJANG ---
        if (basketHolder != null)
        {
            float basketX = totalCount * currentSwayValue;
            // Keranjang menempel di item paling atas
            float basketY = currentLocalY;

            // Safety: Pastikan gak visualnya bablas
            basketY = Mathf.Min(basketY, maxStackY + 0.5f);

            basketHolder.localPosition = new Vector3(basketX, basketY, 0);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 topLimit = transform.position + new Vector3(0, maxStackY, 0);
        Vector3 bottomLimit = transform.position + new Vector3(0, headYOffset, 0);
        Gizmos.DrawLine(bottomLimit, topLimit);
    }
}