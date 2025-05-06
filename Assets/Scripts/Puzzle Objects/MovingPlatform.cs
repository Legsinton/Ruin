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
        if (Switches >= switchAmount && down)
        {
            if (!played)
            {
                PlaySoundFX();
                played = true;
                if (switchCam1) 
                {
                    switchCam.SetCamera(1);
                }
                else if (switchCam2)
                {
                    switchCam.SetCamera(2);
                }
                else if (switchCam3)
                {
                    switchCam.SetCamera(3);
                }
                else
                {

                }
                Invoke(nameof(UnSwitch), 3);
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
        if (Switches >= switchAmount && sideZ)
        {
            if (!played)
            {
                
                PlaySoundFX();
                played = true;
                if (switchCam1)
                {
                    switchCam.SetCamera(1);
                }
                else if (switchCam2)
                {
                    switchCam.SetCamera(2);
                }
                else if (switchCam3)
                {
                    switchCam.SetCamera(3);
                }
                else
                {

                }
                Invoke(nameof(UnSwitch), 3);

            }
            targetPosition = originalPosition - Vector3.forward * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && sideZ)
        {
            if (played)
            {
                PlaySoundFX();
                played= false;
            }
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementX()
    {
        if (Switches >= switchAmount && sideX)
        {
            if (!played)
            {
                
                PlaySoundFX();
                played = true; 
                if (switchCam1)
                {
                    switchCam.SetCamera(1);
                }
                else if (switchCam2)
                {
                    switchCam.SetCamera(2);
                }
                else if (switchCam3)
                {
                    switchCam.SetCamera(3);
                }
                else
                {

                }
                Invoke(nameof(UnSwitch), 3);
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
        if ( switchCam1 || switchCam2 || switchCam3)
        {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pullable"))
        {
            blockTransform = collision.transform;
            blockTransform.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pullable"))
        {
            blockTransform = null;
            if (blockTransform != null)
            {
                blockTransform.transform.SetParent(null);
            }
        }
    }
    public void AddSwitch()
    {
        switches++;
    }
}
