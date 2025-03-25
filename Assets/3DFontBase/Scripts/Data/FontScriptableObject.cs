using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThreeDFont
{
    [CreateAssetMenu(fileName = "Font", menuName = "Font")]
    public class FontScriptableObject : ScriptableObject
    {
        public List<CharacterData> CharacterData;

        public static FontScriptableObject Load()
        {
            //TODO : ファイル名を指定してのロードに対応させる
            var guid = AssetDatabase.FindAssets("t:" + nameof(FontScriptableObject)).FirstOrDefault();
            var filePath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(filePath))
            {
                throw new System.IO.FileNotFoundException("FontScriptableObject does not found");
            }

            var data = AssetDatabase.LoadAssetAtPath<FontScriptableObject>(filePath);

            return data;
        }

        public static void Save(UnityEngine.Object asset )
        {
            //TODO : ファイル名を指定しての保存に対応させる
            var guid = AssetDatabase.FindAssets("t:" + nameof(FontScriptableObject)).FirstOrDefault();
            var filePath = AssetDatabase.GUIDToAssetPath(guid);
            //アセットが存在しない場合はそのまま作成(metaファイルも新規作成)
            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(asset, filePath);
                return;
            }

            //仮ファイルを作るためのディレクトリを作成
            var fileName = Path.GetFileName(filePath);
            var tmpDirectoryPath = Path.Combine(filePath.Replace(fileName, ""), "tmpDirectory");
            Directory.CreateDirectory(tmpDirectoryPath);

            //仮ファイルを保存
            var tmpFilePath = Path.Combine(tmpDirectoryPath, fileName);
            AssetDatabase.CreateAsset(asset, tmpFilePath);

            //仮ファイルを既存のファイルに上書き(metaデータはそのまま)
            FileUtil.ReplaceFile(tmpFilePath, filePath);

            //仮ディレクトリとファイルを削除
            AssetDatabase.DeleteAsset(tmpDirectoryPath);

            //データ変更をUnityに伝えるためインポートしなおし
            AssetDatabase.ImportAsset(filePath);
        }
    }
}