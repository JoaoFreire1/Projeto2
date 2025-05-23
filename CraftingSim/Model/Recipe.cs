using System;
using System.Collections.Generic;

namespace CraftingSim.Model
{
    public class Recipe : IRecipe
    {
        public string Name { get; }
        public double SuccessRate { get; }
        private readonly Dictionary<IMaterial, int> requiredMaterials;

        public IReadOnlyDictionary<IMaterial, int> RequiredMaterials => requiredMaterials;

        public Recipe(string name, double successRate, Dictionary<IMaterial, int> requiredMaterials)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SuccessRate = successRate;
            this.requiredMaterials = requiredMaterials ?? new Dictionary<IMaterial, int>();
        }

        public int CompareTo(IRecipe other)
        {
            if (other == null) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"Recipe(Name={Name}, SuccessRate={SuccessRate}, Materials={requiredMaterials.Count})";
        }
    }
}