using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class Front : Form
    {
        public Front()
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

        }

        private void RoundedForm_Paint(object sender, PaintEventArgs e)
        {

            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int radius = 30;
                int diameter = radius * 2;

                GraphicsPath path = new GraphicsPath();

                Rectangle topLeftArc = new Rectangle(0, 0, diameter, diameter);
                Rectangle topRightArc = new Rectangle(this.Width - diameter, 0, diameter, diameter);
                Rectangle bottomRightArc = new Rectangle(this.Width - diameter, this.Height - diameter, diameter, diameter);
                Rectangle bottomLeftArc = new Rectangle(0, this.Height - diameter, diameter, diameter);

                path.AddArc(topLeftArc, 180, 90);
                path.AddArc(topRightArc, 270, 90); // Top-right corner
                path.AddArc(bottomRightArc, 0, 90); // Bottom-right corner
                path.AddArc(bottomLeftArc, 90, 90); // Bottom-left corner

                path.CloseFigure();

                this.Region = new Region(path);
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void regbtn_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Register mainForm = new Register();
            mainForm.Show();
            this.Hide();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Front_Load(object sender, EventArgs e)
        {

        }
    }
}
