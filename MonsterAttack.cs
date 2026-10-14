using UnityEngine;
using UnityEngine.SceneManagement; // Для перезапуска сцены
using System.Collections; // Для работы корутин

public class MonsterAttack : MonoBehaviour
{
    [Header("Настройки скримера")]
    [SerializeField] private GameObject screamerUI;   // Ссылка на картинку ScreamerUI
    [SerializeField] private AudioSource audioSource;  // Твой AudioSource для вопля
    [SerializeField] private AudioClip screamClip;     // Сам жуткий звук крика

    void OnTriggerEnter(Collider other)
    {
        // 1. Проверяем, что монстр врезался именно в ИГРОКА
        if (other.CompareTag("Player"))
        {
            // Запускаем корутину финала
            StartCoroutine(ScreamerRoutine());
        }
    }

        IEnumerator ScreamerRoutine()
    {
        // 1. Включаем страшную картинку на весь экран
        screamerUI.SetActive(true);

        // 2. Врубаем вопль
        if (audioSource != null && screamClip != null)
        {
            audioSource.PlayOneShot(screamClip);
        }

        // 3. Даем игроку погореть со скримера, например, 1.5 секунды
        yield return new WaitForSeconds(1.5f);

        // 4. ЖЕСТКО ВЫКЛЮЧАЕМ ИГРУ И ВЫЛЕТАЕМ НА РАБОЧИЙ СТОЛ
        Application.Quit(); 

        // Маленький лайфхак: в самом редакторе Unity команда Application.Quit() не закрывает окно (чтобы не вырубать редактор).
        // Чтобы вылет работал прямо во время тестов в Unity, можно дописать вот такой стоп-кран:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
