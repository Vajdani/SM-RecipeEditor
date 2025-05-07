using Newtonsoft.Json;

namespace SM_RecipeEditor
{
    public partial class Main : Form
    {
        private string ModsPath = "";
        public string ModPath = "";
        public string GamePath = "";
        public Dictionary<string, ItemDescription> itemDescriptions = [];

        public static Main Instance { get; private set; }

        public Main()
        {
            Instance = this;

            InitializeComponent();

            p_main.Visible = false;

            string userID = Util.GetLoggedInSteamUserID();
            if (userID == null)
            {
                MessageBox.Show("poopoo steam", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            else
            {
                ModsPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + $"\\Roaming\\Axolot Games\\Scrap Mechanic\\User\\User_{userID}\\Mods\\";
                foreach (string mod in Directory.GetDirectories(ModsPath))
                {
                    cb_mod.Items.Add(mod.Replace(ModsPath, ""));
                }

                GamePath = Util.GetGameInstallPath("387990")!;
                ParseDescriptions(GamePath + "\\Data\\Gui\\Language\\English\\InventoryItemDescriptions.json");
                ParseDescriptions(GamePath + "\\Survival\\Gui\\Language\\English\\inventoryDescriptions.json");
            }
        }

        private void OnModSelected(object sender, EventArgs e)
        {
            RefreshRecipeFileList();
            ParseDescriptions(ModPath + "\\Gui\\Language\\English\\inventoryDescriptions.json");
        }

        private void OnFileSelected(object sender, EventArgs e)
        {
            listb_recipeFiles.Items.Clear();

            List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(
                File.ReadAllText(ModPath + $"CraftingRecipes\\{cb_file.Items[cb_file.SelectedIndex]}")
            )!;

            foreach (Recipe recipe in recipes)
            {
                listb_recipeFiles.Items.Add(GetItemName(recipe.itemId));
            }

            listb_recipeFiles.Visible = true;
        }

        private void bt_new_Click(object sender, EventArgs e)
        {
            new NewRecipeFile().ShowDialog();
        }

        public void OnRecipeFileCreated()
        {
            RefreshRecipeFileList();
        }

        private void OnRecipeSelected(object sender, EventArgs e)
        {

        }




        private void ParseDescriptions(string path)
        {
            Dictionary<string, ItemDescription> _itemDescriptions = JsonConvert.DeserializeObject<Dictionary<string, ItemDescription>>(
                File.ReadAllText(path)
            )!;
            foreach (var item in _itemDescriptions)
            {
                itemDescriptions.TryAdd(item.Key, item.Value);
            }
        }

        private void RefreshRecipeFileList()
        {
            cb_file.Items.Clear();
            p_main.Visible = true;
            listb_recipeFiles.Visible = false;

            string value = cb_mod.Items[cb_mod.SelectedIndex]!.ToString()!;
            ModPath = $"{ModsPath}{value}\\";
            string recipesPath = ModPath + "CraftingRecipes\\";
            if (!Directory.Exists(recipesPath))
            {
                Directory.CreateDirectory(recipesPath);
            }

            string[] files = Directory.GetFiles(recipesPath);
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    cb_file.Items.Add(file.Replace(recipesPath, ""));
                }
            }
        }

        private string GetItemName(string uuid)
        {
            if (itemDescriptions.TryGetValue(uuid, out var itemDescription))
            {
                return itemDescription.title;
            }

            return uuid;
        }
    }
}
