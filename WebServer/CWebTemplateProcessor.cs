using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebServer
{
    class CWebTemplateProcessor : IScriptProcessor
    {

        private CscriptProcessor _scriptProcessor;

        /* Constructor
         */
        public CWebTemplateProcessor()
        {
            _scriptProcessor = new CscriptProcessor();
        }

        public WebResult ProcessScript(string path, IDictionary<string, string> requestParameters)
        {
            StringBuilder finalContent = new StringBuilder();

            StringBuilder scriptBody = new StringBuilder();

            /* read the contents of the file into a string 
             * builder line by line to create a single 
             * string that represents the script */
            using (FileStream fs = File.OpenRead(path))
            {
                StreamReader reader = new StreamReader(fs);
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    scriptBody.Append(line);
                }
            }

            // Need to replace @{ xx } with { wout.WriteLine(xx); }
            string content = scriptBody.ToString();
            int start = 0;
            
            while (content.Contains("@{"))
            {
                int i = content.IndexOf("@{");
                int j = content.IndexOf("}", i);

                i = i + 2;

                string literal = content.Substring(i, (j - i));
                content = content.Replace("@{" + literal + "}", "{ wout.WriteLine(" + literal + ");}");
            }

            while(content.Contains("{"))
            {
                int i = content.IndexOf("{");

                finalContent.Append("wout.WriteLine(\"" + content.Substring(start, (i - start)) + "\");");

                int j = content.IndexOf("}");

                i = i + 1;

                finalContent.Append(content.Substring(i, (j - i)));

                j = j + 1;

                content = content.Remove(start, j);
            }

            finalContent.Append("wout.WriteLine(\"" + content + "\");");

            Console.WriteLine(finalContent.ToString());

            return _scriptProcessor.ProcessScriptString(finalContent.ToString(), requestParameters);
        }

    }
}
