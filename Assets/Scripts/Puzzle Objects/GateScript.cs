using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    [SerializeField] int switches;
    [SerializeField] Outline outlineScript;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public bool solved;
    public float pressDepth;
    public float moveSpeed;
    bool played;
    [SerializeField] bool canSwitch;
    [SerializeField] SwitchCamera switchCam;
    [SerializeField] private bool switchCam1;
    [SerializeField] private bool switchCam2;
    [SerializeField] private bool switchCam3;
    public PlayerMovement playerMovement;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Switches >= switchAmount)
        {
            if (!played)
            {
                PlaySoundFX();
                played = true;
                Invoke(nameof(CameraSwitch), 0.5f);
            }
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount)
        {
            if (played)
            {
                PlaySoundFX();
                played = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void UnSwitch()
    {
        if (switchCam1 || switchCam2 || switchCam3)
        {
            playerMovement.enabled = true;
            playerMovement.movement = new Vector3(0, 0, 0);
            switchCam.SetCamera(0);
        }
    }

    void CameraSwitch()
    {
        if (switchCam != null)
        {
            if (switchCam1)
            {
                playerMovement.movement = new Vector3(0, 0, 0);
                playerMovement.enabled = false;
                switchCam.SetCamera(1);
            }
            else if (switchCam2)
            {
                playerMovement.movement = new Vector3(0, 0, 0);
                playerMovement.enabled = false;
                switchCam.SetCamera(2);
            }
            else if (switchCam3)
            {
                playerMovement.movement = new Vector3(0, 0, 0);
                playerMovement.enabled = false;
                switchCam.SetCamera(3);
            }
            else
            {

            }
            Invoke(nameof(UnSwitch), 3);

        }
    }

    void PlaySoundFX()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Chain, transform.position);
    }
    public void AddSwitch()
    {
        switches++;
    }
}
