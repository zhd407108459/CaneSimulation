using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float speed;
    public GameObject playerObject;
    // 指定 Center Eye 的 Transform，在 Inspector 中赋值（例如 OVRCameraRig 的 CenterEyeAnchor）
    public Transform centerEye;

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
        // 获取右手摇杆输入
        Vector2 rightJoystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // 当 playerObject 和 centerEye 均不为空且摇杆输入不为零时进行移动
        if (playerObject != null && centerEye != null && rightJoystickInput != Vector2.zero)
        {
            // 以 centerEye 的朝向为基准，但忽略 y 方向
            Vector3 forward = centerEye.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = centerEye.right;
            right.y = 0;
            right.Normalize();

            // 根据摇杆输入计算移动方向（前后：右摇杆 Y，左右：右摇杆 X）
            Vector3 movementDirection = forward * rightJoystickInput.y + right * rightJoystickInput.x;
            movementDirection.Normalize();

            // 使用世界坐标系平移 playerObject
            playerObject.transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
        }
    }
}
