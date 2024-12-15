using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class PromptGenerate
{
    public string create_persona_info_prompt(Persona persona){
        List<string> prompt_input = new List<string>();
        prompt_input.Add(persona.name);
        prompt_input.Add(persona.faction);
        prompt_input.Add(persona.health.ToString());
        prompt_input.Add(persona.attack.ToString());
        prompt_input.Add(persona.defense.ToString());
        prompt_input.Add(persona.speed.ToString());
        prompt_input.Add(persona.mentality.ToString());
        prompt_input.Add(persona.vision_r.ToString());
        return string.Join(",", prompt_input);
    }

    public string create_all_persona_pos_prompt(List<Persona> personas){
        List<string> prompt_input = new List<string>();
        prompt_input.Add("\n现在各个将领的x坐标、y坐标和视野范围为:");
        for(int i=0;i<personas.Count;i++){
            string x = personas[i].getX().ToString();
            string y = personas[i].getY().ToString();
            string vision_r = personas[i].getVision().ToString();
            prompt_input.Add(personas[i].name+": x="+x+" y="+y+" 视野范围="+vision_r);
        }
        prompt_input.Add("将领可以检测到以视野范围为半径的圆的范围内其他将领");
        return string.Join(",", prompt_input);
    }

    public string create_persona_choice_prompt(){
        string prompt_input = "此时将领们根据视野范围内检测到的信息作出抉择，请生成每个将领此时的抉择，每个将领只能有一个抉择，抉择只能从移动，攻击，佯攻，防守，撤退，诈败这六个抉择中选择一个，抉择对象只能有一个，抉择要求符合将领的背景和性格以及战役背景，每个抉择中间需要换行，生成的格式为（将领姓名），（抉择名称），（抉择对象），其中移动、诈败、撤退和防守的抉择对象必须为\"无\"，这是一个示例：曹操，攻击，刘备";
        return prompt_input;
    }

    public string create_meet_prompt(Persona persona1, Persona persona2){
        List<string> prompt_input = new List<string>();
        string person1_name = persona1.name;
        string person2_name = persona2.name;
        prompt_input.Add("Now "+person1_name+"fight with "+person2_name);
        prompt_input.Add("generate two sentence that the two people will say according to the background of the story in Chinese, the format is: (Person\'s name):(words)");
        return string.Join(",", prompt_input);
    }

    // 读取文本文件的方法
    public string ReadTextFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                Debug.LogError("File does not exist: " + path);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading file: " + e.Message);
            return null;
        }
    }

}
