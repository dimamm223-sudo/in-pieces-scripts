using UnityEngine;
using TMPro;

public class FinalDoor : MonoBehaviour
{
    [SerializeField] int finalKeysCount = 0;
    [SerializeField] TextMeshProUGUI finalCounterText;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip keySoundClip;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Key"))
        {
            finalKeysCount++;
            finalCounterText.text = $"Ключи: {finalKeysCount} / 999";
            audioSource.PlayOneShot(keySoundClip);
            Destroy(other.gameObject);

            if(finalKeysCount == 5)
            {
                PlayerPrefs.DeleteKey("LabyrinthUnlocked");
                victoryScreen.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
