using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string namaKarakter;
        [TextArea(3, 10)] public string isiTeks;
        public string namaEkspresi;
    }
    [SerializeField] private BubbleChatController bubbleManager;

    [Header("Referensi UI")]
    [SerializeField] private Image karakterImage;
    [SerializeField] private TextMeshProUGUI textNama;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private Animator bubbleNameAnimator;
    [SerializeField] private Animator spriteAnimator;

    [Header("Daftar Dialog")]
    [SerializeField] private DialogueLine[] daftarDialog;

    [Header("Aset Ekspresi Karakter")]
    [SerializeField] private Sprite ekspresiSenang;
    [SerializeField] private Sprite ekspresiTalk;
    [SerializeField] private Sprite ekspresiIdle;
    [SerializeField] private Sprite ekspresiHore;
    [SerializeField] private Sprite ekspresiAntusias;
    [SerializeField] private Sprite ekspresiSakit;

    private int indexSekarang = -1;

    void Start()
    {
        LanjutDialog();
    }

    public void LanjutDialog()
    {
        int indexSebelumnya = indexSekarang;
        indexSekarang++;

        // CEK: Jika masih ada baris dialog di dalam array
        if (indexSekarang < daftarDialog.Length)
        {
            DialogueLine barisBaru = daftarDialog[indexSekarang];

            // LOGIKA ANIMASI MUNCUL:
            // Munculkan animasi hanya jika:
            // 1. Ini dialog pertama kali (index -1 ke 0)
            // 2. Karakter yang bicara berbeda dari sebelumnya
            if (indexSebelumnya == -1 || barisBaru.namaKarakter != daftarDialog[indexSebelumnya].namaKarakter)
            {
                bubbleManager.TampilkanSemua();
            }

            // Update konten teks dan sprite
            UpdateVisualDialog(indexSekarang);
        }
        // LOGIKA JIKA ARRAY HABIS:
        else
        {
            // Memicu animasi fade out dan bergerak keluar
            bubbleManager.SembunyikanSemua();

            // Opsional: Reset index jika ingin dialog bisa diulang dari awal nanti
            indexSekarang = -1;
        }
    }

    private void UpdateVisualDialog(int index)
    {
        DialogueLine baris = daftarDialog[index];
        textNama.text = baris.namaKarakter;
        dialogText.text = baris.isiTeks;
        GantiEkspresi(baris.namaEkspresi);
    }

    public void GantiEkspresi(string namaEkspresi)
    {
        switch (namaEkspresi)
        {
            case "Senang": karakterImage.sprite = ekspresiSenang; break;
            case "Sakit": karakterImage.sprite = ekspresiSakit; break;
            case "Talk": karakterImage.sprite = ekspresiTalk; break;
            case "Idle": karakterImage.sprite = ekspresiIdle; break;
            case "Hore": karakterImage.sprite = ekspresiHore; break;
            case "Antusias": karakterImage.sprite = ekspresiAntusias; break;
        }
    }
}