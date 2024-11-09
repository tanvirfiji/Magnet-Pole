using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsDisplay; // Assign a UI Text element in the Inspector
    private float deltaTime = 0.0f;

    private void Update()
    {
        // Update deltaTime with the time between frames
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate frames per second
        float fps = 1.0f / deltaTime;

        // Display the FPS as an integer
        if (fpsDisplay != null)
        {
            fpsDisplay.text = Mathf.Ceil(fps).ToString() + " FPS";
        }
    }
}
