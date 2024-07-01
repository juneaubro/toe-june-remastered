public class Utilities
{

    #region READONLY FIELDS
    private readonly string CurrentDirectory;
    private readonly string CurrentFolder;

    public readonly string GameFileName;
    public readonly string GameFilePath;
    public readonly string GamePidName;
    public readonly string GamePidPath;

    public readonly string BinPath;

    public readonly string LobbyInfoFileName;
    public readonly string LobbyInfoFilePath;

    public readonly string ServerFileName;
    public readonly string ServerPidName;
    public readonly string ServerTxtFileName;
    public readonly string ServerPidPath;
    public readonly string ServerFilePath;
    public readonly string ServerTxtFilePath;

    public readonly string ClientFileName;
    public readonly string ClientPidName;
    public readonly string ClientPidPath;
    public readonly string ClientFilePath;
    #endregion

    public Utilities()
    {
        CurrentDirectory = Directory.GetCurrentDirectory();
        CurrentFolder =
            CurrentDirectory.Substring(CurrentDirectory.LastIndexOf('\\') + 1);

        BinPath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\";

        GameFileName = "SCP Unity.exe";
        GamePidName = "gamepid.txt";
        GameFilePath = CurrentFolder == "SCMP" 
            ? $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\{GameFileName}"
            : $@"{CurrentDirectory}\{GameFileName}";
        GamePidPath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\{GamePidName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{GamePidName}";

        LobbyInfoFileName = "lobbyinfo.txt";
        LobbyInfoFilePath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\{LobbyInfoFileName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{LobbyInfoFileName}";

        ServerFileName = "Server.exe";
        ServerPidName = "serverpid.txt";
        ServerTxtFileName = "server.txt";
        ServerPidPath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\{ServerPidName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ServerPidName}";
        ServerFilePath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\{ServerFileName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\{ServerFileName}";
        ServerTxtFilePath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\{ServerTxtFileName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ServerTxtFileName}";

        ClientFileName = "Client.exe";
        ClientPidName = "pid.txt";
        ClientPidPath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\bin\{ClientPidName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\bin\{ClientPidName}";
        ClientFilePath = CurrentFolder == "SCMP"
            ? $@"{CurrentDirectory}\{ClientFileName}"
            : $@"{CurrentDirectory}\BepInEx\plugins\SCMP\{ClientFileName}";
}

    public void WriteToFile(string filePath, string value, char delimiter = default)
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
                if (delimiter != default)
                    value += delimiter;
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

    public void WriteToFile(string filePath, int value, char delimiter = default)
    {
        WriteToFile(filePath, value.ToString(), delimiter);
    }

    /// <summary>
    /// Read all bytes into a <see langword="byte[]"/> in a file until end of stream is reached
    /// </summary>
    /// <param name="filePath">File path to attempt to open</param>
    /// <returns>All bytes read as a <see cref="string"/> or <see langword="null"/></returns>
    public string ReadFileBytes(string filePath, bool printRead = false)
    {
        byte[] buffer = null;

        if(printRead)
            Console.WriteLine($"Reading file: {filePath.Substring(filePath.LastIndexOf('\\') + 1)}");

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

    public string ReadFile(string filePath)
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
    /// <param name="shouldCreateFile">If <see cref="File"/> should be created if not found</param>
    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <returns><see langword="true"/> if <see cref="File"/> can opened and has bytes to read, <see langword="false"/> otherwise</returns>
    public bool IsFileReady(string filePath = "", bool shouldCreateFile = true, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None)
    {
        if (!File.Exists(filePath) && shouldCreateFile)
        {
            File.Create(filePath).Close();
            //return false;
        }

        try
        {
            using (FileStream inputStream = File.Open(filePath, FileMode.Open, fileAccess, fileShare))
            {
                if (fileAccess == FileAccess.Write)
                {
                    inputStream.Close();
                    return true;
                }

                return inputStream.Length > 0;
            }
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
    /// <param name="shouldCreateFile">If <see cref="File"/> should be created if not found</param>    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <param name="milliseconds">Time in milliseconds for <see cref="Thread"/> to sleep before checking <see cref="File"/> again</param>
    public void WaitForFile(string filePath, bool shouldCreateFile = true, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None, int milliseconds = 1000)
    {
        while (!IsFileReady(filePath, shouldCreateFile, fileAccess, fileShare))
        {
            Thread.Sleep(milliseconds);
        }
    }

    /// <summary>
    /// Wait for file to be available
    /// </summary>
    /// <param name="filePath">Path to <see cref="File"/> to wait for</param>
    /// <param name="shouldCreateFile">If <see cref="File"/> should be created if not found</param>    /// <param name="fileAccess"><see cref="FileAccess"/> to attempt to open file with</param>
    /// <param name="fileShare"><see cref="FileShare"/> to attempt to open file with</param>
    /// <param name="milliseconds">Time in milliseconds for <see cref="Thread"/> to sleep before checking <see cref="File"/> again</param>
    /// <param name="end"><see langword="bool"/> to check if false each iteration</param>
    public void WaitForFile(string filePath, ref bool end, bool shouldCreateFile = true, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.None, int milliseconds = 1000)
    {
        while (!IsFileReady(filePath, shouldCreateFile, fileAccess, fileShare) && !end)
        {
            Thread.Sleep(milliseconds);
        }
    }

    /// <summary>
    /// Clears [root game folder]/Logs/ folder
    /// </summary>
    public void ClearLogFiles()
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
    public char GetLastCharacter(string input)
    {
        return string.IsNullOrEmpty(input) ? '\0' : input[^1];
    }

    public bool WriteToFile(string filePath, params string[] values)
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

    public void RemakeBinIfExists()
    {
        if (Directory.Exists(BinPath))
            Directory.Delete(BinPath, true);

        Directory.CreateDirectory(BinPath);
    }
}