using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Growl.Destinations;

namespace Notifo_Forwarder
{
    /// <summary>
    /// Forwards notifications to any url using an HTTP POST request.
    /// </summary>
    [Serializable]
    public class NotifoDestination : ForwardDestination
    {
        /// <summary>
        /// Notifo Apikey
        /// </summary>
        private string _Apikey;

        /// <summary>
        /// Notifo Username
        /// </summary>
        private string _Username;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookDestination"/> class.
        /// </summary>
        /// <param name="url">The URL of the webhook</param>
        public NotifoDestination(string name, string url)
            : base(name, true)
        {
            this._Username = name;
            this._Apikey = url;
        }

        /// <summary>
        /// Gets the address display.
        /// </summary>
        /// <value>The address display.</value>
        /// <remarks>
        /// This is shown in GfW as the second line of the item in the 'Forwards' list view.
        /// </remarks>
        public override string AddressDisplay
        {
            get { return this._Apikey; }
        }

        /// <summary>
        /// Gets or sets the URL of the webhook
        /// </summary>
        /// <value>string</value>
        public string Apikey
        {
            get
            {
                return this._Apikey;
            }
            set
            {
                this._Apikey = value;
            }
        }

        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                this._Username = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WebhookDestination"/> is available.
        /// </summary>
        /// <value>Always returns <c>true</c>.</value>
        /// <remarks>
        /// This value is essentially read-only. Setting the value will have no effect.
        /// </remarks>
        public override bool Available
        {
            get
            {
                return true;
            }
            protected set
            {
                //throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Called when a notification is received by GfW.
        /// </summary>
        /// <param name="notification">The notification information</param>
        /// <param name="callbackContext">The callback context.</param>
        /// <param name="requestInfo">The request info.</param>
        /// <param name="isIdle"><c>true</c> if the user is currently idle;<c>false</c> otherwise</param>
        /// <param name="callbackFunction">The function GfW will run if this notification is responded to on the forwarded computer</param>
        /// <remarks>
        /// Unless your forwarder is going to handle socket-style callbacks from the remote computer, you should ignore
        /// the <paramref name="callbackFunction"/> parameter.
        /// </remarks>
        public override void ForwardNotification(Growl.Connector.Notification notification, Growl.Connector.CallbackContext callbackContext, Growl.Connector.RequestInfo requestInfo, bool isIdle, ForwardDestination.ForwardedNotificationCallbackHandler callbackFunction)
        {
            // check for request loop
            if ((notification.ApplicationName == "Notifo") &&
                notification.CustomTextAttributes.ContainsKey("NotifoUsername") &&
                notification.CustomTextAttributes["NotifoUsername"].Equals(this.Username))
            {
                Growl.CoreLibrary.DebugInfo.WriteLine(String.Format("Aborted forwarding due to circular notification (username:{0})", this.Username));
                return;
            }

            try
            {
                WebRequest wr = WebRequest.Create(new Uri("https://api.notifo.com/v1/send_notification"));
                wr.Method = "POST";
                wr.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Username + ":" + this.Apikey)));
                wr.ContentType = "application/x-www-form-urlencoded";
                ((HttpWebRequest)wr).UserAgent = "Growl Notifo Forwarder";

                string body = "msg=" + System.Uri.EscapeDataString(notification.Text);
                body += "&title=" + System.Uri.EscapeDataString(notification.Title);
                body += "&label=" + System.Uri.EscapeDataString(notification.ApplicationName);

                byte[] message = Encoding.ASCII.GetBytes(body);
                wr.GetRequestStream().Write(message, 0, message.Length);
                wr.GetRequestStream().Close();

                WebResponse response = wr.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                string msg = String.Format("Notifo forwarding failed: {0}", ex.Message);
                // System.Windows.Forms.MessageBox.Show(msg);
                // this is an example of writing to the main GfW debug log:
                Growl.CoreLibrary.DebugInfo.WriteLine(msg);
            }
        }

        /// <summary>
        /// Called when an application registration is received by GfW.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="notificationTypes">The notification types.</param>
        /// <param name="requestInfo">The request info.</param>
        /// <param name="isIdle"><c>true</c> if the user is currently idle;<c>false</c> otherwise</param>
        /// <remarks>
        /// Many types of forwarders can just ignore this event.
        /// </remarks>
        public override void ForwardRegistration(Growl.Connector.Application application, List<Growl.Connector.NotificationType> notificationTypes, Growl.Connector.RequestInfo requestInfo, bool isIdle)
        {
            // do nothing
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns><see cref="WebhookDestination"/></returns>
        public override DestinationBase Clone()
        {
            return new NotifoDestination(this.Username, this._Apikey);
        }

        /// <summary>
        /// Gets the icon that represents this type of forwarder.
        /// </summary>
        /// <returns><see cref="System.Drawing.Image"/></returns>
        public override System.Drawing.Image GetIcon()
        {
            return NotifoForwardHandler.GetIcon();
        }
    }
}
