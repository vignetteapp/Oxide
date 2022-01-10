using System;
using System.Runtime.InteropServices;

namespace Mikomi
{
    public class ViewConfig : DisposableObject
    {
        private bool isAccelerated;

        /// <summary>
        /// When enabled, the View will be rendered to an offscreen GPU texture
        /// using the GPU driver set in ulPlatformSetGPUDriver. You can fetch
        /// details for the texture via ulViewGetRenderTarget.
        /// <br/>
        /// When disabled (the default), the View will be rendered to an offscreen
        /// pixel buffer surface. This pixel buffer can optionally be provided by the user--
        /// for more info see ulViewGetSurface.
        /// </summary>
        public bool IsAccelerated
        {
            get => isAccelerated;
            set
            {
                if (value == IsAccelerated)
                    return;

                Ultralight.ulViewConfigSetIsAccelerated(Handle, isAccelerated = value);
            }
        }

        private bool isTransparent;

        /// <summary>
        /// Gets or sets whether the background should be transparent.
        /// </summary>
        public bool IsTransparent
        {
            get => isTransparent;
            set
            {
                if (value == IsTransparent)
                    return;

                Ultralight.ulViewConfigSetIsTransparent(Handle, isTransparent = value);
            }
        }

        private bool initialFocus;

        public bool InitialFocus
        {
            get => initialFocus;
            set
            {
                if (value == InitialFocus)
                    return;

                Ultralight.ulViewConfigSetInitialFocus(Handle, initialFocus = value);
            }
        }

        private bool enableImages;

        /// <summary>
        /// Gets or sets whether images should be enabled.
        /// <br/>
        /// (Default = True)
        /// </summary>
        public bool EnableImages
        {
            get => enableImages;
            set
            {
                if (value == enableImages)
                    return;

                Ultralight.ulViewConfigSetEnableImages(Handle, enableImages = value);
            }
        }

        private bool enableJavaScript;

        /// <summary>
        /// Gets or sets whether JavaScript should be enabled.
        /// <br/>
        /// (Default = True)
        /// </summary>
        public bool EnableJavaScript
        {
            get => enableJavaScript;
            set
            {
                if (value == enableJavaScript)
                    return;

                Ultralight.ulViewConfigSetEnableJavaScript(Handle, enableJavaScript = value);
            }
        }

        private string fontFamilyStandard;

        /// <summary>
        /// Gets or Sets the default font-family to use.
        /// <br/>
        /// (Default = Times New Roman)
        /// </summary>
        public string FontFamilyStandard
        {
            get => fontFamilyStandard;
            set
            {
                if (value == fontFamilyStandard)
                    return;

                Ultralight.ulViewConfigSetFontFamilyStandard(Handle, fontFamilyStandard = value);
            }
        }

        private string fontFamilyFixed;

        /// <summary>
        /// Gets or Sets the default font-family to use for fixed fonts, eg <pre> and <code>
        /// <br/>
        /// (Default = Courier New)
        /// </summary>
        public string FontFamilyFixed
        {
            get => fontFamilyFixed;
            set
            {
                if (value == fontFamilyFixed)
                    return;

                Ultralight.ulViewConfigSetFontFamilyFixed(Handle, fontFamilyFixed = value);
            }
        }

        private string fontFamilySerif;

        /// <summary>
        /// Gets or Sets the default font-family to use for serif fonts.
        /// <br/>
        /// (Default = Times New Roman)
        /// </summary>
        public string FontFamilySerif
        {
            get => fontFamilySerif;
            set
            {
                if (value == fontFamilySerif)
                    return;

                Ultralight.ulViewConfigSetFontFamilySerif(Handle, fontFamilySerif = value);
            }
        }

        private string fontFamilySansSerif;

        /// <summary>
        /// Gets or Sets the default font-family to use for sans-serif fonts.
        /// <br/>
        /// (Default = Arial)
        /// </summary>
        public string FontFamilySansSerif
        {
            get => fontFamilySansSerif;
            set
            {
                if (value == fontFamilySansSerif)
                    return;

                Ultralight.ulViewConfigSetFontFamilySansSerif(Handle, fontFamilySansSerif = value);
            }
        }

        private string userAgent;

        /// <summary>
        /// Gets or Sets the user agent string.
        /// <br/>
        /// (Default = Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/608.3.10 (KHTML, like Gecko) Ultralight/1.3.0 Safari/608.3.10)
        /// </summary>
        public string UserAgent
        {
            get => userAgent;
            set
            {
                if (value == userAgent)
                    return;

                Ultralight.ulViewConfigSetFontFamilyUserAgent(Handle, userAgent = value);
            }
        }

        public ViewConfig()
            : base(Ultralight.ulCreateConfig())
        {
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyViewConfig(Handle);
    }

#pragma warning disable CA2101 // Custom marshaler is used

    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateViewConfig();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulDestroyViewConfig(IntPtr config);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetIsAccelerated(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isAccelerated);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetIsTransparent(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isTransparent);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetInitialDeviceScale(IntPtr config, double initialDeviceScale);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetInitialFocus(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool isFocussed);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetEnableImages(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetEnableJavaScript(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyStandard(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyFixed(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilySerif(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilySansSerif(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern void ulViewConfigSetFontFamilyUserAgent(
            IntPtr config,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string fontName
        );
    }

#pragma warning restore CA2101

}
