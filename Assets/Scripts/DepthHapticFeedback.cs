using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthHapticFeedback : MonoBehaviour
{
    // 指定控制器，这里使用 Oculus 右手手柄
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    // 当穿透达到该距离时，震动达到最大幅度
    public float maxPenetrationDistance = 1.0f;
    // 震动频率（可根据需求调整）
    public float vibrationFrequency = 0.5f;
    // cane 本身即为深度参考（棍尖），在 Inspector 中指定
    public Transform depthReference;

    // cane（及其子物体）上的所有 Collider
    private Collider[] caneColliders;

    void Start()
    {
        if (depthReference == null)
        {
            Debug.LogError("请在 Inspector 中指定 depthReference 对象（cane 本身）。");
            return;
        }
        // 获取 depthReference 及其子物体上的所有 Collider
        caneColliders = depthReference.GetComponentsInChildren<Collider>();
        if (caneColliders.Length == 0)
        {
            Debug.LogError("未在 depthReference 上找到任何 Collider。");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (depthReference == null)
            return;

        float maxPenetration = 0f;
        Vector3 direction;

        // 遍历 cane 上的所有 Collider，计算与外部物体 other 的穿透情况
        foreach (Collider col in caneColliders)
        {
            float penetrationDistance;
            // ComputePenetration 计算 col 与 other 的最小分离距离（penetrationDistance），
            // 如果返回 true 则说明存在重叠
            bool penetrating = Physics.ComputePenetration(
                col, col.transform.position, col.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out penetrationDistance);

            // 取所有重叠中最大的 penetrationDistance 作为“插入深度”
            if (penetrating && penetrationDistance > maxPenetration)
            {
                maxPenetration = penetrationDistance;
            }
        }

        // 当 penetration 越大，则表示 cane 越深插入其他物体
        // 归一化穿透深度，当 penetrationDistance 达到 maxPenetrationDistance 时震动幅度为 1
        float normalizedDepth = Mathf.Clamp01(maxPenetration / maxPenetrationDistance);
        float amplitude = normalizedDepth;

        // 调用 Oculus 震动接口，设置震动频率和幅度
        OVRInput.SetControllerVibration(vibrationFrequency, amplitude, controller);
        Debug.Log("Max penetration: " + maxPenetration + ", amplitude: " + amplitude);
    }

    void OnTriggerExit(Collider other)
    {
        // 离开 trigger 时停止震动
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
