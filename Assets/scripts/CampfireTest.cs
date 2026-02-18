using UnityEngine;

[ExecuteAlways]   // runs in Edit mode
public class CampfireGenerator : MonoBehaviour
{
    [Header("Editor Controls")]
    public bool generateCampfire;

    private const string CAMPFIRE_ROOT_NAME = "Generated_Campfire";

    void OnValidate()
    {
        if (generateCampfire)
        {
            GenerateCampfire();
            generateCampfire = false; // auto-uncheck after creation
        }
    }

    void GenerateCampfire()
    {
        // Prevent duplicates
        Transform existing = transform.Find(CAMPFIRE_ROOT_NAME);
        if (existing != null)
        {
            Debug.Log("Campfire already exists.");
            return;
        }

        GameObject root = new GameObject(CAMPFIRE_ROOT_NAME);
        root.transform.SetParent(transform);
        root.transform.localPosition = Vector3.zero;

        CreateLogs(root.transform);
        CreateStones(root.transform);
    }

    void CreateLogs(Transform parent)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            log.name = "Log";
            log.transform.SetParent(parent);

            log.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            log.transform.localPosition = Vector3.up * 0.15f;
            log.transform.localRotation = Quaternion.Euler(90f, i * 60f, 0f);
        }
    }

    void CreateStones(Transform parent)
    {
        int count = 8;
        float radius = 0.6f;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;

            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            stone.name = "Stone";
            stone.transform.SetParent(parent);
            stone.transform.localPosition = pos;
            stone.transform.localScale = Vector3.one * 0.2f;
        }
    }
}
