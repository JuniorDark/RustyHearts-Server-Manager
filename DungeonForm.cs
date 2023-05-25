namespace RHServerManager
{
    public partial class DungeonForm : Form
    {

        private readonly Form1 mainForm;
        public DungeonForm(Form1 form)
        {
            InitializeComponent();
            mainForm = form;
        }

        private Dictionary<string, string> dungeonSettings;

        private readonly bool isEditMode;

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
            dungeonSettings["use"] = checkBoxUseDungeon.Checked ? "1" : "0";
            dungeonSettings["id"] = dungeonIDTextBox.Text;
            dungeonSettings["name"] = dungeonNameTextBox.Text;
            dungeonSettings["diff"] = numericUpDownDungeonDiff.Value.ToString();
            dungeonSettings["delay"] = dungeonDelayTextBox.Text;
            dungeonSettings["combo"] = numericUpDownDungeonCombo.Value.ToString();
            dungeonSettings["hp"] = dungeonHPTextBox.Text;
            dungeonSettings["attack"] = numericUpDownDungeonAttack.Value.ToString();
            dungeonSettings["range_delay"] = rangeDelayTextBox.Text;
            dungeonSettings["range_combo"] = rangeComboTextBox.Text;
            dungeonSettings["range_hp"] = rangeHPTextBox.Text;
            dungeonSettings["range_attack"] = numericUpDownRangeAttack.Value.ToString();

            return dungeonSettings;
        }

        public string GetDungeonName()
        {
            return dungeonNameTextBox.Text;
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
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

            if (!isEditMode && mainForm.DungeonExists(DungeonName, Diff))
            {
                MessageBox.Show($"Dungeon {DungeonName} with difficulty {Diff} already exists.", "Duplicate Dungeon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
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
