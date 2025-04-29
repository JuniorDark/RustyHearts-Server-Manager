using Microsoft.Data.SqlClient;
using RHServerManager.Properties;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;

namespace RHServerManager
{
    public partial class MainForm : Form
    {
        private readonly string Url = "https://github.com/JuniorDark/RustyHearts-Server-Manager";

        #region Configuration Constants

        private readonly string localReplace = "PRIVATE_IP";
        private readonly string publicReplace = "PUBLIC_IP";
        private readonly string dbServerReplace = "SQL_DB_SERVER";
        private readonly string dbUserReplace = "SQL_DB_USER";
        private readonly string dbPasswdReplace = "SQL_DB_PASSWORD";

        #endregion

        #region Configuration Variables

        private string privateIP = string.Empty;
        private string publicIP = string.Empty;
        private string dbServer = string.Empty;
        private string dbUser = string.Empty;
        private string dbPasswd = string.Empty;
        private string dirServer = string.Empty;
        private string dirAPI = string.Empty;
        private string apiIP = string.Empty;

        #endregion

        #region Default Configuration Values

        private const string DefaultPrivateIP = "127.0.0.1";
        private const string DefaultServerRegion = "jpn";
        private const string DefaultAuthUrl = "http://127.0.0.1:8070/Auth";
        private const string DefaultBillingUrl = "http://127.0.0.1:8070/Billing";

        #endregion

        #region Form
        public MainForm()
        {
            InitializeComponent();

            lbVersion.Text = $"Version: {GetVersion()}";

            System.Windows.Forms.Timer t = new()
            {
                Interval = 5000
            };
            t.Tick += CheckProcessStatus;

            t.Start();
        }

        #region Event Handlers

        private readonly Dictionary<Button, string> originalButtonTexts = [];

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            CheckProcessStatus();

            foreach (var button in new[]
            {
                btnStartAPIJpn,
                btnStartAPIUsa,
                btnStartAPIAll,
                btnStartAPIPM2Jpn,
                btnStartAPIPM2Usa,
                btnStartAPIPM2All
            })
            {
                originalButtonTexts[button] = button.Text;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfiguration();
        }

        #endregion

        #region Configuration

        private const string ConfigFileName = "Config.ini";

        private void LoadConfiguration()
        {
            try
            {
                IniFile ini = new(ConfigFileName);
                tbPrivateIP.Text = ini.ReadValue("PrivateIP", DefaultPrivateIP);
                tbPublicIP.Text = ini.ReadValue("PublicIP", GetIPv4Address());
                tbServerDir.Text = ini.ReadValue("DirServer");
                tbAPIDir.Text = ini.ReadValue("DirAPI");
                tbSQLAddress.Text = ini.ReadValue("SqlServer", Dns.GetHostName());
                tbSQLPassword.Text = ini.ReadValue("SqlPasswd");
                tbSQLAccount.Text = ini.ReadValue("SqlUser", "sa");
                tbServerName.Text = ini.ReadValue("ServerName", "My Server");

                if (string.IsNullOrEmpty(dirServer))
                {
                    gbServerControls.Enabled = false;
                    gbSaveSettings.Enabled = false;
                    tabService.Enabled = false;
                    tabContentControl.Enabled = false;
                    btnOpenServerDir.Enabled = false;
                }
                if (string.IsNullOrEmpty(dirAPI))
                {
                    btnSaveAPI.Enabled = false;
                    gbApiConfig.Enabled = false;
                    btnOpenApiDir.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                IniFile ini = new(ConfigFileName);
                ini.WriteValue("DirServer", tbServerDir.Text);
                ini.WriteValue("DirAPI", tbAPIDir.Text);
                ini.WriteValue("SqlServer", tbSQLAddress.Text);
                ini.WriteValue("SqlUser", tbSQLAccount.Text);
                ini.WriteValue("SqlPasswd", tbSQLPassword.Text);
                ini.WriteValue("PrivateIP", tbPrivateIP.Text);
                ini.WriteValue("PublicIP", tbPublicIP.Text);
                ini.WriteValue("ServerName", tbServerName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving configuration: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Utilities

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
                            return ipAddress.Address.ToString();
                        }
                    }
                }
            }

            return string.Empty;
        }

        public static string GetVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

            string version = $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";

            return version;
        }

        #endregion

        #region Process Handling

        private void CheckProcessStatus()
        {
            bool agentProcessRunning = IsProcessRunning("Agent_Release_x64") || IsProcessRunning("AgentManager_Release_x64");
            bool gatewayServerRunning = IsProcessRunning("GameGatewayServer_Release_x64");

            tabControlServerSettings.Enabled = !agentProcessRunning;
            btnStartServers.Enabled = !agentProcessRunning;
            btnStartServers.Text = agentProcessRunning ? "Running..." : "Start Servers";
            btnStopServers.Enabled = agentProcessRunning;
            btnStopServers.Text = agentProcessRunning ? "Stop Servers" : "Stop Servers";
            btnKillServers.Enabled = agentProcessRunning;
            btnKillServers.Text = agentProcessRunning ? "Kill Servers" : "Kill Servers";
            btnClearServerLogs.Enabled = !agentProcessRunning;

            btnGateLogin.Enabled = gatewayServerRunning;
        }

        public static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }
        #endregion

        #region API
        private Dictionary<string, Control> GetUiElements()
        {
            return new Dictionary<string, Control>
            {
                { "API_LISTEN_HOST", tbAPIIP },
                { "API_LISTEN_PORT", tbApiPort },
                { "API_LOCAL_LISTEN_HOST", tbApiLocalIP },
                { "API_USA_PORT", tbUsaPort },
                { "API_JPN_PORT", tbJpnPort },
                { "API_PROXY_PORT", tbProxyPort },
                { "API_ENABLE_HELMET", cbHelmet },
                { "API_TRUSTPROXY_ENABLE", cbTrustProxy },
                { "API_TRUSTPROXY_HOSTS", tbTrustProxyHosts },
                { "TZ", tbTimeZone },
                { "API_SHOP_INITIAL_BALANCE", tbShopBalance },
                { "LOG_LEVEL", cmbLogLevel },
                { "LOG_AUTH_CONSOLE", cbLogAuthConsole },
                { "LOG_BILLING_CONSOLE", cbLogBillingConsole },
                { "LOG_ACCOUNT_CONSOLE", cbLogAccountConsole },
                { "LOG_MAILER_CONSOLE", cbLogMailerConsole },
                { "LOG_IP_ADDRESSES", cbLogIPAddresses },
                { "DB_SERVER", tbDBServer },
                { "DB_DATABASE", tbDBName },
                { "DB_USER", tbDBUser },
                { "DB_PASSWORD", tbDBPassword },
                { "DB_ENCRYPT", cbDBEncrypt },
                { "GATESERVER_IP", tbGateServerIP },
                { "GATESERVER_PORT", tbGateServerPort },
                { "SMTP_HOST", tbSmtpHost },
                { "SMTP_PORT", tbSmtpPort },
                { "SMTP_ENCRYPTION", cmbSmtpEncryption },
                { "SMTP_USERNAME", tbSmtpUsername },
                { "SMTP_PASSWORD", tbSmtpPassword },
                { "SMTP_EMAIL_FROM_ADDRESS", tbSmtpSender },
                { "SMTP_FROM_NAME", tbSmtpFromName }
            };
        }

        private void LoadAPIConfig()
        {
            try
            {
                dirAPI = tbAPIDir.Text;

                if (string.IsNullOrEmpty(dirAPI))
                {
                    gbAPIControls.Enabled = false;
                    gbApiConfig.Enabled = false;
                    btnSaveAPI.Enabled = false;
                    btnOpenApiDir.Enabled = false;
                    ClearControls(gbApiConfig);
                }
                else
                {
                    string envFilePath = Path.Combine(dirAPI, ".env");

                    if (!Directory.Exists(dirAPI) || !File.Exists(envFilePath))
                    {
                        MessageBox.Show("Invalid API folder. Please select the server folder with the .env config file.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tbAPIDir.Text = "";
                        gbAPIControls.Enabled = false;
                        gbApiConfig.Enabled = false;
                        btnSaveAPI.Enabled = false;
                        btnOpenApiDir.Enabled = false;
                        ClearControls(gbApiConfig);
                    }
                    else
                    {
                        ClearControls(gbApiConfig);
                        LoadAPIConfig(envFilePath);
                        gbAPIControls.Enabled = true;
                        gbApiConfig.Enabled = true;
                        btnSaveAPI.Enabled = true;
                        btnOpenApiDir.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading api settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAPIConfig(string envFilePath)
        {
            try
            {
                if (!File.Exists(envFilePath))
                {
                    return;
                }

                var uiElements = GetUiElements();
                string[] logLevels = ["info", "debug", "warn", "error"];
                cmbLogLevel.Items.AddRange(logLevels);
                string[] smtpEncryptionTypes = ["ssl", "tsl"];
                cmbSmtpEncryption.Items.AddRange(smtpEncryptionTypes);

                using StreamReader reader = new(envFilePath);

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    {
                        continue;
                    }

                    string[] parts = line.Split(['='], 2, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2 && uiElements.TryGetValue(parts[0].Trim(), out Control? control))
                    {
                        string value = parts[1].Trim();

                        if (control is TextBox textBox)
                        {
                            textBox.Text = value;
                        }
                        else if (control is CheckBox checkBox)
                        {
                            if (bool.TryParse(value, out bool boolValue))
                            {
                                checkBox.Checked = boolValue;
                            }
                        }
                        else if (control is ComboBox comboBox)
                        {
                            comboBox.SelectedItem = comboBox.Items.Cast<object>()
                                        .FirstOrDefault(item => item.ToString() == value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the API .env file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBrowseAPI_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowser = new();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowser.SelectedPath;
                tbAPIDir.Text = selectedPath;
            }
        }

        private void BtnUpdateAPI_Click(object sender, EventArgs e)
        {
            try
            {
                dirAPI = tbAPIDir.Text;
                string envFilePath = Path.Combine(dirAPI, ".env");

                if (!string.IsNullOrEmpty(envFilePath))
                {
                    string fileName = ".env";
                    object resource = Resources.env;

                    byte[] resourceBytes = (byte[])resource;
                    string resourceString = Encoding.UTF8.GetString(resourceBytes);
                    string? content = ReplaceApiVALUES(resourceString);

                    SaveApiText(fileName, content);
                    MessageBox.Show("Api settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving api settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string? ReplaceApiVALUES(string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            var uiElements = GetUiElements();

            foreach (var entry in uiElements)
            {
                string key = entry.Key;
                Control control = entry.Value;

                string placeholder = $"{key}_VALUE";

                string? controlValue = control switch
                {
                    TextBox textBox => textBox.Text,
                    ComboBox comboBox => comboBox.Text,
                    CheckBox checkBox => checkBox.Checked ? "true" : "false",
                    _ => null
                };

                if (controlValue != null)
                {
                    content = content.Replace(placeholder, controlValue);
                }
            }

            return content;
        }

        private void SaveApiText(string fileName, string? content)
        {
            File.WriteAllText(Path.Combine(dirAPI, fileName), content, Encoding.Default);
        }

        private Process? cmdProcess;
        private System.Windows.Forms.Timer? processCheckTimer;

        private async void BtnStartAPIJpn_Click(object sender, EventArgs e)
        {
            await StartAPI("Jpn", false);
        }

        private async void BtnStartAPIUsa_Click(object sender, EventArgs e)
        {
            await StartAPI("Usa", false);
        }

        private async void BtnStartAPIAll_Click(object sender, EventArgs e)
        {
            await StartAPI("All", false);
        }

        private async void BtnStopAPI_Click(object sender, EventArgs e)
        {
            await StopAPI(false);
        }

        private async void BtnStartAPIPM2Jpn_Click(object sender, EventArgs e)
        {
            await StartAPI("Jpn", true);
        }

        private async void BtnStartAPIPM2Usa_Click(object sender, EventArgs e)
        {
            await StartAPI("Usa", true);
        }

        private async void BtnStartAPIPM2All_Click(object sender, EventArgs e)
        {
            await StartAPI("All", true);
        }

        private async void BtnStopAPIPM2_Click(object sender, EventArgs e)
        {
            await StopAPI(true);
        }

        private async Task StartAPI(string service, bool usePM2)
        {
            if (!Directory.Exists(tbAPIDir.Text))
            {
                MessageBox.Show("Please select the API folder location", "API folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => UpdateAPIControlsState("Starting...", true, 0, usePM2)));
                    }
                    else
                    {
                        UpdateAPIControlsState("Starting...", true, 0, usePM2);
                    }

                    string batchFilePath = service switch
                    {
                        "Jpn" => Path.Combine(dirAPI, "start-JPN.bat"),
                        "Usa" => Path.Combine(dirAPI, "start-USA.bat"),
                        "All" => Path.Combine(dirAPI, "start-All.bat"),
                        _ => throw new ArgumentException($"Invalid service name: {service}")
                    };
                    string batchFilePM2Path = service switch
                    {
                        "Jpn" => Path.Combine(dirAPI, "start_with_pm2_JPN.bat"),
                        "Usa" => Path.Combine(dirAPI, "start_with_pm2_USA.bat"),
                        "All" => Path.Combine(dirAPI, "start_with_pm2_All.bat"),
                        _ => throw new ArgumentException($"Invalid service name: {service}")
                    };

                    string batchFileToRun = usePM2 ? batchFilePM2Path : batchFilePath;

                    if (!File.Exists(batchFileToRun))
                    {
                        MessageBox.Show($"Batch file not found: {batchFileToRun}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ProcessStartInfo startInfo = new("cmd.exe")
                    {
                        WorkingDirectory = dirAPI,
                        Arguments = "/c \"" + batchFileToRun + "\"",
                        UseShellExecute = false
                    };

                    cmdProcess = new Process
                    {
                        StartInfo = startInfo
                    };
                    cmdProcess.Start();

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => UpdateAPIControlsState("Running...", false, 2000, usePM2)));
                    }
                    else
                    {
                        UpdateAPIControlsState("Running...", false, 2000, usePM2);
                    }
                    
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while starting the API: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task StopAPI(bool usePM2)
        {
            await Task.Run(() =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateAPIControlsState("Stopping...", false, 0, usePM2)));
                }
                else
                {
                    UpdateAPIControlsState("Stopping...", false, 0, usePM2);
                }

                if (usePM2)
                {
                    if (cmdProcess != null && !cmdProcess.HasExited)
                    {
                        cmdProcess.CloseMainWindow();
                        cmdProcess.WaitForExit();
                    }

                    ProcessStartInfo stopInfo = new("cmd.exe")
                    {
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = dirAPI,
                        Arguments = "/c npx pm2 stop all"
                    };

                    Process stopProcess = new()
                    {
                        StartInfo = stopInfo
                    };

                    stopProcess.Start();
                    stopProcess.WaitForExit();
                }
                else
                {
                    if (cmdProcess != null && !cmdProcess.HasExited)
                    {
                        cmdProcess.CloseMainWindow();
                        cmdProcess.WaitForExit();
                    }
                }

                StopAPITimerAndDispose();

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateAPIControlsState("RestoreOriginalText", true)));
                }
                else
                {
                    UpdateAPIControlsState("RestoreOriginalText", true);
                }
                
            });
        }

        private void UpdateAPIControlsState(string buttonText, bool enable, int timerInterval = 0, bool isPM2 = false)
        {
            var buttons = new[]
            {
                btnStartAPIJpn,
                btnStartAPIUsa,
                btnStartAPIAll,
                btnStartAPIPM2Jpn,
                btnStartAPIPM2Usa,
                btnStartAPIPM2All
            };

            foreach (var button in buttons)
            {
                if (buttonText == "RestoreOriginalText")
                    button.Text = originalButtonTexts[button];
                else
                    button.Text = buttonText;

                button.Enabled = enable;
            }

            btnStopAPI.Enabled = !enable && !isPM2;
            btnStopAPIPM2.Enabled = !enable && isPM2;
            tabApi.Enabled = enable;
            btnClearAPILogs.Enabled = enable;

            if (timerInterval > 0)
            {
                processCheckTimer = new System.Windows.Forms.Timer
                {
                    Interval = timerInterval
                };
                processCheckTimer.Tick += ProcessCheckTimer_Tick;
                processCheckTimer.Start();
            }
        }

        private void ProcessCheckTimer_Tick(object? sender, EventArgs e)
        {
            if (cmdProcess == null || cmdProcess.HasExited)
            {
                StopAPITimerAndDispose();

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateAPIControlsState("RestoreOriginalText", true)));
                }
                else
                {
                    UpdateAPIControlsState("RestoreOriginalText", true);
                }
            }
        }

        private void StopAPITimerAndDispose()
        {
            processCheckTimer?.Stop();
            cmdProcess?.Dispose();
            cmdProcess = null;
        }
        #endregion

        #region Server Config

        private void BtnUpdateOption_Click(object sender, EventArgs e)
        {
            try
            {
                FetchServerInputs();

                List<string> missingFields = [];

                if (string.IsNullOrEmpty(dirServer)) missingFields.Add("Server Folder");
                if (string.IsNullOrEmpty(privateIP)) missingFields.Add("Private IP");
                if (string.IsNullOrEmpty(publicIP)) missingFields.Add("Public IP");
                if (string.IsNullOrEmpty(dbServer)) missingFields.Add("SQL Server");
                if (string.IsNullOrEmpty(dbUser)) missingFields.Add("SQL User");
                if (string.IsNullOrEmpty(dbPasswd)) missingFields.Add("SQL Password");

                if (missingFields.Count > 0)
                {
                    ShowMissingFieldsWarning(missingFields, "Incomplete Information");
                    return;
                }

                if (MessageBox.Show("Save server settings?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string optionPath = Path.Combine(dirServer, "Option");
                    Directory.CreateDirectory(optionPath);

                    string[] fileNames =
                    [
                        "agent.xml", "AGENT_800.xml", "agent_serveroption.xml", "agentmanager_serveroption.xml", "AGENTMANAGER_900.xml",
                        "auction_serveroption.xml", "AUCTION_101.xml", "customoption.xml", "DBC_CHAT_3.xml", "DBC_EMERGENCY_4.xml",
                        "DBC_GAME_1.xml", "DBC_LOG_2.xml", "dbc_serveroption.xml", "dbc_serveroptionlog.xml", "dungeon_serveroption.xml",
                        "DUNGEON_2001.xml", "game_serveroption.xml", "GATE_81.xml", "gate_serveroption.xml", "gm_serveroption.xml",
                        "GM_71.xml", "guild_serveroption.xml", "GUILD_5001.xml", "lobby_serveroption.xml", "LOBBY_21001.xml",
                        "manager_serveroption.xml", "MANAGER_91.xml", "match_serveroption.xml", "MATCH_111.xml", "msg_serveroption.xml",
                        "MSG_61.xml", "pvp_serveroption.xml", "PVP_3003.xml", "server_info.ini", "serveroption.xml"
                    ];

                    object[] resources =
                    [
                        Resources.agent, Resources.AGENT_800, Resources.agent_serveroption, Resources.agentmanager_serveroption, Resources.AGENTMANAGER_900,
                        Resources.auction_serveroption, Resources.AUCTION_101, Resources.customoption, Resources.DBC_CHAT_3, Resources.DBC_EMERGENCY_4,
                        Resources.DBC_GAME_1, Resources.DBC_LOG_2, Resources.dbc_serveroption, Resources.dbc_serveroptionlog, Resources.dungeon_serveroption,
                        Resources.DUNGEON_2001, Resources.game_serveroption, Resources.GATE_81, Resources.gate_serveroption, Resources.gm_serveroption,
                        Resources.GM_71, Resources.guild_serveroption, Resources.GUILD_5001, Resources.lobby_serveroption, Resources.LOBBY_21001,
                        Resources.manager_serveroption, Resources.MANAGER_91, Resources.match_serveroption, Resources.MATCH_111, Resources.msg_serveroption,
                        Resources.MSG_61, Resources.pvp_serveroption, Resources.PVP_3003, Resources.server_info, Resources.serveroption
                    ];

                    try
                    {
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            string? content = ReplaceOptionValues((string)resources[i]);
                            SaveOptionText(fileNames[i], content);
                        }

                        SaveConfiguration();
                        MessageBox.Show("Options settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while saving option settings:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateDB_Click(object sender, EventArgs e)
        {
            try
            {
                FetchServerInputs();
                string serverName = tbServerName.Text;
                List<string> missingFields = [];

                if (string.IsNullOrEmpty(serverName)) missingFields.Add("Server Name");
                if (string.IsNullOrEmpty(privateIP)) missingFields.Add("Private IP");
                if (string.IsNullOrEmpty(publicIP)) missingFields.Add("Public IP");
                if (string.IsNullOrEmpty(dbServer)) missingFields.Add("SQL Server");
                if (string.IsNullOrEmpty(dbUser)) missingFields.Add("SQL User");
                if (string.IsNullOrEmpty(dbPasswd)) missingFields.Add("SQL Password");

                if (missingFields.Count > 0)
                {
                    ShowMissingFieldsWarning(missingFields, "Incomplete Database Information");
                    return;
                }

                if (MessageBox.Show("Save settings to the database?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string connectionString = $"Data Source={dbServer};Initial Catalog=RustyHearts_Auth;TrustServerCertificate=true;User Id={dbUser};Password={dbPasswd};";

                    try
                    {
                        using SqlConnection connection = new(connectionString);
                        connection.Open();

                        UpdateDatabase(connection, "UPDATE [ServerOption] SET [PublicAddress] = @publicIP WHERE [PublicAddress] != ''", "@publicIP", publicIP);
                        UpdateDatabase(connection, "UPDATE [ServerOption] SET [PrivateAddress] = @privateIP WHERE [PrivateAddress] != ''", "@privateIP", privateIP);
                        UpdateDatabase(connection, "UPDATE WorldServer SET Name = @serverName", "@serverName", serverName);

                        MessageBox.Show("Database settings saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Database Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBrowseServer_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowser = new();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                tbServerDir.Text = folderBrowser.SelectedPath;
            }
        }

        private void BtnOpenServerDir_Click(object sender, EventArgs e)
        {
            OpenFolder(tbServerDir.Text);
        }

        private void BtnOpenApiDir_Click(object sender, EventArgs e)
        {
            OpenFolder(tbAPIDir.Text);
        }

        private static void OpenFolder(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                Process.Start("explorer.exe", directory);
            }
        }

        private string? ReplaceOptionValues(string content)
        {
            return content?
                .Replace(dbServerReplace, dbServer)
                .Replace(dbPasswdReplace, dbPasswd)
                .Replace(dbUserReplace, dbUser)
                .Replace(localReplace, privateIP)
                .Replace(publicReplace, publicIP);
        }

        private void SaveOptionText(string name, string? content)
        {
            if (content != null)
            {
                File.WriteAllText(Path.Combine(dirServer, "Option", name), content, Encoding.Default);
            }
        }

        private void FetchServerInputs()
        {
            dirServer = tbServerDir.Text;
            privateIP = tbPrivateIP.Text;
            publicIP = tbPublicIP.Text;
            dbServer = tbSQLAddress.Text;
            dbUser = tbSQLAccount.Text;
            dbPasswd = tbSQLPassword.Text;
        }

        private static void ShowMissingFieldsWarning(List<string> missingFields, string caption)
        {
            string message = "Please fill in all required fields (marked with *)\nMissing:\n" + string.Join(", ", missingFields);
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void UpdateDatabase(SqlConnection connection, string commandText, string paramName, string paramValue)
        {
            using SqlCommand command = new(commandText, connection);
            command.Parameters.AddWithValue(paramName, paramValue);
            command.ExecuteNonQuery();
        }

        private const string AgentProcessName = "Agent_Release_x64";
        private const string AgentManagerProcessName = "AgentManager_Release_x64";
        private const string AuctionProcessName = "AuctionServer_Release_USA_x64";
        private const int ProcessStartDelay = 1000;
        private const int SetForegroundWindowDelay = 500;

        private async void BtnStartServers_Click(object sender, EventArgs e)
        {
            string serverDirectory = tbServerDir.Text;

            if (!Directory.Exists(serverDirectory))
            {
                MessageBox.Show("Please select the server folder location", "Server folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetServerButtonsState(true, "Starting...");

            string[] serverProcesses = [AgentProcessName, AgentManagerProcessName];

            try
            {
                foreach (string processName in serverProcesses)
                {
                    string fullProcessPath = Path.Combine(serverDirectory, $"{processName}.exe");

                    if (!File.Exists(fullProcessPath))
                    {
                        MessageBox.Show($"The process {processName} was not found in the server folder.", "Process not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetServerButtonsState(false, "Start Servers");
                        return;
                    }

                    ProcessStartInfo startInfo = new(Path.Combine(serverDirectory, processName))
                    {
                        WorkingDirectory = serverDirectory
                    };

                    using Process? serverProcess = Process.Start(startInfo);

                    if (serverProcess != null)
                    {
                        SetForegroundWindow(serverProcess.MainWindowHandle);

                        if (processName == AgentProcessName)
                        {
                            await Task.Delay(ProcessStartDelay);

                            // Activate the main window and send keys
                            SetForegroundWindow(serverProcess.MainWindowHandle);
                            await Task.Delay(SetForegroundWindowDelay);
                            SendKeys.SendWait("1");
                        }
                    }

                }

                SetServerButtonsState(true, "Running...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while starting servers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetServerButtonsState(bool enabled, string buttonText)
        {
            btnStopServers.Enabled = enabled;
            btnStopServers.Text = enabled ? "Stop Servers" : buttonText;
            btnKillServers.Enabled = enabled;
            btnKillServers.Text = enabled ? "Kill Servers" : buttonText;
            tabServer.Enabled = !enabled;
            btnStartServers.Enabled = !enabled;
            btnStartServers.Text = !enabled ? "Start Servers" : buttonText;
            btnClearServerLogs.Enabled = !enabled;
        }

        private void BtnStopServers_Click(object sender, EventArgs e)
        {
            StopServer("Stop");
        }

        private void BtnKillServers_Click(object sender, EventArgs e)
        {
            StopServer("Kill");
        }

        private void StopServer(string Type)
        {
            StopServer(Type, tabControlSettings);
        }

        private async void StopServer(string Type, TabControl tabControlSettings)
        {
            try
            {
                string confirmationMessage = Type == "Stop" ? "Are you sure you want to stop the servers?" : "Are you sure you want to kill the servers?";

                DialogResult result = MessageBox.Show(confirmationMessage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await Task.Run(async () =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => SetServerButtonsState(true, "Closing...")));
                        }
                        else
                        {
                            SetServerButtonsState(true, "Closing...");
                        }
                        
                        string stopServersParameter = Type == "Stop" ? "5" : "9";
                        await StopServers(stopServersParameter);
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => SetServerButtonsState(false, "Start Servers")));
                        }
                        else
                        {
                            SetServerButtonsState(false, "Start Servers");
                        }
                        
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while stopping the servers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task StopServers(string keyCode)
        {
            await Task.Run(() =>
            {
                string[] processNames = [AgentProcessName, AgentManagerProcessName, AuctionProcessName];
                List<Process> processes = [];
                Process? selectedProcess = null;

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
                    SendKeys.SendWait(keyCode);
                }

                foreach (Process process in processes)
                {
                    if (!process.HasExited)
                    {
                        Thread.Sleep(5000);
                        process.CloseMainWindow();
                        process.WaitForExit();
                        process.Dispose();
                    }
                }
            });
        }

        private static void ClearLogs(string directory, string folderName, string[] fileTypes)
        {
            if (DialogResult.Yes == MessageBox.Show($"Are you sure you want to delete the {folderName} logs?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {
                if (!Directory.Exists(directory))
                {
                    MessageBox.Show($"Please select the {folderName} folder location", $"{folderName} folder not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] files = [.. Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly).Where(file => fileTypes.Any(file.EndsWith))];

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting {file}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Clear PM2 log files
                string pm2LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".pm2", "logs");

                if (Directory.Exists(pm2LogsDirectory))
                {
                    string[] pm2Files = [.. Directory.GetFiles(pm2LogsDirectory, "*.*", SearchOption.TopDirectoryOnly).Where(file => fileTypes.Any(file.EndsWith))];

                    foreach (string pm2File in pm2Files)
                    {
                        try
                        {
                            File.Delete(pm2File);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while deleting {pm2File}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                if (Directory.Exists(Path.Combine(directory, "Server_Log")))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(directory, "Server_Log"), true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting Server_Log folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                var logPaths = new[]
                {
                    Path.Combine(directory, "Log"),
                    Path.Combine(directory, "logs")
                };

                foreach (var logPath in logPaths)
                {
                    if (Directory.Exists(logPath))
                    {
                        try
                        {
                            Directory.Delete(logPath, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while deleting {Path.GetFileName(logPath)} folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                string[] dumpDirectories = Directory.GetDirectories(directory, "Dump_*", SearchOption.TopDirectoryOnly);

                foreach (string dumpDirectory in dumpDirectories)
                {
                    try
                    {
                        Directory.Delete(dumpDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting {dumpDirectory}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                MessageBox.Show($"{folderName} Logs clean!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClearServerLogs_Click(object sender, EventArgs e)
        {
            ClearLogs(tbServerDir.Text, "Server", [".log"]);
        }

        private void BtnClearAPILogs_Click(object sender, EventArgs e)
        {
            ClearLogs(tbAPIDir.Text, "API", [".log"]);
        }

        private const int SendKeysDelay = 300;
        private const string GameGatewayServerProcessName = "GameGatewayServer_Release_x64";

        private void BtnGateLogin_Click(object sender, EventArgs e)
        {
            if (IsProcessRunning(GameGatewayServerProcessName))
            {
                Process[] processes = Process.GetProcessesByName(GameGatewayServerProcessName);
                if (processes.Length > 0)
                {
                    Process process = processes[0];
                    bool isOpening = btnGateLogin.Text == "Open Login";
                    string buttonText = isOpening ? "Close Login" : "Open Login";

                    btnGateLogin.Text = buttonText;
                    SetForegroundWindow(process.MainWindowHandle);
                    SendKeysToGameGatewayServer("uol");
                }
                else
                {
                    MessageBox.Show(GameGatewayServerProcessName + " is not running!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                btnGateLogin.Enabled = false;
                btnGateLogin.Text = "Open Login";
            }
        }

        private static void SendKeysToGameGatewayServer(string keysToSend)
        {
            foreach (char key in keysToSend)
            {
                SendKeys.SendWait(key.ToString());
                Thread.Sleep(SendKeysDelay);
            }
        }

        private void CheckProcessStatus(object? sender, EventArgs e)
        {
            UpdateServerStatus();
            UpdateGameGatewayStatus();
        }

        private void UpdateServerStatus()
        {
            bool isAgentRunning = IsProcessRunning("Agent_Release_x64");
            tabControlServerSettings.Enabled = !isAgentRunning;
            btnStartServers.Enabled = !isAgentRunning;
            btnStopServers.Enabled = isAgentRunning;
            btnKillServers.Enabled = isAgentRunning;
            btnClearServerLogs.Enabled = !isAgentRunning;

            if (isAgentRunning)
            {
                btnStartServers.Text = "Running...";
                btnStopServers.Text = "Stop Servers";
                btnKillServers.Text = "Kill Servers";
            }
            else
            {
                btnStartServers.Text = "Start Servers";
                btnStopServers.Text = "Stop Servers";
                btnKillServers.Text = "Kill Servers";
            }
        }

        private void UpdateGameGatewayStatus()
        {
            bool isGameGatewayRunning = IsProcessRunning(GameGatewayServerProcessName);
            btnGateLogin.Enabled = isGameGatewayRunning;

            if (!isGameGatewayRunning)
            {
                btnGateLogin.Text = "Open Login";
            }
        }

        [System.Runtime.InteropServices.LibraryImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        #region Database Backup
        private async void BtnRestoreDB_Click(object sender, EventArgs e)
        {
            if (!ValidateDatabaseInfo()) return;

            FolderBrowserDialog folderBrowserDialog = new();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string backupFolderPath = folderBrowserDialog.SelectedPath;

                if (!ConfirmRestore()) return;

                try
                {
                    DisableDatabaseButtons("Restoring...");

                    using SqlConnection connection = new(GetConnectionString());
                    await connection.OpenAsync();

                    Dictionary<string, string> databaseBackupMap = new()
                    {
                        { "GMRustyHearts", "GMRustyHearts.bak" },
                        { "RustyHearts", "RustyHearts.bak" },
                        { "RustyHearts_Auth", "RustyHearts_Auth.bak" },
                        { "RustyHearts_Log", "RustyHearts_Log.bak" },
                        { "RustyHearts_Account", "RustyHearts_Account.bak" }
                    };

                    foreach (KeyValuePair<string, string> databaseBackupPair in databaseBackupMap)
                    {
                        string dbName = databaseBackupPair.Key;
                        string dbBackupFile = databaseBackupPair.Value;
                        string dbPath = Path.Combine(backupFolderPath, dbBackupFile);

                        if (!DatabaseExists(connection, dbName))
                        {
                            CreateDatabase(connection, dbName);
                        }

                        RestoreDatabase(connection, dbName, dbPath);
                    }

                    MessageBox.Show("Database restored successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlException ex)
                {
                    ShowDatabaseError("restoring backups", ex);
                }
                finally
                {
                    EnableDatabaseButtons();
                }
            }
        }

        private async void BtnBackupDB_Click(object sender, EventArgs e)
        {
            if (!ValidateDatabaseInfo()) return;

            string[] dbNames = ["GMRustyHearts", "RustyHearts", "RustyHearts_Auth", "RustyHearts_Log", "RustyHearts_Account"];
            FolderBrowserDialog folderBrowserDialog = new();

            if (!ConfirmBackup("backups of the databases", dbNames)) return;

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string backupFolderPath = Path.Combine(folderBrowserDialog.SelectedPath, "RustyHearts_Backup_" + DateTime.Now.ToString("yyyyMMddHHmmss"));

                try
                {
                    DisableDatabaseButtons("Backing up...");

                    Directory.CreateDirectory(backupFolderPath);

                    using SqlConnection connection = new(GetConnectionString());
                    await connection.OpenAsync();

                    foreach (string dbName in dbNames)
                    {
                        string backupFilePath = Path.Combine(backupFolderPath, dbName + ".bak");
                        BackupDatabase(connection, dbName, backupFilePath);
                    }

                    MessageBox.Show("Databases backed up successfully to " + backupFolderPath, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlException ex)
                {
                    ShowDatabaseError("creating database backups", ex);
                }
                finally
                {
                    EnableDatabaseButtons();
                }
            }
        }

        private bool ValidateDatabaseInfo()
        {
            dbServer = tbSQLAddress.Text;
            dbUser = tbSQLAccount.Text;
            dbPasswd = tbSQLPassword.Text;

            List<string> missingFields = [];

            if (string.IsNullOrEmpty(dbServer))
                missingFields.Add("SQL Server");

            if (string.IsNullOrEmpty(dbUser))
                missingFields.Add("SQL User");

            if (string.IsNullOrEmpty(dbPasswd))
                missingFields.Add("SQL Password");

            if (missingFields.Count > 0)
            {
                string missingFieldsMessage = "Please fill in all required fields (marked with *)\nMissing or incomplete fields: " + string.Join(", ", missingFields);
                MessageBox.Show(missingFieldsMessage, "Incomplete Database Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private static bool ConfirmRestore()
        {
            return DialogResult.Yes == MessageBox.Show("Are you sure you want to restore the databases? This action is permanent and cannot be undone.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        private string GetConnectionString()
        {
            return $"Data Source={dbServer};Initial Catalog=master;TrustServerCertificate=true;User Id={dbUser};Password={dbPasswd};";
        }

        private static bool DatabaseExists(SqlConnection connection, string dbName)
        {
            string checkDbExistsSql = "SELECT COUNT(*) FROM master.sys.databases WHERE name = @name";

            using SqlCommand checkDbExistsCommand = new(checkDbExistsSql, connection);
            checkDbExistsCommand.Parameters.AddWithValue("@name", dbName);
            int dbExists = (int)checkDbExistsCommand.ExecuteScalar();

            return dbExists > 0;
        }

        private static void CreateDatabase(SqlConnection connection, string dbName)
        {
            string createDbSql = $"CREATE DATABASE {dbName}";

            using SqlCommand createDbCommand = new(createDbSql, connection);
            createDbCommand.ExecuteNonQuery();
        }

        private static void RestoreDatabase(SqlConnection connection, string dbName, string dbPath)
        {
            string dataFilePathSql = $"SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('{dbName}') AND type_desc = 'ROWS'";
            string logFilePathSql = $"SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('{dbName}') AND type_desc = 'LOG'";

            using SqlCommand getDataFilePathCommand = new(dataFilePathSql, connection);
            string dataFilePath = (string)getDataFilePathCommand.ExecuteScalar();

            using SqlCommand getLogFilePathCommand = new(logFilePathSql, connection);
            string logFilePath = (string)getLogFilePathCommand.ExecuteScalar();

            string sqlRestore = $"RESTORE DATABASE {dbName} FROM DISK='{dbPath}' WITH MOVE '{dbName}' TO '{dataFilePath}', MOVE '{dbName}_Log' TO '{logFilePath}', REPLACE";

            using SqlCommand cmdRestore = new(sqlRestore, connection);
            cmdRestore.CommandTimeout = 60 * 15;
            cmdRestore.ExecuteNonQuery();
        }

        private static bool ConfirmBackup(string action, string[] dbNames)
        {
            string databaseList = string.Join(", ", dbNames);
            return DialogResult.Yes == MessageBox.Show($"Create {action}: {databaseList}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private static void BackupDatabase(SqlConnection connection, string dbName, string backupFilePath)
        {
            string backupSQL = $"BACKUP DATABASE {dbName} TO DISK='{backupFilePath}'";

            using SqlCommand backupCommand = new(backupSQL, connection);
            backupCommand.ExecuteNonQuery();
        }

        private void DisableDatabaseButtons(string text)
        {
            btnUpdateDB.Enabled = false;
            btnRestoreDB.Enabled = false;
            btnBackupDB.Enabled = false;
            btnRestoreDB.Text = text;
        }

        private void EnableDatabaseButtons()
        {
            btnUpdateDB.Enabled = true;
            btnRestoreDB.Enabled = true;
            btnBackupDB.Enabled = true;
            btnRestoreDB.Text = "Restore Database";
        }

        private static void ShowDatabaseError(string action, SqlException ex)
        {
            MessageBox.Show($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #endregion

        #region XML
        private XmlDocument contentXmlDoc = new();
        private XmlDocument logXmlDoc = new();
        private XmlDocument serviceXmlDoc = new();


        #region Load XML
        private XmlDocument LoadXml(string filePath, TabPage tab, XmlDocument xmlDocument)
        {
            try
            {
                if (string.IsNullOrEmpty(dirServer))
                {
                    tab.Enabled = false;
                    gbSaveSettings.Enabled = false;
                    gbServerControls.Enabled = false;
                    btnOpenServerDir.Enabled = false;
                    throw new Exception("Server directory is empty or null.");
                }

                // Clear the controls on the tab
                ClearControls(tab);

                // Load the XML file
                xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);

                // Enable the tab and the group box
                tab.Enabled = true;
                gbSaveSettings.Enabled = true;
                gbServerControls.Enabled = true;

                return xmlDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading {Path.GetFileName(filePath)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tab.Enabled = false;
                throw new Exception("Error loading XML document.", ex);
            }
        }

        private void LoadServerConfig()
        {
            try
            {
                dirServer = tbServerDir.Text;

                if (string.IsNullOrEmpty(dirServer))
                {
                    gbServerControls.Enabled = false;
                    gbSaveSettings.Enabled = false;
                    tabService.Enabled = false;
                    ClearControls(tabService);
                    tabContentControl.Enabled = false;
                    ClearControls(tabContentControl);
                    btnOpenServerDir.Enabled = false;
                }
                else
                {
                    string serverFilePath = Path.Combine(dirServer, "Agent_Release_x64.exe");

                    if (!Directory.Exists(dirServer) || !File.Exists(serverFilePath))
                    {
                        MessageBox.Show("Invalid Server folder. Please select the server folder with the server executables.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tbServerDir.Text = "";
                        gbServerControls.Enabled = false;
                        btnOpenServerDir.Enabled = false;
                    }
                    else
                    {
                        LoadContentXml();
                        LoadServiceXml();
                        LoadLogXml();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading server settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContentXml()
        {
            try
            {
                dirServer = tbServerDir.Text;
                string contentFilePath = Path.Combine(dirServer, "Option", "content_control.xml");
                contentXmlDoc = LoadXml(contentFilePath, tabContentControl, contentXmlDoc);

                // Populate the controls with the values from the XML file
                LoadClassSettings();
                LoadGeneralSettings();
                LoadGuildSettings();
                LoadGuildContentSettings();
                LoadShopSettings();
                LoadDungeonSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading content settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLogXml()
        {
            try
            {
                dirServer = tbServerDir.Text;
                string logFilePath = Path.Combine(dirServer, "Option", "log_control.xml");
                logXmlDoc = LoadXml(logFilePath, tabLog, logXmlDoc);

                // Populate the controls with the values from the XML file
                LoadLogSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading log settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadServiceXml()
        {
            try
            {
                dirServer = tbServerDir.Text;
                string serviceFilePath = Path.Combine(dirServer, "Option", "service_control.xml");
                serviceXmlDoc = LoadXml(serviceFilePath, tabService, serviceXmlDoc);

                // Populate the controls with the values from the XML file
                LoadServiceSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading service settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Save XML

        private void BtnSaveContent_Click(object sender, EventArgs e)
        {
            string contentFilePath = GetOptionFilePath("content_control.xml");
            string logFilePath = GetOptionFilePath("log_control.xml");

            if (!Directory.Exists(Path.GetDirectoryName(contentFilePath)))
            {
                MessageBox.Show("Invalid server directory.\nPlease select the server root folder with the Option folder.", "XML file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ValidateFileExists(contentFilePath, "content_control.xml") && ValidateFileExists(logFilePath, "log_control.xml"))
            {
                SaveXmlDocument(contentXmlDoc, contentFilePath, SaveSettings);
                SaveXmlDocument(logXmlDoc, logFilePath, SaveLogSettings);
            }
        }

        private void SaveSettings()
        {
            SaveClassSettings();
            SaveGeneralSettings();
            SaveGuildSettings();
            SaveGuildContentSettings();
            SaveShopSettings();
            SaveCurrentDungeonSettings();
            SaveDungeonSettings();
            SaveLogSettings();
        }

        private string GetOptionFilePath(string fileName)
        {
            dirServer = tbServerDir.Text;
            string optionDirectory = Path.Combine(dirServer, "Option");
            return Path.Combine(optionDirectory, fileName);
        }

        private static bool ValidateFileExists(string filePath, string errorMessage)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"An error occurred while saving settings:\nFile {filePath} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private static void SaveXmlDocument(XmlDocument? xmlDocument, string filePath, Action saveSettings)
        {
            try
            {
                saveSettings();
                xmlDocument?.Save(filePath);
                MessageBox.Show($"{filePath} settings saved successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Service XML

        private List<Dictionary<string, string>> serviceSettingsList = [];
        private Dictionary<string, Dictionary<string, string>> serviceComboBoxMap = [];
        private Dictionary<string, string> currentServiceSettings = [];

        private void LoadServiceSettings()
        {
            if (serviceXmlDoc == null)
            {
                return;
            }

            XmlNodeList? serviceNodes = serviceXmlDoc.SelectNodes("//service/area");

            serviceSettingsList.Clear();
            serviceComboBoxMap.Clear();

            if (serviceNodes == null || serviceNodes.Count == 0)
            {
                ResetServiceFields();
            }
            else
            {
                PopulateServiceSettings(serviceNodes);
            }
        }

        private void ResetServiceFields()
        {
            currentServiceSettings.Clear();
            cmbActiveArea.SelectedIndex = -1;
            cmbServerRegion.SelectedIndex = -1;
            cmbServerMode.SelectedIndex = -1;
            tbAuthUrl.Text = "";
            tbBillingUrl.Text = "";
            cbSkipAuth.Checked = false;
            cbSkipBilling.Checked = false;
            cbFreeZen.Checked = false;
            cbSkipNick.Checked = false;
            cbSecondPwd.Checked = false;
            cbBetazone.Checked = false;
            numChannelLimit.Value = 0;
            numUserLimit.Value = 0;
            numBillingIdc.Value = 0;
            EnableServiceFields(false);
        }

        private void PopulateServiceSettings(XmlNodeList serviceNodes)
        {
            foreach (XmlNode serviceNode in serviceNodes)
            {
                Dictionary<string, string> serviceSettings = [];

                if (serviceNode.Attributes != null)
                {
                    foreach (XmlAttribute attribute in serviceNode.Attributes)
                    {
                        serviceSettings[attribute.Name] = attribute.Value;
                    }

                    serviceSettingsList.Add(serviceSettings);
                    serviceComboBoxMap[serviceSettings["country"]] = serviceSettings;
                }
            }

            PopulateComboBoxes();

            if (cmbServerRegion.Items.Count > 0)
            {
                ConfigureServerMode();
                ConfigureActiveArea();
            }
        }

        private void PopulateComboBoxes()
        {
            cmbServerRegion.Items.AddRange(serviceSettingsList.Select(d => d["country"]).ToArray());
            cmbActiveArea.Items.AddRange(serviceSettingsList.Select(d => d["country"]).ToArray());
        }

        private void ConfigureServerMode()
        {
            cmbServerMode.Items.Add("WAG");
            cmbServerMode.Items.Add("DEV");
            cmbServerRegion.SelectedIndex = 0;
        }

        private void ConfigureActiveArea()
        {
            XmlNode? activeAreaNode = serviceXmlDoc.SelectSingleNode("//service/active_area");

            if (activeAreaNode != null && activeAreaNode.Attributes != null)
            {
                XmlAttribute? countryAttribute = activeAreaNode.Attributes["country"];
                if (countryAttribute != null)
                {
                    string activeAreaCountry = countryAttribute.Value;
                    int activeAreaIndex = cmbActiveArea.Items.IndexOf(activeAreaCountry);
                    if (activeAreaIndex >= 0)
                    {
                        cmbActiveArea.SelectedIndex = activeAreaIndex;
                    }
                }
            }
        }

        private void CmbServerRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveCurrentServiceSettings();

            int selectedServiceIndex = cmbServerRegion.SelectedIndex;

            if (selectedServiceIndex >= 0 && selectedServiceIndex < serviceSettingsList.Count)
            {
                currentServiceSettings = serviceSettingsList[selectedServiceIndex];
                UpdateServiceFields(currentServiceSettings);
            }
        }

        private void UpdateServiceFields(Dictionary<string, string> serviceSettings)
        {
            tbAuthUrl.Text = GetServiceSettingOrDefault(serviceSettings, "auth_url", "");
            tbBillingUrl.Text = GetServiceSettingOrDefault(serviceSettings, "billing_url", "");
            cmbServerMode.Text = GetServiceSettingOrDefault(serviceSettings, "server_mode", "");
            numChannelLimit.Text = GetServiceSettingOrDefault(serviceSettings, "channel_limit_count", "1");
            numUserLimit.Text = GetServiceSettingOrDefault(serviceSettings, "world_user_limit_count", "10");
            numBillingIdc.Text = GetServiceSettingOrDefault(serviceSettings, "billing_idc", "10101");
            cmbServerMode.Text = GetServiceSettingOrDefault(serviceSettings, "server_mode", "");
            cmbServerMode.Text = GetServiceSettingOrDefault(serviceSettings, "server_mode", "");
            cbSkipAuth.Checked = GetServiceSettingOrDefault(serviceSettings, "skip_auth", "0") == "1";
            cbSkipBilling.Checked = GetServiceSettingOrDefault(serviceSettings, "skip_billing", "0") == "1";
            cbFreeZen.Checked = GetServiceSettingOrDefault(serviceSettings, "free_cash", "0") == "1";
            cbSkipNick.Checked = GetServiceSettingOrDefault(serviceSettings, "skip_abuse_nick", "0") == "1";
            cbSecondPwd.Checked = GetServiceSettingOrDefault(serviceSettings, "second_pass", "0") == "1";
            cbBetazone.Checked = GetServiceSettingOrDefault(serviceSettings, "betazone", "0") == "1";
        }

        private static string GetServiceSettingOrDefault(Dictionary<string, string> serviceSettings, string key, string defaultValue)
        {
            return serviceSettings.TryGetValue(key, out string? value) ? value : defaultValue;
        }

        private void SaveCurrentServiceSettings()
        {
            if (currentServiceSettings != null)
            {
                currentServiceSettings["auth_url"] = tbAuthUrl.Text;
                currentServiceSettings["billing_url"] = tbBillingUrl.Text;
                currentServiceSettings["server_mode"] = cmbServerMode.Text;
                currentServiceSettings["channel_limit_count"] = numChannelLimit.Text;
                currentServiceSettings["world_user_limit_count"] = numUserLimit.Text;
                currentServiceSettings["billing_idc"] = numBillingIdc.Text;
                currentServiceSettings["skip_auth"] = cbSkipAuth.Checked ? "1" : "0";
                currentServiceSettings["skip_billing"] = cbSkipBilling.Checked ? "1" : "0";
                currentServiceSettings["free_cash"] = cbFreeZen.Checked ? "1" : "0";
                currentServiceSettings["skip_abuse_nick"] = cbSkipNick.Checked ? "1" : "0";
                currentServiceSettings["second_pass"] = cbSecondPwd.Checked ? "1" : "0";
                currentServiceSettings["betazone"] = cbBetazone.Checked ? "1" : "0";
            }
        }

        private void EnableServiceFields(bool enable)
        {
            cmbActiveArea.Enabled = enable;
            cmbServerRegion.Enabled = enable;
            cmbServerMode.Enabled = enable;
            tbAuthUrl.Enabled = enable;
            btnEditService.Enabled = enable;
            btnRemoveService.Enabled = enable;
            tbBillingUrl.Enabled = enable;
            cbSkipAuth.Enabled = enable;
            cbSkipBilling.Enabled = enable;
            cbFreeZen.Enabled = enable;
            cbSkipNick.Enabled = enable;
            cbSecondPwd.Enabled = enable;
            cbBetazone.Enabled = enable;
            numChannelLimit.Enabled = enable;
            numUserLimit.Enabled = enable;
            numBillingIdc.Enabled = enable;
        }

        private void SaveServiceSettings()
        {
            if (serviceXmlDoc == null)
            {
                return;
            }

            XmlNode? activeAreaNode = serviceXmlDoc.SelectSingleNode("//service/active_area");

            if (activeAreaNode != null)
            {
                XmlAttribute? activeAreaCountryAttribute = activeAreaNode.Attributes?["country"];

                if (activeAreaCountryAttribute != null)
                {
                    string selectedActiveArea = cmbActiveArea.SelectedItem?.ToString() ?? string.Empty;
                    activeAreaCountryAttribute.Value = selectedActiveArea;
                }
                else
                {
                    CreateCountryAttribute(activeAreaNode);
                }
            }
            else
            {
                CreateActiveAreaNode();
            }

            RemoveExistingServiceNodes();

            foreach (Dictionary<string, string> serviceSettings in serviceSettingsList)
            {
                XmlElement newServiceNode = CreateServiceNode(serviceSettings);
                serviceXmlDoc.DocumentElement?.AppendChild(newServiceNode);
            }
        }

        private void CreateCountryAttribute(XmlNode activeAreaNode)
        {
            if (serviceXmlDoc != null)
            {
                XmlAttribute countryAttribute = serviceXmlDoc.CreateAttribute("country");
                string selectedActiveArea = cmbActiveArea.SelectedItem?.ToString() ?? string.Empty;
                countryAttribute.Value = selectedActiveArea;

                activeAreaNode.Attributes?.Append(countryAttribute);
            }
        }

        private void CreateActiveAreaNode()
        {
            if (serviceXmlDoc != null)
            {
                XmlNode activeAreaNode = serviceXmlDoc.CreateElement("active_area");
                serviceXmlDoc.SelectSingleNode("//service")?.AppendChild(activeAreaNode);

                CreateCountryAttribute(activeAreaNode);
            }
        }

        private void RemoveExistingServiceNodes()
        {
            XmlNodeList? serviceNodes = serviceXmlDoc.SelectNodes("//service/area");

            if (serviceNodes != null)
            {
                foreach (XmlNode serviceNode in serviceNodes)
                {
                    serviceXmlDoc.DocumentElement?.RemoveChild(serviceNode);
                }
            }
        }

        private XmlElement CreateServiceNode(Dictionary<string, string> serviceSettings)
        {
            XmlElement newServiceNode = serviceXmlDoc.CreateElement("area");

            foreach (KeyValuePair<string, string> kvp in serviceSettings)
            {
                newServiceNode.SetAttribute(kvp.Key, kvp.Value);
            }

            return newServiceNode;
        }

        private void BtnSaveService_Click(object sender, EventArgs e)
        {
            string serviceFilePath = GetOptionFilePath("service_control.xml");

            if (!Directory.Exists(Path.GetDirectoryName(serviceFilePath)))
            {
                MessageBox.Show("Invalid server directory.\nPlease select the server root folder with the Option folder.", "XML file not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ValidateFileExists(serviceFilePath, "service_control.xml"))
            {
                SaveXmlDocument(serviceXmlDoc, serviceFilePath, SaveService);
            }
        }

        private void SaveService()
        {
            SaveCurrentServiceSettings();
            SaveServiceSettings();
        }

        public bool ServiceExists(string country)
        {
            return serviceSettingsList.Any(s => s != null && s.ContainsKey("country") && s["country"] == country);
        }

        private void BtnNewService_Click(object sender, EventArgs e)
        {
            ServiceForm serviceForm = new(this);
            DialogResult result = serviceForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                Dictionary<string, string>? newServiceSettings = serviceForm.GetServiceSettings();

                if (newServiceSettings != null && newServiceSettings.TryGetValue("country", out string? value))
                {
                    serviceSettingsList.Add(newServiceSettings);

                    string newServiceName = value;
                    cmbActiveArea.Items.Add(newServiceName);
                    cmbServerRegion.Items.Add(newServiceName);
                    cmbServerMode.Items.Add("WAG");
                    cmbServerMode.Items.Add("DEV");

                    if (cmbServerRegion.Items.Count == 1)
                    {
                        // If the list was empty, set the index to the first item.
                        cmbServerRegion.SelectedIndex = 0;
                        cmbActiveArea.SelectedIndex = 0;
                    }
                    else
                    {
                        // Set the index to the last item.
                        cmbServerRegion.SelectedIndex = cmbServerRegion.Items.Count - 1;
                    }

                    EnableServiceFields(true);
                }
            }
        }

        private void BtnEditService_Click(object sender, EventArgs e)
        {
            if (cmbServerRegion.SelectedItem != null)
            {
                int selectedIndex = cmbServerRegion.SelectedIndex;
                Dictionary<string, string> selectedServiceSettings = serviceSettingsList[selectedIndex];

                ServiceForm serviceForm = new(selectedServiceSettings, true);
                DialogResult result = serviceForm.ShowDialog();

                if (serviceForm != null && result == DialogResult.OK)
                {
                    serviceSettingsList[selectedIndex] = serviceForm.GetServiceSettings();
                    cmbServerRegion.Items[selectedIndex] = serviceForm.GetServiceName();

                    if (currentServiceSettings == selectedServiceSettings)
                    {
                        currentServiceSettings = serviceForm.GetServiceSettings();
                        UpdateServiceFields(currentServiceSettings);
                    }
                }
            }
        }

        private void BtnRemoveService_Click(object sender, EventArgs e)
        {
            if (currentServiceSettings != null && cmbServerRegion.SelectedItem != null)
            {
                int selectedServiceIndex = cmbServerRegion.SelectedIndex;

                UpdateCurrentServiceAfterRemoval(selectedServiceIndex);

                serviceSettingsList.RemoveAt(selectedServiceIndex);
                cmbServerRegion.Items.RemoveAt(selectedServiceIndex);
                cmbActiveArea.Items.RemoveAt(selectedServiceIndex);

                if (cmbServerRegion.Items.Count == 0)
                {
                    ResetServiceFields();
                }
            }
        }

        private void UpdateCurrentServiceAfterRemoval(int selectedServiceIndex)
        {
            if (selectedServiceIndex >= 0 && selectedServiceIndex < cmbServerRegion.Items.Count)
            {
                if (selectedServiceIndex > 0)
                {
                    cmbServerRegion.SelectedIndex = selectedServiceIndex - 1;
                }
                else
                {
                    cmbServerRegion.SelectedIndex = 0;
                    cmbActiveArea.SelectedIndex = 0;
                }

                currentServiceSettings = serviceSettingsList[cmbServerRegion.SelectedIndex];
                UpdateServiceFields(currentServiceSettings);
            }
        }
        #endregion

        #region Log XML

        private Dictionary<string, XmlNode> logTypeNodes = [];

        private void LoadLogSettings()
        {
            XmlNodeList? logTypeNodesList = logXmlDoc.SelectNodes("//log/log_type");

            cmbLog.Items.Clear();
            logTypeNodes.Clear();

            if (logTypeNodesList != null)
            {
                foreach (XmlNode logTypeNode in logTypeNodesList)
                {
                    string? logName = logTypeNode.Attributes?["name"]?.Value;

                    if (logName != null)
                    {
                        cmbLog.Items.Add(logName);
                        logTypeNodes[logName] = logTypeNode;
                    }
                }
            }

            if (cmbLog.Items.Count > 0)
            {
                cmbLog.SelectedIndex = 0;
            }
        }

        private void CmbLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedLogName = cmbLog.SelectedItem?.ToString();

            if (selectedLogName != null && logTypeNodes.TryGetValue(selectedLogName, out XmlNode? selectedLogTypeNode))
            {
                UpdateLogTypeNodeUI(selectedLogTypeNode);
            }
        }

        private void UpdateLogTypeNodeUI(XmlNode logTypeNode)
        {
            if (logTypeNode != null)
            {
                int logID = GetLogAttributeAsInt(logTypeNode, "id", 0);
                bool logUse = GetLogAttributeAsInt(logTypeNode, "use", 0) != 0;

                LogIDTextBox.Text = logID.ToString();
                cbLogUse.Checked = logUse;
            }
        }

        private static int GetLogAttributeAsInt(XmlNode logTypeNode, string attributeName, int defaultValue)
        {
            string? attributeValue = logTypeNode.Attributes?[attributeName]?.Value;
            return int.TryParse(attributeValue, out int result) ? result : defaultValue;
        }

        private void CbLogUse_CheckedChanged(object sender, EventArgs e)
        {
            string? selectedLogName = cmbLog.SelectedItem?.ToString();

            if (selectedLogName != null && logTypeNodes.TryGetValue(selectedLogName, out XmlNode? selectedLogTypeNode))
            {
                UpdateLogTypeNodeAttribute(selectedLogTypeNode, "use", cbLogUse.Checked ? "1" : "0");
            }
        }

        private void SaveLogSettings()
        {
            string? selectedLogName = cmbLog.SelectedItem?.ToString();

            if (selectedLogName != null && logTypeNodes.TryGetValue(selectedLogName, out XmlNode? selectedLogTypeNode))
            {
                UpdateLogTypeNodeAttribute(selectedLogTypeNode, "id", LogIDTextBox.Text);
                UpdateLogTypeNodeAttribute(selectedLogTypeNode, "use", cbLogUse.Checked ? "1" : "0");
            }
        }

        private static void UpdateLogTypeNodeAttribute(XmlNode logTypeNode, string attributeName, string newValue)
        {
            XmlAttribute? attribute = logTypeNode.Attributes?[attributeName];

            if (attribute != null)
            {
                attribute.Value = newValue;
            }
        }

        #endregion

        #region Content XML

        #region Class Node

        private Dictionary<string, XmlNode> classNodes = [];

        private void LoadClassSettings()
        {
            XmlNode? avatarNode = contentXmlDoc.SelectSingleNode("//avatar");
            if (avatarNode != null)
            {
                int onlyCreate = GetAttributeValueAsInt(avatarNode, "only_create", 0);
                cbOnlyCreate.Checked = (onlyCreate == 1);
            }

            XmlNodeList? classNodeList = contentXmlDoc.SelectNodes("//class");
            classNodes = [];

            cmbClass.Items.Clear();

            if (classNodeList != null)
            {
                foreach (XmlNode classNode in classNodeList)
                {
                    string className = GetAttributeValue(classNode, "name");

                    if (!string.IsNullOrEmpty(className))
                    {
                        cmbClass.Items.Add(className);
                        classNodes[className] = classNode;
                    }
                }
            }

            if (cmbClass.Items.Count > 0)
            {
                cmbClass.SelectedIndex = 0;
            }
        }

        private void CmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedClassName = cmbClass.SelectedItem?.ToString();

            if (selectedClassName != null && classNodes.TryGetValue(selectedClassName, out XmlNode? value))
            {
                XmlNode classNode = value;

                int classID = GetAttributeValueAsInt(classNode, "id", 0);

                tbClassID.Text = classID.ToString();
                cbClassUse.Checked = GetAttributeValueAsBool(classNode, "use");
                cbClassFreeUse.Checked = GetAttributeValueAsBool(classNode, "freeuse");
            }
            else
            {
                tbClassID.Text = string.Empty;
                cbClassUse.Checked = false;
                cbClassFreeUse.Checked = false;
            }
        }

        private int GetAttributeValueAsInt(XmlNode node, string attributeName, int defaultValue)
        {
            string attributeValue = GetAttributeValue(node, attributeName);
            return int.TryParse(attributeValue, out int result) ? result : defaultValue;
        }

        private bool GetAttributeValueAsBool(XmlNode node, string attributeName)
        {
            string attributeValue = GetAttributeValue(node, attributeName);
            return attributeValue == "1";
        }

        private static string GetAttributeValue(XmlNode node, string attributeName)
        {
            var attribute = node.Attributes?[attributeName];

            return attribute != null ? attribute.Value : string.Empty;
        }


        private void CbClassUse_CheckedChanged(object sender, EventArgs e)
        {
            string? selectedClassName = cmbClass.SelectedItem?.ToString();

            if (selectedClassName != null && classNodes.TryGetValue(selectedClassName, out XmlNode? value))
            {
                XmlNode classNode = value;
                SetAttributeValue(classNode, "use", cbClassUse.Checked ? "1" : "0");
            }
        }

        private void CbClassFreeUse_CheckedChanged(object sender, EventArgs e)
        {
            string? selectedClassName = cmbClass.SelectedItem?.ToString();

            if (selectedClassName != null && classNodes.TryGetValue(selectedClassName, out XmlNode? value))
            {
                XmlNode classNode = value;
                SetAttributeValue(classNode, "freeuse", cbClassFreeUse.Checked ? "1" : "0");
            }
        }

        private void SetAttributeValue(XmlNode node, string attributeName, string newValue)
        {
            XmlAttribute? attribute = node.Attributes?[attributeName];

            if (attribute == null)
            {
                attribute = contentXmlDoc.CreateAttribute(attributeName);
                node.Attributes?.Append(attribute);
            }

            attribute.Value = newValue;
        }

        private void SaveClassSettings()
        {
            XmlElement? documentElement = contentXmlDoc.DocumentElement;

            XmlNode? avatarNode = contentXmlDoc.SelectSingleNode("//avatar");
            if (avatarNode == null)
            {
                avatarNode = contentXmlDoc.CreateElement("avatar");
                documentElement?.AppendChild(avatarNode);
            }

            SetAttributeValue(avatarNode, "only_create", cbOnlyCreate.Checked ? "1" : "0");

            string? selectedClassName = cmbClass.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedClassName) && classNodes.TryGetValue(selectedClassName, out XmlNode? classNode))
            {
                SetClassNodeAttributes(classNode);
            }
            else if (!string.IsNullOrEmpty(selectedClassName))
            {
                XmlNode newClassNode = contentXmlDoc.CreateElement("class");
                XmlAttribute nameAttribute = contentXmlDoc.CreateAttribute("name");
                nameAttribute.Value = selectedClassName;
                newClassNode.Attributes?.Append(nameAttribute);
                documentElement?.AppendChild(newClassNode);

                SetClassNodeAttributes(newClassNode);

                classNodes[selectedClassName] = newClassNode;
            }
        }

        private void SetClassNodeAttributes(XmlNode classNode)
        {
            SetAttributeValue(classNode, "id", tbClassID.Text);
            SetAttributeValue(classNode, "use", cbClassUse.Checked ? "1" : "0");
            SetAttributeValue(classNode, "freeuse", cbClassFreeUse.Checked ? "1" : "0");
        }

        #endregion

        #region General Nodes

        private void LoadGeneralSettings()
        {
            LoadCheckBoxValue("play_point", playPointCheckBox);
            LoadCheckBoxValue("cash_shop", cashShopCheckBox);
            LoadCheckBoxValue("cash_shop", cashShopViewCheckBox, "all_view");
            LoadCheckBoxValue("pet_inven", petInvenCheckBox);
            LoadCheckBoxValue("bloodmode", bloodmodeCheckBox);
            LoadCheckBoxValue("inquiry", inquiryCheckBox);
            LoadCheckBoxValue("itemrune_mix", itemruneMixCheckBox);
            LoadCheckBoxValue("fortune", fortuneCheckBox);
            LoadCheckBoxValue("pcroom", pcroomCheckBox);
            LoadCheckBoxValue("shut_down", cbShutdown);
            LoadCheckBoxValue("coupon", couponCheckBox);
            LoadCheckBoxValue("book_pictorial", bookPictorialCheckBox);
            LoadCheckBoxValue("party_mission", partyMissionCheckBox);
            LoadCheckBoxValue("hidden_dungeon", hiddenDungeonCheckBox);
            LoadCheckBoxValue("lobby_run", cbLobbyRun);
            LoadCheckBoxValue("party_summon", cbPartySummon);
            LoadCheckBoxValue("trade_costume", cbTradeCostume);
            LoadCheckBoxValue("quest_btn", cbQuestBtn);
            LoadCheckBoxValue("quest_btn", cbQuestBtnAccept, "accept");
            LoadCheckBoxValue("quest_btn", cbQuestBtnReject, "reject");
            LoadCheckBoxValue("pvp_ladder", cbPvpLadder);
            LoadCheckBoxValue("pvp_massive", cbPvpMassive);
            LoadCheckBoxValue("pvp_raid", cbPvpRaid);
            LoadCheckBoxValue("pvp_ladder_pingcheck", cbPvpPing);
            LoadTextNodeValue("event", "item_drop", tbItemDrop);
            LoadNumNodeValue("level", "value", numLevelCap);
            LoadNumNodeValue("shut_down", "time", numShutdownTime);
            LoadPartyUserNodeValues();
            LoadSmartServerNodeValues();
        }

        private void LoadCheckBoxValue(string nodeName, CheckBox checkBox, string attributeName = "use")
        {
            XmlNode? node = contentXmlDoc.SelectSingleNode($"//{nodeName}");
            if (node != null)
            {
                XmlAttribute? attribute = node.Attributes?[attributeName];
                if (attribute != null)
                {
                    bool value = Convert.ToInt32(attribute.Value) == 1;
                    checkBox.Checked = value;
                }
            }
        }

        private void LoadTextNodeValue(string nodeName, string attributeName, TextBox textBox)
        {
            XmlNode? node = contentXmlDoc.SelectSingleNode($"//{nodeName}");
            if (node != null)
            {
                XmlAttribute? attribute = node.Attributes?[attributeName];
                if (attribute != null)
                {
                    textBox.Text = attribute.Value;
                }
            }
        }

        private void LoadNumNodeValue(string nodeName, string attributeName, NumericUpDown numericUpDown)
        {
            XmlNode? node = contentXmlDoc.SelectSingleNode($"//{nodeName}");
            if (node != null)
            {
                XmlAttribute? attribute = node.Attributes?[attributeName];
                if (attribute != null)
                {
                    numericUpDown.Text = attribute.Value;
                }
            }
        }

        private static void LoadNumNodeValue(XmlNode nodeName, string attributeName, NumericUpDown numericUpDown)
        {
            if (nodeName != null)
            {
                if (nodeName.Attributes?.GetNamedItem(attributeName) is XmlAttribute attributeNode)
                {
                    if (decimal.TryParse(attributeNode.Value, out decimal parsedValue))
                    {
                        numericUpDown.Value = parsedValue;
                    }
                    else
                    {
                        numericUpDown.Value = 0;
                    }
                }
            }
        }

        private void LoadPartyUserNodeValues()
        {
            XmlNode? partyUserNode = contentXmlDoc.SelectSingleNode("//party_user");
            if (partyUserNode != null)
            {
                LoadNumNodeValue(partyUserNode, "solo_max", numSoloMax);
                LoadNumNodeValue(partyUserNode, "tutorial_max", numTutorialMax);
                LoadNumNodeValue(partyUserNode, "normal_max", numNormalMax);
                LoadNumNodeValue(partyUserNode, "matching_max", numMatchingMax);
                LoadNumNodeValue(partyUserNode, "raid_max", numRaidMax);
                LoadNumNodeValue(partyUserNode, "default_max", numDefaultMax);
            }
        }

        private void LoadSmartServerNodeValues()
        {
            XmlNode? smartServerNode = contentXmlDoc.SelectSingleNode("//smart_server");
            if (smartServerNode != null)
            {
                LoadNumNodeValue(smartServerNode, "basis_count", numBasisCount);
                LoadNumNodeValue(smartServerNode, "server_lockdown", numServerLockdown);
                LoadNumNodeValue(smartServerNode, "channel_add", numChannelAdd);
                LoadNumNodeValue(smartServerNode, "channel_lockdown", numChannelLockdown);
                LoadNumNodeValue(smartServerNode, "max_server_channel", numMaxServerChannel);
            }
        }

        private void SaveGeneralSettings()
        {
            SaveCheckBoxValue("play_point", playPointCheckBox);
            SaveCheckBoxValue("cash_shop", cashShopCheckBox);
            SaveCheckBoxValue("cash_shop", cashShopViewCheckBox, "all_view");
            SaveCheckBoxValue("pet_inven", petInvenCheckBox);
            SaveCheckBoxValue("bloodmode", bloodmodeCheckBox);
            SaveCheckBoxValue("inquiry", inquiryCheckBox);
            SaveCheckBoxValue("itemrune_mix", itemruneMixCheckBox);
            SaveCheckBoxValue("fortune", fortuneCheckBox);
            SaveCheckBoxValue("pcroom", pcroomCheckBox);
            SaveCheckBoxValue("shut_down", cbShutdown);
            SaveCheckBoxValue("coupon", couponCheckBox);
            SaveCheckBoxValue("book_pictorial", bookPictorialCheckBox);
            SaveCheckBoxValue("party_mission", partyMissionCheckBox);
            SaveCheckBoxValue("hidden_dungeon", hiddenDungeonCheckBox);
            SaveCheckBoxValue("lobby_run", cbLobbyRun);
            SaveCheckBoxValue("party_summon", cbPartySummon);
            SaveCheckBoxValue("trade_costume", cbTradeCostume);
            SaveCheckBoxValue("quest_btn", cbQuestBtn);
            SaveCheckBoxValue("quest_btn", cbQuestBtnAccept, "accept");
            SaveCheckBoxValue("quest_btn", cbQuestBtnReject, "reject");
            SaveCheckBoxValue("pvp_ladder", cbPvpLadder);
            SaveCheckBoxValue("pvp_massive", cbPvpMassive);
            SaveCheckBoxValue("pvp_raid", cbPvpRaid);
            SaveCheckBoxValue("pvp_ladder_pingcheck", cbPvpPing);
            SaveNumNodeValue("level", "value", numLevelCap);
            SaveNumNodeValue("shut_down", "time", numShutdownTime);
            SaveTextNodeValue("event", "item_drop", tbItemDrop);
            SavePartyUserNodeValues();
            SaveSmartServerNodeValues();
        }

        private XmlNode GetOrCreateNode(string nodeName)
        {
            XmlNode? node = contentXmlDoc.SelectSingleNode($"//{nodeName}");
            if (node == null)
            {
                node = contentXmlDoc.CreateElement(nodeName);
                contentXmlDoc.DocumentElement?.AppendChild(node);
            }
            return node;
        }

        private void SaveCheckBoxValue(string nodeName, CheckBox checkBox, string attributeName = "use")
        {
            XmlNode node = GetOrCreateNode(nodeName);
            XmlAttribute? attribute = node.Attributes?[attributeName];
            if (attribute != null)
            {
                attribute.Value = checkBox.Checked ? "1" : "0";
            }
        }

        private void SaveTextNodeValue(string nodeName, string attributeName, TextBox textBox)
        {
            XmlNode node = GetOrCreateNode(nodeName);
            XmlAttribute? attribute = node.Attributes?[attributeName];
            if (attribute != null)
            {
                attribute.Value = textBox.Text;
            }
        }

        private void SaveNumNodeValue(string nodeName, string attributeName, NumericUpDown numericUpDown)
        {
            XmlNode node = GetOrCreateNode(nodeName);
            XmlAttribute? attribute = node.Attributes?[attributeName];
            if (attribute != null)
            {
                attribute.Value = numericUpDown.Text;
            }
        }

        private static void SaveNumNodeValue(XmlNode node, string attributeName, NumericUpDown numericUpDown)
        {
            XmlAttribute? attribute = node.Attributes?[attributeName];
            if (attribute != null)
            {
                attribute.Value = numericUpDown.Text;
            }
        }

        private void SavePartyUserNodeValues()
        {
            XmlNode node = GetOrCreateNode("party_user");
            SaveNumNodeValue(node, "solo_max", numSoloMax);
            SaveNumNodeValue(node, "tutorial_max", numTutorialMax);
            SaveNumNodeValue(node, "normal_max", numNormalMax);
            SaveNumNodeValue(node, "matching_max", numMatchingMax);
            SaveNumNodeValue(node, "raid_max", numRaidMax);
            SaveNumNodeValue(node, "default_max", numDefaultMax);
        }

        private void SaveSmartServerNodeValues()
        {
            XmlNode node = GetOrCreateNode("smart_server");
            SaveNumNodeValue(node, "basis_count", numBasisCount);
            SaveNumNodeValue(node, "server_lockdown", numServerLockdown);
            SaveNumNodeValue(node, "channel_add", numChannelAdd);
            SaveNumNodeValue(node, "channel_lockdown", numChannelLockdown);
            SaveNumNodeValue(node, "max_server_channel", numMaxServerChannel);
        }
        #endregion

        #region Guild Node

        private void LoadGuildSettings()
        {
            XmlNode? guildCreateNode = contentXmlDoc.SelectSingleNode("//guild_create");

            if (guildCreateNode != null)
            {
                LoadGuildCreateAttributes(guildCreateNode);
            }
        }

        private void LoadGuildCreateAttributes(XmlNode guildCreateNode)
        {
            int guildCreateUse = GetAttributeValueAsInt(guildCreateNode, "use");
            int characterLevel = GetAttributeValueAsInt(guildCreateNode, "character_level");
            int createCost = GetAttributeValueAsInt(guildCreateNode, "create_cost");

            cbGuildCreateUse.Checked = (guildCreateUse == 1);
            numCharacterLevel.Text = characterLevel.ToString();
            tbCreateCost.Text = createCost.ToString();
        }

        private XmlNode? selectedGuildContentNode;

        private void LoadGuildContentSettings()
        {
            XmlNodeList? guildContentNodes = contentXmlDoc.SelectNodes("//guild_content");

            cmbGuild.Items.Clear();

            if (guildContentNodes != null)
            {
                foreach (XmlNode guildContentNode in guildContentNodes)
                {
                    string guildContentName = GetAttributeValue(guildContentNode, "name");
                    if (!string.IsNullOrEmpty(guildContentName))
                    {
                        cmbGuild.Items.Add(guildContentName);
                    }
                }
            }

            if (cmbGuild.Items.Count > 0)
            {
                cmbGuild.SelectedIndex = 0;
            }
        }

        private void CmbGuild_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedGuildContentName = cmbGuild.SelectedItem?.ToString();

            if (selectedGuildContentName != null)
            {
                selectedGuildContentNode = contentXmlDoc.SelectSingleNode($"//guild_content[@name='{selectedGuildContentName}']");

                if (selectedGuildContentNode != null)
                {
                    LoadGuildContentAttributes(selectedGuildContentNode);
                }
            }
        }

        private void LoadGuildContentAttributes(XmlNode guildContentNode)
        {
            int guildContentID = GetAttributeValueAsInt(guildContentNode, "id");
            bool guildContentUse = GetAttributeValueAsBool(guildContentNode, "use");

            tbGuildContentID.Text = guildContentID.ToString();
            cbGuildContentUse.Checked = guildContentUse;
        }

        private void CbGuildContentUse_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedGuildContentNode != null)
            {
                SetAttributeValue(selectedGuildContentNode, "use", cbGuildContentUse.Checked ? "1" : "0");
            }
        }

        private void SaveGuildSettings()
        {
            XmlNode? guildCreateNode = contentXmlDoc.SelectSingleNode("//guild_create");

            if (guildCreateNode != null)
            {
                contentXmlDoc.DocumentElement?.RemoveChild(guildCreateNode);
            }

            guildCreateNode = contentXmlDoc.CreateElement("guild_create");

            SetAttributeValue(guildCreateNode, "use", cbGuildCreateUse.Checked ? "1" : "0");
            SetAttributeValue(guildCreateNode, "character_level", numCharacterLevel.Text);
            SetAttributeValue(guildCreateNode, "create_cost", tbCreateCost.Text);

            contentXmlDoc.DocumentElement?.AppendChild(guildCreateNode);
        }

        private void SaveGuildContentSettings()
        {
            XmlNodeList? guildContentNodes = contentXmlDoc.SelectNodes("//guild_content");

            if (guildContentNodes != null)
            {
                foreach (XmlNode guildContentNode in guildContentNodes)
                {
                    string guildContentName = GetAttributeValue(guildContentNode, "name");

                    if (guildContentName == cmbGuild.SelectedItem?.ToString())
                    {
                        XmlAttribute? idAttribute = guildContentNode.Attributes?["id"];
                        if (idAttribute != null)
                        {
                            SetAttributeValue(guildContentNode, "id", tbGuildContentID.Text);
                        }

                        XmlAttribute? useAttribute = guildContentNode.Attributes?["use"];
                        if (useAttribute != null)
                        {
                            SetAttributeValue(guildContentNode, "use", cbGuildContentUse.Checked ? "1" : "0");
                        }
                    }
                }
            }
        }

        private int GetAttributeValueAsInt(XmlNode node, string attributeName)
        {
            string attributeValue = GetAttributeValue(node, attributeName);
            return int.TryParse(attributeValue, out int result) ? result : 0;
        }

        #endregion

        #region Shop Node
        private XmlNode? selectedShopNode;

        private void LoadShopSettings()
        {
            XmlNodeList? shopNodes = contentXmlDoc.SelectNodes("//shop");

            cmbShop.Items.Clear();

            if (shopNodes != null)
            {
                foreach (XmlNode shopNode in shopNodes)
                {
                    string shopName = GetAttributeValue(shopNode, "name");
                    if (!string.IsNullOrEmpty(shopName))
                    {
                        cmbShop.Items.Add(shopName);
                    }
                }
            }

            if (cmbShop.Items.Count > 0)
            {
                cmbShop.SelectedIndex = 0;
            }
        }

        private void CmbShop_SelectedIndexChanged(object sender, EventArgs e)
        {
            string? selectedShopName = cmbShop.SelectedItem?.ToString();

            if (selectedShopName != null)
            {
                selectedShopNode = contentXmlDoc.SelectSingleNode($"//shop[@name='{selectedShopName}']");

                if (selectedShopNode != null)
                {
                    LoadShopAttributes(selectedShopNode);
                }
            }
        }

        private void LoadShopAttributes(XmlNode shopNode)
        {
            int shopID = GetAttributeValueAsInt(shopNode, "id");
            bool shopUse = GetAttributeValueAsBool(shopNode, "use");

            tbShopID.Text = shopID.ToString();
            cbShopUse.Checked = shopUse;
        }

        private void CbShopUse_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedShopNode != null)
            {
                SetAttributeValue(selectedShopNode, "use", cbShopUse.Checked ? "1" : "0");
            }
        }

        private void SaveShopSettings()
        {
            XmlNodeList? shopNodes = contentXmlDoc.SelectNodes("//shop");

            if (shopNodes != null)
            {
                foreach (XmlNode shopNode in shopNodes)
                {
                    string shopName = GetAttributeValue(shopNode, "name");

                    if (shopName == cmbShop.SelectedItem?.ToString())
                    {
                        XmlAttribute? idAttribute = shopNode.Attributes?["id"];
                        if (idAttribute != null)
                        {
                            SetAttributeValue(shopNode, "id", tbShopID.Text);
                        }

                        XmlAttribute? useAttribute = shopNode.Attributes?["use"];
                        if (useAttribute != null)
                        {
                            SetAttributeValue(shopNode, "use", cbShopUse.Checked ? "1" : "0");
                        }
                    }
                }
            }
        }
        #endregion

        #region Dungeon Nodes

        private List<Dictionary<string, string>> dungeonSettingsList = [];
        private Dictionary<string, Dictionary<string, string>> dungeonComboBoxMap = [];
        private int maxDiffEntries = 4;

        private void LoadDungeonSettings()
        {
            XmlNodeList? dungeonNodes = contentXmlDoc.SelectNodes("//dungeon");

            dungeonSettingsList = [];
            dungeonComboBoxMap = [];

            if (dungeonNodes != null)
            {
                foreach (XmlNode dungeonNode in dungeonNodes)
                {
                    Dictionary<string, string> dungeonSettings = [];

                    if (dungeonNode.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in dungeonNode.Attributes)
                        {
                            dungeonSettings[attribute.Name] = attribute.Value;
                        }
                    }

                    dungeonSettingsList.Add(dungeonSettings);
                    dungeonComboBoxMap[dungeonSettings["name"]] = dungeonSettings;
                }

                cmbDungeon.Items.AddRange(dungeonSettingsList.Select(d => d["name"]).ToArray());

                if (cmbDungeon.Items.Count > 0)
                {
                    cmbDungeon.SelectedIndex = 0;
                }
            }
            else
            {
                currentDungeonSettings = null;
                tbDungeonID.Text = "";
                numDungeonDiff.Text = "0";
                tbDungeonDelay.Text = "";
                numDungeonCombo.Text = "0.0";
                tbDungeonHP.Text = "";
                numDungeonAttack.Text = "0.0";
                tbRangeDelay.Text = "";
                tbRangeCombo.Text = "0.0";
                tbRangeHP.Text = "";
                numRangeAttack.Text = "0.0";
                EnableDungeonFields(false);
            }
        }


        private Dictionary<string, string>? currentDungeonSettings;

        private void CmbDungeon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentDungeonSettings != null)
            {
                SaveCurrentDungeonSettings();
            }

            int selectedDungeonIndex = cmbDungeon.SelectedIndex;

            if (selectedDungeonIndex >= 0 && selectedDungeonIndex < dungeonSettingsList.Count)
            {
                currentDungeonSettings = dungeonSettingsList[selectedDungeonIndex];
                UpdateDungeonFields(currentDungeonSettings);
            }
        }

        private void UpdateDungeonFields(Dictionary<string, string> dungeonSettings)
        {
            tbDungeonID.Text = GetDungeonSettingOrDefault(dungeonSettings, "id", "");
            numDungeonDiff.Text = GetDungeonSettingOrDefault(dungeonSettings, "diff", "0");
            tbDungeonDelay.Text = GetDungeonSettingOrDefault(dungeonSettings, "delay", "0");
            numDungeonCombo.Text = GetDungeonSettingOrDefault(dungeonSettings, "combo", "0.0");
            tbDungeonHP.Text = GetDungeonSettingOrDefault(dungeonSettings, "hp", "0");
            numDungeonAttack.Text = GetDungeonSettingOrDefault(dungeonSettings, "attack", "0.0");
            tbRangeDelay.Text = GetDungeonSettingOrDefault(dungeonSettings, "range_delay", "0");
            tbRangeCombo.Text = GetDungeonSettingOrDefault(dungeonSettings, "range_combo", "0.0");
            tbRangeHP.Text = GetDungeonSettingOrDefault(dungeonSettings, "range_hp", "0");
            numRangeAttack.Text = GetDungeonSettingOrDefault(dungeonSettings, "range_attack", "0.0");
            cbUseDungeon.Checked = GetDungeonSettingOrDefault(dungeonSettings, "use", "0") == "1";
        }

        private static string GetDungeonSettingOrDefault(Dictionary<string, string> dungeonSettings, string key, string defaultValue)
        {
            return dungeonSettings.TryGetValue(key, out string? value) ? value : defaultValue;
        }

        private void SaveCurrentDungeonSettings()
        {
            if (currentDungeonSettings != null)
            {
                currentDungeonSettings["id"] = tbDungeonID.Text;
                currentDungeonSettings["diff"] = numDungeonDiff.Text;
                currentDungeonSettings["delay"] = tbDungeonDelay.Text;
                currentDungeonSettings["combo"] = numDungeonCombo.Text;
                currentDungeonSettings["hp"] = tbDungeonHP.Text;
                currentDungeonSettings["attack"] = numDungeonAttack.Text;
                currentDungeonSettings["range_delay"] = tbRangeDelay.Text;
                currentDungeonSettings["range_combo"] = tbRangeCombo.Text;
                currentDungeonSettings["range_hp"] = tbRangeHP.Text;
                currentDungeonSettings["range_attack"] = numRangeAttack.Text;
                currentDungeonSettings["use"] = cbUseDungeon.Checked ? "1" : "0";
            }
        }

        private void EnableDungeonFields(bool enable)
        {
            tbDungeonID.Enabled = enable;
            numDungeonDiff.Enabled = enable;
            tbDungeonDelay.Enabled = enable;
            numDungeonCombo.Enabled = enable;
            tbDungeonHP.Enabled = enable;
            numDungeonAttack.Enabled = enable;
            tbRangeDelay.Enabled = enable;
            tbRangeCombo.Enabled = enable;
            tbRangeHP.Enabled = enable;
            numRangeAttack.Enabled = enable;
        }

        private void SaveDungeonSettings()
        {
            XmlNodeList? dungeonNodes = contentXmlDoc.SelectNodes("//dungeon");

            if (dungeonNodes != null)
            {
                foreach (XmlNode dungeonNode in dungeonNodes)
                {
                    contentXmlDoc.DocumentElement?.RemoveChild(dungeonNode);
                }
            }

            foreach (Dictionary<string, string> dungeonSettings in dungeonSettingsList)
            {
                XmlElement newDungeonNode = contentXmlDoc.CreateElement("dungeon");

                foreach (KeyValuePair<string, string> kvp in dungeonSettings)
                {
                    newDungeonNode.SetAttribute(kvp.Key, kvp.Value);
                }

                contentXmlDoc.DocumentElement?.AppendChild(newDungeonNode);
            }
        }

        public bool DungeonExists(string name, string diff)
        {
            return dungeonSettingsList.Any(d =>
                d != null &&
                d.ContainsKey("name") && d.ContainsKey("diff") &&
                d["name"] == name && d["diff"] == diff
            );
        }


        private void BtnAddDiff_Click(object sender, EventArgs e)
        {
            if (currentDungeonSettings == null || cmbDungeon.SelectedItem == null)
            {
                MessageBox.Show("Dungeon list is empty, please add a dungeon first.", "Empty List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedDungeonName = currentDungeonSettings["name"];
            int diffCount = dungeonSettingsList.Count(d => d["name"] == selectedDungeonName);

            if (diffCount >= maxDiffEntries)
            {
                MessageBox.Show($"Maximum number of entries ({maxDiffEntries}) reached for dungeon {selectedDungeonName}.", "Dungeon Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int lastDungeonIndex = dungeonSettingsList.FindLastIndex(d => d["name"] == selectedDungeonName);
            int highestDiff = dungeonSettingsList
                .Where(d => d["name"] == selectedDungeonName)
                .Select(d => d.TryGetValue("diff", out string? value) ? int.Parse(value) : 0)
                .Max();

            int newDiff = highestDiff + 1;

            while (dungeonSettingsList.Any(d => d.ContainsKey("name") && d["name"] == selectedDungeonName && d.ContainsKey("diff") && int.Parse(d["diff"]) == newDiff))
            {
                newDiff++;
            }

            Dictionary<string, string> newSetting = new(currentDungeonSettings)
            {
                ["diff"] = newDiff.ToString()
            };

            dungeonSettingsList.Insert(lastDungeonIndex + 1, newSetting);
            string newDungeonName = selectedDungeonName + newDiff;
            cmbDungeon.Items.Insert(lastDungeonIndex + 1, newDungeonName);
            cmbDungeon.SelectedIndex = lastDungeonIndex + 1;
            EnableDungeonFields(true);
        }

        private void BtnNewDungeon_Click(object sender, EventArgs e)
        {
            DungeonForm dungeonForm = new(this);
            DialogResult result = dungeonForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                Dictionary<string, string> newDungeonSettings = dungeonForm.GetDungeonSettings();
                if (newDungeonSettings != null && newDungeonSettings.TryGetValue("name", out string? value))
                {
                    dungeonSettingsList.Add(newDungeonSettings);
                    string newDungeonName = value;
                    cmbDungeon.Items.Add(newDungeonName);
                    cmbDungeon.SelectedIndex = cmbDungeon.Items.Count - 1;
                    EnableDungeonFields(true);
                }

            }
        }

        private void BtnEditDungeon_Click(object sender, EventArgs e)
        {
            if (cmbDungeon.SelectedItem != null)
            {
                int selectedIndex = cmbDungeon.SelectedIndex;
                Dictionary<string, string> selectedDungeonSettings = dungeonSettingsList[selectedIndex];

                DungeonForm dungeonForm = new(selectedDungeonSettings, true);
                DialogResult result = dungeonForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    dungeonSettingsList[selectedIndex] = dungeonForm.GetDungeonSettings();
                    cmbDungeon.Items[selectedIndex] = dungeonForm.GetDungeonName();

                    if (currentDungeonSettings == selectedDungeonSettings)
                    {
                        currentDungeonSettings = dungeonForm.GetDungeonSettings();
                        UpdateDungeonFields(currentDungeonSettings);
                    }
                }
            }
        }

        private void BtnRemoveDungeon_Click(object sender, EventArgs e)
        {
            if (currentDungeonSettings != null && cmbDungeon.SelectedItem != null)
            {
                int selectedDungeonIndex = cmbDungeon.SelectedIndex;

                dungeonSettingsList.RemoveAt(selectedDungeonIndex);
                cmbDungeon.Items.RemoveAt(selectedDungeonIndex);

                if (selectedDungeonIndex > 0)
                {
                    cmbDungeon.SelectedIndex = selectedDungeonIndex - 1;
                }
                else if (cmbDungeon.Items.Count > 0)
                {
                    cmbDungeon.SelectedIndex = 0;
                }
                else
                {
                    currentDungeonSettings = null;
                    tbDungeonID.Text = "";
                    numDungeonDiff.Text = "0";
                    tbDungeonDelay.Text = "";
                    numDungeonCombo.Text = "0.0";
                    tbDungeonHP.Text = "";
                    numDungeonAttack.Text = "0.0";
                    tbRangeDelay.Text = "";
                    tbRangeCombo.Text = "0.0";
                    tbRangeHP.Text = "";
                    numRangeAttack.Text = "0.0";
                    EnableDungeonFields(false);
                }
            }
        }
        #endregion

        #region Helpers

        private static void ClearControls(Control control)
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
        #endregion

        #endregion

        #endregion

        #region Controls Events

        private void Num_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;

            if (numericUpDown.Value == numericUpDown.Maximum)
            {
                numericUpDown.Value = numericUpDown.Minimum;
            }
        }

        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LbVersion_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = Url, UseShellExecute = true });
        }

        private void PbEyeSqlPassword_MouseDown(object sender, MouseEventArgs e)
        {
            tbSQLPassword.UseSystemPasswordChar = false;
        }

        private void PbEyeSqlPassword_MouseUp(object sender, MouseEventArgs e)
        {
            tbSQLPassword.UseSystemPasswordChar = true;
        }

        private void PbEyeDBPassword_MouseDown(object sender, MouseEventArgs e)
        {
            tbDBPassword.UseSystemPasswordChar = false;
        }

        private void PbEyeDBPassword_MouseUp(object sender, MouseEventArgs e)
        {
            tbDBPassword.UseSystemPasswordChar = true;
        }

        private void PbEyeSmtpPassword_MouseDown(object sender, MouseEventArgs e)
        {
            tbSmtpPassword.UseSystemPasswordChar = false;
        }

        private void PbEyeSmtpPassword_MouseUp(object sender, MouseEventArgs e)
        {
            tbSmtpPassword.UseSystemPasswordChar = true;
        }

        private void TbAPIDir_TextChanged(object sender, EventArgs e)
        {
            LoadAPIConfig();
        }

        private void TbServerDir_TextChanged(object sender, EventArgs e)
        {
            LoadServerConfig();
        }
        #endregion

    }

}