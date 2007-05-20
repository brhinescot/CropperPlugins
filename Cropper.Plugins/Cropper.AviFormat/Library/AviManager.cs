/* This class has been written by
 * Corinna John (Hannover, Germany)
 * cj@binary-universe.net
 * 
 * You may do with this code whatever you like,
 * except selling it or claiming any rights/ownership.
 * 
 * Please send me a little feedback about what you're
 * using this code for and what changes you'd like to
 * see in later versions. (And please excuse my bad english.)
 * 
 * WARNING: This is experimental code.
 * Please do not expect "Release Quality".
 * */

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AviFile
{
	public class AviManager
	{
		private int aviFile = 0;
		private ArrayList streams = new ArrayList();

		/// <summary>Open or create an AVI file</summary>
		/// <param name="fileName">Name of the AVI file</param>
		/// <param name="open">true: Open the file; false: Create or overwrite the file</param>
		public AviManager(String fileName, bool open)
		{
			Avi.AVIFileInit();
			int result;

			if (open)
			{
				//open existing file

				result = Avi.AVIFileOpen(
					ref aviFile, fileName,
					Avi.OF_READWRITE, 0);
			}
			else
			{
				//create empty file

				result = Avi.AVIFileOpen(
					ref aviFile, fileName,
					Avi.OF_WRITE | Avi.OF_CREATE, 0);
			}

			if (result != 0)
			{
				throw new Exception("Exception in AVIFileOpen: " + result.ToString());
			}
		}

		/// <summary>Get the first video stream - usually there is only one video stream</summary>
		/// <returns>VideoStream object for the stream</returns>
		public VideoStream GetVideoStream()
		{
			IntPtr aviStream;

			int result = Avi.AVIFileGetStream(
				aviFile,
				out aviStream,
				Avi.streamtypeVIDEO, 0);

			if (result != 0)
			{
				throw new Exception("Exception in AVIFileGetStream: " + result.ToString());
			}

			VideoStream stream = new VideoStream(aviFile, aviStream);
			streams.Add(stream);
			return stream;
		}

		/// <summary>Getthe first wave audio stream</summary>
		/// <returns>AudioStream object for the stream</returns>
		public AudioStream GetWaveStream()
		{
			IntPtr aviStream;

			int result = Avi.AVIFileGetStream(
				aviFile,
				out aviStream,
				Avi.streamtypeAUDIO, 0);

			if (result != 0)
			{
				throw new Exception("Exception in AVIFileGetStream: " + result.ToString());
			}

			AudioStream stream = new AudioStream(aviFile, aviStream);
			streams.Add(stream);
			return stream;
		}

		/// <summary>Get a stream from the internal list of opened streams</summary>
		/// <param name="index">Index of the stream. The streams are not sorted, the first stream is the one that was opened first.</param>
		/// <returns>VideoStream at position [index]</returns>
		/// <remarks>
		/// Use this method after DecompressToNewFile,
		/// to get the copied stream from the new AVI file
		/// </remarks>
		/// <example>
		/// //streams cannot be edited - copy to a new file
		///	AviManager newManager = aviStream.DecompressToNewFile(@"..\..\testdata\temp.avi", true);
		/// //there is only one stream in the new file - get it and add a frame
		///	VideoStream aviStream = newManager.GetOpenStream(0);
		///	aviStream.AddFrame(bitmap);
		/// </example>
		public VideoStream GetOpenStream(int index)
		{
			return (VideoStream) streams[index];
		}

		/// <summary>Add an empty video stream to the file</summary>
		/// <param name="isCompressed">true: Create a compressed stream before adding frames</param>
		/// <param name="frameRate">Frames per second</param>
		/// <param name="frameSize">Size of one frame in bytes</param>
		/// <param name="width">Width of each image</param>
		/// <param name="height">Height of each image</param>
		/// <param name="format">PixelFormat of the images</param>
		/// <returns>VideoStream object for the new stream</returns>
		public VideoStream AddVideoStream(bool isCompressed, int frameRate, int frameSize, int width, int height,
		                                  PixelFormat format)
		{
			VideoStream stream = new VideoStream(aviFile, isCompressed, frameRate, frameSize, width, height, format);
			streams.Add(stream);
			return stream;
		}

		/// <summary>Add an empty video stream to the file</summary>
		/// <param name="isCompressed">true: Create a compressed stream before adding frames</param>
		/// <param name="frameRate">Frames per second</param>
		/// <param name="firstFrame">Image to write into the stream as the first frame</param>
		/// <returns>VideoStream object for the new stream</returns>
		public VideoStream AddVideoStream(bool isCompressed, int frameRate, Bitmap firstFrame)
		{
			VideoStream stream = new VideoStream(aviFile, isCompressed, frameRate, firstFrame);
			streams.Add(stream);
			return stream;
		}

		/// <summary>Add a wave audio stream from another file to this file</summary>
		/// <param name="waveFileName">Name of the wave file to add</param>
		public void AddAudioStream(String waveFileName)
		{
			AviManager audioManager = new AviManager(waveFileName, true);
			AudioStream newStream = audioManager.GetWaveStream();
			AddAudioStream(newStream);
			audioManager.Close();
		}

		/// <summary>Add an existing wave audio stream to the file</summary>
		/// <param name="newStream">The stream to add</param>
		public void AddAudioStream(AudioStream newStream)
		{
			Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
			Avi.PCMWAVEFORMAT streamFormat = new Avi.PCMWAVEFORMAT();
			int streamLength = 0;
			IntPtr waveData = newStream.GetStreamData(ref streamInfo, ref streamFormat, ref streamLength);

			IntPtr aviStream;
			int result = Avi.AVIFileCreateStream(aviFile, out aviStream, ref streamInfo);
			if (result != 0)
			{
				throw new Exception("Exception in AVIFileCreateStream: " + result.ToString());
			}

			result = Avi.AVIStreamSetFormat(aviStream, 0, ref streamFormat, Marshal.SizeOf(streamFormat));
			if (result != 0)
			{
				throw new Exception("Exception in AVIStreamSetFormat: " + result.ToString());
			}

			result = Avi.AVIStreamWrite(aviStream, 0, streamLength, waveData, streamLength, Avi.AVIIF_KEYFRAME, 0, 0);
			if (result != 0)
			{
				throw new Exception("Exception in AVIStreamWrite: " + result.ToString());
			}

			result = Avi.AVIStreamRelease(aviStream);
			if (result != 0)
			{
				throw new Exception("Exception in AVIStreamRelease: " + result.ToString());
			}
		}

		/// <summary>Release all ressources</summary>
		public void Close()
		{
			foreach (AviStream stream in streams)
			{
				stream.Close();
			}

			Avi.AVIFileRelease(aviFile);
			Avi.AVIFileExit();
		}
	}
}