using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVIBE_TextRPG
{
    internal class RestSystem
    {
        public static void Rest()
        {
            bool restConfirmed = false;

            while (!restConfirmed)
            {
                Console.Clear();
                Console.WriteLine("[휴식하기]");
                Console.WriteLine();
                Console.WriteLine("1500 G 를 내면 체력과 마나를 회복할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine($"현재 체력 : {Player.CurrentHP} / {Player.MaxHP}");
                Console.WriteLine($"현재 마나 : {Player.CurrentMP} / {Player.MaxMP}");
                Console.WriteLine($"보유 골드 : {Player.Gold}");
                Console.WriteLine();
                Console.Write("1. 휴식하기\n0. 나가기\n>>");

                string input = Console.ReadLine();
                bool rest = false;

                // 복합포션(대) 보다 효율 안나오게 해야 포션 시스템이 의미있어짐

                while (!rest)
                {
                    switch (input)
                    {
                        case "1":
                            if (Player.Gold >= 1500)
                            {
                                Player.Gold -= 1500;
                                Player.CurrentHP = Math.Min(Player.CurrentHP + 100, Player.MaxHP);
                                Player.CurrentMP = Math.Min(Player.CurrentMP + 50, Player.MaxMP);

                                Console.WriteLine("\n체력이 회복되었습니다.");
                                Console.WriteLine($"현재 체력 : {Player.CurrentHP} / {Player.MaxHP}");
                                Console.WriteLine($"현재 마나 : {Player.CurrentMP} / {Player.MaxMP}");
                            }
                            else
                            {
                                Console.WriteLine("\nGold가 부족합니다.");
                            }
                            Console.WriteLine("\nEnter 키를 눌러 계속하세요.");
                            Console.ReadLine();
                            rest = true;
                            break;
                        case "0":
                            restConfirmed = true;
                            rest = true;
                            break;
                        default:
                            Console.WriteLine("\n잘못된 입력입니다.");
                            Thread.Sleep(1000);
                            rest = true;
                            break;
                    }
                }
            }
        }
    }
}
