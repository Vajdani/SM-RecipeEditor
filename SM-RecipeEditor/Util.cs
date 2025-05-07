using Microsoft.Win32;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SM_RecipeEditor
{
	static class Util
	{
		public static string programVersion = Application.ProductVersion.Substring(0, Application.ProductVersion.IndexOf('+'));
		public static string programName = "MyGui.net " + programVersion;
		#region Steam Utils
		public static string? GetSteamInstallPath()
		{
			try
			{
				using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
				{
					if (key != null)
					{
						Object o = key.GetValue("InstallPath");
						if (o != null)
						{
							return o.ToString();
						}
					}
				}
			}
			catch (Exception){ return null; }
			return null;
		}

		public static string GetLoggedInSteamUserID(string steamDirectory = null)
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
			List<string> libraryFolders = new List<string> { Path.Combine(steamInstallPath, "steamapps") };
			string libraryFoldersFile = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");

			if (Util.IsValidFile(libraryFoldersFile))
			{
				string vdfContent = File.ReadAllText(libraryFoldersFile);
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
		#endregion

		#region Path Converting

		/// <summary>
		/// It might only work with paths with backslashes!
		/// </summary>
		public static string ConvertToGamePath(string path, string smPath, string localModsPath, string workshopModsPath)
		{
			string dataPath = Path.Combine(smPath, "Data");
			string guiPath = Path.Combine(dataPath, "Gui");
			if (path.StartsWith(guiPath))
			{
				return Path.GetFileName(path.Replace(guiPath, ""));
			}
			if(path.StartsWith(dataPath))
			{
				return path.Replace(dataPath, "$GAME_DATA").Replace('\\', '/');
			}
			string survivalPath = Path.Combine(smPath, "Survival");
			if(path.StartsWith(survivalPath))
			{
				return path.Replace(survivalPath, "$SURVIVAL_DATA").Replace('\\', '/');
			}
			string challengePath = Path.Combine(smPath, "ChallengeData");
			if (path.StartsWith(challengePath))
			{
				return path.Replace(challengePath, "$CHALLENGE_DATA").Replace('\\', '/');
			}

			string modWhere = "";
			if (path.StartsWith(localModsPath)) modWhere = localModsPath;
			else if (path.StartsWith(workshopModsPath)) modWhere = workshopModsPath;
			if(modWhere != "")
			{
				string modName = path.Replace(modWhere, "").Split(['/', '\\'])[1];
				string modPath = Path.Combine(modWhere, modName);

				string descPath = Path.Combine(modPath, "description.json");
				if (!Util.IsValidFile(descPath))
				{
					return "";
				}
				string descJsonStr = File.ReadAllText(descPath);
				JsonElement descJson = JsonSerializer.Deserialize<JsonElement>(descJsonStr);
				string localId = descJson.GetProperty("localId").GetString();
				return path.Replace(modPath, "$CONTENT_" + localId).Replace('\\', '/');
			}

			return "";
		}

		public static string ConvertToSystemPath(string path, string smPath, Dictionary<string, string> modUuidToPath)
		{
			if (path.StartsWith("$GAME_DATA"))
			{
				return path.Replace("$GAME_DATA", Path.Combine(smPath, "Data")).Replace('/', '\\');
			}
			if (path.StartsWith("$SURVIVAL_DATA"))
			{
				return path.Replace("$SURVIVAL_DATA", Path.Combine(smPath, "Survival")).Replace('/', '\\');
			}
			if (path.StartsWith("$CHALLENGE_DATA"))
			{
				return path.Replace("$CHALLENGE_DATA", Path.Combine(smPath, "ChallengeData")).Replace('/', '\\');
			}
			if (path.StartsWith("$CONTENT_"))
			{
				string uuid = path.Substring(9, 36);
				if (modUuidToPath.TryGetValue(uuid, out string modPath))
				{
					return Path.Combine(modPath, path[46..]).Replace('/', '\\');
				}
				return "";
			}
			if (path.Contains('.') && !path.Contains('\\') && !path.Contains('/'))
			{
				return FindFileInSubDirs(Path.Combine(smPath, "Data", "Gui"), path) ?? "";
			}

			return "";
		}

		/// <summary>
		/// modFolders should contain the path to the local mods folder and workshop mods folder
		/// The return should also be cached, so this shouldn't be called every time you want to get it
		/// </summary>
		public static Dictionary<string, string> GetModUuidsAndPaths(string[] modFolders)
		{
			Dictionary<string, string> dict = new();
			JsonSerializerOptions option = new()
			{
				ReadCommentHandling = JsonCommentHandling.Skip
			};
			foreach (var path in modFolders)
			{
				if (!Directory.Exists(path))
				{
					continue;
				}
				foreach (var modPath in Directory.GetDirectories(path))
				{
					string descJsonPath = Path.Combine(modPath, "description.json");
					if (!File.Exists(descJsonPath)) continue;
					string descJsonStr = File.ReadAllText(descJsonPath).Replace("\r\n", "").Replace("\n", "");
					try
					{
						JsonElement descJson = JsonSerializer.Deserialize<JsonElement>(descJsonStr, option);
						string type = descJson.GetProperty("type").GetString();
						if (type == "Blocks and Parts" || type == "Custom Game")
						{
							string localId = descJson.GetProperty("localId").GetString();
							dict[localId] = modPath;
						}
					} catch (Exception) { } //Some mods have shit wrong
				}
			}
			return dict;
		}
		#endregion

		#region Util Utils
		public static Random rand = new();

		public static string SystemToMyGuiString(string input)
		{
			return input.ReplaceLineEndings("\\n");

		}

		public static string MyGuiToSystemString(string input)
		{
			return input.Replace("\\n", Environment.NewLine);
		}

		public static T ShallowCopy<T>(T source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			MethodInfo memberwiseClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)memberwiseClone.Invoke(source, null);
		}

		static JsonSerializerOptions deepCopyJsonSerializerOptions = new JsonSerializerOptions
		{
			IncludeFields = true
		};

		public static T DeepCopy<T>(T source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			// Serialize the source object to JSON and deserialize it back into a new object
			var serialized = JsonSerializer.Serialize(source, deepCopyJsonSerializerOptions);
			return JsonSerializer.Deserialize<T>(serialized, deepCopyJsonSerializerOptions);
		}

		public static Point GetWidgetPos(bool isReal, string input, Point? parentSize = null)
		{
			parentSize ??= new(1, 1);
			string[] numbers = input.Split(' ');
			double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

			if (isReal)
			{
				parsedNumbers[0] *= parentSize.Value.X;
				parsedNumbers[1] *= parentSize.Value.Y;
			}

			int x1 = (int)Math.Round(parsedNumbers[0]);
			int y1 = (int)Math.Round(parsedNumbers[1]);
			return new(x1, y1);
		}

		public static Tuple<Point, Point> GetWidgetPosAndSize(bool isReal, string input, Point? parentSize = null)
		{
			parentSize ??= new(1, 1);
			string[] numbers = input.Split(' ');
			double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

			if (isReal)
			{
				parsedNumbers[0] *= parentSize.Value.X;
				parsedNumbers[1] *= parentSize.Value.Y;
				parsedNumbers[2] *= parentSize.Value.X;
				parsedNumbers[3] *= parentSize.Value.Y;
			}

			int x1 = (int)Math.Round(parsedNumbers[0]);
			int y1 = (int)Math.Round(parsedNumbers[1]);
			int x2 = (int)Math.Round(parsedNumbers[2]);
			int y2 = (int)Math.Round(parsedNumbers[3]);

			Point point1 = new(x1, y1);
			Point point2 = new(x2, y2);
			return Tuple.Create(point1, point2);
		}

		public static double ProperlyParseDouble(string input)
		{
			if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
			{
				return result;
			}
			return double.NaN;
		}

		public static float ProperlyParseFloat(string input)
		{
			if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
			{
				return result;
			}
			return float.NaN;
		}

		public static string ResolutionIdxToString(int idx)
		{
			return idx switch
			{
				0 => "1280x720",
				1 => "1920x1080",
				2 => "2560x1440",
				3 => "3840x2160",
				_ => "1920x1080", //Invalid idx, just give 1080p, idk
			};
		}

		public static string ConvertGameFilesPath(string path, string smPath)
		{
			return path.Replace("$GAME_DATA", Path.Combine(smPath, "Data"));
		}

		public static bool IsValidPath(string path, bool checkRW = false)
		{
			//Check if the path is well-formed
			if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
			{
				return false;
			}

			//Check if the directory exists
			if (!Directory.Exists(path))
			{
				return false;
			}

			if (checkRW) //Check for read/write access
			{
				try
				{
					string testFile = Path.Combine(path, "tempfile.tmp");
					File.WriteAllText(testFile, "test");
					File.Delete(testFile);
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
					using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
					{
						// Optionally, you can write and delete a temporary file to test permissions
						string testFile = Path.GetTempFileName();
						using (StreamWriter sw = new StreamWriter(testFile))
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

		public static string AppendToFile(string filePath, string appendant)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			string extension = Path.GetExtension(filePath);
			string directory = Path.GetDirectoryName(filePath);
			string newFileName = fileNameWithoutExtension + appendant + extension;
			return Path.Combine(directory, newFileName);
		}

		public static string? FindFileInSubDirs(string directory, string fileName)
		{
			if (fileName == null || fileName == "")
			{
				return null;
			}
			try
			{
				foreach (string file in Directory.EnumerateFiles(directory, fileName, SearchOption.AllDirectories))
				{
					return file;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while searching for file '{fileName}'!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"An error occurred while searching for file '{fileName}'!\n{ex.Message}");
			}
			return null;
		}

		public static Color? ParseColorFromString(string colorString, bool throwOnError = true)
		{
			// Split the string by spaces
			if (string.IsNullOrWhiteSpace(colorString))
			{
				return null;
			}
			string[] parts = colorString.Split(' ');
			if (parts.Length == 1)
			{
				if (!parts[0].StartsWith('#'))
				{
					parts[0] = "#" + parts[0];
				}
				if (parts[0].Length != 7)
				{
					if (throwOnError)
					{
						throw new FormatException("Color string must be in the format 'r g b' or '#rrggbb'");
					}
					else
					{
						return null;
					}
				}
				return HexStringToColor(parts[0]);
			}
			if (parts.Length != 3)
			{
				if (throwOnError)
				{
					throw new FormatException("Color string must be in the format 'r g b' or '#rrggbb'");
				}
				else
				{
					return null;
				}
			}

			// Parse each component with InvariantCulture and scale from [0, 1] to [0, 255]
			int r = (int)(double.Parse(parts[0], CultureInfo.InvariantCulture) * 255);
			int g = (int)(double.Parse(parts[1], CultureInfo.InvariantCulture) * 255);
			int b = (int)(double.Parse(parts[2], CultureInfo.InvariantCulture) * 255);

			return Color.FromArgb(r, g, b);
		}

		public static string ColorToString(Color color)
		{
			// Convert each component to a 0-1 double, using InvariantCulture for full precision
			double r = color.R / 255.0;
			double g = color.G / 255.0;
			double b = color.B / 255.0;

			return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", r, g, b);
		}

		public static Color? HexStringToColor(string color)
		{

			if (!Regex.IsMatch(color, @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$"))
			{
				return null;
			}

			// Get the HTML color string from ColorTranslator
			Color htmlColor = Color.Empty;
			try
			{
				htmlColor = ColorTranslator.FromHtml(color);
			}
			catch (Exception)
			{
				//Do nothing, as it doesnt matter
			}

			// If the result is a named color (e.g., "White"), convert it to hex manually
			return htmlColor != Color.Empty ? htmlColor : null;
		}

		public static string ColorToHexString(Color color)
		{
			// Get the HTML color string from ColorTranslator
			string htmlColor = ColorTranslator.ToHtml(color);

			// If the result is a named color (e.g., "White"), convert it to hex manually
			if (!htmlColor.StartsWith("#"))
			{
				return $"{color.R:X2}{color.G:X2}{color.B:X2}";
			}

			// Otherwise, it's already in hex format, so just remove the #
			return htmlColor.Substring(1);
		}

		public static string ReplaceInvalidChars(string input, string allowedChars)
		{
			char[] result = new char[input.Length];

			for (int i = 0; i < input.Length; i++)
			{
				result[i] = allowedChars.Contains(input[i]) ? input[i] : '\u25AF';
			}

			return new string(result);
		}
		#endregion
	}
}
