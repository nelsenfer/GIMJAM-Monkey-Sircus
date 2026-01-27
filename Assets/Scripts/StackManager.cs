using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Components")]
    public Transform basketHolder;

    [Header("Settings")]
    public float headYOffset = 2.5f;
    public float maxStackY = 8.0f;

    [Header("Spacing Settings")]
    public float normalSpacing = 0.5f;

    // --- TAMBAHAN BARU (Biar bisa disetting di Inspector) ---
    [Header("Settings - Splash 🌊")]
    public int splashLimit = 10;
    // --------------------------------------------------------

    [Header("🎭 Physics Sway (Inersia)")]
    // UPDATE NILAI DEFAULT SESUAI GAMBAR TERAKHIR
    [Range(0f, 0.5f)] public float swayMultiplier = 0.015f; // Ganti dari 0.15 jadi 0.015
    public float swaySmoothness = 5f;
    public float maxSwayOffset = 4.0f; // Ganti dari 1.0 jadi 4.0

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
        // 1. Matikan physics
        Destroy(fruit.GetComponent<Rigidbody2D>());
        Destroy(fruit.GetComponent<Collider2D>());
        Destroy(fruit.GetComponent<FallingItem>());

        Transform fruitTransform = fruit.transform;
        stackedItems.Add(fruitTransform);
        fruitTransform.SetParent(transform);

        // 2. Update visual
        UpdateStackY();

        // --- UPDATE: PAKAI VARIABEL SPLASH LIMIT ---
        // Dulu: if (stackedItems.Count >= 10)
        // Sekarang: Pakai variabel biar fleksibel
        if (stackedItems.Count >= splashLimit)
        {
            TriggerSplashEvent();
        }
    }

    void TriggerSplashEvent()
    {
        // A. Panggil Efek Teks di GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.TriggerSplashEffect();
        }

        // B. Hapus Semua Buah
        for (int i = stackedItems.Count - 1; i >= 0; i--)
        {
            if (stackedItems[i] != null)
            {
                Destroy(stackedItems[i].gameObject);
            }
        }

        // C. Reset List
        stackedItems.Clear();
        UpdateStackY();

        Debug.Log("🌊 SPLASH TRIGGERED! Tumpukan Bersih!");
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
        if (totalCount == 0)
        {
            // Reset posisi keranjang kalau kosong
            if (basketHolder != null)
            {
                basketHolder.localPosition = new Vector3(0, headYOffset, 0);
            }
            return;
        }

        float totalAvailableHeight = maxStackY - headYOffset;
        float requiredHeight = totalCount * normalSpacing;
        float currentSpacing = normalSpacing;

        if (requiredHeight > totalAvailableHeight)
        {
            currentSpacing = totalAvailableHeight / totalCount;
            currentSpacing = Mathf.Max(0.05f, currentSpacing);
        }

        float currentLocalY = headYOffset;

        for (int i = 0; i < totalCount; i++)
        {
            Transform item = stackedItems[i];

            float yPos = currentLocalY;
            float xOffset = i * currentSwayValue;

            item.localPosition = new Vector3(xOffset, yPos, 0);

            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10 + i;

            currentLocalY += currentSpacing;
        }

        if (basketHolder != null)
        {
            float basketX = totalCount * currentSwayValue;
            float basketY = Mathf.Min(currentLocalY, maxStackY + 0.5f);
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