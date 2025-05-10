using Newtonsoft.Json;

namespace SM_RecipeEditor
{
    public partial class NewRecipeFile : Form
    {
        public NewRecipeFile()
        {
            InitializeComponent();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnOKClick(object sender, EventArgs e)
        {
            if (tx_name.Text == "")
            {
                MessageBox.Show("No file name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath = Main.Instance!.ModPath + $"CraftingRecipes\\{tx_name.Text}.json";
            if (File.Exists(filePath) && 
                MessageBox.Show("This file already exists. Do you want to replace it?", "File conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                tx_name.Text = "";
                return;
            }

            List<Recipe> recipes = [ //Connection Tool example
                new Recipe()
                {
                    itemId = "8c7efc37-cd7c-4262-976e-39585f8527bf",
                    quantity = 1,
                    craftTime = 32,
                    ingredientList = [
                        new RecipeItem()
                        {
                            quantity = 10,
                            itemId = "1f7ac0bb-ad45-4246-9817-59bdf7f7ab39"
                        },
                        new RecipeItem()
                        {
                            quantity = 2,
                            itemId = "f152e4df-bc40-44fb-8d20-3b3ff70cdfe3"
                        }
                    ]
                }
            ];

            using StreamWriter sw = new(filePath);
            using JsonTextWriter writer = new(sw)
            { 
                IndentChar = '\t',
                Indentation = 1
            };

            JsonSerializer serializer = new()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            serializer.Serialize(writer, recipes);

            Main.Instance.OnRecipeFileCreated();
            Close();
        }
    }
}
