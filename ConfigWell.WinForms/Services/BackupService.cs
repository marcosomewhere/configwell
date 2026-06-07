using System.IO;

namespace ConfigWell.WinForms.Services
{
    public class BackupService
    {
        public string CreateBackup(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Source file not found for backup.", filePath);

            string backupPath = filePath + ".bak";
            File.Copy(filePath, backupPath, overwrite: true);
            return backupPath;
        }
    }
}
