using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace XVIBE_TextRPG
{
    internal static class SaveSystem
    {
        private static string savePath = "save.json";

        public static void Save(GameData data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented); //GameData 클래스 객체에 Json 문자열로 변환 후 json 변수에 담기
            File.WriteAllText(savePath, json);
            Console.WriteLine("[저장 완료]");
        }

        public static GameData Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                return JsonConvert.DeserializeObject<GameData>(json); // json 문자열을 GameData 클래스로 되돌리기
            }

            Console.WriteLine("저장 데이터가 없습니다.");
            return null;
        }

        public static void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Console.WriteLine("[저장 데이터 삭제]");
            }           
        }

        public static bool SaveExists() => File.Exists(savePath); // 세이브파일이 있는지 확인

        // 저장된 게임 불러오기 / 새로운 게임 시작하기
        public static void LoadOrNewGame()
        {
            string choice;

            do
            {
                Console.Clear();
                Console.WriteLine("저장된 게임을 불러오시겠습니까? (y = 불러오기 / n = 새 게임");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "y":
                        if (SaveSystem.SaveExists())
                        {
                            GameData loaded = SaveSystem.Load();
                            if (loaded != null)
                            {
                                Quest.QuestManager.QuestListDB(); // 퀘스트 리스트 초기화
                                Player.LoadPlayerData(loaded);
                                Console.WriteLine("[불러오기 완료]\nEnter를 눌러 계속하세요.");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine("저장된 데이터를 불러오는 데 실패했습니다.");
                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            Console.WriteLine("저장된 게임이 없습니다! 새 게임을 시작합니다.");
                            Thread.Sleep(1000);
                            PlayerSettings.InitializePlayer();
                            choice = "n";
                        }
                            break;
                    case "n":
                        Console.WriteLine("[새 게임을 시작합니다]");
                        Thread.Sleep(1000);
                        PlayerSettings.InitializePlayer();
                        break;
                    default:
                        Console.WriteLine("y 또는 n 만 입력 가능합니다.");
                        Thread.Sleep(1000);
                        break;
                }
            }
            while (choice != "y" &&  choice != "n");
        }


    }
}
