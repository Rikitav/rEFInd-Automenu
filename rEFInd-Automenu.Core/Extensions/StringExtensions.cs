using System;

namespace rEFInd_Automenu.Extensions
{
    public static class StringExtensions
    {
        public static string FirstLetterToUpper(this string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
                throw new ArgumentException(nameof(Value) + " was null or white space");

            int LetterPos = 0;
            if (!char.IsLetter(Value[0]))
            {
                while (!char.IsLetter(Value[LetterPos]))
                {
                    if (++LetterPos == Value.Length)
                        return Value;
                }
            }

            if (char.IsUpper(Value[LetterPos]))
                return Value;

            return Value.Insert(LetterPos, char.ToUpper(Value[LetterPos]).ToString()).Remove(LetterPos + 1, 1);
        }

        public static string Quotation(this string Value, char StartQuotationSymbol, char EndQuotationSymbol)
        {
            if (!Value.StartsWith(StartQuotationSymbol))
                Value = Value.Insert(0, StartQuotationSymbol.ToString());

            if (!Value.EndsWith(EndQuotationSymbol))
                Value = Value.Insert(Value.Length, StartQuotationSymbol.ToString());

            return Value;
        }

        public static string Quotation(this string Value)
            => Value.Quotation('\"', '\"');

        public static string Quotation(this string Value, char QuotationSymbol)
            => Value.Quotation(QuotationSymbol, QuotationSymbol);
    }
}
