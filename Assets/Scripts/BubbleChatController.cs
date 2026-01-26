using UnityEngine;

public class BubbleChatController : MonoBehaviour
{
    // Menggunakan Array agar bisa mengontrol lebih dari satu Animator
    [SerializeField] private GameObject visualContainer;
    [SerializeField] private Animator[] allAnimators; 

    public void TampilkanSemua()
    {
        visualContainer.SetActive(true);
        foreach (Animator anim in allAnimators)
        {
            if (anim != null) anim.SetTrigger("Show");
        }
    }

    public void SembunyikanSemua()
    {
        foreach (Animator anim in allAnimators)
        {
            if (anim != null) anim.SetTrigger("Hide");
        }
        Invoke("DisableObject", 0.5f); 
    }

    private void DisableObject()
    {
        visualContainer.SetActive(false);
    }
}