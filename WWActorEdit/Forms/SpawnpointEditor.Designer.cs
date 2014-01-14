namespace WWActorEdit.Forms
{
    partial class SpawnpointEditor
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.spawnPosY = new System.Windows.Forms.NumericUpDown();
            this.spawnPosZ = new System.Windows.Forms.NumericUpDown();
            this.spawnPosX = new System.Windows.Forms.NumericUpDown();
            this.spawnRotX = new System.Windows.Forms.NumericUpDown();
            this.spawnRotZ = new System.Windows.Forms.NumericUpDown();
            this.spawnRotY = new System.Windows.Forms.NumericUpDown();
            this.spawnRoomNum = new System.Windows.Forms.NumericUpDown();
            this.spawnType = new System.Windows.Forms.NumericUpDown();
            this.spawnUnknown1 = new System.Windows.Forms.NumericUpDown();
            this.spawnEventIndex = new System.Windows.Forms.NumericUpDown();
            this.spawnDropdown = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.spawnName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRoomNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnUnknown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnEventIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(268, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Event Index:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Unknown 1:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Spawn Type:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Room Number:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Position:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 224);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Rotation:";
            // 
            // spawnPosY
            // 
            this.spawnPosY.Location = new System.Drawing.Point(148, 196);
            this.spawnPosY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.spawnPosY.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.spawnPosY.Name = "spawnPosY";
            this.spawnPosY.Size = new System.Drawing.Size(63, 20);
            this.spawnPosY.TabIndex = 7;
            this.spawnPosY.Value = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.spawnPosY.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnPosZ
            // 
            this.spawnPosZ.Location = new System.Drawing.Point(217, 196);
            this.spawnPosZ.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.spawnPosZ.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.spawnPosZ.Name = "spawnPosZ";
            this.spawnPosZ.Size = new System.Drawing.Size(63, 20);
            this.spawnPosZ.TabIndex = 8;
            this.spawnPosZ.Value = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.spawnPosZ.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnPosX
            // 
            this.spawnPosX.Location = new System.Drawing.Point(79, 196);
            this.spawnPosX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.spawnPosX.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.spawnPosX.Name = "spawnPosX";
            this.spawnPosX.Size = new System.Drawing.Size(63, 20);
            this.spawnPosX.TabIndex = 9;
            this.spawnPosX.Value = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.spawnPosX.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnRotX
            // 
            this.spawnRotX.Location = new System.Drawing.Point(136, 222);
            this.spawnRotX.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnRotX.Name = "spawnRotX";
            this.spawnRotX.Size = new System.Drawing.Size(44, 20);
            this.spawnRotX.TabIndex = 12;
            this.spawnRotX.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnRotZ
            // 
            this.spawnRotZ.Location = new System.Drawing.Point(236, 222);
            this.spawnRotZ.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnRotZ.Name = "spawnRotZ";
            this.spawnRotZ.Size = new System.Drawing.Size(44, 20);
            this.spawnRotZ.TabIndex = 11;
            this.spawnRotZ.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnRotY
            // 
            this.spawnRotY.Location = new System.Drawing.Point(186, 222);
            this.spawnRotY.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnRotY.Name = "spawnRotY";
            this.spawnRotY.Size = new System.Drawing.Size(44, 20);
            this.spawnRotY.TabIndex = 10;
            this.spawnRotY.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnRoomNum
            // 
            this.spawnRoomNum.Location = new System.Drawing.Point(236, 170);
            this.spawnRoomNum.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnRoomNum.Name = "spawnRoomNum";
            this.spawnRoomNum.Size = new System.Drawing.Size(44, 20);
            this.spawnRoomNum.TabIndex = 13;
            this.spawnRoomNum.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnType
            // 
            this.spawnType.Location = new System.Drawing.Point(236, 144);
            this.spawnType.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnType.Name = "spawnType";
            this.spawnType.Size = new System.Drawing.Size(44, 20);
            this.spawnType.TabIndex = 14;
            this.spawnType.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnUnknown1
            // 
            this.spawnUnknown1.Location = new System.Drawing.Point(236, 118);
            this.spawnUnknown1.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnUnknown1.Name = "spawnUnknown1";
            this.spawnUnknown1.Size = new System.Drawing.Size(44, 20);
            this.spawnUnknown1.TabIndex = 15;
            this.spawnUnknown1.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnEventIndex
            // 
            this.spawnEventIndex.Location = new System.Drawing.Point(236, 92);
            this.spawnEventIndex.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.spawnEventIndex.Name = "spawnEventIndex";
            this.spawnEventIndex.Size = new System.Drawing.Size(44, 20);
            this.spawnEventIndex.TabIndex = 16;
            this.spawnEventIndex.ValueChanged += new System.EventHandler(this.spawnIndex_ValueChanged);
            // 
            // spawnDropdown
            // 
            this.spawnDropdown.FormattingEnabled = true;
            this.spawnDropdown.Location = new System.Drawing.Point(12, 39);
            this.spawnDropdown.Name = "spawnDropdown";
            this.spawnDropdown.Size = new System.Drawing.Size(130, 21);
            this.spawnDropdown.TabIndex = 17;
            this.spawnDropdown.SelectedIndexChanged += new System.EventHandler(this.spawnDropdown_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(148, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Add";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(203, 37);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(203, 248);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(77, 23);
            this.button3.TabIndex = 21;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(136, 248);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(61, 23);
            this.button4.TabIndex = 20;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // spawnName
            // 
            this.spawnName.Enabled = false;
            this.spawnName.Location = new System.Drawing.Point(173, 66);
            this.spawnName.Name = "spawnName";
            this.spawnName.Size = new System.Drawing.Size(107, 20);
            this.spawnName.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Name:";
            // 
            // SpawnpointEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 280);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.spawnName);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.spawnDropdown);
            this.Controls.Add(this.spawnEventIndex);
            this.Controls.Add(this.spawnUnknown1);
            this.Controls.Add(this.spawnType);
            this.Controls.Add(this.spawnRoomNum);
            this.Controls.Add(this.spawnRotX);
            this.Controls.Add(this.spawnRotZ);
            this.Controls.Add(this.spawnRotY);
            this.Controls.Add(this.spawnPosX);
            this.Controls.Add(this.spawnPosZ);
            this.Controls.Add(this.spawnPosY);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Name = "SpawnpointEditor";
            this.Text = "Spawnpoint Editor";
            this.Load += new System.EventHandler(this.SpawnpointEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnPosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnRoomNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnUnknown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spawnEventIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown spawnPosY;
        private System.Windows.Forms.NumericUpDown spawnPosZ;
        private System.Windows.Forms.NumericUpDown spawnPosX;
        private System.Windows.Forms.NumericUpDown spawnRotX;
        private System.Windows.Forms.NumericUpDown spawnRotZ;
        private System.Windows.Forms.NumericUpDown spawnRotY;
        private System.Windows.Forms.NumericUpDown spawnRoomNum;
        private System.Windows.Forms.NumericUpDown spawnType;
        private System.Windows.Forms.NumericUpDown spawnUnknown1;
        private System.Windows.Forms.NumericUpDown spawnEventIndex;
        private System.Windows.Forms.ComboBox spawnDropdown;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox spawnName;
        private System.Windows.Forms.Label label7;
    }
}