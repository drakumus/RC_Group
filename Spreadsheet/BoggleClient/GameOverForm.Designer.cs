namespace BoggleClient
{
    partial class GameOverForm
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
            this.Player1 = new System.Windows.Forms.ListBox();
            this.Player2 = new System.Windows.Forms.ListBox();
            this.score2Label = new System.Windows.Forms.Label();
            this.score1Label = new System.Windows.Forms.Label();
            this.player2Label = new System.Windows.Forms.Label();
            this.player1Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Player1
            // 
            this.Player1.FormattingEnabled = true;
            this.Player1.ItemHeight = 16;
            this.Player1.Location = new System.Drawing.Point(14, 46);
            this.Player1.Name = "Player1";
            this.Player1.Size = new System.Drawing.Size(120, 84);
            this.Player1.TabIndex = 0;
            // 
            // Player2
            // 
            this.Player2.FormattingEnabled = true;
            this.Player2.ItemHeight = 16;
            this.Player2.Location = new System.Drawing.Point(155, 46);
            this.Player2.Name = "Player2";
            this.Player2.Size = new System.Drawing.Size(120, 84);
            this.Player2.TabIndex = 1;
            // 
            // score2Label
            // 
            this.score2Label.AutoSize = true;
            this.score2Label.Location = new System.Drawing.Point(207, 26);
            this.score2Label.Name = "score2Label";
            this.score2Label.Size = new System.Drawing.Size(16, 17);
            this.score2Label.TabIndex = 12;
            this.score2Label.Text = "0";
            // 
            // score1Label
            // 
            this.score1Label.AutoSize = true;
            this.score1Label.Location = new System.Drawing.Point(10, 26);
            this.score1Label.Name = "score1Label";
            this.score1Label.Size = new System.Drawing.Size(16, 17);
            this.score1Label.TabIndex = 11;
            this.score1Label.Text = "0";
            // 
            // player2Label
            // 
            this.player2Label.AutoSize = true;
            this.player2Label.Location = new System.Drawing.Point(207, 9);
            this.player2Label.Name = "player2Label";
            this.player2Label.Size = new System.Drawing.Size(56, 17);
            this.player2Label.TabIndex = 10;
            this.player2Label.Text = "Player2";
            // 
            // player1Label
            // 
            this.player1Label.AutoSize = true;
            this.player1Label.Location = new System.Drawing.Point(10, 9);
            this.player1Label.Name = "player1Label";
            this.player1Label.Size = new System.Drawing.Size(56, 17);
            this.player1Label.TabIndex = 9;
            this.player1Label.Text = "Player1";
            // 
            // GameOverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 148);
            this.Controls.Add(this.score2Label);
            this.Controls.Add(this.score1Label);
            this.Controls.Add(this.player2Label);
            this.Controls.Add(this.player1Label);
            this.Controls.Add(this.Player2);
            this.Controls.Add(this.Player1);
            this.Name = "GameOverForm";
            this.Text = "GameOverForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox Player1;
        private System.Windows.Forms.ListBox Player2;
        private System.Windows.Forms.Label score2Label;
        private System.Windows.Forms.Label score1Label;
        private System.Windows.Forms.Label player2Label;
        private System.Windows.Forms.Label player1Label;
    }
}