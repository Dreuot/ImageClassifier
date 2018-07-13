using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ImageClassifier
{
    [Serializable]
    public class Category
    {
        [Display(Name = "Категория")]
        public string Name { get; set; }

        public Category()
        {
        }

        public Category(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            Category cat = obj as Category;
            return Name == cat.Name;
        }

        public static bool operator ==(Category left, Category right)
        {
            return left?.Name == right?.Name;
        }

        public static bool operator !=(Category left, Category right)
        {
            return left?.Name != right?.Name;
        }
    }
}
