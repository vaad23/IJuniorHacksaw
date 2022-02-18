using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 Good iPhone12 = new Good("IPhone 12");
Good iPhone11 = new Good("IPhone 11");

Warehouse warehouse = new Warehouse();

Shop shop = new Shop(warehouse);

warehouse.Delive(iPhone12, 10);
warehouse.Delive(iPhone11, 1);

//Вывод всех товаров на складе с их остатком

Cart cart = shop.Cart();
cart.Add(iPhone12, 4);
cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

//Вывод всех товаров в корзине

Console.WriteLine(cart.Order().Paylink);

cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
*/

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Shop
    {

    }

    class Warehouse
    {
        private Dictionary<string, Cell> _cells = new Dictionary<string, Cell>();

        public IEnumerable<IReadOnlyCell> Cells => _cells.Values;

        public bool AreItemsExist(string itemName, int count = 1)
        {
            if (_cells.ContainsKey(itemName))
                if (_cells[itemName].Count >= count)
                    return true;

            return false;
        }

        public void Delive(Item item, int count)
        {
            if (_cells.ContainsKey(item.Name))
                _cells[item.Name].Add(count);
            else
                _cells.Add(item.Name, new Cell(item, count));
        }

        public Cell Unload(string itemName, int count)
        {
            if (AreItemsExist(itemName, count) == false)
                throw new ArgumentException(nameof(AreItemsExist));

            _cells[itemName].Remove(count);

            if (_cells[itemName].Count == 0)
                _cells.Remove(itemName);

            return new Cell(new Item(itemName), count);
        }
    }

    class Cell : IReadOnlyCell
    {
        public Cell(Item item, int count)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Add(count);
        }

        public Item Item { get; }

        public int Count { get; private set; }

        public void Add(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count += count;
        }

        public void Remove(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count > Count)
                throw new ArgumentOutOfRangeException("removedCount" + " > " + nameof(Count));

            Count -= count;
        }
    }

    interface IReadOnlyCell
    {
        Item Item { get; }
        int Count { get; }
    }

    class Item
    {
        public readonly string Name;

        public Item(string name)
        {
            Name = name;
        }
    }
}
