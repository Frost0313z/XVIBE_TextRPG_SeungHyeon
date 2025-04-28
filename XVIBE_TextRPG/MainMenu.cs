using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVIBE_TextRPG
{
    internal class MainMenu
    {
        // Main menu class
        static public void ShowMainMenu()
        {
            while (true)
            {
                Quest.CheckQuestConditions(); // 퀘스트 상태 메인 메뉴에서 계속 최신화

                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전 입장");
                Console.WriteLine("5. 휴식");
                Console.WriteLine("6. 퀘스트");
                Console.WriteLine("0. 종료하기");
                Console.WriteLine("8. 게임 데이터 저장");
                Console.WriteLine("9. 게임 데이터 삭제");
                Console.WriteLine();
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {

                    case "1":// 스테이터스
                        Console.Clear();
                        Player.ShowStatus();
                        break;
                    case "2":
                        Console.Clear();
                        Equipment.ShowInventory();
                        break;
                    case "3":
                        Console.Clear();
                        Shop.EnterShop();
                        break;
                    case "4"://던전 입장 (던전 입장 이외의 메서드들은 생략)
                        Battlephase.EnterDungeon();
                        break;
                    case "5":
                        RestSystem.Rest();
                        break;
                    case "6":
                        Quest.ShowQuestList();
                        break;
                    case "0":
                        Console.WriteLine("게임을 종료합니다.");
                        return; // 프로그램 종료
                    case "8": // 게임 저장 (8말고 다른숫자나 기호 사용해도 무방합니다)
                        Player.SavePlayerData();
                        Console.WriteLine("Enter 키를 눌러주세요.");
                        Console.ReadLine();
                        break;
                    case "9": // 저장 데이터 삭제 (9 말고 다른숫자나 기호 사용해도 무방합니다)
                        SaveSystem.DeleteSave();
                        Console.WriteLine("Enter 키를 눌러주세요.");
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Console.WriteLine("Enter 키를 눌러주세요.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
    
    }

