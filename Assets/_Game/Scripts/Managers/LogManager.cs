using UnityEngine;
using System.IO;

public class LogManager : MonoBehaviour
{
    private static string logFilePath;

    void Awake()
    {
        // Initialize the log file path
        string parentDirectory = Application.dataPath;
        logFilePath = Path.Combine(parentDirectory, "Logs", "debug_log.txt");
        Debug.Log(logFilePath);
        // Create the Logs directory if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        // Clear the log file
        ClearLogFile();

        // Subscribe to the log message event
        Application.logMessageReceived += LogMessageReceived;
    }

    private void LogMessageReceived(string logString, string stackTrace, LogType type)
    {
        // Append log message to the file
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"{System.DateTime.Now}: {type} - {logString}");
            if (type == LogType.Exception)
            {
                writer.WriteLine(stackTrace);
            }
        }
    }

    private void ClearLogFile()
    {
        // Ensure the file exists before trying to clear it
        if (File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, string.Empty);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the log message event
        Application.logMessageReceived -= LogMessageReceived;
    }
}
