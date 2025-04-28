using System;
using System.Collections.Generic;

namespace XVIBE_TextRPG
{
    public class Shop
    {
        // ▒▒▒ [장비 밸런스 기준 정리] ▒▒▒
        //
        // ▶ 스탯 구조 기준
        //    - 레벨업 시 공격력 +1, 방어력 +2
        //    - 장비 1개 = 3~10레벨 분량 성장치 제공
        //
        // ▶ 구매 비용 기준
        //    - 장비 하나 구매 = 중급 던전 1~2회 보상으로 충당 가능
        //    - 너무 비싸면 접근 불가, 너무 싸면 성장 압축 과도
        //
        // ▶ 난이도 기준
        //    - 고급 던전 생존 조건 = 방어력 25~30 필요
        //    - 고급 장비는 반드시 착용해야 클리어 가능하도록 설정

        public static List<Equipment.Weapon> storeWeapons = new List<Equipment.Weapon>()
        {
            new Equipment.Weapon("나무 검", Equipment.WeaponType.Sword, 3, 100),
            new Equipment.Weapon("강철 검", Equipment.WeaponType.Sword, 6, 300),
            new Equipment.Weapon("은 단검", Equipment.WeaponType.Dagger, 10, 600),
            new Equipment.Weapon("전설의 마법 지팡이", Equipment.WeaponType.Staff, 15, 1000),
        };

        public static List<Equipment.Armor> storeArmors = new List<Equipment.Armor>()
        {
            new Equipment.Armor("가죽 조끼", Equipment.ArmorType.Leather, 3, 120),
            new Equipment.Armor("판금 갑옷", Equipment.ArmorType.Metal, 7, 400),
            new Equipment.Armor("마법 로브", Equipment.ArmorType.Leather, 10, 600),
            new Equipment.Armor("용의 갑옷", Equipment.ArmorType.Robe, 17, 1000),
        };

        public static void EnterShop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(@"
 _____________________________________________
|                _                            |
|  __      _____| | ___ ___  _ __ ___   ___   |
|  \ \ /\ / / _ \ |/ __/ _ \| '_ ` _ \ / _ \  |
|   \ V  V /  __/ | (_| (_) | | | | | |  __/  |
|    \_/\_/ \___|_|\___\___/|_| |_| |_|\___|  |
|_____________________________________________|                                        
");
                Console.WriteLine();
                Console.WriteLine("1. 무기 상점\n2. 방어구 상점\n3. 소모품 상점\n0. 나가기\n");
                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        WeaponShop();
                        break;
                    case "2":
                        ArmorShop();
                        break;
                    case "3":
                        ConsumableShop();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n잘못된 입력입니다.");
                        Console.WriteLine("\nEnter를 눌러 계속...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void WeaponShop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 무기 상점 =====");
                Console.WriteLine($"보유 골드: {Player.Gold} G\n");
                Console.WriteLine("1. 무기 구매");
                Console.WriteLine("2. 무기 판매");
                Console.WriteLine("0. 나가기");
                Console.Write("\n원하는 작업을 선택하세요: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        BuyWeapons();
                        break;
                    case "2":
                        SellWeapons();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n잘못된 입력입니다.");
                        Console.WriteLine("\nEnter를 눌러 계속...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void ArmorShop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 방어구 상점 =====");
                Console.WriteLine($"보유 골드: {Player.Gold} G\n");
                Console.WriteLine("1. 방어구 구매");
                Console.WriteLine("2. 방어구 판매");
                Console.WriteLine("0. 나가기");
                Console.Write("\n원하는 작업을 선택하세요: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        BuyArmors();
                        break;
                    case "2":
                        SellArmors();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n잘못된 입력입니다.");
                        Console.WriteLine("\nEnter를 눌러 계속...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void ConsumableShop()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 소모품 상점 =====");
                Console.WriteLine($"보유 골드: {Player.Gold} G\n");
                Console.WriteLine("1. 소모품 구매");
                Console.WriteLine("2. 소모품 판매");
                Console.WriteLine("0. 나가기");
                Console.Write("\n원하는 작업을 선택하세요: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        BuyConsumables();
                        break;
                    case "2":
                        SellConsumables();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("\n잘못된 입력입니다.");
                        Console.WriteLine("\nEnter를 눌러 계속...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void BuyWeapons()
        {
            Console.Clear();
            Console.WriteLine("===== 무기 구매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            for (int i = 0; i < storeWeapons.Count; i++)
            {
                var w = storeWeapons[i];
                Console.WriteLine($"{i + 1}. {w}");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n구매할 무기 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storeWeapons.Count)
            {
                var selected = storeWeapons[choice - 1];

                if (Equipment.Inventory.Contains(selected))
                {
                    Console.WriteLine("\n이미 보유 중인 무기입니다.");
                }
                else if (Player.Gold >= selected.Price)
                {
                    Equipment.Inventory.Add(selected);
                    Player.Gold -= selected.Price;
                    Console.WriteLine($"\n{selected.Name}을(를) 구매했습니다!");
                }
                else
                {
                    Console.WriteLine("\n골드가 부족합니다!");
                }
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }

        private static void SellWeapons()
        {
            Console.Clear();
            Console.WriteLine("===== 무기 판매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            if (Equipment.Inventory.Count == 0)
            {
                Console.WriteLine("판매할 무기가 없습니다.");
                Console.WriteLine("\nEnter를 눌러 계속...");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < Equipment.Inventory.Count; i++)
            {
                var w = Equipment.Inventory[i];
                Console.WriteLine($"{i + 1}. {w}");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n판매할 무기 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= Equipment.Inventory.Count)
            {
                var selected = Equipment.Inventory[choice - 1];
                Player.Gold += selected.Price / 2; // 판매 가격은 구매 가격의 50%
                Equipment.Inventory.Remove(selected);
                Console.WriteLine($"\n{selected.Name}을(를) 판매했습니다!");
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }

        private static void BuyArmors()
        {
            Console.Clear();
            Console.WriteLine("===== 방어구 구매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            for (int i = 0; i < storeArmors.Count; i++)
            {
                var a = storeArmors[i];
                Console.WriteLine($"{i + 1}. {a}");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n구매할 방어구 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storeArmors.Count)
            {
                var selected = storeArmors[choice - 1];

                if (Equipment.ArmorInventory.Contains(selected))
                {
                    Console.WriteLine("\n이미 보유 중인 방어구입니다.");
                }
                else if (Player.Gold >= selected.Price)
                {
                    Equipment.ArmorInventory.Add(selected);
                    Player.Gold -= selected.Price;
                    Console.WriteLine($"\n{selected.Name}을(를) 구매했습니다!");
                }
                else
                {
                    Console.WriteLine("\n골드가 부족합니다!");
                }
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }

        private static void SellArmors()
        {
            Console.Clear();
            Console.WriteLine("===== 방어구 판매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            if (Equipment.ArmorInventory.Count == 0)
            {
                Console.WriteLine("판매할 방어구가 없습니다.");
                Console.WriteLine("\nEnter를 눌러 계속...");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < Equipment.ArmorInventory.Count; i++)
            {
                var a = Equipment.ArmorInventory[i];
                Console.WriteLine($"{i + 1}. {a}");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n판매할 방어구 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= Equipment.ArmorInventory.Count)
            {
                var selected = Equipment.ArmorInventory[choice - 1];
                Player.Gold += selected.Price / 2; // 판매 가격은 구매 가격의 50%
                Equipment.ArmorInventory.Remove(selected);
                Console.WriteLine($"\n{selected.Name}을(를) 판매했습니다!");
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }

        private static void BuyConsumables()
        {
            Console.Clear();
            Console.WriteLine("===== 소모품 구매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            for (int i = 0; i < Consumable.consumable.Count; i++)
            {
                var c = Consumable.consumable[i];
                Console.WriteLine($"{i + 1}. {c.Name}, 가격: {c.Price} G, 효과: {c.Description}");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n구매할 소모품 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= Consumable.consumable.Count)
            {
                var selected = Consumable.consumable[choice - 1];

                if (Player.Gold >= selected.Price)
                {
                    selected.Amount++;
                    Player.Gold -= selected.Price;
                    Console.WriteLine($"\n{selected.Name}을(를) 구매했습니다!");
                }
                else
                {
                    Console.WriteLine("\n골드가 부족합니다!");
                }
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }

        private static void SellConsumables()
        {
            Console.Clear();
            Console.WriteLine("===== 소모품 판매 =====");
            Console.WriteLine($"보유 골드: {Player.Gold} G\n");

            var ownedConsumables = Consumable.consumable.Where(c => c.Amount > 0).ToList();

            if (ownedConsumables.Count == 0)
            {
                Console.WriteLine("판매할 소모품이 없습니다.");
                Console.WriteLine("\nEnter를 눌러 계속...");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < ownedConsumables.Count; i++)
            {
                var c = ownedConsumables[i];
                Console.WriteLine($"{i + 1}. {c.Name}, 보유량: {c.Amount}, 판매 가격: {c.Price / 2} G");
            }

            Console.WriteLine("0. 나가기");
            Console.Write("\n판매할 소모품 번호를 입력하세요: ");
            string input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= ownedConsumables.Count)
            {
                var selected = ownedConsumables[choice - 1];
                selected.Amount--;
                Player.Gold += selected.Price / 2; // 판매 가격은 구매 가격의 50%
                Console.WriteLine($"\n{selected.Name}을(를) 판매했습니다!");
            }
            else
            {
                Console.WriteLine("\n잘못된 입력입니다.");
            }

            Console.WriteLine("\nEnter를 눌러 계속...");
            Console.ReadLine();
        }
    }
}