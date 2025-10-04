using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;

namespace PipeApp
{
    public class CustomBorderTextBox : TextBox
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        private const int WM_NCPAINT = 0x85;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_NCPAINT)
            {
                var dc = GetWindowDC(this.Handle);
                using (Graphics g = Graphics.FromHdc(dc))
                {
                    // SetDistance the border color to RGB(21, 87, 138)
                    Color customColor = Color.SteelBlue;
                    g.DrawRectangle(new Pen(customColor), 0, 0, this.Width - 1, this.Height - 1);
                }
            }
        }
    }
    partial class DrawPipeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawPipeForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBranchPipeSubMaterial = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBoxBranchCFactor = new PipeApp.CustomBorderTextBox();
            this.branchPipeSubMaterialComboBox = new System.Windows.Forms.ComboBox();
            this.branchPipeMaterialComboBox = new System.Windows.Forms.ComboBox();
            this.chboxBranchLineWeight = new System.Windows.Forms.CheckBox();
            this.comboBranchLineWeight = new System.Windows.Forms.ComboBox();
            this.cBoxBranchSize = new System.Windows.Forms.ComboBox();
            this.branchWidth = new PipeApp.CustomBorderTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.linesColorpickerButton = new System.Windows.Forms.Button();
            this.branchColorPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.grBoxPreview = new System.Windows.Forms.GroupBox();
            this.PicBox = new System.Windows.Forms.PictureBox();
            this.blockRefListBox = new System.Windows.Forms.ListBox();
            this.selectBlockButtons = new System.Windows.Forms.Button();
            this.chBoxBreakPipe = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblMainPipeSubMaterial = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtBoxMainCFactor = new PipeApp.CustomBorderTextBox();
            this.mainPipeSubMaterialComboBox = new System.Windows.Forms.ComboBox();
            this.mainPipeMaterialComboBox = new System.Windows.Forms.ComboBox();
            this.chboxMainLineWeight = new System.Windows.Forms.CheckBox();
            this.comboMainLineWeight = new System.Windows.Forms.ComboBox();
            this.cBoxMainSize = new System.Windows.Forms.ComboBox();
            this.mainWidth = new PipeApp.CustomBorderTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mainsColorpickerButton = new System.Windows.Forms.Button();
            this.mainsColorPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnDrawMain = new System.Windows.Forms.Button();
            this.branchColorDialog = new System.Windows.Forms.ColorDialog();
            this.mainsColorDialog = new System.Windows.Forms.ColorDialog();
            this.grBoxUnit = new System.Windows.Forms.GroupBox();
            this.rdButtMetric = new System.Windows.Forms.RadioButton();
            this.rdButtImperial = new System.Windows.Forms.RadioButton();
            this.systemTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.dryRadioButton = new System.Windows.Forms.RadioButton();
            this.wetRadioButton = new System.Windows.Forms.RadioButton();
            this.comboBoxSystemType = new System.Windows.Forms.ComboBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grBoxPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.grBoxUnit.SuspendLayout();
            this.systemTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.lblBranchPipeSubMaterial);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtBoxBranchCFactor);
            this.groupBox1.Controls.Add(this.branchPipeSubMaterialComboBox);
            this.groupBox1.Controls.Add(this.branchPipeMaterialComboBox);
            this.groupBox1.Controls.Add(this.chboxBranchLineWeight);
            this.groupBox1.Controls.Add(this.comboBranchLineWeight);
            this.groupBox1.Controls.Add(this.cBoxBranchSize);
            this.groupBox1.Controls.Add(this.branchWidth);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.linesColorpickerButton);
            this.groupBox1.Controls.Add(this.branchColorPanel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.groupBox1.Location = new System.Drawing.Point(254, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 216);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Branch Pipe Properties";
            // 
            // lblBranchPipeSubMaterial
            // 
            this.lblBranchPipeSubMaterial.AutoSize = true;
            this.lblBranchPipeSubMaterial.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBranchPipeSubMaterial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.lblBranchPipeSubMaterial.Location = new System.Drawing.Point(6, 50);
            this.lblBranchPipeSubMaterial.Name = "lblBranchPipeSubMaterial";
            this.lblBranchPipeSubMaterial.Size = new System.Drawing.Size(62, 15);
            this.lblBranchPipeSubMaterial.TabIndex = 16;
            this.lblBranchPipeSubMaterial.Text = "Schedule:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 15);
            this.label9.TabIndex = 15;
            this.label9.Text = "Pipe Material:";
            // 
            // txtBoxBranchCFactor
            // 
            this.txtBoxBranchCFactor.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxBranchCFactor.Location = new System.Drawing.Point(119, 186);
            this.txtBoxBranchCFactor.Name = "txtBoxBranchCFactor";
            this.txtBoxBranchCFactor.Size = new System.Drawing.Size(48, 22);
            this.txtBoxBranchCFactor.TabIndex = 2;
            this.txtBoxBranchCFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.branchWidth_KeyPress);
            // 
            // branchPipeSubMaterialComboBox
            // 
            this.branchPipeSubMaterialComboBox.BackColor = System.Drawing.Color.SteelBlue;
            this.branchPipeSubMaterialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.branchPipeSubMaterialComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.branchPipeSubMaterialComboBox.Font = new System.Drawing.Font("Arial", 9F);
            this.branchPipeSubMaterialComboBox.ForeColor = System.Drawing.Color.White;
            this.branchPipeSubMaterialComboBox.FormattingEnabled = true;
            this.branchPipeSubMaterialComboBox.Location = new System.Drawing.Point(119, 45);
            this.branchPipeSubMaterialComboBox.Name = "branchPipeSubMaterialComboBox";
            this.branchPipeSubMaterialComboBox.Size = new System.Drawing.Size(108, 23);
            this.branchPipeSubMaterialComboBox.TabIndex = 14;
            this.branchPipeSubMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.branchPipeSubMaterialComboBox_SelectedIndexChanged);
            // 
            // branchPipeMaterialComboBox
            // 
            this.branchPipeMaterialComboBox.BackColor = System.Drawing.Color.SteelBlue;
            this.branchPipeMaterialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.branchPipeMaterialComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.branchPipeMaterialComboBox.Font = new System.Drawing.Font("Arial", 9F);
            this.branchPipeMaterialComboBox.ForeColor = System.Drawing.Color.White;
            this.branchPipeMaterialComboBox.FormattingEnabled = true;
            this.branchPipeMaterialComboBox.Items.AddRange(new object[] {
            "Steel"});
            this.branchPipeMaterialComboBox.Location = new System.Drawing.Point(119, 16);
            this.branchPipeMaterialComboBox.Name = "branchPipeMaterialComboBox";
            this.branchPipeMaterialComboBox.Size = new System.Drawing.Size(108, 23);
            this.branchPipeMaterialComboBox.TabIndex = 13;
            this.branchPipeMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.branchMaterialComboBox_SelectedIndexChanged);
            // 
            // chboxBranchLineWeight
            // 
            this.chboxBranchLineWeight.AutoSize = true;
            this.chboxBranchLineWeight.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chboxBranchLineWeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.chboxBranchLineWeight.Location = new System.Drawing.Point(6, 162);
            this.chboxBranchLineWeight.Name = "chboxBranchLineWeight";
            this.chboxBranchLineWeight.Size = new System.Drawing.Size(111, 19);
            this.chboxBranchLineWeight.TabIndex = 12;
            this.chboxBranchLineWeight.Text = "Use line weight";
            this.chboxBranchLineWeight.UseVisualStyleBackColor = true;
            this.chboxBranchLineWeight.CheckedChanged += new System.EventHandler(this.chboxBranchLineWeight_CheckedChanged);
            // 
            // comboBranchLineWeight
            // 
            this.comboBranchLineWeight.BackColor = System.Drawing.Color.SteelBlue;
            this.comboBranchLineWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBranchLineWeight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBranchLineWeight.Font = new System.Drawing.Font("Arial", 9F);
            this.comboBranchLineWeight.ForeColor = System.Drawing.Color.White;
            this.comboBranchLineWeight.FormattingEnabled = true;
            this.comboBranchLineWeight.Location = new System.Drawing.Point(119, 157);
            this.comboBranchLineWeight.Name = "comboBranchLineWeight";
            this.comboBranchLineWeight.Size = new System.Drawing.Size(108, 23);
            this.comboBranchLineWeight.TabIndex = 0;
            // 
            // cBoxBranchSize
            // 
            this.cBoxBranchSize.BackColor = System.Drawing.Color.SteelBlue;
            this.cBoxBranchSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxBranchSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cBoxBranchSize.Font = new System.Drawing.Font("Arial", 9F);
            this.cBoxBranchSize.ForeColor = System.Drawing.Color.White;
            this.cBoxBranchSize.FormattingEnabled = true;
            this.cBoxBranchSize.Location = new System.Drawing.Point(119, 74);
            this.cBoxBranchSize.Name = "cBoxBranchSize";
            this.cBoxBranchSize.Size = new System.Drawing.Size(108, 23);
            this.cBoxBranchSize.TabIndex = 0;
            // 
            // branchWidth
            // 
            this.branchWidth.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.branchWidth.Location = new System.Drawing.Point(119, 129);
            this.branchWidth.Name = "branchWidth";
            this.branchWidth.Size = new System.Drawing.Size(91, 22);
            this.branchWidth.TabIndex = 2;
            this.branchWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.branchWidth_KeyPress);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label13.Location = new System.Drawing.Point(6, 190);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 15);
            this.label13.TabIndex = 8;
            this.label13.Text = "C Factor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label5.Location = new System.Drawing.Point(6, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Visual Width:";
            // 
            // linesColorpickerButton
            // 
            this.linesColorpickerButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.linesColorpickerButton.Location = new System.Drawing.Point(173, 102);
            this.linesColorpickerButton.Name = "linesColorpickerButton";
            this.linesColorpickerButton.Size = new System.Drawing.Size(37, 20);
            this.linesColorpickerButton.TabIndex = 1;
            this.linesColorpickerButton.Text = "•••";
            this.linesColorpickerButton.UseVisualStyleBackColor = true;
            this.linesColorpickerButton.Click += new System.EventHandler(this.branchColorpickerButton_Click);
            // 
            // branchColorPanel
            // 
            this.branchColorPanel.BackColor = System.Drawing.Color.Blue;
            this.branchColorPanel.Location = new System.Drawing.Point(119, 103);
            this.branchColorPanel.Name = "branchColorPanel";
            this.branchColorPanel.Size = new System.Drawing.Size(48, 20);
            this.branchColorPanel.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label3.Location = new System.Drawing.Point(6, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Select Color:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label1.Location = new System.Drawing.Point(6, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pipe Size:";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.deleteButton);
            this.groupBox3.Controls.Add(this.grBoxPreview);
            this.groupBox3.Controls.Add(this.blockRefListBox);
            this.groupBox3.Controls.Add(this.selectBlockButtons);
            this.groupBox3.Controls.Add(this.chBoxBreakPipe);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.groupBox3.Location = new System.Drawing.Point(9, 285);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(478, 193);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Trim Pipes on Custom Blocks";
            // 
            // grBoxPreview
            // 
            this.grBoxPreview.Controls.Add(this.PicBox);
            this.grBoxPreview.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grBoxPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(78)))), ((int)(((byte)(138)))));
            this.grBoxPreview.Location = new System.Drawing.Point(245, 11);
            this.grBoxPreview.Name = "grBoxPreview";
            this.grBoxPreview.Size = new System.Drawing.Size(220, 139);
            this.grBoxPreview.TabIndex = 9;
            this.grBoxPreview.TabStop = false;
            this.grBoxPreview.Text = "Preview";
            // 
            // PicBox
            // 
            this.PicBox.Location = new System.Drawing.Point(16, 22);
            this.PicBox.Margin = new System.Windows.Forms.Padding(2);
            this.PicBox.Name = "PicBox";
            this.PicBox.Size = new System.Drawing.Size(188, 105);
            this.PicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicBox.TabIndex = 2;
            this.PicBox.TabStop = false;
            // 
            // blockRefListBox
            // 
            this.blockRefListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.blockRefListBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blockRefListBox.ForeColor = System.Drawing.Color.White;
            this.blockRefListBox.FormattingEnabled = true;
            this.blockRefListBox.ItemHeight = 14;
            this.blockRefListBox.Location = new System.Drawing.Point(6, 20);
            this.blockRefListBox.Name = "blockRefListBox";
            this.blockRefListBox.Size = new System.Drawing.Size(230, 130);
            this.blockRefListBox.TabIndex = 1;
            this.blockRefListBox.SelectedIndexChanged += new System.EventHandler(this.blockRefListBox_SelectedIndexChanged);
            // 
            // selectBlockButtons
            // 
            this.selectBlockButtons.AutoSize = true;
            this.selectBlockButtons.BackColor = System.Drawing.Color.SteelBlue;
            this.selectBlockButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectBlockButtons.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectBlockButtons.ForeColor = System.Drawing.Color.White;
            this.selectBlockButtons.Location = new System.Drawing.Point(76, 156);
            this.selectBlockButtons.Name = "selectBlockButtons";
            this.selectBlockButtons.Size = new System.Drawing.Size(160, 29);
            this.selectBlockButtons.TabIndex = 0;
            this.selectBlockButtons.Text = "Select";
            this.selectBlockButtons.UseVisualStyleBackColor = false;
            this.selectBlockButtons.Click += new System.EventHandler(this.SelectCustomBlockButtons_Click);
            // 
            // chBoxBreakPipe
            // 
            this.chBoxBreakPipe.AutoSize = true;
            this.chBoxBreakPipe.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBoxBreakPipe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.chBoxBreakPipe.Location = new System.Drawing.Point(245, 161);
            this.chBoxBreakPipe.Name = "chBoxBreakPipe";
            this.chBoxBreakPipe.Size = new System.Drawing.Size(142, 19);
            this.chBoxBreakPipe.TabIndex = 12;
            this.chBoxBreakPipe.Text = "Use Cross Pipe Sign";
            this.chBoxBreakPipe.UseVisualStyleBackColor = true;
            this.chBoxBreakPipe.CheckedChanged += new System.EventHandler(this.chboxBranchLineWeight_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.lblMainPipeSubMaterial);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtBoxMainCFactor);
            this.groupBox2.Controls.Add(this.mainPipeSubMaterialComboBox);
            this.groupBox2.Controls.Add(this.mainPipeMaterialComboBox);
            this.groupBox2.Controls.Add(this.chboxMainLineWeight);
            this.groupBox2.Controls.Add(this.comboMainLineWeight);
            this.groupBox2.Controls.Add(this.cBoxMainSize);
            this.groupBox2.Controls.Add(this.mainWidth);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.mainsColorpickerButton);
            this.groupBox2.Controls.Add(this.mainsColorPanel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.groupBox2.Location = new System.Drawing.Point(12, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(233, 216);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Main Pipe Properties";
            // 
            // lblMainPipeSubMaterial
            // 
            this.lblMainPipeSubMaterial.AutoSize = true;
            this.lblMainPipeSubMaterial.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainPipeSubMaterial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.lblMainPipeSubMaterial.Location = new System.Drawing.Point(5, 48);
            this.lblMainPipeSubMaterial.Name = "lblMainPipeSubMaterial";
            this.lblMainPipeSubMaterial.Size = new System.Drawing.Size(62, 15);
            this.lblMainPipeSubMaterial.TabIndex = 20;
            this.lblMainPipeSubMaterial.Text = "Schedule:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label12.Location = new System.Drawing.Point(5, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 15);
            this.label12.TabIndex = 19;
            this.label12.Text = "Pipe Material:";
            // 
            // txtBoxMainCFactor
            // 
            this.txtBoxMainCFactor.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBoxMainCFactor.Location = new System.Drawing.Point(118, 186);
            this.txtBoxMainCFactor.Name = "txtBoxMainCFactor";
            this.txtBoxMainCFactor.Size = new System.Drawing.Size(48, 22);
            this.txtBoxMainCFactor.TabIndex = 2;
            this.txtBoxMainCFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.branchWidth_KeyPress);
            // 
            // mainPipeSubMaterialComboBox
            // 
            this.mainPipeSubMaterialComboBox.BackColor = System.Drawing.Color.SteelBlue;
            this.mainPipeSubMaterialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mainPipeSubMaterialComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mainPipeSubMaterialComboBox.Font = new System.Drawing.Font("Arial", 9F);
            this.mainPipeSubMaterialComboBox.ForeColor = System.Drawing.Color.White;
            this.mainPipeSubMaterialComboBox.FormattingEnabled = true;
            this.mainPipeSubMaterialComboBox.Location = new System.Drawing.Point(118, 45);
            this.mainPipeSubMaterialComboBox.Name = "mainPipeSubMaterialComboBox";
            this.mainPipeSubMaterialComboBox.Size = new System.Drawing.Size(108, 23);
            this.mainPipeSubMaterialComboBox.TabIndex = 18;
            this.mainPipeSubMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.mainPipeSubMaterialComboBox_SelectedIndexChanged);
            // 
            // mainPipeMaterialComboBox
            // 
            this.mainPipeMaterialComboBox.BackColor = System.Drawing.Color.SteelBlue;
            this.mainPipeMaterialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mainPipeMaterialComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mainPipeMaterialComboBox.Font = new System.Drawing.Font("Arial", 9F);
            this.mainPipeMaterialComboBox.ForeColor = System.Drawing.Color.White;
            this.mainPipeMaterialComboBox.FormattingEnabled = true;
            this.mainPipeMaterialComboBox.Items.AddRange(new object[] {
            "Steel"});
            this.mainPipeMaterialComboBox.Location = new System.Drawing.Point(118, 16);
            this.mainPipeMaterialComboBox.Name = "mainPipeMaterialComboBox";
            this.mainPipeMaterialComboBox.Size = new System.Drawing.Size(108, 23);
            this.mainPipeMaterialComboBox.TabIndex = 17;
            this.mainPipeMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.mainMaterialComboBox_SelectedIndexChanged);
            // 
            // chboxMainLineWeight
            // 
            this.chboxMainLineWeight.AutoSize = true;
            this.chboxMainLineWeight.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chboxMainLineWeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.chboxMainLineWeight.Location = new System.Drawing.Point(6, 160);
            this.chboxMainLineWeight.Name = "chboxMainLineWeight";
            this.chboxMainLineWeight.Size = new System.Drawing.Size(111, 19);
            this.chboxMainLineWeight.TabIndex = 12;
            this.chboxMainLineWeight.Text = "Use line weight";
            this.chboxMainLineWeight.UseVisualStyleBackColor = true;
            this.chboxMainLineWeight.CheckedChanged += new System.EventHandler(this.chboxMainLineWeight_CheckedChanged);
            // 
            // comboMainLineWeight
            // 
            this.comboMainLineWeight.BackColor = System.Drawing.Color.SteelBlue;
            this.comboMainLineWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMainLineWeight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboMainLineWeight.Font = new System.Drawing.Font("Arial", 9F);
            this.comboMainLineWeight.ForeColor = System.Drawing.Color.White;
            this.comboMainLineWeight.FormattingEnabled = true;
            this.comboMainLineWeight.Location = new System.Drawing.Point(118, 157);
            this.comboMainLineWeight.Name = "comboMainLineWeight";
            this.comboMainLineWeight.Size = new System.Drawing.Size(110, 23);
            this.comboMainLineWeight.TabIndex = 0;
            // 
            // cBoxMainSize
            // 
            this.cBoxMainSize.BackColor = System.Drawing.Color.SteelBlue;
            this.cBoxMainSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxMainSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cBoxMainSize.Font = new System.Drawing.Font("Arial", 9F);
            this.cBoxMainSize.ForeColor = System.Drawing.Color.White;
            this.cBoxMainSize.FormattingEnabled = true;
            this.cBoxMainSize.Location = new System.Drawing.Point(118, 74);
            this.cBoxMainSize.Name = "cBoxMainSize";
            this.cBoxMainSize.Size = new System.Drawing.Size(108, 23);
            this.cBoxMainSize.TabIndex = 0;
            // 
            // mainWidth
            // 
            this.mainWidth.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainWidth.Location = new System.Drawing.Point(118, 129);
            this.mainWidth.Name = "mainWidth";
            this.mainWidth.Size = new System.Drawing.Size(91, 22);
            this.mainWidth.TabIndex = 2;
            this.mainWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mainWidth_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label7.Location = new System.Drawing.Point(5, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 15);
            this.label7.TabIndex = 8;
            this.label7.Text = "C Factor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label2.Location = new System.Drawing.Point(5, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Visual Width:";
            // 
            // mainsColorpickerButton
            // 
            this.mainsColorpickerButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.mainsColorpickerButton.Location = new System.Drawing.Point(172, 103);
            this.mainsColorpickerButton.Name = "mainsColorpickerButton";
            this.mainsColorpickerButton.Size = new System.Drawing.Size(37, 20);
            this.mainsColorpickerButton.TabIndex = 1;
            this.mainsColorpickerButton.Text = "•••";
            this.mainsColorpickerButton.UseVisualStyleBackColor = true;
            this.mainsColorpickerButton.Click += new System.EventHandler(this.mainsColorPickerButton_Click);
            // 
            // mainsColorPanel
            // 
            this.mainsColorPanel.BackColor = System.Drawing.Color.Red;
            this.mainsColorPanel.Location = new System.Drawing.Point(118, 103);
            this.mainsColorPanel.Name = "mainsColorPanel";
            this.mainsColorPanel.Size = new System.Drawing.Size(48, 20);
            this.mainsColorPanel.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label4.Location = new System.Drawing.Point(5, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Select Color:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label6.Location = new System.Drawing.Point(5, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Pipe Size:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 14);
            this.label8.TabIndex = 3;
            // 
            // btnDrawMain
            // 
            this.btnDrawMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.btnDrawMain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDrawMain.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDrawMain.ForeColor = System.Drawing.Color.White;
            this.btnDrawMain.Location = new System.Drawing.Point(9, 483);
            this.btnDrawMain.Name = "btnDrawMain";
            this.btnDrawMain.Size = new System.Drawing.Size(478, 32);
            this.btnDrawMain.TabIndex = 0;
            this.btnDrawMain.Text = "Draw Pipe";
            this.btnDrawMain.UseVisualStyleBackColor = false;
            this.btnDrawMain.Click += new System.EventHandler(this.DrawPipeButton_Click);
            // 
            // branchColorDialog
            // 
            this.branchColorDialog.Color = System.Drawing.Color.Blue;
            // 
            // mainsColorDialog
            // 
            this.mainsColorDialog.Color = System.Drawing.Color.Red;
            // 
            // grBoxUnit
            // 
            this.grBoxUnit.Controls.Add(this.rdButtMetric);
            this.grBoxUnit.Controls.Add(this.rdButtImperial);
            this.grBoxUnit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.grBoxUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.grBoxUnit.Location = new System.Drawing.Point(12, 10);
            this.grBoxUnit.Name = "grBoxUnit";
            this.grBoxUnit.Size = new System.Drawing.Size(233, 48);
            this.grBoxUnit.TabIndex = 3;
            this.grBoxUnit.TabStop = false;
            this.grBoxUnit.Text = "Unit";
            // 
            // rdButtMetric
            // 
            this.rdButtMetric.AutoSize = true;
            this.rdButtMetric.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdButtMetric.Location = new System.Drawing.Point(118, 19);
            this.rdButtMetric.Name = "rdButtMetric";
            this.rdButtMetric.Size = new System.Drawing.Size(61, 20);
            this.rdButtMetric.TabIndex = 1;
            this.rdButtMetric.TabStop = true;
            this.rdButtMetric.Text = "Metric";
            this.rdButtMetric.UseVisualStyleBackColor = true;
            this.rdButtMetric.CheckedChanged += new System.EventHandler(this.rdButtMetric_CheckedChanged);
            // 
            // rdButtImperial
            // 
            this.rdButtImperial.AutoSize = true;
            this.rdButtImperial.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdButtImperial.Location = new System.Drawing.Point(9, 19);
            this.rdButtImperial.Name = "rdButtImperial";
            this.rdButtImperial.Size = new System.Drawing.Size(70, 20);
            this.rdButtImperial.TabIndex = 0;
            this.rdButtImperial.TabStop = true;
            this.rdButtImperial.Text = "Imperial";
            this.rdButtImperial.UseVisualStyleBackColor = true;
            this.rdButtImperial.CheckedChanged += new System.EventHandler(this.rdButtImperial_CheckedChanged);
            // 
            // systemTypeGroupBox
            // 
            this.systemTypeGroupBox.Controls.Add(this.dryRadioButton);
            this.systemTypeGroupBox.Controls.Add(this.wetRadioButton);
            this.systemTypeGroupBox.Controls.Add(this.comboBoxSystemType);
            this.systemTypeGroupBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.systemTypeGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.systemTypeGroupBox.Location = new System.Drawing.Point(254, 10);
            this.systemTypeGroupBox.Name = "systemTypeGroupBox";
            this.systemTypeGroupBox.Size = new System.Drawing.Size(233, 48);
            this.systemTypeGroupBox.TabIndex = 4;
            this.systemTypeGroupBox.TabStop = false;
            this.systemTypeGroupBox.Text = "System Type";
            // 
            // dryRadioButton
            // 
            this.dryRadioButton.AutoSize = true;
            this.dryRadioButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dryRadioButton.Location = new System.Drawing.Point(62, 19);
            this.dryRadioButton.Name = "dryRadioButton";
            this.dryRadioButton.Size = new System.Drawing.Size(45, 20);
            this.dryRadioButton.TabIndex = 1;
            this.dryRadioButton.TabStop = true;
            this.dryRadioButton.Text = "Dry";
            this.dryRadioButton.UseVisualStyleBackColor = true;
            this.dryRadioButton.CheckedChanged += new System.EventHandler(this.dryRadioButton_CheckedChanged);
            // 
            // wetRadioButton
            // 
            this.wetRadioButton.AutoSize = true;
            this.wetRadioButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wetRadioButton.Location = new System.Drawing.Point(10, 19);
            this.wetRadioButton.Name = "wetRadioButton";
            this.wetRadioButton.Size = new System.Drawing.Size(49, 20);
            this.wetRadioButton.TabIndex = 0;
            this.wetRadioButton.TabStop = true;
            this.wetRadioButton.Text = "Wet";
            this.wetRadioButton.UseVisualStyleBackColor = true;
            this.wetRadioButton.CheckedChanged += new System.EventHandler(this.wetRadioButton_CheckedChanged);
            // 
            // comboBoxSystemType
            // 
            this.comboBoxSystemType.BackColor = System.Drawing.Color.SteelBlue;
            this.comboBoxSystemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSystemType.DropDownWidth = 280;
            this.comboBoxSystemType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxSystemType.Font = new System.Drawing.Font("Arial", 9F);
            this.comboBoxSystemType.ForeColor = System.Drawing.Color.White;
            this.comboBoxSystemType.FormattingEnabled = true;
            this.comboBoxSystemType.Location = new System.Drawing.Point(119, 16);
            this.comboBoxSystemType.Name = "comboBoxSystemType";
            this.comboBoxSystemType.Size = new System.Drawing.Size(108, 23);
            this.comboBoxSystemType.TabIndex = 0;
            this.comboBoxSystemType.SelectedIndexChanged += new System.EventHandler(this.comboBoxSystemType_SelectedIndexChanged);
            // 
            // deleteButton
            // 
            this.deleteButton.AutoSize = true;
            this.deleteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(21)))), ((int)(((byte)(21)))));
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.ForeColor = System.Drawing.Color.White;
            this.deleteButton.Location = new System.Drawing.Point(6, 156);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(64, 29);
            this.deleteButton.TabIndex = 13;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // DrawPipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(496, 523);
            this.Controls.Add(this.systemTypeGroupBox);
            this.Controls.Add(this.grBoxUnit);
            this.Controls.Add(this.btnDrawMain);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DrawPipeForm";
            this.Text = "Pipe";
            this.Load += new System.EventHandler(this.prettyForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.prettyForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grBoxPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grBoxUnit.ResumeLayout(false);
            this.grBoxUnit.PerformLayout();
            this.systemTypeGroupBox.ResumeLayout(false);
            this.systemTypeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private CustomBorderTextBox LineLineWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel branchColorPanel;
        private System.Windows.Forms.Button linesColorpickerButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button mainsColorpickerButton;
        private System.Windows.Forms.Panel mainsColorPanel;
        private System.Windows.Forms.Label label4;
        private CustomBorderTextBox MainLineWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button selectBlockButtons;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnDrawMain;
        private System.Windows.Forms.ListBox blockRefListBox;
        private System.Windows.Forms.ColorDialog branchColorDialog;
        private System.Windows.Forms.ColorDialog mainsColorDialog;
        private CustomBorderTextBox mainWidth;
        private Label label2;
        private CustomBorderTextBox branchWidth;
        private Label label5;
        private GroupBox grBoxUnit;
        private RadioButton rdButtMetric;
        private RadioButton rdButtImperial;
        private ComboBox cBoxBranchSize;
        private ComboBox cBoxMainSize;
        private CheckBox chboxMainLineWeight;
        private CheckBox chboxBranchLineWeight;
        private ComboBox comboBranchLineWeight;
        private ComboBox comboMainLineWeight;
        private Label lblBranchPipeSubMaterial;
        private Label label9;
        private ComboBox branchPipeSubMaterialComboBox;
        private ComboBox branchPipeMaterialComboBox;
        private Label lblMainPipeSubMaterial;
        private Label label12;
        private ComboBox mainPipeSubMaterialComboBox;
        private ComboBox mainPipeMaterialComboBox;
        private GroupBox systemTypeGroupBox;
        private RadioButton dryRadioButton;
        private RadioButton wetRadioButton;
        private GroupBox grBoxPreview;
        private PictureBox PicBox;
        private CheckBox chBoxBreakPipe;
        private CustomBorderTextBox txtBoxMainCFactor;
        private ComboBox comboBoxSystemType;
        private CustomBorderTextBox txtBoxBranchCFactor;
        private Label label13;
        private Label label7;
        private Button deleteButton;
    }
}