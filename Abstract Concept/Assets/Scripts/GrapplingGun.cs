using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float maxDistance = 100f;
    private SpringJoint joint;
    
    //Editor Variables for Spring Joint
    public float Spring = 4.5f;
    public float Damper = 7f;
    public float Mass = 4.5f;

    //Fucken Experiments
    private bool LineActive = false;

    private float maxDis = 0;
    private float minDis = 0;






    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (LineActive && Input.GetMouseButtonUp(0))
        {
            StopGrapple();
            LineActive = false;
        }
        if (LineActive && Input.GetMouseButtonDown(1))
        {
            ContractGrapple();
        }
        else if (LineActive && Input.GetMouseButtonUp(1))
        {
            RelaxGrapple();
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            maxDis = distanceFromPoint * 0.8f;
            minDis = distanceFromPoint * 0.01f;

            joint.maxDistance = maxDis;
            joint.minDistance = minDis;

            //Adjust these values (Now with bonus in editor) to fit your game.
            joint.spring = Spring;
            joint.damper = Damper;
            joint.massScale = Mass;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            LineActive = true;
            Debug.Log("Line length" + maxDis);
        }
    }

    void ContractGrapple()
    {
        joint.maxDistance = maxDis / 10000;
    }

    void RelaxGrapple()
    {
        joint.maxDistance = maxDis * 10000;
    }

    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        if (LineActive)
        {
            lr.positionCount = 0;
            Destroy(joint);
        }
        
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
