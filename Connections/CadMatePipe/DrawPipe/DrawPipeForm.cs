using Gssoft.Gscad.ApplicationServices;
using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.EditorInput;
using Gssoft.Gscad.Geometry;
using Gssoft.Gscad.Windows;
using NSVLIBConstants;
using PipeApp.DrawPipe;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ACAD = Gssoft.Gscad.ApplicationServices.Application;
using Button = System.Windows.Forms.Button;
using converter = Gssoft.Gscad.Runtime.Converter;
using Properties = CadMatePipe.Properties;
using TextBox = System.Windows.Forms.TextBox;
using Unit = NSVLIBConstants.Enums.Unit;

namespace PipeApp
{
    public partial class DrawPipeForm : Form
    {
        private Document _doc;
        public bool OK { get; set; } = false;
        private int _mainColor { get; set; }
        private int _branchColor { get; set; }

        private CheckedListBox _blockRefCheckedListBox;

        public DrawPipeForm(Document doc)
        {
            InitializeComponent();
            _doc = doc;
            wetRadioButton.Checked = true;
        }
        string branchPipeMaterial => branchPipeMaterialComboBox.SelectedItem.ToString();
        string mainPipeMaterial => mainPipeMaterialComboBox.SelectedItem.ToString();
        string branchPipeSubMaterial => branchPipeSubMaterialComboBox.SelectedItem.ToString();
        string mainPipeSubMaterial => mainPipeSubMaterialComboBox.SelectedItem.ToString();
        private void prettyForm_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.KeyPreview = true;


            mainPipeMaterialComboBox.DataSource = Enum.GetNames(typeof(MaterialEnum));
            branchPipeMaterialComboBox.DataSource = Enum.GetNames(typeof(MaterialEnum));

            comboMainLineWeight.Enabled = false;
            comboMainLineWeight.DataSource = FormPopulator.LineWeights;
            if (string.IsNullOrEmpty(Properties.Settings.Default.MainLineWeight.ToString()))
                comboMainLineWeight.SelectedItem = "LineWeight040";
            else
                comboMainLineWeight.SelectedItem = CadMatePipe.Properties.Settings.Default.MainLineWeight.ToString();
            if (Properties.Settings.Default.UseMainLineWeight)
            {
                chboxMainLineWeight.Checked = true;
                comboMainLineWeight.Enabled = true;
            }

            comboBranchLineWeight.Enabled = false;
            comboBranchLineWeight.DataSource = FormPopulator.LineWeights;
            if (string.IsNullOrEmpty(Properties.Settings.Default.BranchLineWeight.ToString()))
                comboBranchLineWeight.SelectedItem = "LineWeight040";
            else
                comboBranchLineWeight.SelectedItem = Properties.Settings.Default.BranchLineWeight.ToString();
            if (Properties.Settings.Default.UseBranchLineWeight)
            {
                chboxBranchLineWeight.Checked = true;
                comboBranchLineWeight.Enabled = true;
            }

            rdButtImperial.Checked = true;
            rdButtImperial.Checked = true;
            //label7.Location = new Point(Convert.ToInt32(groupBox3.Width * 0.05), Convert.ToInt32(groupBox3.Height * 0.1));
            //blockRefListBox.Location = new Point(Convert.ToInt32(groupBox3.Width * 0.05),label7.Location.Y+label7.Height) ;
            //blockRefListBox.Size = new Size(Convert.ToInt32((groupBox3.Width) * 0.9), Convert.ToInt32(groupBox3.Height * 0.6));

            //selectBlockButtons.Size =new Size(blockRefListBox.Size.Width,selectBlockButtons.Size.Height);
            //selectBlockButtons.Location =new Point(blockRefListBox.Location.X + blockRefListBox.Width-selectBlockButtons.Width,blockRefListBox.Location.Y+blockRefListBox.Height+selectBlockButtons.Height/5);
            //groupBox1.Paint += blueGroupBox_Paint;
            //groupBox2.Paint += blueGroupBox_Paint;
            //groupBox3.Paint += blueGroupBox_Paint;
            //grBoxUnit.Paint += blueGroupBox_Paint;
            //systemTypeGroupBox.Paint += blueGroupBox_Paint;

            var mainColor = Color.FromArgb(
                Properties.Settings.Default.MainRed,
                Properties.Settings.Default.MainGreen,
                Properties.Settings.Default.MainBlue);

            if (mainColor != Color.Black)
            {
                mainsColorPanel.BackColor = mainColor;
                mainsColorDialog.Color = mainColor;
                _mainColor = Gssoft.Gscad.Colors.Color.FromRgb(
                    (byte)Properties.Settings.Default.MainRed,
                    (byte)Properties.Settings.Default.MainGreen,
                    (byte)Properties.Settings.Default.MainBlue)
                    .ColorIndex;
            }
            var branchColor = Color.FromArgb(
                Properties.Settings.Default.BranchRed,
                Properties.Settings.Default.BranchGreen,
                Properties.Settings.Default.BranchBlue);

            if (branchColor != Color.Black)
            {
                branchColorPanel.BackColor = branchColor;
                branchColorDialog.Color = branchColor;
                _branchColor = Gssoft.Gscad.Colors.Color.FromRgb(
                    (byte)Properties.Settings.Default.BranchRed,
                    (byte)Properties.Settings.Default.BranchGreen,
                    (byte)Properties.Settings.Default.BranchBlue)
                    .ColorIndex;
            }

            if (Properties.Settings.Default.BranchPipeSize != string.Empty &&
                cBoxBranchSize.Items.Contains(Properties.Settings.Default.BranchPipeSize))
            {
                cBoxBranchSize.SelectedItem = Properties.Settings.Default.BranchPipeSize;
            }
            if (Properties.Settings.Default.MainPipeSize != string.Empty &&
                cBoxMainSize.Items.Contains(Properties.Settings.Default.MainPipeSize))
            {
                cBoxMainSize.SelectedItem = Properties.Settings.Default.MainPipeSize;
            }

            branchWidth.Text = Properties.Settings.Default.BranchPipeWidth.ToString();
            mainWidth.Text = Properties.Settings.Default.MainPipeWidth.ToString();
            if (Properties.Settings.Default.SprinklerList != null)
                blockRefListBox.DataSource = Properties.Settings.Default.SprinklerList;

            PopulateBlockRefListBox();
            chBoxBreakPipe.Checked = Properties.Settings.Default.IncludeBreakPipe;

            if (Properties.Settings.Default.mainPipeMaterial != string.Empty
                && mainPipeMaterialComboBox.Items.Contains(Properties.Settings.Default.mainPipeMaterial))
            {
                mainPipeMaterialComboBox.SelectedItem = Properties.Settings.Default.mainPipeMaterial;
            }
            else
            {
                mainPipeMaterialComboBox.SelectedIndex = 0;
            }

            if (Properties.Settings.Default.branchPipeMaterial != string.Empty
                && branchPipeMaterialComboBox.Items.Contains(Properties.Settings.Default.branchPipeMaterial))
            {
                branchPipeMaterialComboBox.SelectedItem = Properties.Settings.Default.branchPipeMaterial;
            }
            else
            {
                branchPipeMaterialComboBox.SelectedIndex = 0;
            }

            if (Properties.Settings.Default.mainPipeSubMaterial != string.Empty
                && mainPipeSubMaterialComboBox.Items.Contains(Properties.Settings.Default.mainPipeSubMaterial))
            {
                mainPipeSubMaterialComboBox.SelectedItem = Properties.Settings.Default.mainPipeSubMaterial;
            }
            else
            {
                mainPipeSubMaterialComboBox.SelectedIndex = 0;
            }

            if (Properties.Settings.Default.branchPipeSubMaterial != string.Empty
                && branchPipeSubMaterialComboBox.Items.Contains(Properties.Settings.Default.branchPipeSubMaterial))
            {
                branchPipeSubMaterialComboBox.SelectedItem = Properties.Settings.Default.branchPipeSubMaterial;
            }
            else
            {
                branchPipeSubMaterialComboBox.SelectedIndex = 0;
            }

            if (Properties.Settings.Default.systemType != string.Empty
                && comboBoxSystemType.Items.Contains(Properties.Settings.Default.systemType))
            {
                comboBoxSystemType.SelectedItem = Properties.Settings.Default.systemType;
            }



            rdButtImperial.Checked = Properties.Settings.Default.imperialRadioButton;
            rdButtMetric.Checked = Properties.Settings.Default.metricRadioButton;
            wetRadioButton.Checked = Properties.Settings.Default.wetRadioButton;
            dryRadioButton.Checked = Properties.Settings.Default.dryRadioButton;

            if (wetRadioButton.Checked)
            {
                dryRadioButton.Checked = true;
                wetRadioButton.Checked = true;
            }
            else
            {
                wetRadioButton.Checked = true;
                dryRadioButton.Checked = true;
            }

            txtBoxBranchCFactor.Text = Properties.Settings.Default.branchPipeCFactor;
            txtBoxMainCFactor.Text = Properties.Settings.Default.mainPipeCFactor;

            _blockRefCheckedListBox = new CheckedListBox();
            _blockRefCheckedListBox.Size = blockRefListBox.Size;
            _blockRefCheckedListBox.Location = blockRefListBox.Location;
            _blockRefCheckedListBox.BackColor = blockRefListBox.BackColor;
            _blockRefCheckedListBox.Font = blockRefListBox.Font;
            _blockRefCheckedListBox.ForeColor = blockRefListBox.ForeColor;
            _blockRefCheckedListBox.CheckOnClick = true;

            _blockRefCheckedListBox.Visible = false;

            blockRefListBox.SelectedIndex = 0;

            groupBox3.Controls.Add(_blockRefCheckedListBox);


            //_DeleteSelectedItemsButton = new Button();
            //_DeleteSelectedItemsButton.Text = "Remove Selected Items";
            //_DeleteSelectedItemsButton.BackColor = deleteButton.BackColor;
            //_DeleteSelectedItemsButton.ForeColor = deleteButton.ForeColor;
            //_DeleteSelectedItemsButton.Font = deleteButton.Font;
            //_DeleteSelectedItemsButton.Location = deleteButton.Location;
            //_DeleteSelectedItemsButton.Size = new Size(blockRefListBox.Width, deleteButton.Height);
            //_DeleteSelectedItemsButton.Visible = false;

            //_DeleteSelectedItemsButton.Click += new EventHandler(this.DeleteItems);

            //groupBox3.Controls.Add(_DeleteSelectedItemsButton);

        }

        

        private void PopulateBlockRefListBox()
        {
            if (Properties.Settings.Default.SprinklerList != null)
            {
                var sprinklerList = new List<string>();
                using (var tr = _doc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    var blockTable = tr.GetObject(_doc.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    foreach (var sprinkler in Properties.Settings.Default.SprinklerList)
                    {
                        if (blockTable.Has(sprinkler))
                            sprinklerList.Add(sprinkler);
                    }
                    tr.Commit();
                }
                blockRefListBox.DataSource = sprinklerList;
                var sprinklerCollection = new StringCollection();
                sprinklerCollection.AddRange(sprinklerList.ToArray());
                Properties.Settings.Default.SprinklerList = sprinklerCollection;
                Properties.Settings.Default.Save();
            }
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
        
        private void DrawPipeButton_Click(object sender, EventArgs e)
        {
            if(mainPipeMaterialComboBox.SelectedItem is null)
            {
                MessageBox.Show("Please select a main pipe material.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if(mainPipeSubMaterialComboBox.SelectedItem is null)
            {
                MessageBox.Show("Please select a main pipe sub material.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            

            if (branchPipeMaterialComboBox.SelectedItem is null)
            {
                MessageBox.Show("Please select a branch pipe material.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (branchPipeSubMaterialComboBox.SelectedItem is null)
            {
                MessageBox.Show("Please select a branch pipe sub material.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

            SetSettings();
            SetInputData();
            
            OK = true;

            this.Close();
        }

        private void SetSettings()
        {
            Properties.Settings.Default.MainPipeSize = cBoxMainSize.SelectedItem.ToString();
            Properties.Settings.Default.BranchPipeSize = cBoxBranchSize.SelectedItem.ToString();
            Properties.Settings.Default.IncludeBreakPipe = chBoxBreakPipe.Checked;
            if (double.TryParse(mainWidth.Text, out double value))
                Properties.Settings.Default.MainPipeWidth = value;
            if (double.TryParse(branchWidth.Text, out value))
                Properties.Settings.Default.BranchPipeWidth = value;

            Properties.Settings.Default.UseMainLineWeight = chboxMainLineWeight.Checked;
            if (chboxMainLineWeight.Checked)
            {
                Properties.Settings.Default.MainLineWeight = comboMainLineWeight.SelectedItem.ToString();
                ACAD.SetSystemVariable("LWDISPLAY", 1);
            }

            Properties.Settings.Default.UseBranchLineWeight = chboxBranchLineWeight.Checked;
            if (chboxMainLineWeight.Checked)
            {
                Properties.Settings.Default.BranchLineWeight = comboBranchLineWeight.SelectedItem.ToString();
                ACAD.SetSystemVariable("LWDISPLAY", 1);
            }

            Properties.Settings.Default.mainPipeMaterial = mainPipeMaterialComboBox.SelectedText;
            Properties.Settings.Default.branchPipeMaterial = branchPipeMaterialComboBox.SelectedText;

            Properties.Settings.Default.mainPipeSubMaterial = mainPipeSubMaterialComboBox.SelectedText;
            Properties.Settings.Default.branchPipeSubMaterial = branchPipeSubMaterialComboBox.SelectedText;

            Properties.Settings.Default.imperialRadioButton = rdButtImperial.Checked;
            Properties.Settings.Default.metricRadioButton = rdButtMetric.Checked;
            Properties.Settings.Default.wetRadioButton = wetRadioButton.Checked;
            Properties.Settings.Default.dryRadioButton = dryRadioButton.Checked;
            Properties.Settings.Default.systemType = comboBoxSystemType.SelectedText;
            Properties.Settings.Default.mainPipeCFactor = txtBoxMainCFactor.Text;
            Properties.Settings.Default.branchPipeCFactor = txtBoxBranchCFactor.Text;

            Properties.Settings.Default.Save();
        }

        private void SetInputData()
        {
            _mainColor = Gssoft.Gscad.Colors.Color.FromRgb(
                mainsColorDialog.Color.R,
                mainsColorDialog.Color.G,
                mainsColorDialog.Color.B).ColorIndex;
            _branchColor = Gssoft.Gscad.Colors.Color.FromRgb(
                 branchColorDialog.Color.R,
                 branchColorDialog.Color.G,
                 branchColorDialog.Color.B).ColorIndex;

            InputData.SprinklerNames.AddRange(NSVLibUtils.BlockDefs.GetNSVSprinklerBlocks());
            InputData.SprinklerNames.AddRange(NSVLibUtils.BlockDefs.GetNSVValveBlocks());
            InputData.SprinklerNames.Add("NSVSIDETEMPSPRINKLER");
            InputData.SprinklerNames.Add("NSVPENDENTTEMPSPRINKLER");

            InputData.Unit = rdButtMetric.Checked? Unit.metric: Unit.imperial;
            InputData.UseMainPipeLineWeight= chboxMainLineWeight.Checked;
            InputData.MainColor = _mainColor;
            InputData.MainSize = cBoxMainSize.SelectedItem.ToString();
            InputData.MainWidth = double.TryParse(mainWidth.Text, out double value) ? value : 1;
            if (Enum.TryParse(comboMainLineWeight.Text, out LineWeight lineWeight))
                InputData.MainLineWeight = lineWeight;

            InputData.UseBranchPipeLineWeight = chboxBranchLineWeight.Checked;
            InputData.BranchColor = _branchColor;
            InputData.BranchSize = cBoxBranchSize.SelectedItem.ToString();
            InputData.BranchWidth = double.TryParse(branchWidth.Text, out value) ? value : 1;
            if (Enum.TryParse(comboBranchLineWeight.Text, out lineWeight))
                InputData.BranchLineWeight = lineWeight;

            InputData.IncludeElevationSign = chBoxBreakPipe.Checked;
            InputData.IsDrawingMain = true;

            if (InputData.CrossSymbolDistance == 0)
            {
                var previousDist = Properties.Settings.Default.pipeDistImperial;
                previousDist = FixScaleUnitChange(previousDist);
            }

            if (InputData.CrossSymbolScale == 0)
            {
                var previousScale = Properties.Settings.Default.pipeScaleImperial;
                previousScale = FixScaleUnitChange(previousScale);
                InputData.CrossSymbolScale = InputData.Unit == Unit.metric ? previousScale * 40 : previousScale;
            }

            InputData.MainPipeMaterial = (MaterialEnum)Enum.Parse(typeof(MaterialEnum), mainPipeMaterialComboBox.SelectedItem.ToString());
            InputData.BranchPipeMaterial = (MaterialEnum)Enum.Parse(typeof(MaterialEnum), branchPipeMaterialComboBox.SelectedItem.ToString());
            InputData.MainPipeSubMaterial = mainPipeSubMaterialComboBox.SelectedItem.ToString();
            InputData.BranchPipeSubeMaterial = branchPipeSubMaterialComboBox.SelectedItem.ToString();
            InputData.MainPipeCFactor = txtBoxMainCFactor.Text;
            InputData.BranchPipeCFactor = txtBoxMainCFactor.Text;
            var mainPipeMaterial = mainPipeMaterialComboBox.SelectedItem.ToString();
            var mainPipeSubMaterial = mainPipeSubMaterialComboBox.SelectedItem.ToString();
            var mainPipeSize = PipeDiameterTable.GetPipeSize(mainPipeSubMaterial, Unit.imperial)[cBoxMainSize.SelectedIndex];
            var mainPipeDiameter = PipeDiameterTable.GetPipeDiameter(mainPipeSize, mainPipeMaterial, mainPipeSubMaterial);
            var branchPipeSubMaterial = branchPipeSubMaterialComboBox.SelectedItem.ToString();
            var branchPipeMaterial = branchPipeMaterialComboBox.SelectedItem.ToString();
            var branchPipeSize = PipeDiameterTable.GetPipeSize(branchPipeSubMaterial, Unit.imperial)[cBoxBranchSize.SelectedIndex];
            string branchPipeDiameter = PipeDiameterTable.GetPipeDiameter(branchPipeSize, branchPipeMaterial, branchPipeSubMaterial);

            InputData.MainPipeDiameter = mainPipeDiameter;
            InputData.BranchPipeDiameter = branchPipeDiameter;
        }

        private double FixScaleUnitChange(double value)
        {
            if (InputData.Unit == Unit.imperial && value >= 100)
                value /= 25;

            return value;
        }
        private void branchColorpickerButton_Click(object sender, EventArgs e)
        {
            branchColorDialog.ShowDialog();
            branchColorPanel.BackColor = branchColorDialog.Color;
            _branchColor = Gssoft.Gscad.Colors.Color.FromRgb(
                branchColorDialog.Color.R, branchColorDialog.Color.G, branchColorDialog.Color.B).ColorIndex;

            Properties.Settings.Default.BranchRed = branchColorDialog.Color.R;
            Properties.Settings.Default.BranchGreen = branchColorDialog.Color.G;
            Properties.Settings.Default.BranchBlue = branchColorDialog.Color.B;
            Properties.Settings.Default.Save();
        }
        private void mainsColorPickerButton_Click(object sender, EventArgs e)
        {
            mainsColorDialog.ShowDialog();
            mainsColorPanel.BackColor = mainsColorDialog.Color;
            _mainColor = Gssoft.Gscad.Colors.Color.FromRgb(
                mainsColorDialog.Color.R, mainsColorDialog.Color.G, mainsColorDialog.Color.B).ColorIndex;

            Properties.Settings.Default.MainRed = mainsColorDialog.Color.R;
            Properties.Settings.Default.MainGreen = mainsColorDialog.Color.G;
            Properties.Settings.Default.MainBlue = mainsColorDialog.Color.B;
            Properties.Settings.Default.Save();
        }
        private void prettyForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                this.OK = false;
            }
        }
        private void rdButtImperial_CheckedChanged(object sender, EventArgs e)
        {
            if(rdButtImperial.Checked)
            {
                cBoxBranchSize.DataSource = null;
                cBoxMainSize.DataSource = null;
                //cBoxBranchSize.DataSource =PipeData.GetSize(Unit.imperial);
                //cBoxMainSize.DataSource =PipeData.GetSize(Unit.imperial);
                cBoxBranchSize.DataSource = PipeDiameterTable.GetPipeSize(branchPipeSubMaterial, Unit.imperial);
                cBoxMainSize.DataSource = PipeDiameterTable.GetPipeSize(mainPipeSubMaterial, Unit.imperial);
            }
        }
        private void rdButtMetric_CheckedChanged(object sender, EventArgs e)
        {
            if (rdButtMetric.Checked)
            {
                cBoxBranchSize.DataSource = null;
                cBoxMainSize.DataSource = null;
                //cBoxBranchSize.DataSource = PipeData.GetSize(Unit.metric);
                //cBoxMainSize.DataSource = PipeData.GetSize(Unit.metric);
                cBoxBranchSize.DataSource = PipeDiameterTable.GetPipeSize(branchPipeSubMaterial, Unit.metric);
                cBoxMainSize.DataSource = PipeDiameterTable.GetPipeSize(mainPipeSubMaterial, Unit.metric);
            }
        }
        private void branchWidth_KeyPress(object sender, KeyPressEventArgs e)
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
        private void mainWidth_KeyPress(object sender, KeyPressEventArgs e)
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
        private void chboxMainLineWeight_CheckedChanged(object sender, EventArgs e)
        {
            if (chboxMainLineWeight.Checked)
                comboMainLineWeight.Enabled = true;
            else
                comboMainLineWeight.Enabled=false;
        }
        private void chboxBranchLineWeight_CheckedChanged(object sender, EventArgs e)
        {
            if (chboxBranchLineWeight.Checked)
                comboBranchLineWeight.Enabled = true;
            else
                comboBranchLineWeight.Enabled = false;
        }

        private void mainMaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(mainPipeMaterial)
            {
                case "Black_Steel":
                case "Galvanized_Steel":
                    mainPipeSubMaterialComboBox.DataSource = new List<string>() { "10", "30", "40" };
                    break;
                case "Stainless_Steel":
                    mainPipeSubMaterialComboBox.DataSource = new List<string>() { "5S", "10S", "40S","80S" };
                    break ;
                case "Copper":
                    mainPipeSubMaterialComboBox.DataSource = new List<string>() { "Type K", "Type L", "Type M"};
                    break;
                case "Brass":
                    mainPipeSubMaterialComboBox.DataSource = new List<string>() { "Regular", "Extra Strong"};
                    break;
                case "CPVC":
                    mainPipeSubMaterialComboBox.DataSource = new List<string>() { "SDR32.5", "SDS26", "SDR21","SDR17","SDR13.5","SDR11" };
                    break;
            }
            mainPipeSubMaterialComboBox.SelectedIndex = 0;
            var system = string.Empty;
            if(comboBoxSystemType.SelectedItem != null)
                system = comboBoxSystemType.SelectedItem.ToString();
            txtBoxMainCFactor.Text = PipeCFactor.Get(mainPipeMaterial, dryRadioButton.Checked, system);
        }

        private void branchMaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (branchPipeMaterialComboBox.SelectedItem.ToString())
            {
                case "Black_Steel":
                case "Galvanized_Steel":
                    branchPipeSubMaterialComboBox.DataSource = new List<string>() { "10", "30", "40" };
                    break;
                case "Stainless_Steel":
                    branchPipeSubMaterialComboBox.DataSource = new List<string>() { "5S", "10S", "40S", "80S" };
                    break;
                case "Copper":
                    branchPipeSubMaterialComboBox.DataSource = new List<string>() { "Type K", "Type L", "Type M" };
                    break;
                case "Brass":
                    branchPipeSubMaterialComboBox.DataSource = new List<string>() { "Regular", "Extra Strong" };
                    break;
                case "CPVC":
                    branchPipeSubMaterialComboBox.DataSource = new List<string>() { "SDR32.5", "SDS26", "SDR21", "SDR17", "SDR13.5", "SDR11" };
                    break;
            }
            branchPipeSubMaterialComboBox.SelectedIndex = 0;
            var system = string.Empty;
            if(comboBoxSystemType.SelectedItem != null)
                system = comboBoxSystemType.SelectedItem.ToString();
            txtBoxBranchCFactor.Text = PipeCFactor.Get(branchPipeMaterial, dryRadioButton.Checked, system);
        }
        private void wetRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (wetRadioButton.Checked)
            {
                comboBoxSystemType.Enabled = false;
                comboBoxSystemType.DataSource = new List<string>();
            }
        }

        private void dryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var material = branchPipeMaterialComboBox.SelectedItem.ToString();
            if (material == "Stainless_Steel" || material == "Brass" || material == "Copper" || material == "CPVC")
            {
                comboBoxSystemType.DataSource = new List<string>();
                comboBoxSystemType.Enabled = false;
                txtBoxBranchCFactor.Text = "150";
                txtBoxMainCFactor.Text = "150";
            }
            else if (dryRadioButton.Checked)
            {
                if (material == "Black_Steel")
                {
                    comboBoxSystemType.Enabled = true;
                    comboBoxSystemType.DataSource = new List<string>
                    {
                        "Preaction",
                        "Preaction using Nitrogen" ,
                        "Preaction using Vacuum Pressure",
                        "Preaction using Vapor Corrosion Inhibitor",
                    };
                    comboBoxSystemType.SelectedIndex = 0;
                    return;
                }
                else if (material == "Galvanized_Steel")
                {
                    comboBoxSystemType.Enabled = true;
                    comboBoxSystemType.DataSource = new List<string>
                    {
                        "Preaction",
                        "Preaction using Nitrogen" ,
                        "Preaction using Vacuum Pressure",
                        "Preaction using Vapor Corrosion Inhibitor",
                    };
                    comboBoxSystemType.SelectedIndex = 0;
                    return;
                }
            }
        }

        private void comboBoxSystemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var system = comboBoxSystemType.SelectedItem.ToString();
            txtBoxBranchCFactor.Text = PipeCFactor.Get(branchPipeMaterial,dryRadioButton.Checked,system);
            txtBoxMainCFactor.Text = PipeCFactor.Get(mainPipeMaterial, dryRadioButton.Checked, system);
        }
        
        private void branchPipeSubMaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cBoxBranchSize.DataSource = PipeDiameterTable.GetPipeSize(branchPipeSubMaterial, (rdButtMetric.Checked)? Unit.metric: Unit.imperial);
        }

        private void mainPipeSubMaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cBoxMainSize.DataSource = PipeDiameterTable.GetPipeSize(mainPipeSubMaterial, (rdButtMetric.Checked) ? Unit.metric : Unit.imperial);
        }

        private static double ParseFraction(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Trim();

            if (input.Contains("-"))
            {
                var parts = input.Split('-');
                double whole = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double frac = ParseFraction(parts[1]);
                return whole + frac;
            }

            if (input.Contains("/"))
            {
                var parts = input.Split('/');
                double numerator = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double denominator = double.Parse(parts[1], CultureInfo.InvariantCulture);
                return numerator / denominator;
            }

            return double.Parse(input, CultureInfo.InvariantCulture);
        }

        private void blockRefListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            return;
            var image = NSVLibUtils.BlockUtils.GetBlockDefImage(blockRefListBox.SelectedItem.ToString());

            if(image != null)
            {
                PicBox.Image = image;
            }
        }

        private void SelectCustomBlockButtons_Click(object sender, EventArgs e)
        {
            if (selectBlockButtons.Text == "Select")
            {
                var ed = ACAD.DocumentManager.MdiActiveDocument.Editor;
                this.Hide();
                using (ed.StartUserInteraction(this))
                {
                    var sprinklerBlockNames = new CustomBlockSelector(_doc);
                    blockRefListBox.DataSource = sprinklerBlockNames.SelectedBlockNames;
                    InputData.SprinklerNames.AddRange(sprinklerBlockNames.SelectedBlockNames);

                    var sprinklerCollection = new StringCollection();
                    sprinklerCollection.AddRange(sprinklerBlockNames.SelectedBlockNames.ToArray());
                    Properties.Settings.Default.SprinklerList = sprinklerCollection;
                    Properties.Settings.Default.Save();
                }

                this.Show();
            }
            else
            {
                var itemsCount = _blockRefCheckedListBox.CheckedItems.Count;
                if (itemsCount == 0)
                {
                    MessageBox.Show("There are no items selected to remove", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var choose = MessageBox.Show($"Are you sure you want to remove the {itemsCount} selected {(itemsCount > 1 ? "item" : "items")}?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (choose == DialogResult.No)
                    return;

                for (int i = blockRefListBox.Items.Count - 1; i >= 0; i--)
                {
                    var item = blockRefListBox.Items[i];

                    if (_blockRefCheckedListBox.CheckedItems.Contains(item.ToString()))
                    {
                        blockRefListBox.Items.RemoveAt(i);
                    }
                }

                blockRefListBox.Visible = true;
                _blockRefCheckedListBox.Visible = false;


                selectBlockButtons.Text = "Select";
                selectBlockButtons.BackColor = Color.SteelBlue;

                deleteButton.Text = "Delete";
                deleteButton.BackColor = Color.FromArgb(209, 21, 21);
            }

        }


        private void deleteButton_Click(object sender, EventArgs e)
        {
            if(deleteButton.Text == "Delete")
            {
                blockRefListBox.Visible = false;
                _blockRefCheckedListBox.Visible = true;

                selectBlockButtons.Text = "Remove Selected Items";
                selectBlockButtons.BackColor = deleteButton.BackColor;
                deleteButton.Text = "Cancel";
                deleteButton.BackColor = Color.SteelBlue;

                _blockRefCheckedListBox.Items.Clear();
                foreach (var item in blockRefListBox.Items)
                {
                    _blockRefCheckedListBox.Items.Add(item);
                }
            }
            else
            {
                blockRefListBox.Visible = true;
                _blockRefCheckedListBox.Visible = false;


                selectBlockButtons.Text = "Select";
                selectBlockButtons.BackColor = Color.SteelBlue;

                deleteButton.Text = "Delete";
                deleteButton.BackColor = Color.FromArgb(209, 21, 21);
            }
        }

    }
}

