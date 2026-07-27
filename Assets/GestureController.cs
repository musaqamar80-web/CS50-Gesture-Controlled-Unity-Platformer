using UnityEngine;

public class GestureController : MonoBehaviour
{
    public YOLOInference yoloInference;
    public PlayerController playerController;
    public float gestureCooldown = 0.2f;

    private float lastGestureTime = -999f;

    void Start()
    {
        yoloInference.OnGesturesDetected += HandleGestures;
    }

    void HandleGestures(string left, string right)
    {
        if (Time.time - lastGestureTime < gestureCooldown) return;
        lastGestureTime = Time.time;

        if (right != "Open Palm" && right != "Point")
        {
            playerController.StandStill();
        }

        

        // RIGHT HAND — movement
        if (right != null)
        {
            switch (right)
            {
                case "Closed Fist": playerController.StandStill(); break;
                case "Open Palm": playerController.MoveRight(); break;
                case "Point": playerController.MoveLeft(); break;
                case "Peace": playerController.Rollright(); break;
                case "Ok": playerController.Rollleft(); break;
                case "Thumbs Up": QuitGame(); break;
            }
        }

        // LEFT HAND — jump / attack
        if (left != null)
        {
            switch (left)
            {
                
                case "Open Palm": playerController.Jump(); break;
                case "Point": playerController.Attack2(); break;
                case "Peace": playerController.Attack1(); break;
                case "Thumbs Up": QuitGame(); break;
            }
        }
    }

    void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    void OnDestroy()
    {
        yoloInference.OnGesturesDetected -= HandleGestures;
    }
}