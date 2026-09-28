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

    // Constructor to set data paths and names
    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {

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


        } catch (Exception e)
        {
            Debug.LogError("Error trying to save data file: " + fullPath + "\n" + e);
        }
    }
}
