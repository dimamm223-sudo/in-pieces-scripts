using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] int getKey;
    [SerializeField] TextMeshProUGUI currentKeys;
    [SerializeField] AudioSource keySound;
    [SerializeField] GameObject finalKey;
    [SerializeField] GameObject blackScreen;
    public Transform labyrinthSpawnPoint;

        void OnTriggerEnter(Collider other)
    {
        // ВСЯ логика должна выполняться СТРОГО внутри этой проверки тега
        if(other.CompareTag("Key"))
        {
            getKey++;
            currentKeys.text = $"Ключей {getKey} / 5";
            keySound.Play();
            
            // Сначала делаем все проверки счетчика, пока объект не удален:
            if(getKey == 4)
            {
                finalKey.SetActive(true);
            }

            if(getKey == 5)
            {
                PlayerPrefs.SetInt("LabyrinthUnlocked", 1);
                PlayerPrefs.Save();

                blackScreen.SetActive(true);
                SceneManager.LoadScene("Lab");
            }

            Destroy(other.gameObject);
        }
    }


}
