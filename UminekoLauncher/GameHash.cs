using System;
using System.IO;
using System.Security.Cryptography;

namespace UminekoLauncher
{
    /// <summary>
    /// 该静态类用于执行游戏文件校验及 locale_game.hash 文件的相关操作。
    /// </summary>
    static class GameHash
    {
        /// <summary>
        /// 获取游戏校验值文件（locale_game.hash）所在路径。
        /// </summary>
        public static string HashPath { get; } = "locale_game.hash";

        /// <summary>
        /// 获取或设置游戏资源版本。
        /// </summary>
        public static Version ResourceVersion { get; set; }

        /// <summary>
        /// 载入游戏校验值文件（locale_game.hash）。
        /// </summary>
        public static void LoadHashFile()
        {
            ResourceVersion = GetVersion();
        }

        /// <summary>
        /// 获取某文件的校验值。
        /// </summary>
        /// <param name="filePath">需校验文件所在路径。</param>
        /// <param name="checksum">校验算法。</param>
        /// <returns>以大写形式返回校验值。</returns>
        public static Checksum GetHash(string filePath, Checksum checksum)
        {
            try
            {
                using (var hashAlgorithm = HashAlgorithm.Create(checksum.HashingAlgorithm ?? "MD5"))
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        if (hashAlgorithm != null)
                        {
                            var hash = hashAlgorithm.ComputeHash(stream);
                            return new Checksum()
                            {
                                Value = BitConverter.ToString(hash).Replace("-", string.Empty),
                                HashingAlgorithm = checksum.HashingAlgorithm
                            };
                        }
                        throw new Exception("不支持该校验算法。");
                    }
                }

            }
            catch (Exception)
            {
                return new Checksum() { Value = "0", HashingAlgorithm = "0" };
            }
        }
        /// <summary>
        /// 获取游戏资源版本号。
        /// </summary>
        /// <returns>版本号。</returns>
        private static Version GetVersion()
        {
            try
            {
                using (var reader = new StreamReader(HashPath))
                {
                    string str;
                    do
                    {
                        str = reader.ReadLine();
                    } while (!str.StartsWith("\"resver\""));
                    return new Version(str.Split('=')[1].Trim('\"'));
                }

            }
            catch (Exception)
            {
                return new Version(0, 0, 0, 0);
            }
        }
    }
}
