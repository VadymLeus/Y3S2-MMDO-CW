namespace MMDO_CW_CODE
{
    partial class MMDO_CW_CODE
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelInput = new System.Windows.Forms.Panel();
            this.btnSolve = new System.Windows.Forms.Button();
            this.panelOutput = new System.Windows.Forms.Panel();
            this.groupBoxCounts = new System.Windows.Forms.GroupBox();
            this.lblConstraintsCount = new System.Windows.Forms.Label();
            this.lblVarCount = new System.Windows.Forms.Label();
            this.numConstraintsCount = new System.Windows.Forms.NumericUpDown();
            this.numVarCount = new System.Windows.Forms.NumericUpDown();
            this.groupBoxInputArea = new System.Windows.Forms.GroupBox();
            this.panelInputButtons = new System.Windows.Forms.Panel();
            this.btnLoadFromExcel = new System.Windows.Forms.Button();
            this.btn_example = new System.Windows.Forms.Button();
            this.btnClearClick = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.groupBoxHistory = new System.Windows.Forms.GroupBox();
            this.splitContainerHistory = new System.Windows.Forms.SplitContainer();
            this.listBoxHistory = new System.Windows.Forms.ListBox();
            this.richTextBoxHistoryDetails = new System.Windows.Forms.RichTextBox();
            this.panelHistoryButtons = new System.Windows.Forms.Panel();
            this.btnLoadHistory = new System.Windows.Forms.Button();
            this.btnDeleteHistory = new System.Windows.Forms.Button();
            this.groupBoxCounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConstraintsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVarCount)).BeginInit();
            this.groupBoxInputArea.SuspendLayout();
            this.panelInputButtons.SuspendLayout();
            this.groupBoxHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHistory)).BeginInit();
            this.splitContainerHistory.Panel1.SuspendLayout();
            this.splitContainerHistory.Panel2.SuspendLayout();
            this.splitContainerHistory.SuspendLayout();
            this.panelHistoryButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelInput
            // 
            this.panelInput.AutoScroll = true;
            this.panelInput.BackColor = System.Drawing.Color.White;
            this.panelInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInput.Location = new System.Drawing.Point(3, 21);
            this.panelInput.Margin = new System.Windows.Forms.Padding(2);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(390, 300);
            this.panelInput.TabIndex = 4;
            // 
            // btnSolve
            // 
            this.btnSolve.BackColor = System.Drawing.Color.Green;
            this.btnSolve.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSolve.ForeColor = System.Drawing.Color.White;
            this.btnSolve.Location = new System.Drawing.Point(245, 16);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(2);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(147, 43);
            this.btnSolve.TabIndex = 5;
            this.btnSolve.Text = "Розрахувати";
            this.btnSolve.UseVisualStyleBackColor = false;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // panelOutput
            // 
            this.panelOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelOutput.AutoScroll = true;
            this.panelOutput.BackColor = System.Drawing.Color.White;
            this.panelOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelOutput.Location = new System.Drawing.Point(409, 10);
            this.panelOutput.Margin = new System.Windows.Forms.Padding(2);
            this.panelOutput.Name = "panelOutput";
            this.panelOutput.Size = new System.Drawing.Size(547, 442);
            this.panelOutput.TabIndex = 5;
            // 
            // groupBoxCounts
            // 
            this.groupBoxCounts.BackColor = System.Drawing.Color.White;
            this.groupBoxCounts.Controls.Add(this.lblConstraintsCount);
            this.groupBoxCounts.Controls.Add(this.lblVarCount);
            this.groupBoxCounts.Controls.Add(this.numConstraintsCount);
            this.groupBoxCounts.Controls.Add(this.numVarCount);
            this.groupBoxCounts.Controls.Add(this.btnSolve);
            this.groupBoxCounts.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxCounts.Location = new System.Drawing.Point(9, 10);
            this.groupBoxCounts.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxCounts.Name = "groupBoxCounts";
            this.groupBoxCounts.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxCounts.Size = new System.Drawing.Size(396, 72);
            this.groupBoxCounts.TabIndex = 10;
            this.groupBoxCounts.TabStop = false;
            this.groupBoxCounts.Text = "Параметри задачи";
            // 
            // lblConstraintsCount
            // 
            this.lblConstraintsCount.AutoSize = true;
            this.lblConstraintsCount.BackColor = System.Drawing.Color.White;
            this.lblConstraintsCount.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.lblConstraintsCount.Location = new System.Drawing.Point(4, 46);
            this.lblConstraintsCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConstraintsCount.Name = "lblConstraintsCount";
            this.lblConstraintsCount.Size = new System.Drawing.Size(136, 17);
            this.lblConstraintsCount.TabIndex = 8;
            this.lblConstraintsCount.Text = "Кількість обмежень:";
            // 
            // lblVarCount
            // 
            this.lblVarCount.AutoSize = true;
            this.lblVarCount.BackColor = System.Drawing.Color.White;
            this.lblVarCount.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.lblVarCount.Location = new System.Drawing.Point(4, 22);
            this.lblVarCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVarCount.Name = "lblVarCount";
            this.lblVarCount.Size = new System.Drawing.Size(120, 17);
            this.lblVarCount.TabIndex = 7;
            this.lblVarCount.Text = "Кількість змінних:";
            // 
            // numConstraintsCount
            // 
            this.numConstraintsCount.BackColor = System.Drawing.Color.White;
            this.numConstraintsCount.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.numConstraintsCount.Location = new System.Drawing.Point(165, 39);
            this.numConstraintsCount.Margin = new System.Windows.Forms.Padding(2);
            this.numConstraintsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numConstraintsCount.Name = "numConstraintsCount";
            this.numConstraintsCount.Size = new System.Drawing.Size(56, 24);
            this.numConstraintsCount.TabIndex = 1;
            this.numConstraintsCount.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numVarCount
            // 
            this.numVarCount.BackColor = System.Drawing.Color.White;
            this.numVarCount.Font = new System.Drawing.Font("Times New Roman", 11F);
            this.numVarCount.Location = new System.Drawing.Point(165, 11);
            this.numVarCount.Margin = new System.Windows.Forms.Padding(2);
            this.numVarCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVarCount.Name = "numVarCount";
            this.numVarCount.Size = new System.Drawing.Size(56, 24);
            this.numVarCount.TabIndex = 0;
            this.numVarCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // groupBoxInputArea
            // 
            this.groupBoxInputArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxInputArea.BackColor = System.Drawing.Color.White;
            this.groupBoxInputArea.Controls.Add(this.panelInput);
            this.groupBoxInputArea.Controls.Add(this.panelInputButtons);
            this.groupBoxInputArea.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxInputArea.Location = new System.Drawing.Point(9, 86);
            this.groupBoxInputArea.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxInputArea.Name = "groupBoxInputArea";
            this.groupBoxInputArea.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxInputArea.Size = new System.Drawing.Size(396, 366);
            this.groupBoxInputArea.TabIndex = 11;
            this.groupBoxInputArea.TabStop = false;
            this.groupBoxInputArea.Text = "Вхідні дані";
            // 
            // panelInputButtons
            // 
            this.panelInputButtons.Controls.Add(this.btnLoadFromExcel);
            this.panelInputButtons.Controls.Add(this.btn_example);
            this.panelInputButtons.Controls.Add(this.btnClearClick);
            this.panelInputButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInputButtons.Location = new System.Drawing.Point(3, 321);
            this.panelInputButtons.Name = "panelInputButtons";
            this.panelInputButtons.Size = new System.Drawing.Size(390, 43);
            this.panelInputButtons.TabIndex = 5;
            // 
            // btnLoadFromExcel
            // 
            this.btnLoadFromExcel.BackColor = System.Drawing.Color.Lime;
            this.btnLoadFromExcel.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadFromExcel.Location = new System.Drawing.Point(6, 6);
            this.btnLoadFromExcel.Name = "btnLoadFromExcel";
            this.btnLoadFromExcel.Size = new System.Drawing.Size(120, 30);
            this.btnLoadFromExcel.TabIndex = 13;
            this.btnLoadFromExcel.Text = "Excel";
            this.btnLoadFromExcel.UseVisualStyleBackColor = false;
            // 
            // btn_example
            // 
            this.btn_example.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_example.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_example.Location = new System.Drawing.Point(135, 6);
            this.btn_example.Name = "btn_example";
            this.btn_example.Size = new System.Drawing.Size(120, 30);
            this.btn_example.TabIndex = 12;
            this.btn_example.Text = "Приклад";
            this.btn_example.UseVisualStyleBackColor = false;
            this.btn_example.Click += new System.EventHandler(this.btn_example_Click);
            // 
            // btnClearClick
            // 
            this.btnClearClick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnClearClick.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClearClick.Location = new System.Drawing.Point(264, 6);
            this.btnClearClick.Name = "btnClearClick";
            this.btnClearClick.Size = new System.Drawing.Size(120, 30);
            this.btnClearClick.TabIndex = 14;
            this.btnClearClick.Text = "Очистити";
            this.btnClearClick.UseVisualStyleBackColor = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // groupBoxHistory
            // 
            this.groupBoxHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxHistory.BackColor = System.Drawing.Color.White;
            this.groupBoxHistory.Controls.Add(this.splitContainerHistory);
            this.groupBoxHistory.Controls.Add(this.panelHistoryButtons);
            this.groupBoxHistory.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.groupBoxHistory.Location = new System.Drawing.Point(961, 10);
            this.groupBoxHistory.Name = "groupBoxHistory";
            this.groupBoxHistory.Size = new System.Drawing.Size(278, 442);
            this.groupBoxHistory.TabIndex = 15;
            this.groupBoxHistory.TabStop = false;
            this.groupBoxHistory.Text = "Історія розв\'язків";
            // 
            // splitContainerHistory
            // 
            this.splitContainerHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHistory.Location = new System.Drawing.Point(3, 22);
            this.splitContainerHistory.Name = "splitContainerHistory";
            this.splitContainerHistory.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHistory.Panel1
            // 
            this.splitContainerHistory.Panel1.Controls.Add(this.listBoxHistory);
            // 
            // splitContainerHistory.Panel2
            // 
            this.splitContainerHistory.Panel2.Controls.Add(this.richTextBoxHistoryDetails);
            this.splitContainerHistory.Size = new System.Drawing.Size(272, 377);
            this.splitContainerHistory.SplitterDistance = 180;
            this.splitContainerHistory.TabIndex = 2;
            // 
            // listBoxHistory
            // 
            this.listBoxHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxHistory.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBoxHistory.FormattingEnabled = true;
            this.listBoxHistory.ItemHeight = 15;
            this.listBoxHistory.Location = new System.Drawing.Point(0, 0);
            this.listBoxHistory.Name = "listBoxHistory";
            this.listBoxHistory.Size = new System.Drawing.Size(272, 180);
            this.listBoxHistory.TabIndex = 0;
            // 
            // richTextBoxHistoryDetails
            // 
            this.richTextBoxHistoryDetails.BackColor = System.Drawing.Color.White;
            this.richTextBoxHistoryDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxHistoryDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxHistoryDetails.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.richTextBoxHistoryDetails.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxHistoryDetails.Name = "richTextBoxHistoryDetails";
            this.richTextBoxHistoryDetails.ReadOnly = true;
            this.richTextBoxHistoryDetails.Size = new System.Drawing.Size(272, 193);
            this.richTextBoxHistoryDetails.TabIndex = 0;
            this.richTextBoxHistoryDetails.Text = "";
            this.richTextBoxHistoryDetails.WordWrap = false;
            // 
            // panelHistoryButtons
            // 
            this.panelHistoryButtons.Controls.Add(this.btnLoadHistory);
            this.panelHistoryButtons.Controls.Add(this.btnDeleteHistory);
            this.panelHistoryButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHistoryButtons.Location = new System.Drawing.Point(3, 399);
            this.panelHistoryButtons.Name = "panelHistoryButtons";
            this.panelHistoryButtons.Size = new System.Drawing.Size(272, 40);
            this.panelHistoryButtons.TabIndex = 1;
            // 
            // btnLoadHistory
            // 
            this.btnLoadHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnLoadHistory.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadHistory.Location = new System.Drawing.Point(3, 5);
            this.btnLoadHistory.Name = "btnLoadHistory";
            this.btnLoadHistory.Size = new System.Drawing.Size(145, 30);
            this.btnLoadHistory.TabIndex = 0;
            this.btnLoadHistory.Text = "Завантажити";
            this.btnLoadHistory.UseVisualStyleBackColor = false;
            // 
            // btnDeleteHistory
            // 
            this.btnDeleteHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnDeleteHistory.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDeleteHistory.Location = new System.Drawing.Point(154, 5);
            this.btnDeleteHistory.Name = "btnDeleteHistory";
            this.btnDeleteHistory.Size = new System.Drawing.Size(115, 30);
            this.btnDeleteHistory.TabIndex = 1;
            this.btnDeleteHistory.Text = "Видалити";
            this.btnDeleteHistory.UseVisualStyleBackColor = false;
            // 
            // MMDO_CW_CODE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1251, 464);
            this.Controls.Add(this.groupBoxHistory);
            this.Controls.Add(this.groupBoxInputArea);
            this.Controls.Add(this.groupBoxCounts);
            this.Controls.Add(this.panelOutput);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(1000, 500);
            this.Name = "MMDO_CW_CODE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MMDO-CW-CODE";
            this.groupBoxCounts.ResumeLayout(false);
            this.groupBoxCounts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConstraintsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVarCount)).EndInit();
            this.groupBoxInputArea.ResumeLayout(false);
            this.panelInputButtons.ResumeLayout(false);
            this.groupBoxHistory.ResumeLayout(false);
            this.splitContainerHistory.Panel1.ResumeLayout(false);
            this.splitContainerHistory.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerHistory)).EndInit();
            this.splitContainerHistory.ResumeLayout(false);
            this.panelHistoryButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.NumericUpDown numVarCount;
        private System.Windows.Forms.NumericUpDown numConstraintsCount;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Label lblVarCount;
        private System.Windows.Forms.Label lblConstraintsCount;
        private System.Windows.Forms.Panel panelOutput;
        private System.Windows.Forms.GroupBox groupBoxCounts;
        private System.Windows.Forms.GroupBox groupBoxInputArea;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btn_example;
        private System.Windows.Forms.Button btnLoadFromExcel;
        private System.Windows.Forms.Button btnClearClick;
        private System.Windows.Forms.GroupBox groupBoxHistory;
        private System.Windows.Forms.ListBox listBoxHistory;
        private System.Windows.Forms.Panel panelHistoryButtons;
        private System.Windows.Forms.Button btnLoadHistory;
        private System.Windows.Forms.Button btnDeleteHistory;
        private System.Windows.Forms.Panel panelInputButtons;
        private System.Windows.Forms.SplitContainer splitContainerHistory;
        private System.Windows.Forms.RichTextBox richTextBoxHistoryDetails;
    }
}