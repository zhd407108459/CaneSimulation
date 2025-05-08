using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthHapticFeedback : MonoBehaviour
{
    // ָ��������������ʹ�� Oculus �����ֱ�
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    // ����͸�ﵽ�þ���ʱ���𶯴ﵽ������
    public float maxPenetrationDistance = 1.0f;
    // ��Ƶ�ʣ��ɸ������������
    public float vibrationFrequency = 0.5f;
    // cane ����Ϊ��Ȳο������⣩���� Inspector ��ָ��
    public Transform depthReference;

    // cane�����������壩�ϵ����� Collider
    private Collider[] caneColliders;

    void Start()
    {
        if (depthReference == null)
        {
            Debug.LogError("���� Inspector ��ָ�� depthReference ����cane ������");
            return;
        }
        // ��ȡ depthReference �����������ϵ����� Collider
        caneColliders = depthReference.GetComponentsInChildren<Collider>();
        if (caneColliders.Length == 0)
        {
            Debug.LogError("δ�� depthReference ���ҵ��κ� Collider��");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (depthReference == null)
            return;

        float maxPenetration = 0f;
        Vector3 direction;

        // ���� cane �ϵ����� Collider���������ⲿ���� other �Ĵ�͸���
        foreach (Collider col in caneColliders)
        {
            float penetrationDistance;
            // ComputePenetration ���� col �� other ����С������루penetrationDistance����
            // ������� true ��˵�������ص�
            bool penetrating = Physics.ComputePenetration(
                col, col.transform.position, col.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out penetrationDistance);

            // ȡ�����ص������� penetrationDistance ��Ϊ��������ȡ�
            if (penetrating && penetrationDistance > maxPenetration)
            {
                maxPenetration = penetrationDistance;
            }
        }

        // �� penetration Խ�����ʾ cane Խ�������������
        // ��һ����͸��ȣ��� penetrationDistance �ﵽ maxPenetrationDistance ʱ�𶯷���Ϊ 1
        float normalizedDepth = Mathf.Clamp01(maxPenetration / maxPenetrationDistance);
        float amplitude = normalizedDepth;

        // ���� Oculus �𶯽ӿڣ�������Ƶ�ʺͷ���
        OVRInput.SetControllerVibration(vibrationFrequency, amplitude, controller);
        Debug.Log("Max penetration: " + maxPenetration + ", amplitude: " + amplitude);
    }

    void OnTriggerExit(Collider other)
    {
        // �뿪 trigger ʱֹͣ��
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
