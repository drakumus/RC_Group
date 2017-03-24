namespace BoggleClient
{
    partial class GameOverWindow
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
            this.player1Box = new System.Windows.Forms.ListBox();
            this.player2Box = new System.Windows.Forms.ListBox();
            this.player2ScoreLabel = new System.Windows.Forms.Label();
            this.player1ScoreLabel = new System.Windows.Forms.Label();
            this.player2Label = new System.Windows.Forms.Label();
            this.player1Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // player1Box
            // 
            this.player1Box.FormattingEnabled = true;
            this.player1Box.ItemHeight = 16;
            this.player1Box.Location = new System.Drawing.Point(14, 46);
            this.player1Box.Name = "player1Box";
            this.player1Box.Size = new System.Drawing.Size(120, 84);
            this.player1Box.TabIndex = 0;
            // 
            // player2Box
            // 
            this.player2Box.FormattingEnabled = true;
            this.player2Box.ItemHeight = 16;
            this.player2Box.Location = new System.Drawing.Point(155, 46);
            this.player2Box.Name = "player2Box";
            this.player2Box.Size = new System.Drawing.Size(120, 84);
            this.player2Box.TabIndex = 1;
            // 
            // player2ScoreLabel
            // 
            this.player2ScoreLabel.AutoSize = true;
            this.player2ScoreLabel.Location = new System.Drawing.Point(152, 26);
            this.player2ScoreLabel.Name = "player2ScoreLabel";
            this.player2ScoreLabel.Size = new System.Drawing.Size(16, 17);
            this.player2ScoreLabel.TabIndex = 12;
            this.player2ScoreLabel.Text = "0";
            // 
            // player1ScoreLabel
            // 
            this.player1ScoreLabel.AutoSize = true;
            this.player1ScoreLabel.Location = new System.Drawing.Point(10, 26);
            this.player1ScoreLabel.Name = "player1ScoreLabel";
            this.player1ScoreLabel.Size = new System.Drawing.Size(16, 17);
            this.player1ScoreLabel.TabIndex = 11;
            this.player1ScoreLabel.Text = "0";
            // 
            // player2Label
            // 
            this.player2Label.AutoSize = true;
            this.player2Label.Location = new System.Drawing.Point(152, 9);
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
            // GameOverWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 148);
            this.Controls.Add(this.player2ScoreLabel);
            this.Controls.Add(this.player1ScoreLabel);
            this.Controls.Add(this.player2Label);
            this.Controls.Add(this.player1Label);
            this.Controls.Add(this.player2Box);
            this.Controls.Add(this.player1Box);
            this.Name = "GameOverWindow";
            this.Text = "GameOverForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox player1Box;
        private System.Windows.Forms.ListBox player2Box;
        private System.Windows.Forms.Label player2ScoreLabel;
        private System.Windows.Forms.Label player1ScoreLabel;
        private System.Windows.Forms.Label player2Label;
        private System.Windows.Forms.Label player1Label;
    }
}