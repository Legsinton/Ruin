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

    void PlaySoundFX()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Chain, transform.position);
    }
    public void AddSwitch()
    {
        switches++;
    }
}
