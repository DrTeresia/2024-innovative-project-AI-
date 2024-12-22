using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChatWithOpenAI {
    public string background;
    public string prompt_input;
    public ChatWithOpenAI(string background){
        this.background = background;
    }

    public void setPrompt(string input){
        this.prompt_input = input;
    }

    public async Task<string> choice()
    {
        try {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer sk-8d0818dd5b7841699423c41c0808b6c4");

            string requestBody = $@"{{
                ""messages"": [
                    {{
                        ""content"": ""You are a helpful assistant"",
                        ""role"": ""system""
                    }},
                    {{
                        ""content"": ""{background} {prompt_input}"",
                        ""role"": ""user""
                    }}
                ],
                ""model"": ""deepseek-chat"",
                ""frequency_penalty"": 0,
                ""max_tokens"": 2048,
                ""presence_penalty"": 0,
                ""response_format"": {{
                    ""type"": ""text""
                }},
                ""stop"": null,
                ""stream"": false,
                ""temperature"": 1,
                ""top_p"": 1,
                ""tool_choice"": ""none"",
                ""logprobs"": false,
                ""top_logprobs"": null
            }}";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            Debug.Log("Request body size: " + content.Headers.ContentLength);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            // 解析 JSON 响应
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseString);
            if (responseData != null && responseData.choices.Length > 0) {
                string messageContent = responseData.choices[0].message.content;
                //Debug.Log("Message Content: " + messageContent);
                string filePath = Path.Combine(".\\", "choices.txt");
                File.WriteAllText(filePath, messageContent);
                //Debug.Log("Message content has been written to file: " + filePath);
                return messageContent;
            }
            return null;
        } catch (HttpRequestException e) {
            Debug.LogError("HttpRequestException: " + e.Message + " - " + e.InnerException?.Message);
            return null;
        } catch (Exception e) {
            Debug.LogError("Exception: " + e.Message);
            return null;
        }
    }
    public async Task<string> chat()
    {
    try {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer sk-8d0818dd5b7841699423c41c0808b6c4");

            string requestBody = $@"{{
                ""messages"": [
                    {{
                        ""content"": ""You are a helpful assistant"",
                        ""role"": ""system""
                    }},
                    {{
                        ""content"": ""{background} {prompt_input}"",
                        ""role"": ""user""
                    }}
                ],
                ""model"": ""deepseek-chat"",
                ""frequency_penalty"": 0,
                ""max_tokens"": 2048,
                ""presence_penalty"": 0,
                ""response_format"": {{
                    ""type"": ""text""
                }},
                ""stop"": null,
                ""stream"": false,
                ""temperature"": 1,
                ""top_p"": 1,
                ""tool_choice"": ""none"",
                ""logprobs"": false,
                ""top_logprobs"": null
            }}";

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            Debug.Log("Request body size: " + content.Headers.ContentLength);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            Debug.Log("Response: " + responseString);

            // 解析 JSON 响应
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseString);
            if (responseData != null && responseData.choices.Length > 0) {
                string messageContent = responseData.choices[0].message.content;
                Debug.Log("Message Content: " + messageContent);
                string filePath = Path.Combine(Application.persistentDataPath, "dialogue.txt");
                File.WriteAllText(filePath, messageContent);
                Debug.Log("Message content has been written to file: " + filePath);
            }
            return responseString;
        } catch (HttpRequestException e) {
            Debug.LogError("HttpRequestException: " + e.Message + " - " + e.InnerException?.Message);
            return null;
        } catch (Exception e) {
            Debug.LogError("Exception: " + e.Message);
            return null;
        }
    }
}

// JSON 数据结构
[Serializable]
public class ResponseData
{
    public string id;
    public string objectName;
    public int created;
    public string model;
    public Choice[] choices;
    public Usage usage;
    public string systemFingerprint;
}

[Serializable]
public class Choice
{
    public int index;
    public Message message;
    public object logprobs;
    public string finish_reason;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class Usage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
    public int prompt_cache_hit_tokens;
    public int prompt_cache_miss_tokens;
}
