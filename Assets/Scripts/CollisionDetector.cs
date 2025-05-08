using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public GameObject collisionIndicator;
    public Material defaultMaterial;
    public Material collisionMaterial;
    public AudioClip defaultCollisionSound;

    // ��ǩ�б����·�����Ч�б�һһ��Ӧ
    public List<string> collisionTags = new List<string>();
    // ��Ӧÿ����ǩ����Ч�б�ע������Ҫ��Ӧ
    public List<AudioClip> collisionSounds = new List<AudioClip>();

    // ���ڿ����Ƿ���ʾ��ײָʾ��
    public bool showCollisionIndicator = true;

    // ����ٶȣ����ٶȴﵽ��ֵ������ʱ������Ϊ���1��
    public float maxSpeed = 10f;
    // ��С�ٶȣ����ڴ��ٶ�ʱ����Ϊ0
    public float minSpeed = 0.1f;

    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    // ��¼��ǰ������ײ�� Collider ����
    private int collisionCount = 0;

    // ���ڼ����ƶ��ٶȵ���һ֡λ��
    private Vector3 lastPosition;

    void Start()
    {
        // ��ȡ MeshRenderer ������Ĭ�ϲ���
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }

        // ��ȡ����� AudioSource �����������Ϊѭ������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // ���� showCollisionIndicator �����Ƿ�������ײָʾ��
        if (collisionIndicator != null && !showCollisionIndicator)
        {
            collisionIndicator.SetActive(false);
        }

        // ��ʼ�� lastPosition
        lastPosition = transform.position;
    }

    void Update()
    {
        // ��֮֡���λ�ò�����ٶȣ���λ����/�룩
        float distance = Vector3.Distance(transform.position, lastPosition);
        float speed = distance / Time.deltaTime;
        float normalizedSpeed = 0f;

        // ������С�ٶ�ʱ��������������С�ٶ�ʱ�� [minSpeed, maxSpeed] ӳ�䵽 [0, 1]
        if (speed < minSpeed)
        {
            normalizedSpeed = 0f;
        }
        else
        {
            normalizedSpeed = Mathf.Clamp01((speed - minSpeed) / (maxSpeed - minSpeed));
        }

        // �����ٶȵ�����Ƶ��������
        if (audioSource != null)
        {
            audioSource.volume = normalizedSpeed;
        }
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;

        // ��һ�� Collider ����ʱ����ʾ��ײЧ��
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

        // ������ײ����ı�ǩѡ����Ч
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

        // ����ѡ�е���Ч
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
            // ���� Collider ���˳�ʱ���ָ�Ĭ��״̬
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
