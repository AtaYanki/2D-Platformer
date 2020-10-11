using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GameObject cross;
    public GrappleRope grappleRope;
    public CharacterMovement character;

    [Header("Layers Settings:")]
    public LayerMask grappable;

    [Header("Main Camera:")]
    public Camera cam;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D joint;

    [Header("Distance:")]
    [SerializeField]private float maxDistance = 10;

    [Header("Launching:")]
    [SerializeField]private float launchSpeed = 1;

    [HideInInspector]public bool hasRigidbody;
    [HideInInspector]public Vector2 grapplePoint;
    [HideInInspector]public Vector2 grappleDistanceVector;

    private void Start()
    {
        joint.enabled = false;
        grappleRope.enabled = false;
    }

    private void Update()
    {
        Vector2 dist = cam.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, dist.normalized, 100, grappable))
        {
            RaycastHit2D grappablePoint = Physics2D.Raycast(firePoint.position, dist.normalized, 100, grappable);
            if (Vector2.Distance(grappablePoint.point, firePoint.position) <= maxDistance)
            {
                cross.SetActive(true);
                cross.transform.position = grappablePoint.point;
            }else
            {
                cross.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if(Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            { 
                RotateGun(grapplePoint);
            }
            else
            {
                Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            hasRigidbody = false;
            joint.enabled = false;
            character.enabled = true;
            joint.connectedBody = null;
            grappleRope.enabled = false;
        }
        else
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos);
        }
    }

    void RotateGun(Vector3 lookPoint)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = cam.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistance, grappable))
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistance, grappable);
            if (Vector2.Distance(hit.point, firePoint.position) <= maxDistance)
            {
                grapplePoint = hit.point;
                character.enabled = false;
                grappleRope.enabled = true;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                if(hit.transform.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    hasRigidbody = true;
                    character.enabled = true;
                    joint.connectedAnchor = new Vector2(0,0);
                    joint.connectedBody = hit.transform.gameObject.GetComponent<Rigidbody2D>();
                }
            }
        }
    }

    public void Grapple()
    {
        if(!hasRigidbody)
        {
            joint.autoConfigureDistance = false;

            joint.connectedAnchor = grapplePoint;
            Vector2 firePointDistanceVector = firePoint.position - gunHolder.position;
            joint.distance = firePointDistanceVector.magnitude;
            joint.frequency = launchSpeed;
            joint.enabled = true;
        }
        else
        {
            joint.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}
