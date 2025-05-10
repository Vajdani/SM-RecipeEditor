using Newtonsoft.Json;

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
        private List<Recipe> CurrentRecipes = [];
        private Dictionary<string, ItemDescription> itemDescriptions = [];
        private Dictionary<string, string> itemNameToUUID = [];
        private Dictionary<string, int> itemNameCount = [];

        public static Main? Instance { get; private set; }

        public Main()
        {
            Instance = this;

            InitializeComponent();

            p_main.Visible = false;

            string? userID = Util.GetLoggedInSteamUserID();
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

                string? path = Util.GetGameInstallPath("387990");
                if (path == null)
                {
                    MessageBox.Show("SM game files not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                GamePath = path;
                ParseDescriptions(GamePath + "\\Data\\Gui\\Language\\English\\InventoryItemDescriptions.json");
                ParseDescriptions(GamePath + "\\Survival\\Gui\\Language\\English\\inventoryDescriptions.json");
            }
        }

        private void OnModSelected(object sender, EventArgs e)
        {
            LastRecipeIndex = -1;
            RecipeHasChanged = false;

            RefreshRecipeFileList();
            ParseDescriptions(ModPath + "\\Gui\\Language\\English\\inventoryDescriptions.json");
        }

        private void OnFileSelected(object sender, EventArgs e)
        {
            RefreshRecipeList();
        }

        private void OnNewRecipeClick(object sender, EventArgs e)
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
            Recipe? recipe = GetRecipeByName(item);
            if (recipe == null)
            {
                tx_quantity.Text = "0";
                tx_craftTime.Text = "0";

                cb_item.Items.Add("");
                foreach (var pair in itemNameToUUID)
                {
                    cb_item.Items.Add(pair.Key);
                }

                cb_item.SelectedIndex = 0;

                dgv_ingredients.Rows.Clear();
            }
            else
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

                dgv_ingredients.Rows.Clear();
                foreach (RecipeItem ingredient in recipe.ingredientList)
                {
                    dgv_ingredients.Rows.Add(GetItemName(ingredient.itemId), ingredient.quantity);
                }

                dgv_extras.Rows.Clear();
                if (recipe.extras != null)
                {
                    foreach (RecipeItem extra in recipe.extras)
                    {
                        dgv_extras.Rows.Add(GetItemName(extra.itemId), extra.quantity);
                    }
                }
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

        private void OnSaveRecipeClick(object sender, EventArgs e)
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

            string itemName = listb_recipeFiles.Items[LastRecipeIndex].ToString()!;
            bool isNew = false;
            Recipe? modified = GetRecipeByName(itemName);
            if (modified == null)
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
                recipes.Add(item);
            }

            serializer.Serialize(writer, recipes);
            
            writer.Close();
            sw.Close();

            RefreshRecipeList();
        }

        private void ParseDescriptions(string path)
        {
            Dictionary<string, ItemDescription> _itemDescriptions = JsonConvert.DeserializeObject<Dictionary<string, ItemDescription>>(
                File.ReadAllText(path)
            )!;
            foreach (var item in _itemDescriptions)
            {
                string originalName = item.Value.title;
                string name = originalName;
                if (itemNameToUUID.ContainsKey(name))
                {
                    if (!itemNameCount.ContainsKey(originalName))
                    {
                        itemNameCount.Add(originalName, 1);
                    }

                    name = $"{name} #{itemNameCount[originalName]}";
                    itemNameCount[originalName]++;
                }

                item.Value.title = name;

                itemDescriptions.TryAdd(item.Key, item.Value);
                itemNameToUUID.TryAdd(name, item.Key);
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
                listb_recipeFiles.Items.Add(GetItemName(recipe.itemId));
                CurrentRecipes.Add(recipe);
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

        private Recipe? GetRecipeByName(string name)
        {
            if (!itemNameToUUID.TryGetValue(name, out var uuid))
            {
                return null;
            }

            foreach (Recipe recipe in CurrentRecipes)
            {
                if (uuid == recipe.itemId)
                {
                    return recipe;
                }
            }

            return null;
        }
    }
}
