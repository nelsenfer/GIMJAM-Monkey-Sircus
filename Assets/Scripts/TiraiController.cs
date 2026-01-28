using UnityEngine;

public class TiraiController : MonoBehaviour
{
    [SerializeField] private GameObject visualContainer;
    [SerializeField] private GameObject bg;
    [SerializeField] private Animator[] allAnimators;

    public void TampilkanTirai()
    {
        visualContainer.SetActive(true);
        foreach (Animator anim in allAnimators)
        {
            if (anim != null) 
            {
                anim.SetTrigger("TiraiShow");
                Debug.Log("Trigger TiraiShow telah dikirim ke: " + anim.gameObject.name);
            }
        }
        
    }

    public void SembunyikanTirai()
    {
        bg.SetActive(false);
        foreach (Animator anim in allAnimators)
        {
            if (anim != null) 
            {
                anim.SetTrigger("TiraiHide");
                // Tips: Pastikan log ini benar agar tidak bingung saat debugging
                Debug.Log("Trigger TiraiHide telah dikirim ke: " + anim.gameObject.name);
            }
        }
        // Ganti 0.5f sesuai durasi animasi sembunyi kamu di Unity
        Invoke("DisableObject", 2.0f); 
}

    private void DisableObject()
    {
        visualContainer.SetActive(false);
    }
}
