using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public class Assignment(string leftId, string rightId) : Instruction
    {
        public string LeftId { get; } = leftId;
        public string RightId { get; } = rightId;
    }
}
