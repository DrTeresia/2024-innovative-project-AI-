import os
import sys

from backend_server.persona_structure.memory_structures.associative_memory import AssociativeMemory
from backend_server.persona_structure.memory_structures.scratch import Scratch
from backend_server.persona_structure.memory_structures.spatial_memory import MemoryTree

sys.path.append('../../')
from backend_server.persona_structure.cognitive_modules.perceive import perceive

class Persona:
    def __init__(self, name, faction, health, attack, defense, speed, mentality, vision_r, skill, folder_mem_saved=False):
        #将领的基本信息
        self.name = name
        self.faction = faction
        self.health = health
        self.attack = attack
        self.defense = defense
        self.speed = speed
        self.mentality = mentality
        self.vision_r = vision_r
        self.skill = skill



        if not os.path.exists('F:\\CS project/2024 fall innovative project\\AI三国/frontend_server\\storage\\test\\personas/{}/bootstrap_memory/associative_memory'.format(self.name)):
            os.makedirs('F:\\CS project/2024 fall innovative project\\AI三国/frontend_server\\storage\\test\\personas/{}/bootstrap_memory/associative_memory'.format(self.name))

        f_s_mem_saved = f"{folder_mem_saved}/bootstrap_memory/spatial_memory.json"
        self.s_mem = MemoryTree(f_s_mem_saved)

        f_a_mem_saved = f"{folder_mem_saved}/bootstrap_memory/associative_memory"
        # self.a_mem = AssociativeMemory(f_a_mem_saved)

        scratch_saved = f"{folder_mem_saved}/bootstrap_memory/scratch.json"
        self.scratch = Scratch(scratch_saved)


    def save(self, save_folder):
        f_s_mem = f"{save_folder}/spatial_memory.json"
        self.s_mem.save(f_s_mem)

        # Associative memory contains a csv with the following rows:
        # [event.type, event.created, event.expiration, s, p, o]
        # e.g., event,2022-10-23 00:00:00,,Isabella Rodriguez,is,idle

        # f_a_mem = f"{save_folder}/associative_memory"
        # self.a_mem.save(f_a_mem)

        # Scratch contains non-permanent data associated with the persona. When
        # it is saved, it takes a json form. When we load it, we move the values
        # to Python variables.
        f_scratch = f"{save_folder}/scratch.json"
        self.scratch.save(f_scratch)
    #
    #
    def perceive(self, map):
        return perceive(self, map)
    #
    # def retrieve(self, perceived_info):
    #     return retrieve(self, perceived_info)
    #
    # def plan(self, map, personas, retrieved):
    #     return plan(self, map, personas, retrieved)
    #
    # def execute(self, map, personas, plan):
    #     return execute(self, map, personas, plan)
    #
    # def reflect(self):
    #     reflect(self)
    #
    # def move(self, map, personas, curr_tile):
    #     # Updating persona's scratch memory with <curr_tile>.
    #     self.scratch.curr_tile = curr_tile
    #
    #     # Main cognitive sequence begins here.
    #     perceived = self.perceive(map)
    #     retrieved = self.retrieve(perceived)
    #     plan = self.plan(map, personas, retrieved)
    #     self.reflect()
    #     return self.execute(map, personas, plan)







