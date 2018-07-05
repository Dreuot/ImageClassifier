using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
            string dataPath = ConfigurationManager.AppSettings["dataPath"];
            List<string> tracking = new List<string>();
            var configKeys = ConfigurationManager.AppSettings.AllKeys.Where(key => key.IndexOf("tracking") == 0);
            foreach (var key in configKeys)
            {
                tracking.Add(ConfigurationManager.AppSettings[key]);
            }

            repo = new Repo(dataPath, tracking);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
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
            var r = 2;
        }
    }
}
