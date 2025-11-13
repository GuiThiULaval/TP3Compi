using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class Token(string name, object? value)
    {
        public static readonly Token End = new("End", null);

        public string Name { get; init; } = name;
        public object? Value { get; init; } = value;
    }
}
