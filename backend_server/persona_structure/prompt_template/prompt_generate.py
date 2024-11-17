class PromptGenerate:
    def __init__(self):
        pass
    def create_persona_info_prompt(self,persona):
        prompt_input = [
                persona.name,
                persona.health,
                persona.attack,
                persona.defense,
                persona.speed,
                persona.mentality,
                persona.vision_r,
                persona.skill]
        return prompt_input

    def create_event_prompt(self,eventList):
        prompt_input = []
        for event in eventList:
            prompt_input.append([
                event.persona.name,
                event.landmark.name,
                event.x,
                event.y,
                event.info
            ])
        return prompt_input

    def create_meet_prompt(self,persona1,persona2):
        prompt_input = []
        person1_name = persona1.name
        person2_name = persona2.name
        prompt_input.append('Now {} meet {}, he can attack {} but will be attacked by {},'
                              'he can elude so that he will get less chance to be attacked by {} but defense-50,'
                              'he can hold so that he\'s defense+100, still have a bigger chance to be attacked than elude, what will he do?'
                              'There are 3 choices:'
                              'attack,'
                              'elude,'
                              'hold'.format(person1_name,person2_name,person2_name,person2_name,person2_name)
                              )

        return prompt_input