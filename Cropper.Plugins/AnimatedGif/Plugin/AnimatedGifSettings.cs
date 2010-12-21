namespace Cropper.AnimatedGif
{
    public class AnimatedGifSettings
    {
        private static readonly int DEFAULT_CAPTURE_INTERVAL = 100;
        private static readonly int DEFAULT_ENCODING_QUALITY = 10;
        private static readonly int DEFAULT_REPEATS = 0;
        private int _CaptureInterval;
        private int _EncodingQuality;
        private int _Repeats;

        public AnimatedGifSettings()
        {
            // defaults
            PopViewer = true;
            PlaySound = true;
            _CaptureInterval = DEFAULT_CAPTURE_INTERVAL;
            _EncodingQuality = DEFAULT_ENCODING_QUALITY;
            _Repeats = DEFAULT_REPEATS;
        }

        /// <summary>
        ///   True: pop the default GIF viewer with the capture, after it's complete.
        ///   False: don't.
        /// </summary>
        public bool PopViewer { get; set; }

        /// <summary>
        ///   True: play sounds during start/stop of animation capture.
        ///   False: don't.
        /// </summary>
        public bool PlaySound { get; set; }

        /// <summary>
        ///   The interval in ms on which a frame will be captured.
        /// </summary>
        public int CaptureInterval
        {
            get
            {
                return _CaptureInterval;
            }
            set
            {
                _CaptureInterval = (value > 20 && value < 1000)
                    ? value
                    : DEFAULT_CAPTURE_INTERVAL;
            }
        }

        /// <summary>
        ///   The GIF Encoder quality level.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   Valid values are between 1 and 20 inclusive.  1 = highest quality,
        ///   but slowest encoding. Levels above 20 do not appreciably
        ///   increase speed.  The default is 10.
        ///   </para>
        /// </remarks>
        public int EncodingQuality
        {
            get
            {
                return _EncodingQuality;
            }
            set
            {
                _EncodingQuality = (value <= 20 && value > 0)
                    ? value
                    : DEFAULT_ENCODING_QUALITY;
            }
        }

        /// <summary>
        ///   The number of repeats for the Animated GIF.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   -1 implies no repeat at all.  Zero implies infinite repeat.
        ///   Any other number implies the number of times to repeat the play.
        ///   The default is 0, infinite repeats.
        ///   </para>
        /// </remarks>
        public int Repeats
        {
            get
            {
                return _Repeats;
            }
            set
            {
                _Repeats = (value >= -1)
                    ? value
                    : DEFAULT_REPEATS;
            }
        }
   }
}