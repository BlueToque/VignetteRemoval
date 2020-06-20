namespace VignetteRemoval
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
            this.myRemoveVignetteButton.Location = new System.Drawing.Point(12, 70);
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
            this.myFlipButton.Location = new System.Drawing.Point(12, 117);
            this.myFlipButton.Name = "myFlipButton";
            this.myFlipButton.Size = new System.Drawing.Size(75, 41);
            this.myFlipButton.TabIndex = 12;
            this.myFlipButton.Text = "Flip";
            this.myFlipButton.UseVisualStyleBackColor = true;
            this.myFlipButton.Click += new System.EventHandler(this.FlipButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 439);
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

        }

        #endregion

        private System.Windows.Forms.PictureBox myPictureBox;
        private System.Windows.Forms.Button myLoadButton;
        private System.Windows.Forms.Button mySaveButton;
        private System.Windows.Forms.Button myRemoveVignetteButton;
        private System.Windows.Forms.Label myProcessedLabel;
        private System.Windows.Forms.Button myFlipButton;
    }
}

