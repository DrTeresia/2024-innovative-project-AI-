using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

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
    public string name_self;
    public string name_enemy;

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

    private FileSystemWatcher watcher;//监听器变量
    private string filePath;

    private GameObject message; // 用于显示对话的GameObject子类

    public static string background = "根据《三国演义》书籍中的内容";
    PromptGenerate promptGenerate = new PromptGenerate();
    ChatWithOpenAI gpt = new ChatWithOpenAI(background);    

    List<string> textList = new List<string>();
    // Start is called before the first frame update


    void Awake()
    {
        //GetTextFromFile(textFile);
        //SetupWatcher();


    }
    private void OnEnable()  //刚弹出时就显示第一句
    {
        //textlable.text = textList[index];
        //index++;
        textFinished = true;
        //StartCoroutine(SetTextUI());

    }
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //dialogPanel.SetActive(false);
        name_self = gameObject.name;
        //找到该物体下的子物体chatBar
    }

    // Update is called once per frame


    void Update()
    {
        //SetupWatcher();

        //isChatting = false;
        //敌人列表
        Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(transform.position, triggerDistance).Where(c => c.gameObject.layer == enemyLayer.value).ToArray();


        if (enemiesInView.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) && index == textList.Count)
            {
                index = 0;
                //CloseDialog();
                return;
            }
            Dialogtrigger(enemiesInView, encounteredEnemies);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (textFinished && !cancelTyping)
                {
                    //StartCoroutine(SetTextUI());
                }
                else if (!textFinished && !cancelTyping)
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
    void OnDestroy()
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }
    string messageContent;
    private bool isChatting = false;
    async void Dialogtrigger(Collider2D[] enemies, HashSet<GameObject> encounteredEnemies)
    {

        foreach (var enemy in enemies)
        {
            
            if (enemy.gameObject.layer == enemyLayer.value && !encounteredEnemies.Contains(enemy.gameObject) && enemy.gameObject != gameObject && !isChatting)  //排除自身
            {
                name_enemy = enemy.gameObject.name;

                isChatting  = true;

                Persona person1 = new Persona(name_self, "魏国", 800, 100, 30, 5, 10, 8);  //自己信息
                Persona person2 = new Persona(name_enemy, "魏国", 800, 100, 30, 5, 10, 8);  //对面信息
                // 敌人第一次进入范围，显示对话框
                string prompt_input = promptGenerate.create_fight_prompt(person1, person2);
                gpt.setPrompt(prompt_input);

                //receive the response from the GPT use gpt.chat()
                //注意chat返回的是Task<string>类型，需要用await接收
                //responseString = await gpt.chat();
                messageContent = "。。。。。。";
                messageContent = await gpt.chat();
                //write responseString to chatBar
                //chatBar.GetComponent<TextMesh>().text = messageContent;




                //GetTextFromFile(textFile); // 重新加载文本文件
                index = 0; // 重置文本索引
                if (isDialogActive)
                {
                    //StartCoroutine(SetTextUI()); // 重新启动协程以更新对话框
                }

                //OpenDialog();
                isDialogActive = true;
                encounteredEnemies.Add(enemy.gameObject); // 将敌人添加到已遇到的集合中
                break; // 一次只能与一个敌人对话
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

        foreach (var line in lineDate)
        {
            textList.Add(line);
        }

    }
    private void SetupWatcher()
    {
        filePath = Path.Combine(Application.dataPath, textFile.name); // 确保路径正确
        watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(filePath);
        watcher.Filter = Path.GetFileName(filePath);
        watcher.NotifyFilter = NotifyFilters.LastWrite; // 仅监控最后写入时间的变化
        watcher.Changed += OnChanged; // 事件处理器
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        //GetTextFromFile(textFile); // 重新加载文本文件
        index = 0; // 重置文本索引
        textFinished = false; // 重置文本完成状态
        cancelTyping = false; // 重置取消打字状态
        Debug.Log("Onchange is used");
        if (isDialogActive)
        {
            //StartCoroutine(SetTextUI()); // 重新启动协程以更新对话框
        }
    }

    IEnumerator SetTextUI()  //逐字获取文本      需要用到协程
    {
        textFinished = false;
        //textlable.text = "";    //清空初始文本框

        //switch (textList[index])  //更换头像
        //{
        //    case "刘备：":
        //        faceImage.sprite = face01;
        //        index++;
        //        break;
        //    case "曹操：":
        //        faceImage.sprite = face02;
        //        index++;
        //        break;
        //}

        ////for (global::System.Int32 i = 0; i < textList[index].Length; i++)
        ////{
        ////    textlable.text += textList[index][i];

        ////    yield return new WaitForSeconds(TextSpeed);  //间隔固定时间
        ////}

        //int letter = 0;
        //while (!cancelTyping && letter < textList[index].Length - 1)
        //{
        //    textlable.text += textList[index][letter];
        //    letter++;
        //    yield return new WaitForSeconds(TextSpeed);  //间隔固定时间
        //}
        //textlable.text = textList[index];
        //cancelTyping = false;
        //textFinished = true;
        //index++;

        while (index < textList.Count)
        {
            string line = textList[index];
            string trimmedLine = line.Trim();

            // 检测特定标签并更换头像
            if (trimmedLine.StartsWith("刘备:"))
            {
                faceImage.sprite = face01;
            }
            else if (trimmedLine.StartsWith("曹操:"))
            {
                faceImage.sprite = face02;
            }

            // 去除特定标签
            int colonIndex = trimmedLine.IndexOf(':');
            if (colonIndex != -1)
            {
                line = trimmedLine.Substring(colonIndex + 1).Trim();
            }

            // 逐字显示文本
            int letter = 0;
            while (!cancelTyping && letter < line.Length)
            {
                //textlable.text += line[letter];
                letter++;
                yield return new WaitForSeconds(TextSpeed);  //间隔固定时间
            }
            //textlable.text = line;  // 显示整行文本
            index++;
        }
        cancelTyping = false;
        textFinished = true;
    }
}

