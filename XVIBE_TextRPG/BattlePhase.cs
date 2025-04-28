using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVIBE_TextRPG
{
    internal class Battlephase
    {
        static public void EnterDungeon()
        {
            Console.Clear();
            Console.WriteLine("던전 선택");//초급 중급 고급 선택    
            Console.WriteLine("1.초급 던전");
            Console.WriteLine("2.중급 던전");
            Console.WriteLine("3.고급 던전");
            Console.WriteLine("0.마을로 돌아가기");
            Console.Write(">>");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("초급 던전으로 입장합니다.");
                    if(Player.CurrentHP == 0)
                    {
                        Console.WriteLine("체력이 부족하여 던전에 입장할 수 없습니다.\n");
                        Console.WriteLine("계속하려면 Enter를 누르세요...");
                        Console.ReadLine(); // 멈춰줌
                        return;
                    }
                    Player.UpdateStats();
                    Console.WriteLine("던전 입장 중...");
                    System.Threading.Thread.Sleep(2000); // 2초 대기
                    EasyDungeon dungeon = new EasyDungeon(); 

                    break;
                case "2":
                    Console.WriteLine("중급 던전으로 입장합니다.");
                    if (Player.CurrentHP == 0)
                    {
                        Console.WriteLine("체력이 부족하여 던전에 입장할 수 없습니다.\n");
                        Console.WriteLine("계속하려면 Enter를 누르세요...");
                        Console.ReadLine(); // 멈춰줌
                        return;
                    }
                    Player.UpdateStats();
                    Console.WriteLine("던전 입장 중...");
                    System.Threading.Thread.Sleep(2000); // 2초 대기
                    NormalDungeon normalDungeon = new NormalDungeon();

                    break;
                case "3":
                    Console.WriteLine("고급 던전으로 입장합니다.");
                    if (Player.CurrentHP == 0)
                    {
                        Console.WriteLine("체력이 부족하여 던전에 입장할 수 없습니다.\n");
                        Console.WriteLine("계속하려면 Enter를 누르세요...");
                        Console.ReadLine(); // 멈춰줌
                        return;
                    }
                    Player.UpdateStats();
                    Console.WriteLine("던전 입장 중...");
                    System.Threading.Thread.Sleep(2000); // 2초 대기
                    HardDungeon hardDungeon = new HardDungeon();

                    break;
                case "0":
                    Console.WriteLine("마을로 돌아갑니다.");
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    break;
            }
        }
    }
}
