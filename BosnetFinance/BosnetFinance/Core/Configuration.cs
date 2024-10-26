using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BosnetFinance.Core
{
    public class Configuration
    {
        public static string ConnectionString
        {
            get
            {
                return Cryptography.Decrypt(ConfigurationManager.AppSettings["ConnectionString"], KeyString, IVString);
            }
        }

        public static string KeyString
        {
            get { return ConfigurationManager.AppSettings["KeyString"]; }
        }

        public static string IVString
        {
            get { return ConfigurationManager.AppSettings["IVString"]; }
        }
    }
}