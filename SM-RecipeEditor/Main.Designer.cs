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
            listb_recipeFiles = new ListBox();
            bt_new = new Button();
            cb_file = new ComboBox();
            label2 = new Label();
            p_main.SuspendLayout();
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
            p_main.Controls.Add(listb_recipeFiles);
            p_main.Controls.Add(bt_new);
            p_main.Controls.Add(cb_file);
            p_main.Controls.Add(label2);
            p_main.Location = new Point(0, 29);
            p_main.Name = "p_main";
            p_main.Size = new Size(800, 387);
            p_main.TabIndex = 3;
            // 
            // listb_recipeFiles
            // 
            listb_recipeFiles.FormattingEnabled = true;
            listb_recipeFiles.ItemHeight = 15;
            listb_recipeFiles.Location = new Point(12, 38);
            listb_recipeFiles.Name = "listb_recipeFiles";
            listb_recipeFiles.Size = new Size(151, 334);
            listb_recipeFiles.TabIndex = 3;
            listb_recipeFiles.SelectedValueChanged += OnRecipeSelected;
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
    }
}
