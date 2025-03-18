using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using SimpleSaver.Intern;
using UnityEditor;
using UnityEngine.UIElements;

public class SimpleSave
{
    public enum DataPath { persistent, local, streaming }
    public enum JSONMode { none, prettyprint, encrypted }

    private static DataPath dataPath;

    /// <summary>
    /// Sets the base datapath.
    /// </summary>
    /// <param name="dataPath">The base datapath.</param>
    public static void SetDataPath(DataPath dataPath)
    {
        SimpleSave.dataPath = dataPath;
    }

    //Used to get the datapath (Internal function)
    private static string GetDataPath()
    {
        switch (dataPath)
        {
            case DataPath.persistent:
                return Application.persistentDataPath;
            default:
            case DataPath.local:
                return Application.dataPath;
            case DataPath.streaming:
                return Application.streamingAssetsPath;
        }
    }

    //Used to get the set the path (and check for Extensions) (Internal function)
    private static string ValidatePath(string path)
    {
        string newPath = Path.Combine(GetDataPath(), path);

        if (!Path.HasExtension(newPath)) newPath += ".simplesave";

        return newPath;
    }

    #region SaveBinary

    /// <summary>
    /// Save a file as a binary file (Not all values are supported)
    /// </summary>
    /// <typeparam name="T">The data type you want to save.</typeparam>
    /// <param name="data">The data you want to save.</param>
    /// <param name="path">The path you want your data to be saved (Use SetDataPath to set the main path).</param>
    public static void SaveBinary<T>(T data, string path)
    {
        string savepath = ValidatePath(path);

        try
        {
            using (FileStream fs = new FileStream(savepath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                WriteObject(writer, data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save file {path}: {e}");
        }
    }

    private static void WriteObject<T>(BinaryWriter writer, T obj)
    {
        FieldInfo[] fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            object value = field.GetValue(obj);
            if (value is int) writer.Write((int)value);
            else if (value is float) writer.Write((float)value);
            else if (value is bool) writer.Write((bool)value);
            else if (value is string) writer.Write((string)value);
            else if (value is Enum) writer.Write(Convert.ToInt32(value));
            else Debug.LogError($"Unsupported type: {field.FieldType}");
        }
    }

    /// <summary>
    /// Loads the binary file and returns the data.
    /// </summary>
    /// <typeparam name="T">The data type you want to load.</typeparam>
    /// <param name="path">The path you want your data to be loaded from (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the given data type.</returns>
    public static T LoadBinary<T>(string path) where T : new()
    {
        string loadPath = ValidatePath(path);

        if (!File.Exists(loadPath))
        {
            Debug.LogWarning($"Save file not found: {loadPath}");
            return new T();
        }

        try
        {
            using (FileStream fs = new FileStream(loadPath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                return ReadObject<T>(reader);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load file {path}: {e}");
            return new T();
        }
    }

    private static T ReadObject<T>(BinaryReader reader) where T : new()
    {
        T obj = new T();
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(int)) field.SetValue(obj, reader.ReadInt32());
            else if (field.FieldType == typeof(float)) field.SetValue(obj, reader.ReadSingle());
            else if (field.FieldType == typeof(bool)) field.SetValue(obj, reader.ReadBoolean());
            else if (field.FieldType == typeof(string)) field.SetValue(obj, reader.ReadString());
            else if (field.FieldType.IsEnum) field.SetValue(obj, Enum.ToObject(field.FieldType, reader.ReadInt32()));
            else Debug.LogError($"Unsupported type: {field.FieldType}");
        }
        return obj;
    }
    #endregion

    #region SaveJson
    /// <summary>
    /// Saves a serializable class with json
    /// </summary>
    /// <typeparam name="T">The data type you want to save.</typeparam>
    /// <param name="data">The data you want to save.</param>
    /// <param name="path">The path you want your data to be saved (Use SetDataPath to set the main path).</param>
    /// <param name="jsonMode">Sets the JSON save mode. Prettyprint for prettyprint and encrypted for encrypted files.</param>
    public static void SaveJson<T>(T data, string path, JSONMode jsonMode = JSONMode.none)
    {
        string savePath = ValidatePath(path);

        string saveData = JsonUtility.ToJson(data, jsonMode == JSONMode.prettyprint);

        if (jsonMode == JSONMode.encrypted) saveData = EncryptorDecryptor.EncryptDecrypt(saveData);

        File.WriteAllText(savePath, saveData);
    }

    /// <summary>
    /// Loads the json file and returns the data.
    /// </summary>
    /// <typeparam name="T">The data type you want to load.</typeparam>
    /// <param name="path">The path you want your data to be loaded from (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the given data type.</returns>
    public static T LoadJson<T>(string path, bool isEncrypted = false)
    {
        string loadPath = ValidatePath(path);

        if (File.Exists(loadPath))
        {
            string allFileText = File.ReadAllText(loadPath);

            if(isEncrypted) allFileText = EncryptorDecryptor.EncryptDecrypt(allFileText);

            T data = JsonUtility.FromJson<T>(allFileText);
            return data;
        }
        return default;
    }
    #endregion

    #region SaveXML
    /// <summary>
    /// Saves a serializable class with XML
    /// </summary>
    /// <typeparam name="T">The data type you want to save.</typeparam>
    /// <param name="data">The data you want to save.</param>
    /// <param name="path">The path you want your data to be saved (Use SetDataPath to set the main path).</param>
    public static void SaveXML<T>(T data, string path)
    {
        string savePath = ValidatePath(path);

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(savePath, FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// Loads a XML file and returns the data.
    /// </summary>
    /// <typeparam name="T">The data type you want to load.</typeparam>
    /// <param name="path">The path you want your data to be loaded from (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the given data type.</returns>
    public static T LoadXML<T>(string path)
    {
        string loadPath = ValidatePath(path);

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(loadPath, FileMode.Open);
        T loadedData = (T)serializer.Deserialize(stream);
        stream.Close();

        return loadedData;
    }

    #endregion

    #region SavePNG
    /// <summary>
    /// Save a texture2d as a png
    /// </summary>
    /// <param name="texture2D">Texture2D to save.</param>
    /// <param name="path">The path you want your png to be saved (Use SetDataPath to set the main path).</param>
    public static void SavePNG(Texture2D texture2D, string path)
    {
        string savePath = Path.Combine(GetDataPath(), path);

        if (!Path.HasExtension(savePath) || Path.GetExtension(savePath) != ".png") savePath += ".png";

        byte[] png = texture2D.EncodeToPNG();
        File.WriteAllBytes(savePath, png);
    }

    /// <summary>
    /// Loads a png to a Texture2D
    /// </summary>
    /// <param name="path">The path you want your png to be loaded from (Use SetDataPath to set the main path)</param>
    /// <param name="textureWidth">Texture Width</param>
    /// <param name="textureHeight">Texture Height</param>
    /// <param name="textureFormat">Texture Format</param>
    /// <returns>Returns Texture2d.</returns>
    public static Texture2D LoadPNG(string path, int textureWidth, int textureHeight, TextureFormat textureFormat = TextureFormat.RGBA32, bool mipChain = false, bool linear = false)
    {
        string loadPath = Path.Combine(GetDataPath(), path);

        if (!Path.HasExtension(loadPath) || Path.GetExtension(loadPath) != ".png") loadPath += ".png";

        if (File.Exists(loadPath))
        {
            byte[] png = File.ReadAllBytes(loadPath);
            Texture2D texture2D = new Texture2D(textureWidth, textureHeight, textureFormat, mipChain, linear);
            texture2D.LoadImage(png);
            texture2D.Apply();
            return texture2D;
        }
        return null;
    }

    #endregion

    #region SaveVar

    [Serializable] //Save class for var save data.
    private class VariableSave
    {
        public SerializableDictionary<string, string> stringDictionary;
        public SerializableDictionary<string, int> intDictionary;
        public SerializableDictionary<string, float> floatDictionary;
        public SerializableDictionary<string, bool> boolDictionary;
    }

    //Creates the variableSave class
    private static VariableSave CreateDictionary()
    {
        VariableSave save = new VariableSave();
        save.stringDictionary = new SerializableDictionary<string, string>();
        save.intDictionary = new SerializableDictionary<string, int>();
        save.floatDictionary = new SerializableDictionary<string, float>();
        save.boolDictionary = new SerializableDictionary<string, bool>();
        return save;
    }

    /// <summary>
    /// Save single variables to a savefile.
    /// </summary>
    /// <typeparam name="TValue">The variable to save.</typeparam>
    /// <param name="key">The key for the variable.</param>
    /// <param name="value">The variable data.</param>
    /// <param name="path">The path you want your data to be saved (Use SetDataPath to set the main path).</param>
    /// <param name="encrypt">If you want to encrypt the data.</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool SaveValue<TValue>(string key, TValue value, string path, bool encrypt = false)
    {
        string savePath = ValidatePath(path);

        VariableSave saveData;
        if (FileExist(path))
        {
            string allFileText = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<VariableSave>(allFileText);
        }
        else saveData = CreateDictionary();


        if (value.GetType() == typeof(string))
        {
            string stringValue = value.ToString();
            if (saveData.stringDictionary.ContainsKey(key)) saveData.stringDictionary[key] = stringValue;
            else saveData.stringDictionary.Add(key, stringValue);
        }
        else if (value.GetType() == typeof(int))
        {
            int intValue = int.Parse(value.ToString());
            if (saveData.intDictionary.ContainsKey(key)) saveData.intDictionary[key] = intValue;
            else saveData.intDictionary.Add(key, intValue);
        }
        else if (value.GetType() == typeof(float))
        {
            float floatValue = float.Parse(value.ToString());
            if (saveData.floatDictionary.ContainsKey(key)) saveData.floatDictionary[key] = floatValue;
            else saveData.floatDictionary.Add(key, floatValue);
        }
        else if (value.GetType() == typeof(bool))
        {
            bool boolValue = bool.Parse(value.ToString());
            if (saveData.boolDictionary.ContainsKey(key)) saveData.boolDictionary[key] = boolValue;
            else saveData.boolDictionary.Add(key, boolValue);
        }
        else
        {
            Debug.LogError("Type not supported. Supported variables: string, int, float & bool.");
            return false;
        }

        string json = JsonUtility.ToJson(saveData, true);
        if (encrypt) json = EncryptorDecryptor.EncryptDecrypt(json);
        File.WriteAllText(savePath, json);
        return true;
    }

    public enum varType { stringType, intType, floatType, boolType }

    /// <summary>
    /// Loads a single variable from a savefile.
    /// </summary>
    /// <typeparam name="TValue">The value type you want to load</typeparam>
    /// <param name="key">The key of the stored variable</param>
    /// <param name="variableType">The variable type</param>
    /// <param name="path">The path you want your data to be loaded from (Use SetDataPath to set the main path)</param>
    /// <param name="value">The return value</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool LoadValue<TValue>(string key, varType variableType, string path, out TValue value)
    {
        string savePath = ValidatePath(path);

        VariableSave saveData;
        if (FileExist(path))
        {
            string allFileText = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<VariableSave>(allFileText);
        }
        else
        {
            Debug.LogError("File not found.");
            value = default;
            return false;
        }


        if (variableType == varType.stringType)
        {
            if (saveData.stringDictionary.ContainsKey(key))
            {
                value = (TValue)Convert.ChangeType(saveData.stringDictionary[key], typeof(TValue));
                return true;
            }
            else Debug.LogError($"Key: {key} not found.");
        }
        else if (variableType == varType.intType)
        {
            if (saveData.intDictionary.ContainsKey(key))
            {
                value = (TValue)Convert.ChangeType(saveData.intDictionary[key], typeof(TValue));
                return true;
            }
            else Debug.LogError($"Key: {key} not found.");
        }
        else if (variableType == varType.floatType)
        {
            if (saveData.intDictionary.ContainsKey(key))
            {
                value = (TValue)Convert.ChangeType(saveData.intDictionary[key], typeof(TValue));
                return true;
            }
            else Debug.LogError($"Key: {key} not found.");
        }
        else if (variableType == varType.boolType)
        {
            if (saveData.intDictionary.ContainsKey(key))
            {
                value = (TValue)Convert.ChangeType(saveData.intDictionary[key], typeof(TValue));
                return true;
            }
            else Debug.LogError($"Key: {key} not found.");
        }
        else Debug.LogError("Type not supported. Supported variables: string, int, float & bool.");

        value = default;
        return false;
    }

    #endregion

    #region Files
    /// <summary>
    /// Checks if a file exist.
    /// </summary>
    /// <param name="path">The path you want to check. (Use SetDataPath to set the main path) (NEEDS EXTENTION)</param>
    /// <returns>Returns true if file exist.</returns>
    public static bool FileExist(string path)
    {
        string filePath = ValidatePath(path);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Delete a file.
    /// </summary>
    /// <param name="path">The file you want to delete. (Use SetDataPath to set the main path) (NEEDS EXTENTION)</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool DeleteFile(string path)
    {
        string filePath = ValidatePath(path);
        if (File.Exists(filePath)) File.Delete(filePath);
        else return false;
        return true;
    }

    /// <summary>
    /// Returns the number of files in a folder.
    /// </summary>
    /// <param name="path">The folder you want a filecount on. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the ammount of files in a folder.</returns>
    public static int GetFileCount(string path)
    {
        Path.Combine(GetDataPath(), path);
        return GetFiles(path).Length;
    }

    /// <summary>
    /// Returns a string array of the files in a folder.
    /// </summary>
    /// <param name="path">The folder you want the file names from. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns a string array.</returns>
    public static string[] GetFiles(string path)
    {
        string filePath = Path.Combine(GetDataPath(), path);
        return Directory.GetFiles(filePath);
    }

    /// <summary>
    /// Returns FileInfo from the files in a folder.
    /// </summary>
    /// <param name="path">The folder you want the FileInfo from. (Use SetDataPath to set the main path)</param>
    /// <param name="extension">The Extension you want to filter. (.* for all)</param>
    /// <returns>Returns a FileInfo array.</returns>
    public static FileInfo[] GetFilesInDirectory(string path, string extension = ".json")
    {
        string filePath = Path.Combine(GetDataPath(), path);
        DirectoryInfo directory = new DirectoryInfo(filePath);
        FileInfo[] fileInfos = directory.GetFiles("*" + extension);
        return fileInfos;
    }

    /// <summary>
    /// Renames a file.
    /// </summary>
    /// <param name="path">The file you want to rename. (Use SetDataPath to set the main path)</param>
    /// <param name="newName">The new file name (Needs location). (Use SetDataPath to set the main path)</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool RenameFile(string path, string newName)
    {
        return MoveFile(path, newName);
    }

    /// <summary>
    /// Moves a file to a new location
    /// </summary>
    /// <param name="path">The file you want to move. (Use SetDataPath to set the main path)</param>
    /// <param name="source">The new file location. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool MoveFile(string path, string source)
    {
        string filePath = ValidatePath(path);
        string renameFolder = ValidatePath(source);
        if (FileExist(path))
        {
            File.Move(filePath, renameFolder);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Copy a file to a location.
    /// </summary>
    /// <param name="source">The file you want to copy. (Use SetDataPath to set the main path)</param>
    /// <param name="dest">The destination for the coppied file. (Use SetDataPath to set the main path)</param>
    /// <param name="overwrite">Overwrite the new destination. (If fil already exist) Default true.</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool CopyFile(string source, string dest, bool overwrite = true)
    {
        string filePath = ValidatePath(source);
        string destPath = ValidatePath(dest);

        if (FileExist(source) && (!FileExist(dest) || overwrite))
        {
            File.Copy(filePath, destPath, overwrite);
            return true;
        }
        return false;
    }

    public static bool DeleteAllFilesInFolder(string path)
    {
        if (FolderExist(path))
        {
            string[] files = GetFiles(path);

            foreach (string file in files)
            {
                File.Delete(file);
            }

            return true;
        }

        return false;
    }

    #region FileDate
    /// <summary>
    /// Gets the file creation time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <returns>The DateTime the file was created.</returns>
    public static DateTime GetFileCreationTime(string path)
    {
        string filePath = ValidatePath(path);
        return File.GetCreationTime(filePath);
    }

    /// <summary>
    /// Gets the file last access time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <returns>The DateTime the file was last accessed.</returns>
    public static DateTime GetFileLastAccessTime(string path)
    {
        string filePath = ValidatePath(path);
        return File.GetLastAccessTime(filePath);
    }

    /// <summary>
    /// Gets the file last write time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <returns>The DateTime of the last write to the file.</returns>
    public static DateTime GetFileLastWriteTime(string path)
    {
        string filePath = ValidatePath(path);
        return File.GetLastWriteTime(filePath);
    }

    /// <summary>
    /// Sets the file creation time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <param name="creationTime">The creation date.</param>
    public static void SetFileCreationTime(string path, DateTime creationTime)
    {
        string filePath = ValidatePath(path);
        File.SetCreationTime(filePath, creationTime);
    }

    /// <summary>
    /// Sets the file last access time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <param name="accesTime">The last access date.</param>
    public static void SetFileLastAccessTime(string path, DateTime accesTime)
    {
        string filePath = ValidatePath(path);
        File.SetLastAccessTime(filePath, accesTime);
    }

    /// <summary>
    /// Sets the file last write time.
    /// </summary>
    /// <param name="path">The path of the file. (Use SetDataPath to set the main path)</param>
    /// <param name="writeTime">The last write date.</param>
    public static void SetFileLastWriteTime(string path, DateTime writeTime)
    {
        string filePath = ValidatePath(path);
        File.SetLastWriteTime(filePath, writeTime);
    }
    #endregion
    #endregion

    #region Folder
    /// <summary>
    /// Creates a folder.
    /// </summary>
    /// <param name="path">The path where the folder needs to be created. (Use SetDataPath to set the main path)</param>
    public static void CreateFolder(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        Directory.CreateDirectory(folderPath);
    }

    /// <summary>
    /// Checks if the folder exist.
    /// </summary>
    /// <param name="path">The path where the folder should be checked. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns true if the folder exists.</returns>
    public static bool FolderExist(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        return Directory.Exists(folderPath);
    }

    /// <summary>
    /// Deletes a folder.
    /// </summary>
    /// <param name="path">The path where the folder should be deleted. (Use SetDataPath to set the main path)</param>
    /// <param name="recursive">Remove all the content in the folder (If the folder contains data and it's set to false the folder can't be removed!). Default false</param>
    public static void DeleteFolder(string path, bool recursive = false)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        if (Directory.Exists(folderPath)) Directory.Delete(folderPath, recursive);
        else Debug.LogError("Folder not found!" + folderPath);
    }

    /// <summary>
    /// Gets the count of folders in a folder.
    /// </summary>
    /// <param name="path">The path where the folders should be counted. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the amount of folders in the folder.</returns>
    public static int GetFolderCount(string path)
    {
        string newPath = Path.Combine(GetDataPath(), path);
        return GetFolders(newPath).Length;
    }

    /// <summary>
    /// Returns a string array of folders in the folder. 
    /// </summary>
    /// <param name="path">The path where the folder should be checked. (Use SetDataPath to set the main path)</param>
    /// <returns>A string array.</returns>
    public static string[] GetFolders(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);

        if (FolderExist(path))
        {
            return Directory.GetDirectories(folderPath);
        }
        return new string[0];
    }

    /// <summary>
    /// Gets a DirectoryInfo array of the folders in the folder.
    /// </summary>
    /// <param name="path">The path where the folder should be checked. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns a DirectoryInfo array.</returns>
    public static DirectoryInfo[] GetFoldersInfo(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        if (FolderExist(path))
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            DirectoryInfo[] directoryInfo = directory.GetDirectories();
            return directoryInfo;
        }
        return null;
    }

    /// <summary>
    /// Rename a folder.
    /// </summary>
    /// <param name="path">The path of the folder you want to rename. (Use SetDataPath to set the main path)</param>
    /// <param name="newName">The new path for the folder. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool RenameFolder(string path, string newName)
    {
        return MoveFolder(path, newName);
    }


    /// <summary>
    /// Move a folder.
    /// </summary>
    /// <param name="source">The path of the folder you want to move. (Use SetDataPath to set the main path)</param>
    /// <param name="dest">The path where the folder should be moved to. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool MoveFolder(string source, string dest)
    {
        string filePath = Path.Combine(GetDataPath(), source);

        if (FolderExist(filePath))
        {
            string renameFolder = Path.Combine(GetDataPath(), dest);
            Directory.Move(filePath, renameFolder);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Copy a folder.
    /// </summary>
    /// <param name="source">The path where the folder should be copied from. (Use SetDataPath to set the main path)</param>
    /// <param name="dest">The destination path for the new folder. (Use SetDataPath to set the main path)</param>
    /// <param name="recursive">Copy all the content in the folder. Default true.</param>
    /// <returns>Returns true if succesfull.</returns>
    public static bool CopyFolder(string source, string dest, bool recursive = true)
    {
        string folderPath = Path.Combine(GetDataPath(), source);

        if (FolderExist(folderPath))
        {
            string destPath = Path.Combine(GetDataPath(), dest);
            CopyDirectory(folderPath, destPath, recursive);
            return true;
        }
        return false;
    }

    //Internal function
    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        string sourcePath = Path.Combine(GetDataPath(), sourceDir);
        string destPath = Path.Combine(GetDataPath(), destinationDir);

        DirectoryInfo dir = new DirectoryInfo(sourcePath);

        if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(destPath);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destPath, file.Name);
            file.CopyTo(targetFilePath);
        }

        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destPath, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    #region FolderDate
    /// <summary>
    /// Get the folder creation date
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the creation DateTime of the folder.</returns>
    public static DateTime GetFolderCreationTime(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        return Directory.GetCreationTime(folderPath);
    }

    /// <summary>
    /// Get the folder last access time.
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the last acces DateTime of the folder.</returns>
    public static DateTime GetFolderLastAccessTime(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        return Directory.GetLastAccessTime(folderPath);
    }

    /// <summary>
    /// Get the folder last write time
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <returns>Returns the last write DateTime of the folder.</returns>
    public static DateTime GetFolderLastWriteTime(string path)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        return Directory.GetLastWriteTime(folderPath);
    }

    /// <summary>
    /// Set the creation date of the folder.
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <param name="creationTime">The creation date.</param>
    public static void SetFolderCreationTime(string path, DateTime creationTime)
    {
        string folderPath = GetDataPath() + path;
        Directory.SetCreationTime(folderPath, creationTime);
    }

    /// <summary>
    /// Set the last access date of the folder.
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <param name="accesTime">The last access date of the folder.</param>
    public static void SetFolderLastAccessTime(string path, DateTime accesTime)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        Directory.SetLastAccessTime(folderPath, accesTime);
    }

    /// <summary>
    /// Set the last write date of the folder.
    /// </summary>
    /// <param name="path">The path of the folder. (Use SetDataPath to set the main path)</param>
    /// <param name="writeTime">The last write date of the folder.</param>
    public static void SetFolderLastWriteTime(string path, DateTime writeTime)
    {
        string folderPath = Path.Combine(GetDataPath(), path);
        Directory.SetLastWriteTime(folderPath, writeTime);
    }
    #endregion
    #endregion
}
