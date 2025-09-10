using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObstacleTracker : MonoBehaviour
{
    private string filePath;
    private string directoryPath;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeFile();
        
        var objs = GameObject.FindObjectsOfType<Collider>();
        foreach (var obj in objs)
        {
            WriteLine(obj.name);
        }
    }
    
    private void InitializeFile()
    {
        directoryPath = Application.persistentDataPath;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        filePath = Path.Combine(directoryPath, "ObstacleList.csv");

        // Write header line
        string header = "Object Name";
        File.WriteAllText(filePath, header + Environment.NewLine);
        Debug.Log("File Created: " + filePath);
    }
    
    private void WriteLine(string line)
    {
        try
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write record: {ex.Message}");
        }
    }
}
