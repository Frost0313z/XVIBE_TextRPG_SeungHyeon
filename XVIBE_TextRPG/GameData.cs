using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using static XVIBE_TextRPG.Equipment;

namespace XVIBE_TextRPG
{
    public class GameData
    {
        public string Name { get; set; }
        public string Job { get; set; }

        public int Exp { get; set; }

        public int Level { get; set; }

        public int Gold { get; set; }

        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int MaxMP { get; set; }
        public int CurrentMP { get; set; }
        public int TotalATK { get; set; }
        public int TotalDEF { get; set; }        

        public List<WeaponData> Inventory { get; set; } = new List<WeaponData>();        
        public WeaponData EquippedWeapon { get; set; }

        public List<ArmorData> ArmorInventory { get; set; } = new List<ArmorData>();
        public ArmorData EquippedArmor { get; set; }

        public List<QuestData> Quests { get; set; } = new List<QuestData>();
        public int CurrentKillCount { get; set; }

        public List<ConsumableData> Consumables { get; set; } = new List<ConsumableData>();
        public class QuestData
        {
            public string Name { get; set; }
            public bool IsAccepted { get; set; }
            public bool IsCompleted { get; set; }
            public bool IsRewardReceived { get; set; }
        }

        public class ConsumableData
        {
            public string Name { get; set; }
            public int HealHP { get; set; }
            public int HealMP { get; set; }
            public int Amount { get; set; }
            public string Type { get; set; }
        }
    }

    public class WeaponData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int ATK { get; set; }
        public int Price { get; set; }
    }

    public class ArmorData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int DEF { get; set; }
        public int Price { get; set; }
    }
}
