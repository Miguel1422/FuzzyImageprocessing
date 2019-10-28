using FuzzyLogic.ImageProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyLogic
{
    public partial class Form1 : Form
    {
        private Bitmap cImage;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Create a new Bitmap object from the picture file on disk,
                    // and assign that to the PictureBox.Image property
                    cImage = new Bitmap(dlg.FileName);
                    pictureBox1.Image = cImage;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Corrector corrector = new Corrector(cImage)
            {
                Alpha = trackBar1.Value
            };
            Form2 f2 = new Form2(corrector.Process());
            f2.Show();
        }
    }
}
