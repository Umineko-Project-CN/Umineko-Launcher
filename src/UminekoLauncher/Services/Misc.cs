using System.IO;
using System.Security.Cryptography;
using UminekoLauncher.Localization;
using UminekoLauncher.Models;

namespace UminekoLauncher.Services;

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
            using HashAlgorithm hashAlgorithm = algorithm switch
            {
                "MD5" => MD5.Create(),
                "SHA1" => SHA1.Create(),
                "SHA256" => SHA256.Create(),
                "SHA384" => SHA384.Create(),
                "SHA512" => SHA512.Create(),
                _ => throw new Exception(Lang.Error_Checksum),
            };
            using FileStream stream = File.OpenRead(filePath);
            byte[] hash = hashAlgorithm.ComputeHash(stream);
            return new Checksum() { HashAlgorithm = algorithm, Value = Convert.ToHexString(hash) };
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
            using var reader = new StreamReader(FilePath);
            string? str;
            do
            {
                str = reader.ReadLine();
            } while (str != null && !str.StartsWith("\"resver\""));
            string? versionStr = str?.Split('=')[1].Trim('\"');
            return versionStr != null ? new Version(versionStr) : new Version(0, 0, 0, 0);
        }
        catch
        {
            return new Version(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// 检测繁中语言包资源是否存在。
    /// </summary>
    /// <returns>表示检测结果的 <see cref="bool"/> 值。</returns>
    public static bool LangCHTResourceExist()
    {
        return File.Exists("cht.cfg");
    }
}
