public class Utilities
{
    public static void WriteToFile(string filePath, string value)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"{filePath} does not exist, creating it");
            File.Create(filePath).Close();
            Console.WriteLine($"{filePath} was created");
        }

        try
        {
            Console.WriteLine($"Attempting to write to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(value);
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing to file: {e.Message}");
        }

        Console.WriteLine($"Finished writing to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
    }

    public static void WriteToFile(string filePath, int value )
    {
        WriteToFile(filePath, value.ToString());
    }

    /// <summary>
    /// Read all bytes into a <see langword="byte[]"/> in a file until end of stream is reached
    /// </summary>
    /// <param name="filePath">File path to attempt to open</param>
    /// <returns>All bytes read as a <see cref="string"/> or <see langword="null"/></returns>
    public static string ReadFileBytes(string filePath)
    {
        byte[] buffer = null;

        try
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // old school straight C style
                int length = (int)fileStream.Length;    // get file length
                buffer = new byte[length];              // create buffer
                int count;                              // actual number of bytes read
                int offset = 0;                         // total number of bytes read

                while ((count = fileStream.Read(buffer, offset, length - offset)) > 0)
                {
                    offset += count;
                }

                fileStream.Close();
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }

        if (buffer != null) return System.Text.Encoding.Default.GetString(buffer);
        return null;
    }

    public static string ReadFile(string filePath)
    {
        string value = "";
        using (StreamReader file = new StreamReader(filePath))
        {
            while ((value = file.ReadLine()) != null)
            {
            }

            file.Close();
        }

        return value;
    }

    /// <summary>
    /// Check if <see cref="File"/> is ready by attempting to open file
    /// </summary>
    /// <param name="filePath">Path to <see cref="File"/> to check</param>
    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <returns><see langword="true"/> if <see cref="File"/> can opened and has bytes to read, <see langword="false"/> otherwise</returns>
    public static bool IsFileReady(string filePath = null, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
            return false;
        }

        try
        {
            using (FileStream inputStream = File.Open(filePath, FileMode.Open, fileAccess, fileShare))
                return inputStream.Length > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Wait for file to be available
    /// </summary>
    /// <param name="filePath">Path to <see cref="File"/> to wait for</param>
    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <param name="milliseconds">Time in milliseconds for <see cref="Thread"/> to sleep before checking <see cref="File"/> again</param>
    public static void WaitForFile(string filePath, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None, int milliseconds = 1000)
    {
        while (!IsFileReady(filePath, fileAccess, fileShare))
        {
            Thread.Sleep(milliseconds);
        }
    }

    /// <summary>
    /// Wait for file to be available
    /// </summary>
    /// <param name="filePath">Path to <see cref="File"/> to wait for</param>
    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <param name="milliseconds">Time in milliseconds for <see cref="Thread"/> to sleep before checking <see cref="File"/> again</param>
    /// <param name="end"><see langword="bool"/> to check if false each iteration</param>
    public static void WaitForFile(string filePath, ref bool end, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None, int milliseconds = 1000)
    {
        while (!IsFileReady(filePath, fileAccess, fileShare) && !end)
        {
            Thread.Sleep(milliseconds);
        }
    }

    /// <summary>
    /// Clears [root game folder]/Logs/ folder
    /// </summary>
    public static void ClearLogFiles()
    {
        string logDirectory = Directory.GetCurrentDirectory() + @"\Logs\";
        if (Directory.Exists(logDirectory))
            Directory.Delete(logDirectory, true);
    }

    /// <summary>
    /// Get the last character of a <see cref="string"/>
    /// </summary>
    /// <param name="input">Input <see cref="string"/></param>
    /// <returns>Last <see langword="char"/> in <see cref="string"/></returns>
    public static char GetLastCharacter(string input)
    {
        return string.IsNullOrEmpty(input) ? '\0' : input[^1];
    }

    public static bool WriteToFile(string filePath, params string[] values)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"{filePath} does not exist, creating it");
            File.Create(filePath).Close();
            Console.WriteLine($"{filePath} was created");
        }

        try
        {
            Console.WriteLine($"Attempting to write to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (string value in values)
                {
                    writer.WriteLine($"{value},");
                }

                writer.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing to file: {e.Message}");
            return false;
        }

        Console.WriteLine($"Finished writing to {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");
        return true;
    }
}
