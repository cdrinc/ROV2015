namespace CServe
{
    partial class Form1
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
            this.startListening = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sendData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startListening
            // 
            this.startListening.Location = new System.Drawing.Point(8, 49);
            this.startListening.Margin = new System.Windows.Forms.Padding(2);
            this.startListening.Name = "startListening";
            this.startListening.Size = new System.Drawing.Size(169, 57);
            this.startListening.TabIndex = 2;
            this.startListening.Text = "Start Listening";
            this.startListening.UseVisualStyleBackColor = true;
            this.startListening.Click += new System.EventHandler(this.startListening_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 3;
            // 
            // sendData
            // 
            this.sendData.Location = new System.Drawing.Point(86, 165);
            this.sendData.Name = "sendData";
            this.sendData.Size = new System.Drawing.Size(75, 23);
            this.sendData.TabIndex = 4;
            this.sendData.Text = "Send data";
            this.sendData.UseVisualStyleBackColor = true;
            this.sendData.Click += new System.EventHandler(this.sendData_click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 319);
            this.Controls.Add(this.sendData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startListening);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "CS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startListening;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendData;
    }
}

