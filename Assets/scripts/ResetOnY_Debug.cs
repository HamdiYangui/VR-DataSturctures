using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ResetOnY_Debug : MonoBehaviour
{
    [SerializeField] private bool restoreTimeScale = true;

    private void Update()
    {
        bool pressedY = false;

#if ENABLE_INPUT_SYSTEM
        // New Input System
        if (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame)
            pressedY = true;
#endif

        // Legacy Input System
        if (Input.GetKeyDown(KeyCode.Y))
            pressedY = true;

        // Extra fallback: checks the typed character (works well for layouts)
        if (!string.IsNullOrEmpty(Input.inputString) &&
            (Input.inputString.Contains("y") || Input.inputString.Contains("Y")))
            pressedY = true;

        if (pressedY)
        {
            Debug.Log("Y detected -> resetting scene");
            ResetApplication();
        }
    }

    private void ResetApplication()
    {
        if (restoreTimeScale) Time.timeScale = 1f;

        var scene = SceneManager.GetActiveScene();

        if (scene.buildIndex >= 0)
        {
            SceneManager.LoadScene(scene.buildIndex);
        }
        else
        {
            Debug.LogError(
                $"Scene '{scene.name}' is NOT in Build Settings. " +
                $"Add it via File > Build Settings > Add Open Scenes."
            );
        }
    }
}
