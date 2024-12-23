using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class Dialog_system : MonoBehaviour
{
    private Animator animator;// Animator�������
    private Vector2 targetPosition; // Ŀ��λ��
    private Rigidbody2D rb; // �����ƶ���Rigidbody2D���
    public GameObject dialogPanel; // �Ի����GameObject
    private bool isDialogActive = false; // �Ի����Ƿ񼤻�
    public LayerMask enemyLayer; // �������ڵ�LayerMask
    private HashSet<GameObject> encounteredEnemies = new HashSet<GameObject>(); // �������ĵ��˼���
    public float triggerDistance;
    public string name_self;
    public string name_enemy;

    [Header("UI���")]
    public Text textlable;           // * ������������ֿ򣬼�С���űȣ����������ָ�����
    public Image faceImage;

    [Header("�ı��ļ�")]  //Ҫ������ı��ַ����ĳ�UTF-8���ü��±�������ı� Ȼ�����Ϊ ע��Ҫ���Ϊ �ڱ���� ����и����� ����Ǹ��ĳ�UTF-8��֮����VS��д�ı�������
    public TextAsset textFile;
    public int index;

    public float TextSpeed;

    [Header("������ͷ��")]
    public Sprite face01, face02;

    bool textFinished;    //�Ƿ���ɴ���

    bool cancelTyping;    //ȡ����һ����

    private FileSystemWatcher watcher;//����������
    private string filePath;

    private GameObject message; // ������ʾ�Ի���GameObject����

    public static string background = "���ݡ��������塷�鼮�е�����";
    PromptGenerate promptGenerate = new PromptGenerate();
    ChatWithOpenAI gpt = new ChatWithOpenAI(background);    

    List<string> textList = new List<string>();
    // Start is called before the first frame update


    void Awake()
    {
        //GetTextFromFile(textFile);
        //SetupWatcher();


    }
    private void OnEnable()  //�յ���ʱ����ʾ��һ��
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
        //�ҵ��������µ�������chatBar
    }

    // Update is called once per frame


    void Update()
    {
        //SetupWatcher();

        //isChatting = false;
        //�����б�
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
            
            if (enemy.gameObject.layer == enemyLayer.value && !encounteredEnemies.Contains(enemy.gameObject) && enemy.gameObject != gameObject && !isChatting)  //�ų�����
            {
                name_enemy = enemy.gameObject.name;

                isChatting  = true;

                Persona person1 = new Persona(name_self, "κ��", 800, 100, 30, 5, 10, 8);  //�Լ���Ϣ
                Persona person2 = new Persona(name_enemy, "κ��", 800, 100, 30, 5, 10, 8);  //������Ϣ
                // ���˵�һ�ν��뷶Χ����ʾ�Ի���
                string prompt_input = promptGenerate.create_fight_prompt(person1, person2);
                gpt.setPrompt(prompt_input);

                //receive the response from the GPT use gpt.chat()
                //ע��chat���ص���Task<string>���ͣ���Ҫ��await����
                //responseString = await gpt.chat();
                messageContent = "������������";
                messageContent = await gpt.chat();
                //write responseString to chatBar
                //chatBar.GetComponent<TextMesh>().text = messageContent;




                //GetTextFromFile(textFile); // ���¼����ı��ļ�
                index = 0; // �����ı�����
                if (isDialogActive)
                {
                    //StartCoroutine(SetTextUI()); // ��������Э���Ը��¶Ի���
                }

                //OpenDialog();
                isDialogActive = true;
                encounteredEnemies.Add(enemy.gameObject); // ��������ӵ��������ļ�����
                break; // һ��ֻ����һ�����˶Ի�
            }
        }

        //// ����Ի����Ѽ�����ո���Ƿ񱻰����ԹرնԻ���
        //if (isDialogActive && Input.GetKeyDown(KeyCode.Space))
        //{
        //    CloseDialog();
        //}
    }
    void OpenDialog()
    {
        // ��ʾ�Ի���
        dialogPanel.SetActive(true);
    }

    void CloseDialog()
    {
        // ���ضԻ���
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
        filePath = Path.Combine(Application.dataPath, textFile.name); // ȷ��·����ȷ
        watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(filePath);
        watcher.Filter = Path.GetFileName(filePath);
        watcher.NotifyFilter = NotifyFilters.LastWrite; // ��������д��ʱ��ı仯
        watcher.Changed += OnChanged; // �¼�������
        watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        //GetTextFromFile(textFile); // ���¼����ı��ļ�
        index = 0; // �����ı�����
        textFinished = false; // �����ı����״̬
        cancelTyping = false; // ����ȡ������״̬
        Debug.Log("Onchange is used");
        if (isDialogActive)
        {
            //StartCoroutine(SetTextUI()); // ��������Э���Ը��¶Ի���
        }
    }

    IEnumerator SetTextUI()  //���ֻ�ȡ�ı�      ��Ҫ�õ�Э��
    {
        textFinished = false;
        //textlable.text = "";    //��ճ�ʼ�ı���

        //switch (textList[index])  //����ͷ��
        //{
        //    case "������":
        //        faceImage.sprite = face01;
        //        index++;
        //        break;
        //    case "�ܲ٣�":
        //        faceImage.sprite = face02;
        //        index++;
        //        break;
        //}

        ////for (global::System.Int32 i = 0; i < textList[index].Length; i++)
        ////{
        ////    textlable.text += textList[index][i];

        ////    yield return new WaitForSeconds(TextSpeed);  //����̶�ʱ��
        ////}

        //int letter = 0;
        //while (!cancelTyping && letter < textList[index].Length - 1)
        //{
        //    textlable.text += textList[index][letter];
        //    letter++;
        //    yield return new WaitForSeconds(TextSpeed);  //����̶�ʱ��
        //}
        //textlable.text = textList[index];
        //cancelTyping = false;
        //textFinished = true;
        //index++;

        while (index < textList.Count)
        {
            string line = textList[index];
            string trimmedLine = line.Trim();

            // ����ض���ǩ������ͷ��
            if (trimmedLine.StartsWith("����:"))
            {
                faceImage.sprite = face01;
            }
            else if (trimmedLine.StartsWith("�ܲ�:"))
            {
                faceImage.sprite = face02;
            }

            // ȥ���ض���ǩ
            int colonIndex = trimmedLine.IndexOf(':');
            if (colonIndex != -1)
            {
                line = trimmedLine.Substring(colonIndex + 1).Trim();
            }

            // ������ʾ�ı�
            int letter = 0;
            while (!cancelTyping && letter < line.Length)
            {
                //textlable.text += line[letter];
                letter++;
                yield return new WaitForSeconds(TextSpeed);  //����̶�ʱ��
            }
            //textlable.text = line;  // ��ʾ�����ı�
            index++;
        }
        cancelTyping = false;
        textFinished = true;
    }
}

