using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] private TiraiController tiraiManager;

    [Header("Referensi UI")]
    [SerializeField] private Image karakterImage;
    [SerializeField] private TextMeshProUGUI textNama;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private Animator tiraiKiriAnimator;
    [SerializeField] private Animator tiraiKananAnimator;


    [Header("Daftar Dialog")]
    [SerializeField] private DialogueLine[] daftarDialog;

    [Header("Aset Ekspresi Karakter")]
    [SerializeField] private Sprite ekspresiSenang;
    [SerializeField] private Sprite ekspresiTalk;
    [SerializeField] private Sprite ekspresiIdle;
    [SerializeField] private Sprite ekspresiHore;
    [SerializeField] private Sprite ekspresiAntusias;
    [SerializeField] private Sprite ekspresiSakit;
    [Header("GameObject")]
    [SerializeField] private GameObject move;
    [SerializeField] private GameObject buah;
    [SerializeField] private GameObject bomb;

    private int indexSekarang = -1;
    private static int tutorialSudahSelesai = 0;

    void Start()
    {
        StartCoroutine(MunculkanTiraiRoutine());
    }

    public void LanjutDialog()
    {
        int indexSebelumnya = indexSekarang;
        indexSekarang++;
        move.SetActive(false);
        buah.SetActive(false);
        bomb.SetActive(false);

        // CEK: Jika masih ada baris dialog di dalam array
        if (indexSekarang < daftarDialog.Length)
        {
            DialogueLine barisBaru = daftarDialog[indexSekarang];


            if (indexSebelumnya == -1 || barisBaru.namaKarakter != daftarDialog[indexSebelumnya].namaKarakter)
            {
                bubbleManager.TampilkanSemua();
            }

            // Update konten teks dan sprite
            UpdateVisualDialog(indexSekarang);
            if (indexSekarang == 5) move.SetActive(true);
            if (indexSekarang == 6) { buah.SetActive(true); }
            if (indexSekarang == 7) { bomb.SetActive(true); }
        }
        else
        {
            bubbleManager.SembunyikanSemua();
            indexSekarang = -1;
            //Tutorial.Instance.tutorialSelesai();
            StartCoroutine(MunculkanTiraiRoutine());
        }
    }

    public IEnumerator MunculkanTiraiRoutine()
    {
        // 1. Tampilkan Tirai
        tiraiManager.TampilkanTirai();
        Debug.Log("Animasi sedang bermain...");

        // 2. Tunggu selama durasi animasi (misal: 2 detik)
        // Sesuaikan angka 2f dengan panjang animasi TiraiShow kamu
        yield return new WaitForSeconds(3f);

        // 5. Lanjut Dialog
        if (Tutorial.sudahTutorial == 1 && Tutorial.sudahTutorial == 1)
        {
            Tutorial.Instance.tutorialSelesai();
        }
        if (tutorialSudahSelesai == 0)
        {

            tiraiManager.SembunyikanTirai();
            Debug.Log("Animasi selesai, sekarang bersembunyi");
            yield return new WaitForSeconds(3f);
            tutorialSudahSelesai = 1;
            Tutorial.Instance.simpanTutorial();
            LanjutDialog();
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