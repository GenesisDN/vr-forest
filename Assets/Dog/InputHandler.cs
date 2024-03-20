using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private TerrainCollider terrainCollider;
    private Collider dogCollider;
    private Vector3 targetPosition;
    private DogAgentController dogAgentController;
    public GameObject dogGameObject;


    void Start()
    {
        dogAgentController = dogGameObject.GetComponent<DogAgentController>();
        dogCollider = GetComponent<Collider>();
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (dogCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                dogAgentController.Pet();
            }
            else if (terrainCollider.Raycast(ray, out hit, 1000))
            {
                targetPosition = hit.point;
                dogAgentController.SetTargetPosition(targetPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key was pressed");
            dogAgentController.SetTargetTag("Bone");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D key was pressed");
            dogAgentController.DropObject();
        }
    }
}
