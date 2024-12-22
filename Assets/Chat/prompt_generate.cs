using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class PromptGenerate
{
    public string create_persona_info_prompt(List<Persona> personas){
        List<string> prompt_input = new List<string>();
        prompt_input.Add("现在我们模拟这个战役完成一次游戏，各个人物的数据如下：");
        foreach (Persona p in personas){
            prompt_input.Add("name:"+p.name);
            prompt_input.Add("faction:"+p.faction);
            prompt_input.Add("health:"+p.health.ToString());
            prompt_input.Add("attack:"+p.attack.ToString());
            prompt_input.Add("defense:"+p.defense.ToString());
            prompt_input.Add("speed:"+p.speed.ToString());
            prompt_input.Add("mentality:"+p.mentality.ToString());
            prompt_input.Add("vision_r::"+p.vision_r.ToString());
        }
        return string.Join(",", prompt_input);
    }

    public string create_all_persona_pos_prompt(Persona persona){
        List<string> prompt_input = new List<string>();
        prompt_input.Add("现在"+persona.name+"周围检测到的信息为:");
        string info = "";
        foreach(Persona p in persona.surroundings){
            info+=p.name+",";    
        }
        prompt_input.Add(info);
        prompt_input.Add("只有检测到的信息才能对其进行抉择,检测到自己忽略(无法对自己进行抉择),检测不到的信息无法对其进行抉择");
        return string.Join(",", prompt_input);
    }

    public string create_persona_choice_prompt(){
        string prompt_input = "此时将领们根据检测到的信息作出抉择,请生成每个将领此时的抉择,每个将领只能有一个抉择,抉择只能从移动,攻击,佯攻,防守,撤退,诈败,结盟这七个抉择中选择一个,抉择对象只能有一个,抉择要求符合将领的背景和性格以及战役背景,每个抉择中间需要换行,生成的格式为（将领姓名）,（抉择名称）,（抉择对象）,其中移动,诈败,撤退和防守的抉择对象必须为无,这是一个示例:曹操,攻击,刘备";
        return prompt_input;
    }

    public string create_fight_prompt(Persona persona1, Persona persona2){
        List<string> prompt_input = new List<string>();
        string person1_name = persona1.name;
        string person2_name = persona2.name;
        prompt_input.Add("Now "+person1_name+"fight with "+person2_name);
        prompt_input.Add("generate two sentence that the two people will say according to the background of the story in Chinese, the format is: (Person\'s name):(words)");
        return string.Join(",", prompt_input);
    }

    public string create_align_prompt(Persona persona1, Persona persona2){
        List<string> prompt_input = new List<string>();
        string person1_name = persona1.name;
        string person2_name = persona2.name;
        prompt_input.Add("Now "+person1_name+"align with "+person2_name);
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
            string content = File.ReadAllText(path);
            content = content.Replace("\\", ""); // 去除反斜杠
            content = content.Replace("\n", ""); // 去除换行符
            content = content.Replace("\r", ""); // 去除回车符
            content = content.Replace("\t", ""); // 去除制表符
            // 继续添加其他需要去除的转义符
            return content;
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
