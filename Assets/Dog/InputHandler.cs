using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private DogAgentController dogAgentController;
    public GameObject dogGameObject;
    public Transform characterTransform;

    void Start()
    {
        dogAgentController = dogGameObject.GetComponent<DogAgentController>();
    }


    void Update()
    {
        
    }


    
}