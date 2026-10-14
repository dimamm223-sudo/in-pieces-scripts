using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light _flashlightLight;
    public AudioSource playOn;
    public AudioClip flashlightSound;

    void Start()
    {
        _flashlightLight = GetComponent<Light>();
    }

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.F))
       {
        
        if(_flashlightLight != null && playOn != null && flashlightSound != null)
        {
            playOn.PlayOneShot(flashlightSound);
            _flashlightLight.enabled = !_flashlightLight.enabled;
        }

       }
    }
}
