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
        //���ڲ�ͬ��ս���������ɲ�ͬ�������������ʷ�����ڲ�ͬ��λ��
        switch (mapName)
        {
            case "NorthUnifyBattle":
                //���ɱ���ͳһս�۵������Լ����������������Ԥ���壬������������Ԥ���壬��������λ��������
                //������
                /*
                 * 1. �ܲپ�
                    - �ܲ�
                    - [갰�](https://baike.baidu.com/item/갰�/0?fromModule=lemma_inlink)
                    - [�ڽ�](https://baike.baidu.com/item/�ڽ�/0?fromModule=lemma_inlink)
                    - ����
                    - ����
                    - ����
                    - �ƽ��
                 * 2. Ԭ�ܾ�
                    - Ԭ��
                    - [Ԭ̷](https://baike.baidu.com/item/Ԭ̷/0?fromModule=lemma_inlink)
                    - [Ԭ��](https://baike.baidu.com/item/Ԭ��/0?fromModule=lemma_inlink)
                    - [�߸�](https://baike.baidu.com/item/�߸�/0?fromModule=lemma_inlink)
                    - [����](https://baike.baidu.com/item/����/0?fromModule=lemma_inlink)
                    - [������](https://baike.baidu.com/item/������/0?fromModule=lemma_inlink)
                 * 3. �ƽ��
                 * 4. ��׿
                 * 5. Ԭ��
                 * 6. ��ǫ
                 * 7. ̣�ٵ���
                 * 8. ���￵
                 */
                break;
            case "JiangdongUnifyBattle":
                //���ɽ���ͳһս�۵�����
                //������
                /*
                 * 1. ��߾�
                    - ���
                    - ����
                    - �Ƹ�
                    - �⾰
                 * 2. �����
                    -  ����
                    -  Ѧ��
                    -  ����
                 * 3. �ϰ׻���
                    - �ϰ׻�
                 * 4. ���ʾ�
                    - ����
                 */

                break;
            default:
                break;
        }
    }
}

