using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using Microsoft.Win32;

namespace ImageClassifier
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Repo repo;
        Configuration config;

        public MainWindow()
        {
            InitializeComponent();

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string dataPath = config.AppSettings.Settings["dataPath"].Value;
            string catPath = config.AppSettings.Settings["catPath"].Value;
            List<string> tracking = new List<string>();
            var configKeys = config.AppSettings.Settings.AllKeys.Where(key => key.IndexOf("tracking") == 0);
            foreach (var key in configKeys)
            {
                tracking.Add(config.AppSettings.Settings[key].Value);
            }

            repo = new Repo(dataPath, catPath, tracking);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                if (!repo.TrackingDirs.Any(dir => dir == path))
                {
                    int.TryParse(config.AppSettings.Settings.AllKeys.Max().Replace("tracking", ""), out int number);
                    config.AppSettings.Settings.Add(new KeyValueConfigurationElement($"tracking{number + 1}", path));
                    config.Save();

                    repo.AddTracking(path);
                    ConfigurationManager.RefreshSection("appSettings");
                }

            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            repo.AddCategoryToPicture(repo.Images[0], "Проверка");
            repo.Save();
            ImagesContainer.Children.Clear();
            ImagesContainer.Children.Add(GetView(repo.Images[0]));
            var r = 2;
        }

        private StackPanel GetView(ImageDescriptor descriptor)
        {
            StackPanel descContainer = new StackPanel();
            //descContainer.VerticalAlignment = VerticalAlignment.Bottom;
            descContainer.DataContext = descriptor;

            TextBox tb = new TextBox();
            tb.Text = descriptor.Name;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(descriptor.Path));
            image.VerticalAlignment = VerticalAlignment.Stretch;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Height = double.NaN;
            image.Width = double.NaN;
            image.Margin = new Thickness(5, 5, 5, 5);

            WrapPanel categoryContainer = new WrapPanel();
            foreach (var category in descriptor.Categories)
            {
                DockPanel categ = new DockPanel(); //категория с названием и кнопкой "x"
                categ.Height = 30;
                Label l = new Label();
                l.Content = category.Name;

                Button x = new Button(); //кнопка "x"
                x.Content = "x";
                x.Width = 20;
                x.Height = 20;
                x.Click += (s, e) =>
                {
                    repo.RemoveCategoryFromPicture(descriptor, category);
                    categoryContainer.Children.Remove(categ);
                }; // удаляем категорию у дескриптора и элемент с UI

                categ.Children.Add(x);//кнопка удалить
                categ.Children.Add(l);//название категории
                categoryContainer.Children.Add(categ);
            }

            descContainer.Children.Add(tb); //название файла
            descContainer.Children.Add(categoryContainer); // категории
            descContainer.Children.Add(image); // изображение

            return descContainer;
        }
    }
}
