using System;
using System.Collections.Generic;

namespace XVIBE_TextRPG
{
    public class Equipment
    {
        public static int ATKBonus { get; set; } = 0;
        public static int DEFBonus { get; set; } = 0;

        // 무기 타입 정의
        public enum WeaponType { Sword, Dagger, Staff }

        // 방어구 타입 정의
        public enum ArmorType { Metal, Leather, Robe }

        // 무기 클래스 정의
        public class Weapon
        {
            public string Name { get; }
            public WeaponType Type { get; }
            public int ATK { get; }            
            public int Price { get; }

            public Weapon(string name, WeaponType type, int atk, int price)
            {
                Name = name;
                Type = type;
                ATK = atk;
                Price = price;
            }

            public override string ToString()
            {
                return $"{Name} (공격력: {ATK}, 가격: {Price}G)";
            }

            public override bool Equals(object obj)
            {
                if (obj is Weapon other)
                {
                    return Name == other.Name && Type == other.Type && ATK == other.ATK && Price == other.Price ;
                }
                return false ;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Type, ATK, Price);
            }
        }

        

        // 방어구 클래스 정의
        public class Armor
        {
            public string Name { get; }
            public ArmorType Type { get; }
            public int DEF { get; }
            public int Price { get; }

            public Armor(string name, ArmorType type, int dEF, int price)
            {
                Name = name;
                Type = type;
                DEF = dEF;
                Price = price;
            }

            public override string ToString()
            {
                return $"{Name} (방어력: {DEF}, 가격: {Price}G)";
            }

            public override bool Equals(object? obj)
            {
                if (obj is Armor other)
                {
                    return Name == other.Name && Type == other.Type && DEF == other.DEF && Price == other.Price;
                }
                return false ;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Type, DEF, Price);
            }
        }

        // 인벤토리와 장비
        public static List<Weapon> Inventory = new List<Weapon>();
        public static Weapon EquippedWeapon = null;
        public static List<Armor> ArmorInventory = new List<Armor>();
        public static Armor EquippedArmor = null;

        // 무기 장착
        public static void Equip(Weapon weapon)
        {
            if (EquippedWeapon != null && weapon.Equals(EquippedWeapon)) // 저장하고 불러왔을 때 참조가 달라 객체 비교로 변경
            {
                Console.WriteLine($"{weapon.Name}은(는) 이미 장착되어 있습니다!");
                return;
            }

            if (EquippedWeapon != null)
            {
                Console.WriteLine($"{EquippedWeapon.Name}을(를) 해제합니다.");
            }

            EquippedWeapon = weapon;
            Quest.CheckQuestConditions();
            ATKBonus = weapon.ATK;
            Console.WriteLine($"{weapon.Name}을(를) 새로 장착했습니다!");
            Player.UpdateStats();
        }

        // 방어구 장착
        public static void Equip(Armor armor)
        {
            if (EquippedArmor != null && armor.Equals(EquippedArmor))
            {
                Console.WriteLine($"{armor.Name}은(는) 이미 장착 중입니다!");
                return;
            }

            if (EquippedArmor != null)
            {
                Console.WriteLine($"{EquippedArmor.Name}을(를) 해제합니다.");
            }

            EquippedArmor = armor;
            DEFBonus = armor.DEF;
            Console.WriteLine($"{armor.Name}을(를) 새로 장착했습니다!");
            Player.UpdateStats();
        }

        // 장비 해제
        public static void UnequipAll()
        {
            bool unequipped = false;

            if (EquippedWeapon != null)
            {
                Console.WriteLine($"{EquippedWeapon.Name}을(를) 해제했습니다!");
                EquippedWeapon = null;
                ATKBonus = 0;
                unequipped = true;
                Player.UpdateStats();
            }

            if (EquippedArmor != null)
            {
                Console.WriteLine($"{EquippedArmor.Name} 방어구를 해제했습니다!");
                EquippedArmor = null;
                DEFBonus = 0;
                unequipped = true;
                Player.UpdateStats();
            }

            if (!unequipped)
            {
                Console.WriteLine("장착된 장비가 없습니다.");
            }
        }

        // 인벤토리 표시 및 장착 / 해제
        public static void ShowInventory()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 인벤토리 =====");
                if (Inventory.Count == 0)
                {
                    Console.WriteLine("보유한 무기가 없습니다.");
                }
                else
                {
                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        var w = Inventory[i];
                        string equippedTag = (EquippedWeapon != null && w.Equals(EquippedWeapon)) ? "[E] " : ""; // 이 부분도 참조가 달라 적용 안되던 부분 수정
                        Console.WriteLine($"{i + 1}. {equippedTag}{w}");
                    }
                }

                if(ArmorInventory.Count == 0)
                {
                    Console.WriteLine("보유한 방어구가 없습니다.");
                }
                else
                {
                    for (int i = 0; i < ArmorInventory.Count; i++)
                    {
                        var a = ArmorInventory[i];
                        string equippedTag = (EquippedArmor != null && a.Equals(EquippedArmor)) ? "[E] " : "";
                        Console.WriteLine($"{i + 1 + Inventory.Count}. {equippedTag}{a}");
                    }
                }
                Console.WriteLine("\n0. 뒤로가기");
                Console.WriteLine("장착할 장비 번호를 입력하거나 해제하려면 U를 입력하세요.");
                Console.Write(">> ");
                var input = Console.ReadLine();

                if (input == "0") break;
                else if (input.ToUpper() == "U")
                {
                    UnequipAll();
                }
                else if (int.TryParse(input, out int index))
                {
                    if (index >= 1 && index <= Inventory.Count)
                    {
                        Equip(Inventory[index - 1]); // 무기 장착
                    }
                    else if (index > Inventory.Count && index <= Inventory.Count + ArmorInventory.Count)
                    {
                        Equip(ArmorInventory[index - Inventory.Count - 1]);
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                    }
                }



                Console.WriteLine("\n계속하려면 Enter...");
                Console.ReadLine();
            }
        }
    }
}
