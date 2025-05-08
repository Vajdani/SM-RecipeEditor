using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace SM_RecipeEditor
{
    public partial class Main : Form
    {
        private string ModsPath = "";
        public string ModPath = "";
        private string ModName = "";
        private string GamePath = "";
        private int LastRecipeIndex = -1;
        private bool RecipeHasChanged = false;
        private bool RecipeSetupInProgress = false;
        private Dictionary<string, Recipe> CurrentRecipes = [];
        private Dictionary<string, ItemDescription> itemDescriptions = [];
        private Dictionary<string, string> itemNameToUUID = [];

        public static Main? Instance { get; private set; }

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
                    if (File.Exists(mod + "\\Gui\\Language\\English\\inventoryDescriptions.json"))
                    {
                        cb_mod.Items.Add(mod.Replace(ModsPath, ""));
                    }
                }

                GamePath = Util.GetGameInstallPath("387990")!;
                ParseDescriptions(GamePath + "\\Data\\Gui\\Language\\English\\InventoryItemDescriptions.json", "Creative");
                ParseDescriptions(GamePath + "\\Survival\\Gui\\Language\\English\\inventoryDescriptions.json", "Survival");
            }
        }

        private void OnModSelected(object sender, EventArgs e)
        {
            LastRecipeIndex = -1;
            RecipeHasChanged = false;

            RefreshRecipeFileList();
            ParseDescriptions(ModPath + "\\Gui\\Language\\English\\inventoryDescriptions.json", ModName);
        }

        private void OnFileSelected(object sender, EventArgs e)
        {
            RefreshRecipeList();
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
            int id = listb_recipeFiles.SelectedIndex;
            if (id == -1 || id == LastRecipeIndex) { return; }

            if (RecipeHasChanged)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes, would you like to save them?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveChanges();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            RecipeSetupInProgress = true;

            LastRecipeIndex = id;
            if (id == listb_recipeFiles.Items.Count - 1)
            {
                listb_recipeFiles.Items[id] = "New recipe";
                listb_recipeFiles.Items.Add("-- Add new recipe --");
            }

            cb_item.Items.Clear();

            string item = listb_recipeFiles.Items[id]!.ToString()!;
            if (CurrentRecipes.TryGetValue(item, out Recipe? recipe))
            {
                tx_quantity.Text = recipe.quantity.ToString();
                tx_craftTime.Text = recipe.craftTime.ToString();

                int i = -1;
                foreach (var pair in itemNameToUUID)
                {
                    string name = pair.Key;
                    cb_item.Items.Add(name);

                    i++;
                    if (name == item)
                    {
                        cb_item.SelectedIndex = i;
                    }
                }
            }
            else
            {
                tx_quantity.Text = "0";
                tx_craftTime.Text = "0";

                cb_item.Items.Add("");
                foreach (var pair in itemNameToUUID)
                {
                    cb_item.Items.Add(pair.Key);
                }

                cb_item.SelectedIndex = 0;
            }

            RecipeSetupInProgress = false;
            
            RecipeHasChanged = false;
            bt_save.Visible = false;

            p_edit.Visible = true;
        }

        private void RecipeItemChanged(object sender, EventArgs e)
        {
            if (RecipeSetupInProgress) { return; }

            RecipeHasChanged = true;
            bt_save.Visible = true;
        }

        private void RecipeNumberChanged(object sender, EventArgs e)
        {
            if (RecipeSetupInProgress) { return; }
           
            TextBox box = (sender as TextBox)!;
            if (!int.TryParse(box.Text, out int _))
            {
                MessageBox.Show("This recipe property must be a whole number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                box.Text = "";
            }
            else
            {
                RecipeHasChanged = true;
                bt_save.Visible = true;
            }
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }




        private void SaveChanges()
        {
            RecipeHasChanged = false;
            bt_save.Visible = false;

            StreamWriter sw = new(ModPath + $"CraftingRecipes\\{cb_file.Items[cb_file.SelectedIndex]}");
            JsonTextWriter writer = new(sw)
            {
                IndentChar = '\t',
                Indentation = 1
            };

            JsonSerializer serializer = new()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            string itemName = listb_recipeFiles.Items[listb_recipeFiles.SelectedIndex].ToString()!;
            bool isNew = false;
            if (!CurrentRecipes.TryGetValue(itemName, out Recipe? modified))
            {
                modified = new Recipe();
                isNew = true;
            }

            modified.itemId = itemNameToUUID[cb_item.Items[cb_item.SelectedIndex]!.ToString()!];
            modified.craftTime = int.Parse(tx_craftTime.Text);
            modified.quantity = int.Parse(tx_quantity.Text);

            List<Recipe> recipes = [];
            if (isNew)
            {
                recipes.Add(modified);
            }

            foreach (var item in CurrentRecipes)
            {
                recipes.Add(item.Value);
            }

            serializer.Serialize(writer, recipes);
            
            writer.Close();
            sw.Close();

            RefreshRecipeList();
        }

        private void ParseDescriptions(string path, string source)
        {
            Dictionary<string, ItemDescription> _itemDescriptions = JsonConvert.DeserializeObject<Dictionary<string, ItemDescription>>(
                File.ReadAllText(path)
            )!;
            foreach (var item in _itemDescriptions)
            {
                itemDescriptions.TryAdd(item.Key, item.Value);

                string name = item.Value.title;
                if (!itemNameToUUID.TryAdd(name, item.Key))
                {
                    itemNameToUUID.TryAdd($"{name}({source})", item.Key); //Thanks Craftbot for TryAdd
                }
            }
        }

        private void RefreshRecipeFileList()
        {
            cb_file.Items.Clear();
            p_main.Visible = true;
            p_edit.Visible = false;
            listb_recipeFiles.Visible = false;

            string value = cb_mod.Items[cb_mod.SelectedIndex]!.ToString()!;
            ModName = value;
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

        private void RefreshRecipeList()
        {
            listb_recipeFiles.Items.Clear();

            CurrentRecipes = [];
            List<Recipe> recipes = JsonConvert.DeserializeObject<List<Recipe>>(
                File.ReadAllText(ModPath + $"CraftingRecipes\\{cb_file.Items[cb_file.SelectedIndex]}")
            )!;

            foreach (Recipe recipe in recipes)
            {
                string name = GetItemName(recipe.itemId);
                listb_recipeFiles.Items.Add(name);
                CurrentRecipes.Add(name, recipe);
            }
            listb_recipeFiles.Items.Add("-- Add new recipe --");

            listb_recipeFiles.Visible = true;
            bt_save.Visible = false;
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
