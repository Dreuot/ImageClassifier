using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ImageClassifier
{
    [Serializable]
    class Category
    {
        [Display(Name = "Категория")]
        public string Name { get; set; }

        public Category(string name)
        {
            Name = name;
        }
    }
}
