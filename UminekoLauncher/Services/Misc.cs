using System;
using System.IO;
using System.Security.Cryptography;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services
{
    /// <summary>
    /// 杂项类。
    /// </summary>
    internal static class Misc
    {
        /// <summary>
        /// 获取某文件的校验值。
        /// </summary>
        /// <param name="filePath">需校验文件所在路径。</param>
        /// <param name="algorithm">校验算法。</param>
        /// <returns>以大写形式返回校验值。</returns>
        public static Checksum GetHash(string filePath, string algorithm = "MD5")
        {
            try
            {
                using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(algorithm))
                {
                    if (hashAlgorithm == null)
                    {
                        throw new Exception("不支持该校验算法。");
                    }
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        byte[] hash = hashAlgorithm.ComputeHash(stream);
                        return new Checksum()
                        {
                            HashAlgorithm = algorithm,
                            Value = BitConverter.ToString(hash).Replace("-", string.Empty)
                        };
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // 若文件不存在，返回空值。
                return new Checksum();
            }
        }

        /// <summary>
        /// 获取游戏资源版本号。
        /// </summary>
        /// <returns>版本号。</returns>
        public static Version GetResourceVersion()
        {
            const string FilePath = "locale_game.hash";
            try
            {
                using (var reader = new StreamReader(FilePath))
                {
                    string str;
                    do
                    {
                        str = reader.ReadLine();
                    } while (!str.StartsWith("\"resver\""));
                    return new Version(str.Split('=')[1].Trim('\"'));
                }
            }
            catch
            {
                return new Version(0, 0, 0, 0);
            }
        }
    }
}