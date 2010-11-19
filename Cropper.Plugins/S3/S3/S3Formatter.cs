using System;

namespace Cropper.SendToS3.S3
{
	internal class S3Formatter
	{
		private static string TIMESTAMP_FORMAT = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

		/// <summary>
		/// Converts the date to an ISO-8601, resolved to milliseconds.
		/// </summary>
		public static string FormatAsISO8601(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString(TIMESTAMP_FORMAT,
				System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets time in the DateTime format, resolved to milliseconds.
		/// </summary>
		public static DateTime GetCurrentTimeResolvedToMillis()
		{
			DateTime dateTime = DateTime.Now;

			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
				dateTime.Hour, dateTime.Minute, dateTime.Second,
				dateTime.Millisecond
				);
		}
	}
}
