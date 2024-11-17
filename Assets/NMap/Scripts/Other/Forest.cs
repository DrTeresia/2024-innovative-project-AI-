using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    [SerializeField]
    private GameObject treePrefeb;

    private List<GameObject> trees = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        int treeCount = Random.Range(15, 20);

        for (int i = 0; i < treeCount; i++)
        {
            float dif_x = Random.Range(-2.0f, 2.0f);
            float dif_y = Random.Range(-2.0f, 2.0f);
            Vector3 pos = new Vector3(transform.position.x + dif_x, transform.position.y + dif_y, 0);
            //实例化树木, 并将树木设置为Forest的子物体
            GameObject tree = Instantiate(treePrefeb, pos, Quaternion.identity);
            tree.transform.SetParent(transform);

            trees.Add(tree);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //树木被砍伐减少
    public void CutDownTree()
    {
        if (trees.Count > 0)
        {
            GameObject tree = trees[0];
            trees.Remove(tree);
            Destroy(tree);
        }
    }
}
