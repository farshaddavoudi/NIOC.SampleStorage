using System.Reflection;

namespace NIOC.SampleStorage.Shared.App
{
    public static class AppMetadata
    {
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

        public static readonly string ApplicationPersianName = "سامانه‌ی انباری نمونه‌ها";

        public static readonly string ApplicationEnglishName = "NIOC Sample Storage";

        public static readonly string SolutionName = "NIOC.SampleStorage";
    }
}