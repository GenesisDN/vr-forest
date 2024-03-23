using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private DogAgentController dogAgentController;
    public GameObject dogGameObject;
    public Transform cameraTransform;
    public float distanceWalkTargetThreshold = 1.2f;
    public LayerMask terrainLayerMask;

    void Start()
    {
        dogAgentController = dogGameObject.GetComponent<DogAgentController>();
    }


    void Update()
    {
        //FollowCharacter();
    }


    void FollowCharacter()
    {
        if (dogAgentController != null && cameraTransform != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(cameraTransform.position, Vector3.down);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
            {
                Vector3 newPosition = hit.point;
                newPosition.y += 0.01f; // Offset to prevent clipping into terrain
                dogAgentController.SetTargetPosition(newPosition);
            }
        }
    }
}