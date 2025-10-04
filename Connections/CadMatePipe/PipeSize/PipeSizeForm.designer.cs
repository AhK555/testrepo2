using System;

namespace PipeApp
{
    partial class PipeSizeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipeSizeForm));
            this.grBoxUnit = new System.Windows.Forms.GroupBox();
            this.rdButtMetric = new System.Windows.Forms.RadioButton();
            this.rdButtImperial = new System.Windows.Forms.RadioButton();
            this.grBoxSize = new System.Windows.Forms.GroupBox();
            this.rdnPipeSchedule = new System.Windows.Forms.RadioButton();
            this.rdnPreviosSize = new System.Windows.Forms.RadioButton();
            this.cBoxSize = new System.Windows.Forms.ComboBox();
            this.pipeSizeLabel = new System.Windows.Forms.Label();
            this.buttDone = new System.Windows.Forms.Button();
            this.fontSettingGroupBox = new System.Windows.Forms.GroupBox();
            this.fontSizeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fontstyleComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rdnSetNewSiz = new System.Windows.Forms.RadioButton();
            this.grBoxUnit.SuspendLayout();
            this.grBoxSize.SuspendLayout();
            this.fontSettingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // grBoxUnit
            // 
            this.grBoxUnit.Controls.Add(this.rdButtMetric);
            this.grBoxUnit.Controls.Add(this.rdButtImperial);
            this.grBoxUnit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.grBoxUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.grBoxUnit.Location = new System.Drawing.Point(8, 5);
            this.grBoxUnit.Name = "grBoxUnit";
            this.grBoxUnit.Size = new System.Drawing.Size(222, 54);
            this.grBoxUnit.TabIndex = 1;
            this.grBoxUnit.TabStop = false;
            this.grBoxUnit.Text = "Unit";
            // 
            // rdButtMetric
            // 
            this.rdButtMetric.AutoSize = true;
            this.rdButtMetric.Font = new System.Drawing.Font("Arial", 9F);
            this.rdButtMetric.Location = new System.Drawing.Point(112, 23);
            this.rdButtMetric.Name = "rdButtMetric";
            this.rdButtMetric.Size = new System.Drawing.Size(57, 19);
            this.rdButtMetric.TabIndex = 0;
            this.rdButtMetric.TabStop = true;
            this.rdButtMetric.Text = "Metric";
            this.rdButtMetric.UseVisualStyleBackColor = true;
            this.rdButtMetric.CheckedChanged += new System.EventHandler(this.rdButtMetric_CheckedChanged);
            // 
            // rdButtImperial
            // 
            this.rdButtImperial.AutoSize = true;
            this.rdButtImperial.Font = new System.Drawing.Font("Arial", 9F);
            this.rdButtImperial.Location = new System.Drawing.Point(13, 23);
            this.rdButtImperial.Name = "rdButtImperial";
            this.rdButtImperial.Size = new System.Drawing.Size(70, 19);
            this.rdButtImperial.TabIndex = 0;
            this.rdButtImperial.TabStop = true;
            this.rdButtImperial.Text = "Imperial";
            this.rdButtImperial.UseVisualStyleBackColor = true;
            this.rdButtImperial.CheckedChanged += new System.EventHandler(this.rdButtImperial_CheckedChanged);
            // 
            // grBoxSize
            // 
            this.grBoxSize.Controls.Add(this.rdnSetNewSiz);
            this.grBoxSize.Controls.Add(this.rdnPipeSchedule);
            this.grBoxSize.Controls.Add(this.rdnPreviosSize);
            this.grBoxSize.Controls.Add(this.cBoxSize);
            this.grBoxSize.Controls.Add(this.pipeSizeLabel);
            this.grBoxSize.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.grBoxSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.grBoxSize.Location = new System.Drawing.Point(8, 65);
            this.grBoxSize.Name = "grBoxSize";
            this.grBoxSize.Size = new System.Drawing.Size(222, 119);
            this.grBoxSize.TabIndex = 2;
            this.grBoxSize.TabStop = false;
            this.grBoxSize.Text = "Sizing Method";
            // 
            // rdnPipeSchedule
            // 
            this.rdnPipeSchedule.AutoSize = true;
            this.rdnPipeSchedule.Font = new System.Drawing.Font("Arial", 9F);
            this.rdnPipeSchedule.Location = new System.Drawing.Point(13, 47);
            this.rdnPipeSchedule.Name = "rdnPipeSchedule";
            this.rdnPipeSchedule.Size = new System.Drawing.Size(191, 19);
            this.rdnPipeSchedule.TabIndex = 0;
            this.rdnPipeSchedule.TabStop = true;
            this.rdnPipeSchedule.Text = "Apply Size from Pipe Schedule";
            this.rdnPipeSchedule.UseVisualStyleBackColor = true;
            this.rdnPipeSchedule.CheckedChanged += new System.EventHandler(this.pipeSched_CheckedChanged);
            // 
            // rdnPreviosSize
            // 
            this.rdnPreviosSize.AutoSize = true;
            this.rdnPreviosSize.Font = new System.Drawing.Font("Arial", 9F);
            this.rdnPreviosSize.Location = new System.Drawing.Point(14, 23);
            this.rdnPreviosSize.Name = "rdnPreviosSize";
            this.rdnPreviosSize.Size = new System.Drawing.Size(198, 19);
            this.rdnPreviosSize.TabIndex = 0;
            this.rdnPreviosSize.TabStop = true;
            this.rdnPreviosSize.Text = "Apply Size from Pipe Command";
            this.rdnPreviosSize.UseVisualStyleBackColor = true;
            this.rdnPreviosSize.CheckedChanged += new System.EventHandler(this.UsePreviousData_CheckedChanged);
            // 
            // cBoxSize
            // 
            this.cBoxSize.BackColor = System.Drawing.Color.SteelBlue;
            this.cBoxSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cBoxSize.Font = new System.Drawing.Font("Arial", 9F);
            this.cBoxSize.ForeColor = System.Drawing.Color.White;
            this.cBoxSize.FormattingEnabled = true;
            this.cBoxSize.Location = new System.Drawing.Point(118, 89);
            this.cBoxSize.Name = "cBoxSize";
            this.cBoxSize.Size = new System.Drawing.Size(86, 23);
            this.cBoxSize.TabIndex = 13;
            // 
            // pipeSizeLabel
            // 
            this.pipeSizeLabel.AutoSize = true;
            this.pipeSizeLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pipeSizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.pipeSizeLabel.Location = new System.Drawing.Point(17, 94);
            this.pipeSizeLabel.Name = "pipeSizeLabel";
            this.pipeSizeLabel.Size = new System.Drawing.Size(61, 15);
            this.pipeSizeLabel.TabIndex = 12;
            this.pipeSizeLabel.Text = "Pipe Size:";
            // 
            // buttDone
            // 
            this.buttDone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.buttDone.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.buttDone.ForeColor = System.Drawing.Color.White;
            this.buttDone.Location = new System.Drawing.Point(8, 276);
            this.buttDone.Name = "buttDone";
            this.buttDone.Size = new System.Drawing.Size(222, 35);
            this.buttDone.TabIndex = 0;
            this.buttDone.Text = "Done";
            this.buttDone.UseVisualStyleBackColor = false;
            this.buttDone.Click += new System.EventHandler(this.ButtonAssign_Click);
            // 
            // fontSettingGroupBox
            // 
            this.fontSettingGroupBox.Controls.Add(this.fontSizeTextBox);
            this.fontSettingGroupBox.Controls.Add(this.label3);
            this.fontSettingGroupBox.Controls.Add(this.fontstyleComboBox);
            this.fontSettingGroupBox.Controls.Add(this.label2);
            this.fontSettingGroupBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.fontSettingGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.fontSettingGroupBox.Location = new System.Drawing.Point(8, 190);
            this.fontSettingGroupBox.Name = "fontSettingGroupBox";
            this.fontSettingGroupBox.Size = new System.Drawing.Size(222, 80);
            this.fontSettingGroupBox.TabIndex = 3;
            this.fontSettingGroupBox.TabStop = false;
            this.fontSettingGroupBox.Text = "Font Settings";
            // 
            // fontSizeTextBox
            // 
            this.fontSizeTextBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fontSizeTextBox.Location = new System.Drawing.Point(94, 49);
            this.fontSizeTextBox.Name = "fontSizeTextBox";
            this.fontSizeTextBox.Size = new System.Drawing.Size(110, 22);
            this.fontSizeTextBox.TabIndex = 15;
            this.fontSizeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fontSizeTextBox_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label3.Location = new System.Drawing.Point(11, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Font Height:";
            // 
            // fontstyleComboBox
            // 
            this.fontstyleComboBox.BackColor = System.Drawing.Color.SteelBlue;
            this.fontstyleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontstyleComboBox.DropDownWidth = 180;
            this.fontstyleComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fontstyleComboBox.Font = new System.Drawing.Font("Arial", 9F);
            this.fontstyleComboBox.ForeColor = System.Drawing.Color.White;
            this.fontstyleComboBox.FormattingEnabled = true;
            this.fontstyleComboBox.Location = new System.Drawing.Point(94, 21);
            this.fontstyleComboBox.Name = "fontstyleComboBox";
            this.fontstyleComboBox.Size = new System.Drawing.Size(110, 23);
            this.fontstyleComboBox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(87)))), ((int)(((byte)(138)))));
            this.label2.Location = new System.Drawing.Point(11, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "Font Style:";
            // 
            // rdnSetNewSiz
            // 
            this.rdnSetNewSiz.AutoSize = true;
            this.rdnSetNewSiz.Font = new System.Drawing.Font("Arial", 9F);
            this.rdnSetNewSiz.Location = new System.Drawing.Point(13, 71);
            this.rdnSetNewSiz.Name = "rdnSetNewSiz";
            this.rdnSetNewSiz.Size = new System.Drawing.Size(97, 19);
            this.rdnSetNewSiz.TabIndex = 0;
            this.rdnSetNewSiz.TabStop = true;
            this.rdnSetNewSiz.Text = "Set New Size";
            this.rdnSetNewSiz.UseVisualStyleBackColor = true;
            this.rdnSetNewSiz.CheckedChanged += new System.EventHandler(this.SetNewSize_CheckedChanged);
            // 
            // PipeSizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 318);
            this.Controls.Add(this.fontSettingGroupBox);
            this.Controls.Add(this.buttDone);
            this.Controls.Add(this.grBoxSize);
            this.Controls.Add(this.grBoxUnit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "PipeSizeForm";
            this.Text = "Pipe Size";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PipeSizeForm_FormClosed);
            this.Load += new System.EventHandler(this.PipeSizeForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PipeSizeForm_KeyDown);
            this.grBoxUnit.ResumeLayout(false);
            this.grBoxUnit.PerformLayout();
            this.grBoxSize.ResumeLayout(false);
            this.grBoxSize.PerformLayout();
            this.fontSettingGroupBox.ResumeLayout(false);
            this.fontSettingGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        



        #endregion

        private System.Windows.Forms.GroupBox grBoxUnit;
        private System.Windows.Forms.RadioButton rdButtMetric;
        private System.Windows.Forms.RadioButton rdButtImperial;
        private System.Windows.Forms.GroupBox grBoxSize;
        private System.Windows.Forms.ComboBox cBoxSize;
        private System.Windows.Forms.Label pipeSizeLabel;
        private System.Windows.Forms.Button buttDone;
        private System.Windows.Forms.GroupBox fontSettingGroupBox;
        private System.Windows.Forms.ComboBox fontstyleComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fontSizeTextBox;
        private System.Windows.Forms.RadioButton rdnPipeSchedule;
        private System.Windows.Forms.RadioButton rdnPreviosSize;
        private System.Windows.Forms.RadioButton rdnSetNewSiz;
    }
}