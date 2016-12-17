using System;
using System.IO;
using System.Windows.Forms;

namespace Sedati
{
    public partial class frmQuickNote : Form
    {
        public frmQuickNote()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmQuickNote_Load(object sender, EventArgs e)
        {
            if (File.Exists("note.txt"))
            {
                string raw = System.IO.File.ReadAllText("note.txt");
                txtQuickNote.Text = raw;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("note.txt"))
            {
                writer.WriteLine(txtQuickNote.Text);
            }
            this.Close();
        }
    }
}
