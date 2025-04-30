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

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Switches >= switchAmount)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        if (Switches != switchAmount)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void AddSwitch()
    {
        switches++;
    }
}
