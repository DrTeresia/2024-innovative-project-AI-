using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dialog_system : MonoBehaviour
{
    private Animator animator;// Animator组件引用
    private Vector2 targetPosition; // 目标位置
    private Rigidbody2D rb; // 用于移动的Rigidbody2D组件
    public GameObject dialogPanel; // 对话框的GameObject
    private bool isDialogActive = false; // 对话框是否激活
    public LayerMask enemyLayer; // 敌人所在的LayerMask
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // 已遇到的敌人集合
    public float triggerDistance;

    [Header("UI组件")]
    public Text textlable;           // * 调大字体和文字框，减小缩放比，即可让文字更清晰
    public Image faceImage;

    [Header("文本文件")]  //要把你的文本字符集改成UTF-8，用记事本打开你的文本 然后另存为 注意要另存为 在保存的 左边有个编码 你把那个改成UTF-8，之后在VS里写文本都可以
    public TextAsset textFile;
    public int index;

    public float TextSpeed;

    [Header("讲话人头像")]
    public Sprite face01, face02;

    bool textFinished;    //是否完成打字

    bool cancelTyping;    //取消逐一打字


    List<string> textList = new List<string>();
    // Start is called before the first frame update

    void Awake()
    {
        GetTextFromFile(textFile);
    }
    private void OnEnable()  //刚弹出时就显示第一句
    {
        //textlable.text = textList[index];
        //index++;
        textFinished = true;
        StartCoroutine(SetTextUI());

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dialogPanel.SetActive(false);
    }

    // Update is called once per frame


    void Update()
    {

        //敌人列表
        Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, triggerDistance).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();
        

        if (enemiesInView.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) && index == textList.Count)
            {
                index = 0;
                CloseDialog();
                return;
            }
            Dialogtrigger(enemiesInView, encounteredEnemies);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textFinished && !cancelTyping)
                {
                    StartCoroutine(SetTextUI());
                }
                else if(!textFinished && !cancelTyping)
                {
                    cancelTyping = true;

                }
                //textlable.text = textList[index];
                //index++;
                //StartCoroutine(SetTextUI());

            }

        }

        //if (Input.GetKeyDown(KeyCode.Space)&&index == textList.Count)
        //{
        //    index = 0;
        //    return;
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) 
        //{
        //    textlable.text = textList[index];
        //    index++;
        //}
    }
    void Dialogtrigger(Collider2D[] enemies, HashSet<GameObject> encounteredEnemies)
    {

        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.layer == enemyLayer.value && !encounteredEnemies.Contains(enemy.gameObject) && enemy.gameObject != gameObject)  //排除自身
            {
                // 敌人第一次进入范围，显示对话框
                OpenDialog();
                isDialogActive = true;
                encounteredEnemies.Add(enemy.gameObject); // 将敌人添加到已遇到的集合中
                break; // 假设一次只能与一个敌人对话
            }
        }

        //// 如果对话框已激活，检测空格键是否被按下以关闭对话框
        //if (isDialogActive && Input.GetKeyDown(KeyCode.Space))
        //{
        //    CloseDialog();
        //}
    }
    void OpenDialog()
    {
        // 显示对话框
        dialogPanel.SetActive(true);
    }

    void CloseDialog()
    {
        // 隐藏对话框
        dialogPanel.SetActive(false);
        isDialogActive = false;
    }
    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;

        var lineDate = file.text.Split('\n');

        foreach(var line in lineDate)
        {
            textList.Add(line);
        }
        
    }

    IEnumerator SetTextUI()  //逐字获取文本      需要用到协程
    {
        textFinished = false;
        textlable.text = "";    //清空初始文本框

        switch (textList[index])  //更换头像
        {
            case "A":
                faceImage.sprite = face01;
                index++;
                break;
            case "B":
                faceImage.sprite = face02;
                index++;
                break;
        }

        //for (global::System.Int32 i = 0; i < textList[index].Length; i++)
        //{
        //    textlable.text += textList[index][i];

        //    yield return new WaitForSeconds(TextSpeed);  //间隔固定时间
        //}

        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textlable.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(TextSpeed);  //间隔固定时间
        }
        textlable.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }
}
