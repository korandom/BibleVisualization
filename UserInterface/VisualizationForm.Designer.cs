namespace UserInterface
{
    partial class VisualizationForm
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
            avaloniaHost = new Avalonia.Win32.Interoperability.WinFormsAvaloniaControlHost();
            SuspendLayout();
            // 
            // avaloniaHost
            // 
            avaloniaHost.Content = null;
            avaloniaHost.Dock = DockStyle.Fill;
            avaloniaHost.Location = new Point(0, 0);
            avaloniaHost.Name = "avaloniaHost";
            avaloniaHost.Size = new Size(800, 450);
            avaloniaHost.TabIndex = 0;
            avaloniaHost.Text = "winFormsAvaloniaControlHost1";
            // 
            // VisualizationForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(avaloniaHost);
            Name = "VisualizationForm";
            Text = "VisualizationForm";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
        }

        #endregion

        private Avalonia.Win32.Interoperability.WinFormsAvaloniaControlHost avaloniaHost;
    }
}