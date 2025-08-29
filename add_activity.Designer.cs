namespace WinFormsApp1.Solver
{
    partial class add_activity
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
            weight_label = new Label();
            con1_label = new Label();
            con2_label = new Label();
            cance_button = new Button();
            okay_button = new Button();
            con1_text = new TextBox();
            con2_text = new TextBox();
            obj_fun_text = new TextBox();
            obj_label = new Label();
            SuspendLayout();
            // 
            // weight_label
            // 
            weight_label.AutoSize = true;
            weight_label.Location = new Point(142, 23);
            weight_label.Name = "weight_label";
            weight_label.Size = new Size(45, 15);
            weight_label.TabIndex = 0;
            weight_label.Text = "Weight";
            // 
            // con1_label
            // 
            con1_label.AutoSize = true;
            con1_label.Location = new Point(57, 94);
            con1_label.Name = "con1_label";
            con1_label.Size = new Size(71, 15);
            con1_label.TabIndex = 1;
            con1_label.Text = "Constraint 1";
            // 
            // con2_label
            // 
            con2_label.AutoSize = true;
            con2_label.Location = new Point(57, 137);
            con2_label.Name = "con2_label";
            con2_label.Size = new Size(71, 15);
            con2_label.TabIndex = 2;
            con2_label.Text = "Constriant 2";
            // 
            // cance_button
            // 
            cance_button.Location = new Point(23, 194);
            cance_button.Name = "cance_button";
            cance_button.Size = new Size(75, 23);
            cance_button.TabIndex = 3;
            cance_button.Text = "Cancel";
            cance_button.UseVisualStyleBackColor = true;
            cance_button.Click += cance_button_Click;
            // 
            // okay_button
            // 
            okay_button.Location = new Point(183, 194);
            okay_button.Name = "okay_button";
            okay_button.Size = new Size(75, 23);
            okay_button.TabIndex = 4;
            okay_button.Text = "OK";
            okay_button.UseVisualStyleBackColor = true;
            okay_button.Click += okay_button_Click;
            // 
            // con1_text
            // 
            con1_text.Location = new Point(148, 91);
            con1_text.Name = "con1_text";
            con1_text.Size = new Size(41, 23);
            con1_text.TabIndex = 5;
            // 
            // con2_text
            // 
            con2_text.Location = new Point(148, 134);
            con2_text.Name = "con2_text";
            con2_text.Size = new Size(41, 23);
            con2_text.TabIndex = 6;
            // 
            // obj_fun_text
            // 
            obj_fun_text.Location = new Point(148, 50);
            obj_fun_text.Name = "obj_fun_text";
            obj_fun_text.Size = new Size(41, 23);
            obj_fun_text.TabIndex = 8;
            // 
            // obj_label
            // 
            obj_label.AutoSize = true;
            obj_label.Location = new Point(23, 53);
            obj_label.Name = "obj_label";
            obj_label.Size = new Size(105, 15);
            obj_label.TabIndex = 7;
            obj_label.Text = "Objective function";
            // 
            // add_activity
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(270, 229);
            Controls.Add(obj_fun_text);
            Controls.Add(obj_label);
            Controls.Add(con2_text);
            Controls.Add(con1_text);
            Controls.Add(okay_button);
            Controls.Add(cance_button);
            Controls.Add(con2_label);
            Controls.Add(con1_label);
            Controls.Add(weight_label);
            Name = "add_activity";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add activity X-4";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label weight_label;
        private Label con1_label;
        private Label con2_label;
        private Button cance_button;
        private Button okay_button;
        private TextBox con1_text;
        private TextBox con2_text;
        private TextBox obj_fun_text;
        private Label obj_label;
    }
}