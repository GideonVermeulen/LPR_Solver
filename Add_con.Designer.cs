namespace WinFormsApp1.Solver
{
    partial class Add_con
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
            okay_button = new Button();
            close_button = new Button();
            x1_label = new Label();
            x2_label = new Label();
            x3_label = new Label();
            x1_text = new TextBox();
            x2_text = new TextBox();
            x3_text = new TextBox();
            sign_box = new ComboBox();
            sign_label = new Label();
            rhs_label = new Label();
            rhs_text = new TextBox();
            SuspendLayout();
            // 
            // okay_button
            // 
            okay_button.Location = new Point(294, 140);
            okay_button.Name = "okay_button";
            okay_button.Size = new Size(75, 23);
            okay_button.TabIndex = 0;
            okay_button.Text = "OK";
            okay_button.UseVisualStyleBackColor = true;
            okay_button.Click += okay_button_Click;
            // 
            // close_button
            // 
            close_button.Location = new Point(38, 140);
            close_button.Name = "close_button";
            close_button.Size = new Size(75, 23);
            close_button.TabIndex = 1;
            close_button.Text = "Cancel";
            close_button.UseVisualStyleBackColor = true;
            close_button.Click += close_button_Click;
            // 
            // x1_label
            // 
            x1_label.AutoSize = true;
            x1_label.Location = new Point(39, 30);
            x1_label.Name = "x1_label";
            x1_label.Size = new Size(25, 15);
            x1_label.TabIndex = 2;
            x1_label.Text = "X-1";
            x1_label.Click += label1_Click;
            // 
            // x2_label
            // 
            x2_label.AutoSize = true;
            x2_label.Location = new Point(115, 30);
            x2_label.Name = "x2_label";
            x2_label.Size = new Size(25, 15);
            x2_label.TabIndex = 3;
            x2_label.Text = "X-2";
            // 
            // x3_label
            // 
            x3_label.AutoSize = true;
            x3_label.Location = new Point(190, 30);
            x3_label.Name = "x3_label";
            x3_label.Size = new Size(25, 15);
            x3_label.TabIndex = 4;
            x3_label.Text = "X-3";
            // 
            // x1_text
            // 
            x1_text.Location = new Point(38, 62);
            x1_text.Name = "x1_text";
            x1_text.Size = new Size(39, 23);
            x1_text.TabIndex = 5;
            // 
            // x2_text
            // 
            x2_text.Location = new Point(115, 62);
            x2_text.Name = "x2_text";
            x2_text.Size = new Size(38, 23);
            x2_text.TabIndex = 6;
            // 
            // x3_text
            // 
            x3_text.Location = new Point(190, 62);
            x3_text.Name = "x3_text";
            x3_text.Size = new Size(38, 23);
            x3_text.TabIndex = 7;
            // 
            // sign_box
            // 
            sign_box.FormattingEnabled = true;
            sign_box.Items.AddRange(new object[] { "<=", ">=", "=" });
            sign_box.Location = new Point(270, 62);
            sign_box.Name = "sign_box";
            sign_box.Size = new Size(49, 23);
            sign_box.TabIndex = 8;
            // 
            // sign_label
            // 
            sign_label.AutoSize = true;
            sign_label.Location = new Point(270, 30);
            sign_label.Name = "sign_label";
            sign_label.Size = new Size(30, 15);
            sign_label.TabIndex = 9;
            sign_label.Text = "Sign";
            // 
            // rhs_label
            // 
            rhs_label.AutoSize = true;
            rhs_label.Location = new Point(349, 30);
            rhs_label.Name = "rhs_label";
            rhs_label.Size = new Size(29, 15);
            rhs_label.TabIndex = 10;
            rhs_label.Text = "RHS";
            // 
            // rhs_text
            // 
            rhs_text.Location = new Point(349, 62);
            rhs_text.Name = "rhs_text";
            rhs_text.Size = new Size(38, 23);
            rhs_text.TabIndex = 11;
            // 
            // Add_con
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(411, 183);
            Controls.Add(rhs_text);
            Controls.Add(rhs_label);
            Controls.Add(sign_label);
            Controls.Add(sign_box);
            Controls.Add(x3_text);
            Controls.Add(x2_text);
            Controls.Add(x1_text);
            Controls.Add(x3_label);
            Controls.Add(x2_label);
            Controls.Add(x1_label);
            Controls.Add(close_button);
            Controls.Add(okay_button);
            Name = "Add_con";
            Text = "Add a new Constraint";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button okay_button;
        private Button close_button;
        private Label x1_label;
        private Label x2_label;
        private Label x3_label;
        private TextBox x1_text;
        private TextBox x2_text;
        private TextBox x3_text;
        private ComboBox sign_box;
        private Label sign_label;
        private Label rhs_label;
        private TextBox rhs_text;
    }
}