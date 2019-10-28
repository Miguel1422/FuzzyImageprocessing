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
    public partial class Form2 : Form
    {
        public Form2(Bitmap image)
        {
            InitializeComponent();
            pictureBox1.Image = image;
        }
    }
}
