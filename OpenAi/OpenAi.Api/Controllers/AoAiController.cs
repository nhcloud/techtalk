using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;

namespace OpenAi.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AoAiController : ControllerBase
    {
        private readonly OpenAIClient _client = new OpenAIClient(new Uri(AppSettings.AoAiEndpoint), new AzureKeyCredential(AppSettings.AoAiKey));

        [HttpGet]
        [Route("Completion")]
        public async Task<string> GetCompletion()
        {
            string prompt = "What is Azure OpenAI?";
            Response<Completions> completionsResponse = await _client.GetCompletionsAsync(AppSettings.AoAiDeploymentName, prompt);
            string completion = completionsResponse.Value.Choices[0].Text;
            return completion;
        }

        [HttpGet]
        [Route("ChatCompletion")]
        public async Task<string> GetChatCompletion()
        {
            // Build completion options object
            ChatCompletionsOptions chatCompletionsOptions = new()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful AI bot."),
                    new ChatMessage(ChatRole.User, "What is Azure OpenAI?")
                }
            };
            chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a casual, helpful assistant. You will talk like an American old western film character."),
                    new ChatMessage(ChatRole.User, "Can you direct me to the library?")
                }
            };
            chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatRole.User, "That was an awesome experience"),
                    new ChatMessage(ChatRole.Assistant, "positive"),
                    new ChatMessage(ChatRole.User, "I won't do that again"),
                    new ChatMessage(ChatRole.Assistant, "negative"),
                    new ChatMessage(ChatRole.User, "That was not worth my time"),
                    new ChatMessage(ChatRole.Assistant, "negative"),
                    new ChatMessage(ChatRole.User, "You can't miss this")
                }
            };
            // Send request to Azure OpenAI model
            ChatCompletions chatCompletionsResponse = await _client.GetChatCompletionsAsync(
                AppSettings.AoAiDeploymentName, chatCompletionsOptions);

            ChatMessage completion = chatCompletionsResponse.Choices[0].Message;
            Console.WriteLine($"Chatbot: {completion.Content}");
            return completion.Content;
        }
        /// <summary>
        /// 1: Basic prompt (no prompt engineering)
        /// 2: Prompt with email formatting and basic system message
        /// 3: Prompt with formatting and specifying content
        /// 4: Prompt adjusting system message to be light and use jokes
        /// </summary>
        [HttpGet]
        [Route("Prompt")]
        public async Task<string> GetPromptRespons(string command)
        {
            switch (command)
            {
                case "1":
                    return await GetResponseFromOpenAI("prompts/basic.txt");

                case "2":
                    return await GetResponseFromOpenAI("prompts/email-format.txt");

                case "3":
                    return await GetResponseFromOpenAI("prompts/specify-content.txt");

                case "4":
                    return await GetResponseFromOpenAI("prompts/specify-tone.txt");

                default:
                    return "Invalid input. Please try again.";
            }
        }
        async Task<string> GetResponseFromOpenAI(string fileText)
        {
            // Read text file into system and user prompts
            string[] prompts = System.IO.File.ReadAllLines(fileText);
            string systemPrompt = prompts[0].Split(":", 2)[1].Trim();
            string userPrompt = prompts[1].Split(":", 2)[1].Trim();

            // Write prompts to console
            Console.WriteLine("System prompt: " + systemPrompt);
            Console.WriteLine("User prompt: " + userPrompt);

            // Create chat completion options
            // Create chat completion options
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                 {
                     new ChatMessage(ChatRole.System, systemPrompt),
                     new ChatMessage(ChatRole.User, userPrompt)
                 },
                Temperature = 0.7f,
                MaxTokens = 800,
            };

            // Get response from Azure OpenAI
            Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(
                AppSettings.AoAiDeploymentName, chatCompletionsOptions
            );

            ChatCompletions completions = response.Value;
            string completion = completions.Choices[0].Message.Content;

            return completion;
        }
    }
}
