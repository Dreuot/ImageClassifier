using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImageClassifier
{
    class Repo
    {
        private string dataPath;
        public List<string> TrackingDirs { get; set; }
        public List<Category> categories;
        public List<ImageDescriptor> Images { get; set; }

        public Repo(string dataPath, IEnumerable<string> trackingDirs)
        {
            this.dataPath = dataPath;
            TrackingDirs = trackingDirs.ToList();
            Images = new List<ImageDescriptor>();
            //Load();
            foreach (var item in TrackingDirs)
            {
                Read(item);
            }
        }

        private void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImageDescriptor));
            using (FileStream writter = new FileStream(dataPath, FileMode.Create))
            {
                serializer.Serialize(writter, Images);
            }
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImageDescriptor));
            try
            {
                using (FileStream writter = new FileStream(dataPath, FileMode.Open))
                {
                    Images = (List<ImageDescriptor>)serializer.Deserialize(writter);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            
        }

        private void Read(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                var files = dir.GetFiles("*.jpg", SearchOption.AllDirectories);
                files.Concat(dir.GetFiles("*.jpeg", SearchOption.AllDirectories));
                files.Concat(dir.GetFiles("*.png", SearchOption.AllDirectories));
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
            var category = categories.FirstOrDefault(c => c.Name == name);
            if (category == null)
            {
                category = new Category(name);
                categories.Add(category);
                Save();
            }

            return category;
        }

        public void AddCategoryAtPicture(ImageDescriptor image, Category category)
        {
            image.AddCategory(category);
        }

        public void AddCategoryAtPicture(ImageDescriptor image, string categoryName)
        {
            var category = GetCategory(categoryName);
            image.AddCategory(category);
        }
    }
}
