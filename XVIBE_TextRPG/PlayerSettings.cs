using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVIBE_TextRPG
{
    internal class PlayerSettings
    {
        // 이름 설정과 직업 선택을 마치고 메인 메뉴를 보여주는 메서드
        public static void InitializePlayer()
        {
            SetPlayerName(); // 이름 설정
            ChooseJob();     // 직업 선택

            Console.WriteLine($"직업 선택이 완료되었습니다.{Player.Name}님의 직업은 {Player.Job}입니다.");
            Player.SavePlayerData();
            Thread.Sleep(2000);
        }
        public static string SetPlayerName()
        {
            bool nameConfirmed = false;

            while (!nameConfirmed) // 사용자 이름을 입력하고 확정할 때까지 반복하는 루프
            {
                Console.Clear();
                Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");
                Console.WriteLine("원하시는 이름을 설정해주세요.");
                Console.WriteLine();
                Console.Write(">>");
                Player.Name = Console.ReadLine()!;

                bool validChoice = false;

                while (!validChoice) // 입력한 이름이 유효한지 확인하고, 유효하지 않다면 재입력을 요청하는 루프
                {
                    Console.Clear();
                    Console.WriteLine($"입력하신 이름은 '{Player.Name}' 입니다.");
                    Console.WriteLine();
                    Console.WriteLine("1. 맞다");
                    Console.WriteLine("2. 아니다");
                    Console.WriteLine();
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    Console.Write(">>");
                    string choice = Console.ReadLine()!;


                    
                    if (choice == "2")
                    {
                        Console.WriteLine("다시 이름을 설정합니다...");
                        Thread.Sleep(1000);
                        validChoice = true; // 안쪽 루프 종료 -> 바깥쪽 루프부터 다시 시작
                    }
                    else if (Player.Name.Length > 8)
                    {
                        Console.WriteLine("이름은 8자 이내로 설정해주세요.");
                        Thread.Sleep(1000);
                        validChoice = true;
                    }
                    else if (Player.Name.Length == 0)//공백 고려
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000); // 안쪽 루프 계속 반복
                        validChoice = true;
                    } 
                    else if(choice == "1")
                    {
                        nameConfirmed = true; // 바깥쪽 루프 종료
                        validChoice = true; // 안쪽 루프 종료
                        Console.WriteLine();
                        Console.WriteLine($"'{Player.Name}'님, 환영합니다.");
                        Console.WriteLine();
                        Console.WriteLine("계속하려면 Enter 키를 누르세요...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000); // 안쪽 루프 계속 반복
                        validChoice = true;
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("이름 설정이 완료되었습니다.");
            return Player.Name; // 최종적으로 설정된 이름 반환

        }

        public static string ChooseJob()
        {
            bool jobValid = false;

            while (!jobValid) // 바깥쪽 루프
            {
                Console.Clear();
                Console.WriteLine($"{Player.Name}님 당신의 직업을 선택해주세요.");
                Console.WriteLine();
                Console.WriteLine("1. 전사");
                Console.WriteLine("2. 도적");
                Console.WriteLine("3. 마법사");
                Console.WriteLine();
                Console.WriteLine("원하시는 직업을 선택해주세요.");
                Console.Write(">>");
                string jobChoice = Console.ReadLine()!;

                switch (jobChoice)
                {
                    case "1":
                        Player.Job = "전사";
                        break;
                    case "2":
                        Player.Job = "도적";
                        break;
                    case "3":
                        Player.Job = "마법사";
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000);
                        continue;
                }

                bool confirmValid = false;
                while (!confirmValid) // 안쪽 루프
                {
                    Console.Clear();
                    switch (Player.Job)
                    {
                        case "전사":
                            Console.WriteLine("전사는 높은 체력과 힘을 기반으로 싸우는 직업입니다.");
                            Console.WriteLine("다른 직업에 비해 민첩과 지능이 낮습니다.");
                            break;
                        case "도적":
                            Console.WriteLine("도적은 높은 민첩과 힘을 기반으로 싸우는 직업입니다.");
                            Console.WriteLine("다른 직업에 비해 체력과 방어력이 낮습니다.");
                            break;
                        case "마법사":
                            Console.WriteLine("마법사는 높은 지능과 마법력을 기반으로 싸우는 직업입니다.");
                            Console.WriteLine("다른 직업에 비해 체력과 방어력이 낮습니다.");
                            break;
                    }

                    Console.WriteLine();
                    Console.WriteLine("당신의 운명을 선택하시겠습니까?");
                    Console.WriteLine("1. 선택");
                    Console.WriteLine("2. 취소");
                    Console.WriteLine();
                    Console.Write(">>");
                    string confirm = Console.ReadLine()!;

                    if (confirm == "1")
                    {
                        jobValid = true; // 바깥쪽 루프 종료
                        confirmValid = true; // 안쪽 루프 종료
                    }
                    else if (confirm == "2")
                    {
                        Console.WriteLine("직업을 다시 선택합니다...");
                        Thread.Sleep(1000);
                        confirmValid = true; // 안쪽 루프만 종료, 바깥쪽 루프 다시 시작
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000);
                        // 루프 종료 없음 안쪽 루프 다시시작
                    }
                }
            }

            Console.Clear();
            Player.UpdateStats(); // 직업에 따라 스탯 업데이트
            return Player.Job;
        }
    }
}
 
