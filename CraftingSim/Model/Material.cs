using System;

namespace CraftingSim.Model
{
    public class Material : IMaterial
    {
        public int Id { get; }
        public string Name { get; }

        public Material(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            
            Id = id;
            Name = name;
        }

        public bool Equals(IMaterial other)
        {
            if (other == null)
                return false;

            // Verifica se existem semelhanças por Id ou Name (case insensitive)
            return this.Id == other.Id || 
                   string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IMaterial);
        }

        public override int GetHashCode()
        {
            // Combina id and hashcodes
            int hashId = Id.GetHashCode();
            int hashName = StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            return hashId ^ hashName;
        }

        public override string ToString()
        {
            return $"Material(Id={Id}, Name={Name})";
        }
    }
}