using UnityEngine;

public class PaddlePickup : MonoBehaviour
{
    public GameObject boatWithoutPaddles;
    public GameObject boatWithPaddles;
    public float interactDistance = 3f;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                CollectPaddles();
            }
        }
    }

    void CollectPaddles()
    {
        // Disable old boat
        boatWithoutPaddles.SetActive(false);

        // Enable new boat
        boatWithPaddles.SetActive(true);

        // Match position & rotation
        boatWithPaddles.transform.position = boatWithoutPaddles.transform.position;
        boatWithPaddles.transform.rotation = boatWithoutPaddles.transform.rotation;

        // Enable movement
        BoatController controller = boatWithPaddles.GetComponent<BoatController>();
        if (controller != null)
        {
            controller.EnableMovement();
        }

        // Remove paddles from world
        Destroy(gameObject);

        Debug.Log("Paddles collected! Boat upgraded and ready to move.");
    }

}
