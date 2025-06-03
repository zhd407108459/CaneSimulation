using System.Collections;
using System.Collections.Generic;
using Event;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float speed;
    public GameObject playerObject;
    // Reference to the Center Eye Transform, assign in the Inspector (e.g., CenterEyeAnchor from OVRCameraRig)
    public Transform centerEye;
    
    private EventBinding<CompleteLevel> onCompleteLevel;
    
    private LinkedList<KeyValuePair<float, string>> positions = new LinkedList<KeyValuePair<float, string>>();

    void OnEnable()
    {
        onCompleteLevel = new EventBinding<CompleteLevel>(OnCompleteLevel);
        EventBus<CompleteLevel>.Register(onCompleteLevel);
    }

    void OnDisable()
    {
        EventBus<CompleteLevel>.Deregister(onCompleteLevel);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the speed is set to a reasonable default value if not assigned
        if (speed <= 0)
        {
            speed = 1.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the right-hand joystick
        Vector2 rightJoystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Move only when playerObject and centerEye are not null and joystick input is non-zero
        if (playerObject != null && centerEye != null && rightJoystickInput != Vector2.zero)
        {
            // Use the orientation of the centerEye as reference, ignoring the y component
            Vector3 forward = centerEye.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = centerEye.right;
            right.y = 0;
            right.Normalize();

            // Calculate movement direction based on joystick input (forward/back: joystick Y, left/right: joystick X)
            Vector3 movementDirection = forward * rightJoystickInput.y + right * rightJoystickInput.x;
            movementDirection.Normalize();

            // Translate playerObject in world space
            playerObject.transform.Translate(movementDirection * (speed * Time.deltaTime), Space.World);
        }
    }

    void FixedUpdate()
    {
        // Record player position per frame.
        positions.AddLast(new KeyValuePair<float, string>(Time.frameCount, playerObject.transform.position.ToString()));
    }
    
    private void OnCompleteLevel(CompleteLevel level)
    {
        // Save collision data.
        Debug.Log(Application.persistentDataPath);

        string positionData = JsonConvert.SerializeObject(positions, Formatting.Indented);
        System.IO.File.AppendAllText(Application.persistentDataPath + "/PositionData.json", "Session Time: " + Time.time + "\n" + positionData + "\n\n");
    }
}
