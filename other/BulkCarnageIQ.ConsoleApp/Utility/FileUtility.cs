using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkCarnageIQ.ConsoleApp.Utility
{
    public class FileUtility
    {
        public static string GetProjectRoot()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectRoot = FindProjectRoot(currentDirectory, "BulkCarnageIQ.ConsoleApp.csproj");

            if (projectRoot == null)
            {
                throw new Exception("Could not find project root.");
            }
            return projectRoot;
        }

        private static string FindProjectRoot(string currentDir, string projectFileName)
        {
            string dir = currentDir;
            while (!File.Exists(Path.Combine(dir, projectFileName)))
            {
                dir = Directory.GetParent(dir)?.FullName;
                if (dir == null) return null;
            }
            return dir;
        }
    }
}
