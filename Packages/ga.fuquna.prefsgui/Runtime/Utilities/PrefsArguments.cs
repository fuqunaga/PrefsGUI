using System;
using UnityEngine;

namespace PrefsGUI
{
    /// <summary>
    /// 引数解析
    /// </summary>
    public static class PrefsArguments
    {
        public const string ArgumentPrefix = "-prefsgui";
        public const string FileNameArgument = ArgumentPrefix+"-file-name";
        public const string FolderPathArgument = ArgumentPrefix+"-folder-path";
        
        public static string FileName { get; private set; }
        public static string FolderPath { get; private set; }
        
        static PrefsArguments()
        {
            var args = Environment.GetCommandLineArgs();
            for(var i = 0; i < args.Length - 1; i++)
            {
                var arg = args[i].ToLower();
                if (!arg.StartsWith(ArgumentPrefix))
                {
                    continue;
                }
                
                switch (arg)
                {
                    case FileNameArgument:
                        FileName = args[i + 1];
                        i++;
                        continue;
                    case FolderPathArgument:
                        FolderPath = args[i + 1];
                        i++;
                        continue;
                }
                
                Debug.LogWarning($"PrefsArguments: Unknown argument [{arg}]");
            }
        }
   }
}