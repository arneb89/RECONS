namespace CONTROL
{
    partial class SurfaceViewerForm
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
            this.btnMaxColor = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnMinColor = new System.Windows.Forms.Button();
            this.btnShowLines = new System.Windows.Forms.Button();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.txtLong = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSetPath = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSaveMapData = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMaxColor
            // 
            this.btnMaxColor.Location = new System.Drawing.Point(372, 14);
            this.btnMaxColor.Name = "btnMaxColor";
            this.btnMaxColor.Size = new System.Drawing.Size(115, 23);
            this.btnMaxColor.TabIndex = 0;
            this.btnMaxColor.Text = "Max Color";
            this.btnMaxColor.UseVisualStyleBackColor = true;
            this.btnMaxColor.Click += new System.EventHandler(this.btnMaxColor_Click);
            // 
            // btnMinColor
            // 
            this.btnMinColor.Location = new System.Drawing.Point(372, 43);
            this.btnMinColor.Name = "btnMinColor";
            this.btnMinColor.Size = new System.Drawing.Size(115, 23);
            this.btnMinColor.TabIndex = 1;
            this.btnMinColor.Text = "Min Color";
            this.btnMinColor.UseVisualStyleBackColor = true;
            this.btnMinColor.Click += new System.EventHandler(this.btnMinColor_Click);
            // 
            // btnShowLines
            // 
            this.btnShowLines.Location = new System.Drawing.Point(291, 6);
            this.btnShowLines.Name = "btnShowLines";
            this.btnShowLines.Size = new System.Drawing.Size(75, 65);
            this.btnShowLines.TabIndex = 2;
            this.btnShowLines.Text = "Show Lines";
            this.btnShowLines.UseVisualStyleBackColor = true;
            this.btnShowLines.Click += new System.EventHandler(this.btnShowLines_Click);
            // 
            // txtLat
            // 
            this.txtLat.Location = new System.Drawing.Point(648, 3);
            this.txtLat.Name = "txtLat";
            this.txtLat.ReadOnly = true;
            this.txtLat.Size = new System.Drawing.Size(77, 20);
            this.txtLat.TabIndex = 3;
            // 
            // txtLong
            // 
            this.txtLong.Location = new System.Drawing.Point(648, 29);
            this.txtLong.Name = "txtLong";
            this.txtLong.ReadOnly = true;
            this.txtLong.Size = new System.Drawing.Size(77, 20);
            this.txtLong.TabIndex = 4;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(648, 55);
            this.txtValue.Name = "txtValue";
            this.txtValue.ReadOnly = true;
            this.txtValue.Size = new System.Drawing.Size(77, 20);
            this.txtValue.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(597, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Latitude";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(588, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Longitude";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(608, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Value";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.btnSaveMapData);
            this.panel1.Controls.Add(this.btnSetPath);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.btnSaveImage);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.txtValue);
            this.panel1.Controls.Add(this.btnMaxColor);
            this.panel1.Controls.Add(this.btnMinColor);
            this.panel1.Controls.Add(this.btnShowLines);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtLat);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtLong);
            this.panel1.Location = new System.Drawing.Point(12, 399);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 82);
            this.panel1.TabIndex = 9;
            // 
            // btnSetPath
            // 
            this.btnSetPath.Location = new System.Drawing.Point(14, 6);
            this.btnSetPath.Name = "btnSetPath";
            this.btnSetPath.Size = new System.Drawing.Size(58, 23);
            this.btnSetPath.TabIndex = 13;
            this.btnSetPath.Text = "Path";
            this.btnSetPath.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(78, 6);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(207, 20);
            this.textBox3.TabIndex = 12;
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(116, 35);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(96, 36);
            this.btnSaveImage.TabIndex = 11;
            this.btnSaveImage.Text = "Save Image";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(218, 55);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(67, 20);
            this.textBox2.TabIndex = 10;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(218, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(67, 20);
            this.textBox1.TabIndex = 9;
            // 
            // btnSaveMapData
            // 
            this.btnSaveMapData.Location = new System.Drawing.Point(14, 36);
            this.btnSaveMapData.Name = "btnSaveMapData";
            this.btnSaveMapData.Size = new System.Drawing.Size(96, 36);
            this.btnSaveMapData.TabIndex = 14;
            this.btnSaveMapData.Text = "Save Map Data";
            this.btnSaveMapData.UseVisualStyleBackColor = true;
            this.btnSaveMapData.Click += new System.EventHandler(this.btnSaveMapData_Click);
            // 
            // SurfaceViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(752, 493);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SurfaceViewerForm";
            this.Text = "SurfaceViewer";
            this.Load += new System.EventHandler(this.SurfaceViewerForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMaxColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnMinColor;
        private System.Windows.Forms.Button btnShowLines;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.TextBox txtLong;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnSetPath;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button btnSaveMapData;




    }
}