using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public GameObject collisionIndicator;
    public Material defaultMaterial;
    public Material collisionMaterial;
    public AudioClip defaultCollisionSound;

    // 标签列表，与下方的音效列表一一对应
    public List<string> collisionTags = new List<string>();
    // 对应每个标签的音效列表，注意索引要对应
    public List<AudioClip> collisionSounds = new List<AudioClip>();

    // 用于控制是否显示碰撞指示器
    public bool showCollisionIndicator = true;

    // 最大速度：当速度达到此值或以上时，音量为最大（1）
    public float maxSpeed = 10f;
    // 最小速度：低于此速度时音量为0
    public float minSpeed = 0.1f;

    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    // 记录当前触发碰撞的 Collider 数量
    private int collisionCount = 0;

    // 用于计算移动速度的上一帧位置
    private Vector3 lastPosition;

    void Start()
    {
        // 获取 MeshRenderer 并设置默认材质
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }

        // 获取或添加 AudioSource 组件，并设置为循环播放
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // 根据 showCollisionIndicator 决定是否隐藏碰撞指示器
        if (collisionIndicator != null && !showCollisionIndicator)
        {
            collisionIndicator.SetActive(false);
        }

        // 初始化 lastPosition
        lastPosition = transform.position;
    }

    void Update()
    {
        // 用帧之间的位置差计算速度（单位：米/秒）
        float distance = Vector3.Distance(transform.position, lastPosition);
        float speed = distance / Time.deltaTime;
        float normalizedSpeed = 0f;

        // 低于最小速度时无声音，高于最小速度时将 [minSpeed, maxSpeed] 映射到 [0, 1]
        if (speed < minSpeed)
        {
            normalizedSpeed = 0f;
        }
        else
        {
            normalizedSpeed = Mathf.Clamp01((speed - minSpeed) / (maxSpeed - minSpeed));
        }

        // 根据速度调整音频整体音量
        if (audioSource != null)
        {
            audioSource.volume = normalizedSpeed;
        }
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;

        // 第一个 Collider 进入时，显示碰撞效果
        if (collisionCount == 1)
        {
            if (collisionIndicator != null && showCollisionIndicator)
            {
                collisionIndicator.SetActive(true);
            }
            if (meshRenderer != null && collisionMaterial != null)
            {
                meshRenderer.material = collisionMaterial;
            }
        }

        // 根据碰撞对象的标签选择音效
        AudioClip selectedSound = defaultCollisionSound;
        for (int i = 0; i < collisionTags.Count; i++)
        {
            if (other.CompareTag(collisionTags[i]))
            {
                if (i < collisionSounds.Count && collisionSounds[i] != null)
                {
                    selectedSound = collisionSounds[i];
                    break;
                }
            }
        }

        // 播放选中的音效
        if (audioSource != null)
        {
            audioSource.clip = selectedSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        if (collisionCount <= 0)
        {
            collisionCount = 0;
            // 所有 Collider 都退出时，恢复默认状态
            if (collisionIndicator != null && showCollisionIndicator)
            {
                collisionIndicator.SetActive(false);
            }
            if (meshRenderer != null && defaultMaterial != null)
            {
                meshRenderer.material = defaultMaterial;
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
