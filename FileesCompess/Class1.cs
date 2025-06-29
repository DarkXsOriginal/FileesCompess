using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace FileesCompess
{
    internal class Compress
    {
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 10; i <= 100; i = i + 10)
            {
                cmbQuality.Items.Add(i);
            }
            cmbQuality.SelectedIndex = 4;
        }
        private void OpenFolderDialog(TextBox Filepath)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Filepath.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }
        private void btnSouceBrowse_Click(object sender, EventArgs e)
        {
            OpenFolderDialog(txtSource);
        }
        private void btnDestFolder_Click(object sender, EventArgs e)
        {
            OpenFolderDialog(txtDestination);
        }
       
        private void btnCompress_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(txtSource.Text);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToUpper();
                if (ext == ".PNG" || ext == ".JPG")
                {
                    CompressImage(file, txtDestination.Text, (int)cmbQuality.SelectedItem);
                }
            }
            MessageBox.Show("Compressed Images have been stored to\n" + txtDestination.Text);
            txtDestination.Text = "";
            txtSource.Text = "";
        }
    }
}
