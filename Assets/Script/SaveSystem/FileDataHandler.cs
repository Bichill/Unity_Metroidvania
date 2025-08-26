using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.CompilerServices;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encrypData = false;
    private string codeWord = "bichill";

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encrypData = _encryData;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(_data, true);

            if (encrypData)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error on trying to save data on file" + fullPath + "\n" + e);
        }
    }

    public GameData Load()
    {
        //自动组合路径
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encrypData)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on trying to load data on file" + fullPath + "\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

    }

    private string EncryptDecrypt(string _data)
    {
        string modifierData = "";
        
        for (int i = 0; i < _data.Length; i++)
        {
            modifierData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifierData;
    }
}
