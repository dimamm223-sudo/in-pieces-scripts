using UnityEngine;

public class PhysicsPickup : MonoBehaviour
{
    [Header("Настройки луча")]
    [SerializeField] private GameObject ventCover;

[SerializeField] private float PanelOnDistance = 2f;
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private LayerMask pickupLayer; 

    [Header("Настройки удержания")]
    [SerializeField] private Transform holdPosition;
    [SerializeField] private float attractSpeed = 12f;
    // Максимальная скорость, которую может развить предмет при притягивании
    [SerializeField] private float maxAttractSpeed = 15f; 

    [Header("Настройки UI")]
    public GameObject handIcon; 

    private Rigidbody grabbedObject; 

    void Awake()
{
    // 1. Берем имя сцены, которая запущена прямо сейчас
    string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

    // 2. Проверяем сохранение СТРОГО если мы находимся на первой сцене (в комнате)
    // Замени "НазваниеТвоейПервойСцены" на точное имя сцены с комнатой (например, "Room" или "SampleScene")
    if (currentSceneName == "SampleScene")
    {
        if (PlayerPrefs.HasKey("LabyrinthUnlocked") && PlayerPrefs.GetInt("LabyrinthUnlocked") == 1)
        {
            // Только отсюда перенаправляем в лабиринт
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lab");
        }
    }
}


    void Start()
    {
        if (handIcon != null) 
        {
            handIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickupObject();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DropObject();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, PanelOnDistance))
            {
                if(hit.collider.CompareTag("Switch"))
                {
                    ventCover.SetActive(false);
                }
            }
        }

        UpdateCrosshairVisibility();
    }

    void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            MoveObject();
        }
    }

    private void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance, pickupLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                grabbedObject = rb;
                grabbedObject.useGravity = false;
                
                // Сбрасываем накопленную скорость объекта в момент подбора, 
                // чтобы его не "швыряло" по инерции
                grabbedObject.linearVelocity = Vector3.zero;
                grabbedObject.angularVelocity = Vector3.zero;
            }
        }
    }

    private void UpdateCrosshairVisibility()
    {
        if (handIcon == null) return;

        if (grabbedObject != null)
        {
            handIcon.SetActive(true);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance, pickupLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                handIcon.SetActive(true);
            }
            else
            {
                handIcon.SetActive(false);
            }
        }
        else
        {
            handIcon.SetActive(false);
        }
    }
    

    private void MoveObject()
    {
        // Вычисляем направление и расстояние до целевой точки удержания
        Vector3 targetVelocity = (holdPosition.position - grabbedObject.position) * attractSpeed;
        
        // Ограничиваем скорость: Vector3.ClampMagnitude не дает вектору стать длиннее, чем maxAttractSpeed
        grabbedObject.linearVelocity = Vector3.ClampMagnitude(targetVelocity, maxAttractSpeed);
    }

    private void DropObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.useGravity = true;
            grabbedObject = null;
            UpdateCrosshairVisibility();
        }
    }
}
