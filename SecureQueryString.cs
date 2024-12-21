using System.Security.Cryptography;
using System.Text;
using System.Web;
using System;
using System.Collections.Specialized;

namespace AeS.ADFSSAML_MAX
{
    public class SecureQueryString : NameValueCollection
    {
        private const string timeStampKey = "__TimeStamp__";

        private const string cryptoKey = "balaganeshTheGreat!";

        private SecureQueryString.Level _Scope;

        private readonly byte[] IV = new byte[] { 240, 3, 45, 29, 0, 76, 173, 59 };

        public string EncryptedString
        {
            get
            {
                return HttpUtility.UrlEncode(this.encrypt(this.serialize()));
            }
        }

        public SecureQueryString.Level Scope
        {
            get
            {
                return this._Scope;
            }
            set
            {
                this._Scope = value;
            }
        }

        public SecureQueryString()
        {
        }

        public SecureQueryString(string encryptedString)
        {
            this.deserialize(this.decrypt(encryptedString));
        }

        private string decrypt(string encryptedQueryString)
        {
            string str;
            try
            {
                byte[] numArray = Convert.FromBase64String(encryptedQueryString);
                TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
                tripleDESCryptoServiceProvider.Key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes("balaganeshTheGreat!"));
                tripleDESCryptoServiceProvider.IV = this.IV;
                str = Encoding.ASCII.GetString(tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(numArray, 0, (int)numArray.Length));
            }
            catch (CryptographicException cryptographicException)
            {
                throw new Exception("Invalid QueryString");
            }
            catch (FormatException formatException)
            {
                throw new Exception("Invalid QueryString");
            }
            return str;
        }

        private void deserialize(string decryptedQueryString)
        {
            string str = "";
            string str1 = "";
            string str2 = "";
            string str3 = "";
            string[] strArrays = decryptedQueryString.Split(new char[] { '&' });
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str4 = strArrays[i];
                string[] strArrays1 = str4.Split(new char[] { '=' });
                if ((int)strArrays1.Length == 2)
                {
                    if (strArrays1[0] == "PageQuery_StaffID")
                    {
                        str = strArrays1[1];
                    }
                    else if (strArrays1[0] == "PageQuery_Scope")
                    {
                        str1 = strArrays1[1];
                    }
                    else if (strArrays1[0] == "PageQuery_ScopeValue")
                    {
                        str2 = strArrays1[1];
                    }
                    else if (strArrays1[0] != "PageQuery_FromConfig")
                    {
                        this.Add(strArrays1[0], strArrays1[1]);
                    }
                    else
                    {
                        str3 = strArrays1[1];
                    }
                }
            }
            //if (HttpContext.Current == null)
            //{
            //    return;
            //}
            //AeS_SessionInfo item = (AeS_SessionInfo)HttpContext.Current.Session["AeSSessionInfo"];
            //if (str == item.get_LoginStaffID())
            //{
            //    return;
            //}
            //string str5 = " ";
            //if (HttpContext.Current.Request.UrlReferrer != null)
            //{
            //    str5 = HttpContext.Current.Request.UrlReferrer.ToString();
            //}
            //string str6 = HttpContext.Current.Request.Url.ToString();
            //if (Convert.ToInt32(str1) == Convert.ToInt32(SecureQueryString.Level.UserLevel))
            //{
            //    if (str3 == "N")
            //    {
            //        CHelperFunctions.RecordEvents(item, "Querystring Mismatch occured.For more information refer the table - CORE_ERROR_AUDIT_LOG table.", string.Empty, string.Empty, 1);
            //        ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat("Querystring Mismatch from :", str5, " to ", str6)));
            //        this.Clear();
            //        return;
            //    }
            //    ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat(new string[] { "Querystring Mismatch from :", str5, " to ", str6, " --Recommended add scope in SecureQuerystring object for above mentioned URL" })));
            //    this.Clear();
            //    return;
            //}
            //if (Convert.ToInt32(str1) == Convert.ToInt32(SecureQueryString.Level.CompanyLevel))
            //{
            //    if (str3 == "N")
            //    {
            //        if (str2 == item.get_CompanyID())
            //        {
            //            return;
            //        }
            //        CHelperFunctions.RecordEvents(item, "Querystring Mismatch occured.For more information refer the table - CORE_ERROR_AUDIT_LOG table.", string.Empty, string.Empty, 1);
            //        ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat("Querystring Mismatch from :", str5, " to ", str6)));
            //        this.Clear();
            //        return;
            //    }
            //    if (str2 == item.get_CompanyID())
            //    {
            //        return;
            //    }
            //    ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat(new string[] { "Querystring Mismatch from :", str5, " to ", str6, " --Recommended add scope in SecureQuerystring object for above mentioned URL" })));
            //    this.Clear();
            //    return;
            //}
            //if (Convert.ToInt32(str1) != Convert.ToInt32(SecureQueryString.Level.CompanyOULevel))
            //{
            //    return;
            //}
            //if (str3 == "N")
            //{
            //    if (str2 == item.get_CompanyOUID())
            //    {
            //        return;
            //    }
            //    CHelperFunctions.RecordEvents(item, "Querystring Mismatch occured.For more information refer the table - CORE_ERROR_AUDIT_LOG table.", string.Empty, string.Empty, 1);
            //    ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat("Querystring Mismatch from :", str5, " to ", str6)));
            //    this.Clear();
            //    return;
            //}
            //if (str2 == item.get_CompanyOUID())
            //{
            //    return;
            //}
            //ApplicationAudit.Log(item, 3, "", str6, new Exception(string.Concat(new string[] { "Querystring Mismatch from :", str5, " to ", str6, " --Recommended add scope in SecureQuerystring object for above mentioned URL." })));
            //this.Clear();
        }

        private string encrypt(string serializedQueryString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(serializedQueryString);
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            tripleDESCryptoServiceProvider.Key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes("balaganeshTheGreat!"));
            tripleDESCryptoServiceProvider.IV = this.IV;
            return Convert.ToBase64String(tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, (int)bytes.Length));
        }

        private string serialize()
        {
            int i;
            StringBuilder stringBuilder = new StringBuilder();
            string[] allKeys = this.AllKeys;
            for (i = 0; i < (int)allKeys.Length; i++)
            {
                string str = allKeys[i];
                stringBuilder.Append(str);
                stringBuilder.Append('=');
                stringBuilder.Append(base[str]);
                stringBuilder.Append('&');
            }
            //if (HttpContext.Current != null)
            //{
            //    string empty = string.Empty;
            //    AeS_SessionInfo item = (AeS_SessionInfo)HttpContext.Current.Session["AeSSessionInfo"];
            //    stringBuilder.Append(string.Concat("PageQuery_StaffID=", item.get_LoginStaffID()));
            //    if (Convert.ToInt32(this._Scope) != 0)
            //    {
            //        i = Convert.ToInt32(this._Scope);
            //        stringBuilder.Append(string.Concat("&PageQuery_Scope=", i.ToString()));
            //        stringBuilder.Append("&PageQuery_FromConfig=N");
            //    }
            //    else
            //    {
            //        string configValue = (new CCustom()).GetConfigValue(item, "CORE", "VALIDATE_SECURE_STRING");
            //        stringBuilder.Append(string.Concat("&PageQuery_Scope=", configValue));
            //        this._Scope = (SecureQueryString.Level)Enum.Parse(typeof(SecureQueryString.Level), configValue);
            //        stringBuilder.Append("&PageQuery_FromConfig=Y");
            //    }
            //    if (this._Scope == SecureQueryString.Level.CompanyOULevel)
            //    {
            //        empty = item.get_CompanyOUID();
            //    }
            //    else if (this._Scope == SecureQueryString.Level.CompanyLevel)
            //    {
            //        empty = item.get_CompanyID();
            //    }
               // stringBuilder.Append(string.Concat("&PageQuery_ScopeValue=", empty));
            //}
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            return this.EncryptedString;
        }

        public enum Level
        {
            UserLevel = 1,
            CompanyOULevel = 2,
            CompanyLevel = 3
        }

    }
}
