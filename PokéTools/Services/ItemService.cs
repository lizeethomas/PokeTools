using PokéToolsThèque.Misc;
using System.Text;
using System.Text.Json;

namespace PokéTools.Services
{
    public class ItemService
    {
        private readonly List<Item> _items;
        public List<Item> Items => _items;
        private static readonly string _filePath = "E:\\Bureau\\CODE\\PokéToolsProject\\PokéTools\\Data\\items.tsv";

        public ItemService()
        {
            _items = LoadItemsFromFile();
        }

        private static List<Item> LoadItemsFromFile() 
        { 
            var items = new List<Item>();
            var lines = File.ReadAllLines(_filePath, Encoding.UTF8);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i]; 
                var fields = line.Split('\t');

                items.Add(new Item
                {
                    Name = fields[0] ?? "",
                    Category = fields[1] ?? "",
                    Description = fields[2]
                });
            }

            return items;
        }
    }
}
