using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ThreeDFont
{
    [Serializable]
    public partial class FontData
    {
        [Header("--------------------------------")] [Header("Fontの読み込み情報")]
        public string Exported3DFontBaseVersion;

        [Header("--------------------------------")] 
        [Header("Fontの名前")]public string FontName = String.Empty;
        [Header("Font作成者の名前")]public string FontAuthorName;

        [Header("Fontのバージョン")] public string FontVersion;
        [Header("Fontの一意なハッシュ")] public string FontHash;

        [Header("FontのRawデータ")] public List<CharacterData> CharacterData = new List<CharacterData>();
        [Header("Font録画時のFPS")] public int FontFPS = 30;
        [Header("筆画の間の情報を保持しているか")] public bool IsBetweenStrokePosition = false;

        public FontData()
        {
        }

        public static FontData Load(string path)
        {
            if (!File.Exists(path)) return null;

            try
            {
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<FontData>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void Save(FontData fontData, string path)
        {
            if (String.IsNullOrEmpty(path)) return;

            // Fontのハッシュを作成
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fontData.FontName + fontData.FontAuthorName));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                fontData.FontHash = builder.ToString();
            }
                 
            var json = JsonUtility.ToJson(fontData, false);
            try
            {
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}