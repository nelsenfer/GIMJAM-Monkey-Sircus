using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Components")]
    public Transform basketHolder;

    [Header("Settings")]
    public float headYOffset = 2.5f;    // Tinggi awal di atas kepala
    public float maxStackY = 8.0f;      // <--- BARU: Batas ketinggian tumpukan (Langit-langit)

    [Header("Spacing Settings")]
    public float normalSpacing = 0.8f;  // Jarak buah segar (pucuk)
    public float pressedSpacing = 0.3f; // Jarak buah penyok (bawah)
    public int maxFreshStack = 5;       // <--- GANTI NAMA: Jumlah buah pucuk yang tetap 'segar' (jaraknya normal)

    private List<Transform> stackedItems = new List<Transform>();

    // Fungsi Getter untuk Spawner
    public float GetCurrentHeight()
    {
        if (basketHolder != null)
        {
            return basketHolder.position.y;
        }
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

        UpdateStackVisuals();
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
        UpdateStackVisuals();
    }

    void UpdateStackVisuals()
    {
        // 1. Hitung dulu berapa buah yang masuk kategori "Segar" (Atas) dan "Penyok" (Bawah)
        int totalCount = stackedItems.Count;
        int freshCount = Mathf.Min(totalCount, maxFreshStack); // Maksimal misal 5 buah teratas
        int squashedCount = totalCount - freshCount; // Sisanya di bawah

        // 2. Hitung Tinggi yang dibutuhkan oleh buah Segar
        float heightUsedByFresh = freshCount * normalSpacing;

        // 3. Hitung Sisa Ruang untuk buah Penyok
        // Rumus: (LangitLangit - OffsetKepala) - RuangBuahSegar
        // Kita hitung dalam Global Position dulu biar gampang, lalu konversi
        float absoluteMaxY = transform.position.y + maxStackY; // Posisi Y dunia batas atas
        float absoluteStartY = transform.position.y + headYOffset; // Posisi Y dunia kepala
        float totalAvailableHeight = maxStackY - headYOffset; // Ruang total dalam lokal

        float heightAvailableForSquashed = totalAvailableHeight - heightUsedByFresh;

        // 4. Tentukan Spacing untuk buah penyok secara DINAMIS
        float currentSquashedSpacing = pressedSpacing; // Default awal

        if (squashedCount > 0)
        {
            // Jika ruang sisa lebih kecil daripada yang dibutuhkan normalnya...
            if ((squashedCount * pressedSpacing) > heightAvailableForSquashed)
            {
                // FORCE COMPRESS!
                // Bagi sisa ruang dengan jumlah buah penyok. 
                // Hasilnya bisa sangat kecil (0.01) -> Efek garis-garis
                currentSquashedSpacing = heightAvailableForSquashed / squashedCount;

                // Opsional: Batasi biar gak minus (minimal 0.01f)
                currentSquashedSpacing = Mathf.Max(0.01f, currentSquashedSpacing);
            }
        }

        // --- MULAI MENATA POSISI ---
        float currentY = headYOffset;

        for (int i = 0; i < totalCount; i++)
        {
            Transform item = stackedItems[i];

            // Set posisi
            item.localPosition = new Vector3(0, currentY, 0);

            // Sorting Order (Makin atas makin depan)
            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10 + i;

            // Tentukan jarak untuk item BERIKUTNYA
            // Apakah item ini masuk golongan bawah (Squashed) atau atas (Fresh)?
            // Kita cek indexnya.
            // Index 0 sampai (squashedCount - 1) adalah item bawah.

            if (i < squashedCount)
            {
                currentY += currentSquashedSpacing; // Pakai jarak super gepeng
            }
            else
            {
                currentY += normalSpacing; // Pakai jarak normal
            }
        }

        // --- UPDATE KERANJANG ---
        if (basketHolder != null)
        {
            // Keranjang ada di pucuk tumpukan
            // Kita clamp (kunci) biar gak pernah melebihi MaxY
            float basketY = Mathf.Min(currentY + 0.5f, maxStackY);
            basketHolder.localPosition = new Vector3(0, basketY, 0);
        }
    }

    // Visualisasi batas Max di Scene View biar enak settingnya
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Gambar garis batas atas relative terhadap player
        Vector3 topLimit = transform.position + new Vector3(0, maxStackY, 0);
        Vector3 bottomLimit = transform.position + new Vector3(0, headYOffset, 0);

        Gizmos.DrawLine(topLimit - Vector3.right * 2, topLimit + Vector3.right * 2);
        Gizmos.DrawLine(bottomLimit, topLimit); // Garis vertikal indikator tinggi
        Gizmos.DrawWireSphere(topLimit, 0.3f);
    }
}