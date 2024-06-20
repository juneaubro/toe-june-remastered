using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Mods
{
    internal class Helpers
    {

        private static int _level = 0;
        private static Component[] components;

        /// <summary>
        /// Recursively prints all children and components attached to the gameObject, optionally printing to a log file
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // Force no inlining for stack walking in Print()
        public static void PrintGameObjectInfo(GameObject gameObject, bool printToLog = true)
        {
            string additionalTab = "";
            string tabs = "";

            if (_level == 0)
            {
                Print($"PrintGameObjectInfo: {gameObject}", printToLog);
            }
            else
            {
                // why does this work
                for (int j = 0; j < _level; j++)
                {
                    tabs += "\t";
                }
            }

            _level++;

            for (int j = 0; j < _level; j++)
            {
                tabs += "\t";
            }

            Print($"{tabs}Components:", printToLog);
            components = gameObject.GetComponents<Component>();
            foreach (Component c in components)
            {
                Print($"{tabs}\t{c.GetType()}", printToLog);
            }
            if (gameObject.transform.childCount != 0)
            {
                Print($"{tabs}Children:", printToLog);

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    GameObject child = gameObject.transform.GetChild(i).gameObject;
                    Print($"{tabs}\t{child}", printToLog);
                    PrintGameObjectInfo(child);
                    _level--;
                }
            }

        }

        /// <summary>
        /// Prints string to console and optionally to a log file in [root game folder]/Logs/
        /// It will create a folder in the Logs folder for each new class
        /// and a seperate text file for each method name that uses the Print() method
        /// If txt file is given the wrong name, try decorating the method with [MethodImpl(MethodImplOptions.NoInlining)]
        /// </summary>
        public static void Print(string stringToPrint, bool logToOutputFile = false, LogType logType = LogType.Log,
            LogOption logOption = LogOption.None, params object[] args)
        {
            Debug.LogFormat(logType, logOption, null, stringToPrint, args);

            if (logToOutputFile)
            {
                MethodBase methodInfo;

                // using PrintGameObjectInfo while printing to a log file
                // should give the method calling it rather than PrintGameObjectInfo itself
                // r e  c   u    r     s      i       o        n
                for (int framesToSkip = 1; (methodInfo = new StackFrame(framesToSkip, true).GetMethod()).Name == "PrintGameObjectInfo"; ++framesToSkip)
                {
                    // an "interesting" way to inline methodInfo initialization without having to remake the StackFrame when broken from loop
                }

                string className = "";
                string methodName = methodInfo.Name + ".txt";

                if (methodInfo.ReflectedType != null)
                {
                    className = methodInfo.ReflectedType.Name;
                }

                string logDirectory = Directory.GetCurrentDirectory() + @"\Logs\";
                Directory.CreateDirectory(logDirectory);
                DirectoryInfo dirInfo = Directory.CreateDirectory(logDirectory + className + @"\");
                string directory = dirInfo.FullName;

                using (StreamWriter outputFile = new StreamWriter(Path.Combine(directory, methodName), true))
                {
                    outputFile.WriteLine(stringToPrint);
                    outputFile.Close();

                }
            }
        }

        /// <summary>
        /// Clears [root game folder]/Logs/ folder
        /// </summary>
        public static void ClearLogFiles()
        {
            string logDirectory = Directory.GetCurrentDirectory() + @"\Logs\";
            if(Directory.Exists(logDirectory))
                Directory.Delete(logDirectory, true);
        }

        /// <summary>
        /// Copy a component to a different GameObject if it doesn't already exist,
        /// otherwise copy all fields to the existing component on the destination GameObject
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="original">Original component to copy</param>
        /// <param name="destination">Destination GameObject to copy component to</param>
        /// <returns>Copied component</returns>
        public static T CopyComponent<T>(T original, ref GameObject destination) where T : Component
        {
            var type = original.GetType();
            var fields = type.GetFields();
            if (destination.GetComponent(type) == null)
            {
                var copy = destination.AddComponent(type);
                foreach (var field in fields) 
                    field.SetValue(copy, field.GetValue(original));
                return copy as T;
            } 
            var copy2 = destination.GetComponent(type);
            foreach (var field in fields)
                field.SetValue(copy2, field.GetValue(original));

            return copy2 as T;
        }
    }
}