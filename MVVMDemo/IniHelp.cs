using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MVVMDemo
{

    public enum Base
    {
        /// <summary>二进制</summary>
        BASE_BINARY = 2,
        /// <summary>8进制</summary>
        BASE_OCTAL = 8,
        /// <summary>10进制</summary>
        BASE_DECIMAL = 10, // 0x0000000A
                           /// <summary>16进制</summary>
        BASE_HEX = 16, // 0x00000010
    }

    /// <summary>INI文件读写类</summary>
    public class IniHelper
  {
    private Encoding m_encoding = Encoding.UTF8;
    private string m_strIniFile;
    private const int DEF_PROFILE_THRESHOLD = 512;

    /// <summary>获取INI文件中的键值</summary>
    /// <param name="lpAppName">ini节名</param>
    /// <param name="lpKeyName">ini键名</param>
    /// <param name="lpDefault">默认值：当无对应键值，则返回该值。</param>
    /// <param name="lpReturnedString">结果缓冲区</param>
    /// <param name="nSize">结果缓冲区大小</param>
    /// <param name="lpFileName">ini文件位置</param>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileString(byte[] lpAppName, byte[] lpKeyName, byte[] lpDefault, byte[] lpReturnedString, int nSize, string lpFileName);

    /// <summary>写入INI文件键值</summary>
    /// <param name="lpAppName">ini节名</param>
    /// <param name="lpKeyName">ini键名</param>
    /// <param name="lpString">写入值</param>
    /// <param name="lpFileName">ini文件位置</param>
    /// <returns>0：写入失败 1：写入成功</returns>
    [DllImport("kernel32.dll")]
    private static extern int WritePrivateProfileString(byte[] lpAppName, byte[] lpKeyName, byte[] lpString, string lpFileName);

    /// <summary>获取INI文件中所有节点的名称</summary>
    /// <param name="lpszReturnBuffer">结果缓冲区</param>
    /// <param name="nSize">缓冲区大小</param>
    /// <param name="lpFileName">INI文件路径</param>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

    /// <summary>获取INI文件中节点对应的所有关键字和值</summary>
    /// <param name="lpAppName"></param>
    /// <param name="lpReturnedString"></param>
    /// <param name="nSize"></param>
    /// <param name="lpFileName"></param>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileSection(byte[] lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);

    /// <summary>替换INI文件中的节点名称</summary>
    /// <param name="lpAppName">旧节点名称</param>
    /// <param name="lpString">新的节点名称</param>
    /// <param name="lpFileName">INI文件</param>
    /// <returns></returns>
    [DllImport("kernel32.dll")]
    private static extern int WritePrivateProfileSection(byte[] lpAppName, byte[] lpString, string lpFileName);

    /// <summary>编码格式</summary>
    public Encoding Encoding
    {
      get
      {
        return this.m_encoding;
      }
      set
      {
        this.m_encoding = value;
      }
    }

    /// <summary>构造函数</summary>
    public IniHelper()
    {
    }

    /// <summary>构造函数</summary>
    /// <param name="strIniFile"></param>
    public IniHelper(string strIniFile)
    {
      this.m_strIniFile = strIniFile;
    }

    /// <summary>构造函数</summary>
    /// <param name="strIniFile"></param>
    /// <param name="encode"></param>
    public IniHelper(string strIniFile, Encoding encode)
    {
      this.m_strIniFile = strIniFile;
      this.m_encoding = encode;
    }

    /// <summary>INI文件</summary>
    public string IniFile
    {
      get
      {
        return this.m_strIniFile;
      }
      set
      {
        this.m_strIniFile = value;
      }
    }

    private byte[] GetBytes(string s)
    {
      return s == null ? (byte[]) null : this.m_encoding.GetBytes(s);
    }

    /// <summary>获取指定节点的键值</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="strDefault">默认值</param>
    /// <returns>键值</returns>
    public string GetString(string strSection, string strKey, string strDefault = "")
    {
      if (string.IsNullOrEmpty(strSection) || string.IsNullOrEmpty(strKey))
      {
        if (string.IsNullOrEmpty(strDefault))
          strDefault = "";
        return strDefault;
      }
      int nSize = 0;
      byte[] numArray;
      int privateProfileString;
      do
      {
        nSize += 512;
        numArray = new byte[nSize + 1];
        privateProfileString = IniHelper.GetPrivateProfileString(this.GetBytes(strSection), this.GetBytes(strKey), this.GetBytes(strDefault), numArray, nSize, this.m_strIniFile);
      }
      while (privateProfileString + 1 >= nSize);
      return this.m_encoding.GetString(numArray, 0, privateProfileString);
    }

    /// <summary>写入指定节点键值</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="strValue">值</param>
    /// <returns></returns>
    public bool WriteString(string strSection, string strKey, string strValue)
    {
      if (string.IsNullOrEmpty(strSection) || string.IsNullOrEmpty(strKey))
        return false;
      return IniHelper.WritePrivateProfileString(this.GetBytes(strSection), this.GetBytes(strKey), this.GetBytes(strValue), this.m_strIniFile) == 1;
    }

    /// <summary>往指定节点和关键字追加字符串</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    public bool AppendString(string strSection, string strKey, string strValue)
    {
      string strValue1 = this.GetString(strSection, strKey, "") + strValue;
      return this.WriteString(strSection, strKey, strValue1);
    }

    /// <summary>获取指定节点和关键字的数值数组</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="separator">分隔符</param>
    /// <param name="bTrim">是否Trim</param>
    /// <returns></returns>
    public string[] GetArray(string strSection, string strKey, char separator = ',', bool bTrim = true)
    {
      string[] strArray = this.GetString(strSection, strKey, "").Split(separator);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (bTrim)
          strArray[index] = strArray[index].Trim();
      }
      return strArray;
    }

    /// <summary>获取指定节点和关键字的数值数组</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="separator">分隔符</param>
    /// <returns></returns>
    public int[] GetIntArray(string strSection, string strKey, char separator = ',')
    {
      string str = this.GetString(strSection, strKey, "");
      try
      {
        string[] strArray = str.Split(new char[1]
        {
          separator
        }, StringSplitOptions.RemoveEmptyEntries);
        int[] numArray = new int[strArray.Length];
        for (int index = 0; index < strArray.Length; ++index)
          numArray[index] = Convert.ToInt32(strArray[index].Trim());
        return numArray;
      }
      catch (Exception ex)
      {
        return new int[0];
      }
    }

    /// <summary>获取指定节点和关键字的数值数组</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="separator">分隔符</param>
    /// <returns></returns>
    public double[] GetDoubleArray(string strSection, string strKey, char separator = ',')
    {
      string str = this.GetString(strSection, strKey, "");
      try
      {
        string[] strArray = str.Split(new char[1]
        {
          separator
        }, StringSplitOptions.RemoveEmptyEntries);
        double[] numArray = new double[strArray.Length];
        for (int index = 0; index < strArray.Length; ++index)
          numArray[index] = Convert.ToDouble(strArray[index].Trim());
        return numArray;
      }
      catch (Exception ex)
      {
        return new double[0];
      }
    }

    /// <summary>写指定节点和关键字的数组</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="strArray">字符串数组</param>
    /// <param name="nWriteCount">写入数组的长度</param>
    /// <param name="separator">分隔符</param>
    /// <returns></returns>
    public bool WriteArray(string strSection, string strKey, string[] strArray, int nWriteCount = -1, char separator = ',')
    {
      nWriteCount = nWriteCount >= 0 ? Math.Min(nWriteCount, strArray.Length) : strArray.Length;
      StringBuilder stringBuilder = new StringBuilder("");
      for (int index = 0; index < nWriteCount; ++index)
      {
        stringBuilder.Append(strArray[index]);
        if (index != nWriteCount - 1)
          stringBuilder.Append(separator);
      }
      return this.WriteString(strSection, strKey, stringBuilder.ToString());
    }

    /// <summary>写指定节点和关键字的数组</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="array">数组</param>
    /// <param name="nWriteCount">写入数组的长度</param>
    /// <param name="separator">分隔符</param>
    /// <returns></returns>
    public bool WriteArray(string strSection, string strKey, int[] array, int nWriteCount = -1, char separator = ',')
    {
      nWriteCount = nWriteCount >= 0 ? Math.Min(nWriteCount, array.Length) : array.Length;
      StringBuilder stringBuilder = new StringBuilder("");
      for (int index = 0; index < nWriteCount; ++index)
      {
        stringBuilder.Append(array[index]);
        if (index != nWriteCount - 1)
          stringBuilder.Append(separator);
      }
      return this.WriteString(strSection, strKey, stringBuilder.ToString());
    }

    /// <summary>写指定节点和关键字的数组</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="array">数组</param>
    /// <param name="nWriteCount">写入数组的长度</param>
    /// <param name="separator">分隔符</param>
    /// <returns></returns>
    public bool WriteArray(string strSection, string strKey, double[] array, int nWriteCount = -1, char separator = ',')
    {
      nWriteCount = nWriteCount >= 0 ? Math.Min(nWriteCount, array.Length) : array.Length;
      StringBuilder stringBuilder = new StringBuilder("");
      for (int index = 0; index < nWriteCount; ++index)
      {
        stringBuilder.Append(array[index].ToString("F3"));
        if (index != nWriteCount - 1)
          stringBuilder.Append(separator);
      }
      return this.WriteString(strSection, strKey, stringBuilder.ToString());
    }

    /// <summary>获取指定节点和关键字的数值</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="nDefault">默认值</param>
    /// <param name="fromBase">进制</param>
    /// <returns></returns>
    public int GetInt(string strSection, string strKey, int nDefault, Base fromBase = Base.BASE_DECIMAL)
    {
      string str = this.GetString(strSection, strKey, "");
      try
      {
        return Convert.ToInt32(str, (int) fromBase);
      }
      catch
      {
        return nDefault;
      }
    }

    /// <summary>写入指定节点关键字的数值</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="nValue"></param>
    /// <param name="toBase"></param>
    /// <returns></returns>
    public bool WriteInt(string strSection, string strKey, int nValue, Base toBase = Base.BASE_DECIMAL)
    {
      string strValue = Convert.ToString(nValue, (int) toBase);
      return this.WriteString(strSection, strKey, strValue);
    }

    /// <summary>获取指定节点和关键字的浮点型数值</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="dbDefault"></param>
    /// <returns></returns>
    public double GetDouble(string strSection, string strKey, double dbDefault)
    {
      string str = this.GetString(strSection, strKey, "");
      try
      {
        return Convert.ToDouble(str);
      }
      catch
      {
        return dbDefault;
      }
    }

    /// <summary>写入指定节点关键字的浮点数数值</summary>
    /// <param name="strSection">节点</param>
    /// <param name="strKey">关键字</param>
    /// <param name="dbValue">数值</param>
    /// <param name="nPrecision">精度</param>
    /// <returns></returns>
    public bool WriteDouble(string strSection, string strKey, double dbValue, int nPrecision = -1)
    {
      string strValue = dbValue.ToString();
      if (nPrecision > 0)
      {
        string format = string.Format("f{0}", (object) nPrecision);
        strValue = dbValue.ToString(format);
      }
      return this.WriteString(strSection, strKey, strValue);
    }

    /// <summary>获取指定节点和关键字的bool型数值</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="bDefault"></param>
    /// <returns></returns>
    public bool GetBool(string strSection, string strKey, bool bDefault)
    {
      string str = this.GetString(strSection, strKey, "");
      try
      {
        return Convert.ToBoolean(str);
      }
      catch
      {
        return bDefault;
      }
    }

    /// <summary>写入指定节点关键字的bool型数值</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <param name="bValue"></param>
    /// <returns></returns>
    public bool WriteBool(string strSection, string strKey, bool bValue)
    {
      string strValue = bValue.ToString();
      return this.WriteString(strSection, strKey, strValue);
    }

    /// <summary>判断节点是否存在</summary>
    /// <param name="strSection">节点</param>
    /// <returns></returns>
    public bool IsSectionExist(string strSection)
    {
      return Array.IndexOf<string>(this.GetSectionNames(), strSection) >= 0;
    }

    /// <summary>获取所有的节点</summary>
    /// <returns></returns>
    public string[] GetSectionNames()
    {
      int nSize = 0;
      byte[] numArray;
      int profileSectionNames;
      do
      {
        nSize += 512;
        numArray = new byte[nSize + 1];
        profileSectionNames = IniHelper.GetPrivateProfileSectionNames(numArray, nSize, this.m_strIniFile);
      }
      while (profileSectionNames + 2 >= nSize);
      return Encoding.Default.GetString(numArray, 0, profileSectionNames).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>删除节点</summary>
    /// <param name="strSection">节点</param>
    /// <returns></returns>
    public bool DeleteSection(string strSection)
    {
      return IniHelper.WritePrivateProfileString(this.GetBytes(strSection), (byte[]) null, this.GetBytes(""), this.m_strIniFile) == 1;
    }

    /// <summary>指定键是否存在</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public bool IsKeyExist(string strSection, string strKey)
    {
      return Array.IndexOf<string>(this.GetKeyNames(strSection), strKey) >= 0;
    }

    /// <summary>获取指定节点的所有键</summary>
    /// <param name="strSection"></param>
    /// <returns></returns>
    public string[] GetKeyNames(string strSection)
    {
      int nSize = 0;
      byte[] numArray;
      int privateProfileSection;
      do
      {
        nSize += 512;
        numArray = new byte[nSize + 1];
        privateProfileSection = IniHelper.GetPrivateProfileSection(this.GetBytes(strSection), numArray, nSize, this.m_strIniFile);
      }
      while (privateProfileSection + 2 >= nSize);
      string[] strArray1 = this.m_encoding.GetString(numArray, 0, privateProfileSection).ToString().Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
      List<string> stringList = new List<string>();
      foreach (string str in strArray1)
      {
        char[] separator = new char[1]{ '=' };
        int num = 0;
        string[] strArray2 = str.Split(separator, (StringSplitOptions) num);
        if (strArray2.Length == 2 && strArray2[0].Trim().Length > 0)
          stringList.Add(strArray2[0].Trim());
      }
      return stringList.ToArray();
    }

    /// <summary>删除键</summary>
    /// <param name="strSection"></param>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public bool DeleteKey(string strSection, string strKey)
    {
      return IniHelper.WritePrivateProfileString(this.GetBytes(strSection), this.GetBytes(strKey), (byte[]) null, this.m_strIniFile) == 1;
    }

    /// <summary>删除节点下的所有键</summary>
    /// <param name="strSection"></param>
    /// <returns></returns>
    public bool DeleteKeys(string strSection)
    {
      foreach (string keyName in this.GetKeyNames(strSection))
      {
        if (!this.DeleteKey(strSection, keyName))
          return false;
      }
      return true;
    }
  }
}
