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

namespace AviFile
{
	public abstract class AviStream
	{
		protected int aviFile;
		protected IntPtr aviStream;
		protected IntPtr compressedStream;
		protected bool writeCompressed;

		public virtual void Close()
		{
			if (writeCompressed)
			{
				Avi.AVIStreamRelease(compressedStream);
			}
			Avi.AVIStreamRelease(aviStream);
		}

		public abstract void ExportStream(String fileName);
	}
}