namespace RHServerManager
{
    public partial class DungeonForm : Form
    {

        private readonly MainForm? mainForm;
        private readonly Dictionary<string, string>? dungeonSettings;
        private readonly bool isEditMode;

        public DungeonForm(MainForm form)
        {
            InitializeComponent();
            mainForm = form;
        }

        public DungeonForm(Dictionary<string, string> settings, bool editMode)
        {
            InitializeComponent();

            isEditMode = editMode;

            // Set the initial values of the form controls based on the provided settings
            dungeonSettings = new Dictionary<string, string>(settings);
            checkBoxUseDungeon.Checked = dungeonSettings.GetValueOrDefault("use", "0") == "1";
            dungeonIDTextBox.Text = dungeonSettings.GetValueOrDefault("id", "");
            dungeonNameTextBox.Text = dungeonSettings.GetValueOrDefault("name", "");
            numericUpDownDungeonDiff.Value = int.Parse(dungeonSettings.GetValueOrDefault("diff", "0"));
            dungeonDelayTextBox.Text = dungeonSettings.GetValueOrDefault("delay", "");
            numericUpDownDungeonCombo.Value = decimal.Parse(dungeonSettings.GetValueOrDefault("combo", "0"));
            dungeonHPTextBox.Text = dungeonSettings.GetValueOrDefault("hp", "");
            numericUpDownDungeonAttack.Value = decimal.Parse(dungeonSettings.GetValueOrDefault("attack", "0"));
            rangeDelayTextBox.Text = dungeonSettings.GetValueOrDefault("range_delay", "");
            rangeComboTextBox.Text = dungeonSettings.GetValueOrDefault("range_combo", "");
            rangeHPTextBox.Text = dungeonSettings.GetValueOrDefault("range_hp", "");
            numericUpDownRangeAttack.Value = decimal.Parse(dungeonSettings.GetValueOrDefault("range_attack", "0"));
        }

        public Dictionary<string, string> GetDungeonSettings()
        {
            // Retrieve the edited values from the form controls and update the dungeon settings
            Dictionary<string, string> dungeonSettings = new()
            {
                ["use"] = checkBoxUseDungeon.Checked ? "1" : "0",
                ["id"] = dungeonIDTextBox.Text,
                ["name"] = dungeonNameTextBox.Text,
                ["diff"] = numericUpDownDungeonDiff.Value.ToString(),
                ["delay"] = dungeonDelayTextBox.Text,
                ["combo"] = numericUpDownDungeonCombo.Value.ToString(),
                ["hp"] = dungeonHPTextBox.Text,
                ["attack"] = numericUpDownDungeonAttack.Value.ToString(),
                ["range_delay"] = rangeDelayTextBox.Text,
                ["range_combo"] = rangeComboTextBox.Text,
                ["range_hp"] = rangeHPTextBox.Text,
                ["range_attack"] = numericUpDownRangeAttack.Value.ToString()
            };

            return dungeonSettings;
        }

        public string GetDungeonName()
        {
            return dungeonNameTextBox.Text;
        }

        private void ButtonConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dungeonNameTextBox.Text))
            {
                MessageBox.Show("Please enter a valid name.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(dungeonIDTextBox.Text))
            {
                MessageBox.Show("Please enter a valid ID.", "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Use = checkBoxUseDungeon.Checked;
            Id = dungeonIDTextBox.Text;
            DungeonName = dungeonNameTextBox.Text;
            Diff = numericUpDownDungeonDiff.Text;
            Delay = dungeonDelayTextBox.Text;
            Combo = numericUpDownDungeonCombo.Text;
            HP = dungeonHPTextBox.Text;
            Attack = numericUpDownDungeonAttack.Text;
            RangeDelay = rangeDelayTextBox.Text;
            RangeCombo = rangeComboTextBox.Text;
            RangeHP = rangeHPTextBox.Text;
            RangeAttack = numericUpDownRangeAttack.Text;

            if (!isEditMode && mainForm?.DungeonExists(DungeonName, Diff) == true)
            {
                MessageBox.Show($"Dungeon {DungeonName} with difficulty {Diff} already exists.", "Duplicate Dungeon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void DungeonForm_Load(object sender, EventArgs e)
        {
            if (isEditMode)
            {
                Text = "Edit Dungeon";
                buttonConfirm.Text = "Save";
            }
            else
            {
                Text = "Add Dungeon";
                buttonConfirm.Text = "Add";
            }
        }

        public bool? Use { get; private set; }
        public string? Id { get; private set; }
        public string? DungeonName { get; private set; }
        public string? Diff { get; private set; }
        public string? Delay { get; private set; }
        public string? Combo { get; private set; }
        public string? HP { get; private set; }
        public string? Attack { get; private set; }
        public string? RangeDelay { get; private set; }
        public string? RangeCombo { get; private set; }
        public string? RangeHP { get; private set; }
        public string? RangeAttack { get; private set; }
    }

}
