using RestaurantManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return AccountDAO.instance;
            }
            private set
            {
                instance = value;
            }
        }

        private AccountDAO()
        { }

        public bool Signin(string userName, string passWord)
        {
            string passEncode = MD5Hash(Base64Encode(passWord));
            //byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            //byte[] hashData = new MD5CryptoServiceProvider().ComputeHash(temp);
            //string hasPass = "";

            //foreach(byte item in hashData)
            //{
            //    hasPass += item;
            //}
            string query = "usp_Login @userName , @passWord";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] {userName, passEncode });

            return result.Rows.Count > 0;
        }

        public Account GetAccountByUserName(string userName)
        {
           DataTable data = DataProvider.Instance.ExecuteQuery("select * from Account where userName = '" + userName + "'");

           foreach(DataRow item in data.Rows)
           {
                return new Account(item);
           }
            return null;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select UserName, DisplayName, Type from dbo.Account");
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            string passEncode = MD5Hash(Base64Encode(pass));
            string newpassEncode = MD5Hash(Base64Encode(newPass));

            int result = DataProvider.Instance.ExecuteNonQuery("exec usp_UpdateAccount @userName, @displayName, @password,@newPassword", new object[] {userName,displayName,passEncode,newpassEncode });
            return result > 0;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public bool InsertAccount(string userName, string displayName, int type)
        {
            string query = string.Format("insert dbo.Account (UserName, DisplayName, Type, password) values (N'{0}',N'{1}',{2},N'{3}')", userName, displayName, type, "80432911e07b111f6a05fd7c904c1bc9");

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateAccount(string userName, string displayName, int type)
        {
            string query = string.Format("update dbo.Account set DisplayName = N'{1}', type = {2} where UserName = N'{0}'", userName, displayName, type);

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteAccount(string userName)
        {
            string query = string.Format("Delete Account where UserName = N'{0}'", userName);

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool ResetPassWord(string userName)
        {
            string query = string.Format("update Account set password = N'80432911e07b111f6a05fd7c904c1bc9' where UserName = N'{0}'", userName);

            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }
}
