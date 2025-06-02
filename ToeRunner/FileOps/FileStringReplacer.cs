using System;
using System.IO;
using System.Text;

namespace ToeRunner.FileOps
{
    /// <summary>
    /// Provides file string replacement functionality.
    /// </summary>
    public class FileStringReplacer
    {
        /// <summary>
        /// Reads a file, replaces specified string with a replacement string, and writes to output path.
        /// </summary>
        /// <param name="filePath">Path to the input file to read</param>
        /// <param name="stringToReplace">String to be replaced</param>
        /// <param name="replacementString">String to replace with</param>
        /// <param name="outputPath">Path to write the modified content</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool ReplaceStringInFile(string filePath, string stringToReplace, string replacementString, string outputPath)
        {
            try
            {
                // Read the file into a string
                string fileContent = System.IO.File.ReadAllText(filePath);
                
                // Replace the string
                string modifiedContent = fileContent.Replace(stringToReplace, replacementString);
                
                // Write to output path
                System.IO.File.WriteAllText(outputPath, modifiedContent, Encoding.UTF8);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error replacing string in file: {ex.Message}");
                return false;
            }
        }
    }
}
