using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace CmpFiles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class IntPercent
        {
            public int Value;
            public IntPercent(int val)
            {
                Value = val;
            }
            public override string ToString() => $"{Value}%";
        }
        IntPercent[] qualityOptions => new IntPercent[]{
            new IntPercent(10),
            new IntPercent(20),
            new IntPercent(30),
            new IntPercent(40),
            new IntPercent(50),
            new IntPercent(60),
            new IntPercent(70),
            new IntPercent(80),
            new IntPercent(90),
            new IntPercent(100),
        };

        class PathRef
        {
            public string Path;
        }
        void Compress()
        {
            string[] files = Directory.GetFiles(sourceFolder.Path);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToUpper();
                if (ext == ".PNG")
                {
                    CmpFiles.CompressImage(file, destFolder.Path, quality);
                }
            }
            MessageBox.Show("Compressed Images have been stored to\n" + destFolder);
        }
        private void OpenFolderDialog(PathRef filepath)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filepath.Path = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        PathRef sourceFolder = new PathRef();
        PathRef destFolder = new PathRef();
        int quality = 50;

        void SelectSourcePath()
        {
            OpenFolderDialog(sourceFolder);
        }
        void SelectDestPath()
        {
            OpenFolderDialog(destFolder);
        }
        public MainWindow()
        {
            InitializeComponent();
            cmbQuality.ItemsSource = qualityOptions;
        }

        private void btSource_Click(object sender, RoutedEventArgs e)
        {
            SelectSourcePath();
            tbSource.Text = sourceFolder.Path;
        }

        private void btDest_Click(object sender, RoutedEventArgs e)
        {
            SelectDestPath();
            tbDest.Text = destFolder.Path;
        }

        private void cmbQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            quality = (cmbQuality.SelectedItem as IntPercent).Value;
        }

        private void btCompress_Click(object sender, RoutedEventArgs e)
        {
            Compress();
        }
    }
}
