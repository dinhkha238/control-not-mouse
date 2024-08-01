class Image
{
    public string image { get; set; } = "../../../../../Media Sources/ProShow Producer 9.0 - Built-In Content/Backgrounds/01_menu-gallery.jpg";
    public int imageEnable { get; set; } = 1;
    public string name { get; set; } = "01_menu-gallery";
    public string notes { get; set; } = "";
    public int fromStyle { get; set; } = 1;
    public int templateImageId { get; set; } = 0;
    public int replaceableTemplate { get; set; } = 1;
    public int sizeMode { get; set; } = 1;
    public int colorize { get; set; } = 1;
    public int colorizeColor { get; set; } = 8421504;
    public int colorizeStrength { get; set; } = 10000;
    public int outline { get; set; } = 1;
    public int outlineColor { get; set; } = 16777215;
    public int shadow { get; set; } = 1;
    public int aspectX { get; set; } = 4;
    public int aspectY { get; set; } = 3;
    public int videoVolume { get; set; } = 100;
    public int objectId { get; set; } = 1;
    public int videoSpeed { get; set; } = 100;
    public int useTransitionIn { get; set; } = 1;
    public int useTransitionOut { get; set; } = 1;
    public int outlineSize { get; set; } = 10;
    public int shadowSize { get; set; } = 70;
    public int shadowOpacity { get; set; } = 128;
    public int maskChannel { get; set; } = 10;
    public string filterName { get; set; } = "";
    public string savedFilterName { get; set; } = "";
    public string motionFilterName { get; set; } = "";
    public string motionFilterNameIn { get; set; } = "";
    public string motionFilterNameOut { get; set; } = "";
    public int nrOfKeyframes { get; set; } = 2;
    public Keyframe[] keyframes { get; set; } = new Keyframe[2];

    public Image()
    {
        keyframes[0] = new Keyframe();
        keyframes[1] = new Keyframe
        {
            timestamp = 10000,
            timeSegment = 3,
            segmentTimestamp = 10000,
            zoomX = 7500,
            zoomY = 7500,
        };
    }
}

class Keyframe
{
    public int timeSegment { get; set; } = 1;
    public int attributeMask { get; set; } = -1;
    public int offsetX { get; set; } = -134;
    public int offsetY { get; set; } = 245;
    public int zoomX { get; set; } = 5000;
    public int zoomY { get; set; } = 5000;
    public int panAccelType { get; set; } = 1;
    public int zoomXAccelType { get; set; } = 1;
    public int zoomYAccelType { get; set; } = 1;
    public int rotationAccelType { get; set; } = 1;
    public int tiltVAccelType { get; set; } = 1;
    public int tiltHAccelType { get; set; } = 1;
    public int motionSmoothness { get; set; } = 0;
    public int lockAR { get; set; } = 1;
    public int transparency { get; set; } = 0;
    public int audioFade { get; set; } = 2000;
    public int colorizeColor { get; set; } = 8421504;
    public int colorizeStrength { get; set; } = 10000;
    public int shadowOffsetX { get; set; } = 70;
    public int shadowOffsetY { get; set; } = 70;
    public int timestamp { get; set; } = 0;
    public int segmentTimestamp { get; set; } = 0;
}

class Sound
{
    public int useDefault { get; set; } = 1;
    public int volume { get; set; } = 100;
    public int fadeIn { get; set; } = 100;
    public int fadeOut { get; set; } = 100;
    public int async { get; set; } = 1;
    public int musicUseDefault { get; set; } = 1;
    public int musicVolume { get; set; } = 50;
    public int musicFadeIn { get; set; } = 100;
    public int musicFadeOut { get; set; } = 100;
    public int normalizeCustom { get; set; } = 0;
    public int normalizePreset { get; set; } = 0;
    public string file { get; set; } = null;
    public int length { get; set; } = 0;
    public float startTime { get; set; } = 0;
    public float endTime { get; set; } = 0;
}

class Cell
{
    public int imageEnable { get; set; } = 1;

    public string notes { get; set; } = "";

    public string slideStyleFileName { get; set; } = "Default";
    public int nrOfImages { get; set; } = 2;
    public Image[] images { get; set; } = new Image[2];
    public int background { get; set; } = 1;
    public int bgDefault { get; set; } = 1;
    public int bgSizeMode { get; set; } = 1;
    public int bgColorizeColor { get; set; } = 8421504;
    public Sound sound { get; set; } = new Sound();
    public int musicVolumeOffset { get; set; } = 50;
    public float time { get; set; } = 3000;
    public int transId { get; set; } = 2;
    public int transTime { get; set; } = 1000;
    public int includeGlobalCaptions { get; set; } = 1;

    public Cell()
    {
        images[0] = new Image();

        images[1] = new Image
        {
            sizeMode = 2,
            videoVolume = 0,
            keyframes = [
                new Keyframe{
                    offsetX=2279,
                    zoomX=17500,
                    zoomY=17500,
                    panAccelType=0,
                    zoomXAccelType=0,
                    zoomYAccelType=0,
                },
                new Keyframe{
                    offsetX=-2397,
                    offsetY=-711,
                    zoomX=15000,
                    zoomY=15000,
                    timeSegment=3,
                    segmentTimestamp=10000,
                    timestamp=10000,
                }
            ]

        };
    }
}