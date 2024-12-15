using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake ()
    {
        string background = "根据《三国演义》书籍中的内容";
        PromptGenerate promptGenerate = new PromptGenerate();
        ChatWithOpenAI gpt = new ChatWithOpenAI(background);
        Persona Cao_cao = new Persona("曹操", "魏国",800, 100, 30, 5, 10, 4);
        Persona Liu_bei = new Persona("刘备", "蜀国",800, 100, 30, 5, 10, 4);
        Persona Sun_quan = new Persona("孙权", "吴国",800, 100, 30, 5, 10, 4);
        List<Persona> personas = new List<Persona>();
        personas.Add(Cao_cao);
        personas.Add(Liu_bei);
        personas.Add(Sun_quan);
        Cao_cao.setPos(5,5);
        Liu_bei.setPos(3,3);
        Sun_quan.setPos(10,10);
        string prompt_input = promptGenerate.create_meet_prompt(Cao_cao,Liu_bei);
        // string prompt_input = promptGenerate.ReadTextFile("C:\\Users\\21196\\Desktop\\战役背景\\荆州之战.txt")+promptGenerate.create_all_persona_pos_prompt(personas)+promptGenerate.create_persona_choice_prompt();
        gpt.setPrompt(prompt_input);
        Debug.Log(prompt_input);
        gpt.chat();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
