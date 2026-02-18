using UnityEngine;

public class BoatCamera : MonoBehaviour
{
    public Camera playerCamera;   // drag your player camera here

    void Awake()
    {
        GetComponent<Camera>().enabled = false;
    }

    public void Activate()
    {
        if (playerCamera != null) playerCamera.enabled = false;
        GetComponent<Camera>().enabled = true;
    }

    public void Deactivate()
    {
        GetComponent<Camera>().enabled = false;
        if (playerCamera != null) playerCamera.enabled = true;
    }
}