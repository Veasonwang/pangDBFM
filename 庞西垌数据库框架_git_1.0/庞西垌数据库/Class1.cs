using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//保存文件对象的实现
namespace 庞西垌数据库
{
    [Serializable]
    public  class Connection_set
    {
        string user;
        string psd;
        string inital;
        string s_d;
        string t_d;
        string w_d;
       public Connection_set()
        {
            
        }
        public string get_user()
        {
            return pds(user,0);
        }
        public string get_psd()
        {
            return pds(psd,0);
        }
        public string get_inital()
        {
            return inital;
        }
        public string get_s_d()
        {
            return s_d;
        }
        public string get_t_d()
        {
            return t_d;
        }
        public string get_w_d()
        {
            return w_d;
        }
        public Connection_set readfile()                                                                                    //读取文件函数，文件名为“eky.dll”
        {
            FileStream fs = new FileStream("eky.dll", FileMode.Open, FileAccess.Read); //使用第6个构造函数  
            BinaryFormatter bf = new BinaryFormatter();  //创建一个序列化和反序列化类的对象                                 //创建序列与反序列类的对象来获取数据
            Connection_set c_s = (Connection_set)bf.Deserialize(fs);  //调用反序列化方法，从文件中读取对象信息  
            fs.Close();   //关闭文件流  
            return c_s;
        }
        public  Connection_set( string _inital, string _user,string _psd,string _s_d,string _t_d,string _w_d)
        {
            user = pds(_user,1);
            psd = pds( _psd,1);
            inital =_inital;
            s_d = _s_d;
            t_d = _t_d;
            w_d = _w_d;

        }
        public string pds(string tx,int j)                                                                              //设置密码字典对其进行加密
        {
            char[] aft = tx.ToCharArray();
            char temp;
            int sign = 0;
          
            char[,] tb= new char[2, 36];
            tb[0, 0] = 'g';
            tb[0, 1] = 'a';
            tb[0, 2] = 'd';
            tb[0, 3] = 's';
            tb[0, 4] = 'f';
            tb[0, 5] = 'h';
            tb[0, 6] = 'j';
            tb[0, 7] = 'k';
            tb[0, 8] = 'l';
            tb[0, 9] = 'w';
            tb[0, 10] = 'q';
            tb[0, 11] = 'e';
            tb[0, 12] = 't';
            tb[0, 13] = 'r';
            tb[0, 14] = 'y';
            tb[0, 15] = 'u';
            tb[0, 16] = 'i';
            tb[0, 17] = 'o';
            tb[0, 18] = '9';
            tb[0, 19] = 'p';
            tb[0, 20] = 'z';
            tb[0, 21] = 'x';
            tb[0, 22] = 'c';
            tb[0, 23] = 'b';
            tb[0, 24] = 'v';
            tb[0, 25] = 'n';
            tb[0, 26] = 'm';
            tb[0, 27] = '0';
            tb[0, 28] = '2';
            tb[0, 29] = '3';
            tb[0, 30] = '1';
            tb[0, 31] = '4';
            tb[0, 32] = '5';
            tb[0, 33] = '6';
            tb[0, 34] = '7';
            tb[0, 35] = '8';
            if (j == 1)//加密
            {
                for (int i = 0; i < aft.Length; i++)
                {
                    temp = aft[i];
                    if (temp >= 97 && temp <= 122)
                    {
                        sign = temp - 'a' + 10;
                    }
                    else if (temp >= 48 && temp <= 57)
                    {
                        sign = temp - '0';
                    }
                    aft[i] = tb[0, sign];
                }
                string str = new string(aft);
                return str;
            }
            if(j==0)            //解密
            {
                for(int i = 0; i < aft.Length; i++)
                {
                    temp = aft[i];
                    for(int k=0;k<36;k++)
                    {
                        if(temp==tb[0,k])
                        {
                            if(k>9)
                            {
                                aft[i] = (char)('a' + k-10);
                            }
                            else
                            {
                                aft[i] = (char)('0' + k);
                            }
                        }
                    }
                }
                string str = new string(aft);
                return str;
            }
            return tx;

        }
       public void savefile()                                                                               
        {
          //  FileStream fs = new FileStream("key", FileMode.Open, FileAccess.Read); //使用第6个构造函数  
            FileStream fs = new FileStream("eky.dll", FileMode.Create, FileAccess.Write);  //创建一个文件流对象  
            BinaryFormatter bf = new BinaryFormatter();  //创建一个序列化和反序列化对象  
            bf.Serialize(fs,this);   //要先将类先设为可以序列化(即在类的前面加[Serializable])。将用户集合信息写入到硬盘中  
            fs.Close();
        }
    }

    //DES加密算法，正在研究如何使用，暂未应用。目前使用字典替换加密算法
    public class MyEncrypt
        {
            private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            private static string encryptKey = "dfgtrwq";
            /// <summary>  
                    /// DES加密字符串  
                    /// </summary>  
                    /// <param name="encryptString">待加密的字符串</param>  
                    /// <param name="encryptKey">加密密钥,要求为8位</param>  
                    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>  
            public static string EncryptDES(string encryptString)
            {
                try
                {
                    byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                    byte[] rgbIV = Keys;
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                    DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                    MemoryStream mStream = new MemoryStream();
                    CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                    return Convert.ToBase64String(mStream.ToArray());
                }
                catch
                {
                    return encryptString;
                }
            }
            /// <summary>  
                    /// DES解密字符串  
                    /// </summary>  
                    /// <param name="decryptString">待解密的字符串</param>  
                    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>  
                    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>  
            public static string DecryptDES(string decryptString)
            {
                try
                {
                    byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
                    byte[] rgbIV = Keys;
                    byte[] inputByteArray = Convert.FromBase64String(decryptString);
                    DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                    MemoryStream mStream = new MemoryStream();
                    CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
                catch
                {
                    return decryptString;
                }
            }
        }
    //权限控制模块，包含权限控制参数
    public class power
    {
        public bool sd;
        public bool wd;
        public bool td;
        public bool serch;
        public bool delete;
        public bool save;
        public bool set;
        public bool pw;
        public string id;
        public power()
        {
            sd = true;
            wd = true;
            td = true;
            serch = true;
            delete = true;
            save = true;
            set = true;
            pw = true;
        }
    }

}
