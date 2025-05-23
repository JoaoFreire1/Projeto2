using System.Collections.Generic;
using System.IO;

namespace CraftingSim.Model
{
    /// <summary>
    /// Inventory of materials and quantities the user has
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Dictionary that contains all the materials the user has
        /// and the respective amount
        /// </summary>
        private readonly Dictionary<IMaterial, int> materials;

        public Inventory()
        {
            materials = new Dictionary<IMaterial, int>();
        }

        /// <summary>
        /// Provides all materials in the inventory.
        /// </summary>
        public IEnumerable<IMaterial> Materials => materials.Keys;

        // <summary>
        /// provides the amount of the specified material in the inventory.
        /// </summary>
        /// <param name="material">The material to view corresponding amount.</param>
        /// <returns>The quantity in inventory, or 0 if not contained</returns>
        public int GetQuantity(IMaterial material) => materials.ContainsKey(material) ? materials[material] : 0;


        /// <summary>
        /// Adds or replaces the quantity for a specific material
        /// </summary>
        /// <param name="material">The material to add</param>
        /// <param name="quantity">The new amount to set</param>
        public void AddMaterial(IMaterial material, int quantity)
        {
            if (material == null || quantity <= 0)
                return;

            // Verifica se já existe um material equivalente (ou por ID ou pelo nome)
            IMaterial existing = GetMaterial(material.Id);
            if (existing == null)
            {
                // Nenhum material com mesmo Id - procurar por nome
                foreach (var m in materials.Keys)
                {
                    if (m.Equals(material))
                    {
                        existing = m;
                        break;
                    }
                }
            }

            if (existing != null)
            {
                materials[existing] += quantity;
            }
            else
            {
                materials[material] = quantity;
            }
        }

        /// <summary>
        /// Removes a given amount of a material from inventory
        /// If theres not enough material it is not removed.
        /// </summary>
        /// <param name="material">The material we want to remove from</param>
        /// <param name="quantity">The amount to remove</param>
        /// <returns>True if removed successfuly, false if not enough material</returns>
        public bool RemoveMaterial(IMaterial material, int quantity)
        {
            if (material == null || quantity <= 0)
                return false;

            IMaterial existing = GetMaterial(material.Id);
            if (existing == null)
            {
                foreach (var m in materials.Keys)
                {
                    if (m.Equals(material))
                    {
                        existing = m;
                        break;
                    }
                }
            }

            if (existing != null && materials[existing] >= quantity)
            {
                materials[existing] -= quantity;
                if (materials[existing] == 0)
                    materials.Remove(existing);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all the materials the user has in the inventory.
        /// </summary>
        /// <returns>A read only dictionary of materials</returns>
        public IReadOnlyDictionary<IMaterial, int> GetAllMaterials()
        {
            return materials;
        }

        /// <summary>
        /// Search and return a material by the Id.
        /// </summary>
        /// <param name="id">The material id</param>
        /// <returns>The material if it's in the inventory, if not returns null
        /// </returns>
        public IMaterial GetMaterial(int id)
        {
            foreach (IMaterial m in materials.Keys)
                if (m.Id == id)
                    return m;

            return null;
        }

        /// <summary>
        /// Loads the materials and their quantities from the text file.
        /// </summary>
        /// <param name="file">Path to the materials file</param>
        public void LoadMaterialsFromFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException($"File not found: {file}");

            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length != 3)
                    continue;

                if (!int.TryParse(parts[0], out int id))
                    continue;

                string name = parts[1].Trim();

                if (!int.TryParse(parts[2], out int quantity))
                    continue;

                var material = new Material(id, name);
                AddMaterial(material, quantity);
            }
        }
    }
}
