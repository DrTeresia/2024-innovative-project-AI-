import json
import threading
from backend_server.persona import *
from backend_server.persona_structure.cognitive_modules.chat_with_gpt import *
from backend_server.persona_structure.prompt_template.prompt_generate import *

save_path = 'F:\CS project\\2024 fall innovative project\\2024-innovative-project-AI-\\frontend_server\storage\\test\dialog'

prompt_generate = PromptGenerate()
def chat_with_persona(persona, personas, chat, prompt_message, persona_index):
    if persona.faction==personas[(persona_index + 1) % 5].faction:
        prompt_message.append(prompt_generate.create_league_prompt(persona, personas[(persona_index + 1) % 5]))
    else:
        prompt_message.append(prompt_generate.create_meet_prompt(persona, personas[(persona_index + 1) % 5]))
    prompt_message.append('also generate a sentence that the person will say according to the background of the story in Chinese, the format is: (Person\'s name):(words)')
    message = str(prompt_message)
    result = chat.chat_with_user(message)
    print(f"AI ({persona.name}): ", result)
    file_path = os.path.join(save_path)
    with open(file_path, 'a', encoding='utf-8') as file:
        file.write(result + '\n')
    prompt_message.pop()

def main():
    chat = ChatWithOpenAI(api_key)

    persona1 = Persona('Cao Cao', '魏国',800, 100, 30, 5, 10, 8, None, 'F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/Cao Cao')
    persona3 = Persona('Liu Bei', '蜀国',1000, 50, 30, 5, 10, 8, None, 'F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/Liu Bei')
    persona2 = Persona('Sun Quan', '吴国',700, 120, 20, 5, 10, 8, None, 'F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/Sun Quan')
    persona4 = Persona('Zhang Fei', '蜀国',700, 120, 20, 5, 10, 8, None,
                       'F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/ZhangFei')
    persona5 = Persona('Guan Yu', '蜀国',700, 120, 20, 5, 10, 8, None,
                       'F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/Guan Yu')

    persona1.scratch.character = 'Ambitious and Ruthless'
    persona2.scratch.character = 'Benevolent and Righteous'
    persona3.scratch.character = 'Youthful and Energetic'


    personas = [persona1, persona2, persona3, persona4, persona5]

    for persona in personas:
        persona.save('F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/' + persona.name + '/bootstrap_memory')

    with open('F:\\CS project/2024 fall innovative project\\AI三国/backend_server/background', 'r', encoding='utf-8') as file:
        background = file.read()

    prompt_message = [background]

    for persona in personas:
        with open('F:\\CS project/2024 fall innovative project\\AI三国/frontend_server/storage/test/personas/' + persona.name + '/bootstrap_memory/scratch.json', 'r', encoding='utf-8') as f:
            persona_data = json.load(f)
            prompt_message.append(json.dumps(persona_data, indent=4, ensure_ascii=False))

        prompt_message.append(prompt_generate.create_persona_info_prompt(persona))


    prompt_message.append('You need to response the first question with format Subject Verb Object. For example, I ask you about your favorite food, you can response with \'I like pizza\'. No other format is allowed.')

    threads = []

    for i, persona in enumerate(personas):
        thread = threading.Thread(target=chat_with_persona, args=(persona, personas, chat, prompt_message.copy(), i))
        threads.append(thread)
        thread.start()

    for thread in threads:
        thread.join()

if __name__ == "__main__":
    main()