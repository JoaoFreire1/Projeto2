using System;
using System.Collections.Generic;


namespace CraftingSim.Model
{
    /// <summary>
    /// Implementation of ICrafter. 
    /// </summary>
    public class Crafter : ICrafter
    {
        private readonly Inventory inventory;
        private readonly List<IRecipe> recipeList;

        public Crafter(Inventory inventory)
        {
            this.inventory = inventory;
            recipeList = new List<IRecipe>();
        }

        /// <summary>
        /// returns a read only list of loaded recipes.
        /// </summary>
        public IEnumerable<IRecipe> RecipeList => recipeList;

        /// <summary>
        /// Loads recipes from the files.
        /// Must parse the name, success rate, required materials and
        /// necessary quantities.
        /// </summary>
        /// <param name="recipeFiles">Array of file paths</param>
        public void LoadRecipesFromFile(string[] recipeFiles)
        {
            foreach (string file in recipeFiles)
            {
                if (!File.Exists(file))
                    continue;

                string[] lines = File.ReadAllLines(file);
                if (lines.Length < 2)
                    continue;

                // Primeira linha: <nome>,<probabilidade>
                string[] headerParts = lines[0].Split(',');
                if (headerParts.Length != 2)
                    continue;

                string itemName = headerParts[0].Trim();
                if (!double.TryParse(headerParts[1], out double successRate))
                    continue;

                Dictionary<IMaterial, int> requiredMaterials = new Dictionary<IMaterial, int>();

                // Linhas seguintes: <id>,<quantidade>
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] parts = lines[i].Split(',');
                    if (parts.Length != 2)
                        continue;

                    if (!int.TryParse(parts[0], out int materialId))
                        continue;

                    if (!int.TryParse(parts[1], out int quantity))
                        continue;

                    IMaterial material = inventory.GetMaterial(materialId);
                    if (material == null)
                    {
                        // Se o material não estiver no inventário, criar um temporário com nome vazio (ou opcional)
                        // Para isso precisamos de acesso ao nome do material, que não temos aqui.
                        // Melhor ignorar essa receita? Ou criar um material apenas com Id?

                        // Vamos criar um material temporário só com Id e um nome placeholder:
                        material = new Material(materialId, $"Material{materialId}");
                    }

                    if (requiredMaterials.ContainsKey(material))
                        requiredMaterials[material] += quantity;
                    else
                        requiredMaterials[material] = quantity;
                }

                // Criar a receita e adicionar na lista
                IRecipe recipe = new Recipe(itemName, successRate, requiredMaterials);
                recipeList.Add(recipe);
            }

            // Ordenar recipeList pelo nome (case insensitive)
            recipeList.Sort((r1, r2) => string.Compare(r1.Name, r2.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Attempts to craft an item from a given recipe. Consumes inventory 
        /// materials and returns the result message.
        /// </summary>
        /// <param name="recipeName">Name of the recipe to craft</param>
        /// <returns>A message indicating success, failure, or error</returns>
        public string CraftItem(string recipeName)
        {
            IRecipe selected = null;

            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Name.Equals(recipeName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    selected = recipeList[i];
                    break;
                }
            }
            
            if (selected == null)
                return "Recipe not found.";

            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
            {
                IMaterial material = required.Key;
                int need = required.Value;
                int have = inventory.GetQuantity(material);

                if (have < need)
                {
                    if (have == 0)
                    {
                        return "Missing material: " + material.Name;
                    }
                    return "Not enough " + material.Name +
                           " (need " + need +
                           ", have " + have + ")";
                }
            }

            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
                if (!inventory.RemoveMaterial(required.Key, required.Value))
                    return "Not enough materials";

            Random rng = new Random();
            if (rng.NextDouble() < selected.SuccessRate)
                return "Crafting '" + selected.Name + "' succeeded!";
            else
                return "Crafting '" + selected.Name + "' failed. Materials lost.";

        }
    }
}