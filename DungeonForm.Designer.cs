namespace RHServerManager
{
    partial class DungeonForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DungeonForm));
            checkBoxUseDungeon = new CheckBox();
            buttonCancel = new Button();
            buttonConfirm = new Button();
            numericUpDownRangeAttack = new NumericUpDown();
            numericUpDownDungeonAttack = new NumericUpDown();
            numericUpDownDungeonCombo = new NumericUpDown();
            numericUpDownDungeonDiff = new NumericUpDown();
            labelRangeAttack = new Label();
            labelRangeHP = new Label();
            labelRangeCombo = new Label();
            labelRangeDelay = new Label();
            labelAttack = new Label();
            labelHP = new Label();
            labelCombo = new Label();
            labelDelay = new Label();
            labelDungeonDiff = new Label();
            labelDungeonId = new Label();
            rangeHPTextBox = new TextBox();
            rangeComboTextBox = new TextBox();
            rangeDelayTextBox = new TextBox();
            dungeonHPTextBox = new TextBox();
            dungeonDelayTextBox = new TextBox();
            dungeonIDTextBox = new TextBox();
            label1 = new Label();
            dungeonNameTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownRangeAttack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonAttack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonCombo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonDiff).BeginInit();
            SuspendLayout();
            // 
            // checkBoxUseDungeon
            // 
            checkBoxUseDungeon.AutoSize = true;
            checkBoxUseDungeon.Location = new Point(12, 38);
            checkBoxUseDungeon.Name = "checkBoxUseDungeon";
            checkBoxUseDungeon.Size = new Size(45, 19);
            checkBoxUseDungeon.TabIndex = 67;
            checkBoxUseDungeon.Text = "Use";
            checkBoxUseDungeon.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(292, 183);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 66;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonConfirm
            // 
            buttonConfirm.Location = new Point(176, 183);
            buttonConfirm.Name = "buttonConfirm";
            buttonConfirm.Size = new Size(75, 23);
            buttonConfirm.TabIndex = 65;
            buttonConfirm.Text = "Add ";
            buttonConfirm.UseVisualStyleBackColor = true;
            buttonConfirm.Click += buttonConfirm_Click;
            // 
            // numericUpDownRangeAttack
            // 
            numericUpDownRangeAttack.DecimalPlaces = 1;
            numericUpDownRangeAttack.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            numericUpDownRangeAttack.Location = new Point(60, 123);
            numericUpDownRangeAttack.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            numericUpDownRangeAttack.Name = "numericUpDownRangeAttack";
            numericUpDownRangeAttack.Size = new Size(100, 23);
            numericUpDownRangeAttack.TabIndex = 64;
            // 
            // numericUpDownDungeonAttack
            // 
            numericUpDownDungeonAttack.DecimalPlaces = 1;
            numericUpDownDungeonAttack.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            numericUpDownDungeonAttack.Location = new Point(60, 81);
            numericUpDownDungeonAttack.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            numericUpDownDungeonAttack.Name = "numericUpDownDungeonAttack";
            numericUpDownDungeonAttack.Size = new Size(100, 23);
            numericUpDownDungeonAttack.TabIndex = 63;
            // 
            // numericUpDownDungeonCombo
            // 
            numericUpDownDungeonCombo.DecimalPlaces = 1;
            numericUpDownDungeonCombo.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownDungeonCombo.Location = new Point(305, 80);
            numericUpDownDungeonCombo.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            numericUpDownDungeonCombo.Name = "numericUpDownDungeonCombo";
            numericUpDownDungeonCombo.Size = new Size(100, 23);
            numericUpDownDungeonCombo.TabIndex = 62;
            // 
            // numericUpDownDungeonDiff
            // 
            numericUpDownDungeonDiff.Location = new Point(305, 36);
            numericUpDownDungeonDiff.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDownDungeonDiff.Name = "numericUpDownDungeonDiff";
            numericUpDownDungeonDiff.Size = new Size(100, 23);
            numericUpDownDungeonDiff.TabIndex = 61;
            // 
            // labelRangeAttack
            // 
            labelRangeAttack.AutoSize = true;
            labelRangeAttack.Location = new Point(58, 107);
            labelRangeAttack.Name = "labelRangeAttack";
            labelRangeAttack.Size = new Size(77, 15);
            labelRangeAttack.TabIndex = 60;
            labelRangeAttack.Text = "Range Attack";
            // 
            // labelRangeHP
            // 
            labelRangeHP.AutoSize = true;
            labelRangeHP.Location = new Point(181, 108);
            labelRangeHP.Name = "labelRangeHP";
            labelRangeHP.Size = new Size(59, 15);
            labelRangeHP.TabIndex = 59;
            labelRangeHP.Text = "Range HP";
            // 
            // labelRangeCombo
            // 
            labelRangeCombo.AutoSize = true;
            labelRangeCombo.Location = new Point(305, 108);
            labelRangeCombo.Name = "labelRangeCombo";
            labelRangeCombo.Size = new Size(83, 15);
            labelRangeCombo.TabIndex = 58;
            labelRangeCombo.Text = "Range Combo";
            // 
            // labelRangeDelay
            // 
            labelRangeDelay.AutoSize = true;
            labelRangeDelay.Location = new Point(423, 111);
            labelRangeDelay.Name = "labelRangeDelay";
            labelRangeDelay.Size = new Size(72, 15);
            labelRangeDelay.TabIndex = 57;
            labelRangeDelay.Text = "Range Delay";
            // 
            // labelAttack
            // 
            labelAttack.AutoSize = true;
            labelAttack.Location = new Point(60, 62);
            labelAttack.Name = "labelAttack";
            labelAttack.Size = new Size(41, 15);
            labelAttack.TabIndex = 56;
            labelAttack.Text = "Attack";
            // 
            // labelHP
            // 
            labelHP.AutoSize = true;
            labelHP.Location = new Point(183, 62);
            labelHP.Name = "labelHP";
            labelHP.Size = new Size(23, 15);
            labelHP.TabIndex = 55;
            labelHP.Text = "HP";
            // 
            // labelCombo
            // 
            labelCombo.AutoSize = true;
            labelCombo.Location = new Point(305, 63);
            labelCombo.Name = "labelCombo";
            labelCombo.Size = new Size(47, 15);
            labelCombo.TabIndex = 54;
            labelCombo.Text = "Combo";
            // 
            // labelDelay
            // 
            labelDelay.AutoSize = true;
            labelDelay.Location = new Point(424, 65);
            labelDelay.Name = "labelDelay";
            labelDelay.Size = new Size(36, 15);
            labelDelay.TabIndex = 53;
            labelDelay.Text = "Delay";
            // 
            // labelDungeonDiff
            // 
            labelDungeonDiff.AutoSize = true;
            labelDungeonDiff.Location = new Point(305, 18);
            labelDungeonDiff.Name = "labelDungeonDiff";
            labelDungeonDiff.Size = new Size(107, 15);
            labelDungeonDiff.TabIndex = 52;
            labelDungeonDiff.Text = "Dungeon Difficulty";
            // 
            // labelDungeonId
            // 
            labelDungeonId.AutoSize = true;
            labelDungeonId.Location = new Point(181, 18);
            labelDungeonId.Name = "labelDungeonId";
            labelDungeonId.Size = new Size(70, 15);
            labelDungeonId.TabIndex = 51;
            labelDungeonId.Text = "Dungeon ID";
            // 
            // rangeHPTextBox
            // 
            rangeHPTextBox.Location = new Point(181, 126);
            rangeHPTextBox.Name = "rangeHPTextBox";
            rangeHPTextBox.Size = new Size(100, 23);
            rangeHPTextBox.TabIndex = 50;
            // 
            // rangeComboTextBox
            // 
            rangeComboTextBox.Location = new Point(305, 126);
            rangeComboTextBox.Name = "rangeComboTextBox";
            rangeComboTextBox.Size = new Size(100, 23);
            rangeComboTextBox.TabIndex = 49;
            // 
            // rangeDelayTextBox
            // 
            rangeDelayTextBox.Location = new Point(423, 126);
            rangeDelayTextBox.Name = "rangeDelayTextBox";
            rangeDelayTextBox.Size = new Size(100, 23);
            rangeDelayTextBox.TabIndex = 48;
            // 
            // dungeonHPTextBox
            // 
            dungeonHPTextBox.Location = new Point(181, 80);
            dungeonHPTextBox.Name = "dungeonHPTextBox";
            dungeonHPTextBox.Size = new Size(100, 23);
            dungeonHPTextBox.TabIndex = 47;
            // 
            // dungeonDelayTextBox
            // 
            dungeonDelayTextBox.Location = new Point(423, 81);
            dungeonDelayTextBox.Name = "dungeonDelayTextBox";
            dungeonDelayTextBox.Size = new Size(100, 23);
            dungeonDelayTextBox.TabIndex = 46;
            // 
            // dungeonIDTextBox
            // 
            dungeonIDTextBox.Location = new Point(181, 36);
            dungeonIDTextBox.Name = "dungeonIDTextBox";
            dungeonIDTextBox.Size = new Size(100, 23);
            dungeonIDTextBox.TabIndex = 45;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 18);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 69;
            label1.Text = "Dungeon Name";
            // 
            // dungeonNameTextBox
            // 
            dungeonNameTextBox.Location = new Point(60, 36);
            dungeonNameTextBox.Name = "dungeonNameTextBox";
            dungeonNameTextBox.Size = new Size(100, 23);
            dungeonNameTextBox.TabIndex = 68;
            // 
            // DungeonForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(532, 221);
            Controls.Add(label1);
            Controls.Add(dungeonNameTextBox);
            Controls.Add(checkBoxUseDungeon);
            Controls.Add(buttonCancel);
            Controls.Add(buttonConfirm);
            Controls.Add(numericUpDownRangeAttack);
            Controls.Add(numericUpDownDungeonAttack);
            Controls.Add(numericUpDownDungeonCombo);
            Controls.Add(numericUpDownDungeonDiff);
            Controls.Add(labelRangeAttack);
            Controls.Add(labelRangeHP);
            Controls.Add(labelRangeCombo);
            Controls.Add(labelRangeDelay);
            Controls.Add(labelAttack);
            Controls.Add(labelHP);
            Controls.Add(labelCombo);
            Controls.Add(labelDelay);
            Controls.Add(labelDungeonDiff);
            Controls.Add(labelDungeonId);
            Controls.Add(rangeHPTextBox);
            Controls.Add(rangeComboTextBox);
            Controls.Add(rangeDelayTextBox);
            Controls.Add(dungeonHPTextBox);
            Controls.Add(dungeonDelayTextBox);
            Controls.Add(dungeonIDTextBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "DungeonForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Dungeon";
            Load += DungeonForm_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDownRangeAttack).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonAttack).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonCombo).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDungeonDiff).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBoxUseDungeon;
        private Button buttonCancel;
        private Button buttonConfirm;
        private NumericUpDown numericUpDownRangeAttack;
        private NumericUpDown numericUpDownDungeonAttack;
        private NumericUpDown numericUpDownDungeonCombo;
        private NumericUpDown numericUpDownDungeonDiff;
        private Label labelRangeAttack;
        private Label labelRangeHP;
        private Label labelRangeCombo;
        private Label labelRangeDelay;
        private Label labelAttack;
        private Label labelHP;
        private Label labelCombo;
        private Label labelDelay;
        private Label labelDungeonDiff;
        private Label labelDungeonId;
        private TextBox rangeHPTextBox;
        private TextBox rangeComboTextBox;
        private TextBox rangeDelayTextBox;
        private TextBox dungeonHPTextBox;
        private TextBox dungeonDelayTextBox;
        private TextBox dungeonIDTextBox;
        private Label label1;
        private TextBox dungeonNameTextBox;
    }
}