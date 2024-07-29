class Image
{
    public string image { get; set; } = "../../../../../Media Sources/ProShow Producer 9.0 - Built-In Content/Backgrounds/01_menu-gallery.jpg";
    public int imageEnable { get; set; } = 1;
    public string name { get; set; } = "01_menu-gallery";
    public int replaceableTemplate { get; set; } = 1;
    public int sizeMode { get; set; } = 1;
    public int colorizeColor { get; set; } = 8421504;
    public int colorizeStrength { get; set; } = 10000;
    public int outlineColor { get; set; } = 16777215;
    public int aspectX { get; set; } = 4;
    public int aspectY { get; set; } = 3;
    public int videoVolume { get; set; } = 100;
    public int objectId { get; set; } = 1;
    public int videoSpeed { get; set; } = 100;
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
        keyframes[0] = new Keyframe
        {
            timeSegment = 1,
            attributeMask = -1,
            zoomX = 10000,
            zoomY = 10000,
            panAccelType = 1,
            zoomXAccelType = 1,
            zoomYAccelType = 1,
            rotationAccelType = 1,
            tiltVAccelType = 1,
            tiltHAccelType = 1,
            motionSmoothness = 5000,
            lockAR = 1,
            transparency = 0,
            audioFade = 2000,
            colorizeColor = 8421504,
            colorizeStrength = 10000,
            shadowOffsetX = 70,
            shadowOffsetY = 70
        };

        keyframes[1] = new Keyframe
        {
            timestamp = 10000,
            timeSegment = 3,
            segmentTimestamp = 10000,
            attributeMask = -1,
            zoomX = 10000,
            zoomY = 10000,
            panAccelType = 1,
            zoomXAccelType = 1,
            zoomYAccelType = 1,
            rotationAccelType = 1,
            tiltVAccelType = 1,
            tiltHAccelType = 1,
            motionSmoothness = 5000,
            lockAR = 1,
            transparency = 0,
            audioFade = 2000,
            colorizeColor = 8421504,
            colorizeStrength = 10000,
            shadowOffsetX = 70,
            shadowOffsetY = 70
        };
    }
}

class Keyframe
{
    public int timeSegment { get; set; }
    public int attributeMask { get; set; }
    public int zoomX { get; set; }
    public int zoomY { get; set; }
    public int panAccelType { get; set; }
    public int zoomXAccelType { get; set; }
    public int zoomYAccelType { get; set; }
    public int rotationAccelType { get; set; }
    public int tiltVAccelType { get; set; }
    public int tiltHAccelType { get; set; }
    public int motionSmoothness { get; set; }
    public int lockAR { get; set; }
    public int transparency { get; set; }
    public int audioFade { get; set; }
    public int colorizeColor { get; set; }
    public int colorizeStrength { get; set; }
    public int shadowOffsetX { get; set; }
    public int shadowOffsetY { get; set; }
    public int timestamp { get; set; }
    public int segmentTimestamp { get; set; }
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
}

class Cell
{
    public int imageEnable { get; set; } = 1;
    public int nrOfImages { get; set; } = 1;
    public Image[] images { get; set; } = { new Image() };
    public int background { get; set; } = 1;
    public int bgDefault { get; set; } = 1;
    public int bgSizeMode { get; set; } = 1;
    public int bgColorizeColor { get; set; } = 8421504;
    public Sound sound { get; set; } = new Sound();
    public int musicVolumeOffset { get; set; } = 50;
    public int time { get; set; } = 3000;
    public int transId { get; set; } = 2;
    public int transTime { get; set; } = 3000;
    public int includeGlobalCaptions { get; set; } = 1;
}