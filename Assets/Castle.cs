using UnityEngine;
using System.Collections;

public class Castle : MonoBehaviour
{
    public ArmyManager armyManager;
    [Header("Spawn Settings")]
    public GameObject spawnPrefab1;      // shu
    public GameObject spawnPrefab2;      // wei
    public GameObject spawnPrefab3;      // wu
    public GameObject spawnPrefab;      //����ȷ��ʹ�õ�Ԥ����
    public int spawnCount = 3;          // ÿ�����ɵ�����
    public float spawnRadius = 5f;      // ���ɰ뾶
    public float spawnHeightOffset = 0; // �߶�ƫ��
    public float spawnInterval = 20f;   // ���ɼ�����룩
    public bool spawnOnStart = true;    // �Ƿ�����Ϸ��ʼʱ���ɵ�һ��

    private Coroutine spawnCoroutine;   // Э������

    private void Start()
    {
        if (armyManager != null)
        {
            armyManager.RegisterCastle(this);
        }
        if (gameObject.tag == "shu")
        {
            spawnPrefab = spawnPrefab1;
        }
        else if (gameObject.tag == "wei")
        {
            spawnPrefab = spawnPrefab2;
        }
        else if (gameObject.tag == "wu")
        {
            spawnPrefab = spawnPrefab3;
        }
        spawnCount = 3;
        spawnInterval = 20f;
        // ������������Э��
        spawnCoroutine = StartCoroutine(PeriodicSpawn());
    }
    //�ݻ��߼�
    private void OnDestroy()
    {

        // ֹͣ����Э��
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    // ��������Ԥ����
    private IEnumerator PeriodicSpawn()
    {
        // ��ʼ�ȴ�����ѡ��
        yield return null;

        // ��Ϸ��ʼʱ�������ɵ�һ��
        if (spawnOnStart && spawnPrefab != null)
        {
            SpawnPrefabsAroundCastle();
        }

        // ��������ѭ��
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPrefab != null)
            {
                SpawnPrefabsAroundCastle();
            }
        }
    }

    // �ڳǱ���Χ����һ��Ԥ����
    private void SpawnPrefabsAroundCastle()
    {
        float angleIncrement = 360f / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = i * angleIncrement * Mathf.Deg2Rad;

            Vector3 spawnPos = new Vector3(
                transform.position.x + Mathf.Cos(angle) * spawnRadius,
                transform.position.y + spawnHeightOffset,
                transform.position.z + Mathf.Sin(angle) * spawnRadius
            );

            Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"������һ�� {spawnCount} �� {spawnPrefab.name} ��λ");
    }
}