namespace CppOptimizationTool
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.addFilesBtn = new System.Windows.Forms.Button();
            this.filesListbox = new System.Windows.Forms.ListBox();
            this.selectedFileContent = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startAnalysisBtn = new System.Windows.Forms.Button();
            this.removeFromListBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addFilesBtn
            // 
            this.addFilesBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.addFilesBtn.Location = new System.Drawing.Point(18, 15);
            this.addFilesBtn.Name = "addFilesBtn";
            this.addFilesBtn.Size = new System.Drawing.Size(127, 31);
            this.addFilesBtn.TabIndex = 0;
            this.addFilesBtn.Text = "Append files";
            this.addFilesBtn.UseVisualStyleBackColor = true;
            this.addFilesBtn.Click += new System.EventHandler(this.addFilesBtn_Click);
            // 
            // filesListbox
            // 
            this.filesListbox.AllowDrop = true;
            this.filesListbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.filesListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.filesListbox.FormattingEnabled = true;
            this.filesListbox.ItemHeight = 20;
            this.filesListbox.Location = new System.Drawing.Point(18, 68);
            this.filesListbox.Name = "filesListbox";
            this.filesListbox.Size = new System.Drawing.Size(266, 484);
            this.filesListbox.TabIndex = 1;
            this.filesListbox.Click += new System.EventHandler(this.filesListbox_Click);
            this.filesListbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.filesListbox_DragDrop);
            this.filesListbox.DragOver += new System.Windows.Forms.DragEventHandler(this.filesListbox_DragOver);
            // 
            // selectedFileContent
            // 
            this.selectedFileContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedFileContent.BackColor = System.Drawing.Color.Ivory;
            this.selectedFileContent.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.selectedFileContent.ForeColor = System.Drawing.Color.Black;
            this.selectedFileContent.Location = new System.Drawing.Point(290, 68);
            this.selectedFileContent.Name = "selectedFileContent";
            this.selectedFileContent.Size = new System.Drawing.Size(788, 486);
            this.selectedFileContent.TabIndex = 2;
            this.selectedFileContent.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Список файлов:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(290, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Содержимое файла";
            // 
            // startAnalysisBtn
            // 
            this.startAnalysisBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.startAnalysisBtn.Location = new System.Drawing.Point(330, 16);
            this.startAnalysisBtn.Name = "startAnalysisBtn";
            this.startAnalysisBtn.Size = new System.Drawing.Size(194, 31);
            this.startAnalysisBtn.TabIndex = 5;
            this.startAnalysisBtn.Text = "Run optimization tool";
            this.startAnalysisBtn.UseVisualStyleBackColor = true;
            this.startAnalysisBtn.Click += new System.EventHandler(this.startAnalysisBtn_Click);
            // 
            // removeFromListBtn
            // 
            this.removeFromListBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.removeFromListBtn.Location = new System.Drawing.Point(151, 15);
            this.removeFromListBtn.Name = "removeFromListBtn";
            this.removeFromListBtn.Size = new System.Drawing.Size(173, 31);
            this.removeFromListBtn.TabIndex = 6;
            this.removeFromListBtn.Text = "Удалить из списка";
            this.removeFromListBtn.UseVisualStyleBackColor = true;
            this.removeFromListBtn.Click += new System.EventHandler(this.removeFromListBtn_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.selectedFileContent);
            this.panel1.Controls.Add(this.removeFromListBtn);
            this.panel1.Controls.Add(this.filesListbox);
            this.panel1.Controls.Add(this.startAnalysisBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.addFilesBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(899, 473);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1111, 591);
            this.panel1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1111, 591);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(905, 510);
            this.Name = "Form1";
            this.Text = "Оптимизация производительности программ методом упреждающего резервирования блоко" +
    "в динамической памяти с использованием статических массивов.";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addFilesBtn;
        private System.Windows.Forms.ListBox filesListbox;
        private System.Windows.Forms.RichTextBox selectedFileContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startAnalysisBtn;
        private System.Windows.Forms.Button removeFromListBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

