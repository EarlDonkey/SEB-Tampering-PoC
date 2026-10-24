using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySEBPatcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Starting patch...\r\n";
            string sebPath = @"C:\Program Files\SafeExamBrowser\Application\";

            if (!Directory.Exists(sebPath))
            {
                MessageBox.Show("Safe Exam Browser is not installed!");
                return;
            }

            button1.Enabled = false;
            textBox1.Text += "Installing custom certificate...\r\n";

            try
            {
                await Task.Run(() =>
                {
                    // 1. Install the Certificate silently
                    string regPath = Path.Combine(Path.GetTempPath(), "SEB.reg");
                    File.WriteAllBytes(regPath, Properties.Resources.SEB_reg);

                    ProcessStartInfo regInfo = new ProcessStartInfo
                    {
                        FileName = "regedit.exe",
                        Arguments = $"/s \"{regPath}\"", // /s means silent
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(regInfo).WaitForExit();
                    File.Delete(regPath);

                    // 2. Extract the 5 signed files
                    textBox1.Invoke(new Action(() => textBox1.Text += "Extracting patched files...\r\n"));
                    string tempZipPath = Path.Combine(Path.GetTempPath(), "MySEBPatch.zip");
                    File.WriteAllBytes(tempZipPath, Properties.Resources.SEB_x64_Small);

                    using (ZipArchive archive = ZipFile.OpenRead(tempZipPath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string destinationPath = Path.Combine(sebPath, entry.FullName);
                            entry.ExtractToFile(destinationPath, true);
                        }
                    }

                    File.Delete(tempZipPath);
                });

                textBox1.Text += "PATCH SUCCESSFUL!\r\n";
            }
            catch (Exception ex)
            {
                textBox1.Text += "ERROR: " + ex.Message + "\r\n";
            }
            finally
            {
                button1.Enabled = true;
            }
        }
    }
}
