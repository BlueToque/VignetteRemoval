using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace VignetteRemoval
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        Image m_original = new Bitmap(1, 1);
        Image m_processed = new Bitmap(1, 1);

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
        private void RemoveVignetteButton_Click(object sender, EventArgs e)
        {
            var correction = new VignetteCorrection();
            m_processed = correction.Process(m_original);
            myPictureBox.Image = m_processed;

            new PictureForm(correction.VignetteEstimate).Show(this);
        }

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
    }
}
