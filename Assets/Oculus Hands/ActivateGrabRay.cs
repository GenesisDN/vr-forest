using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateGrabRay : MonoBehaviour
{

    public GameObject leftGrabRay;
    public GameObject rightGrabRay;
    public XRDirectInteractor leftDirectInteractor;
    public XRDirectInteractor rightDirectInteractor;


    void Update()
    {
        leftGrabRay.SetActive(leftDirectInteractor.interactablesSelected.Count == 0);
        rightGrabRay.SetActive(rightDirectInteractor.interactablesSelected.Count == 0);
    }
}
