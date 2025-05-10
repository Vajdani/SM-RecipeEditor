namespace SM_RecipeEditor
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cb_mod = new ComboBox();
            label1 = new Label();
            p_main = new Panel();
            p_edit = new Panel();
            p_extras = new Panel();
            dgv_extras = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            label7 = new Label();
            bt_save = new Button();
            tx_craftTime = new TextBox();
            label5 = new Label();
            tx_quantity = new TextBox();
            label4 = new Label();
            label3 = new Label();
            cb_item = new ComboBox();
            listb_recipeFiles = new ListBox();
            bt_new = new Button();
            cb_file = new ComboBox();
            label2 = new Label();
            p_ingredients = new Panel();
            dgv_ingredients = new DataGridView();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            label6 = new Label();
            p_main.SuspendLayout();
            p_edit.SuspendLayout();
            p_extras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_extras).BeginInit();
            p_ingredients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_ingredients).BeginInit();
            SuspendLayout();
            // 
            // cb_mod
            // 
            cb_mod.FormattingEnabled = true;
            cb_mod.Location = new Point(56, 6);
            cb_mod.Name = "cb_mod";
            cb_mod.Size = new Size(332, 23);
            cb_mod.TabIndex = 0;
            cb_mod.SelectedValueChanged += OnModSelected;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(35, 15);
            label1.TabIndex = 1;
            label1.Text = "Mod:";
            // 
            // p_main
            // 
            p_main.Controls.Add(p_edit);
            p_main.Controls.Add(listb_recipeFiles);
            p_main.Controls.Add(bt_new);
            p_main.Controls.Add(cb_file);
            p_main.Controls.Add(label2);
            p_main.Location = new Point(0, 29);
            p_main.Name = "p_main";
            p_main.Size = new Size(800, 387);
            p_main.TabIndex = 3;
            // 
            // p_edit
            // 
            p_edit.Controls.Add(p_ingredients);
            p_edit.Controls.Add(p_extras);
            p_edit.Controls.Add(bt_save);
            p_edit.Controls.Add(tx_craftTime);
            p_edit.Controls.Add(label5);
            p_edit.Controls.Add(tx_quantity);
            p_edit.Controls.Add(label4);
            p_edit.Controls.Add(label3);
            p_edit.Controls.Add(cb_item);
            p_edit.Location = new Point(169, 38);
            p_edit.Name = "p_edit";
            p_edit.Size = new Size(619, 334);
            p_edit.TabIndex = 4;
            // 
            // p_extras
            // 
            p_extras.Controls.Add(dgv_extras);
            p_extras.Controls.Add(label7);
            p_extras.Location = new Point(305, 94);
            p_extras.Name = "p_extras";
            p_extras.Size = new Size(311, 209);
            p_extras.TabIndex = 8;
            // 
            // dgv_extras
            // 
            dgv_extras.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_extras.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dgv_extras.Location = new Point(11, 18);
            dgv_extras.Name = "dgv_extras";
            dgv_extras.Size = new Size(289, 191);
            dgv_extras.TabIndex = 12;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Item";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 170;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Quantity";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 75;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 0);
            label7.Name = "label7";
            label7.Size = new Size(40, 15);
            label7.TabIndex = 11;
            label7.Text = "Extras:";
            // 
            // bt_save
            // 
            bt_save.Location = new Point(496, 308);
            bt_save.Name = "bt_save";
            bt_save.Size = new Size(120, 23);
            bt_save.TabIndex = 7;
            bt_save.Text = "Save changes";
            bt_save.UseVisualStyleBackColor = true;
            bt_save.Click += bt_save_Click;
            // 
            // tx_craftTime
            // 
            tx_craftTime.Location = new Point(79, 65);
            tx_craftTime.Name = "tx_craftTime";
            tx_craftTime.Size = new Size(121, 23);
            tx_craftTime.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 68);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 5;
            label5.Text = "Craft time:";
            // 
            // tx_quantity
            // 
            tx_quantity.Location = new Point(79, 36);
            tx_quantity.Name = "tx_quantity";
            tx_quantity.Size = new Size(121, 23);
            tx_quantity.TabIndex = 4;
            tx_quantity.TextChanged += RecipeNumberChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 39);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 3;
            label4.Text = "Quantity:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(10, 10);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 1;
            label3.Text = "Item:";
            // 
            // cb_item
            // 
            cb_item.FormattingEnabled = true;
            cb_item.Location = new Point(79, 7);
            cb_item.Name = "cb_item";
            cb_item.Size = new Size(121, 23);
            cb_item.TabIndex = 0;
            cb_item.SelectedValueChanged += RecipeItemChanged;
            // 
            // listb_recipeFiles
            // 
            listb_recipeFiles.FormattingEnabled = true;
            listb_recipeFiles.ItemHeight = 15;
            listb_recipeFiles.Location = new Point(12, 38);
            listb_recipeFiles.Name = "listb_recipeFiles";
            listb_recipeFiles.Size = new Size(151, 334);
            listb_recipeFiles.TabIndex = 3;
            listb_recipeFiles.SelectedIndexChanged += OnRecipeSelected;
            // 
            // bt_new
            // 
            bt_new.Location = new Point(179, 9);
            bt_new.Name = "bt_new";
            bt_new.Size = new Size(120, 23);
            bt_new.TabIndex = 2;
            bt_new.Text = "Create new file";
            bt_new.UseVisualStyleBackColor = true;
            bt_new.Click += bt_new_Click;
            // 
            // cb_file
            // 
            cb_file.FormattingEnabled = true;
            cb_file.Location = new Point(56, 9);
            cb_file.Name = "cb_file";
            cb_file.Size = new Size(117, 23);
            cb_file.TabIndex = 1;
            cb_file.SelectedValueChanged += OnFileSelected;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 12);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 0;
            label2.Text = "File:";
            // 
            // p_ingredients
            // 
            p_ingredients.Controls.Add(dgv_ingredients);
            p_ingredients.Controls.Add(label6);
            p_ingredients.Location = new Point(10, 94);
            p_ingredients.Name = "p_ingredients";
            p_ingredients.Size = new Size(289, 209);
            p_ingredients.TabIndex = 9;
            // 
            // dgv_ingredients
            // 
            dgv_ingredients.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_ingredients.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            dgv_ingredients.Location = new Point(0, 18);
            dgv_ingredients.Name = "dgv_ingredients";
            dgv_ingredients.Size = new Size(289, 191);
            dgv_ingredients.TabIndex = 12;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Item";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 170;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Quantity";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 75;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(0, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 15);
            label6.TabIndex = 11;
            label6.Text = "Ingredients:";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 415);
            Controls.Add(p_main);
            Controls.Add(label1);
            Controls.Add(cb_mod);
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Recipe Editor";
            p_main.ResumeLayout(false);
            p_main.PerformLayout();
            p_edit.ResumeLayout(false);
            p_edit.PerformLayout();
            p_extras.ResumeLayout(false);
            p_extras.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_extras).EndInit();
            p_ingredients.ResumeLayout(false);
            p_ingredients.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_ingredients).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cb_mod;
        private Label label1;
        private Panel p_main;
        private ComboBox cb_file;
        private Label label2;
        private Button bt_new;
        private ListBox listb_recipeFiles;
        private Panel p_edit;
        private Label label3;
        private ComboBox cb_item;
        private Label label4;
        private TextBox tx_craftTime;
        private Label label5;
        private TextBox tx_quantity;
        private Button bt_save;
        private Panel p_extras;
        private DataGridView dgv_extras;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Label label7;
        private Panel p_ingredients;
        private DataGridView dgv_ingredients;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Label label6;
    }
}
