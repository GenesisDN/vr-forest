using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAgentController : MonoBehaviour
{
    private Animator animator;
    private Collider dogCollider;
    private Vector3 targetPosition;
    private AudioSource audioSource;
    public AudioClip barkSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip dropSound;
    public AudioClip squeekSound;
    public AudioClip pantSound;
    private string currentClip;
    private Rigidbody rb;
    private bool isMoving = false;
    private bool isFetching = false;
    private bool isCarrying = false;
    private bool canFollow = true;
    private float moveSpeed = 0.5f;
    public float walkingTresholdDistance = 1f;
    public float walkSpeed = 0.2f;
    public float runSpeed = 0.5f;
    public SphereCollider characterCollider;
    public Camera cameraObject;
    public LayerMask terrainLayerMask;
    public GameObject bone;

    private float lastSoundTime = 0f;
    private float minTimeBetweenSounds = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        dogCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        HandleSound("pant");
    }

    void FixedUpdate()
    {
        if (isFetching) FetchBone();
        if (canFollow) FollowCharacter();
        if (isMoving)
        {
            MoveToTarget();
            if (isFetching) PickUp();

        }
    }

    private void FollowCharacter()
    {
        SetPlayerTargetPosition();

        if (dogCollider.bounds.Intersects(characterCollider.bounds) ||
           Vector3.Distance(transform.position, cameraObject.transform.position) < 0.5f)
        {
            isMoving = false;
            HandleMovementAnimation();
            if (isCarrying) DropObject();
            return;
        }
    }

    private void SetPlayerTargetPosition()
    {
        RaycastHit hit;
        Ray ray = new Ray(characterCollider.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            Vector3 newPosition = hit.point;
            newPosition.y += 0.03f;

            Vector3 cameraPosition = cameraObject.transform.position;
            Vector3 cameraForward = cameraObject.transform.forward;
            Vector3 position = cameraPosition + (cameraForward * 0.5f);
            newPosition.x = position.x;
            newPosition.z = position.z;

            this.targetPosition.y = newPosition.y;
            this.targetPosition.x = position.x;
            this.targetPosition.z = position.z;

            this.isMoving = true;
            HandleMovementAnimation();
        }
    }

    public void StartFetching()
    {
        isFetching = true;
        canFollow = false;
    }

    public void FetchBone()
    {
        if (bone == null) return;
        if (isCarrying) return;

        Vector3 targetObjectPosition = GetLongerSideOfBone(bone);
        RaycastHit hit;
        Ray ray = new Ray(targetObjectPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            Vector3 newPosition = hit.point;
            newPosition.y += 0.01f;

            this.targetPosition.y = newPosition.y;
            this.targetPosition.x = newPosition.x;
            this.targetPosition.z = newPosition.z;

            this.isMoving = true;
            HandleMovementAnimation();
        }
    }

    private Vector3 GetLongerSideOfBone(GameObject bone)
    {

        Vector3 longerSideDirection = bone.transform.forward;
        if (bone.transform.localScale.x > bone.transform.localScale.z)
        {
            longerSideDirection = bone.transform.right;
        }
        return bone.transform.position + (longerSideDirection.normalized * 0.01f);
    }


    private void MoveToTarget()
    {
        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
        rb.MoveRotation(Quaternion.LookRotation(rb.velocity));
    }


    private void PickUp()
    {
        if (Vector3.Distance(rb.transform.position, bone.transform.position) < 0.5f) {
            isFetching = false;
            isMoving = false;
            isCarrying = true;

            HandleMovementAnimation();

            animator.SetTrigger("PickUp");
            animator.SetBool("Carry", true);
        }
    }

    public void DropObject()
    {
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
        directionToCamera.y = 0f;
        transform.rotation = Quaternion.LookRotation(directionToCamera);

        isCarrying = false;
        animator.SetBool("Carry", false);
        animator.SetTrigger("PutDown");
    }

    private void MountBone()
    {
        GameObject mouth = GameObject.FindWithTag("Mouth");
        Rigidbody boneRb = bone.GetComponent<Rigidbody>();
        boneRb.isKinematic = true;

        bone.transform.parent = mouth.transform;
        bone.transform.localPosition = Vector3.zero;
        bone.transform.localRotation = Quaternion.identity;

        audioSource.PlayOneShot(squeekSound);
    }

    private void UnmountBone()
    {
        GameObject dogHead = GameObject.FindWithTag("DogHead");

        if (bone != null && dogHead != null)
        {
            bone.transform.SetParent(null);
            Rigidbody boneRb = bone.GetComponent<Rigidbody>();
            boneRb.isKinematic = false;
        }
    }

    public void OnPickUp()
    {
        MountBone();
        canFollow = true;
    }

    public void OnPutDown()
    {
        UnmountBone();
    }

    public void OnBark()
    {
        audioSource.PlayOneShot(barkSound);
    }

    private void HandleMovementAnimation()
    {
        if (!isMoving)
        {
            animator.SetBool("MoveRun", false);
            animator.SetBool("MoveWalk", false);
            HandleSound("pant");
            return;
        }

        float distanceToTarget = Vector3.Distance(rb.transform.position, targetPosition);
        if (distanceToTarget > walkingTresholdDistance)
        {
            moveSpeed = runSpeed;
            animator.SetBool("MoveRun", true);
            animator.SetBool("MoveWalk", false);
            HandleSound("run");
        }
        else
        {
            moveSpeed = walkSpeed;
            animator.SetBool("MoveRun", false);
            animator.SetBool("MoveWalk", true);
            HandleSound("walk");
        }
    }

    private void HandleSound(string newClip)
    {
        if ((currentClip != null && currentClip.Equals(newClip)) || Time.time - lastSoundTime < minTimeBetweenSounds) return;

        currentClip = newClip;
        audioSource.clip = getClip(currentClip);
        audioSource.Play();
        lastSoundTime = Time.time;

    }

    private AudioClip getClip(string clip)
    {
        switch(clip)
        {
            case "bark": return barkSound;
            case "run": return runSound;
            case "walk": return walkSound;
            case "drop": return dropSound;
            case "squeek": return squeekSound;
        }

        return pantSound;
    }

    public void Pet()
    {
        if (!isMoving && !isFetching && !isCarrying)
        {
            animator.SetTrigger("PetMiddle");
        }
    }
}