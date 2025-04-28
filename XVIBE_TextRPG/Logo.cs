using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XVIBE_TextRPG
{
    internal class Logo
    {
        public static void LogoPrint()
        {
            string XVibe = "\r\n __   ____      _______ ____  ______ \r\n \\ \\ / /\\ \\    / /_   _|  _ \\|  ____|\r\n  \\ V /  \\ \\  / /  | | | |_) | |__   \r\n   > <    \\ \\/ /   | | |  _ <|  __|  \r\n  / . \\    \\  /   _| |_| |_) | |____ \r\n /_/ \\_\\    \\/   |_____|____/|______|\r\n                                     \r\n                                     \r\n";

            foreach (char c in XVibe)
            {
                Console.Write(c);
                Thread.Sleep(1); // 타자 효과
            }

            string teamMember = "팀장:변재웅, 팀원:서정원, 두승현, 정광훈, 조혜령";

            foreach (char c in teamMember)
            {
                Console.Write(c);
                Thread.Sleep(25); // 타자 효과
            }

            Thread.Sleep(1750);
            Console.Clear();
        }

        public static void Intro()
        {
            Console.WriteLine("...언제부터인가, 세계 곳곳에 '던전'이 나타나기 시작했다.");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("수많은 이들이 목숨을 걸고 그 안으로 들어갔고,");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("그 끝엔 보물도, 절망도 기다리고 있었다.");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("던전 주변엔 사람과 시장이 모여들었고,");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("그리하여 하나의 도시가 형성되었다.");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("사람들은 그곳을 이렇게 불렀다...");
            Thread.Sleep(1000);

            Console.WriteLine();
            Console.WriteLine("<<던전도시>>");
            Thread.Sleep(1000);

            Console.Clear();
        }

    }
}