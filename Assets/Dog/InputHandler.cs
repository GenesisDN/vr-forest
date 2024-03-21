using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Vector3 targetPosition;
    private DogAgentController dogAgentController;
    public GameObject dogGameObject;
    public InputActionProperty gripRightAnimationAction;
    public InputActionProperty gripLeftAnimationAction;
    public Transform rightHandController;
    public Transform leftHandController;
    public Transform cameraTransform;
    public float distancePetThreshold = 0.3f;
    public float distanceWalkTargetThreshold = 1.2f;


    void Start()
    {
        dogAgentController = dogGameObject.GetComponent<DogAgentController>();
    }


    void Update()
    {
        float gripValueRight = gripRightAnimationAction.action.ReadValue<float>();
        float gripValueLeft = gripLeftAnimationAction.action.ReadValue<float>();
        if (gripValueRight > 0 || gripValueLeft > 0)
        {
            Pet();
        }

        //WalkToCamera();

    }


    void Pet()
    {
        // Check distance for right hand controller
        if (rightHandController != null)
        {
            float distanceToRightHand = Vector3.Distance(rightHandController.position, dogGameObject.transform.position);
            if (distanceToRightHand < distancePetThreshold)
            {
                dogAgentController.Pet();
            }
        }

        // Check distance for right hand controller
        if (leftHandController != null)
        {
            float distanceToLeftHand = Vector3.Distance(leftHandController.position, dogGameObject.transform.position);
            if (distanceToLeftHand < distancePetThreshold)
            {
                dogAgentController.Pet();
            }
        }
    }


    void WalkToCamera()
    {
        if (dogGameObject != null && cameraTransform != null && Vector3.Distance(dogGameObject.transform.position, cameraTransform.position) > 2f)
        {
            Vector3 newPosition = cameraTransform.position + cameraTransform.forward * distanceWalkTargetThreshold;
            newPosition.y = 0.01f;
            dogAgentController.SetTargetPosition(newPosition);
        }
        else
        {
            dogAgentController.StopMovement();
        }
    }
}