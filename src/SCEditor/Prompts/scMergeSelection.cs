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
            string toImport = "assault_troop_lvl1_attack1_1;assault_troop_lvl1_attack1_2;assault_troop_lvl1_attack1_3;assault_troop_lvl1_cheer1_1;assault_troop_lvl1_cheer1_2;assault_troop_lvl1_cheer1_3;assault_troop_lvl1_idle1_1;assault_troop_lvl1_idle1_2;assault_troop_lvl1_idle1_3;assault_troop_lvl1_run1_1;assault_troop_lvl1_run1_2;assault_troop_lvl1_run1_3;atom_troop_lvl1_attack1_1;atom_troop_lvl1_attack1_2;atom_troop_lvl1_attack1_3;atom_troop_lvl1_cheer1_1;atom_troop_lvl1_cheer1_2;atom_troop_lvl1_cheer1_3;atom_troop_lvl1_hit1_1;atom_troop_lvl1_hit1_2;atom_troop_lvl1_hit1_3;atom_troop_lvl1_idle1_1;atom_troop_lvl1_idle1_2;atom_troop_lvl1_idle1_3;atom_troop_lvl1_run1_1;atom_troop_lvl1_run1_2;atom_troop_lvl1_run1_3;bandit_attack1_3;bandit_attack1_5;bandit_attack1_7;bandit_dash1_3;bandit_dash1_5;bandit_dash1_7;bandit_idle1_3;bandit_idle1_5;bandit_idle1_7;bandit_run1_3;bandit_run1_5;bandit_run1_7;bazooka_girl_lvl0_1_dodge_1;bazooka_girl_lvl0_attack1_1;bazooka_girl_lvl0_attack1_2;bazooka_girl_lvl0_attack1_3;bazooka_girl_lvl0_idle1_1;bazooka_girl_lvl0_idle1_2;bazooka_girl_lvl0_idle1_3;bazooka_girl_lvl0_run1_1;bazooka_girl_lvl0_run1_2;bazooka_girl_lvl0_run1_3;bombardier_lvl1_attack1_1;bombardier_lvl1_attack1_2;bombardier_lvl1_attack1_3;bombardier_lvl1_idle1_1;bombardier_lvl1_idle1_2;bombardier_lvl1_idle1_3;bombardier_lvl1_run1_1;bombardier_lvl1_run1_2;bombardier_lvl1_run1_3;chr_musketeer_attack1_3;chr_musketeer_attack1_5;chr_musketeer_attack1_7;chr_musketeer_idle1_3;chr_musketeer_idle1_5;chr_musketeer_idle1_7;chr_musketeer_run1_3;chr_musketeer_run1_5;chr_musketeer_run1_7;electro_wizard_attack1_3;electro_wizard_attack1_5;electro_wizard_attack1_7;electro_wizard_idle1_3;electro_wizard_idle1_5;electro_wizard_idle1_7;electro_wizard_run1_3;electro_wizard_run1_5;electro_wizard_run1_7;elixir_blob_attack1_3;elixir_blob_attack1_5;elixir_blob_attack1_7;elixir_blob_idle1_3;elixir_blob_idle1_5;elixir_blob_idle1_7;elixir_blob_run1_3;elixir_blob_run1_5;elixir_blob_run1_7;elixir_golem_attack1_3;elixir_golem_attack1_5;elixir_golem_attack1_7;elixir_golem_idle1_3;elixir_golem_idle1_5;elixir_golem_idle1_7;elixir_golem_run1_3;elixir_golem_run1_5;elixir_golem_run1_7;elixir_golemite_attack1_3;elixir_golemite_attack1_5;elixir_golemite_attack1_7;elixir_golemite_idle1_3;elixir_golemite_idle1_5;elixir_golemite_idle1_7;elixir_golemite_run1_3;elixir_golemite_run1_5;elixir_golemite_run1_7;Ftank_lvl1_drive1_0;Ftank_lvl1_drive1_0.3;Ftank_lvl1_drive1_0.6;Ftank_lvl1_drive1_1;Ftank_lvl1_drive1_1.3;Ftank_lvl1_drive1_1.6;Ftank_lvl1_drive1_2;Ftank_lvl1_drive1_2.3;Ftank_lvl1_drive1_2.6;Ftank_lvl1_drive1_3;Ftank_lvl1_drive1_3.3;Ftank_lvl1_drive1_3.6;Ftank_lvl1_drive1_4;Ftank_lvl1_idle_0;Ftank_lvl1_idle_0.3;Ftank_lvl1_idle_0.6;Ftank_lvl1_idle_1;Ftank_lvl1_idle_1.3;Ftank_lvl1_idle_1.6;Ftank_lvl1_idle_2;Ftank_lvl1_idle_2.3;Ftank_lvl1_idle_2.6;Ftank_lvl1_idle_3;Ftank_lvl1_idle_3.3;Ftank_lvl1_idle_3.6;Ftank_lvl1_idle_4;goku_attack1_1;goku_attack1_2;goku_attack1_3;goku_idle1_1;goku_idle1_2;goku_idle1_3;goku_walk1_1;goku_walk1_2;goku_walk1_3;GoldenKnight_ability1_3;GoldenKnight_ability1_5;GoldenKnight_ability1_7;GoldenKnight_attack1_3;GoldenKnight_attack1_5;GoldenKnight_attack1_7;GoldenKnight_bersek1_3;GoldenKnight_bersek1_5;GoldenKnight_bersek1_7;GoldenKnight_dash1_3;GoldenKnight_dash1_5;GoldenKnight_dash1_7;GoldenKnight_dash2_3;GoldenKnight_dash2_5;GoldenKnight_dash2_7;GoldenKnight_dash3_3;GoldenKnight_dash3_5;GoldenKnight_dash3_7;GoldenKnight_dash4_3;GoldenKnight_dash4_5;GoldenKnight_dash4_7;GoldenKnight_idle1_3;GoldenKnight_idle1_5;GoldenKnight_idle1_7;GoldenKnight_run1_3;GoldenKnight_run1_5;GoldenKnight_run1_7;granadier_lvl1_attack_1;granadier_lvl1_attack_2;granadier_lvl1_attack_3;granadier_lvl1_idle_1;granadier_lvl1_idle_2;granadier_lvl1_idle_3;granadier_lvl1_run_1;granadier_lvl1_run_2;granadier_lvl1_run_3;heavy_gunner_lvl1_idle1_1;heavy_gunner_lvl1_idle1_2;heavy_gunner_lvl1_idle1_3;heavy_gunner_lvl1_run1_1;heavy_gunner_lvl1_run1_2;heavy_gunner_lvl1_run1_3;heavy_sniper_attack1_1;heavy_sniper_attack1_2;heavy_sniper_attack1_3;heavy_sniper_idle1_1;heavy_sniper_idle1_2;heavy_sniper_idle1_3;magic_archer_attack1_3;magic_archer_attack1_5;magic_archer_attack1_7;magic_archer_idle1_3;magic_archer_idle1_5;magic_archer_idle1_7;magic_archer_run1_3;magic_archer_run1_5;magic_archer_run1_7;magic_arrow_projectile_blue;medic_troop_lvl1_attack1_1;medic_troop_lvl1_attack1_2;medic_troop_lvl1_attack1_3;medic_troop_lvl1_cheer1_1;medic_troop_lvl1_cheer1_2;medic_troop_lvl1_cheer1_3;medic_troop_lvl1_idle1_1;medic_troop_lvl1_idle1_2;medic_troop_lvl1_idle1_3;medic_troop_lvl1_run1_1;medic_troop_lvl1_run1_2;medic_troop_lvl1_run1_3;medic_troop_lvl1_runAtt_1;medic_troop_lvl1_runAtt_2;medic_troop_lvl1_runAtt_3;mega_knight_attack1_3;mega_knight_attack1_5;mega_knight_attack1_7;mega_knight_deploy1_1;mega_knight_gloveA;mega_knight_gloveB;mega_knight_idle1_3;mega_knight_idle1_5;mega_knight_idle1_7;mega_knight_jump1_3;mega_knight_jump1_5;mega_knight_jump1_7;mega_knight_run1_3;mega_knight_run1_5;mega_knight_run1_7;native_warrior1_attack1_1;native_warrior1_attack1_2;native_warrior1_attack1_3;native_warrior1_cheer1_1;native_warrior1_cheer1_2;native_warrior1_cheer1_3;native_warrior1_idle1_1;native_warrior1_idle1_2;native_warrior1_idle1_3;native_warrior1_run_1;native_warrior1_run_2;native_warrior1_run_3;princess_attack1_3;princess_attack1_5;princess_attack1_7;princess_idle1_3;princess_idle1_5;princess_idle1_7;princess_run1_3;princess_run1_5;princess_run1_7;princess_tower_attack1_3;princess_tower_attack1_5;princess_tower_attack1_7;princess_tower_idle1_3;princess_tower_idle1_5;princess_tower_idle1_7;princess_tower_run1_3;princess_tower_run1_5;princess_tower_run1_7;rascal_boy_attack1_3;rascal_boy_attack1_5;rascal_boy_attack1_7;rascal_boy_idle1_3;rascal_boy_idle1_5;rascal_boy_idle1_7;rascal_boy_run1_3;rascal_boy_run1_5;rascal_boy_run1_7;rascal_girl_attack1_3;rascal_girl_attack1_5;rascal_girl_attack1_7;rascal_girl_idle1_3;rascal_girl_idle1_5;rascal_girl_idle1_7;rascal_girl_run1_3;rascal_girl_run1_5;rascal_girl_run1_7;skeleton_dragon_bone;skeleton_dragon_bone_b;skeletondragon_attack1_3;skeletondragon_attack1_5;skeletondragon_attack1_7;skeletondragon_projectile;skeletondragon_run1_3;skeletondragon_run1_5;skeletondragon_run1_7;SkeletonKing_ability1_3;SkeletonKing_ability1_5;SkeletonKing_ability1_7;SkeletonKing_attack1_3;SkeletonKing_attack1_5;SkeletonKing_attack1_7;SkeletonKing_idle1_3;SkeletonKing_idle1_5;SkeletonKing_idle1_7;SkeletonKing_run1_3;SkeletonKing_run1_5;SkeletonKing_run1_7;tank_laser_lvl1_drive1_0;tank_laser_lvl1_drive1_0.3;tank_laser_lvl1_drive1_0.6;tank_laser_lvl1_drive1_1;tank_laser_lvl1_drive1_1.3;tank_laser_lvl1_drive1_1.6;tank_laser_lvl1_drive1_2;tank_laser_lvl1_drive1_2.3;tank_laser_lvl1_drive1_2.6;tank_laser_lvl1_drive1_3;tank_laser_lvl1_drive1_3.3;tank_laser_lvl1_drive1_3.6;tank_laser_lvl1_drive1_4;tank_laser_lvl1_idle_0;tank_laser_lvl1_idle_0.3;tank_laser_lvl1_idle_0.6;tank_laser_lvl1_idle_1;tank_laser_lvl1_idle_1.3;tank_laser_lvl1_idle_1.6;tank_laser_lvl1_idle_2;tank_laser_lvl1_idle_2.3;tank_laser_lvl1_idle_2.6;tank_laser_lvl1_idle_3;tank_laser_lvl1_idle_3.3;tank_laser_lvl1_idle_3.6;tank_laser_lvl1_idle_4;tank_lvl1_drive1_0;tank_lvl1_drive1_0.3;tank_lvl1_drive1_0.6;tank_lvl1_drive1_1;tank_lvl1_drive1_1.3;tank_lvl1_drive1_1.6;tank_lvl1_drive1_2;tank_lvl1_drive1_2.3;tank_lvl1_drive1_2.6;tank_lvl1_drive1_3;tank_lvl1_drive1_3.3;tank_lvl1_drive1_3.6;tank_lvl1_drive1_4;tank_lvl1_idle_0;tank_lvl1_idle_0.3;tank_lvl1_idle_0.6;tank_lvl1_idle_1;tank_lvl1_idle_1.3;tank_lvl1_idle_1.6;tank_lvl1_idle_2;tank_lvl1_idle_2.3;tank_lvl1_idle_2.6;tank_lvl1_idle_3;tank_lvl1_idle_3.3;tank_lvl1_idle_3.6;tank_lvl1_idle_4;tank_mortar_lvl1_drive1_0;tank_mortar_lvl1_drive1_0.3;tank_mortar_lvl1_drive1_0.6;tank_mortar_lvl1_drive1_1;tank_mortar_lvl1_drive1_1.3;tank_mortar_lvl1_drive1_1.6;tank_mortar_lvl1_drive1_2;tank_mortar_lvl1_drive1_2.3;tank_mortar_lvl1_drive1_2.6;tank_mortar_lvl1_drive1_3;tank_mortar_lvl1_drive1_3.3;tank_mortar_lvl1_drive1_3.6;tank_mortar_lvl1_drive1_4;tank_mortar_lvl1_idle_0;tank_mortar_lvl1_idle_0.3;tank_mortar_lvl1_idle_0.6;tank_mortar_lvl1_idle_1;tank_mortar_lvl1_idle_1.3;tank_mortar_lvl1_idle_1.6;tank_mortar_lvl1_idle_2;tank_mortar_lvl1_idle_2.3;tank_mortar_lvl1_idle_2.6;tank_mortar_lvl1_idle_3;tank_mortar_lvl1_idle_3.3;tank_mortar_lvl1_idle_3.6;tank_mortar_lvl1_idle_4;ZapMachine_attack1_1;ZapMachine_attack1_10;ZapMachine_attack1_11;ZapMachine_attack1_12;ZapMachine_attack1_13;ZapMachine_attack1_14;ZapMachine_attack1_15;ZapMachine_attack1_16;ZapMachine_attack1_17;ZapMachine_attack1_18;ZapMachine_attack1_2;ZapMachine_attack1_3;ZapMachine_attack1_4;ZapMachine_attack1_5;ZapMachine_attack1_6;ZapMachine_attack1_7;ZapMachine_attack1_8;ZapMachine_attack1_9;zapMachine_deathWheel;zapMachine_deathWheel2;ZapMachine_idle1_1;ZapMachine_idle1_10;ZapMachine_idle1_11;ZapMachine_idle1_12;ZapMachine_idle1_13;ZapMachine_idle1_14;ZapMachine_idle1_15;ZapMachine_idle1_16;ZapMachine_idle1_17;ZapMachine_idle1_18;ZapMachine_idle1_2;ZapMachine_idle1_3;ZapMachine_idle1_4;ZapMachine_idle1_5;ZapMachine_idle1_6;ZapMachine_idle1_7;ZapMachine_idle1_8;ZapMachine_idle1_9;ZapMachine_run1_1;ZapMachine_run1_10;ZapMachine_run1_11;ZapMachine_run1_12;ZapMachine_run1_13;ZapMachine_run1_14;ZapMachine_run1_15;ZapMachine_run1_16;ZapMachine_run1_17;ZapMachine_run1_18;ZapMachine_run1_2;ZapMachine_run1_3;ZapMachine_run1_4;ZapMachine_run1_5;ZapMachine_run1_6;ZapMachine_run1_7;ZapMachine_run1_8;ZapMachine_run1_9";
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
