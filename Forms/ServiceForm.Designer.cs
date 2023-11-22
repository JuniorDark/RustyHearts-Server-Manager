namespace RHServerManager
{
    partial class ServiceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceForm));
            buttonCancel = new Button();
            buttonConfirm = new Button();
            label9 = new Label();
            label8 = new Label();
            lbServerMode = new Label();
            cmbServerMode = new ComboBox();
            cbBetazone = new CheckBox();
            numUserLimit = new NumericUpDown();
            numChannelLimit = new NumericUpDown();
            cbSecondPwd = new CheckBox();
            cbSkipNick = new CheckBox();
            cbFreeZen = new CheckBox();
            tbBillingUrl = new TextBox();
            lbBillingAddress = new Label();
            tbAuthUrl = new TextBox();
            lbAuthAddress = new Label();
            cbSkipBilling = new CheckBox();
            cbSkipAuth = new CheckBox();
            tbServiceName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            numBillingIdc = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numUserLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numChannelLimit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBillingIdc).BeginInit();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(232, 273);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 66;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonConfirm
            // 
            buttonConfirm.Location = new Point(141, 273);
            buttonConfirm.Name = "buttonConfirm";
            buttonConfirm.Size = new Size(75, 23);
            buttonConfirm.TabIndex = 65;
            buttonConfirm.Text = "Add ";
            buttonConfirm.UseVisualStyleBackColor = true;
            buttonConfirm.Click += ButtonConfirm_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(4, 195);
            label9.Name = "label9";
            label9.Size = new Size(117, 15);
            label9.TabIndex = 92;
            label9.Text = "Channel Limit Count";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(4, 226);
            label8.Name = "label8";
            label8.Size = new Size(131, 15);
            label8.TabIndex = 91;
            label8.Text = "World User Limit Count";
            // 
            // lbServerMode
            // 
            lbServerMode.AutoSize = true;
            lbServerMode.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            lbServerMode.Location = new Point(2, 124);
            lbServerMode.Name = "lbServerMode";
            lbServerMode.Size = new Size(74, 13);
            lbServerMode.TabIndex = 88;
            lbServerMode.Text = "Server Mode:";
            // 
            // cmbServerMode
            // 
            cmbServerMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbServerMode.FormattingEnabled = true;
            cmbServerMode.Location = new Point(141, 120);
            cmbServerMode.Name = "cmbServerMode";
            cmbServerMode.Size = new Size(91, 23);
            cmbServerMode.TabIndex = 89;
            // 
            // cbBetazone
            // 
            cbBetazone.AutoSize = true;
            cbBetazone.Location = new Point(260, 232);
            cbBetazone.Name = "cbBetazone";
            cbBetazone.Size = new Size(74, 19);
            cbBetazone.TabIndex = 87;
            cbBetazone.Text = "Betazone";
            cbBetazone.UseVisualStyleBackColor = true;
            // 
            // numUserLimit
            // 
            numUserLimit.Location = new Point(143, 222);
            numUserLimit.Name = "numUserLimit";
            numUserLimit.Size = new Size(89, 23);
            numUserLimit.TabIndex = 86;
            numUserLimit.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // numChannelLimit
            // 
            numChannelLimit.Location = new Point(143, 190);
            numChannelLimit.Name = "numChannelLimit";
            numChannelLimit.Size = new Size(89, 23);
            numChannelLimit.TabIndex = 85;
            numChannelLimit.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // cbSecondPwd
            // 
            cbSecondPwd.AutoSize = true;
            cbSecondPwd.Location = new Point(260, 207);
            cbSecondPwd.Name = "cbSecondPwd";
            cbSecondPwd.Size = new Size(118, 19);
            cbSecondPwd.TabIndex = 84;
            cbSecondPwd.Text = "Second Password";
            cbSecondPwd.UseVisualStyleBackColor = true;
            // 
            // cbSkipNick
            // 
            cbSkipNick.AutoSize = true;
            cbSkipNick.Location = new Point(260, 182);
            cbSkipNick.Name = "cbSkipNick";
            cbSkipNick.Size = new Size(104, 19);
            cbSkipNick.TabIndex = 83;
            cbSkipNick.Tag = "";
            cbSkipNick.Text = "Skip Nick Filter";
            cbSkipNick.UseVisualStyleBackColor = true;
            // 
            // cbFreeZen
            // 
            cbFreeZen.AutoSize = true;
            cbFreeZen.Location = new Point(260, 157);
            cbFreeZen.Name = "cbFreeZen";
            cbFreeZen.Size = new Size(71, 19);
            cbFreeZen.TabIndex = 82;
            cbFreeZen.Text = "Free Zen";
            cbFreeZen.UseVisualStyleBackColor = true;
            // 
            // tbBillingUrl
            // 
            tbBillingUrl.Location = new Point(108, 81);
            tbBillingUrl.Name = "tbBillingUrl";
            tbBillingUrl.PlaceholderText = "http://localhost:8080/serverApi/billing";
            tbBillingUrl.Size = new Size(240, 23);
            tbBillingUrl.TabIndex = 80;
            // 
            // lbBillingAddress
            // 
            lbBillingAddress.AutoSize = true;
            lbBillingAddress.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            lbBillingAddress.Location = new Point(2, 85);
            lbBillingAddress.Name = "lbBillingAddress";
            lbBillingAddress.Size = new Size(98, 13);
            lbBillingAddress.TabIndex = 79;
            lbBillingAddress.Text = "Billing IP Address:";
            // 
            // tbAuthUrl
            // 
            tbAuthUrl.Location = new Point(108, 46);
            tbAuthUrl.Name = "tbAuthUrl";
            tbAuthUrl.PlaceholderText = "http://localhost:8070/serverApi/auth";
            tbAuthUrl.Size = new Size(240, 23);
            tbAuthUrl.TabIndex = 77;
            // 
            // lbAuthAddress
            // 
            lbAuthAddress.AutoSize = true;
            lbAuthAddress.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            lbAuthAddress.Location = new Point(4, 49);
            lbAuthAddress.Name = "lbAuthAddress";
            lbAuthAddress.Size = new Size(98, 13);
            lbAuthAddress.TabIndex = 76;
            lbAuthAddress.Text = "Auth API Address:";
            // 
            // cbSkipBilling
            // 
            cbSkipBilling.AutoSize = true;
            cbSkipBilling.Location = new Point(354, 84);
            cbSkipBilling.Name = "cbSkipBilling";
            cbSkipBilling.Size = new Size(84, 19);
            cbSkipBilling.TabIndex = 94;
            cbSkipBilling.Text = "Skip Billing";
            cbSkipBilling.UseVisualStyleBackColor = true;
            // 
            // cbSkipAuth
            // 
            cbSkipAuth.AutoSize = true;
            cbSkipAuth.Location = new Point(354, 46);
            cbSkipAuth.Name = "cbSkipAuth";
            cbSkipAuth.Size = new Size(77, 19);
            cbSkipAuth.TabIndex = 93;
            cbSkipAuth.Text = "Skip Auth";
            cbSkipAuth.UseVisualStyleBackColor = true;
            // 
            // tbServiceName
            // 
            tbServiceName.Location = new Point(108, 11);
            tbServiceName.Name = "tbServiceName";
            tbServiceName.PlaceholderText = "usa";
            tbServiceName.Size = new Size(108, 23);
            tbServiceName.TabIndex = 96;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(2, 15);
            label1.Name = "label1";
            label1.Size = new Size(74, 13);
            label1.TabIndex = 95;
            label1.Text = "Service Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 161);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 98;
            label2.Text = "Billing Idc";
            // 
            // numBillingIdc
            // 
            numBillingIdc.Location = new Point(143, 157);
            numBillingIdc.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numBillingIdc.Name = "numBillingIdc";
            numBillingIdc.Size = new Size(89, 23);
            numBillingIdc.TabIndex = 97;
            numBillingIdc.Value = new decimal(new int[] { 10101, 0, 0, 0 });
            // 
            // ServiceForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 305);
            Controls.Add(label2);
            Controls.Add(numBillingIdc);
            Controls.Add(tbServiceName);
            Controls.Add(label1);
            Controls.Add(cbSkipBilling);
            Controls.Add(cbSkipAuth);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(lbServerMode);
            Controls.Add(cmbServerMode);
            Controls.Add(cbBetazone);
            Controls.Add(numUserLimit);
            Controls.Add(numChannelLimit);
            Controls.Add(cbSecondPwd);
            Controls.Add(cbSkipNick);
            Controls.Add(cbFreeZen);
            Controls.Add(tbBillingUrl);
            Controls.Add(lbBillingAddress);
            Controls.Add(tbAuthUrl);
            Controls.Add(lbAuthAddress);
            Controls.Add(buttonCancel);
            Controls.Add(buttonConfirm);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ServiceForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Add Service";
            Load += ServiceForm_Load;
            ((System.ComponentModel.ISupportInitialize)numUserLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numChannelLimit).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBillingIdc).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonCancel;
        private Button buttonConfirm;
        private Label label9;
        private Label label8;
        private Label lbServerMode;
        private ComboBox cmbServerMode;
        private CheckBox cbBetazone;
        private NumericUpDown numUserLimit;
        private NumericUpDown numChannelLimit;
        private CheckBox cbSecondPwd;
        private CheckBox cbSkipNick;
        private CheckBox cbFreeZen;
        private TextBox tbBillingUrl;
        private Label lbBillingAddress;
        private TextBox tbAuthUrl;
        private Label lbAuthAddress;
        private CheckBox cbSkipBilling;
        private CheckBox cbSkipAuth;
        private TextBox tbServiceName;
        private Label label1;
        private Label label2;
        private NumericUpDown numBillingIdc;
    }
}