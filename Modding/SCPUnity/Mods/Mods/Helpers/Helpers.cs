using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using static SECTR_Member;

namespace Mods
{
    internal class Helpers
    {
        private static int _level = 0;
        private static bool firstRun = true;
        private static int childrenCount = 0;
        private static Component[] components;
        //private static string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "PrintGameObjectInfo.txt"));
        
        // 
        public static void PrintGameObjectInfo(GameObject gameObject)
        {
            string tabs = "";
            _level++;
            for (int j = 0; j < _level; j++)
            {
                tabs += "\t";
            }

            if (firstRun)
            {
                firstRun = false;
                Debug.LogWarning($"PrintGameObjectInfo: {gameObject}");
                outputFile.WriteLine($"PrintGameObjectInfo: {gameObject}");
            }
            else
            {
                // child object, not original one
                Debug.Log($"{tabs}{gameObject}");
                outputFile.WriteLine($"{tabs}{gameObject}");
                tabs += "\t";
            }

            Debug.LogWarning($"{tabs}Components:");
            outputFile.WriteLine($"{tabs}Components:");
            components = gameObject.GetComponents<Component>();
            foreach (Component c in components)
            {
                Debug.Log($"{tabs}\t{c.GetType()}");
                outputFile.WriteLine($"{tabs}\t{c.GetType()}");
            }

            Debug.LogWarning($"{tabs}Children:");
            outputFile.WriteLine($"{tabs}Children:");
            for(int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                //Debug.Log($"{tabs}\t{child}");
                PrintGameObjectInfo(child);
                _level--;
            }
        }

        ~Helpers()
        {
            // need to close application to close file
            outputFile.Close();
        }
    }
}