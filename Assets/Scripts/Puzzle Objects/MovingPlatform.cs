using UnityEngine;
using UnityEngine.Rendering;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] int switches;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool played;
    [SerializeField] bool down;
    [SerializeField] bool sideZ;
    [SerializeField] bool sideX;
    Transform blockTransform;
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
        MovementZ();
        MovementX();
        MovementUp();
    }

    void MovementUp()
    {
        if (Switches == switchAmount && down)
        {
            if (!played)
            {
                PlaySoundFX();
                played = true;
                if (switchCam != null)
                {
                    if (switchCam1 && switchCam)
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
                    Invoke(nameof(UnSwitch), 4);

                }
            }

            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && down)
        {

            if (played)
            {

                PlaySoundFX();
                played = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementZ()
    {
        if (Switches == switchAmount && sideZ)
        {
            if (!played)
            {

                PlaySoundFX();
                played = true;
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
                    Invoke(nameof(UnSwitch), 4);

                }

            }
            targetPosition = originalPosition - Vector3.forward * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && sideZ)
        {
            if (played)
            {
                PlaySoundFX();
                played = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementX()
    {
        if (Switches == switchAmount && sideX)
        {
            if (!played)
            {

                PlaySoundFX();
                played = true;
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
                    Invoke(nameof(UnSwitch), 4);
                }
            }
            targetPosition = originalPosition - Vector3.right * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && sideX)
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
        else
        {

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
