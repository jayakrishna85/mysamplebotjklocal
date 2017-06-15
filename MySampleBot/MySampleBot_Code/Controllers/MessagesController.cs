using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using MySampleBot.Models;

namespace MySampleBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {        
        private string[] separators;
        string InvalidDataMessage;

        public MessagesController()
        {            
            separators = new string[] { " ", ":", ";", "," };
            InvalidDataMessage = "Invalid data. Please enter valid data.";
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                string message = string.Empty;
                                
                if (ShowFirstTimeMessage(activity.Text))
                {
                    message = GetWelcomeMessage(activity.Text);
                }
                else
                {
                    message = ProcessMessage(activity.Text);
                }

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // return our reply to the user
                Activity reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private string GetWelcomeMessage(string text)
        {
            StringBuilder message = new StringBuilder();
            if (text.Trim().ToLower() != "help")
            {
                message.AppendLine("Welcome to My Sample Bot");
                message.AppendLine("___");
            }
            message.AppendLine("Your have below options to use calculator:");
            message.AppendLine("___");
            message.AppendLine();
            message.AppendLine("Add");
            message.AppendLine("");
            message.AppendLine("Subtract");
            message.AppendLine("");
            message.AppendLine("Multiply");
            message.AppendLine("");
            message.AppendLine("Divide");
            message.AppendLine("");

            message.AppendLine("___");
            message.AppendLine("Ex:");
            message.AppendLine("--");
            message.AppendLine("___");
            message.AppendLine();
            message.AppendLine("Multiply 5 6");
            message.AppendLine();
            message.AppendLine("Multiply 5*6");
            message.AppendLine();
            message.AppendLine("Calculate 5*6");
            message.AppendLine();
            message.AppendLine("5*6");
            message.AppendLine();

            return message.ToString();
        }

        private bool ShowFirstTimeMessage(string text)
        {
            text = text.Trim().ToLower();
            bool showWelcomeMessage = false;

            switch(text)
            {
                case "hi":
                case "hi!":
                case "hello":
                case "welcome":
                case "help":
                    showWelcomeMessage = true;
                    break;
            }

            return showWelcomeMessage;
        }
                
        private async void SendMessage(Activity activity, string message)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            
            // return our reply to the user
            Activity reply = activity.CreateReply(message);
            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private string ProcessMessage(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string message = string.Empty;

            var words = GetWords(text);

            if (words.Length > 4 || words.Length <= 2)
                return InvalidDataMessage;

            int outputParam = 0;
            int? firstItem = null;
            int? secondItem = null;
            CalculatorFunc func = CalculatorFunc.Default;

            if (words.Length >= 3)
            {
                if (int.TryParse(words[0], out outputParam))
                    firstItem = outputParam;
                else if (func == CalculatorFunc.Default)
                    CheckForCalculatorFun(words[0], out func);

                if (int.TryParse(words[1], out outputParam))
                {
                    if (!firstItem.HasValue)
                        firstItem = outputParam;
                    else
                        secondItem = outputParam;
                }
                else if (func == CalculatorFunc.Default)
                    CheckForCalculatorFun(words[1], out func);

                if (int.TryParse(words[2], out outputParam))
                {
                    if (!firstItem.HasValue)
                        firstItem = outputParam;
                    else if(!secondItem.HasValue)
                        secondItem = outputParam;
                    else
                        return InvalidDataMessage;
                }
                else if (func == CalculatorFunc.Default)
                    CheckForCalculatorFun(words[2], out func);

                if (words.Length == 4)
                {
                    if (int.TryParse(words[3], out outputParam))
                    {
                        if (!firstItem.HasValue)
                            firstItem = outputParam;
                        else if (!secondItem.HasValue)
                            secondItem = outputParam;
                        else
                            return InvalidDataMessage;
                    }
                    else if (func == CalculatorFunc.Default)
                        CheckForCalculatorFun(words[3], out func);
                }
            }
            else
                return InvalidDataMessage;

            if(firstItem.HasValue && secondItem.HasValue && func != CalculatorFunc.Default)
            {
                if (func == CalculatorFunc.Divide)
                {
                    if (secondItem == 0)
                        return string.Format("{0} cannot be divided by 0", firstItem.Value);
                }

                double result = 0;

                switch(func)
                {
                    case CalculatorFunc.Add:
                        result = firstItem.Value + secondItem.Value;
                        break;
                    case CalculatorFunc.Subtract:
                        result = firstItem.Value - secondItem.Value;
                        break;
                    case CalculatorFunc.Multiply:
                        result = firstItem.Value * secondItem.Value;
                        break;
                    case CalculatorFunc.Divide:
                        result = firstItem.Value / secondItem.Value;
                        break;
                }

                return "Output : " + result;
            }

            return InvalidDataMessage;
        }

        private void CheckForCalculatorFun(string value, out CalculatorFunc func)
        {
            CalculatorFunc parsedCalculatorFunc = ParseCalculatorFunc(value);

            if (parsedCalculatorFunc != CalculatorFunc.Default)
                func = parsedCalculatorFunc;
            else
                func = CalculatorFunc.Default;
        }

        private CalculatorFunc ParseCalculatorFunc(string value)
        {
            value.Trim().ToLower();

            CalculatorFunc calculatorFunc = CalculatorFunc.Default;

            switch (value)
            {
                case "add":
                case "+":
                    calculatorFunc = CalculatorFunc.Add;
                    break;
                case "subtract":
                case "-":
                    calculatorFunc = CalculatorFunc.Subtract;
                    break;
                case "multiply":
                case "*":
                case "x":
                    calculatorFunc = CalculatorFunc.Multiply;
                    break;
                case "divide":
                case "/":
                    calculatorFunc = CalculatorFunc.Divide;
                    break;
                default:
                    calculatorFunc = CalculatorFunc.Default;
                    break;
            }

            return calculatorFunc;
        }

        private string[] GetWords(string text)
        {
            List<string> resultWords = new List<string>();
            string[] parsedWords = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            string[]  funcSeparators = new string[] { "+", "-", "*", "x", "/"};

            foreach (string word in parsedWords)
            {
                string[] innerParsedWord = word.Split(funcSeparators, StringSplitOptions.None);

                if (word.Length == 1 || innerParsedWord.Length == 1)
                {
                    resultWords.Add(word);
                }
                else
                {
                    if(word.Contains("+"))
                    {
                        resultWords.Add("+");
                        resultWords.AddRange(word.Split(new string[] { "+" }, StringSplitOptions.None));
                    }
                    else if (word.Contains("-"))
                    {
                        resultWords.Add("-");
                        resultWords.AddRange(word.Split(new string[] { "-" }, StringSplitOptions.None));
                    }
                    else if (word.Contains("*"))
                    {
                        resultWords.Add("*");
                        resultWords.AddRange(word.Split(new string[] { "*" }, StringSplitOptions.None));
                    }
                    else if (word.Contains("x"))
                    {
                        resultWords.Add("x");
                        resultWords.AddRange(word.Split(new string[] { "x" }, StringSplitOptions.None));
                    }
                    else if (word.Contains("X"))
                    {
                        resultWords.Add("X");
                        resultWords.AddRange(word.Split(new string[] { "X" }, StringSplitOptions.None));
                    }
                    else if (word.Contains("/"))
                    {
                        resultWords.Add("/");
                        resultWords.AddRange(word.Split(new string[] { "/" }, StringSplitOptions.None));
                    }
                }
            }

            return resultWords.ToArray();;
        }
    }
}