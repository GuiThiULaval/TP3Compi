using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp.ParsingTables
{
    public class Item
    {
        private static readonly Dictionary<Production, List<Item?>> _cache = [];

        public Production Production { get; }
        public int Position { get; }
        public List<Symbol> Prefix { get; }
        public List<Symbol> Suffix { get; }

        public bool IsComplete => Suffix.Count == 0;

        public static Item Fetch(Production production, int position)
        {
            ValidatePosition(production, position);

            // Ensure production items exist in cache.
            if (!_cache.TryGetValue(production, out List<Item?>? items))
            {
                int count = production.Body.Count + 1;
                if (production.IsEmpty)
                {
                    count--;
                }

                items = new(new Item[count]);
                _cache.Add(production, items);
            }

            // Try to retrieve item from cache.
            Item? item = items[position];
            if (item is not null)
            {
                return item;
            }

            // Add new item to cache.
            Item newItem = new(production, position);
            items[position] = newItem;
            return newItem;
        }

        private static void ValidatePosition(Production production, int position)
        {
            if (position < 0)
            {
                throw new Exception($"Got position {position}, should be greater than 0.");
            }
            if (production.IsEmpty && position != 0)
            {
                throw new Exception($"Cannot use position {position} for epsilon production.");
            }
            if (position > production.Body.Count)
            {
                throw new Exception($"Got position {position} for production of body size {production.Body.Count}.");
            }
        }

        private Item(Production production, int position)
        {
            Production = production;
            Position = position;

            if (Production.IsEmpty)
            {
                Prefix = [];
                Suffix = [];
            }
            else
            {
                Prefix = production.Body[..position];
                Suffix = production.Body[position..];
            }
        }

        public string GetDisplayText()
        {
            string prefixNames = string.Join("", Prefix.Select(symbol => symbol.Name));
            string suffixNames = string.Join("", Suffix.Select(symbol => symbol.Name));
            return $"[{Production.Head.Name} -> {prefixNames}·{suffixNames}]";
        }
    }
}
