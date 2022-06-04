namespace CppOptimizationTool
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ram = new System.Windows.Forms.NumericUpDown();
            this.ramLabel = new System.Windows.Forms.Label();
            this.do_optimize = new System.Windows.Forms.Button();
            this.ramTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.targetFuncOut = new System.Windows.Forms.Label();
            this.n_i = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.w_i_max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.w_i_min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arr_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.func_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.targetFuncOut);
            this.panel1.Controls.Add(this.ram);
            this.panel1.Controls.Add(this.ramLabel);
            this.panel1.Controls.Add(this.do_optimize);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(799, 426);
            this.panel1.TabIndex = 0;
            // 
            // ram
            // 
            this.ram.Location = new System.Drawing.Point(669, 134);
            this.ram.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.ram.Name = "ram";
            this.ram.Size = new System.Drawing.Size(120, 20);
            this.ram.TabIndex = 4;
            // 
            // ramLabel
            // 
            this.ramLabel.AutoSize = true;
            this.ramLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ramLabel.Location = new System.Drawing.Point(635, 111);
            this.ramLabel.Name = "ramLabel";
            this.ramLabel.Size = new System.Drawing.Size(154, 20);
            this.ramLabel.TabIndex = 3;
            this.ramLabel.Text = "Available RAM (V) = ";
            // 
            // do_optimize
            // 
            this.do_optimize.Location = new System.Drawing.Point(635, 18);
            this.do_optimize.Name = "do_optimize";
            this.do_optimize.Size = new System.Drawing.Size(146, 81);
            this.do_optimize.TabIndex = 1;
            this.do_optimize.Text = "Оптимизировать";
            this.do_optimize.UseVisualStyleBackColor = true;
            this.do_optimize.Click += new System.EventHandler(this.button1_Click);
            // 
            // targetFuncOut
            // 
            this.targetFuncOut.AutoSize = true;
            this.targetFuncOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.targetFuncOut.Location = new System.Drawing.Point(9, 398);
            this.targetFuncOut.Name = "targetFuncOut";
            this.targetFuncOut.Size = new System.Drawing.Size(51, 20);
            this.targetFuncOut.TabIndex = 5;
            this.targetFuncOut.Text = "label1";
            // 
            // n_i
            // 
            this.n_i.HeaderText = "Ni";
            this.n_i.Name = "n_i";
            // 
            // w_i_max
            // 
            this.w_i_max.HeaderText = "Wi max";
            this.w_i_max.Name = "w_i_max";
            // 
            // w_i_min
            // 
            this.w_i_min.HeaderText = "Wi min";
            this.w_i_min.Name = "w_i_min";
            // 
            // arr_name
            // 
            this.arr_name.HeaderText = "Имя массива";
            this.arr_name.Name = "arr_name";
            // 
            // func_name
            // 
            this.func_name.HeaderText = "Имя функции";
            this.func_name.Name = "func_name";
            this.func_name.ReadOnly = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.func_name,
            this.arr_name,
            this.w_i_min,
            this.w_i_max,
            this.n_i});
            this.dataGridView1.Location = new System.Drawing.Point(10, 14);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(619, 377);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 498);
            this.Controls.Add(this.panel1);
            this.Name = "Form2";
            this.Text = "Мастер оптимизации c++ кода";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button do_optimize;
        private System.Windows.Forms.Label ramLabel;
        private System.Windows.Forms.ToolTip ramTooltip;
        private System.Windows.Forms.NumericUpDown ram;
        private System.Windows.Forms.Label targetFuncOut;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn func_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn arr_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn w_i_min;
        private System.Windows.Forms.DataGridViewTextBoxColumn w_i_max;
        private System.Windows.Forms.DataGridViewTextBoxColumn n_i;
    }
}