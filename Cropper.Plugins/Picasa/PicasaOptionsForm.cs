using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Http;
using System.Linq;
using System.ServiceModel.Syndication;
using Fusion8.Cropper.Extensibility;
using RE=System.Text.RegularExpressions;

using CropperPlugins.Utils;

namespace Cropper.SendToPicasa
{
    public partial class PicasaOptionsForm : BaseConfigurationForm
    {
        private PicasaSettings _settings;

        public PicasaOptionsForm(PicasaSettings settings)
        {
            Tracing.Trace("PicasaOptionsForm: ctor");

            InitializeComponent();

            _settings                        = settings;
            this.txtEmailAddress.Text        = settings.EmailAddress;
            this.txtAllPhotosComment.Text    = settings.AllPhotosComment;
            this.cmbImageFormat.SelectedItem = settings.ImageFormat;
            this.qualitySlider.Value         = settings.JpgImageQuality;
            this.chkUseFixedComment.Checked  = settings.UseFixedComment;
            this.chkPopBrowser.Checked       = settings.PopBrowser;

            this.cmbAlbum.Items.Clear();
            this.cmbAlbum.Items.Add(settings.Album);
            this.cmbAlbum.SelectedItem       = settings.Album;
            this.cmbAlbum.DisplayMember      = "Name";
            this.cmbAlbum.ValueMember        = "Id";

            HandleQualitySliderValueChanged(null,null);
            SelectedImageFormatChanged(null,null);
            UseFixedCommentCheckedChanged(null,null);
        }


        public void ApplySettings()
        {
            _settings.EmailAddress     = this.txtEmailAddress.Text.Trim();
            _settings.AllPhotosComment = this.txtAllPhotosComment.Text.Trim();
            _settings.UseFixedComment  = this.chkUseFixedComment.Checked;
            _settings.PopBrowser       = this.chkPopBrowser.Checked;
            _settings.JpgImageQuality  = this.qualitySlider.Value;
            _settings.ImageFormat      = this.cmbImageFormat.Text;
            _settings.Album            = (this.cmbAlbum.SelectedItem as AlbumItem);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }


        /// <summary>
        ///   Display the qualitySlider only if the format is jpg.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This implies that we need to move the checkbox for
        ///     popping the browser, also.
        ///   </para>
        /// </remarks>
        private void SelectedImageFormatChanged(object sender, EventArgs e)
        {
            if (this.cmbImageFormat.Text == "jpg")
            {
                qualitySlider.Visible = true;
                qualitySlider.Enabled = true;
                this.chkPopBrowser.Location = new System.Drawing.Point(114, 216);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 220);
            }
            else
            {
                qualitySlider.Visible = false;
                qualitySlider.Enabled = false;
                this.chkPopBrowser.Location = new System.Drawing.Point(114, 178);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 182);
            }
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }

        private void UseFixedCommentCheckedChanged(object sender, System.EventArgs e)
        {
            this.lblAllPhotosComment.Enabled = this.chkUseFixedComment.Checked;
            this.txtAllPhotosComment.Enabled = this.chkUseFixedComment.Checked;
        }

        private void btnRefreshAlbumList_Click(object sender, System.EventArgs e)
        {
            var c = this.Cursor;
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            string username = null;
            try
            {
                username =
                    GdataSession.Authenticate(txtEmailAddress.Text, "picasa");
            }
            finally
            {
                this.Cursor = c;
            }

            if (String.IsNullOrEmpty(username))
                return;

            txtEmailAddress.Text = username;

            cmbAlbum.Items.Clear();

            // To get a list of albums:
            // GET https://picasaweb.google.com/data/feed/api/user/userID

            var picasa = new HttpClient("https://picasaweb.google.com/");

            var headers =
                GdataSession.Instance.GetHeaders(username, "picasa");

            // get a list of albums
            var response = picasa.Send(HttpMethod.GET,
                                       "/data/feed/api/user/default",
                                       headers);

            response.EnsureStatusIsSuccessful();
            Tracing.Trace("PicasaOptionsForm: Response is success");

            var feed = response.Content.ReadAsSyndicationFeed();

            Tracing.Trace("PicasaOptionsForm: found {0} items", feed.Items.Count());

            var selection = from i in feed.Items
                where i.Title.Text == "Drop Box"
                select i;

            bool foundDropBox = (selection.Count() == 1);
            if (!foundDropBox)
            {
                Tracing.Trace("PicasaOptionsForm: no drop box - adding it");
                cmbAlbum.Items.Add( new AlbumItem
                    {
                        Name = "Drop Box",
                        Id = "default"
                    });
            }

            foreach (var item in feed.Items)
            {
                if (foundDropBox || item.Title.Text != "Drop Box")
                {
                    Tracing.Trace("PicasaOptionsForm: album({0}) id({1})",
                                  item.Title.Text,
                                  RE.Regex.Replace(item.Id, ".*/([^/]+)", "$1"));

                    cmbAlbum.Items.Add(new AlbumItem
                        {
                            Name = item.Title.Text,
                            Id = RE.Regex.Replace(item.Id, ".*/([^/]+)", "$1")
                        });
                }
            }
        }
    }
}
