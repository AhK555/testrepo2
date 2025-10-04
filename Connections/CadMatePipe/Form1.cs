using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CadMate = Gssoft.Gscad.ApplicationServices.Application;

namespace CadMatePipe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var doc = CadMate.DocumentManager.MdiActiveDocument;
            var test = doc.Editor.GetEntity("enter");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Hide();
            var doc = CadMate.DocumentManager.MdiActiveDocument;
            var test = doc.Editor.GetEntity("enter");
            MessageBox.Show("TEST After Get Entity");
            if (test.Status == Gssoft.Gscad.EditorInput.PromptStatus
                .OK)
            {
                MessageBox.Show("test");
            }
        }
    }
}
