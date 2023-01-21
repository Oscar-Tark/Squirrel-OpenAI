using System;
using OpenAI_API;
using System.Security;
using SecureStringExtensions;
using ScorpionConsoleReadWrite;
using SquirrelInheritable;

namespace SquirrelOpenAI
{
    public class TOpenAIUser : SquirrelDisposable
    {
        public TOpenAIUser()
        {
            this.auth = new SecureString();
            this.api_auth = new APIAuthentication(String.Empty);
        }

        public void getKeyFromConfig(string config_path)
        {
            //Unfortunately only supports Unsafe string. But still implementing with Secure String in case of future modification
            if(auth.Length > 0)
                auth.Clear();

            ConsoleWrite.writeOutput("Reading OPENAI key from config @ ", config_path);
            byte[] b_key = File.ReadAllBytes(config_path);

            for(int i = 0; i < b_key.Length; i++)
                auth.AppendChar((char)b_key[i]);

            api_auth = new APIAuthentication(SecureStringExtensions.SecureStringExtensions.FromSecureStringToUnsafeString(auth));
            return;
        }

        public APIAuthentication getAuth()
        {
            return api_auth;
        }

        private SecureString auth;
        private APIAuthentication api_auth;
    }

    public class SquirrelOpenAIController : SquirrelDisposable
    {
        private TOpenAIUser user;
        OpenAIAPI api;

        public SquirrelOpenAIController(string config_path)
        {
            user = new TOpenAIUser();
            user.getKeyFromConfig(config_path);

            api = new OpenAI_API.OpenAIAPI();
            api.Auth = user.getAuth();
            return;
        }

        public async Task<object> openAIQuery(string query)
        {
            object result = await api.Completions.CreateCompletionAsync("One Two Three One Two", temperature: 0.1);
            return result;
        }

        ~SquirrelOpenAIController()
        {
            user.Dispose();
            this.Dispose();
            return;
        }
    }
}
