namespace NIOC.SampleStorage.Shared.Core.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// To Rial display with commas.
        /// 123456 => "123,123"
        /// </summary>
        /// <param name="moneyValue"></param>
        /// <returns></returns>
        public static string ToCurrencyStringFormat(this int moneyValue)
        {
            return moneyValue.ToString("N0");
        }

        /// <summary>
        /// To Rial display with commas.
        /// 123456 => "123,123"
        /// </summary>
        /// <param name="moneyValue"></param>
        /// <returns></returns>
        public static string ToCurrencyStringFormat(this long moneyValue)
        {
            return moneyValue.ToString("N0");
        }

    }
}