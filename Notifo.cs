using System;
using System.Collections.Generic;
using System.Text;
using Growl.Destinations;

namespace Notifo_Forwarder
{
    public class NotifoInputs : DestinationSettingsPanel
    {
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Label labelName;
        private Growl.Destinations.HighlightTextBox textBoxName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private Growl.Destinations.HighlightTextBox textBoxApi;

        public NotifoInputs()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.textBoxApi = new Growl.Destinations.HighlightTextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.textBoxName = new Growl.Destinations.HighlightTextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panelDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.linkLabel1);
            this.panelDetails.Controls.Add(this.labelName);
            this.panelDetails.Controls.Add(this.textBoxName);
            this.panelDetails.Controls.Add(this.labelUrl);
            this.panelDetails.Controls.Add(this.textBoxApi);
            // 
            // textBoxApi
            // 
            this.textBoxApi.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(250)))), ((int)(((byte)(184)))));
            this.textBoxApi.Location = new System.Drawing.Point(83, 42);
            this.textBoxApi.Name = "textBoxApi";
            this.textBoxApi.Size = new System.Drawing.Size(227, 20);
            this.textBoxApi.TabIndex = 2;
            this.textBoxApi.TextChanged += new System.EventHandler(this.textBoxUrl_TextChanged);
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(19, 45);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(61, 13);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.Text = "API Secret:";
            // 
            // textBoxName
            // 
            this.textBoxName.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(250)))), ((int)(((byte)(184)))));
            this.textBoxName.Location = new System.Drawing.Point(83, 16);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(227, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(19, 19);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(58, 13);
            this.labelName.TabIndex = 3;
            this.labelName.Text = "Username:";
            // 
            // linkLabel1
            // 
            string desc1 = "Forwards notification to ";
            string desc2 = "Notifo";
            string desc3 = ". See your ";
            string desc4 = "settings page";
            string desc5 = " for your API secret.";
            this.linkLabel1.Location = new System.Drawing.Point(22, 72);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(288, 56);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = desc1 + desc2 + desc3 + desc4 + desc5;
            this.linkLabel1.Links.Add(desc1.Length, desc2.Length, "http://www.notifo.com");
            this.linkLabel1.Links.Add(desc1.Length + desc2.Length + desc3.Length, desc4.Length, "http://www.notifo.com/user/settings");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // WebhookInputs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "WebhookInputs";
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Initializes the configuration UI when a webhook is being added or edited.
        /// </summary>
        /// <param name="isSubscription">will always be <c>false</c> for <see cref="ForwardDestination"/>s</param>
        /// <param name="dli">The <see cref="DestinationListItem"/> that the user selected</param>
        /// <param name="db">The <see cref="DestinationBase"/> of the item if it is being edited;<c>null</c> otherwise</param>
        /// <remarks>
        /// When an instance is being edited (<paramref name="dli"/> != null), make sure to repopulate any
        /// inputs with the current values.
        /// 
        /// By default, the 'Save' button is disabled and you must call <see cref="DestinationSettingsPanel.OnValidChanged"/>
        /// in order to enable it when appropriate.
        /// </remarks>
        public override void Initialize(bool isSubscription, DestinationListItem dli, DestinationBase db)
        {
            // set text box values
            this.textBoxName.Text = String.Empty;
            this.textBoxName.Enabled = true;
            this.textBoxApi.Text = String.Empty;
            this.textBoxApi.Enabled = true;

            NotifoDestination whd = db as NotifoDestination;
            if (whd != null)
            {
                this.textBoxName.Text = whd.Username;
                this.textBoxApi.Text = whd.Apikey;
            }

            ValidateInputs();

            this.textBoxName.Focus();
        }

        /// <summary>
        /// Creates a new instance of the webhook forwarder.
        /// </summary>
        /// <returns>New <see cref="WebhookDestination"/></returns>
        /// <remarks>
        /// This is called when the user is adding a new destination and clicks the 'Save' button.
        /// </remarks>
        public override DestinationBase Create()
        {
            return new NotifoDestination(this.textBoxName.Text, this.textBoxApi.Text);
        }

        /// <summary>
        /// Updates the specified webhook instance.
        /// </summary>
        /// <param name="db">The <see cref="WebhookDestination"/> to update</param>
        /// <remarks>
        /// This is called when a user is editing an existing webhook and clicks the 'Save' button.
        /// </remarks>
        public override void Update(DestinationBase db)
        {
            NotifoDestination whd = db as NotifoDestination;
            whd.Username = this.textBoxName.Text;
            whd.Apikey = this.textBoxApi.Text;
        }


        private void ValidateInputs()
        {
            bool valid = true;

            // name
            if (String.IsNullOrEmpty(this.textBoxName.Text))
            {
                this.textBoxName.Highlight();
                valid = false;
            }
            else
            {
                this.textBoxName.Unhighlight();
            }

            // apikey
            if (String.IsNullOrEmpty(this.textBoxApi.Text))
            {
                this.textBoxApi.Highlight();
                valid = false;
            }
            //else
            //{
            //    Uri uri = null;
            //    bool ok = Uri.TryCreate(this.textBoxApi.Text, UriKind.Absolute, out uri);
            //    if (!ok)
            //    {
            //        this.textBoxApi.Highlight();
            //        valid = false;
            //    }
            //    else
            //    {
            //        this.textBoxApi.Unhighlight();
            //    }
            //}

            OnValidChanged(valid);
        }

        private void textBoxUrl_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            ValidateInputs();
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
