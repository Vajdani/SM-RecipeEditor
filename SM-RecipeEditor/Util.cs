using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace SM_RecipeEditor
{
	static class Util
	{
		public static string? GetSteamInstallPath()
		{
			try
			{
                using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam")!;
                if (key != null)
                {
                    Object o = key.GetValue("InstallPath")!;
                    if (o != null)
                    {
                        return o.ToString();
                    }
                }
            }
			catch (Exception){ return null; }
			return null;
		}

		public static string? GetLoggedInSteamUserID(string? steamDirectory = null)
		{
			steamDirectory ??= GetSteamInstallPath();
			if (steamDirectory == null)
			{
				return null;
			}
			string loginUsersPath = Path.Combine(steamDirectory, "config", "loginusers.vdf");

			if (!File.Exists(loginUsersPath))
			{
				return null; // File not found
			}

			string fileContent = File.ReadAllText(loginUsersPath);

			// Match each user block and extract relevant information
			var matches = Regex.Matches(fileContent, "\"(\\d+)\"\\s*\\{([^}]*)\\}");
			foreach (Match match in matches)
			{
				string steamID = match.Groups[1].Value;
				string userBlock = match.Groups[2].Value;

				// Check if MostRecent is set to 1
				if (Regex.IsMatch(userBlock, "\"MostRecent\"\\s*\"1\""))
				{
					return steamID;
				}
			}

			return null; // No active user found
		}

		public static List<string> GetSteamLibraryFolders(string steamInstallPath)
		{
			List<string> libraryFolders = [Path.Combine(steamInstallPath, "steamapps")];
			string libraryFoldersFile = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");

			if (Util.IsValidFile(libraryFoldersFile))
			{
				foreach (string line in File.ReadAllLines(libraryFoldersFile))
				{
					if (line.Contains("\"path\""))
					{
						string path = line.Split('\"')[3];
						libraryFolders.Add(Path.Combine(path.Replace(@"\\", @"\"), "steamapps"));
					}
				}
			}

			return libraryFolders;
		}

		public static string? GetGameInstallPath(string appID)
		{
			try
			{
				string? steamInstallPath = GetSteamInstallPath();
				if (steamInstallPath == null)
					return null;

				List<string> libraryFolders = GetSteamLibraryFolders(steamInstallPath);

				foreach (string libraryFolder in libraryFolders)
				{
					string appManifestFile = Path.Combine(libraryFolder, $"appmanifest_{appID}.acf");
					if (File.Exists(appManifestFile))
					{
						string[] lines = File.ReadAllLines(appManifestFile);
						foreach (string line in lines)
						{
							if (line.Trim().StartsWith("\"installdir\""))
							{
								string installDir = line.Split('\"')[3];
								return Path.Combine(libraryFolder, "common", installDir);
							}
						}
					}
				}
			}
			catch (Exception){}
			return null;
		}

        public static bool IsValidFile(string path, bool checkRW = false)
        {
            // Check if the path is well-formed
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
            {
                return false;
            }

            // Check if the file exists
            if (!File.Exists(path))
            {
                return false;
            }

            if (checkRW) // Check for read/write access
            {
                try
                {
                    // Try to open the file for reading and writing
                    using (FileStream fs = new(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        // Optionally, you can write and delete a temporary file to test permissions
                        string testFile = Path.GetTempFileName();
                        using (StreamWriter sw = new(testFile))
                        {
                            sw.Write("test");
                        }
                        File.Delete(testFile);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
