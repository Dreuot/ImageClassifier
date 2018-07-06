using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImageClassifier
{
    public class Repo
    {
        private string dataPath;
        private string catPath;
        public List<string> TrackingDirs { get; set; }
        public List<Category> Categories;
        public List<ImageDescriptor> Images { get; set; }

        public Repo(string dataPath, string catPath, IEnumerable<string> trackingDirs)
        {
            this.dataPath = dataPath;
            this.catPath = catPath;
            TrackingDirs = trackingDirs.ToList();
            Images = new List<ImageDescriptor>();
            Load();
            //Reload();
        }

        private void Reload()
        {
            Images = new List<ImageDescriptor>();
            foreach (var item in TrackingDirs)
            {
                Read(item);
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ImageDescriptor>));
                using (FileStream writter = new FileStream(dataPath, FileMode.Create))
                {
                    serializer.Serialize(writter, Images);
                }

                serializer = new XmlSerializer(typeof(List<Category>));
                using (FileStream stream = new FileStream(catPath, FileMode.Create))
                {
                    serializer.Serialize(stream, Categories);
                }
            }
            catch (Exception ex) 
            {

                throw;
            }
            
        }

        public void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ImageDescriptor>));
            try
            {
                using (FileStream writter = new FileStream(dataPath, FileMode.OpenOrCreate))
                {
                    Images = (List<ImageDescriptor>)serializer.Deserialize(writter) ?? new List<ImageDescriptor>();
                }
            }
            catch (Exception)
            {
                Images = new List<ImageDescriptor>();
            }

            try
            {
                serializer = new XmlSerializer(typeof(List<Category>));
                using (FileStream stream = new FileStream(catPath, FileMode.Open))
                {
                    Categories = (List<Category>)serializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                Categories = new List<Category>();
            }
        }

        private void Read(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                var files = dir.GetFiles("*.jpg", SearchOption.AllDirectories);
                files = files.Concat(dir.GetFiles("*.jpeg", SearchOption.AllDirectories)).ToArray();
                files = files.Concat(dir.GetFiles("*.png", SearchOption.AllDirectories)).ToArray();
                foreach (var file in files)
                {
                    ImageDescriptor img = new ImageDescriptor(file.FullName, path, file.Name);
                    img.LoadImage();
                    Images.Add(img);
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private void RemoveUntracked(string path)
        {

        }

        public void AddTracking(string path)
        {
            TrackingDirs.Add(path);
            Read(path);
        }

        public void RemoveWatching(string path)
        {
            TrackingDirs.Remove(path);
        }

        public Category GetCategory(string name)
        {
            var category = Categories.FirstOrDefault(c => c.Name == name);
            if (category == null)
            {
                category = new Category(name);
                Categories.Add(category);
                Save();
            }

            return category;
        }

        public void AddCategoryToPicture(ImageDescriptor image, Category category)
        {
            image.AddCategory(category);
        }

        public void AddCategoryToPicture(ImageDescriptor image, string categoryName)
        {
            var category = GetCategory(categoryName);
            image.AddCategory(category);
        }
    }
}
