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
        public List<Category> Categories { get; set; }
        public List<ImageDescriptor> Images { get; set; }

        public Repo(string dataPath, string catPath, IEnumerable<string> trackingDirs)
        {
            this.dataPath = dataPath;
            this.catPath = catPath;
            TrackingDirs = trackingDirs.ToList();
            Images = new List<ImageDescriptor>();
            Load();
        }

        private void Reload()
        {
            Images = new List<ImageDescriptor>();
            foreach (var item in TrackingDirs)
            {
                ReadTrackedDir(item);
            }
        }

        public List<ImageDescriptor> GetImages(params Category[] categories)
        {
            if (categories.Length == 0)
                return Images;

            List<ImageDescriptor> result = new List<ImageDescriptor>();
            bool have = true;
            foreach (var image in Images)
            {
                foreach (var category in categories)
                    if (image.Categories.All(c => c != category))
                        have = false;

                if (have)
                    result.Add(image);

                have = true;
            }

            return result;
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

        private void ReadTrackedDir(string path)
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
                    Images.Add(img);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RenameImage(ImageDescriptor img, string name)
        {
            img.Name = name;
            Save();
        }

        private void RemoveUntracked(string path)
        {
            for (int i = 0; i < Images.Count; i++)
            {
                ImageDescriptor current = Images[i];
                if (!TrackingDirs.Any(n => n == current.TrackingDir))
                {
                    Images.Remove(current);
                }
            }
        }

        public void AddTracking(string path)
        {
            TrackingDirs.Add(path);
            ReadTrackedDir(path);
            Save();
        }

        public void RemoveWatching(string path)
        {
            TrackingDirs.Remove(path);
            RemoveUntracked(path);

            Save();
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
            Save();
        }

        public void AddCategoryToPicture(ImageDescriptor image, string categoryName)
        {
            var category = GetCategory(categoryName);
            image.AddCategory(category);
            Save();
        }

        public void RemoveCategoryFromPicture(ImageDescriptor image, Category category)
        {
            image.RemoveCategory(category);
            Save();
        }

        public void RemoveCategoryFromPicture(ImageDescriptor image, string categoryName)
        {
            var category = Categories.FirstOrDefault(n => n.Name == categoryName);
            if (category != null)
            {
                image.RemoveCategory(category);
                Save();
            }
        }
    }
}
