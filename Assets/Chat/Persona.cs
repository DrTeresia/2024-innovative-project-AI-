using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona
{
    public string name;
    public string faction;
    public int health, attack, defense, speed, mentality, vision_r;
    public List<Persona> surroundings = new List<Persona>();
    public int x,y;
    public Persona(string name = "未命名", string faction = "未知", int health = 100, int attack = 10, int defense = 10, int speed = 8, int mentality = 10, int vision_r = 4){
        this.name = name;
        this.faction = faction;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.vision_r = vision_r;
        this.x = 0;
        this.y = 0;
    }

    public void changeSurroundings(List<Persona> personas){
        this.surroundings = personas;
    }    
    public void setName(string name)
    {
        this.name = name;
    }
}
