using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    public float angle;

    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");

    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            //FieldOfViewCheck();
        }
    }
}
