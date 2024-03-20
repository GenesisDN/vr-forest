using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAgentController : MonoBehaviour
{
    private Animator animator;
    private TerrainCollider terrainCollider;
    private Collider dogCollider;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private bool isMoving = false;
    private bool isFetchig = false;
    private bool isCarrying = false;
    private string movementAnimation = "MoveRun";
    private string targetTag;
    private float moveSpeed = 0.5f;
    public float walkingTresholdDistance = 1f;
    public float walkSpeed = 0.2f;
    public float runSpeed = 0.5f;
    public float swimSpeed = 0.1f;



    void Start()
    {
        animator = GetComponent<Animator>();
        dogCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }

    void FixedUpdate()
    {

        if (isMoving)
        {
            MoveToTarget();
        }
    }

    private void SetTargetTagPosition()
    {
        isFetchig = true;
        GameObject targetObject = GameObject.FindWithTag(targetTag);
        Vector3 targetObjectPosition = targetObject.transform.position + (targetObject.transform.forward * -0.2f);

        if (targetTag == "Bone")
        {
            targetObjectPosition = GetLongerSideOfBone(targetObject);
            SetTargetPosition(targetObjectPosition);
        }
    }

    private Vector3 GetLongerSideOfBone(GameObject bone)
    {

        Vector3 longerSideDirection = bone.transform.forward;
        if (bone.transform.localScale.x > bone.transform.localScale.z)
        {
            longerSideDirection = bone.transform.right;
        }
        return bone.transform.position + (longerSideDirection.normalized * 0.2f);
    }

    private void OnReachRotateToTarget()
    {
        GameObject boneObject = GameObject.FindWithTag(targetTag);
        BoxCollider boneCollider = boneObject.GetComponent<BoxCollider>();

        if (boneCollider != null)
        {
            Vector3 boneSize = boneCollider.size;
            Vector3 longerSideDirection;
            if (boneSize.x > boneSize.z)
            {
                longerSideDirection = boneObject.transform.right;
            }
            else
            {
                longerSideDirection = boneObject.transform.forward;
            }
            Vector3 centerPointOfLongerSide = boneObject.transform.position + longerSideDirection * boneSize.magnitude / 2f;
            Quaternion targetRotation = Quaternion.LookRotation(centerPointOfLongerSide - transform.position);
            rb.MoveRotation(targetRotation);
        }
        else
        {
            Debug.LogWarning("Bone object does not have a BoxCollider.");
        }
    }

    private void MoveToTarget()
    {
        isMoving = true;
        animator.SetBool(movementAnimation, true);
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
        rb.MoveRotation(Quaternion.LookRotation(rb.velocity));
    }

    private void TargetReach()
    {
        float distanceToTarget = Vector3.Distance(rb.position, targetPosition);
        float stopDistance = dogCollider.bounds.extents.magnitude;

        if (distanceToTarget <= stopDistance)
        {
            isMoving = false;
            animator.SetBool(movementAnimation, false);

            if (isFetchig)
            {
                OnReachRotateToTarget();
                PickUp();
                isFetchig = false;
            }
        }
    }

    private void MountBone()
    {
        GameObject mouth = GameObject.FindWithTag("Mouth");
        GameObject bone = GameObject.FindWithTag("Bone");
        bone.transform.parent = mouth.transform;
        bone.transform.localPosition = Vector3.zero;
        bone.transform.localRotation = Quaternion.identity;
    }

    private void UnmountBone()
    {
        GameObject bone = GameObject.FindWithTag("Bone");
        GameObject dogHead = GameObject.FindWithTag("DogHead");

        if (bone != null && dogHead != null)
        {
            // Calculate the drop position directly below the dog's head
            Vector3 dropPosition = dogHead.transform.position;
            dropPosition.y = 0.05f;

            RaycastHit hit;
            if (Physics.Raycast(dropPosition, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                // Adjust the drop position to be on the ground level
                dropPosition = hit.point;
            }

            bone.transform.SetParent(null);
            bone.transform.position = dropPosition;
            bone.transform.rotation = Quaternion.identity;
        }
    }

    private void PickUp()
    {
        if (targetTag != "Bone") return;
        isCarrying = true;
        targetTag = null;
        animator.SetTrigger("PickUp");
        animator.SetBool("Carry", true);
    }

    private void HandleMovementAnimation()
    {
        float distanceToTarget = Vector3.Distance(rb.transform.position, targetPosition);
        if (distanceToTarget > walkingTresholdDistance)
        {
            movementAnimation = "MoveRun";
            moveSpeed = runSpeed;
        }
        else
        {
            movementAnimation = "MoveWalk";
            moveSpeed = walkSpeed;
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        HandleMovementAnimation();
        MoveToTarget();
    }

    public void SetTargetTag(string targetTag)
    {
        GameObject targetObject = GameObject.FindWithTag(targetTag);
        if (targetObject == null) return;
        if (isCarrying) return;
        this.targetTag = targetTag;
        HandleMovementAnimation();
        SetTargetTagPosition();
    }

    public void Pet()
    {
        if (!isMoving && !isFetchig && !isCarrying)
        {
            animator.SetTrigger("PetMiddle");
        }
    }

    public void OnPickUp()
    {
        MountBone();
    }

    public void DropObject()
    {
        if (isCarrying)
        {
            isCarrying = false;
            animator.SetBool("Carry", false);
            animator.SetTrigger("PutDown");
        }
    }

    public void OnPutDown()
    {
        UnmountBone();
    }

    public void StopMovement()
    {
        isMoving = false;
        animator.SetBool(movementAnimation, false);
    }
}
