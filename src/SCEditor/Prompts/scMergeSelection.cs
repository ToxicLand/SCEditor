using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEditor.Prompts
{
    public partial class scMergeSelection : Form
    {
        private List<ScObject> exportsToList;
        public CheckedListBox.CheckedItemCollection checkedExports => exportsListBox.CheckedItems;
        private bool allChecked;
        public bool newTextureChecked;
        public float scaleFactor { get; private set; }

        public scMergeSelection(List<ScObject> exportsList)
        {
            InitializeComponent();
            exportsToList = exportsList.OrderBy(ex => ex.GetName()).ToList();
            allChecked = false;
            newTextureChecked = false;
            populateListBox();
        }

        public class exportItemClass
        {
            public string exportName { get; set; }
            public object exportData { get; set; }
        }

        private void populateListBox()
        {
            foreach (object export in exportsToList)
            {
                exportsListBox.Items.Add(new exportItemClass { exportName = ((Export)export).GetName(), exportData = export }, false);
            }

            // REMOVE IN PROD
            string toImport = "barb_wire;barbwire_lvl1_0;barbwire_lvl1_1_a;barbwire_lvl1_1_b;barbwire_lvl1_2_a;barbwire_lvl1_2_b;barbwire_lvl1_3;boom_destroyed_1;boom_destroyed_2;boom_destroyed_3;cannon_destroyed_1;cannon_destroyed_2;cannon_destroyed_3;cannon_destroyed_4;cannon_destroyed_5;flamethrower_destroyed_1;flamethrower_destroyed_2;flamethrower_destroyed_3;gen_build_2;gen_build_3;gen_build_4;gen_build_5;gen_construction_2;gen_construction_3;gen_construction_4;gen_construction_5;gen_destroyed_2;gen_destroyed_2_native;gen_destroyed_3;gen_destroyed_3_C;gen_destroyed_3_M;gen_destroyed_3_native;gen_destroyed_3_sabotage;gen_destroyed_4;gen_destroyed_4_C;gen_destroyed_4_M;gen_destroyed_5;gen_destroyed_5_C;gen_destroyed_5_M;gen_destroyed_cell;hatch_destroyed_01;machinegun_destroyed_1;machinegun_destroyed_2;machinegun_destroyed_3;machinegun_destroyed_4;shock_destroyed_1;shock_destroyed_2;taunt_tower_destroyed;mortar_destroyed_1;mortar_destroyed_2;mortar_destroyed_3;mortar_destroyed_4;sniper_destroyed_1;sniper_destroyed_2;sniper_destroyed_3;sniper_destroyed_4;cryobomb_destroyed_1;tower_turret_lvl1;tower_turret_lvl2;tower_turret_lvl3;tower_turret_lvl4;tower_turret_lvl5;tower_turret_lvl6;tower_turret_lvl7;tower_turret_lvl8;tower_turret_lvl10;tower_turret_lvl11;tower_turret_lvl12;tower_turret_lvl13;tower_turret_lvl14;tower_turret_lvl15;tower_turret_lvl16;tower_turret_lvl17;tower_turret_lvl18;tower_turret_lvl19;tower_turret_lvl21;tower_turret_lvl22;mortar_lvl1;mortar_lvl2;mortar_lvl3;mortar_lvl4;mortar_lvl5;mortar_lvl6;mortar_lvl7;mortar_lvl8;mortar_lvl9;mortar_lvl11;mortar_lvl12;mortar_lvl13;mortar_lvl14;mortar_lvl16;mortar_lvl21;mortar_lvl22;mortar_lvl23;machinegun_turret_lvl1;machinegun_turret_lvl2;machinegun_turret_lvl3;machinegun_turret_lvl4;machinegun_turret_lvl5;machinegun_turret_lvl6;machinegun_turret_lvl8;machinegun_turret_lvl9;machinegun_turret_lvl11;machinegun_turret_lvl12;machinegun_turret_lvl21;machinegun_turret_lvl22;missile_launcher_lvl1;missile_launcher_lvl2;missile_launcher_lvl3;missile_launcher_lvl5;missile_launcher_lvl6;missile_launcher_lvl7;missile_launcher_lvl9;missile_launcher_lvl10;flame_thrower_lvl1;flame_thrower_lvl2;flame_thrower_lvl5;flame_thrower_lvl6;flame_thrower_lvl7;flame_thrower_lvl9;flame_thrower_lvl10;flame_thrower_lvl11;flame_thrower_lvl12;flame_thrower_lvl14;flame_thrower_lvl15;flame_thrower_lvl18;flame_thrower_lvl19;flame_thrower_lvl20;basic_cannon_lvl1;basic_cannon_lvl2;basic_cannon_lvl3;basic_cannon_lvl4;basic_cannon_lvl5;basic_cannon_lvl6;basic_cannon_lvl10;basic_cannon_lvl11;basic_cannon_lvl12;basic_cannon_lvl13;basic_cannon_lvl14;basic_cannon_lvl15;basic_cannon_lvl16;basic_cannon_lvl19;basic_cannon_lvl21;basic_cannon_lvl22;boom_cannon_lvl1;boom_cannon_lvl2;boom_cannon_lvl3;boom_cannon_lvl4;boom_cannon_lvl5;boom_cannon_lvl6;boom_cannon_lvl7;boom_cannon_lvl8;boom_cannon_lvl9;boom_cannon_lvl10;boom_cannon_lvl11;boom_cannon_lvl12;boom_cannon_lvl13;boom_cannon_lvl15;boom_cannon_lvl16;shock_launcher_lvl1;shock_launcher_lvl2;shock_launcher_lvl3;shock_launcher_lvl4;shock_launcher_lvl5;shock_launcher_lvl6;shock_launcher_lvl7;shock_launcher_lvl8;shock_minigun_lvl1;shock_minigun_lvl2;shock_minigun_lvl3;lazer_lvl1;lazer_lvl2;lazer_lvl3;attack_booster_lvl1;attack_booster_lvl2;attack_booster_lvl3;doom_cannon_lvl1;doom_cannon_lvl2;doom_cannon_lvl3;shieldgenerator_lvl1;shieldgenerator_lvl2;shieldgenerator_lvl3;harpoon_lvl1;harpoon_lvl2;harpoon_lvl3;flame_surprise_lvl1;flame_surprise_lvl1_appear;flame_surprise_lvl1_hide;flame_surprise_lvl1_hatch;flame_surprise_lvl2;flame_surprise_lvl2_appear;flame_surprise_lvl2_hide;flame_surprise_lvl2_hatch;flame_surprise_lvl3;flame_surprise_lvl3_appear;flame_surprise_lvl3_hide;flame_surprise_lvl3_hatch;super_sniper_lvl1;super_sniper_lvl2;super_sniper_lvl3;microwave_tower_lvl1;microwave_tower_lvl2;microwave_tower_lvl3;bubble_generator_lvl1;bubble_generator_lvl2;bubble_generator_lvl3;cannon_surprise_lvl1_appear;cannon_surprise_lvl1_hide;cannon_surprise_lvl1_hatch;cannon_surprise_lvl2_appear;cannon_surprise_lvl2_hide;cannon_surprise_lvl2_hatch;cannon_surprise_lvl3_appear;cannon_surprise_lvl3_hide;cannon_surprise_lvl3_hatch;bomb_tower_lvl1;bomb_tower_lvl2;bomb_tower_lvl3;bomb_tower_triggered;bomb_tower_triggered_2;bomb_tower_triggered_3;hidden_cannon_lvl1;hidden_cannon_lvl2;hidden_cannon_lvl3;freezeray_beam;laser_beam_glow";
            for (int i = 0; i < exportsListBox.Items.Count; i++)
            {
                if (toImport.Split(';').Contains(((exportItemClass)exportsListBox.Items[i]).exportName))
                    exportsListBox.SetItemChecked(i, true);
            }

            exportsListBox.DisplayMember = "exportName";
            exportsListBox.ValueMember = "exportData";

            exportsListBox.Refresh();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (exportsListBox.CheckedItems.Count > 0)
            {
                DialogResult confirm = MessageBox.Show("Are you sure you want to import the checked exports?","Confirm", MessageBoxButtons.YesNoCancel);

                if (confirm != DialogResult.Cancel)
                {
                    if (confirm == DialogResult.Yes)
                        this.DialogResult = DialogResult.Yes;

                    this.Close();
                }
            }
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < exportsListBox.Items.Count; i++)
                exportsListBox.SetItemChecked(i, !allChecked);

            allChecked = !allChecked;
        }

        private void newTextureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newTextureChecked = !newTextureChecked;
        }

        private void scaleFactorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Back)
            {
                if (scaleFactorTextBox.Text.Contains(".") && e.KeyData == Keys.OemPeriod)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                else if (e.KeyData != Keys.OemPeriod)
                {
                    if (float.TryParse(Convert.ToString((char)e.KeyData), out float _))
                    {
                        if ((float)Convert.ToDouble(string.Format("{0}{1}", scaleFactor, float.Parse(Convert.ToString((char)e.KeyData)))) >= float.MaxValue)
                        {
                            e.SuppressKeyPress = true;
                        }
                    }
                    else
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }

        private void scaleFactorTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(scaleFactorTextBox.Text))
                scaleFactor = float.Parse(scaleFactorTextBox.Text);
            else
                scaleFactor = 0;
        }
    }
}
