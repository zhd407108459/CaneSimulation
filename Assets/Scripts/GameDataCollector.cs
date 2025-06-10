using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataCollector : MonoBehaviour
{
    public static GameDataCollector instance;
    private string filePath;
    private string directoryPath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddGeneralRecord("Anything", "Can", "Be", "Recorded", "Here");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddPlayerCollisionRecord(new Vector3(1, 1, 1), new Vector3(2, 2, 2), "Test");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddPlayerCrossBoundaryRecord(new Vector3(3, 3, 3));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddTaskStartRecord();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddTaskCompletionRecord(Time.time);
        }
    }

    private void InitializeFile()
    {
        directoryPath = Path.Combine(Application.persistentDataPath, "RecordedData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Path.Combine(directoryPath, $"Records_{timestamp}.csv");

        // Write header line
        string header = "SystemTime,UnityTime,RecordType,Data...";
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

    /// <summary>
    /// Add a general record with arbitrary string parameters.
    /// </summary>
    public void AddGeneralRecord(params string[] args)
    {
        string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string unityTime = Time.time.ToString("F3");
        List<string> fields = new List<string> { systemTime, unityTime, "General" };
        fields.AddRange(args);
        string line = string.Join(",", fields);
        WriteLine(line);
    }

    /// <summary>
    /// Add a collision record: player position, collision position, and object name.
    /// </summary>
    public void AddPlayerCollisionRecord(Vector3 playerPosition, Vector3 collisionPosition, string objectName)
    {
        string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string unityTime = Time.time.ToString("F3");
        List<string> fields = new List<string>
        {
            systemTime,
            unityTime,
            "Collision",
            playerPosition.x.ToString("F3"),
            playerPosition.y.ToString("F3"),
            playerPosition.z.ToString("F3"),
            collisionPosition.x.ToString("F3"),
            collisionPosition.y.ToString("F3"),
            collisionPosition.z.ToString("F3"),
            objectName
        };
        string line = string.Join(",", fields);
        WriteLine(line);
    }

    /// <summary>
    /// Add a cross-boundary record: player went out-of-bounds at this position.
    /// </summary>
    public void AddPlayerCrossBoundaryRecord(Vector3 outPosition)
    {
        string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string unityTime = Time.time.ToString("F3");
        List<string> fields = new List<string>
        {
            systemTime,
            unityTime,
            "CrossBoundary",
            outPosition.x.ToString("F3"),
            outPosition.y.ToString("F3"),
            outPosition.z.ToString("F3")
        };
        string line = string.Join(",", fields);
        WriteLine(line);
    }

    /// <summary>
    /// Add a task start record (no parameters).
    /// </summary>
    public void AddTaskStartRecord()
    {
        string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string unityTime = Time.time.ToString("F3");
        List<string> fields = new List<string>
        {
            systemTime,
            unityTime,
            "TaskStart"
        };
        string line = string.Join(",", fields);
        WriteLine(line);
    }

    /// <summary>
    /// Add a task completion record: time taken to complete the task.
    /// </summary>
    public void AddTaskCompletionRecord(float completionTime)
    {
        string systemTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string unityTime = Time.time.ToString("F3");
        List<string> fields = new List<string>
        {
            systemTime,
            unityTime,
            "TaskCompletion",
            completionTime.ToString("F3")
        };
        string line = string.Join(",", fields);
        WriteLine(line);
    }
}
