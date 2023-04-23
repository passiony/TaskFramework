using System;

namespace TF.Runtime
{
    public class SpeakFilter
    {
        public static readonly SoundReplace[] ignores =
        {
            new SoundReplace("VR", "δ"),
        };

        public static readonly SoundReplace[] replaces =
        {
            new SoundReplace(@"kW.h", "千瓦时"),
            new SoundReplace(@"mm²", "平方毫米"),
            new SoundReplace(@"mm", "毫米"),
            new SoundReplace(@"Ⅲ", "三"),
            new SoundReplace(@"Ⅱ", "二"),
            new SoundReplace(@"Ⅰ", "一"),
            new SoundReplace(@"#", "号"),
            new SoundReplace(@"KM", "千米"),
            new SoundReplace(@"MA", "毫安"),
            new SoundReplace(@"KV", "千伏"),
            new SoundReplace(@"MΩ", "兆欧"),
            new SoundReplace(@"KΩ", "千欧"),
            new SoundReplace(@"M", "米"),
            new SoundReplace(@"V", "伏"),
            new SoundReplace(@"Ω", "欧姆"),
            new SoundReplace(@"\\n", "。"),
            new SoundReplace(@"\n", "。"),
        };
        
        [Serializable]
        public class SoundReplace
        {
            public string origin;
            public string replace;

            public SoundReplace(string origin, string replace)
            {
                this.origin = origin;
                this.replace = replace;
            }
        }
    }
}