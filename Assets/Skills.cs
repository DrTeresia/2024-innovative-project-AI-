<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public GameObject skillPrefab; // 技能预制体
    public float skillRange;// 技能距离角色的距离
    //private Animator animator;// Animator组件引用


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // 检测WASD键的按下事件
        if (Input.GetKeyDown(KeyCode.W))
        {
            ReleaseSkill(new Vector2(0, 1)); // 向上释放技能
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ReleaseSkill(new Vector2(-1, 0)); // 向左释放技能
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ReleaseSkill(new Vector2(0, -1)); // 向下释放技能
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReleaseSkill(new Vector2(1, 0)); // 向右释放技能
        }
    }

    void ReleaseSkill(Vector2 direction)
    {
        if (skillPrefab != null)
        {
            // 将方向向量标准化，确保技能释放的距离一致
            Vector2 normalizedDirection = direction.normalized;

            // 计算技能的实例化位置：角色位置 + 方向 * 技能释放距离
            Vector3 skillPosition = transform.position + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * skillRange;

            // 实例化技能预制体
            Instantiate(skillPrefab, skillPosition, Quaternion.identity);
        }
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public GameObject skillPrefab; // 技能预制体
    public float skillRange;// 技能距离角色的距离
    //private Animator animator;// Animator组件引用


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // 检测WASD键的按下事件
        if (Input.GetKeyDown(KeyCode.W))
        {
            ReleaseSkill(new Vector2(0, 1)); // 向上释放技能
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ReleaseSkill(new Vector2(-1, 0)); // 向左释放技能
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ReleaseSkill(new Vector2(0, -1)); // 向下释放技能
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReleaseSkill(new Vector2(1, 0)); // 向右释放技能
        }
    }

    void ReleaseSkill(Vector2 direction)
    {
        if (skillPrefab != null)
        {
            // 将方向向量标准化，确保技能释放的距离一致
            Vector2 normalizedDirection = direction.normalized;

            // 计算技能的实例化位置：角色位置 + 方向 * 技能释放距离
            Vector3 skillPosition = transform.position + new Vector3(normalizedDirection.x, normalizedDirection.y, 0) * skillRange;

            // 实例化技能预制体
            Instantiate(skillPrefab, skillPosition, Quaternion.identity);
        }
    }
}
>>>>>>> origin/main
