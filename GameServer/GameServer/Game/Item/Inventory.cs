using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Game
{
    public class Inventory
    {
        public Dictionary<int, Item> Items { get; } = new Dictionary<int, Item>();

        public void Add(Item _item)
        {
            if (Items.TryGetValue(_item.ItemDbId, out var item))
            {
                Items[item.ItemDbId] = _item;
                return;
            }

            Items.Add(_item.ItemDbId, _item);
        }

        public Item? Get(int itemDbId)
        {
            Items.TryGetValue(itemDbId, out var item);
            return item;
        }

        public Item? Find(Func<Item, bool> condition)
        {
            return Items.Values.FirstOrDefault(item => condition.Invoke(item));
        }

        public int? GetAvailableSlot(int templateId)
        {
            if ((int)(templateId / 100000) == 4)
            {
                return -1;
            }

            var item = Items.FirstOrDefault(x => x.Value.TemplateId == templateId && x.Value.Stackable).Value;

            if (item != null)
            {
                return item.Slot;
            }

            return GetEmptySlot();
        }

        public int? GetEmptySlot()
        {
            for (int slot = 0; slot < 20; slot++)
            {
                var item = Items.Values.FirstOrDefault(i => i.Slot == slot);
                if (item == null)
                    return slot;
            }

            return null;
        }
    }
}