namespace RHServerManager
{
    public partial class ServiceForm : Form
    {
        private readonly MainForm? mainForm;
        private readonly Dictionary<string, string>? serviceSettings;
        private readonly bool isEditMode;

        public ServiceForm(MainForm form)
        {
            InitializeComponent();
            mainForm = form;

            cmbServerMode.Items.Add("WAG");
            cmbServerMode.Items.Add("DEV");
            cmbServerMode.SelectedIndex = 0;
        }

        public ServiceForm(Dictionary<string, string> settings, bool editMode)
        {
            InitializeComponent();
            cmbServerMode.Items.Add("WAG");
            cmbServerMode.Items.Add("DEV");
            isEditMode = editMode;

            // Set the initial values of the form controls based on the provided settings
            serviceSettings = new Dictionary<string, string>(settings);
            tbServiceName.Text = serviceSettings.GetValueOrDefault("country", "");
            tbAuthUrl.Text = serviceSettings.GetValueOrDefault("auth_url", "");
            tbBillingUrl.Text = serviceSettings.GetValueOrDefault("billing_url", "");
            cbSkipAuth.Checked = serviceSettings.GetValueOrDefault("skip_auth", "0") == "1";
            cbSkipBilling.Checked = serviceSettings.GetValueOrDefault("skip_billing", "0") == "1";
            cbFreeZen.Checked = serviceSettings.GetValueOrDefault("free_cash", "0") == "1";
            cbSkipNick.Checked = serviceSettings.GetValueOrDefault("skip_abuse_nick", "0") == "1";
            cbSecondPwd.Checked = serviceSettings.GetValueOrDefault("second_pass", "0") == "1";
            cbBetazone.Checked = serviceSettings.GetValueOrDefault("betazone", "0") == "1";
            numChannelLimit.Value = decimal.Parse(serviceSettings.GetValueOrDefault("channel_limit_count", "1"));
            numUserLimit.Value = decimal.Parse(serviceSettings.GetValueOrDefault("world_user_limit_count", "10"));
            numBillingIdc.Value = decimal.Parse(serviceSettings.GetValueOrDefault("billing_idc", "10101"));
            cmbServerMode.SelectedItem = serviceSettings.GetValueOrDefault("server_mode", "WAG");
        }

        public Dictionary<string, string> GetServiceSettings()
        {
            Dictionary<string, string> serviceSettings = new()
            {
                ["country"] = tbServiceName.Text,
                ["auth_url"] = tbAuthUrl.Text,
                ["billing_url"] = tbBillingUrl.Text,
                ["skip_auth"] = cbSkipAuth.Checked ? "1" : "0",
                ["skip_billing"] = cbSkipBilling.Checked ? "1" : "0",
                ["free_cash"] = cbFreeZen.Checked ? "1" : "0",
                ["skip_abuse_nick"] = cbSkipNick.Checked ? "1" : "0",
                ["second_pass"] = cbSecondPwd.Checked ? "1" : "0",
                ["betazone"] = cbBetazone.Checked ? "1" : "0",
                ["server_mode"] = cmbServerMode.Text,
                ["channel_limit_count"] = numChannelLimit.Value.ToString(),
                ["world_user_limit_count"] = numUserLimit.Value.ToString(),
                ["billing_idc"] = numBillingIdc.Value.ToString()
            };

            return serviceSettings;
        }

        public string GetServiceName()
        {
            return tbServiceName.Text;
        }

        private void ButtonConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbServiceName.Text))
            {
                MessageBox.Show("Please enter a valid service name.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Country = tbServiceName.Text;
            AuthUrl = tbAuthUrl.Text;
            BillingUrl = tbBillingUrl.Text;
            SkipAuth = cbSkipAuth.Checked;
            SkipBilling = cbSkipBilling.Checked;
            FreeZen = cbFreeZen.Checked;
            SkipNick = cbSkipNick.Checked;
            SecondPwd = cbSecondPwd.Checked;
            Betazone = cbBetazone.Checked;
            ChannelLimit = numChannelLimit.Text;
            UserLimit = numUserLimit.Text;
            UserLimit = numUserLimit.Text;
            BillingIdc = numBillingIdc.Text;

            if (!isEditMode && mainForm?.ServiceExists(Country) == true)
            {
                MessageBox.Show($"Service ({Country}) already exists.", "Duplicate Service", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ServiceForm_Load(object sender, EventArgs e)
        {
            if (isEditMode)
            {
                Text = "Edit Service";
                buttonConfirm.Text = "Save";
            }
            else
            {
                Text = "Add Service";
                buttonConfirm.Text = "Add";
            }
        }

        public string? Country { get; set; }
        public string SelectedServerMode
        {
            get { return cmbServerMode.Text; }
            set { cmbServerMode.Text = value; }
        }
        public string? AuthUrl { get; set; }
        public string? BillingUrl { get; set; }
        public bool SkipAuth { get; set; }
        public bool SkipBilling { get; set; }
        public bool FreeZen { get; set; }
        public bool SkipNick { get; set; }
        public bool SecondPwd { get; set; }
        public bool Betazone { get; set; }
        public string? ChannelLimit { get; set; }
        public string? UserLimit { get; set; }
        public string? BillingIdc { get; set; }

    }

}
