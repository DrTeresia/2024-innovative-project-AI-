# Please install OpenAI SDK first: `pip3 install openai`

from code import interact
from distutils.sysconfig import PREFIX
from openai import OpenAI

api_key = "sk-8d0818dd5b7841699423c41c0808b6c4"
client = OpenAI(api_key="sk-8d0818dd5b7841699423c41c0808b6c4", base_url="https://api.deepseek.com")

"""
response = client.chat.completions.create(
    model="deepseek-chat",
    messages=[
        {"role": "system", "content": "You are a helpful assistant"},
        {"role": "user", "content": "Hello"},
    ],
    stream=False
)
"""

prompt_summary = "Summary the following sentence."
prompt_SVO_format = "You need to response with format Subject Verb Object."

class ChatWithOpenAI:


    background = ''
    list_of_messages = [
        {"role": "system", "content": background}, 
    ]

    def __init__(self, api_key):
        self.client = OpenAI(api_key=api_key, base_url="https://api.deepseek.com")

    def change_background(self, background):
        self.background = background

    def chat_with_user(self, message): #return the latest sentence
        self.list_of_messages.append({"role": "user", "content": message})

        response = self.client.chat.completions.create(
            model="deepseek-chat",
            messages=self.list_of_messages,
            stream=False
        )

        self.list_of_messages.append({"role": "system", "content": response.choices[0].message.content})

        return response.choices[0].message.content
    
    def get_chat(self):
        return self.list_of_messages
    
    def flush_chat(self):
        self.list_of_messages = [
            {"role": "system", "content": self.backgroud}
        ]
    
    def summary(self):
        messages = [{"role": "system", "content": prompt_summary}]
        messages.append({"role": "system", "content": prompt_SVO_format})
        for message in self.list_of_messages:
            messages.append(message)

def main():
    chat = ChatWithOpenAI(api_key)
    while True:
        message = input("You: ")
        print("AI: ", chat.chat_with_user(message))
    
if __name__ == "__main__":
    main()