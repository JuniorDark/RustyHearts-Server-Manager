using RHServerManager.Properties;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;

namespace RHServerManager
{
    public partial class Form1 : Form
    {
        #region Form
        public Form1()
        {
            InitializeComponent();

            System.Windows.Forms.Timer t = new()
            {
                Interval = 5000
            };
            t.Tick += CheckProcessStatus;

            t.Start();

            pictureBoxEyeSqlPassword.MouseDown += new MouseEventHandler(pictureBoxEyeSqlPassword_MouseDown);
            pictureBoxEyeSqlPassword.MouseUp += new MouseEventHandler(pictureBoxEyeSqlPassword_MouseUp);
            pictureBoxEyeDbPassword.MouseDown += new MouseEventHandler(pictureBoxEyeDbPassword_MouseDown);
            pictureBoxEyeDbPassword.MouseUp += new MouseEventHandler(pictureBoxEyeDbPassword_MouseUp);
            pictureBoxEyeSmtpPassword.MouseDown += new MouseEventHandler(pictureBoxEyeSmtpPassword_MouseDown);
            pictureBoxEyeSmtpPassword.MouseUp += new MouseEventHandler(pictureBoxEyeSmtpPassword_MouseUp);

        }

        public string Url = "https://github.com/JuniorDark/RustyHearts-Server-Manager";

        private readonly string wanReplace = "YOUR_WAN_IP";
        private readonly string dbServerReplace = "SQL_DB_SERVER";
        private readonly string dbUserReplace = "SQL_DB_USER";
        private readonly string dbPasswdReplace = "SQL_DB_PASSWORD";
        private readonly string serviceCOUNTRYReplace = "SERVICE_COUNTRY";
        private readonly string authUrlReplace = "AUTH_URL";
        private readonly string billingUrlReplace = "BILLING_URL";
        private string wan_ip = string.Empty;
        private string dbUser = string.Empty;
        private string dbPasswd = string.Empty;
        private string dbServer = string.Empty;
        private string dirServer = string.Empty;
        private string dirAPI = string.Empty;
        private string apiIP = string.Empty;
        private string apiDbServer = string.Empty;
        private string apiDbUser = string.Empty;
        private string apiDbPasswd = string.Empty;
        private string serviceCountry = string.Empty;
        private string authUrl = string.Empty;
        private string billingUrl = string.Empty;

        private const string DefaultServerRegion = "usa";
        private const string DefaultAuthUrl = "http://localhost:8070/serverApi/auth";
        private const string DefaultBillingUrl = "http://localhost:8080/serverApi/billing";

        private void Form1_Load(object sender, EventArgs e)
        {
            VersionLabel.Text = $"Version: {GetVersion()}";

            LoadConfiguration();
            CheckProcessStatus();
            LoadValuesFromEnvFile();
            LoadContentXml();
        }

        private void LoadConfiguration()
        {
            IniFile ini = new("Config.ini");
            textBoxServerIP.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "ServerIP")) ? GetIPv4Address() : ini.ReadValue("Option", "ServerIP");
            textBoxServerDir.Text = ini.ReadValue("Option", "DirServer");
            textBoxAPIDir.Text = ini.ReadValue("Option", "DirAPI");
            textBoxSQLAddress.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "SqlServer")) ? Dns.GetHostName() : ini.ReadValue("Option", "SqlServer");
            textBoxSQLPassword.Text = ini.ReadValue("Option", "SqlPasswd");
            textBoxSQLAccount.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "SqlUser")) ? "sa" : ini.ReadValue("Option", "SqlUser");
            textBoxServerName.Text = ini.ReadValue("Option", "ServerName");
            comboBoxServerRegion.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "ServiceCountry")) ? DefaultServerRegion : ini.ReadValue("Option", "ServiceCountry");
            textBoxAuthIP.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "AuthUrl")) ? DefaultAuthUrl : ini.ReadValue("Option", "AuthUrl");
            textBoxBillingIP.Text = string.IsNullOrEmpty(ini.ReadValue("Option", "BillingUrl")) ? DefaultBillingUrl : ini.ReadValue("Option", "BillingUrl");
        }

        private static string GetIPv4Address()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    foreach (UnicastIPAddressInformation ipAddress in ipProperties.UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                            TcpConnectionInformation[] tcpConnections = ipGlobalProperties.GetActiveTcpConnections();

                            foreach (TcpConnectionInformation tcpConnection in tcpConnections)
                            {
                                if (tcpConnection.LocalEndPoint.Address.Equals(ipAddress.Address))
                                {
                                    return ipAddress.Address.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        private void CheckProcessStatus()
        {
            if (!IsProcessRunning("GameGatewayServer_Release_x64"))
            {
                btnLogin.Enabled = false;
            }

            if (!IsProcessRunning("Agent_Release_x64") || (!IsProcessRunning("AgentManager_Release_x64")))
            {
                btnStopServers.Enabled = false;
                btnStartServers.Enabled = true;
            }
        }

        private void LoadValuesFromEnvFile()
        {
            string dirAPI = textBoxAPIDir.Text;
            string envFilePath = Path.Combine(dirAPI, ".env");

            if (!string.IsNullOrEmpty(dirAPI))
            {
                LoadValuesFromEnvFile(envFilePath);
            }
        }

        private void LoadContentXml()
        {
            string dirServer = textBoxServerDir.Text;
            string optionDir = Path.Combine(dirServer, "Option");
            string contentFilePath = Path.Combine(optionDir, "content_control.xml");

            if (!string.IsNullOrEmpty(dirServer))
            {
                LoadContentXml(contentFilePath);

                tabControlSettings.TabPages["tabContent"].Enabled = true;
            }
            else
            {
                tabControlSettings.TabPages["tabContent"].Enabled = false;
            }

        }

        public static string GetVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

            string version = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";

            return version;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            IniFile ini = new("Config.ini");
            ini.WriteValue("Option", "DirServer", textBoxServerDir.Text);
            ini.WriteValue("Option", "DirAPI", textBoxAPIDir.Text);
            ini.WriteValue("Option", "SqlServer", textBoxSQLAddress.Text);
            ini.WriteValue("Option", "SqlUser", textBoxSQLAccount.Text);
            ini.WriteValue("Option", "SqlPasswd", textBoxSQLPassword.Text);
            ini.WriteValue("Option", "ServerIP", textBoxServerIP.Text);
            ini.WriteValue("Option", "ServerName", textBoxServerName.Text);
            ini.WriteValue("Option", "ServiceCountry", comboBoxServerRegion.Text);
            ini.WriteValue("Option", "AuthUrl", textBoxAuthIP.Text);
            ini.WriteValue("Option", "BillingUrl", textBoxBillingIP.Text);
        }

        private void LoadValuesFromEnvFile(string envFilePath)
        {
            if (!File.Exists(envFilePath))
            {
                return;
            }

            using StreamReader reader = new(envFilePath);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = line.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "PUBLIC_IP":
                            textBoxAPIIP.Text = value;
                            break;
                        case "PORT":
                            textBoxPort.Text = value;
                            break;
                        case "AUTH_PORT":
                            textBoxAuthPort.Text = value;
                            break;
                        case "BILLING_PORT":
                            textBoxBillingPort.Text = value;
                            break;
                        case "ENABLE_HELMET":
                            checkBoxEnableHelmet.Checked = bool.Parse(value);
                            break;
                        case "TZ":
                            textBoxTimeZone.Text = value;
                            break;
                        case "LOG_LEVEL":
                            comboBoxLogLevel.SelectedItem = value;
                            break;
                        case "LOG_AUTH_CONSOLE":
                            checkBoxLogAuthConsole.Checked = bool.Parse(value);
                            break;
                        case "LOG_BILLING_CONSOLE":
                            checkBoxLogBillingConsole.Checked = bool.Parse(value);
                            break;
                        case "LOG_ACCOUNT_CONSOLE":
                            checkBoxLogAccountConsole.Checked = bool.Parse(value);
                            break;
                        case "LOG_MAILER_CONSOLE":
                            checkBoxLogMailerConsole.Checked = bool.Parse(value);
                            break;
                        case "DB_SERVER":
                            textBoxDbServer.Text = value;
                            break;
                        case "DB_DATABASE":
                            textBoxDbName.Text = value;
                            break;
                        case "DB_USER":
                            textBoxDbUser.Text = value;
                            break;
                        case "DB_PASSWORD":
                            textBoxDbPassword.Text = value;
                            break;
                        case "DB_ENCRYPT":
                            checkBoxDbEncrypt.Checked = bool.Parse(value);
                            break;
                        case "GATESERVER_IP":
                            textBoxGateServerIp.Text = value;
                            break;
                        case "GATESERVER_PORT":
                            textBoxGateServerPort.Text = value;
                            break;
                        case "SMTP_HOST":
                            textBoxSmtpHost.Text = value;
                            break;
                        case "SMTP_PORT":
                            textBoxSmtpPort.Text = value;
                            break;
                        case "SMTP_ENCRYPTION":
                            comboBoxSmtpEncryption.SelectedItem = value;
                            break;
                        case "SMTP_USERNAME":
                            textBoxSmtpUsername.Text = value;
                            break;
                        case "SMTP_PASSWORD":
                            textBoxSmtpPassword.Text = value;
                            break;
                        case "SMTP_FROMNAME":
                            textBoxSmtpFromName.Text = value;
                            break;
                        default:
                            // Ignore unrecognized keys
                            break;
                    }
                }
            }
        }

        private void SaveValuesToEnvFile(string envFilePath)
        {
            using StreamWriter writer = new(envFilePath);

            writer.WriteLine($"# API CONFIGURATION");
            writer.WriteLine($"PUBLIC_IP={textBoxAPIIP.Text}");
            writer.WriteLine($"PORT={textBoxPort.Text}");
            writer.WriteLine($"AUTH_PORT={textBoxAuthPort.Text}");
            writer.WriteLine($"BILLING_PORT={textBoxBillingPort.Text}");
            writer.WriteLine($"ENABLE_HELMET={(checkBoxEnableHelmet.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine($"TZ={textBoxTimeZone.Text}");
            writer.WriteLine();

            writer.WriteLine($"# LOGGING CONFIGURATION");
            writer.WriteLine($"LOG_LEVEL={comboBoxLogLevel.SelectedItem}");
            writer.WriteLine($"LOG_AUTH_CONSOLE={(checkBoxLogAuthConsole.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine($"LOG_BILLING_CONSOLE={(checkBoxLogBillingConsole.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine($"LOG_ACCOUNT_CONSOLE={(checkBoxLogAccountConsole.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine($"LOG_MAILER_CONSOLE={(checkBoxLogMailerConsole.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine();

            writer.WriteLine($"# API DATABASE CONFIGURATION");
            writer.WriteLine($"DB_SERVER={textBoxDbServer.Text}");
            writer.WriteLine($"DB_DATABASE={textBoxDbName.Text}");
            writer.WriteLine($"DB_USER={textBoxDbUser.Text}");
            writer.WriteLine($"DB_PASSWORD={textBoxDbPassword.Text}");
            writer.WriteLine($"DB_ENCRYPT={(checkBoxDbEncrypt.Checked ? "true" : "false").ToLower()}");
            writer.WriteLine();

            writer.WriteLine($"# GATEWAY API CONFIGURATION");
            writer.WriteLine($"GATESERVER_IP={textBoxGateServerIp.Text}");
            writer.WriteLine($"GATESERVER_PORT={textBoxGateServerPort.Text}");
            writer.WriteLine();

            writer.WriteLine($"# EMAIL CONFIGURATION");
            writer.WriteLine($"SMTP_HOST={textBoxSmtpHost.Text}");
            writer.WriteLine($"SMTP_PORT={textBoxSmtpPort.Text}");
            writer.WriteLine($"SMTP_ENCRYPTION={comboBoxSmtpEncryption.SelectedItem}");
            writer.WriteLine($"SMTP_USERNAME={textBoxSmtpUsername.Text}");
            writer.WriteLine($"SMTP_PASSWORD={textBoxSmtpPassword.Text}");
            writer.WriteLine($"SMTP_FROMNAME={textBoxSmtpFromName.Text}");
        }

        private void buttonUpdateOption_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to update the options?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dirServer = textBoxServerDir.Text;
                    wan_ip = textBoxServerIP.Text;
                    dbServer = textBoxSQLAddress.Text;
                    dbPasswd = textBoxSQLPassword.Text;
                    dbUser = textBoxSQLAccount.Text;
                    serviceCountry = comboBoxServerRegion.Text;
                    authUrl = textBoxAuthIP.Text;
                    billingUrl = textBoxBillingIP.Text;

                    if (!Directory.Exists(dirServer))
                    {
                        MessageBox.Show("Please select the server folder location!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(wan_ip))
                    {
                        MessageBox.Show("The server IP address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbServer))
                    {
                        MessageBox.Show("SQL connection address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbUser))
                    {
                        MessageBox.Show("SQL connection account cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbPasswd))
                    {
                        MessageBox.Show("SQL connection password cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string option = Path.Combine(dirServer, "Option");
                    if (!Directory.Exists(option)) Directory.CreateDirectory(option);

                    string[] fileNames = { "agent.xml", "AGENT_800.xml", "agent_serveroption.xml", "agentmanager_serveroption.xml", "AGENTMANAGER_900.xml", "auction_serveroption.xml", "AUCTION_101.xml", "customoption.xml", "DBC_CHAT_3.xml", "DBC_EMERGENCY_4.xml", "DBC_GAME_1.xml", "DBC_LOG_2.xml", "dbc_serveroption.xml", "dbc_serveroptionlog.xml", "dungeon_serveroption.xml", "DUNGEON_2001.xml", "game_serveroption.xml", "GATE_81.xml", "gate_serveroption.xml", "gm_serveroption.xml", "GM_71.xml", "guild_serveroption.xml", "GUILD_5001.xml", "lobby_serveroption.xml", "LOBBY_21001.xml", "manager_serveroption.xml", "MANAGER_91.xml", "match_serveroption.xml", "MATCH_111.xml", "msg_serveroption.xml", "MSG_61.xml", "pvp_serveroption.xml", "PVP_3003.xml", "server_info.ini", "serveroption.xml", "service_control.xml" };
                    object[] resources = { Resources.agent, Resources.AGENT_800, Resources.agent_serveroption, Resources.agentmanager_serveroption, Resources.AGENTMANAGER_900, Resources.auction_serveroption, Resources.AUCTION_101, Resources.customoption, Resources.DBC_CHAT_3, Resources.DBC_EMERGENCY_4, Resources.DBC_GAME_1, Resources.DBC_LOG_2, Resources.dbc_serveroption, Resources.dbc_serveroptionlog, Resources.dungeon_serveroption, Resources.DUNGEON_2001, Resources.game_serveroption, Resources.GATE_81, Resources.gate_serveroption, Resources.gm_serveroption, Resources.GM_71, Resources.guild_serveroption, Resources.GUILD_5001, Resources.lobby_serveroption, Resources.LOBBY_21001, Resources.manager_serveroption, Resources.MANAGER_91, Resources.match_serveroption, Resources.MATCH_111, Resources.msg_serveroption, Resources.MSG_61, Resources.pvp_serveroption, Resources.PVP_3003, Resources.server_info, Resources.serveroption, Resources.service_control };

                    try
                    {
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            SaveOptionText(fileNames[i], ReplaceVALUES((string)resources[i]));
                        }

                        MessageBox.Show("Settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ReplaceVALUES(string content)
        {
            return content?.Replace(dbServerReplace, dbServer)
                           .Replace(dbPasswdReplace, dbPasswd)
                           .Replace(dbUserReplace, dbUser)
                           .Replace(serviceCOUNTRYReplace, serviceCountry)
                           .Replace(authUrlReplace, authUrl)
                           .Replace(billingUrlReplace, billingUrl)
                           .Replace(wanReplace, wan_ip) ?? "";
        }

        private void SaveOptionText(string name, string content)
        {
            File.WriteAllText(Path.Combine(dirServer, "Option\\" + name), content, Encoding.Default);
        }

        private void buttonUpdateAPI_Click(object sender, EventArgs e)
        {
            dirAPI = textBoxAPIDir.Text;
            apiIP = textBoxAPIIP.Text;
            apiDbServer = textBoxDbServer.Text;
            apiDbUser = textBoxDbUser.Text;
            apiDbPasswd = textBoxDbPassword.Text;

            if (!Directory.Exists(dirAPI))
            {
                MessageBox.Show("Please select the API folder location!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(apiIP))
            {
                MessageBox.Show("The API IP address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(apiDbServer))
            {
                MessageBox.Show("The API SQL connection address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(apiDbUser))
            {
                MessageBox.Show("The API SQL connection account cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(apiDbPasswd))
            {
                MessageBox.Show("The API SQL connection password cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string envFilePath = Path.Combine(dirAPI, ".env");

                if (!File.Exists(envFilePath))
                {
                    MessageBox.Show(".env file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(envFilePath))
                {
                    SaveValuesToEnvFile(envFilePath);
                }

                MessageBox.Show("Api settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUpdateDBIP_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Save changes to the database?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string wan_ip = textBoxServerIP.Text;
                    string dbServer = textBoxSQLAddress.Text;
                    string dbUser = textBoxSQLAccount.Text;
                    string dbPasswd = textBoxSQLPassword.Text;
                    string serverName = textBoxServerName.Text;

                    if (string.IsNullOrEmpty(wan_ip))
                    {
                        MessageBox.Show("Server IP address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbServer))
                    {
                        MessageBox.Show("SQL connection address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbUser))
                    {
                        MessageBox.Show("SQL connection account cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbPasswd))
                    {
                        MessageBox.Show("SQL connection password cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(serverName))
                    {
                        MessageBox.Show("Server name cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string connectionString = "Data Source=" + dbServer + ";Initial Catalog=RustyHearts_Auth;User Id=" + dbUser + ";Password=" + dbPasswd + ";";

                    try
                    {
                        using SqlConnection connection = new(connectionString);
                        connection.Open();

                        string cmd = "UPDATE [ServerOption] SET [PublicAddress] = @wan_ip WHERE [PublicAddress] != ''";

                        using (SqlCommand command = new(cmd, connection))
                        {
                            command.Parameters.AddWithValue("@wan_ip", wan_ip);
                            command.ExecuteNonQuery();
                        }
                        string updateCmd = "UPDATE WorldServer SET Name = @serverName";

                        using (SqlCommand updateCommand = new(updateCmd, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@serverName", serverName);
                            updateCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("Database settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonBrowseAPI_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBoxAPIDir.Text = fbd.SelectedPath;
            }

            dirAPI = textBoxAPIDir.Text;
            string envFilePath = Path.Combine(dirAPI, ".env");

            if (!File.Exists(envFilePath))
            {
                MessageBox.Show("Invalid API folder.\nPlease select the API folder with the .env file.", "env File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(dirAPI))
            {
                LoadValuesFromEnvFile(envFilePath);
            }

        }

        private void buttonBrowseServer_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBoxServerDir.Text = fbd.SelectedPath;
            }

            dirServer = textBoxServerDir.Text;
            string serverFilePath = Path.Combine(dirServer, "Agent_Release_x64.exe");

            if (!File.Exists(serverFilePath))
            {
                MessageBox.Show("Invalid Server folder.\nPlease select the server folder with the server executables.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadContentXml();
        }

        private void buttonOpenServerDir_Click(object sender, EventArgs e)
        {
            string serverDir = textBoxServerDir.Text;
            OpenFolder(serverDir);
        }

        private void buttonOpenApiDir_Click(object sender, EventArgs e)
        {
            string apiDir = textBoxAPIDir.Text;
            OpenFolder(apiDir);
        }

        static public void OpenFolder(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                Process.Start("explorer.exe", directory);
            }
        }

        private const string AgentProcessName = "Agent_Release_x64";
        private const string AgentManagerProcessName = "AgentManager_Release_x64";
        private const int ProcessStartDelay = 1000;
        private const int SetForegroundWindowDelay = 500;

        private async void btnStartServers_Click(object sender, EventArgs e)
        {
            string serverDirectory = textBoxServerDir.Text;
            if (!Directory.Exists(serverDirectory))
            {
                MessageBox.Show("Please select the server folder location", "Server folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tabControlSettings.TabPages["tabServer"].Enabled = false;
            tabControlSettings.TabPages["tabContent"].Enabled = false;
            btnStartServers.Enabled = false;
            btnStartServers.Text = "Starting...";

            string[] serverProcesses = { AgentProcessName, AgentManagerProcessName };

            try
            {
                foreach (string processName in serverProcesses)
                {
                    ProcessStartInfo startInfo = new(Path.Combine(serverDirectory, processName))
                    {
                        WorkingDirectory = serverDirectory
                    };

                    using Process serverProcess = Process.Start(startInfo);
                    if (processName == AgentProcessName)
                    {
                        await Task.Delay(ProcessStartDelay);
                        SetForegroundWindow(serverProcess.MainWindowHandle);
                        await Task.Delay(SetForegroundWindowDelay);
                        SendKeys.SendWait("1");
                    }
                }

                btnStartServers.Enabled = false;
                btnStartServers.Text = "Running...";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStopServers_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to stop the servers?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnStopServers.Enabled = false;
                    btnStopServers.Text = "Closing...";

                    StopServers();

                    tabControlSettings.TabPages["tabServer"].Enabled = true;
                    tabControlSettings.TabPages["tabContent"].Enabled = true;
                    btnStartServers.Enabled = true;
                    btnStartServers.Text = "Start Servers";
                    btnStopServers.Enabled = false;
                    btnStopServers.Text = "Stop Servers";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopServers()
        {
            string[] processNames = { AgentProcessName, AgentManagerProcessName };
            List<Process> processes = new();
            Process selectedProcess = null;

            foreach (string processName in processNames)
            {
                Process[] processArray = Process.GetProcessesByName(processName);
                if (processArray.Length > 0)
                {
                    processes.Add(processArray[0]);
                    if (processName == "Agent_Release_x64")
                        selectedProcess = processArray[0];
                }
            }

            if (selectedProcess != null)
            {
                SetForegroundWindow(selectedProcess.MainWindowHandle);
                SendKeys.SendWait("9");
            }

            foreach (Process process in processes)
            {
                if (!process.HasExited)
                {
                    Thread.Sleep(3000);
                    process.CloseMainWindow();
                    process.WaitForExit();
                    process.Dispose();
                }
            }
        }

        private Process? cmdProcess;
        private System.Windows.Forms.Timer? processCheckTimer;

        private void btnStartAPI_Click(object sender, EventArgs e)
        {
            string apiDirectory = textBoxAPIDir.Text;
            if (!Directory.Exists(apiDirectory))
            {
                MessageBox.Show("Please select the Api folder location", "API folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tabControlSettings.TabPages["tabApi"].Enabled = false;
            btnStartAPI.Enabled = false;
            ButtonClearAPILogs.Enabled = false;
            btnStartAPI.Text = "Starting...";

            try
            {
                string batchFilePath = Path.Combine(apiDirectory, "rh-api.bat");
                if (!File.Exists(batchFilePath))
                {
                    File.WriteAllText(batchFilePath, "@echo off" + Environment.NewLine +
                                                     "title Rusty Hearts API" + Environment.NewLine +
                                                     "node src/app" + Environment.NewLine +
                                                     "pause");
                }

                ProcessStartInfo startInfo = new("cmd.exe")
                {
                    WorkingDirectory = apiDirectory,
                    Arguments = "/c \"" + batchFilePath + "\"",
                    UseShellExecute = false
                };

                cmdProcess = new Process
                {
                    StartInfo = startInfo
                };
                cmdProcess.Start();

                btnStartAPI.Text = "Running...";
                btnStopAPI.Enabled = true;

                processCheckTimer = new System.Windows.Forms.Timer
                {
                    Interval = 2000
                };
                processCheckTimer.Tick += ProcessCheckTimer_Tick;
                processCheckTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStopAPI_Click(object sender, EventArgs e)
        {
            if (cmdProcess != null && !cmdProcess.HasExited)
            {
                cmdProcess.CloseMainWindow();
                cmdProcess.WaitForExit();
            }
            processCheckTimer?.Stop();
            cmdProcess?.Dispose();
            cmdProcess = null;
            tabControlSettings.TabPages["tabApi"].Enabled = true;
            btnStartAPI.Enabled = true;
            btnStopAPI.Enabled = false;
            ButtonClearAPILogs.Enabled = true;
            btnStartAPI.Text = "Start API";
        }

        private void ProcessCheckTimer_Tick(object sender, EventArgs e)
        {
            if (cmdProcess == null || cmdProcess.HasExited)
            {
                processCheckTimer?.Stop();
                cmdProcess?.Dispose();
                cmdProcess = null;
                tabControlSettings.TabPages["tabApi"].Enabled = true;
                btnStartAPI.Enabled = true;
                btnStopAPI.Enabled = false;
                ButtonClearAPILogs.Enabled = true;
                btnStartAPI.Text = "Start API";
            }
        }

        private void buttonClearLogs_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the server logs?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                dirServer = textBoxServerDir.Text;
                if (!Directory.Exists(dirServer))
                {
                    MessageBox.Show("Please select the server folder location", "Server folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string[] fLogs = Directory.GetFiles(dirServer, "*.log", SearchOption.TopDirectoryOnly);
                foreach (string log in fLogs)
                {
                    try
                    {
                        File.Delete(log);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (Directory.Exists(Path.Combine(dirServer, "Server_Log")))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(dirServer, "Server_Log"), true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (Directory.Exists(Path.Combine(dirServer, "Log")))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(dirServer, "Log"), true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                string[] dDumps = Directory.GetDirectories(dirServer, "Dump_*", SearchOption.TopDirectoryOnly);
                foreach (string log in dDumps)
                {
                    try
                    {
                        Directory.Delete(log, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                MessageBox.Show("Server Logs clean!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void buttonClearAPILogs_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the API logs?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                dirAPI = textBoxAPIDir.Text;
                if (!Directory.Exists(dirAPI))
                {
                    MessageBox.Show("Please select the API folder location", "API folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string[] fLogs = Directory.GetFiles(dirAPI, "*.log", SearchOption.TopDirectoryOnly);
                foreach (string log in fLogs)
                {
                    try
                    {
                        File.Delete(log);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (Directory.Exists(Path.Combine(dirAPI, "logs")))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(dirAPI, "logs"), true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                MessageBox.Show("API Logs clean!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private async void buttonRestoreDB_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> databaseBackupMap = new()
            {
                { "GMRustyHearts", "GMRustyHearts.bak" },
                { "RustyHearts", "RustyHearts.bak" },
                { "RustyHearts_Auth", "RustyHearts_Auth.bak" },
                { "RustyHearts_Log", "RustyHearts_Log.bak" },
                { "RustyHearts_Account", "RustyHearts_Account.bak" }
            };

            string dbServer = textBoxSQLAddress.Text;
            string dbUser = textBoxSQLAccount.Text;
            string dbPasswd = textBoxSQLPassword.Text;

            if (string.IsNullOrEmpty(dbServer))
            {
                MessageBox.Show("SQL connection address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(dbUser))
            {
                MessageBox.Show("SQL connection account cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(dbPasswd))
            {
                MessageBox.Show("SQL connection password cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to restore the databases? This will replace the databases and may take a few minutes.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to restore the databases?\nThis action is permanent and cannot be undone.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    try
                    {
                        buttonUpdateDBIP.Enabled = false;
                        buttonBackupDB.Enabled = false;
                        buttonRestoreDB.Enabled = false;
                        buttonRestoreDB.Text = "Restoring...";

                        using SqlConnection connection = new($"Data Source={dbServer};Initial Catalog=master;User Id={dbUser};Password={dbPasswd};");
                        await connection.OpenAsync();

                        foreach (KeyValuePair<string, string> databaseBackupPair in databaseBackupMap)
                        {
                            string dbName = databaseBackupPair.Key;
                            string dbBackupFile = databaseBackupPair.Value;
                            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), dbBackupFile);

                            // Check if the database already exists
                            string checkDbExistsSql = "SELECT COUNT(*) FROM master.sys.databases WHERE name = @name";
                            using (SqlCommand checkDbExistsCommand = new(checkDbExistsSql, connection))
                            {
                                checkDbExistsCommand.Parameters.AddWithValue("@name", dbName);
                                int dbExists = (int)await checkDbExistsCommand.ExecuteScalarAsync();

                                if (dbExists == 0)
                                {
                                    // Database doesn't exist, create it
                                    string createDbSql = $"CREATE DATABASE {dbName}";
                                    using SqlCommand createDbCommand = new(createDbSql, connection);
                                    await createDbCommand.ExecuteNonQueryAsync();
                                }
                            }

                            // Get the default data and log file paths for the database
                            string getDataFilePathSql = $"SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('{dbName}') AND type_desc = 'ROWS'";
                            string getLogFilePathSql = $"SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('{dbName}') AND type_desc = 'LOG'";
                            string dataFilePath, logFilePath;

                            using (SqlCommand getDataFilePathCommand = new(getDataFilePathSql, connection))
                            using (SqlDataReader dataFilePathReader = await getDataFilePathCommand.ExecuteReaderAsync())
                            {
                                if (dataFilePathReader.Read())
                                {
                                    dataFilePath = dataFilePathReader.GetString(0);
                                }
                                else
                                {
                                    throw new Exception("Failed to retrieve data file path.");
                                }
                            }

                            using (SqlCommand getLogFilePathCommand = new(getLogFilePathSql, connection))
                            using (SqlDataReader logFilePathReader = await getLogFilePathCommand.ExecuteReaderAsync())
                            {
                                if (logFilePathReader.Read())
                                {
                                    logFilePath = logFilePathReader.GetString(0);
                                }
                                else
                                {
                                    throw new Exception("Failed to retrieve log file path.");
                                }
                            }

                            string sqlRestore = $"RESTORE DATABASE {dbName} FROM DISK='{dbPath}' WITH MOVE '{dbName}' TO '{dataFilePath}', MOVE '{dbName}_Log' TO '{logFilePath}', REPLACE";
                            using SqlCommand cmdRestore = new(sqlRestore, connection);
                            cmdRestore.CommandTimeout = 60 * 15;
                            await cmdRestore.ExecuteNonQueryAsync();
                        }

                        MessageBox.Show("Database restored successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error restoring backups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        buttonRestoreDB.Enabled = true;
                        buttonRestoreDB.Text = "Restore Database";
                        buttonUpdateDBIP.Enabled = true;
                        buttonBackupDB.Enabled = true;
                    }
                }
            }
        }

        private async void buttonBackupDB_Click(object sender, EventArgs e)
        {
            string[] dbNames = { "GMRustyHearts", "RustyHearts", "RustyHearts_Auth", "RustyHearts_Log", "RustyHearts_Account", };
            FolderBrowserDialog folderBrowserDialog = new();

            if (MessageBox.Show("Create a backups of the databases: GMRustyHearts, RustyHearts, RustyHearts_Auth, RustyHearts_Log, RustyHearts_Account ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string backupFolderPath = folderBrowserDialog.SelectedPath;

                    string wan_ip = textBoxServerIP.Text;
                    string dbServer = textBoxSQLAddress.Text;
                    string dbUser = textBoxSQLAccount.Text;
                    string dbPasswd = textBoxSQLPassword.Text;

                    if (string.IsNullOrEmpty(wan_ip))
                    {
                        MessageBox.Show("The server IP address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbServer))
                    {
                        MessageBox.Show("SQL connection address cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbUser))
                    {
                        MessageBox.Show("SQL connection account cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.IsNullOrEmpty(dbPasswd))
                    {
                        MessageBox.Show("SQL connection password cannot be empty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    try
                    {
                        buttonUpdateDBIP.Enabled = false;
                        buttonRestoreDB.Enabled = false;
                        buttonBackupDB.Enabled = false;
                        buttonBackupDB.Text = "Backing up...";
                        await Task.Delay(1);

                        string strConnection = string.Format("Data Source={0};User Id={1};Password={2};", dbServer, dbUser, dbPasswd);

                        using SqlConnection connection = new(strConnection);
                        await connection.OpenAsync();

                        foreach (string dbName in dbNames)
                        {
                            string backupFilePath = Path.Combine(backupFolderPath, dbName + ".bak");
                            string backupSQL = string.Format("BACKUP DATABASE {0} TO DISK='{1}'", dbName, backupFilePath);

                            using SqlCommand backupCommand = new(backupSQL, connection);
                            await backupCommand.ExecuteNonQueryAsync();
                        }

                        MessageBox.Show("Databases backed up successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error creating databases backups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        buttonUpdateDBIP.Enabled = true;
                        buttonRestoreDB.Enabled = true;
                        buttonBackupDB.Enabled = true;
                        buttonBackupDB.Text = "Backup Database";
                    }
                }
            }
        }

        private const int SendKeysDelay = 500;
        private const string GameGatewayServerProcessName = "GameGatewayServer_Release_x64";

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (IsProcessRunning(GameGatewayServerProcessName) && btnLogin.Text == "Open Login")
            {
                btnLogin.Text = "Close Login";

                if (!IsProcessRunning(GameGatewayServerProcessName))
                {
                    btnLogin.Enabled = false;
                    btnLogin.Text = "Open Login";
                    MessageBox.Show(GameGatewayServerProcessName + " is not running!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Process process = Process.GetProcessesByName(GameGatewayServerProcessName)[0];
                SetForegroundWindow(process.MainWindowHandle);
                SendKeysToGameGatewayServer("uol");
            }
            else if (IsProcessRunning(GameGatewayServerProcessName) && btnLogin.Text == "Close Login")
            {
                btnLogin.Text = "Open Login";
                Process process = Process.GetProcessesByName(GameGatewayServerProcessName)[0];
                SetForegroundWindow(process.MainWindowHandle);
                SendKeysToGameGatewayServer("uol");
            }
            else
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Open Login";
            }
        }

        private void SendKeysToGameGatewayServer(string keysToSend)
        {
            foreach (char key in keysToSend)
            {
                SendKeys.SendWait(key.ToString());
                Thread.Sleep(SendKeysDelay);
            }
        }

        private void CheckProcessStatus(object? sender, EventArgs e)
        {

            if (IsProcessRunning("Agent_Release_x64"))
            {
                tabControlSettings.TabPages["tabServer"].Enabled = false;
                btnStartServers.Enabled = false;
                btnStartServers.Text = "Running...";
                btnStopServers.Enabled = true;
                btnStopServers.Text = "Stop Servers";
            }
            else
            {
                tabControlSettings.TabPages["tabServer"].Enabled = true;
                btnStartServers.Enabled = true;
                btnStartServers.Text = "Start Servers";
                btnStopServers.Enabled = false;
                btnStopServers.Text = "Stop Servers";
            }

            if (IsProcessRunning("GameGatewayServer_Release_x64"))
            {
                btnLogin.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Open Login";
            }

        }

        public static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        [System.Runtime.InteropServices.LibraryImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        #region Content XML

        private XmlDocument? xmlDoc;

        private void LoadContentXml(string contentFilePath)
        {
            if (!File.Exists(contentFilePath))
            {
                return;
            }

            try
            {
                // Clear the controls on tabContent
                ClearControls(tabContent);

                // Load the XML file
                xmlDoc = new XmlDocument();
                xmlDoc.Load(contentFilePath);

                // Populate the controls with the values from the XML file
                LoadClassSettings();
                LoadGuildSettings();
                LoadGuildContentSettings();
                LoadGeneralSettings();
                LoadPvpSettings();
                LoadShopSettings();
                LoadDungeonSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading settings: " + ex.Message + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ClearControls(Control control)
        {
            foreach (Control childControl in control.Controls)
            {
                if (childControl is TextBox)
                {
                    ((TextBox)childControl).Clear();
                }
                else if (childControl is CheckBox)
                {
                    ((CheckBox)childControl).Checked = false;
                }
                else if (childControl is ComboBox)
                {
                    ((ComboBox)childControl).Items.Clear();
                    ((ComboBox)childControl).SelectedIndex = -1;
                }
                if (childControl.HasChildren)
                {
                    ClearControls(childControl);
                }
            }
        }

        private Dictionary<string, bool> classUseDictionary;
        private Dictionary<string, bool> classFreeUseDictionary;

        private void LoadClassSettings()
        {
            XmlNode? avatarNode = xmlDoc.SelectSingleNode("//avatar");
            if (avatarNode != null)
            {
                int onlyCreate = Convert.ToInt32(avatarNode.Attributes["only_create"].Value);
                onlyCreateCheckBox.Checked = onlyCreate == 1;
            }

            XmlNodeList? classNodes = xmlDoc.SelectNodes("//class");
            if (classNodes != null)
            {
                classUseDictionary = new Dictionary<string, bool>();
                classFreeUseDictionary = new Dictionary<string, bool>();

                foreach (XmlNode classNode in classNodes)
                {
                    string className = classNode.Attributes["name"].Value;
                    classComboBox.Items.Add(className);

                    bool classUse = Convert.ToInt32(classNode.Attributes["use"].Value) != 0;
                    bool classFreeUse = Convert.ToInt32(classNode.Attributes["freeuse"].Value) != 0;

                    classUseDictionary[className] = classUse;
                    classFreeUseDictionary[className] = classFreeUse;
                }
            }

            if (classComboBox.Items.Count > 0)
            {
                classComboBox.SelectedIndex = 0;
            }
        }

        private void classComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedClassName = classComboBox.SelectedItem.ToString();

            XmlNode classNode = xmlDoc.SelectSingleNode($"//class[@name='{selectedClassName}']");
            if (classNode != null)
            {
                string classIDValue = classNode.Attributes["id"].Value;
                if (!string.IsNullOrEmpty(classIDValue) && int.TryParse(classIDValue, out int classID))
                {
                    classIDTextBox.Text = classID.ToString();
                }
                else
                {
                    classIDTextBox.Text = string.Empty;
                }

                classUseCheckBox.Checked = classUseDictionary[selectedClassName];
                classFreeUseCheckBox.Checked = classFreeUseDictionary[selectedClassName];
            }
            else
            {
                classIDTextBox.Text = string.Empty;
                classUseCheckBox.Checked = false;
                classFreeUseCheckBox.Checked = false;
            }
        }


        private void classUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string selectedClassName = classComboBox.SelectedItem?.ToString();

            if (selectedClassName != null && classUseDictionary.ContainsKey(selectedClassName))
            {
                classUseDictionary[selectedClassName] = classUseCheckBox.Checked;
            }
        }

        private void classFreeUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string selectedClassName = classComboBox.SelectedItem?.ToString();

            if (selectedClassName != null && classFreeUseDictionary.ContainsKey(selectedClassName))
            {
                classFreeUseDictionary[selectedClassName] = classFreeUseCheckBox.Checked;
            }
        }

        private void LoadGuildSettings()
        {
            XmlNode? guildCreateNode = xmlDoc.SelectSingleNode("//guild_create");
            if (guildCreateNode != null)
            {
                int guildCreateUse = Convert.ToInt32(guildCreateNode.Attributes["use"].Value);
                int characterLevel = Convert.ToInt32(guildCreateNode.Attributes["character_level"].Value);
                int createCost = Convert.ToInt32(guildCreateNode.Attributes["create_cost"].Value);

                guildCreateUseCheckBox.Checked = guildCreateUse == 1;
                numericUpDownCharacterLevel.Text = characterLevel.ToString();
                createCostTextBox.Text = createCost.ToString();
            }
        }

        private XmlNode? selectedGuildContentNode;

        private void LoadGuildContentSettings()
        {
            XmlNodeList? guildContentNodes = xmlDoc.SelectNodes("//guild_content");
            if (guildContentNodes != null)
            {
                foreach (XmlNode guildContentNode in guildContentNodes)
                {
                    string guildContentName = guildContentNode.Attributes["name"].Value;
                    guildContentComboBox.Items.Add(guildContentName);
                }
            }

            if (guildContentComboBox.Items.Count > 0)
            {
                guildContentComboBox.SelectedIndex = 0;
            }
        }

        private void guildContentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGuildContentName = guildContentComboBox.SelectedItem?.ToString();

            if (selectedGuildContentName != null)
            {
                selectedGuildContentNode = xmlDoc.SelectSingleNode($"//guild_content[@name='{selectedGuildContentName}']");

                if (selectedGuildContentNode != null)
                {
                    int guildContentID = Convert.ToInt32(selectedGuildContentNode.Attributes["id"].Value);
                    bool guildContentUse = Convert.ToInt32(selectedGuildContentNode.Attributes["use"].Value) != 0;

                    guildContentIDTextBox.Text = guildContentID.ToString();
                    guildContentUseCheckBox.Checked = guildContentUse;
                }
            }
        }

        private void guildContentUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedGuildContentNode != null)
            {
                selectedGuildContentNode.Attributes["use"].Value = guildContentUseCheckBox.Checked ? "1" : "0";
            }
        }

        private void LoadGeneralSettings()
        {
            XmlNode? playPointNode = xmlDoc.SelectSingleNode("//play_point");
            XmlNode? cashShopNode = xmlDoc.SelectSingleNode("//cash_shop");
            XmlNode? petInvenNode = xmlDoc.SelectSingleNode("//pet_inven");
            XmlNode? bloodmodeNode = xmlDoc.SelectSingleNode("//bloodmode");
            XmlNode? inquiryNode = xmlDoc.SelectSingleNode("//inquiry");
            XmlNode? itemruneMixNode = xmlDoc.SelectSingleNode("//itemrune_mix");
            XmlNode? eventNode = xmlDoc.SelectSingleNode("//event");
            XmlNode? fortuneNode = xmlDoc.SelectSingleNode("//fortune");
            XmlNode? levelNode = xmlDoc.SelectSingleNode("//level");
            XmlNode? pcroomNode = xmlDoc.SelectSingleNode("//pcroom");
            XmlNode? shutDownNode = xmlDoc.SelectSingleNode("//shut_down");
            XmlNode? couponNode = xmlDoc.SelectSingleNode("//coupon");
            XmlNode? bookPictorialNode = xmlDoc.SelectSingleNode("//book_pictorial");
            XmlNode? partyMissionNode = xmlDoc.SelectSingleNode("//party_mission");
            XmlNode? hiddenDungeonNode = xmlDoc.SelectSingleNode("//hidden_dungeon");
            XmlNode? partyUserNode = xmlDoc.SelectSingleNode("//party_user");
            XmlNode? smartServerNode = xmlDoc.SelectSingleNode("//smart_server");

            // Set the values on the form controls
            playPointCheckBox.Checked = playPointNode != null && Convert.ToInt32(playPointNode.Attributes["use"].Value) == 1;
            cashShopCheckBox.Checked = cashShopNode != null && Convert.ToInt32(cashShopNode.Attributes["use"].Value) == 1;
            cashShopViewCheckBox.Checked = cashShopNode != null && Convert.ToInt32(cashShopNode.Attributes["all_view"].Value) == 1;
            petInvenCheckBox.Checked = petInvenNode != null && Convert.ToInt32(petInvenNode.Attributes["use"].Value) == 1;
            bloodmodeCheckBox.Checked = bloodmodeNode != null && Convert.ToInt32(bloodmodeNode.Attributes["use"].Value) == 1;
            inquiryCheckBox.Checked = inquiryNode != null && Convert.ToInt32(inquiryNode.Attributes["use"].Value) == 1;
            itemruneMixCheckBox.Checked = itemruneMixNode != null && Convert.ToInt32(itemruneMixNode.Attributes["use"].Value) == 1;
            itemDropTextBox.Text = eventNode?.Attributes["item_drop"].Value;
            fortuneCheckBox.Checked = fortuneNode != null && Convert.ToInt32(fortuneNode.Attributes["use"].Value) == 1;
            numericUpDownlevel.Text = levelNode?.Attributes["value"].Value;
            pcroomCheckBox.Checked = pcroomNode != null && Convert.ToInt32(pcroomNode.Attributes["use"].Value) == 1;
            shutdownCheckBox.Checked = shutDownNode != null && Convert.ToInt32(shutDownNode.Attributes["use"].Value) == 1;
            numericUpDownShutdownTime.Text = shutDownNode?.Attributes["time"].Value;
            couponCheckBox.Checked = couponNode != null && Convert.ToInt32(couponNode.Attributes["use"].Value) == 1;
            bookPictorialCheckBox.Checked = bookPictorialNode != null && Convert.ToInt32(bookPictorialNode.Attributes["use"].Value) == 1;
            partyMissionCheckBox.Checked = partyMissionNode != null && Convert.ToInt32(partyMissionNode.Attributes["use"].Value) == 1;
            hiddenDungeonCheckBox.Checked = hiddenDungeonNode != null && Convert.ToInt32(hiddenDungeonNode.Attributes["use"].Value) == 1;

            if (partyUserNode != null)
            {
                numericUpDownSoloMax.Text = partyUserNode.Attributes["solo_max"]?.Value ?? "";
                numericUpDownTutorialMax.Text = partyUserNode.Attributes["tutorial_max"]?.Value ?? "";
                numericUpDownNormalMax.Text = partyUserNode.Attributes["normal_max"]?.Value ?? "";
                numericUpDownMatchingMax.Text = partyUserNode.Attributes["matching_max"]?.Value ?? "";
                numericUpDownRaidMax.Text = partyUserNode.Attributes["raid_max"]?.Value ?? "";
                numericUpDownDefaultMax.Text = partyUserNode.Attributes["default_max"]?.Value ?? "";
            }

            if (smartServerNode != null)
            {
                numericUpDownBasisCount.Text = smartServerNode.Attributes["basis_count"]?.Value ?? "";
                numericUpDownServerLockdown.Text = smartServerNode.Attributes["server_lockdown"]?.Value ?? "";
                numericUpDownChannelAdd.Text = smartServerNode.Attributes["channel_add"]?.Value ?? "";
                numericUpDownChannelLockdown.Text = smartServerNode.Attributes["channel_lockdown"]?.Value ?? "";
                numericUpDownMaxServerChannel.Text = smartServerNode.Attributes["max_server_channel"]?.Value ?? "";
            }
        }

        private void LoadPvpSettings()
        {
            XmlNode? pvpLadderNode = xmlDoc.SelectSingleNode("//pvp_ladder");
            bool pvpLadderUse = pvpLadderNode != null && pvpLadderNode.Attributes["use"].Value != "0";

            XmlNode? pvpMassiveNode = xmlDoc.SelectSingleNode("//pvp_massive");
            bool pvpMassiveUse = pvpMassiveNode != null && pvpMassiveNode.Attributes["use"].Value != "0";

            XmlNode? pvpRaidNode = xmlDoc.SelectSingleNode("//pvp_raid");
            bool pvpRaidUse = pvpRaidNode != null && pvpRaidNode.Attributes["use"].Value != "0";

            pvpLadderCheckBox.Checked = pvpLadderUse;
            pvpMassiveCheckBox.Checked = pvpMassiveUse;
            pvpRaidCheckBox.Checked = pvpRaidUse;
        }

        private XmlNode? selectedShopNode;

        private void LoadShopSettings()
        {
            XmlNodeList? shopNodes = xmlDoc.SelectNodes("//shop");

            if (shopNodes != null)
            {
                foreach (XmlNode shopNode in shopNodes)
                {
                    string shopName = shopNode.Attributes["name"].Value;
                    shopComboBox.Items.Add(shopName);
                }
            }

            if (shopComboBox.Items.Count > 0)
            {
                shopComboBox.SelectedIndex = 0;
            }
        }

        private void shopComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedShopName = shopComboBox.SelectedItem?.ToString();

            if (selectedShopName != null)
            {
                selectedShopNode = xmlDoc.SelectSingleNode($"//shop[@name='{selectedShopName}']");

                if (selectedShopNode != null)
                {
                    int shopID = Convert.ToInt32(selectedShopNode.Attributes["id"].Value);
                    bool shopUse = Convert.ToInt32(selectedShopNode.Attributes["use"].Value) != 0;

                    shopIDTextBox.Text = shopID.ToString();
                    shopUseCheckBox.Checked = shopUse;
                }
            }
        }

        private void shopUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedShopNode != null)
            {
                selectedShopNode.Attributes["use"].Value = shopUseCheckBox.Checked ? "1" : "0";
            }
        }

        private List<Dictionary<string, string>> dungeonSettingsList = new();

        private void LoadDungeonSettings()
        {
            XmlNodeList? dungeonNodes = xmlDoc.SelectNodes("//dungeon");

            dungeonSettingsList = new List<Dictionary<string, string>>();

            if (dungeonNodes != null)
            {
                foreach (XmlNode dungeonNode in dungeonNodes)
                {
                    Dictionary<string, string> dungeonSettings = new();

                    foreach (XmlAttribute attribute in dungeonNode.Attributes)
                    {
                        dungeonSettings[attribute.Name] = attribute.Value;
                    }

                    dungeonSettingsList.Add(dungeonSettings);
                }
            }

            foreach (Dictionary<string, string> settings in dungeonSettingsList)
            {
                if (settings.TryGetValue("name", out string? dungeonName))
                {
                    dungeonComboBox.Items.Add(dungeonName);
                }
            }

            if (dungeonComboBox.Items.Count > 0)
            {
                dungeonComboBox.SelectedIndex = 0;
            }
        }

        private Dictionary<string, string>? currentDungeonSettings;

        private void dungeonComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Save the changes for the current dungeon if there is one selected
            if (currentDungeonSettings != null)
            {
                SaveCurrentDungeonSettings();
            }

            int selectedDungeonIndex = dungeonComboBox.SelectedIndex;

            if (selectedDungeonIndex >= 0 && selectedDungeonIndex < dungeonSettingsList.Count)
            {
                currentDungeonSettings = dungeonSettingsList[selectedDungeonIndex];

                dungeonIDTextBox.Text = currentDungeonSettings["id"];
                numericUpDownDungeonDiff.Text = currentDungeonSettings.ContainsKey("diff") ? currentDungeonSettings["diff"] : "0";
                dungeonDelayTextBox.Text = currentDungeonSettings.ContainsKey("delay") ? currentDungeonSettings["delay"] : "0";
                numericUpDownDungeonCombo.Text = currentDungeonSettings.ContainsKey("combo") ? currentDungeonSettings["combo"] : "0.0";
                dungeonHPTextBox.Text = currentDungeonSettings.ContainsKey("hp") ? currentDungeonSettings["hp"] : "0";
                numericUpDownDungeonAttack.Text = currentDungeonSettings.ContainsKey("attack") ? currentDungeonSettings["attack"] : "0.0";
                rangeDelayTextBox.Text = currentDungeonSettings.ContainsKey("range_delay") ? currentDungeonSettings["range_delay"] : "0";
                rangeComboTextBox.Text = currentDungeonSettings.ContainsKey("range_combo") ? currentDungeonSettings["range_combo"] : "0.0";
                rangeHPTextBox.Text = currentDungeonSettings.ContainsKey("range_hp") ? currentDungeonSettings["range_hp"] : "0";
                numericUpDownRangeAttack.Text = currentDungeonSettings.ContainsKey("range_attack") ? currentDungeonSettings["range_attack"] : "0.0";
                checkBoxUseDungeon.Checked = currentDungeonSettings.ContainsKey("use") && currentDungeonSettings["use"] == "1";
            }
        }

        private void SaveCurrentDungeonSettings()
        {
            if (currentDungeonSettings != null)
            {
                currentDungeonSettings["id"] = dungeonIDTextBox.Text;
                currentDungeonSettings["diff"] = numericUpDownDungeonDiff.Text;
                currentDungeonSettings["delay"] = dungeonDelayTextBox.Text;
                currentDungeonSettings["combo"] = numericUpDownDungeonCombo.Text;
                currentDungeonSettings["hp"] = dungeonHPTextBox.Text;
                currentDungeonSettings["attack"] = numericUpDownDungeonAttack.Text;
                currentDungeonSettings["range_delay"] = rangeDelayTextBox.Text;
                currentDungeonSettings["range_combo"] = rangeComboTextBox.Text;
                currentDungeonSettings["range_hp"] = rangeHPTextBox.Text;
                currentDungeonSettings["range_attack"] = numericUpDownRangeAttack.Text;
                currentDungeonSettings["use"] = checkBoxUseDungeon.Checked ? "1" : "0";

                // Add missing fields if they don't exist in the dictionary
                if (!currentDungeonSettings.ContainsKey("diff"))
                    currentDungeonSettings.Add("diff", "0");
                if (!currentDungeonSettings.ContainsKey("delay"))
                    currentDungeonSettings.Add("delay", "0");
                if (!currentDungeonSettings.ContainsKey("combo"))
                    currentDungeonSettings.Add("combo", "0.0");
                if (!currentDungeonSettings.ContainsKey("hp"))
                    currentDungeonSettings.Add("hp", "0");
                if (!currentDungeonSettings.ContainsKey("attack"))
                    currentDungeonSettings.Add("attack", "0.0");
                if (!currentDungeonSettings.ContainsKey("range_delay"))
                    currentDungeonSettings.Add("range_delay", "0");
                if (!currentDungeonSettings.ContainsKey("range_combo"))
                    currentDungeonSettings.Add("range_combo", "0.0");
                if (!currentDungeonSettings.ContainsKey("range_hp"))
                    currentDungeonSettings.Add("range_hp", "0");
                if (!currentDungeonSettings.ContainsKey("range_attack"))
                    currentDungeonSettings.Add("range_attack", "0.0");
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (currentDungeonSettings != null && dungeonComboBox.SelectedItem != null)
            {
                string selectedDungeonName = currentDungeonSettings["name"];

                // Check if the maximum number of diff entries has been reached for the selected dungeon
                int diffCount = dungeonSettingsList.Count(d => d["name"] == selectedDungeonName);
                if (diffCount >= 4)
                {
                    MessageBox.Show($"Maximum number entries (4) reached for dungeon {selectedDungeonName}.", "Dungeon Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Find the last dungeon with the same name
                int lastDungeonIndex = dungeonSettingsList.FindLastIndex(d => d["name"] == selectedDungeonName);

                // Find the highest diff value for the selected dungeon
                int highestDiff = dungeonSettingsList
                    .Where(d => d["name"] == selectedDungeonName)
                    .Select(d => d.ContainsKey("diff") ? int.Parse(d["diff"]) : 0)
                    .Max();

                // Increment the highest diff value by 1 to get the next available diff
                int newDiff = highestDiff + 1;

                // Check if the new diff already exists, increment it until a unique diff is found
                while (dungeonSettingsList.Any(d => d.ContainsKey("name") && d["name"] == selectedDungeonName && d.ContainsKey("diff") && int.Parse(d["diff"]) == newDiff))
                {
                    newDiff++;
                }

                // Copy the existing dungeon settings and set the new diff value
                Dictionary<string, string> newSetting = new(currentDungeonSettings)
                {
                    ["diff"] = newDiff.ToString()
                };

                // Insert the new setting below the last dungeon with the same name
                dungeonSettingsList.Insert(lastDungeonIndex + 1, newSetting);

                // Insert the new setting name into the dungeonComboBox below the last dungeon with the same name
                string newDungeonName = selectedDungeonName + newDiff;
                dungeonComboBox.Items.Insert(lastDungeonIndex + 1, newDungeonName);

                // Select the new dungeon in the dungeonComboBox
                dungeonComboBox.SelectedIndex = lastDungeonIndex + 1;

                // Enable the fields for the new setting
                EnableDungeonFields(true);
            }
            else
            {
                MessageBox.Show("Dungeon list is empty, please add a dungeon first.", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void EnableDungeonFields(bool enable)
        {
            dungeonIDTextBox.Enabled = enable;
            numericUpDownDungeonDiff.Enabled = enable;
            dungeonDelayTextBox.Enabled = enable;
            numericUpDownDungeonCombo.Enabled = enable;
            dungeonHPTextBox.Enabled = enable;
            numericUpDownDungeonAttack.Enabled = enable;
            rangeDelayTextBox.Enabled = enable;
            rangeComboTextBox.Enabled = enable;
            rangeHPTextBox.Enabled = enable;
            numericUpDownRangeAttack.Enabled = enable;
        }

        private void SaveDungeonSettings()
        {
            // Clear the existing dungeon nodes in the XML
            XmlNodeList? dungeonNodes = xmlDoc.SelectNodes("//dungeon");
            foreach (XmlNode dungeonNode in dungeonNodes)
            {
                xmlDoc.DocumentElement.RemoveChild(dungeonNode);
            }

            // Add the updated dungeon settings to the XML
            foreach (Dictionary<string, string> dungeonSettings in dungeonSettingsList)
            {
                XmlElement newDungeonNode = xmlDoc.CreateElement("dungeon");
                foreach (KeyValuePair<string, string> kvp in dungeonSettings)
                {
                    newDungeonNode.SetAttribute(kvp.Key, kvp.Value);
                }
                xmlDoc.DocumentElement.AppendChild(newDungeonNode);
            }
        }

        private void SaveClassSettings()
        {
            XmlNode avatarNode = xmlDoc.SelectSingleNode("//avatar");
            if (avatarNode == null)
            {
                // Create the avatar node if it doesn't exist
                avatarNode = xmlDoc.CreateElement("avatar");
                xmlDoc.DocumentElement?.AppendChild(avatarNode);
            }

            XmlAttribute onlyCreateAttribute = avatarNode.Attributes["only_create"];
            if (onlyCreateAttribute == null)
            {
                // Create the only_create attribute if it doesn't exist
                onlyCreateAttribute = xmlDoc.CreateAttribute("only_create");
                avatarNode.Attributes.Append(onlyCreateAttribute);
            }

            onlyCreateAttribute.Value = onlyCreateCheckBox.Checked ? "1" : "0";

            XmlNodeList classNodes = xmlDoc.SelectNodes("//class");
            bool classNodeExists = false;
            foreach (XmlNode classNode in classNodes)
            {
                string className = classNode.Attributes["name"].Value;
                if (className == classComboBox.SelectedItem?.ToString())
                {
                    classNode.Attributes["id"].Value = classIDTextBox.Text;
                    classNode.Attributes["use"].Value = classUseCheckBox.Checked ? "1" : "0";
                    classNode.Attributes["freeuse"].Value = classFreeUseCheckBox.Checked ? "1" : "0";
                    classNodeExists = true;
                    break;
                }
            }

            if (!classNodeExists)
            {
                // Create the class node if it doesn't exist
                XmlNode newClassNode = xmlDoc.CreateElement("class");
                XmlAttribute nameAttribute = xmlDoc.CreateAttribute("name");
                nameAttribute.Value = classComboBox.SelectedItem?.ToString() ?? string.Empty;
                newClassNode.Attributes.Append(nameAttribute);
                xmlDoc.DocumentElement?.AppendChild(newClassNode);

                XmlAttribute idAttribute = xmlDoc.CreateAttribute("id");
                idAttribute.Value = classIDTextBox.Text;
                newClassNode.Attributes.Append(idAttribute);

                XmlAttribute useAttribute = xmlDoc.CreateAttribute("use");
                useAttribute.Value = classUseCheckBox.Checked ? "1" : "0";
                newClassNode.Attributes.Append(useAttribute);

                XmlAttribute freeUseAttribute = xmlDoc.CreateAttribute("freeuse");
                freeUseAttribute.Value = classFreeUseCheckBox.Checked ? "1" : "0";
                newClassNode.Attributes.Append(freeUseAttribute);
            }
        }

        private void SaveGuildSettings()
        {
            XmlNode guildCreateNode = xmlDoc.SelectSingleNode("//guild_create");

            if (guildCreateNode != null)
            {
                // Remove the existing guild_create node
                xmlDoc.DocumentElement?.RemoveChild(guildCreateNode);
            }

            // Create a new guild_create node
            guildCreateNode = xmlDoc.CreateElement("guild_create");

            XmlAttribute useAttribute = xmlDoc.CreateAttribute("use");
            useAttribute.Value = guildCreateUseCheckBox.Checked ? "1" : "0";
            guildCreateNode.Attributes.Append(useAttribute);

            XmlAttribute characterLevelAttribute = xmlDoc.CreateAttribute("character_level");
            characterLevelAttribute.Value = numericUpDownCharacterLevel.Text;
            guildCreateNode.Attributes.Append(characterLevelAttribute);

            XmlAttribute createCostAttribute = xmlDoc.CreateAttribute("create_cost");
            createCostAttribute.Value = createCostTextBox.Text;
            guildCreateNode.Attributes.Append(createCostAttribute);

            // Append the new guild_create node to the document
            xmlDoc.DocumentElement?.AppendChild(guildCreateNode);
        }

        private void SaveGuildContentSettings()
        {
            XmlNodeList guildContentNodes = xmlDoc.SelectNodes("//guild_content");

            foreach (XmlNode guildContentNode in guildContentNodes)
            {
                string guildContentName = guildContentNode.Attributes["name"]?.Value;

                if (guildContentName == guildContentComboBox.SelectedItem?.ToString())
                {
                    if (guildContentNode.Attributes["id"] != null)
                    {
                        guildContentNode.Attributes["id"].Value = guildContentIDTextBox.Text;
                    }

                    if (guildContentNode.Attributes["use"] != null)
                    {
                        guildContentNode.Attributes["use"].Value = guildContentUseCheckBox.Checked ? "1" : "0";
                    }
                }
            }
        }

        private void SaveGeneralSettings()
        {
            // play_point node
            XmlNode playPointNode = xmlDoc.SelectSingleNode("//play_point");
            if (playPointNode == null)
            {
                playPointNode = xmlDoc.CreateElement("play_point");
                xmlDoc.DocumentElement?.AppendChild(playPointNode);
            }
            XmlAttribute useAttribute = playPointNode.Attributes["use"];
            if (useAttribute == null)
            {
                useAttribute = xmlDoc.CreateAttribute("use");
                playPointNode.Attributes.Append(useAttribute);
            }
            useAttribute.Value = playPointCheckBox.Checked ? "1" : "0";

            // cash_shop node
            XmlNode cashShopNode = xmlDoc.SelectSingleNode("//cash_shop");
            if (cashShopNode == null)
            {
                cashShopNode = xmlDoc.CreateElement("cash_shop");
                xmlDoc.DocumentElement?.AppendChild(cashShopNode);
            }
            XmlAttribute cashShopUseAttribute = cashShopNode.Attributes["use"];
            if (cashShopUseAttribute == null)
            {
                cashShopUseAttribute = xmlDoc.CreateAttribute("use");
                cashShopNode.Attributes.Append(cashShopUseAttribute);
            }
            cashShopUseAttribute.Value = cashShopCheckBox.Checked ? "1" : "0";

            // cash_shop all_view attribute
            XmlAttribute cashShopAllViewAttribute = cashShopNode.Attributes["all_view"];
            if (cashShopAllViewAttribute == null)
            {
                cashShopAllViewAttribute = xmlDoc.CreateAttribute("all_view");
                cashShopNode.Attributes.Append(cashShopAllViewAttribute);
            }
            cashShopAllViewAttribute.Value = cashShopViewCheckBox.Checked ? "1" : "0";


            // pet_inven node
            XmlNode petInvenNode = xmlDoc.SelectSingleNode("//pet_inven");
            if (petInvenNode == null)
            {
                petInvenNode = xmlDoc.CreateElement("pet_inven");
                xmlDoc.DocumentElement?.AppendChild(petInvenNode);
            }
            XmlAttribute petInvenUseAttribute = petInvenNode.Attributes["use"];
            if (petInvenUseAttribute == null)
            {
                petInvenUseAttribute = xmlDoc.CreateAttribute("use");
                petInvenNode.Attributes.Append(petInvenUseAttribute);
            }
            petInvenUseAttribute.Value = petInvenCheckBox.Checked ? "1" : "0";

            // bloodmode node
            XmlNode bloodmodeNode = xmlDoc.SelectSingleNode("//bloodmode");
            if (bloodmodeNode == null)
            {
                bloodmodeNode = xmlDoc.CreateElement("bloodmode");
                xmlDoc.DocumentElement?.AppendChild(bloodmodeNode);
            }
            XmlAttribute bloodmodeUseAttribute = bloodmodeNode.Attributes["use"];
            if (bloodmodeUseAttribute == null)
            {
                bloodmodeUseAttribute = xmlDoc.CreateAttribute("use");
                bloodmodeNode.Attributes.Append(bloodmodeUseAttribute);
            }
            bloodmodeUseAttribute.Value = bloodmodeCheckBox.Checked ? "1" : "0";

            // inquiry node
            XmlNode inquiryNode = xmlDoc.SelectSingleNode("//inquiry");
            if (inquiryNode == null)
            {
                inquiryNode = xmlDoc.CreateElement("inquiry");
                xmlDoc.DocumentElement?.AppendChild(inquiryNode);
            }
            XmlAttribute inquiryUseAttribute = inquiryNode.Attributes["use"];
            if (inquiryUseAttribute == null)
            {
                inquiryUseAttribute = xmlDoc.CreateAttribute("use");
                inquiryNode.Attributes.Append(inquiryUseAttribute);
            }
            inquiryUseAttribute.Value = inquiryCheckBox.Checked ? "1" : "0";

            // itemrune_mix node
            XmlNode itemruneMixNode = xmlDoc.SelectSingleNode("//itemrune_mix");
            if (itemruneMixNode == null)
            {
                itemruneMixNode = xmlDoc.CreateElement("itemrune_mix");
                xmlDoc.DocumentElement?.AppendChild(itemruneMixNode);
            }
            XmlAttribute itemruneMixUseAttribute = itemruneMixNode.Attributes["use"];
            if (itemruneMixUseAttribute == null)
            {
                itemruneMixUseAttribute = xmlDoc.CreateAttribute("use");
                itemruneMixNode.Attributes.Append(itemruneMixUseAttribute);
            }
            itemruneMixUseAttribute.Value = itemruneMixCheckBox.Checked ? "1" : "0";

            // event node
            XmlNode eventNode = xmlDoc.SelectSingleNode("//event");
            if (eventNode == null)
            {
                eventNode = xmlDoc.CreateElement("event");
                xmlDoc.DocumentElement?.AppendChild(eventNode);
            }
            XmlAttribute eventItemDropAttribute = eventNode.Attributes["item_drop"];
            if (eventItemDropAttribute == null)
            {
                eventItemDropAttribute = xmlDoc.CreateAttribute("item_drop");
                eventNode.Attributes.Append(eventItemDropAttribute);
            }
            eventItemDropAttribute.Value = itemDropTextBox.Text;

            // fortune node
            XmlNode fortuneNode = xmlDoc.SelectSingleNode("//fortune");
            if (fortuneNode == null)
            {
                fortuneNode = xmlDoc.CreateElement("fortune");
                xmlDoc.DocumentElement?.AppendChild(fortuneNode);
            }
            XmlAttribute fortuneUseAttribute = fortuneNode.Attributes["use"];
            if (fortuneUseAttribute == null)
            {
                fortuneUseAttribute = xmlDoc.CreateAttribute("use");
                fortuneNode.Attributes.Append(fortuneUseAttribute);
            }
            fortuneUseAttribute.Value = fortuneCheckBox.Checked ? "1" : "0";

            // level node
            XmlNode levelNode = xmlDoc.SelectSingleNode("//level");
            if (levelNode == null)
            {
                levelNode = xmlDoc.CreateElement("level");
                xmlDoc.DocumentElement?.AppendChild(levelNode);
            }
            XmlAttribute levelValueAttribute = levelNode.Attributes["value"];
            if (levelValueAttribute == null)
            {
                levelValueAttribute = xmlDoc.CreateAttribute("value");
                levelNode.Attributes.Append(levelValueAttribute);
            }
            levelValueAttribute.Value = numericUpDownlevel.Text;

            // pcroom node
            XmlNode pcroomNode = xmlDoc.SelectSingleNode("//pcroom");
            if (pcroomNode == null)
            {
                pcroomNode = xmlDoc.CreateElement("pcroom");
                xmlDoc.DocumentElement?.AppendChild(pcroomNode);
            }
            XmlAttribute pcroomUseAttribute = pcroomNode.Attributes["use"];
            if (pcroomUseAttribute == null)
            {
                pcroomUseAttribute = xmlDoc.CreateAttribute("use");
                pcroomNode.Attributes.Append(pcroomUseAttribute);
            }
            pcroomUseAttribute.Value = pcroomCheckBox.Checked ? "1" : "0";

            // shut_down node
            XmlNode shutDownNode = xmlDoc.SelectSingleNode("//shut_down");
            if (shutDownNode == null)
            {
                shutDownNode = xmlDoc.CreateElement("shut_down");
                xmlDoc.DocumentElement?.AppendChild(shutDownNode);
            }
            XmlAttribute shutDownUseAttribute = shutDownNode.Attributes["use"];
            if (shutDownUseAttribute == null)
            {
                shutDownUseAttribute = xmlDoc.CreateAttribute("use");
                shutDownNode.Attributes.Append(shutDownUseAttribute);
            }
            shutDownUseAttribute.Value = shutdownCheckBox.Checked ? "1" : "0";
            XmlAttribute shutDownTimeAttribute = shutDownNode.Attributes["time"];
            if (shutDownTimeAttribute == null)
            {
                shutDownTimeAttribute = xmlDoc.CreateAttribute("time");
                shutDownNode.Attributes.Append(shutDownTimeAttribute);
            }
            shutDownTimeAttribute.Value = numericUpDownShutdownTime.Text;

            // coupon node
            XmlNode couponNode = xmlDoc.SelectSingleNode("//coupon");
            if (couponNode == null)
            {
                couponNode = xmlDoc.CreateElement("coupon");
                xmlDoc.DocumentElement?.AppendChild(couponNode);
            }
            XmlAttribute couponUseAttribute = couponNode.Attributes["use"];
            if (couponUseAttribute == null)
            {
                couponUseAttribute = xmlDoc.CreateAttribute("use");
                couponNode.Attributes.Append(couponUseAttribute);
            }
            couponUseAttribute.Value = couponCheckBox.Checked ? "1" : "0";

            // book_pictorial node
            XmlNode bookPictorialNode = xmlDoc.SelectSingleNode("//book_pictorial");
            if (bookPictorialNode == null)
            {
                bookPictorialNode = xmlDoc.CreateElement("book_pictorial");
                xmlDoc.DocumentElement?.AppendChild(bookPictorialNode);
            }
            XmlAttribute bookPictorialUseAttribute = bookPictorialNode.Attributes["use"];
            if (bookPictorialUseAttribute == null)
            {
                bookPictorialUseAttribute = xmlDoc.CreateAttribute("use");
                bookPictorialNode.Attributes.Append(bookPictorialUseAttribute);
            }
            bookPictorialUseAttribute.Value = bookPictorialCheckBox.Checked ? "1" : "0";

            // party_mission node
            XmlNode partyMissionNode = xmlDoc.SelectSingleNode("//party_mission");
            if (partyMissionNode == null)
            {
                partyMissionNode = xmlDoc.CreateElement("party_mission");
                xmlDoc.DocumentElement?.AppendChild(partyMissionNode);
            }
            XmlAttribute partyMissionUseAttribute = partyMissionNode.Attributes["use"];
            if (partyMissionUseAttribute == null)
            {
                partyMissionUseAttribute = xmlDoc.CreateAttribute("use");
                partyMissionNode.Attributes.Append(partyMissionUseAttribute);
            }
            partyMissionUseAttribute.Value = partyMissionCheckBox.Checked ? "1" : "0";

            // hidden_dungeon node
            XmlNode hiddenDungeonNode = xmlDoc.SelectSingleNode("//hidden_dungeon");
            if (hiddenDungeonNode == null)
            {
                hiddenDungeonNode = xmlDoc.CreateElement("hidden_dungeon");
                xmlDoc.DocumentElement?.AppendChild(hiddenDungeonNode);
            }
            XmlAttribute hiddenDungeonUseAttribute = hiddenDungeonNode.Attributes["use"];
            if (hiddenDungeonUseAttribute == null)
            {
                hiddenDungeonUseAttribute = xmlDoc.CreateAttribute("use");
                hiddenDungeonNode.Attributes.Append(hiddenDungeonUseAttribute);
            }
            hiddenDungeonUseAttribute.Value = hiddenDungeonCheckBox.Checked ? "1" : "0";

            // party_user node
            XmlNode partyUserNode = xmlDoc.SelectSingleNode("//party_user");
            if (partyUserNode == null)
            {
                partyUserNode = xmlDoc.CreateElement("party_user");
                xmlDoc.DocumentElement?.AppendChild(partyUserNode);
            }

            XmlAttribute soloMaxAttribute = partyUserNode.Attributes["solo_max"];
            if (soloMaxAttribute == null)
            {
                soloMaxAttribute = xmlDoc.CreateAttribute("solo_max");
                partyUserNode.Attributes.Append(soloMaxAttribute);
            }
            soloMaxAttribute.Value = numericUpDownSoloMax.Text;

            XmlAttribute tutorialMaxAttribute = partyUserNode.Attributes["tutorial_max"];
            if (tutorialMaxAttribute == null)
            {
                tutorialMaxAttribute = xmlDoc.CreateAttribute("tutorial_max");
                partyUserNode.Attributes.Append(tutorialMaxAttribute);
            }
            tutorialMaxAttribute.Value = numericUpDownTutorialMax.Text;

            XmlAttribute normalMaxAttribute = partyUserNode.Attributes["normal_max"];
            if (normalMaxAttribute == null)
            {
                normalMaxAttribute = xmlDoc.CreateAttribute("normal_max");
                partyUserNode.Attributes.Append(normalMaxAttribute);
            }
            normalMaxAttribute.Value = numericUpDownNormalMax.Text;

            XmlAttribute matchingMaxAttribute = partyUserNode.Attributes["matching_max"];
            if (matchingMaxAttribute == null)
            {
                matchingMaxAttribute = xmlDoc.CreateAttribute("matching_max");
                partyUserNode.Attributes.Append(matchingMaxAttribute);
            }
            matchingMaxAttribute.Value = numericUpDownMatchingMax.Text;

            XmlAttribute raidMaxAttribute = partyUserNode.Attributes["raid_max"];
            if (raidMaxAttribute == null)
            {
                raidMaxAttribute = xmlDoc.CreateAttribute("raid_max");
                partyUserNode.Attributes.Append(raidMaxAttribute);
            }
            raidMaxAttribute.Value = numericUpDownRaidMax.Text;

            XmlAttribute defaultMaxAttribute = partyUserNode.Attributes["default_max"];
            if (defaultMaxAttribute == null)
            {
                defaultMaxAttribute = xmlDoc.CreateAttribute("default_max");
                partyUserNode.Attributes.Append(defaultMaxAttribute);
            }
            defaultMaxAttribute.Value = numericUpDownDefaultMax.Text;
            partyUserNode.Attributes["solo_max"].Value = numericUpDownSoloMax.Text;
            partyUserNode.Attributes["tutorial_max"].Value = numericUpDownTutorialMax.Text;
            partyUserNode.Attributes["normal_max"].Value = numericUpDownNormalMax.Text;
            partyUserNode.Attributes["matching_max"].Value = numericUpDownMatchingMax.Text;
            partyUserNode.Attributes["raid_max"].Value = numericUpDownRaidMax.Text;
            partyUserNode.Attributes["default_max"].Value = numericUpDownDefaultMax.Text;

            // smart_server node
            XmlNode smartServerNode = xmlDoc.SelectSingleNode("//smart_server");
            if (smartServerNode == null)
            {
                smartServerNode = xmlDoc.CreateElement("smart_server");
                xmlDoc.DocumentElement?.AppendChild(smartServerNode);
            }

            XmlAttribute basisCountAttribute = smartServerNode.Attributes["basis_count"];
            if (basisCountAttribute == null)
            {
                basisCountAttribute = xmlDoc.CreateAttribute("basis_count");
                smartServerNode.Attributes.Append(basisCountAttribute);
            }
            basisCountAttribute.Value = numericUpDownBasisCount.Text;

            XmlAttribute serverLockdownAttribute = smartServerNode.Attributes["server_lockdown"];
            if (serverLockdownAttribute == null)
            {
                serverLockdownAttribute = xmlDoc.CreateAttribute("server_lockdown");
                smartServerNode.Attributes.Append(serverLockdownAttribute);
            }
            serverLockdownAttribute.Value = numericUpDownServerLockdown.Text;

            XmlAttribute channelAddAttribute = smartServerNode.Attributes["channel_add"];
            if (channelAddAttribute == null)
            {
                channelAddAttribute = xmlDoc.CreateAttribute("channel_add");
                smartServerNode.Attributes.Append(channelAddAttribute);
            }
            channelAddAttribute.Value = numericUpDownChannelAdd.Text;

            XmlAttribute channelLockdownAttribute = smartServerNode.Attributes["channel_lockdown"];
            if (channelLockdownAttribute == null)
            {
                channelLockdownAttribute = xmlDoc.CreateAttribute("channel_lockdown");
                smartServerNode.Attributes.Append(channelLockdownAttribute);
            }
            channelLockdownAttribute.Value = numericUpDownChannelLockdown.Text;

            XmlAttribute maxServerChannelAttribute = smartServerNode.Attributes["max_server_channel"];
            if (maxServerChannelAttribute == null)
            {
                maxServerChannelAttribute = xmlDoc.CreateAttribute("max_server_channel");
                smartServerNode.Attributes.Append(maxServerChannelAttribute);
            }
            maxServerChannelAttribute.Value = numericUpDownMaxServerChannel.Text;

            smartServerNode.Attributes["basis_count"].Value = numericUpDownBasisCount.Text;
            smartServerNode.Attributes["server_lockdown"].Value = numericUpDownServerLockdown.Text;
            smartServerNode.Attributes["channel_add"].Value = numericUpDownChannelAdd.Text;
            smartServerNode.Attributes["channel_lockdown"].Value = numericUpDownChannelLockdown.Text;
            smartServerNode.Attributes["max_server_channel"].Value = numericUpDownMaxServerChannel.Text;

        }

        private void SavePvpSettings()
        {
            XmlNode pvpLadderNode = xmlDoc.SelectSingleNode("//pvp_ladder");
            if (pvpLadderNode == null)
            {
                pvpLadderNode = xmlDoc.CreateElement("pvp_ladder");
                xmlDoc.DocumentElement?.AppendChild(pvpLadderNode);
            }

            XmlAttribute useAttribute = pvpLadderNode.Attributes["use"];
            if (useAttribute == null)
            {
                useAttribute = xmlDoc.CreateAttribute("use");
                pvpLadderNode.Attributes.Append(useAttribute);
            }

            useAttribute.Value = pvpLadderCheckBox.Checked ? "1" : "0";

            XmlNode pvpMassiveNode = xmlDoc.SelectSingleNode("//pvp_massive");
            if (pvpMassiveNode == null)
            {
                pvpMassiveNode = xmlDoc.CreateElement("pvp_massive");
                xmlDoc.DocumentElement?.AppendChild(pvpMassiveNode);
            }

            useAttribute = pvpMassiveNode.Attributes["use"];
            if (useAttribute == null)
            {
                useAttribute = xmlDoc.CreateAttribute("use");
                pvpMassiveNode.Attributes.Append(useAttribute);
            }

            useAttribute.Value = pvpMassiveCheckBox.Checked ? "1" : "0";

            XmlNode pvpRaidNode = xmlDoc.SelectSingleNode("//pvp_raid");
            if (pvpRaidNode == null)
            {
                pvpRaidNode = xmlDoc.CreateElement("pvp_raid");
                xmlDoc.DocumentElement?.AppendChild(pvpRaidNode);
            }

            useAttribute = pvpRaidNode.Attributes["use"];
            if (useAttribute == null)
            {
                useAttribute = xmlDoc.CreateAttribute("use");
                pvpRaidNode.Attributes.Append(useAttribute);
            }

            useAttribute.Value = pvpRaidCheckBox.Checked ? "1" : "0";
        }

        private void SaveShopSettings()
        {
            XmlNodeList shopNodes = xmlDoc.SelectNodes("//shop");
            foreach (XmlNode shopNode in shopNodes)
            {
                string shopName = shopNode.Attributes["name"].Value;
                if (shopName == shopComboBox.SelectedItem.ToString())
                {
                    shopNode.Attributes["id"].Value = shopIDTextBox.Text;
                    shopNode.Attributes["use"].Value = shopUseCheckBox.Checked ? "1" : "0";
                }
            }
        }

        private void saveContentButton_Click(object sender, EventArgs e)
        {
            string serverDirectory = textBoxServerDir.Text;
            string optionDirectory = Path.Combine(serverDirectory, "Option");
            string contentFilePath = Path.Combine(optionDirectory, "content_control.xml");

            if (!Directory.Exists(optionDirectory))
            {
                MessageBox.Show("Invalid server directory.\nPlease select the server root folder with the Option folder.", "content_control.xml not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                SaveSettings();
                SaveContentXml(contentFilePath);
                MessageBox.Show("Settings saved successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            SaveClassSettings();
            SaveGuildSettings();
            SaveGuildContentSettings();
            SaveGeneralSettings();
            SavePvpSettings();
            SaveShopSettings();
            SaveCurrentDungeonSettings();
            SaveDungeonSettings();
        }

        private void SaveContentXml(string contentFilePath)
        {
            xmlDoc.Save(contentFilePath);
        }

        public bool DungeonExists(string name, string diff)
        {
            return dungeonSettingsList.Any(d => d["name"] == name && d["diff"] == diff);
        }

        private void buttonNewDungeon_Click(object sender, EventArgs e)
        {
            // Create an instance of the dungeonForm
            DungeonForm dungeonForm = new(this);
            DialogResult result = dungeonForm.ShowDialog();

            // Check if the user confirmed the values
            if (result == DialogResult.OK)
            {
                // Retrieve the entered values from the dungeonForm
                string? Id = dungeonForm.Id;
                string? DungeonName = dungeonForm.DungeonName;
                string? Diff = dungeonForm.Diff;

                string? Delay = dungeonForm.Delay;
                string? Combo = dungeonForm.Combo;
                string? HP = dungeonForm.HP;
                string? Attack = dungeonForm.Attack;
                string? RangeDelay = dungeonForm.RangeDelay;
                string? RangeCombo = dungeonForm.RangeCombo;
                string? RangeHP = dungeonForm.RangeHP;
                string? RangeAttack = dungeonForm.RangeAttack;


                // Create a new dictionary with the entered values
                Dictionary<string, string> newDungeonSettings = new()
                {
                    { "use", "1" },
                    { "id", Id },
                    { "name", DungeonName },
                    { "diff", Diff },
                    { "delay", Delay },
                    { "combo", Combo },
                    { "hp", HP },
                    { "attack", Attack },
                    { "range_delay", RangeDelay },
                    { "range_combo", RangeCombo },
                    { "range_hp", RangeHP },
                    { "range_attack", RangeAttack }
                };


                // Add the new dungeon settings to the list
                dungeonSettingsList.Add(newDungeonSettings);

                // Add the new dungeon name to the combo box
                string newDungeonName = newDungeonSettings["name"];
                dungeonComboBox.Items.Add(newDungeonName);

                // Select the new dungeon in the combo box
                dungeonComboBox.SelectedIndex = dungeonComboBox.Items.Count - 1;

                // Enable the fields for the new setting
                EnableDungeonFields(true);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dungeonComboBox.SelectedItem != null)
            {
                int selectedIndex = dungeonComboBox.SelectedIndex;
                Dictionary<string, string> selectedDungeonSettings = dungeonSettingsList[selectedIndex];

                DungeonForm dungeonForm = new(selectedDungeonSettings, true);
                DialogResult result = dungeonForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Update the selected dungeon settings with the edited values
                    dungeonSettingsList[selectedIndex] = dungeonForm.GetDungeonSettings();

                    // Update the display in the dungeonComboBox
                    dungeonComboBox.Items[selectedIndex] = dungeonForm.GetDungeonName();

                    // Optionally, update the currentDungeonSettings if the edited dungeon was the current one
                    if (currentDungeonSettings == selectedDungeonSettings)
                    {
                        currentDungeonSettings = dungeonForm.GetDungeonSettings();
                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (currentDungeonSettings != null && dungeonComboBox.SelectedItem != null)
            {
                // Get the index of the selected dungeon
                int selectedDungeonIndex = dungeonComboBox.SelectedIndex;

                // Remove the selected dungeon from the list
                dungeonSettingsList.RemoveAt(selectedDungeonIndex);

                // Remove the selected dungeon from the combo box
                dungeonComboBox.Items.RemoveAt(selectedDungeonIndex);

                // Select the previous dungeon in the combo box if available
                if (selectedDungeonIndex > 0)
                {
                    dungeonComboBox.SelectedIndex = selectedDungeonIndex - 1;
                }
                else if (dungeonComboBox.Items.Count > 0)
                {
                    dungeonComboBox.SelectedIndex = 0;
                }
                else
                {
                    // No dungeons left, clear the fields and disable them
                    currentDungeonSettings = null;
                    dungeonIDTextBox.Text = "";
                    numericUpDownDungeonDiff.Value = 0;
                    dungeonDelayTextBox.Text = "";
                    numericUpDownDungeonCombo.Value = 0;
                    dungeonHPTextBox.Text = "";
                    numericUpDownDungeonAttack.Value = 0;
                    rangeDelayTextBox.Text = "";
                    rangeComboTextBox.Text = "";
                    rangeHPTextBox.Text = "";
                    numericUpDownRangeAttack.Value = 0;
                    EnableDungeonFields(false);
                }
            }
        }

        #endregion

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;

            if (numericUpDown.Value == numericUpDown.Maximum)
            {
                numericUpDown.Value = numericUpDown.Minimum;
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void VersionLabel_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = Url, UseShellExecute = true });
        }

        private void pictureBoxEyeSqlPassword_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxSQLPassword.UseSystemPasswordChar = false;

        }

        private void pictureBoxEyeSqlPassword_MouseUp(object sender, MouseEventArgs e)
        {
            textBoxSQLPassword.UseSystemPasswordChar = true;

        }

        private void pictureBoxEyeDbPassword_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxDbPassword.UseSystemPasswordChar = false;

        }

        private void pictureBoxEyeDbPassword_MouseUp(object sender, MouseEventArgs e)
        {
            textBoxDbPassword.UseSystemPasswordChar = true;

        }

        private void pictureBoxEyeSmtpPassword_MouseDown(object sender, MouseEventArgs e)
        {
            textBoxSmtpPassword.UseSystemPasswordChar = false;

        }

        private void pictureBoxEyeSmtpPassword_MouseUp(object sender, MouseEventArgs e)
        {
            textBoxSmtpPassword.UseSystemPasswordChar = true;

        }
    }

}