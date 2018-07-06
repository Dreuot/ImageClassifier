using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageClassifier;
using System.Collections.Generic;
using System.Linq;

namespace ImageClassifierTest
{
    [TestClass]
    public class UnitTest1
    {
        private List<string> tracked = new List<string>() { @"C:\Users\akarpov\Desktop\Images" };

        [TestMethod]
        public void CreateRepo()
        {
            Repo repo = new Repo("data.xml", "cat.xml", tracked);
            repo.Save();

            var count = repo.Images.Count;

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void LoadRepo()
        {
            Repo repo = new Repo("data.xml", "cat.xml", tracked);
            repo.Load();

            var count = repo.Images.Count;

            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void AddCategory()
        {
            Repo repo = new Repo("data.xml", "cat.xml", tracked);

            var im = repo.Images[0];
            repo.AddCategoryToPicture(im, "Проверка");
            repo.Save();

            var cat = repo.Images[0].Categories.FirstOrDefault(n => n.Name == "Проверка");

            Assert.IsNotNull(cat);
        }

        [TestMethod]
        public void GetCategory()
        {
            Repo repo = new Repo("data.xml", "cat.xml", tracked);

            var im = repo.Images[0];
            repo.AddCategoryToPicture(im, "ТЕСТ");
            repo.Save();

            var cat = repo.Categories.FirstOrDefault(n => n.Name == "ТЕСТ");

            Assert.IsNotNull(cat);
        }
    }
}
