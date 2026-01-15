namespace Screenshot_OCR
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            richTextBox1 = new RichTextBox();
            pictureBox2 = new PictureBox();
            label1 = new Label();
            panel1 = new Panel();
            btnFile = new Button();
            btnImage = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(326, 383);
            button1.Name = "button1";
            button1.Size = new Size(143, 29);
            button1.TabIndex = 0;
            button1.Text = "Chụp màn hình";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(445, 47);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(327, 307);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(14, 15);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(340, 259);
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(350, 9);
            label1.Name = "label1";
            label1.Size = new Size(110, 20);
            label1.TabIndex = 4;
            label1.Text = "VINH ĐẸP TRAI";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Controls.Add(pictureBox2);
            panel1.Location = new Point(12, 47);
            panel1.Name = "panel1";
            panel1.Size = new Size(373, 307);
            panel1.TabIndex = 5;
            // 
            // btnFile
            // 
            btnFile.Location = new Point(557, 383);
            btnFile.Name = "btnFile";
            btnFile.Size = new Size(94, 29);
            btnFile.TabIndex = 6;
            btnFile.Text = "Lưu file .txt";
            btnFile.UseVisualStyleBackColor = true;
            btnFile.Click += btnFile_Click;
            // 
            // btnImage
            // 
            btnImage.Location = new Point(131, 383);
            btnImage.Name = "btnImage";
            btnImage.Size = new Size(94, 29);
            btnImage.TabIndex = 7;
            btnImage.Text = "Lưu ảnh";
            btnImage.UseVisualStyleBackColor = true;
            btnImage.Click += btnImage_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnImage);
            Controls.Add(btnFile);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(richTextBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private RichTextBox richTextBox1;
        private PictureBox pictureBox2;
        private Label label1;
        private Panel panel1;
        private Button btnFile;
        private Button btnImage;
    }
}
