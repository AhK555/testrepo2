using Gssoft.Gscad.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Unit = NSVLIBConstants.Enums.Unit;
using Properties = CadMatePipe.Properties;

namespace PipeApp
{
    public partial class PipeSizeForm : Form
    {
        public bool Cancel { get; set; } = false;
        private Document _doc = ACAD.DocumentManager.MdiActiveDocument;
        public PipeSizeForm()
        {
            InitializeComponent();
        }

        private void PipeSizeForm_Load(object sender, EventArgs e)
        {
            grBoxSize.Paint += blueGroupBox_Paint;
            grBoxUnit.Paint += blueGroupBox_Paint;
            fontSettingGroupBox.Paint += blueGroupBox_Paint;
            switch (Properties.Settings.Default.Unit)
            {
                case (int)Unit.metric:
                    rdButtMetric.Checked = true;
                    break;
                case (int)Unit.imperial: 
                    rdButtImperial.Checked = true;
                    break;
            }
            if (cBoxSize.Items.Contains(Properties.Settings.Default.PipeSize))
                cBoxSize.SelectedItem =Properties.Settings.Default.PipeSize;

            fontstyleComboBox.DropDownWidth = Convert.ToInt32(fontstyleComboBox.Width * 1.7);
            loadFonts();
            if(fontstyleComboBox.Items.Count > 0) 
                fontstyleComboBox.SelectedIndex = 0;

            fontstyleComboBox.SelectedItem = Properties.Settings.Default.pipeSizeFontStyle;
            fontSizeTextBox.Text = Properties.Settings.Default.pipeSizeFontSize.ToString();
            switch (Properties.Settings.Default.SizingMethod)
            {
                case (int)SizingMethod.Default:
                    rdnPreviosSize.Checked = true;
                    break;
                case (int)SizingMethod.PipeScheduling:
                    rdnPipeSchedule.Checked = true;
                    break;
                case (int)SizingMethod.AssignNow:
                    rdnSetNewSiz.Checked = true;
                    break;
            }
        }
        void loadFonts()
        {
            InstalledFontCollection installedFonts = new InstalledFontCollection();
            foreach (FontFamily fontFamily in installedFonts.Families) 
                fontstyleComboBox.Items.Add(fontFamily.Name);
        }

        private void blueGroupBox_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.FromArgb(21, 87, 138), Color.SteelBlue);
        }
        private void DrawGroupBox(GroupBox box, Graphics g, Color textColor, Color borderColor)
        {
            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                g.Clear(this.BackColor);

                // Draw text
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }
        private void rdButtImperial_CheckedChanged(object sender, EventArgs e)
        {
            var rd = sender as RadioButton;
            if(rd.Checked)
            {
                cBoxSize.DataSource = null;
                cBoxSize.DataSource = NSVLIBConstants.PipeData.GetSize(Unit.imperial);
            }
        }
        private void rdButtMetric_CheckedChanged(object sender, EventArgs e)
        {
            var rd = sender as RadioButton;
            if (rd.Checked)
            {
                cBoxSize.DataSource = null;
                cBoxSize.DataSource = NSVLIBConstants.PipeData.GetSize(Unit.metric);
            }
        }
        private void PipeSizeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cancel = true;
        }
        private void ButtonAssign_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(fontSizeTextBox.Text, out double fontSize) || fontSizeTextBox.Text == "0")
            {
                MessageBox.Show("Invaild Font Height");
                return;
            }
            if(rdnSetNewSiz.Checked) 
            {
                InputData.PipeSize = cBoxSize.SelectedItem.ToString();
                Properties.Settings.Default.PipeSize = cBoxSize.SelectedItem.ToString();
            }
            else
                InputData.PipeSize = null;

            Properties.Settings.Default.Unit = (int)(rdButtImperial.Checked ? Unit.imperial : Unit.metric);
            Properties.Settings.Default.pipeSizeFontSize = fontSize;
            Properties.Settings.Default.pipeSizeFontStyle = fontstyleComboBox.SelectedItem.ToString();
            Properties.Settings.Default.SizingMethod= (int)InputData.SizingMethod;

            Properties.Settings.Default.Save();
            InputData.Unit = (rdButtImperial.Checked ? Unit.imperial : Unit.metric);
            InputData.TextHeight = fontSize;
            InputData.TextStyle = Fonts.GetTextStyleId(_doc.Database, fontstyleComboBox.SelectedItem.ToString());
            this.Hide();
        }
        private void PipeSizeForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Cancel = true;
                this.Close();
            }
        }
        private void fontSizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            if (char.IsDigit(e.KeyChar) || e.KeyChar == '.')
            {
                TextBox textBox = sender as TextBox;
                if (e.KeyChar == '.' && textBox.Text.Contains("."))
                {
                    e.Handled = true;
                }
                return;
            }

            e.Handled = true;
        }
        private void UsePreviousData_CheckedChanged(object sender, EventArgs e)
        {
            if (rdnPreviosSize.Checked)
            { 
                cBoxSize.Enabled = false;
                pipeSizeLabel.Enabled = false;
                InputData.SizingMethod = SizingMethod.Default;
            }
        }
        private void pipeSched_CheckedChanged(object sender, EventArgs e)
        {
            if (rdnPipeSchedule.Checked)
            {
                cBoxSize.Enabled = false;
                pipeSizeLabel.Enabled = false;
                InputData.SizingMethod = SizingMethod.PipeScheduling;
            }
        }
        private void SetNewSize_CheckedChanged(object sender, EventArgs e)
        {
            if (rdnSetNewSiz.Checked)
            {
                cBoxSize.Enabled = true;
                pipeSizeLabel.Enabled = true;
                InputData.SizingMethod = SizingMethod.AssignNow;
            }
        }
    }
}
