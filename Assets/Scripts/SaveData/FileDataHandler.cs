using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    // Stores save file paths and file names
    private string dataDirPath = "";
    private string dataFileName = "";

    // For Encryption
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";

    // Constructor to set data paths and names
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    // Return game data objects
    public GameData Load()
    {
        // Use Path.Combine to account for different OS
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;

        // Check if load data exists
        if (File.Exists(fullPath))
        {
            // Catch errors while loading
            try
            {
                // Load data from file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserializes the data from JSON
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            } catch  (Exception e)
            {
                Debug.LogError("Error trying to load data file: " + fullPath + "\n" + e);
            }

        }

        return loadedData;
    }

    // Writes save data to file
    public void Save(GameData data)
    {
        // Use Path.Combine to account for different OS
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        // Catches errors while saving
        try
        {
            // Create the directory the file will go to
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize game data object to JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            // Encrypt the data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write data to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        } catch (Exception e)
        {
            Debug.LogError("Error trying to save data file: " + fullPath + "\n" + e);
        }
    }

    // Takes in string data and returns encrypted or decrypted data
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}
