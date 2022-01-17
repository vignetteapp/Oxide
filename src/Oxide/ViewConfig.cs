// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide
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
            set => Ultralight.ulViewConfigSetIsAccelerated(Handle, isAccelerated = value);
        }

        private bool isTransparent;

        /// <summary>
        /// Gets or sets whether the background should be transparent.
        /// </summary>
        public bool IsTransparent
        {
            get => isTransparent;
            set => Ultralight.ulViewConfigSetIsTransparent(Handle, isTransparent = value);
        }

        private bool initialFocus;

        public bool InitialFocus
        {
            get => initialFocus;
            set => Ultralight.ulViewConfigSetInitialFocus(Handle, initialFocus = value);
        }

        private double initialDeviceScale;

        public double InitialDeviceScale
        {
            get => initialDeviceScale;
            set => Ultralight.ulViewConfigSetInitialDeviceScale(Handle, initialDeviceScale = value);
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
            set => Ultralight.ulViewConfigSetEnableImages(Handle, enableImages = value);
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
            set => Ultralight.ulViewConfigSetEnableJavaScript(Handle, enableJavaScript = value);
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
            set => Ultralight.ulViewConfigSetFontFamilyStandard(Handle, fontFamilyStandard = value);
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
            set => Ultralight.ulViewConfigSetFontFamilyFixed(Handle, fontFamilyFixed = value);
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
            set => Ultralight.ulViewConfigSetFontFamilySerif(Handle, fontFamilySerif = value);
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
            set => Ultralight.ulViewConfigSetFontFamilySansSerif(Handle, fontFamilySansSerif = value);
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
            set => Ultralight.ulViewConfigSetFontFamilyUserAgent(Handle, userAgent = value);
        }

        /// <summary>
        /// Creates a view config with default values.
        /// </summary>
        public ViewConfig()
            : base(Ultralight.ulCreateViewConfig())
        {
        }

        protected override void DisposeUnmanaged()
            => Ultralight.ulDestroyViewConfig(Handle);
    }
}
