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

    [Header("Settings - Splash 🌊")]
    public int splashLimit = 10;

    [Header("🎭 Physics Sway (Inersia)")]
    [Range(0f, 0.5f)] public float swayMultiplier = 0.015f;
    public float swaySmoothness = 5f;
    public float maxSwayOffset = 4.0f;

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

    // --- FUNGSI UTAMA ---
    public void AddToStack(GameObject fruit)
    {
        // 1. AMBIL SCRIPT DULUAN (Buat ngecek KTP: Ini Buah atau Bom?)
        FallingItem itemScript = fruit.GetComponent<FallingItem>();

        if (itemScript == null) return; // Safety check

        // 2. PERCABANGAN LOGIKA
        if (itemScript.funValueAmount > 0)
        {
            // ==========================================
            // ✅ SKENARIO A: BUAH (DITUMPUK)
            // ==========================================

            // A. Matikan Fisika biar nempel
            Destroy(fruit.GetComponent<Rigidbody2D>());
            Destroy(fruit.GetComponent<Collider2D>());
            itemScript.enabled = false;

            // B. Masukkan ke List Visual Stack
            Transform fruitTransform = fruit.transform;
            Vector3 capturePosition = fruitTransform.position; // Simpan posisi buat popup skor

            stackedItems.Add(fruitTransform);
            fruitTransform.SetParent(transform);

            // C. Rapikan Tumpukan
            UpdateStackY();

            // D. Cek Splash (Penuh)
            if (stackedItems.Count >= splashLimit)
            {
                TriggerSplashEvent();
            }

            // E. Animasi & Skor
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                player.PlayHappyAnim();
                if (GameManager.instance != null)
                {
                    GameManager.instance.AddScore(itemScript.funValueAmount, capturePosition);
                }
            }
        }
        else
        {
            // ==========================================
            // 💣 SKENARIO B: BOM (LANGSUNG HANCUR)
            // ==========================================

            // A. Mainkan Animasi Sakit
            PlayerController player = GetComponent<PlayerController>();
            if (player != null) player.PlayHurtAnim();

            // B. Kurangi Nyawa Player
            if (GameManager.instance != null)
            {
                GameManager.instance.ReduceLives();
            }

            // C. HANCURKAN BOM-NYA (PENTING!)
            // Jangan dimasukkan ke stackedItems, langsung destroy aja biar hilang dari layar.
            Destroy(fruit);
        }
    }

    // --- FUNGSI SPLASH (SIBLING) ---
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

        if (CrowdManager.instance != null)
        {
            CrowdManager.instance.TriggerCelebrate();
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
            if (basketHolder != null) basketHolder.localPosition = new Vector3(0, headYOffset, 0);
            return;
        }

        float currentLocalY = headYOffset;

        for (int i = 0; i < totalCount; i++)
        {
            Transform item = stackedItems[i];
            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();

            FallingItem itemScript = item.GetComponent<FallingItem>();
            float myVisualOffset = 0f;
            float myHeightReduction = 0f;

            if (itemScript != null)
            {
                myVisualOffset = itemScript.visualOffsetY;
                myHeightReduction = itemScript.heightReduction;
            }

            float spriteHeight = 0.5f;
            if (sr != null) spriteHeight = sr.bounds.size.y;

            float halfHeight = spriteHeight / 2f;
            float yPos = currentLocalY + halfHeight + myVisualOffset;

            float xOffset = i * currentSwayValue;
            item.localPosition = new Vector3(xOffset, yPos, 0);

            if (sr != null) sr.sortingOrder = 10 + i;

            currentLocalY += spriteHeight - 0.1f - myHeightReduction;
        }

        if (basketHolder != null)
        {
            float basketX = totalCount * currentSwayValue;
            basketHolder.localPosition = new Vector3(basketX, currentLocalY, 0);
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