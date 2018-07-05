using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;

namespace ImageClassifier
{
    [Serializable]
    class ImageDescriptor
    {
        [XmlIgnore]
        public Image Picture { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string TrackingDir { get; set; }
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Для сериализации
        /// </summary>
        public ImageDescriptor()
        {

        }

        /// <summary>
        /// Для полноценного создания
        /// </summary>
        /// <param name="path">Путь к изображению</param>
        /// <param name="name">Внутреннее название изображения</param>
        public ImageDescriptor(string path, string trackingDir, string name = "")
        {
            Path = path;
            Name = name;
            TrackingDir = trackingDir;
            Categories = new List<Category>();
            Picture = Image.FromFile(path);
        }

        public void LoadImage()
        {
            Picture = Image.FromFile(Path);
        }

        public void AddCategory(Category category)
        {
            Categories.Add(category);
        }

        public void AddCategoryRange(IEnumerable<Category> categories)
        {
            Categories.AddRange(categories);
        }

        public void RemoveCategory(Category category)
        {
            Categories.Remove(category);
        }

        public void RemoveCategory(string name)
        {
            Category category = Categories.FirstOrDefault(c => c.Name == name);
            if (category != null)
                Categories.Remove(category);
        }
    }
}
