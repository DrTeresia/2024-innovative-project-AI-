using UnityEngine;
using System.Collections;

public class Castle : MonoBehaviour
{
    public ArmyManager armyManager;
    [Header("Spawn Settings")]
    public GameObject spawnPrefab1;      // shu
    public GameObject spawnPrefab2;      // wei
    public GameObject spawnPrefab3;      // wu
    public GameObject spawnPrefab;      //最终确定使用的预制体
    public int spawnCount = 10;          // 每批生成的数量
    public float spawnRadius = 5f;      // 生成半径
    public float spawnHeightOffset = 0; // 高度偏移
    public float spawnInterval = 15f;   // 生成间隔（秒）
    public bool spawnOnStart = true;    // 是否在游戏开始时生成第一波

    private Coroutine spawnCoroutine;   // 协程引用

    private void Start()
    {
        if (armyManager != null)
        {
            armyManager.RegisterCastle(this);
        }
        if (gameObject.tag == spawnPrefab1.tag)
        {
            spawnPrefab = spawnPrefab1;
        }
        else if (gameObject.tag == spawnPrefab2.tag)
        {
            spawnPrefab = spawnPrefab2;
        }
        else if (gameObject.tag == spawnPrefab3.tag)
        {
            spawnPrefab = spawnPrefab3;
        }
        // 启动定期生成协程
        spawnCoroutine = StartCoroutine(PeriodicSpawn());
    }
    //摧毁逻辑
    private void OnDestroy()
    {

        // 停止生成协程
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    // 定期生成预制体
    private IEnumerator PeriodicSpawn()
    {
        // 初始等待（可选）
        yield return null;

        // 游戏开始时立即生成第一波
        if (spawnOnStart && spawnPrefab != null)
        {
            SpawnPrefabsAroundCastle();
        }

        // 定期生成循环
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPrefab != null)
            {
                SpawnPrefabsAroundCastle();
            }
        }
    }

    // 在城堡周围生成一批预制体
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

        Debug.Log($"生成了一批 {spawnCount} 个 {spawnPrefab.name} 单位");
    }
}