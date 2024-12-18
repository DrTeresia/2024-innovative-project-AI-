using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterGen : MonoBehaviour
{
    // Start is called before the first frame update

    public string mapName;

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateCharacter()
    {
        //基于不同的战役名称生成不同的人物，并按照历史生成在不同的位置
        switch (mapName)
        {
            case "NorthUnifyBattle":
                //生成北方统一战役的势力以及人物，势力生成势力预制体，人物生成人物预制体，并且人物位于势力下
                //势力：
                /*
                 * 1. 曹操军
                    - 曹操
                    - [臧霸](https://baike.baidu.com/item/臧霸/0?fromModule=lemma_inlink)
                    - [于禁](https://baike.baidu.com/item/于禁/0?fromModule=lemma_inlink)
                    - 张绣
                    - 刘备
                    - 关羽
                    - 黄巾军
                 * 2. 袁绍军
                    - 袁绍
                    - [袁谭](https://baike.baidu.com/item/袁谭/0?fromModule=lemma_inlink)
                    - [袁熙](https://baike.baidu.com/item/袁熙/0?fromModule=lemma_inlink)
                    - [高干](https://baike.baidu.com/item/高干/0?fromModule=lemma_inlink)
                    - [颜良](https://baike.baidu.com/item/颜良/0?fromModule=lemma_inlink)
                    - [淳于琼](https://baike.baidu.com/item/淳于琼/0?fromModule=lemma_inlink)
                 * 3. 黄巾军
                 * 4. 董卓
                 * 5. 袁术
                 * 6. 陶谦
                 * 7. 蹋顿单于
                 * 8. 公孙康
                 */
                break;
            case "JiangdongUnifyBattle":
                //生成江东统一战役的人物
                //势力：
                /*
                 * 1. 孙策军
                    - 孙策
                    - 程普
                    - 黄盖
                    - 吴景
                 * 2. 刘繇军
                    -  刘繇
                    -  薛礼
                    -  笮融
                 * 3. 严白虎军
                    - 严白虎
                 * 4. 王朗军
                    - 王朗
                 */

                break;
            default:
                break;
        }
    }
}

