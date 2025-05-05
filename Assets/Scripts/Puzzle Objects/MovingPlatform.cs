using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] int switches;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    [SerializeField] bool down;
    [SerializeField] bool sideZ;
    [SerializeField] bool sideX;
    Transform blockTransform;

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
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && down)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementZ()
    {
        if (Switches >= switchAmount && sideZ)
        {
            targetPosition = originalPosition - Vector3.forward * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && sideZ)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementX()
    {
        if (Switches >= switchAmount && sideX)
        {
            targetPosition = originalPosition - Vector3.right * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && sideX)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
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
