using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona
{
    public string name;
    public string faction;
    public int health, attack, defense, speed, mentality, vision_r;

    public int x,y;
    public Persona(string name, string faction, int health, int attack, int defense, int speed, int mentality, int vision_r){
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

    public void setPos(int x,int y){
        this.x = x;
        this.y = y;
    }

    public int getX(){
        return x;
    }
    
    public int getY(){
        return y;
    }

    public int getVision(){
        return vision_r;
    }
}
