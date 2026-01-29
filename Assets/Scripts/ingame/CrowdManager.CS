using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    public static CrowdManager instance;

    [Header("---- DAFTAR NPC ----")]
    [Tooltip("Tarik semua GameObject NPC (Monyet penonton) ke sini")]
    public NPCBouncer[] npcList;

    [Header("---- IDLE SETTINGS (Loncat Iseng) ----")]
    public float minIdleTime = 1.0f;
    public float maxIdleTime = 3.0f;

    [Header("---- PARTY SETTINGS (Splash/Strike) ----")]
    public int partyJumpCount = 3; // Berapa kali loncat pas Splash?

    private float idleTimer;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void Update()
    {
        // LOGIKA IDLE: Loncat satu-satu secara acak
        if (npcList.Length > 0)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                // Pilih 1 NPC acak
                int randomIndex = Random.Range(0, npcList.Length);

                // Suruh loncat 1x, tanpa delay
                npcList[randomIndex].DoJump(0f, 1);

                // Reset Timer
                idleTimer = Random.Range(minIdleTime, maxIdleTime);
            }
        }
    }

    // Panggil fungsi ini saat SPLASH atau STRIKE!
    public void TriggerCelebrate()
    {
        if (npcList.Length == 0) return;

        foreach (NPCBouncer npc in npcList)
        {
            // RAHASIA BIAR NATURAL: Random Delay! 🤫
            // Tiap NPC dikasih jeda acak antara 0.0 detik sampai 0.3 detik
            // Jadi mereka gak loncat kayak robot militer, tapi kayak suporter bola.
            float randomDelay = Random.Range(0f, 0.3f);

            // Suruh loncat berkali-kali (partyJumpCount)
            npc.DoJump(randomDelay, partyJumpCount);
        }
    }
}