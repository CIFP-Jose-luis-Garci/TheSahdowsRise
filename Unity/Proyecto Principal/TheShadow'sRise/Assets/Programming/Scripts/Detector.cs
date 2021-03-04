using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private Light spotLight;
    private float viewDistance;
    private float viewAngle;
    [SerializeField] LayerMask viewMask;
    private float almostViewDistance;
    private Transform player;
    [HideInInspector] public static bool spotted;
    [HideInInspector] public static bool almostSpotted;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        spotLight = GetComponent<Light>();
        viewAngle = spotLight.spotAngle;
        viewDistance = spotLight.range;
        almostViewDistance = spotLight.range * 2;
        spotted = false;
        almostSpotted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerScript.currentRoom == 1)
        {
            if (CanSeePlayer())
            {
                spotted = true;
            }
            else
            {
                spotted = false;
            }
            if (CanAlmostSeePlayer())
            {
                almostSpotted = true;
            }
            else
            {
                almostSpotted = false;
            }
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);
    }
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool CanAlmostSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < almostViewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}

