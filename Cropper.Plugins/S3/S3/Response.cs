using System;
using System.Net;
using System.Text;

namespace Cropper.SendToS3.S3
{
	public class Response
	{
		protected WebResponse response;
		public WebResponse Connection
		{
			get
			{
				return response;
			}
		}

		public HttpStatusCode Status
		{
			get
			{
				HttpWebResponse wr = response as HttpWebResponse;
				return wr.StatusCode;
			}
		}

		public string XAmzId
		{
			get
			{
				return response.Headers.Get( "x-amz-id-2" );
			}
		}

		public string XAmzRequestId
		{
			get
			{
				return response.Headers.Get("x-amz-request-id");
			}
		}

		public Response(WebRequest request)
		{
			try
			{
				this.response = request.GetResponse();
			}
			catch (WebException ex)
			{
				string msg = ex.Response != null ? Utils.slurpInputStream(ex.Response.GetResponseStream()) : ex.Message;
				throw new WebException(msg, ex, ex.Status, ex.Response);
			}
		}

		public string getResponseMessage()
		{
			string data = Utils.slurpInputStream( response.GetResponseStream() );
			response.GetResponseStream().Close();
			return data;
		}
	}
}
