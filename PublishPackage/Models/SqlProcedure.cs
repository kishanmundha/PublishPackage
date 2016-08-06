using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage.Models
{
    public class SqlProcedure : IDataCompare
    {
        public string ProcedureName { get; set; }
        public string ProcedureDefination { get; set; }
        public string KeyName
        {
            get { return this.ProcedureName; }
        }

        public string MD5Hash
        {
            get { return Helper.GetMD5Hash(this.ProcedureDefination); }
        }

        private string replaceCreateWord(string newWord)
        {
            var regx = new System.Text.RegularExpressions.Regex("^(create)([ ]+procedure)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var strLines = this.ProcedureDefination.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i=0; i<strLines.Length; i++)
            {
                var line = strLines[i].Trim();
                var m = regx.Match(line.Trim());
                //regx.Replace()
                if(m.Success)
                {
                    strLines[i] = regx.Replace(line.Trim(), "ALTER" + m.Groups[2]);
                }
            }

            return string.Join("\r\n", strLines);
        }

        public string GetScript()
        {
            return this.ProcedureDefination;
        }

        public string GetCreateScript()
        {
            string script = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
";
            return script + this.GetScript() + "GO\r\n";
        }

        public string GetAlterScript()
        {
            string script = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
";
            return script + this.replaceCreateWord("ALTER") + "GO\r\n";
        }

        public string GetDropScript()
        {
            return string.Format("DROP PROCEDURE [{0}]", this.ProcedureName);
        }

        public object GetJsonObject()
        {
            return new
            {
                this.ProcedureName,
                this.ProcedureDefination
            };
        }
    }
}
