using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace VignetteRemoval.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        Image m_original = new Bitmap(1, 1);
        Image m_processed = new Bitmap(1, 1);
        List<double> m_vignette;

        /// <summary>
        /// Load an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Open Image",
                Filter = "Image File|*.jpg;*.png"
            })
            {
                if (ofd.ShowDialog(this) != DialogResult.OK) return;

                m_original = Image.FromFile(ofd.FileName);
                m_processed = (Image)m_original.Clone();
                myProcessedLabel.Text = "Original";
                myPictureBox.Image = m_original;
            }
        }

        /// <summary>
        /// Save the processed image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save Processed Image",
                Filter = "Image File|*.jpg;*.png"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                m_processed.Save(sfd.FileName, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Remove the vignette
        /// Pops open a window to show the vignette removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveVignetteButton_Click(object sender, EventArgs e) => Process();

        /// <summary>
        /// Flip the processed and unprocessed images to show the difference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlipButton_Click(object sender, EventArgs e)
        {
            if (myPictureBox.Image == m_processed)
            {
                myPictureBox.Image = m_original;
                myProcessedLabel.Text = "Original";
            }
            else
            {
                myPictureBox.Image = m_processed;
                myProcessedLabel.Text = "Processed";
            }
        }

        private void LoadDataButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Load Vignette",
                Filter = "Dat File|*.dat"
            })
            {
                if (ofd.ShowDialog(this) != DialogResult.OK) return;

                m_vignette = new List<double>();
                string line;
                using (StreamReader file = new StreamReader(ofd.FileName))
                    while ((line = file.ReadLine()) != null)
                        m_vignette.Add(double.Parse(line));
            }
        }

        private void SaveDataButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save Vignette",
                Filter = "Dat File|*.dat"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                using (StreamWriter file = new StreamWriter(sfd.FileName))
                    foreach (var value in m_vignette)
                        file.WriteLine(value);
            }
        }

        void Process() 
        {
            switch(comboBox1.SelectedItem as string)
            {
                case "ASIV":
                    {
                        var correction = new ASIV_VignetteCorrection(m_vignette);
                        m_processed = correction.Process(m_original);
                        new PictureForm(correction.VignetteEstimate).Show(this);
                        m_vignette = correction.Vignette;
                    }
                    break;
                case "LIE":  m_processed = new LIE_VignetteCorrection().Process(m_original); break;
                case "HILL":
                    {
                        var correction = new CMLIE_VignetteCorrection();
                        m_processed = correction.Process(m_original);
                        new PictureForm(correction.VignetteEstimate).Show(this);
                    }
                    break;
            }

            myPictureBox.Image = m_processed;
            myProcessedLabel.Text = "Processed";
        }
    }
}
