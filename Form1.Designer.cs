namespace WinFormsApp1
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
            tableLayoutPanel1 = new TableLayoutPanel();
            loadButton = new Button();
            export_button = new Button();
            algorithmComboBox = new ComboBox();
            solveButton = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            inputTextBox = new TextBox();
            outputTextBox = new TextBox();
            panel1 = new Panel();
            panel2 = new Panel();
            sensitivityGroupBox = new GroupBox();
            add_con_button = new Button();
            performAnalysisButton = new Button();
            analysisInputTextBox = new TextBox();
            shadowPricesRadioButton = new RadioButton();
            rhsRangeRadioButton = new RadioButton();
            basicVarRangeRadioButton = new RadioButton();
            nonBasicVarRangeRadioButton = new RadioButton();
            dualProblemRadioButton = new RadioButton();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            sensitivityGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(45, 45, 48);
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(loadButton, 0, 0);
            tableLayoutPanel1.Controls.Add(export_button, 1, 0);
            tableLayoutPanel1.Controls.Add(algorithmComboBox, 2, 0);
            tableLayoutPanel1.Controls.Add(solveButton, 3, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1019, 40);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // loadButton
            // 
            loadButton.BackColor = Color.FromArgb(0, 122, 204);
            loadButton.Dock = DockStyle.Fill;
            loadButton.FlatAppearance.BorderSize = 0;
            loadButton.FlatStyle = FlatStyle.Flat;
            loadButton.ForeColor = Color.White;
            loadButton.Location = new Point(3, 3);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(248, 34);
            loadButton.TabIndex = 0;
            loadButton.Text = "Load LP Problem";
            loadButton.UseVisualStyleBackColor = false;
            loadButton.Click += loadButton_Click;
            // 
            // export_button
            // 
            export_button.BackColor = Color.FromArgb(0, 122, 204);
            export_button.Dock = DockStyle.Fill;
            export_button.FlatAppearance.BorderSize = 0;
            export_button.FlatStyle = FlatStyle.Flat;
            export_button.ForeColor = Color.White;
            export_button.Location = new Point(257, 3);
            export_button.Name = "export_button";
            export_button.Size = new Size(248, 34);
            export_button.TabIndex = 3;
            export_button.Text = "Export to Text";
            export_button.UseVisualStyleBackColor = false;
            export_button.Click += export_button_Click;
            // 
            // algorithmComboBox
            // 
            algorithmComboBox.BackColor = Color.FromArgb(60, 60, 60);
            algorithmComboBox.Dock = DockStyle.Fill;
            algorithmComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            algorithmComboBox.FlatStyle = FlatStyle.Flat;
            algorithmComboBox.ForeColor = Color.White;
            algorithmComboBox.FormattingEnabled = true;
            algorithmComboBox.Location = new Point(511, 3);
            algorithmComboBox.Name = "algorithmComboBox";
            algorithmComboBox.Size = new Size(248, 23);
            algorithmComboBox.TabIndex = 2;
            // 
            // solveButton
            // 
            solveButton.BackColor = Color.FromArgb(0, 122, 204);
            solveButton.Dock = DockStyle.Fill;
            solveButton.FlatAppearance.BorderSize = 0;
            solveButton.FlatStyle = FlatStyle.Flat;
            solveButton.ForeColor = Color.White;
            solveButton.Location = new Point(765, 3);
            solveButton.Name = "solveButton";
            solveButton.Size = new Size(251, 34);
            solveButton.TabIndex = 1;
            solveButton.Text = "Solve";
            solveButton.UseVisualStyleBackColor = false;
            solveButton.Click += solveButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 0;
            label1.Text = "Input";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 0;
            label2.Text = "Output";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(269, 0);
            label3.Name = "label3";
            label3.Size = new Size(62, 15);
            label3.TabIndex = 3;
            label3.Text = "Algorithm";
            // 
            // inputTextBox
            // 
            inputTextBox.BackColor = Color.FromArgb(60, 60, 60);
            inputTextBox.BorderStyle = BorderStyle.FixedSingle;
            inputTextBox.Dock = DockStyle.Fill;
            inputTextBox.ForeColor = Color.White;
            inputTextBox.Location = new Point(0, 0);
            inputTextBox.Multiline = true;
            inputTextBox.Name = "inputTextBox";
            inputTextBox.ScrollBars = ScrollBars.Both;
            inputTextBox.Size = new Size(1019, 200);
            inputTextBox.TabIndex = 1;
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = Color.FromArgb(60, 60, 60);
            outputTextBox.BorderStyle = BorderStyle.FixedSingle;
            outputTextBox.Dock = DockStyle.Fill;
            outputTextBox.ForeColor = Color.White;
            outputTextBox.Location = new Point(0, 0);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ReadOnly = true;
            outputTextBox.ScrollBars = ScrollBars.Both;
            outputTextBox.Size = new Size(1019, 210);
            outputTextBox.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(28, 28, 28);
            panel1.Controls.Add(inputTextBox);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(1019, 200);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(28, 28, 28);
            panel2.Controls.Add(outputTextBox);
            panel2.Controls.Add(label2);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 240);
            panel2.Name = "panel2";
            panel2.Size = new Size(1019, 210);
            panel2.TabIndex = 3;
            // 
            // sensitivityGroupBox
            // 
            sensitivityGroupBox.Controls.Add(add_con_button);
            sensitivityGroupBox.Controls.Add(performAnalysisButton);
            sensitivityGroupBox.Controls.Add(analysisInputTextBox);
            sensitivityGroupBox.Controls.Add(shadowPricesRadioButton);
            sensitivityGroupBox.Controls.Add(rhsRangeRadioButton);
            sensitivityGroupBox.Controls.Add(basicVarRangeRadioButton);
            sensitivityGroupBox.Controls.Add(nonBasicVarRangeRadioButton);
            sensitivityGroupBox.Controls.Add(dualProblemRadioButton);
            sensitivityGroupBox.Dock = DockStyle.Bottom;
            sensitivityGroupBox.ForeColor = Color.White;
            sensitivityGroupBox.Location = new Point(0, 450);
            sensitivityGroupBox.Name = "sensitivityGroupBox";
            sensitivityGroupBox.Size = new Size(1019, 100);
            sensitivityGroupBox.TabIndex = 4;
            sensitivityGroupBox.TabStop = false;
            sensitivityGroupBox.Text = "Sensitivity Analysis";
            // 
            // add_con_button
            // 
            add_con_button.BackColor = Color.Transparent;
            add_con_button.ForeColor = Color.Black;
            add_con_button.Location = new Point(868, 51);
            add_con_button.Name = "add_con_button";
            add_con_button.Size = new Size(108, 23);
            add_con_button.TabIndex = 7;
            add_con_button.Text = "Add Constraint";
            add_con_button.UseVisualStyleBackColor = false;
            add_con_button.Click += add_con_button_Click;
            // 
            // performAnalysisButton
            // 
            performAnalysisButton.BackColor = Color.FromArgb(0, 122, 204);
            performAnalysisButton.FlatAppearance.BorderSize = 0;
            performAnalysisButton.FlatStyle = FlatStyle.Flat;
            performAnalysisButton.ForeColor = Color.White;
            performAnalysisButton.Location = new Point(650, 60);
            performAnalysisButton.Name = "performAnalysisButton";
            performAnalysisButton.Size = new Size(140, 30);
            performAnalysisButton.TabIndex = 5;
            performAnalysisButton.Text = "Perform Analysis";
            performAnalysisButton.UseVisualStyleBackColor = false;
            performAnalysisButton.Click += performAnalysisButton_Click;
            // 
            // analysisInputTextBox
            // 
            analysisInputTextBox.BackColor = Color.FromArgb(60, 60, 60);
            analysisInputTextBox.BorderStyle = BorderStyle.FixedSingle;
            analysisInputTextBox.ForeColor = Color.White;
            analysisInputTextBox.Location = new Point(500, 65);
            analysisInputTextBox.Name = "analysisInputTextBox";
            analysisInputTextBox.Size = new Size(140, 23);
            analysisInputTextBox.TabIndex = 4;
            // 
            // shadowPricesRadioButton
            // 
            shadowPricesRadioButton.AutoSize = true;
            shadowPricesRadioButton.Location = new Point(10, 20);
            shadowPricesRadioButton.Name = "shadowPricesRadioButton";
            shadowPricesRadioButton.Size = new Size(101, 19);
            shadowPricesRadioButton.TabIndex = 0;
            shadowPricesRadioButton.TabStop = true;
            shadowPricesRadioButton.Text = "Shadow Prices";
            shadowPricesRadioButton.UseVisualStyleBackColor = true;
            // 
            // rhsRangeRadioButton
            // 
            rhsRangeRadioButton.AutoSize = true;
            rhsRangeRadioButton.Location = new Point(10, 45);
            rhsRangeRadioButton.Name = "rhsRangeRadioButton";
            rhsRangeRadioButton.Size = new Size(110, 19);
            rhsRangeRadioButton.TabIndex = 1;
            rhsRangeRadioButton.TabStop = true;
            rhsRangeRadioButton.Text = "RHS Range (Idx)";
            rhsRangeRadioButton.UseVisualStyleBackColor = true;
            // 
            // basicVarRangeRadioButton
            // 
            basicVarRangeRadioButton.AutoSize = true;
            basicVarRangeRadioButton.Location = new Point(10, 70);
            basicVarRangeRadioButton.Name = "basicVarRangeRadioButton";
            basicVarRangeRadioButton.Size = new Size(134, 19);
            basicVarRangeRadioButton.TabIndex = 2;
            basicVarRangeRadioButton.TabStop = true;
            basicVarRangeRadioButton.Text = "Basic Var Range (Idx)";
            basicVarRangeRadioButton.UseVisualStyleBackColor = true;
            // 
            // nonBasicVarRangeRadioButton
            // 
            nonBasicVarRangeRadioButton.AutoSize = true;
            nonBasicVarRangeRadioButton.Location = new Point(160, 20);
            nonBasicVarRangeRadioButton.Name = "nonBasicVarRangeRadioButton";
            nonBasicVarRangeRadioButton.Size = new Size(162, 19);
            nonBasicVarRangeRadioButton.TabIndex = 3;
            nonBasicVarRangeRadioButton.TabStop = true;
            nonBasicVarRangeRadioButton.Text = "Non-Basic Var Range (Idx)";
            nonBasicVarRangeRadioButton.UseVisualStyleBackColor = true;
            // 
            // dualProblemRadioButton
            // 
            dualProblemRadioButton.AutoSize = true;
            dualProblemRadioButton.Location = new Point(160, 45);
            dualProblemRadioButton.Name = "dualProblemRadioButton";
            dualProblemRadioButton.Size = new Size(97, 19);
            dualProblemRadioButton.TabIndex = 6;
            dualProblemRadioButton.TabStop = true;
            dualProblemRadioButton.Text = "Dual Problem";
            dualProblemRadioButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(1019, 550);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(sensitivityGroupBox);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "Form1";
            Text = "LP Solver";
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            sensitivityGroupBox.ResumeLayout(false);
            sensitivityGroupBox.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button solveButton;
        private System.Windows.Forms.ComboBox algorithmComboBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox sensitivityGroupBox;
        private System.Windows.Forms.Button performAnalysisButton;
        private System.Windows.Forms.TextBox analysisInputTextBox;
        private System.Windows.Forms.RadioButton shadowPricesRadioButton;
        private System.Windows.Forms.RadioButton rhsRangeRadioButton;
        private System.Windows.Forms.RadioButton basicVarRangeRadioButton;
        private System.Windows.Forms.RadioButton nonBasicVarRangeRadioButton;
        private System.Windows.Forms.RadioButton dualProblemRadioButton;
        private System.Windows.Forms.Button exportButton;
        private Button export_button;
        private Button add_con_button;
    }
}