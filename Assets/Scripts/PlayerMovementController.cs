using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float speed;
    public GameObject playerObject;
    // ָ�� Center Eye �� Transform���� Inspector �и�ֵ������ OVRCameraRig �� CenterEyeAnchor��
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
        // ��ȡ����ҡ������
        Vector2 rightJoystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // �� playerObject �� centerEye ����Ϊ����ҡ�����벻Ϊ��ʱ�����ƶ�
        if (playerObject != null && centerEye != null && rightJoystickInput != Vector2.zero)
        {
            // �� centerEye �ĳ���Ϊ��׼�������� y ����
            Vector3 forward = centerEye.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = centerEye.right;
            right.y = 0;
            right.Normalize();

            // ����ҡ����������ƶ�����ǰ����ҡ�� Y�����ң���ҡ�� X��
            Vector3 movementDirection = forward * rightJoystickInput.y + right * rightJoystickInput.x;
            movementDirection.Normalize();

            // ʹ����������ϵƽ�� playerObject
            playerObject.transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
        }
    }
}
