using System.Drawing;
using System.Windows.Forms;

namespace VignetteRemoval.Demo
{
    public partial class PictureForm : Form
    {
        public PictureForm(Image image)
        {
            InitializeComponent();
            pictureBox1.Image = image;
        }
    }
}
