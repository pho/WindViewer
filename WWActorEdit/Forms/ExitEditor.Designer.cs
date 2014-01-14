namespace WWActorEdit.Forms
{
    partial class ExitEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sclsRoomIndex = new System.Windows.Forms.NumericUpDown();
            this.sclsSpawnIndex = new System.Windows.Forms.NumericUpDown();
            this.sclsExitTypeIndex = new System.Windows.Forms.NumericUpDown();
            this.sclsDestName = new System.Windows.Forms.TextBox();
            this.sclsDropdown = new System.Windows.Forms.ComboBox();
            this.EnvRDropdownDelete = new System.Windows.Forms.Button();
            this.EnvRDropdownAdd = new System.Windows.Forms.Button();
            this.sclsPaddingIndex = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.sclsRoomIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsSpawnIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsExitTypeIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsPaddingIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Destination Worldspace Folder Name:";
            this.toolTip.SetToolTip(this.label1, "Worldspace - A group of rooms in one folder, sharing one Stage.arc");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Destination Room Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Destination Spawn Index:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Exit Type:";
            // 
            // sclsRoomIndex
            // 
            this.sclsRoomIndex.Location = new System.Drawing.Point(208, 94);
            this.sclsRoomIndex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sclsRoomIndex.Name = "sclsRoomIndex";
            this.sclsRoomIndex.Size = new System.Drawing.Size(129, 20);
            this.sclsRoomIndex.TabIndex = 8;
            this.sclsRoomIndex.ValueChanged += new System.EventHandler(this.sclsIndex_ValueChanged);
            // 
            // sclsSpawnIndex
            // 
            this.sclsSpawnIndex.Location = new System.Drawing.Point(208, 120);
            this.sclsSpawnIndex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sclsSpawnIndex.Name = "sclsSpawnIndex";
            this.sclsSpawnIndex.Size = new System.Drawing.Size(129, 20);
            this.sclsSpawnIndex.TabIndex = 9;
            this.sclsSpawnIndex.ValueChanged += new System.EventHandler(this.sclsIndex_ValueChanged);
            // 
            // sclsExitTypeIndex
            // 
            this.sclsExitTypeIndex.Location = new System.Drawing.Point(208, 146);
            this.sclsExitTypeIndex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sclsExitTypeIndex.Name = "sclsExitTypeIndex";
            this.sclsExitTypeIndex.Size = new System.Drawing.Size(129, 20);
            this.sclsExitTypeIndex.TabIndex = 10;
            this.sclsExitTypeIndex.ValueChanged += new System.EventHandler(this.sclsIndex_ValueChanged);
            // 
            // sclsDestName
            // 
            this.sclsDestName.Location = new System.Drawing.Point(208, 68);
            this.sclsDestName.MaxLength = 8;
            this.sclsDestName.Name = "sclsDestName";
            this.sclsDestName.Size = new System.Drawing.Size(129, 20);
            this.sclsDestName.TabIndex = 11;
            this.sclsDestName.TextChanged += new System.EventHandler(this.sclsDestName_TextChanged);
            // 
            // sclsDropdown
            // 
            this.sclsDropdown.FormattingEnabled = true;
            this.sclsDropdown.Location = new System.Drawing.Point(13, 39);
            this.sclsDropdown.Name = "sclsDropdown";
            this.sclsDropdown.Size = new System.Drawing.Size(185, 21);
            this.sclsDropdown.TabIndex = 12;
            this.sclsDropdown.SelectedIndexChanged += new System.EventHandler(this.sclsDropdown_SelectedIndexChanged);
            // 
            // EnvRDropdownDelete
            // 
            this.EnvRDropdownDelete.Enabled = false;
            this.EnvRDropdownDelete.Location = new System.Drawing.Point(270, 39);
            this.EnvRDropdownDelete.Name = "EnvRDropdownDelete";
            this.EnvRDropdownDelete.Size = new System.Drawing.Size(67, 23);
            this.EnvRDropdownDelete.TabIndex = 19;
            this.EnvRDropdownDelete.Text = "Delete";
            this.EnvRDropdownDelete.UseVisualStyleBackColor = true;
            // 
            // EnvRDropdownAdd
            // 
            this.EnvRDropdownAdd.Enabled = false;
            this.EnvRDropdownAdd.Location = new System.Drawing.Point(204, 39);
            this.EnvRDropdownAdd.Name = "EnvRDropdownAdd";
            this.EnvRDropdownAdd.Size = new System.Drawing.Size(60, 23);
            this.EnvRDropdownAdd.TabIndex = 18;
            this.EnvRDropdownAdd.Text = "Add";
            this.EnvRDropdownAdd.UseVisualStyleBackColor = true;
            // 
            // sclsPaddingIndex
            // 
            this.sclsPaddingIndex.Location = new System.Drawing.Point(208, 172);
            this.sclsPaddingIndex.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sclsPaddingIndex.Name = "sclsPaddingIndex";
            this.sclsPaddingIndex.Size = new System.Drawing.Size(129, 20);
            this.sclsPaddingIndex.TabIndex = 21;
            this.sclsPaddingIndex.ValueChanged += new System.EventHandler(this.sclsIndex_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Padding:";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(184, 201);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 22;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(265, 201);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 23;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(325, 21);
            this.comboBox1.TabIndex = 24;
            // 
            // RoomExitEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 233);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.sclsPaddingIndex);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.EnvRDropdownDelete);
            this.Controls.Add(this.EnvRDropdownAdd);
            this.Controls.Add(this.sclsDropdown);
            this.Controls.Add(this.sclsDestName);
            this.Controls.Add(this.sclsExitTypeIndex);
            this.Controls.Add(this.sclsSpawnIndex);
            this.Controls.Add(this.sclsRoomIndex);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RoomExitEditor";
            this.Text = "Exit Editor";
            this.Load += new System.EventHandler(this.RoomExitEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sclsRoomIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsSpawnIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsExitTypeIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sclsPaddingIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown sclsRoomIndex;
        private System.Windows.Forms.NumericUpDown sclsSpawnIndex;
        private System.Windows.Forms.NumericUpDown sclsExitTypeIndex;
        private System.Windows.Forms.TextBox sclsDestName;
        private System.Windows.Forms.ComboBox sclsDropdown;
        private System.Windows.Forms.Button EnvRDropdownDelete;
        private System.Windows.Forms.Button EnvRDropdownAdd;
        private System.Windows.Forms.NumericUpDown sclsPaddingIndex;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}