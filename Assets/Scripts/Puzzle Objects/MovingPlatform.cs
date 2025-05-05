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
    [SerializeField] bool side;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
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

        if (Switches >= switchAmount && side)
        {
            targetPosition = originalPosition - Vector3.forward * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        else if (Switches != switchAmount && side)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void AddSwitch()
    {
        switches++;
    }
}
