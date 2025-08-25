namespace WinFormsApp1
{
    partial class addCon
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
            comboSign = new ComboBox();
            x1 = new TextBox();
            x2 = new TextBox();
            x3 = new TextBox();
            labelx1 = new Label();
            labelx2 = new Label();
            labelx3 = new Label();
            txtRhs = new TextBox();
            labelRhs = new Label();
            buttonOK = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // comboSign
            // 
            comboSign.FormattingEnabled = true;
            comboSign.Items.AddRange(new object[] { "=", "<=", ">=" });
            comboSign.Location = new Point(231, 55);
            comboSign.Name = "comboSign";
            comboSign.Size = new Size(50, 23);
            comboSign.TabIndex = 0;
            // 
            // x1
            // 
            x1.Location = new Point(22, 55);
            x1.Name = "x1";
            x1.Size = new Size(37, 23);
            x1.TabIndex = 1;
            // 
            // x2
            // 
            x2.Location = new Point(91, 55);
            x2.Name = "x2";
            x2.Size = new Size(37, 23);
            x2.TabIndex = 2;
            // 
            // x3
            // 
            x3.Location = new Point(162, 55);
            x3.Name = "x3";
            x3.Size = new Size(37, 23);
            x3.TabIndex = 3;
            // 
            // labelx1
            // 
            labelx1.AutoSize = true;
            labelx1.Location = new Point(23, 30);
            labelx1.Name = "labelx1";
            labelx1.Size = new Size(25, 15);
            labelx1.TabIndex = 4;
            labelx1.Text = "X-1";
            // 
            // labelx2
            // 
            labelx2.AutoSize = true;
            labelx2.Location = new Point(91, 30);
            labelx2.Name = "labelx2";
            labelx2.Size = new Size(25, 15);
            labelx2.TabIndex = 5;
            labelx2.Text = "X-2";
            // 
            // labelx3
            // 
            labelx3.AutoSize = true;
            labelx3.Location = new Point(162, 30);
            labelx3.Name = "labelx3";
            labelx3.Size = new Size(25, 15);
            labelx3.TabIndex = 6;
            labelx3.Text = "X-3";
            // 
            // txtRhs
            // 
            txtRhs.Location = new Point(313, 55);
            txtRhs.Name = "txtRhs";
            txtRhs.Size = new Size(37, 23);
            txtRhs.TabIndex = 7;
            // 
            // labelRhs
            // 
            labelRhs.AutoSize = true;
            labelRhs.Location = new Point(313, 30);
            labelRhs.Name = "labelRhs";
            labelRhs.Size = new Size(29, 15);
            labelRhs.TabIndex = 8;
            labelRhs.Text = "RHS";
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(247, 105);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(75, 23);
            buttonOK.TabIndex = 9;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.ForeColor = Color.OrangeRed;
            buttonCancel.Location = new Point(53, 105);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // addCon
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 140);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(labelRhs);
            Controls.Add(txtRhs);
            Controls.Add(labelx3);
            Controls.Add(labelx2);
            Controls.Add(labelx1);
            Controls.Add(x3);
            Controls.Add(x2);
            Controls.Add(x1);
            Controls.Add(comboSign);
            Name = "addCon";
            Text = "Add Condition";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboSign;
        private TextBox x1;
        private TextBox x2;
        private TextBox x3;
        private Label labelx1;
        private Label labelx2;
        private Label labelx3;
        private TextBox txtRhs;
        private Label labelRhs;
        private Button buttonOK;
        private Button buttonCancel;
    }
}