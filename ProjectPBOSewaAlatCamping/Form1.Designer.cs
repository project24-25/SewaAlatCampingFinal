namespace ProjectPBOSewaAlatCamping
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            textBox1Loguser = new TextBox();
            textBox2Passuser = new TextBox();
            textBox3Reg = new TextBox();
            textBox4Pass = new TextBox();
            button1 = new Button();
            button2 = new Button();
            textBox1Emailreg = new TextBox();
            pictureBox1 = new PictureBox();
            checkBox1 = new CheckBox();
            checkBox2Show = new CheckBox();
            textBox1NoTelp = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // textBox1Loguser
            // 
            textBox1Loguser.Location = new Point(238, 438);
            textBox1Loguser.Name = "textBox1Loguser";
            textBox1Loguser.Size = new Size(167, 27);
            textBox1Loguser.TabIndex = 1;
            textBox1Loguser.Text = "\r\n";
            textBox1Loguser.TextAlign = HorizontalAlignment.Center;
            textBox1Loguser.TextChanged += textBox1Loguser_TextChanged;
            // 
            // textBox2Passuser
            // 
            textBox2Passuser.ForeColor = Color.Black;
            textBox2Passuser.Location = new Point(239, 511);
            textBox2Passuser.Name = "textBox2Passuser";
            textBox2Passuser.Size = new Size(167, 27);
            textBox2Passuser.TabIndex = 2;
            textBox2Passuser.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox3Reg
            // 
            textBox3Reg.Location = new Point(714, 437);
            textBox3Reg.Name = "textBox3Reg";
            textBox3Reg.Size = new Size(153, 27);
            textBox3Reg.TabIndex = 3;
            // 
            // textBox4Pass
            // 
            textBox4Pass.Location = new Point(713, 510);
            textBox4Pass.Name = "textBox4Pass";
            textBox4Pass.Size = new Size(153, 27);
            textBox4Pass.TabIndex = 4;
            // 
            // button1
            // 
            button1.Image = Properties.Resources.Screenshot_2025_05_24_004736;
            button1.Location = new Point(282, 583);
            button1.Name = "button1";
            button1.Size = new Size(135, 30);
            button1.TabIndex = 5;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Image = Properties.Resources.Screenshot_2025_05_26_104208;
            button2.Location = new Point(861, 590);
            button2.Name = "button2";
            button2.Size = new Size(118, 30);
            button2.TabIndex = 6;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox1Emailreg
            // 
            textBox1Emailreg.Location = new Point(936, 437);
            textBox1Emailreg.Name = "textBox1Emailreg";
            textBox1Emailreg.Size = new Size(153, 27);
            textBox1Emailreg.TabIndex = 7;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(25, 77);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1231, 721);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.MediumSlateBlue;
            checkBox1.Location = new Point(239, 550);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(132, 24);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "Show Password";
            checkBox1.UseVisualStyleBackColor = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2Show
            // 
            checkBox2Show.AutoSize = true;
            checkBox2Show.BackColor = Color.MediumSlateBlue;
            checkBox2Show.Location = new Point(708, 553);
            checkBox2Show.Name = "checkBox2Show";
            checkBox2Show.Size = new Size(132, 24);
            checkBox2Show.TabIndex = 9;
            checkBox2Show.Text = "Show Password";
            checkBox2Show.UseVisualStyleBackColor = false;
            checkBox2Show.CheckedChanged += checkBox2Show_CheckedChanged;
            // 
            // textBox1NoTelp
            // 
            textBox1NoTelp.Location = new Point(936, 512);
            textBox1NoTelp.Name = "textBox1NoTelp";
            textBox1NoTelp.Size = new Size(153, 27);
            textBox1NoTelp.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1323, 810);
            Controls.Add(textBox1NoTelp);
            Controls.Add(checkBox2Show);
            Controls.Add(checkBox1);
            Controls.Add(textBox3Reg);
            Controls.Add(button2);
            Controls.Add(textBox4Pass);
            Controls.Add(textBox1Emailreg);
            Controls.Add(button1);
            Controls.Add(textBox2Passuser);
            Controls.Add(textBox1Loguser);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1Loguser;
        private TextBox textBox2Passuser;
        private TextBox textBox3Reg;
        private TextBox textBox4Pass;
        private Button button1;
        private Button button2;
        private TextBox textBox1Emailreg;
        private PictureBox pictureBox1;
        private CheckBox checkBox1;
        private CheckBox checkBox2Show;
        private TextBox textBox1NoTelp;
    }
}
