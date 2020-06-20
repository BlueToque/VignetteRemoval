namespace VignetteRemoval.Demo
{
    partial class MainForm
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
            this.myPictureBox = new System.Windows.Forms.PictureBox();
            this.myLoadButton = new System.Windows.Forms.Button();
            this.mySaveButton = new System.Windows.Forms.Button();
            this.myRemoveVignetteButton = new System.Windows.Forms.Button();
            this.myProcessedLabel = new System.Windows.Forms.Label();
            this.myFlipButton = new System.Windows.Forms.Button();
            this.mySaveDataButton = new System.Windows.Forms.Button();
            this.myLoadDataButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.myPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // myPictureBox
            // 
            this.myPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myPictureBox.Location = new System.Drawing.Point(93, 12);
            this.myPictureBox.Name = "myPictureBox";
            this.myPictureBox.Size = new System.Drawing.Size(710, 378);
            this.myPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.myPictureBox.TabIndex = 0;
            this.myPictureBox.TabStop = false;
            // 
            // myLoadButton
            // 
            this.myLoadButton.Location = new System.Drawing.Point(12, 12);
            this.myLoadButton.Name = "myLoadButton";
            this.myLoadButton.Size = new System.Drawing.Size(75, 23);
            this.myLoadButton.TabIndex = 1;
            this.myLoadButton.Text = "Load";
            this.myLoadButton.UseVisualStyleBackColor = true;
            this.myLoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // mySaveButton
            // 
            this.mySaveButton.Location = new System.Drawing.Point(12, 41);
            this.mySaveButton.Name = "mySaveButton";
            this.mySaveButton.Size = new System.Drawing.Size(75, 23);
            this.mySaveButton.TabIndex = 2;
            this.mySaveButton.Text = "Save";
            this.mySaveButton.UseVisualStyleBackColor = true;
            this.mySaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // myRemoveVignetteButton
            // 
            this.myRemoveVignetteButton.Location = new System.Drawing.Point(11, 113);
            this.myRemoveVignetteButton.Name = "myRemoveVignetteButton";
            this.myRemoveVignetteButton.Size = new System.Drawing.Size(75, 41);
            this.myRemoveVignetteButton.TabIndex = 3;
            this.myRemoveVignetteButton.Text = "Remove Vignette";
            this.myRemoveVignetteButton.UseVisualStyleBackColor = true;
            this.myRemoveVignetteButton.Click += new System.EventHandler(this.RemoveVignetteButton_Click);
            // 
            // myProcessedLabel
            // 
            this.myProcessedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myProcessedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myProcessedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myProcessedLabel.Location = new System.Drawing.Point(93, 402);
            this.myProcessedLabel.Name = "myProcessedLabel";
            this.myProcessedLabel.Size = new System.Drawing.Size(710, 28);
            this.myProcessedLabel.TabIndex = 11;
            this.myProcessedLabel.Text = "Original";
            this.myProcessedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myFlipButton
            // 
            this.myFlipButton.Location = new System.Drawing.Point(11, 160);
            this.myFlipButton.Name = "myFlipButton";
            this.myFlipButton.Size = new System.Drawing.Size(75, 41);
            this.myFlipButton.TabIndex = 12;
            this.myFlipButton.Text = "Flip";
            this.myFlipButton.UseVisualStyleBackColor = true;
            this.myFlipButton.Click += new System.EventHandler(this.FlipButton_Click);
            // 
            // mySaveDataButton
            // 
            this.mySaveDataButton.Location = new System.Drawing.Point(11, 254);
            this.mySaveDataButton.Name = "mySaveDataButton";
            this.mySaveDataButton.Size = new System.Drawing.Size(75, 41);
            this.mySaveDataButton.TabIndex = 13;
            this.mySaveDataButton.Text = "Save Vignette";
            this.mySaveDataButton.UseVisualStyleBackColor = true;
            this.mySaveDataButton.Click += new System.EventHandler(this.SaveDataButton_Click);
            // 
            // myLoadDataButton
            // 
            this.myLoadDataButton.Location = new System.Drawing.Point(11, 207);
            this.myLoadDataButton.Name = "myLoadDataButton";
            this.myLoadDataButton.Size = new System.Drawing.Size(75, 41);
            this.myLoadDataButton.TabIndex = 14;
            this.myLoadDataButton.Text = "Load Vignette";
            this.myLoadDataButton.UseVisualStyleBackColor = true;
            this.myLoadDataButton.Click += new System.EventHandler(this.LoadDataButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "ASIV",
            "LIE",
            "HILL"});
            this.comboBox1.Location = new System.Drawing.Point(11, 86);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(74, 21);
            this.comboBox1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Algortithm";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 439);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.myLoadDataButton);
            this.Controls.Add(this.mySaveDataButton);
            this.Controls.Add(this.myFlipButton);
            this.Controls.Add(this.myProcessedLabel);
            this.Controls.Add(this.myRemoveVignetteButton);
            this.Controls.Add(this.mySaveButton);
            this.Controls.Add(this.myLoadButton);
            this.Controls.Add(this.myPictureBox);
            this.Name = "MainForm";
            this.Text = "Vignette Removal";
            ((System.ComponentModel.ISupportInitialize)(this.myPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox myPictureBox;
        private System.Windows.Forms.Button myLoadButton;
        private System.Windows.Forms.Button mySaveButton;
        private System.Windows.Forms.Button myRemoveVignetteButton;
        private System.Windows.Forms.Label myProcessedLabel;
        private System.Windows.Forms.Button myFlipButton;
        private System.Windows.Forms.Button mySaveDataButton;
        private System.Windows.Forms.Button myLoadDataButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
    }
}

