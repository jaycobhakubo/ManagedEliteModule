using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace GameTech.Elite.Base
{
    public class EnhancedRegularExpression
    {
        //This class extends the regex class to support inline regular expression functions and concatination of 
        //collected groups.  
        //
        //Inline regular expression functions are implemented by naming a group with the function's name followed by
        //a number and defining a function as follows: (??<function name>regular expression to be used)
        //When using a regular expression function, be sure to name all collection groups to maintain order.
        //Ex: ^(?<AN1>.{10}).{3}(?<AN2>.{6})(??<AN>([0-9a-zA-Z]+))
        // Our function is AN and is the regular expression ([0-9a-zA-Z]+) to capture all the alphanumeric characters
        // We take the first 10 characters, skip 3, and take the next 6.  After the expression is resolved, the
        // AN1 group will be processed with the regular expression ([0-9a-zA-Z]+) and then the AN2 group will be.
        // For 12ud432@n4v2;98-01=9 the concatenated result would be 12ud432n49801 

        //literal text can be inserted using a function who's definition is (???<function name>text). use \) for a ) in the string.
        //Ex: ^(?<TEXT1>).{4}(?<NUM>.{9})(???<TEXT>(L0))   for the string 0123987654321ww  would return L0987654321

        //If the expression comment starts with "Ignore" any match will be returned as "Ignore".

        private class RegExFunction
        {
            public string name;
            public string regExp;
            public bool literalText = false;
        }

        private class ProcessedData
        {
            public string regExp = null;
            public List<RegExFunction> functionList = null;
        }

        private List<RegExFunction> regExpFunctions = new List<RegExFunction>();

        public EnhancedRegularExpression() 
        {
        }

        public static string Match(string regExp, string inputString)
        {
            StringBuilder sb = new StringBuilder();
            bool ignoreOnMatch = regExp.ToLower().Contains("(?#ignore");

            try
            {
                //extract the function definitions from the expression
                ProcessedData pd = ExtractFunctions(regExp);


                Regex re = new Regex(pd.regExp);
                string[] groupNames = re.GetGroupNames();

                //process the expression
                MatchCollection collections = Regex.Matches(inputString, pd.regExp);

                //apply function expressions to matching groups

                //concatinate all the results
                foreach (Match match in collections)
                {
                    GroupCollection gc = match.Groups;

                    for (int x = 1; x < gc.Count; x++)
                    {
                        string groupName = groupNames[x];

                        //see if the group name refers to a defined function
                        RegExFunction functionRegEx = null;

                        if (pd.functionList != null)
                        {
                            foreach (RegExFunction fncInfo in pd.functionList)
                            {
                                if (Regex.IsMatch(groupName, fncInfo.name + @"\d+")) //we have a match, save the function 
                                {
                                    functionRegEx = fncInfo;
                                    break;
                                }
                            }
                        }

                        if (functionRegEx != null) //we have a function, apply the function expression and use the result
                        {
                            if (functionRegEx.literalText)
                                sb.Append(functionRegEx.regExp);
                            else
                                sb.Append(Match(functionRegEx.regExp, gc[x].Value));
                        }
                        else //no function used
                        {
                            sb.Append(gc[x].Value);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                sb.Length = 0;
            }

            if (ignoreOnMatch && sb.Length != 0)
            {
                sb.Clear();
                sb.Append("Ignore");
            }

            return sb.ToString();
        }

        private static ProcessedData ExtractFunctions(string regExp)
        {
            ProcessedData pd = new ProcessedData();

            //look for literal text functions first
            while (regExp.Contains("(???<")) //while we have a function
            {
                int nameEnd = -1, depth = 0, start = regExp.IndexOf("(???<"), end = start;

                //find the end of the function
                do //look for ) matching the starting (
                {
                    if (nameEnd == -1 && regExp[end] == '>')
                        nameEnd = end;

                    if (regExp[end] == '(')
                        depth++;

                    if (regExp[end] == ')' && regExp[end-1] != '\\')
                        depth--;

                    end++;
                } while (!(regExp[end - 1] == ')' && depth == 0));

                if (pd.functionList == null)
                    pd.functionList = new List<RegExFunction>();

                RegExFunction rxf = new RegExFunction();

                rxf.name = regExp.Substring(start + 5, nameEnd - (start + 5));
                rxf.regExp = regExp.Substring(nameEnd + 2, (end - 2) - (nameEnd + 2));
                rxf.literalText = true;

                pd.functionList.Add(rxf);

                regExp = regExp.Substring(0, start) + regExp.Substring(end); //remove the function definition
            }

            while(regExp.Contains("(??<")) //while we have a function
            {
                int nameEnd = -1, depth = 0, start = regExp.IndexOf("(??<"), end = start; 

                //find the end of the function
                do //look for ) matching the starting (
                {
                    if (nameEnd == -1 && regExp[end] == '>')
                        nameEnd = end;

                    if (regExp[end] == '(')
                        depth++;

                    if (regExp[end] == ')')
                        depth--;

                    end++;
                } while(!(regExp[end-1] == ')' && depth == 0));

                if (pd.functionList == null)
                    pd.functionList = new List<RegExFunction>();

                RegExFunction rxf = new RegExFunction();

                rxf.name = regExp.Substring(start + 4, nameEnd - (start + 4));
                rxf.regExp = regExp.Substring(nameEnd+1, (end - 1) - (nameEnd+1));

                pd.functionList.Add(rxf);

                regExp = regExp.Substring(0, start) + regExp.Substring(end); //remove the function definition
            }

            pd.regExp = regExp; //the regex with the function definitions removed

            return pd;
        }
    }
}
