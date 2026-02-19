using UnityEngine;

public class PaddlePickup : MonoBehaviour
{
    public GameObject boatWithoutPaddles;
    public GameObject boatWithPaddles;
        public GameObject boatWithoutPaddles2;
    public GameObject boatWithPaddles2;
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
        boatWithoutPaddles2.SetActive(false);

        // Enable new boat
        boatWithPaddles.SetActive(true);
         boatWithPaddles2.SetActive(true);


        // Match position & rotation
        boatWithPaddles.transform.position = boatWithoutPaddles.transform.position;
        boatWithPaddles.transform.rotation = boatWithoutPaddles.transform.rotation;

         boatWithPaddles2.transform.position = boatWithoutPaddles2.transform.position;
        boatWithPaddles2.transform.rotation = boatWithoutPaddles2.transform.rotation;

        // Enable movement
        BoatController controller = boatWithPaddles.GetComponent<BoatController>();
        BoatController controller2 = boatWithPaddles2.GetComponent<BoatController>();
        if (controller != null || controller2!=null)
        {
            controller.EnableMovement();
            controller2.EnableMovement();
        }

        // Remove paddles from world
        Destroy(gameObject);

        Debug.Log("Paddles collected! Boat upgraded and ready to move.");
    }

}
