namespace TfsPlugin2013
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Define class for log operation.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Maintain Event Log 
        /// </summary>
        /// <param name="content"> content/message to log. </param>
        public static void Log(string content)
        {
            Task.Run(() => LogAsync(content));
        }

        /// <summary>
        /// Maintain Event Log asynchronously
        /// </summary>
        /// <param name="content"> content/message to log. </param>
        private static void LogAsync(string content)
        {
            var filePath = RetrieveFilePath();
            using (var file = new StreamWriter(filePath, true))
            {
                file.WriteLine("{0}{1}{2}", DateTime.Now, "\n------------\n", content);
            }
        }

        /// <summary>
        /// Get file path ($DRIVE:/ProgramData/Plugin/Log).
        /// </summary>
        /// <returns> file path  </returns>
        private static string RetrieveFilePath()
        {
            var specialFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            var configurationFolder = Path.Combine(specialFolderPath, @"\Plugin\Log");
            if (!Directory.Exists(configurationFolder))
            {
                Directory.CreateDirectory(configurationFolder);
            }

            var filePath = Path.Combine(configurationFolder, "Sample.log");
            return filePath;
        }
    }
}
