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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AviFile
{
	public class VideoStream : AviStream
	{
		/// <summary>handle for AVIStreamGetFrame</summary>
		private int getFrameObject;

		/// <summary>size of an imge in bytes, stride*height</summary>
		private int frameSize;

		private int frameRate;

		public int FrameRate
		{
			get { return frameRate; }
		}

		private int width;

		public int Width
		{
			get { return width; }
		}

		private int height;

		public int Height
		{
			get { return height; }
		}

		private Int16 countBitsPerPixel;

		public int CountBitsPerPixel
		{
			get { return countBitsPerPixel; }
		}

		/// <summary>count of frames in the stream</summary>
		private int countFrames = 0;

		public int CountFrames
		{
			get { return countFrames; }
		}

		/// <summary>Initialize an empty VideoStream</summary>
		/// <param name="aviFile">The file that contains the stream</param>
		/// <param name="writeCompressed">true: Create a compressed stream before adding frames</param>
		/// <param name="frameRate">Frames per second</param>
		/// <param name="frameSize">Size of one frame in bytes</param>
		/// <param name="width">Width of each image</param>
		/// <param name="height">Height of each image</param>
		/// <param name="format">PixelFormat of the images</param>
		public VideoStream(int aviFile, bool writeCompressed, int frameRate, int frameSize, int width, int height,
		                   PixelFormat format)
		{
			base.aviFile = aviFile;
			base.writeCompressed = writeCompressed;
			this.frameRate = frameRate;
			this.frameSize = frameSize;
			this.width = width;
			this.height = height;
			countBitsPerPixel = ConvertPixelFormatToBitCount(format);

			CreateStream();
		}

		/// <summary>Initialize a new VideoStream and add the first frame</summary>
		/// <param name="aviFile">The file that contains the stream</param>
		/// <param name="writeCompressed">true: create a compressed stream before adding frames</param>
		/// <param name="frameRate">Frames per second</param>
		/// <param name="firstFrame">Image to write into the stream as the first frame</param>
		public VideoStream(int aviFile, bool writeCompressed, int frameRate, Bitmap firstFrame)
		{
			base.aviFile = aviFile;
			base.writeCompressed = writeCompressed;
			this.frameRate = frameRate;

			BitmapData bmpData = firstFrame.LockBits(new Rectangle(
			                                         	0, 0, firstFrame.Width, firstFrame.Height),
			                                         ImageLockMode.ReadOnly, firstFrame.PixelFormat);

			frameSize = bmpData.Stride*bmpData.Width;
			width = firstFrame.Width;
			height = firstFrame.Height;
			countBitsPerPixel = ConvertPixelFormatToBitCount(firstFrame.PixelFormat);

			firstFrame.UnlockBits(bmpData);

			CreateStream();
			AddFrame(firstFrame);
		}

		/// <summary>Initialize a VideoStream for an existing stream</summary>
		/// <param name="aviFile">The file that contains the stream</param>
		/// <param name="aviStream">An IAVISTREAM from [aviFile]</param>
		public VideoStream(int aviFile, IntPtr aviStream)
		{
			base.aviFile = aviFile;
			base.aviStream = aviStream;

			Avi.BITMAPINFOHEADER bih = new Avi.BITMAPINFOHEADER();
			int size = Marshal.SizeOf(bih);
			Avi.AVIStreamReadFormat(aviStream, 0, ref bih, ref size);
			Avi.AVISTREAMINFO streamInfo = GetStreamInfo(aviStream);

			frameRate = streamInfo.dwRate/streamInfo.dwScale;
			width = (int) streamInfo.rcFrame.right;
			height = (int) streamInfo.rcFrame.bottom;
			frameSize = bih.biSizeImage;
			countBitsPerPixel = bih.biBitCount;

			int firstFrame = Avi.AVIStreamStart(aviStream.ToInt32());
			countFrames = firstFrame + Avi.AVIStreamLength(aviStream.ToInt32());
		}

		/// <summary>Get the count of bits per pixel from a PixelFormat value</summary>
		/// <param name="format">One of the PixelFormat members beginning with "Format..." - all others are not supported</param>
		/// <returns>bit count</returns>
		private Int16 ConvertPixelFormatToBitCount(PixelFormat format)
		{
			String formatName = format.ToString();
			if (formatName.Substring(0, 6) != "Format")
			{
				throw new Exception("Unknown pixel format: " + formatName);
			}

			formatName = formatName.Substring(6, 2);
			Int16 bitCount = 0;
			if (Char.IsNumber(formatName[1]))
			{
				//16, 32, 48
				bitCount = Int16.Parse(formatName);
			}
			else
			{
				//4, 8
				bitCount = Int16.Parse(formatName[0].ToString());
			}

			return bitCount;
		}

		/// <summary>Returns a PixelFormat value for a specific bit count</summary>
		/// <param name="bitCount">count of bits per pixel</param>
		/// <returns>A PixelFormat value for [bitCount]</returns>
		private PixelFormat ConvertBitCountToPixelFormat(int bitCount)
		{
			String formatName;
			if (bitCount > 16)
			{
				formatName = String.Format("Format{0}bppRgb", bitCount);
			}
			else if (bitCount == 16)
			{
				formatName = "Format16bppRgb555";
			}
			else
			{
				// < 16
				formatName = String.Format("Format{0}bppIndexed", bitCount);
			}

			return (PixelFormat) Enum.Parse(typeof (PixelFormat), formatName);
		}

		private Avi.AVISTREAMINFO GetStreamInfo(IntPtr aviStream)
		{
			Avi.AVISTREAMINFO streamInfo = new Avi.AVISTREAMINFO();
			int result = Avi.AVIStreamInfo(aviStream.ToInt32(), ref streamInfo, Marshal.SizeOf(streamInfo));
			if (result != 0)
			{
				throw new Exception("Exception in VideoStreamInfo: " + result.ToString());
			}
			return streamInfo;
		}

		/// <summary>Create a new stream</summary>
		private void CreateStream()
		{
			Avi.AVISTREAMINFO strhdr = new Avi.AVISTREAMINFO();
			strhdr.fccType = Avi.mmioStringToFOURCC("vids", 0);
			strhdr.fccHandler = Avi.mmioStringToFOURCC("CVID", 0);
			strhdr.dwFlags = 0;
			strhdr.dwCaps = 0;
			strhdr.wPriority = 0;
			strhdr.wLanguage = 0;
			strhdr.dwScale = 1;
			strhdr.dwRate = frameRate; // Frames per Second
			strhdr.dwStart = 0;
			strhdr.dwLength = 0;
			strhdr.dwInitialFrames = 0;
			strhdr.dwSuggestedBufferSize = frameSize; //height_ * stride_;
			strhdr.dwQuality = -1; //default
			strhdr.dwSampleSize = 0;
			strhdr.rcFrame.top = 0;
			strhdr.rcFrame.left = 0;
			strhdr.rcFrame.bottom = (uint) height;
			strhdr.rcFrame.right = (uint) width;
			strhdr.dwEditCount = 0;
			strhdr.dwFormatChangeCount = 0;
			strhdr.szName = new UInt16[64];

			int result = Avi.AVIFileCreateStream(aviFile, out aviStream, ref strhdr);

			if (result != 0)
			{
				throw new Exception("Exception in AVIFileCreateStream: " + result.ToString());
			}

			if (writeCompressed)
			{
				CreateCompressedStream(false);
			}
			else
			{
				SetFormat(aviStream);
			}
		}

		/// <summary>Create a compressed stream from an uncompressed stream</summary>
		private void CreateCompressedStream(bool dialog)
		{
			Avi.AVICOMPRESSOPTIONS structOptions;

			if (dialog)
			{
				//display the compression options dialog...
				Avi.AVICOMPRESSOPTIONS_CLASS options = new Avi.AVICOMPRESSOPTIONS_CLASS();
				options.fccType = (uint) Avi.streamtypeVIDEO;
				options.lpParms = IntPtr.Zero;
				options.lpFormat = IntPtr.Zero;
				Avi.AVISaveOptions(IntPtr.Zero, Avi.ICMF_CHOOSE_KEYFRAME | Avi.ICMF_CHOOSE_DATARATE, 1, ref aviStream, ref options);
				Avi.AVISaveOptionsFree(1, ref options);
				structOptions = options.ToStruct();
			}
			else
			{
				//..or set static options
				structOptions.fccType = (UInt32) Avi.mmioStringToFOURCC("vids", 0);
				structOptions.fccHandler = (UInt32) Avi.mmioStringToFOURCC("CVID", 0);
				structOptions.dwKeyFrameEvery = 0;
				structOptions.dwQuality = 0; // 0 .. 10000
				structOptions.dwFlags = 0; // AVICOMRPESSF_KEYFRAMES = 4
				structOptions.dwBytesPerSecond = 0;
				structOptions.lpFormat = new IntPtr(0);
				structOptions.cbFormat = 0;
				structOptions.lpParms = new IntPtr(0);
				structOptions.cbParms = 0;
				structOptions.dwInterleaveEvery = 0;
			}

			//get the compressed stream
			int result = Avi.AVIMakeCompressedStream(out compressedStream, aviStream, ref structOptions, 0);
			if (result != 0)
			{
				throw new Exception("Exception in AVIMakeCompressedStream: " + result.ToString());
			}

			SetFormat(compressedStream);
		}

		/// <summary>Add one frame to a new stream</summary>
		/// <param name="bmp"></param>
		/// <remarks>
		/// This works only with uncompressed streams,
		/// and compressed streams that have not been saved yet.
		/// Use DecompressToNewFile to edit saved compressed streams.
		/// </remarks>
		public void AddFrame(Bitmap bmp)
		{
			try
			{
				bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

				BitmapData bmpDat = bmp.LockBits(
					new Rectangle(
						0, 0, bmp.Width, bmp.Height),
					ImageLockMode.ReadOnly, bmp.PixelFormat);

				int result = Avi.AVIStreamWrite(writeCompressed ? compressedStream : aviStream,
				                                countFrames, 1,
				                                bmpDat.Scan0,
				                                (Int32) (bmpDat.Stride*bmpDat.Height),
				                                0, 0, 0);

				if (result != 0)
				{
					throw new Exception("Exception in VideoStreamWrite: " + result.ToString());
				}

				bmp.UnlockBits(bmpDat);

				countFrames++;
			}
			catch (Exception exp)
			{
				Debug.Write(exp.ToString());
			}
		}

		/// <summary>Apply a format to a new stream</summary>
		/// <param name="aviStream">The IAVISTREAM</param>
		/// <remarks>
		/// The format must be set before the first frame can be written,
		/// and it cannot be changed later.
		/// </remarks>
		private void SetFormat(IntPtr aviStream)
		{
			Avi.BITMAPINFOHEADER bi = new Avi.BITMAPINFOHEADER();
			bi.biSize = Marshal.SizeOf(bi);
			bi.biWidth = width;
			bi.biHeight = height;
			bi.biPlanes = 1;
			bi.biBitCount = countBitsPerPixel;
			bi.biSizeImage = frameSize;

			int result = Avi.AVIStreamSetFormat(aviStream, 0, ref bi, bi.biSize);
			if (result != 0)
			{
				throw new Exception("Error in VideoStreamSetFormat: " + result.ToString());
			}
		}

		/// <summary>Prepare for decompressing frames</summary>
		/// <remarks>
		/// This method has to be called before GetBitmap and ExportBitmap.
		/// Release ressources with GetFrameClose.
		/// </remarks>
		public void GetFrameOpen()
		{
			Avi.AVISTREAMINFO streamInfo = GetStreamInfo(aviStream);

			//Open frames

			Avi.BITMAPINFOHEADER bih = new Avi.BITMAPINFOHEADER();
			bih.biBitCount = countBitsPerPixel;
			bih.biClrImportant = 0;
			bih.biClrUsed = 0;
			bih.biCompression = 0;
			bih.biHeight = (Int32) streamInfo.rcFrame.bottom;
			bih.biWidth = (Int32) streamInfo.rcFrame.right;
			bih.biPlanes = 1;
			bih.biSize = Marshal.SizeOf(bih);
			bih.biXPelsPerMeter = 0;
			bih.biYPelsPerMeter = 0;

			getFrameObject = Avi.AVIStreamGetFrameOpen(aviStream, ref bih);

			if (getFrameObject == 0)
			{
				throw new Exception("Exception in VideoStreamGetFrameOpen!");
			}
		}

		/// <summary>Export a frame into a bitmap file</summary>
		/// <param name="position">Position of the frame</param>
		/// <param name="dstFileName">Name of the file to store the bitmap</param>
		public void ExportBitmap(int position, String dstFileName)
		{
			Bitmap bmp = GetBitmap(position);
			bmp.Save(dstFileName, ImageFormat.Bmp);
			bmp.Dispose();
		}

		/// <summary>Export a frame into a bitmap</summary>
		/// <param name="position">Position of the frame</param>
		public Bitmap GetBitmap(int position)
		{
			if (position > countFrames)
			{
				throw new Exception("Invalid frame position: " + position);
			}

			Avi.AVISTREAMINFO streamInfo = GetStreamInfo(aviStream);

			//Decompress the frame and return a pointer to the DIB
			int firstFrame = Avi.AVIStreamStart(aviStream.ToInt32());
			int dib = Avi.AVIStreamGetFrame(getFrameObject, firstFrame + position);
			IntPtr ptrDib = new IntPtr(dib);
			//Copy the bitmap header into a managed struct
			Avi.BITMAPINFOHEADER bih = new Avi.BITMAPINFOHEADER();
			bih = (Avi.BITMAPINFOHEADER) Marshal.PtrToStructure(ptrDib, bih.GetType());
			//Copy bitmap info header into managed bytes
			byte[] bitmapInfo = new byte[Marshal.SizeOf(bih)];
			Marshal.Copy(ptrDib, bitmapInfo, 0, bitmapInfo.Length);

			if (bih.biSizeImage < 1)
			{
				throw new Exception("Exception in VideoStreamGetFrame");
			}

			//copy the image

			byte[] bitmapData;
			int address = dib + Marshal.SizeOf(bih);
			if (bih.biBitCount < 16)
			{
				//copy palette and pixels
				bitmapData = new byte[bih.biSizeImage + Avi.PALETTE_SIZE];
			}
			else
			{
				//copy only pixels
				bitmapData = new byte[bih.biSizeImage];
			}

			for (int offset = 0; offset < bitmapData.Length; offset++)
			{
				bitmapData[offset] = Marshal.ReadByte(new IntPtr(address));
				address++;
			}

			//create file header
			Avi.BITMAPFILEHEADER bfh = new Avi.BITMAPFILEHEADER();
			bfh.bfType = Avi.BMP_MAGIC_COOKIE;
			bfh.bfSize = (Int32) (55 + bih.biSizeImage); //size of file as written to disk
			bfh.bfReserved1 = 0;
			bfh.bfReserved2 = 0;
			bfh.bfOffBits = Marshal.SizeOf(bih) + Marshal.SizeOf(bfh);
			if (bih.biBitCount < 16)
			{
				//There is a palette between header and pixel data
				bfh.bfOffBits += Avi.PALETTE_SIZE;
			}

			//write a bitmap stream
			BinaryWriter bw = new BinaryWriter(new MemoryStream());

			//write header
			bw.Write(bfh.bfType);
			bw.Write(bfh.bfSize);
			bw.Write(bfh.bfReserved1);
			bw.Write(bfh.bfReserved2);
			bw.Write(bfh.bfOffBits);
			//write bitmap info
			bw.Write(bitmapInfo);
			//write bitmap data
			bw.Write(bitmapData);

			Bitmap bmp = (Bitmap) Image.FromStream(bw.BaseStream);
			Bitmap saveableBitmap = new Bitmap(bmp.Width, bmp.Height);
			Graphics g = Graphics.FromImage(saveableBitmap);
			g.DrawImage(bmp, 0, 0);
			g.Dispose();
			bmp.Dispose();

			bw.Close();
			return saveableBitmap;
		}

		/// <summary>Free ressources that have been used by GetFrameOpen</summary>
		public void GetFrameClose()
		{
			if (getFrameObject != 0)
			{
				Avi.AVIStreamGetFrameClose(getFrameObject);
				getFrameObject = 0;
			}
		}

		//TEST
		public AviManager DecompressToNewFile(String fileName, bool recompress)
		{
			return null;
		}

		/// <summary>Copy all frames into a new file</summary>
		/// <param name="fileName">Name of the new file</param>
		/// <param name="recompress">true: Compress the new stream</param>
		/// <returns>AviManager for the new file</returns>
		/// <remarks>Use this method if you want to append frames to an existing, compressed stream</remarks>
		public AviManager DecompressToNewFile(String fileName, bool recompress, out VideoStream newStream2)
		{
			AviManager newFile = new AviManager(fileName, false);

			GetFrameOpen();

			Bitmap frame = GetBitmap(0);
			VideoStream newStream = newFile.AddVideoStream(recompress, frameRate, frame);
			frame.Dispose();

			for (int n = 1; n < countFrames; n++)
			{
				frame = GetBitmap(n);
				newStream.AddFrame(frame);
				frame.Dispose();
			}
			//TEST
			/*Bitmap test = new Bitmap("..\\..\\testdata\\test.bmp");
			newStream.AddFrame(frame);
			test.Dispose();*/

			GetFrameClose();

			newStream2 = newStream;
			return newFile;
		}

		/// <summary>Copy the stream into a new file</summary>
		/// <param name="fileName">Name of the new file</param>
		public override void ExportStream(String fileName)
		{
			/*Avi.AVICOMPRESSOPTIONS_CLASS opts = new Avi.AVICOMPRESSOPTIONS_CLASS();
			opts.fccType         = (UInt32)Avi.mmioStringToFOURCC("vids", 0);
			opts.fccHandler      = (UInt32)Avi.mmioStringToFOURCC("CVID", 0);
			opts.dwKeyFrameEvery = 0;
			opts.dwQuality       = 0;
			opts.dwFlags         = 0;
			opts.dwBytesPerSecond= 0;
			opts.lpFormat        = new IntPtr(0);
			opts.cbFormat        = 0;
			opts.lpParms         = new IntPtr(0);
			opts.cbParms         = 0;
			opts.dwInterleaveEvery = 0;*/

			Avi.AVICOMPRESSOPTIONS_CLASS opts = new Avi.AVICOMPRESSOPTIONS_CLASS();
			opts.fccType = (uint) Avi.streamtypeVIDEO;
			opts.lpParms = IntPtr.Zero;
			opts.lpFormat = IntPtr.Zero;
			Avi.AVISaveOptions(IntPtr.Zero, Avi.ICMF_CHOOSE_KEYFRAME | Avi.ICMF_CHOOSE_DATARATE, 1, ref aviStream, ref opts);
			Avi.AVISaveOptionsFree(1, ref opts);

			Avi.AVISaveV(fileName, 0, 0, 1, ref aviStream, ref opts);
		}
	}
}