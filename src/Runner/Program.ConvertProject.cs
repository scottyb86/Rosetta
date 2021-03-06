﻿/// <summary>
/// Program.ConvertProject.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Runner
{
    using System;

    using Rosetta.Executable;

    /// <summary>
    /// Part of program responsible for translating one single file.
    /// 
    /// TODO: Remove as a build task can be used instead.
    /// </summary>
    internal partial class Program
    {
        protected virtual void ConvertProject()
        {
            
        }

        #region Helpers

        private static string GetOutputFolderForProject(string userInput)
        {
            if (userInput != null)
            {
                // User provided a path: check the path is all right
                if (FileManager.IsDirectoryPathCorrect(userInput))
                {
                    return userInput;
                }

                // Wrong path
                throw new InvalidOperationException("Invalid path provided!");
            }

            // User did not provide a path, we get the current path
            return FileManager.ApplicationExecutingPath;
        }

        #endregion
    }
}
