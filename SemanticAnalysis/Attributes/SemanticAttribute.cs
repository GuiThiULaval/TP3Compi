using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Attributes
{

    public abstract class SemanticAttribute : ISemanticAttribute
    {
        public static readonly SemanticAttribute<string> LEXICAL_VALUE = new("lexval");

        public abstract string Name { get; }
    }

    public class SemanticAttribute<T>(string name) : SemanticAttribute
    {
        public override string Name { get; } = name;
    }
}
