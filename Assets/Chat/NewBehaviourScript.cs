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
        string background = "根据《三国演义》书籍中的内容和提供的游戏信息";
        PromptGenerate promptGenerate = new PromptGenerate();
        ChatWithOpenAI gpt = new ChatWithOpenAI(background);
        Persona Cao_cao = new Persona("曹操", "魏国",800, 100, 30, 5, 10, 4);
        Persona Guan_yu = new Persona("关羽", "蜀国",800, 100, 30, 5, 10, 4);
        Persona Liu_bei = new Persona("刘备", "蜀国",800, 100, 30, 5, 10, 4);
        Persona Sun_quan = new Persona("孙权", "吴国",800, 100, 30, 5, 10, 4);
        List<Persona> personas = new List<Persona>();
        personas.Add(Cao_cao);
        personas.Add(Liu_bei);
        personas.Add(Sun_quan);
        personas.Add(Guan_yu);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
