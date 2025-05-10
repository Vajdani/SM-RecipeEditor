namespace SM_RecipeEditor
{

	internal class RecipeItem
	{
		public string itemId = "";
		public float quantity;
	}

    internal class Recipe
    {
		public string itemId = "";
		public List<RecipeItem>? extras;
		public float quantity;
		public float craftTime;
		public List<RecipeItem> ingredientList = [];
    }
}
