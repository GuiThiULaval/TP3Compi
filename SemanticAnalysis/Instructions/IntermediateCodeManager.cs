using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public static class IntermediateCodeManager
    {
        public const int FIRST_ID = 100;

        public static int NextId => _nextId;

        private static int _nextId = FIRST_ID;
        private static readonly List<Label> _emittedLabels = [];
        private static readonly Dictionary<int, Jump> _instructions = [];

        public static Label EmitLabel()
        {
            Label label = new(_nextId);
            _emittedLabels.Add(label);
            _nextId++;
            return label;
        }

        public static void RegisterForBackpatching(Jump instruction)
        {
            _instructions[instruction.Label.Id] = instruction;
        }

        public static void Backpatch(HashSet<int> list, int labelId)
        {
            if (labelId < FIRST_ID || labelId >= _nextId)
            {
                throw new Exception($"Label={labelId} does not exist.");
            }

            foreach (int id in list)
            {
                _instructions[id].JumpLabel = _emittedLabels.FirstOrDefault((label) => label.Id == labelId);
                _instructions.Remove(id);
            }
        }
    }
}
