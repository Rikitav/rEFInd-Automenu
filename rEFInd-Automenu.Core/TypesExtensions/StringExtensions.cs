using System;
using System.Linq;
using System.Text;

namespace rEFInd_Automenu.TypesExtensions
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpper(this string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new ArgumentException(nameof(Value) + " was null or white space");

            char firstLetter = Value.First(c => char.IsLetter(c));
            int firstIsLetterPos = Value.IndexOf(firstLetter);

            StringBuilder builder = new StringBuilder(Value);
            if (char.IsUpper(Value[firstIsLetterPos + 1]))
                return Value;

            builder.Remove(firstIsLetterPos, 1);
            builder.Insert(firstIsLetterPos, char.ToUpper(firstLetter));
            return builder.ToString();
        }

        public static string Quotation(this string Value, char StartQuotationSymbol, char EndQuotationSymbol)
        {
            if (!Value.StartsWith(StartQuotationSymbol))
                Value = Value.Insert(0, StartQuotationSymbol.ToString());

            if (!Value.EndsWith(EndQuotationSymbol))
                Value = Value.Insert(Value.Length, EndQuotationSymbol.ToString());

            return Value;
        }

        public static string Quotation(this string Value)
            => Value.Quotation('\"', '\"');

        public static string Quotation(this string Value, char QuotationSymbol)
            => Value.Quotation(QuotationSymbol, QuotationSymbol);
    }
}
