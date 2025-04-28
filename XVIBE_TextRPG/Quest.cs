using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* [작성한 것들]
 * - 퀘스트 클래스 생성 및 속성 정의
 * - 퀘스트 완료 조건 판정 로직 
 * - 반복 퀘스트의 경우 상태 리셋 함수
 * - 퀘스트 정보 및 보상 조건 정리 리스트 생성
 * - 퀘스트 목록 출력 기능 
 * - 퀘스트 상세 정보 출력 기능
 * - 퀘스트 수락/거절 시스템
 * - 퀘스트 진행 중 목록 보기 기능
 * - 퀘스트 진행 조건 자동 체크 기능
 * - 퀘스트 보상 지급 처리
 * - 플레이어 행동과 연동하여 퀘스트 조건 달성 확인 기능 기존 코드에 적용하기
 * 
 * [작성해야 하는 것들]
 * - 퀘스트 상태 저장/로드 기능
*/

namespace XVIBE_TextRPG
{
    public enum QuestStatus { NotAccepted, InProgress, Completed, Finished} // 퀘스트 상태를 정의하는 열거형, 퀘스트 상태를 표시하는데 사용함
    public class Quest
    {
        // [정보]
        public int Index; // 퀘스트 번호
        public string QuestName; // 퀘스트 이름
        public string Description; // 상세 설명
        public QuestStatus Status; // 퀘스트 상태 (미수락, 진행중, 완료)
        public bool IsRepeatable; // 반복 퀘스트인가?
        public static List<Quest> questList = new List<Quest>(); // 퀘스트 정보 및 보상을 정리하는 리스트

        // [조건]
        public int RequiredKillCount; // 필요 몬스터 처치 수 (조건)
        public static int CurrentKillCount; // 현재 처치한 몬스터
        public int Required_Level; // 필요 레벨
        public int Required_TotalAtk; // 필요 공격력
        public int Required_TotalDef; // 필요 방어력

        // [보상]
        public int RewardGold; // 보상 골드
        public int RewardExp; // 보상 경험치
        public Equipment.Weapon RewardWeapon; // 장비보상 무기
        public int RewardWeapon_Count; // 보상 무기 수량

        public bool IsCompleted
        {
            get
            {
                if (Index == 1 && RequiredKillCount > 0 && CurrentKillCount >= RequiredKillCount)
                    return true;

                if (Index == 2 && Equipment.EquippedWeapon != null)
                    return true;

                if (Index == 3 && Required_TotalAtk > 0 && Player.TotalATK >= Required_TotalAtk)
                    return true;

                if (Index == 4 && Required_TotalDef > 0 && Player.TotalDEF >= Required_TotalDef)
                    return true;

                if (Index == 5 && Required_Level > 0 && Player.Level >= Required_Level)
                    return true;

                return false;
            }
        }

        public class QuestManager
        {
            // [퀘스트 보상 밸런스 기준표]

            // ▶ 1. 레벨업 기준
            //    - 1레벨 업에 필요한 경험치: 100 EXP 기준으로 설계
            //    - 1~2개 퀘스트 또는 1~2회 던전으로 1레벨 업 가능하도록 조정

            // ▶ 2. 경험치 보상 계산 공식
            //    - 반복 퀘스트: 평균 몬스터 처치 EXP × 킬 수 × 0.8
            //    - 일반 퀘스트: 킬 수 × 6~10 정도, 조건 난이도 따라 차등
            //    - 스탯 조건형 퀘스트(EX: 공격력 30, 방어력 20 등)는 200~300 EXP 제공
            //    - 레벨 조건 퀘스트(EX: Lv3 도달)는 100 EXP 전후로 조정

            // ▶ 3. 골드 보상 기준
            //    - 반복 퀘스트: 골드 없음
            //    - 일반 퀘스트: 보상 EXP × 10 = 골드
            //    - 스탯/레벨 조건 퀘스트는 도달 난이도 고려해 300~700G 배정

            // ▶ 4. 무기 보상
            //    - 보급 무기(EX: 강철검)는 초반 반복 퀘스트나 튜토리얼성 퀘스트에 한해 지급
            //    - 중급 이상 장비는 중후반 스탯/레벨 조건 퀘스트 보상으로만 지급

            // ※ 보상 수치는 플레이어 성장 흐름(레벨, 장비, 재화) 속도를 고려해 조정함


            public static void QuestListDB()
            {
                if (questList.Count > 0) return; // 초기화 된 경우면 다시 안하도록 설정

                questList.Add(new Quest
                {
                    Index = 1,
                    QuestName = "마을을 위협하는 몬스터 처치",
                    Description = "이봐! 마을 근처에 몬스터들이 너무 많아졌다고 생각하지 않나? \n마을주민들의 안전을 위해서라도 저것들 수를 좀 줄여야 한다고! \n모험가인 자네가 좀 처치해주게!",
                    RequiredKillCount = 10, // 아무 몬스터나 5마리 처치하면 퀘스트 클리어
                    RewardGold = 0, // 반복 퀘스트는 보상이 없음
                    RewardExp = 50,
                    RewardWeapon = Shop.storeWeapons[0], // 강철검을 보상으로 준다
                    RewardWeapon_Count = 1, // 강철검 하나를 보상으로 준다
                    Status = QuestStatus.NotAccepted,
                    IsRepeatable = true // 반복 가능한 퀘스트임
                });

                questList.Add(new Quest
                {
                    Index = 2,
                    QuestName = "장비를 장착해보자",
                    Description = "세상에.. 자네 아직도 장비 없이 맨손으로 싸우는 건 아니겠지?? \n어서 빨리, 상점에서 장비를 구매해서 장착해보게나 \n모험은 위험하니 철저히 준비하게!",
                    RewardGold = 100,
                    RewardExp = 30,
                    Status = QuestStatus.NotAccepted, // Equipment.EquippedWeapon != null 요놈 활용하여 장비 장착하면 클리어
                    IsRepeatable = false
                });

                questList.Add(new Quest
                {
                    Index = 3,
                    QuestName = "더욱 더 강해지기!",
                    Description = "강한 몬스터를 상대하기 위해서 수련을 게을리 하지 말게! \n상위 장비를 장착하거나 레벨을 올려서 강해지고 오게나 \n건승을 빌겠네...!",
                    Required_TotalAtk = 30, // 장비 장착 포함 플레이어의 공격력이 30이상이면 클리어 가능
                    RewardGold = 600,
                    RewardExp = 300,
                    Status = QuestStatus.NotAccepted,
                    IsRepeatable = false
                });

                questList.Add(new Quest
                {
                    Index = 4,
                    QuestName = "아픈건 싫으니까 방어력에 올인하고자 합니다!",
                    Description = "강한 자는 단순히 때리는 자가 아니라, 맞아도 쓰러지지 않는 자일세! \n상위 장비를 장착하거나 레벨을 올려서 단단해지고 오게나 \n건승을 빌겠네...!",
                    Required_TotalDef = 20, // 장비 장착 포함 플레이어의 방어력이 20이상이면 클리어 가능
                    RewardGold = 500,
                    RewardExp = 250,
                    Status = QuestStatus.NotAccepted,
                    IsRepeatable = false
                });

                questList.Add(new Quest
                {
                    Index = 5,
                    QuestName = "니 렙에 잠이 오냐?",
                    Description = "렙 2짜리가 잠을 자? \n지금 자면 내일도 몬스터한테 맞는다. \n지금 렙업하면, 내일은 자네가 몬스터를 때린다...!!",
                    Required_Level = 3, // 플레이어의 레벨이 3이상이면 클리어 가능
                    RewardGold = 300,
                    RewardExp = 100,
                    Status = QuestStatus.NotAccepted,
                    IsRepeatable = false
                });
            }
        }
        public static void ShowQuestList() 
        {
            Console.Clear();

            if (questList.Count == 0)
            {
                QuestManager.QuestListDB(); // 퀘스트 데이터 추가
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 퀘스트 목록 ===== \n");

                for (int i = 0; i < questList.Count; i++)
                {
                    Quest quest = questList[i]; // 퀘스트 리스트에 있는 퀘스트 하나를 꺼낸다

                    string statusText = quest.Status switch // 퀘스트 상태에 따라서 변환시킴
                    {
                        QuestStatus.NotAccepted => "미수락",
                        QuestStatus.InProgress => "진행중",
                        QuestStatus.Completed => "보상",
                        QuestStatus.Finished => "완료"
                    };

                    ConsoleColor statusColor = quest.Status switch
                    {
                        QuestStatus.NotAccepted => ConsoleColor.Gray,
                        QuestStatus.InProgress => ConsoleColor.Cyan,
                        QuestStatus.Completed => ConsoleColor.Yellow,
                        QuestStatus.Finished => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    };

                    string Reapeatable_Quest = quest.IsRepeatable ? "[반복]" : "[----]";

                    Console.Write($" {quest.Index}.");
                    Console.ForegroundColor = statusColor;
                    Console.Write($" [{statusText}]");
                    Console.ResetColor();

                    Console.WriteLine($" {Reapeatable_Quest} {quest.QuestName}");
                }

                Console.WriteLine("\n※ 번호를 입력하면 상세 정보를 확인할 수 있습니다.");
                Console.WriteLine("※ 0을 입력하면 목록에서 나갑니다.");
                Console.WriteLine("※ 퀘스트 정보를 갱신하려면 메인 메뉴로 나갔다가 다시 들어오세요.");
                Console.Write(">> ");

                string input = Console.ReadLine();

                if (input == "0") { break; }

                if (int.TryParse(input, out int selectedIndex)) // 숫자만 입력가능하도록 + 입력한 숫자로 퀘스트 선택가능하도록 
                {
                    Quest selectedQuest = null; // 퀘스트 상세보기를 위해서 퀘스트 타입의 변수가 필요함

                    foreach (Quest Num in questList) 
                    {
                        if (Num.Index == selectedIndex) // 선택한 번호랑 인덱스가 같은 퀘스트를 하나하나 찾아라
                        {
                            selectedQuest = Num; // 같은 타입이니까 대입해도 ㄱㅊㄱㅊ
                            break;
                        }
                    }

                    if (selectedIndex > 0)
                    {
                        ShowQuestDetail(selectedQuest); // 상세 정보 출력
                    }
                    else
                    {
                        Console.WriteLine("해당 번호의 퀘스트는 존재하지 않습니다.");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Thread.Sleep(1000); // 1초 대기후 반복문 돌기
                }
                
            }
        }

        public static void ShowQuestDetail(Quest quest) // 퀘스트 상세 보기 함수
        {
            
            Console.Clear();
            Console.WriteLine($"[{quest.QuestName}]\n");
            Console.WriteLine($"{quest.Description}");

            // [퀘스트 조건]
            if (quest.RequiredKillCount > 0)
            { Console.WriteLine($"\n※ 달성 조건: 몬스터 처치 {CurrentKillCount} / {quest.RequiredKillCount}"); }

            if (quest.Index == 2)
            {
                string equippedText = Equipment.EquippedWeapon != null ? "장착 완료" : "미장착";
                Console.WriteLine($"\n※ 달성 조건: 무기 장착 여부 - {equippedText}");
            }
            
            if (quest.Required_TotalAtk > 0)
            { Console.WriteLine($"\n※ 달성 조건: 공격력 {Player.TotalATK} / {quest.Required_TotalAtk}"); }

            if (quest.Required_TotalDef > 0)
            { Console.WriteLine($"\n※ 달성 조건: 방어력 {Player.TotalDEF} / {quest.Required_TotalDef}"); }

            if (quest.Required_Level > 0)
            { Console.WriteLine($"\n※ 달성 조건: 레벨 {Player.Level} / {quest.Required_Level}"); }


            // [퀘스트 보상]
            Console.WriteLine($"\n- 보상: 골드 {quest.RewardGold}, 경험치 {quest.RewardExp}");
            if (quest.RewardWeapon != null)
            {
                Console.WriteLine($"- 장비 보상: {quest.RewardWeapon.Name} x{quest.RewardWeapon_Count}");
            }

            Console.WriteLine("\n===============================================");

            if (quest.Status == QuestStatus.NotAccepted) // 퀘스트가 수락중이 아닐때는
            {
                Console.Write("이 퀘스트를 수락하시겠습니까? (Y/N) \n>> ");
                string input = Console.ReadLine().Trim().ToUpper(); // 문자열 앞뒤 공백 제거 및 대문자화 -> 예외처리 용도

                if (input == "Y")
                {
                    quest.Status = QuestStatus.InProgress;
                    Console.WriteLine("\n퀘스트를 수락하였습니다!");
                }
                else
                {
                    Console.WriteLine("\n퀘스트를 수락하지 않았습니다.");
                }
            }
            else if (quest.Status == QuestStatus.InProgress) // 퀘스트가 진행중일때는
            {
                Console.WriteLine("\n이 퀘스트는 현재 진행 중입니다.\n진행을 중단하시겠습니까? (Y/N) >> ");
                string input = Console.ReadLine().Trim().ToUpper();

                if (input == "Y")
                {
                    quest.Status = QuestStatus.NotAccepted;
                    Console.WriteLine("\n퀘스트를 진행을 취소하였습니다!");
                }
                else
                {
                    Console.WriteLine("\n퀘스트를 계속 진행합니다!");
                }
            }
            else if (quest.Status == QuestStatus.Completed) // 완료한 퀘스트인 경우에는
            {
                GiveQuestReward(quest);

                if (quest.IsRepeatable) // 반복 퀘스트는 보상을 받고 난 뒤에 다시 수락하는 방식
                {
                    Console.WriteLine("\n해당 퀘스트는 반복 진행이 가능합니다! \n퀘스트를 다시 수락합니까? (Y/N) >> ");
                    string input = Console.ReadLine().Trim().ToUpper();
                    if (input == "Y")
                    {
                        CurrentKillCount = 0; // 현재 처치한 몬스터도 초기화한다
                        quest.Status = QuestStatus.InProgress; // 퀘스트의 상태를 진행 상태로 리셋한다
                        Console.WriteLine("\n해당 퀘스트를 다시 수락하여 진행합니다!");
                    }
                    else
                    {
                        CurrentKillCount = 0;
                        quest.Status = QuestStatus.NotAccepted;
                        Console.WriteLine("\n퀘스트 수락을 취소하였습니다.");
                    }
                }
                else
                {
                    Console.WriteLine("\n이 퀘스트는 완료되었습니다. 다른 퀘스트를 선택하세요!");
                }
            }

            Console.WriteLine("\n계속하려면 Enter를 누르세요...");
            Console.ReadLine();
        }

        public static void CheckQuestConditions() // 퀘스트 달성조건 확인 함수 , 몬스터 처치후, 장비 장착후, 레벨업 직후 조건 검사하는 용도로 작성
        {
            foreach (var quest in questList)
            {
                if (quest.Status != QuestStatus.InProgress) continue;

                // 장비 장착 퀘스트
                if (quest.Index == 2 && Equipment.EquippedWeapon != null)
                {
                    quest.Status = QuestStatus.Completed;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n*** 퀘스트 완료 ***\n{quest.QuestName}");
                    Console.ResetColor();
                    continue;
                }

                if (quest.Status == QuestStatus.InProgress && quest.IsCompleted) // 진행중인 퀘스트이면서 IsCompleted가 참이면(퀘스트 조건 달성하면)
                {
                    quest.Status = QuestStatus.Completed; // 완료로 바꿔!

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n*** 퀘스트 완료 ***\n{quest.QuestName}"); // 가독성 있게 노란색으로 표시!
                    Console.ResetColor();
                }
            }
        }

        public static void GiveQuestReward(Quest quest) // 보상 지급 함수 상세 퀘스트 화면에서 지급할 생각
        {

            Console.WriteLine($"[{quest.QuestName}]을(를) 완료하여 보상 획득이 가능합니다.\n보상을 수령하시겠습니까? (Y/N) >> ");
            string input = Console.ReadLine().Trim().ToUpper();
            if (input == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n[보상 지급] 퀘스트 '{quest.QuestName}' 완료!");

                Player.Gold += quest.RewardGold; // 플레이어에게 골드 보상 지급
                Console.WriteLine($"+ {quest.RewardGold} Gold 획득!");

                Player.Exp += quest.RewardExp; // 플레이어에게 경험치 보상 지급
                Console.WriteLine($"+ {quest.RewardExp} EXP 획득!");

                if (quest.RewardWeapon != null && quest.RewardWeapon_Count > 0) // 보상 무기가 있다면?
                {
                    // 인벤토리에 무기가 보유한 상태인지 확인
                    bool alreadyOwned = Equipment.Inventory.Any(w =>
                    w.Name == quest.RewardWeapon.Name &&
                    w.ATK == quest.RewardWeapon.ATK &&
                    w.Price == quest.RewardWeapon.Price);

                    if (alreadyOwned)
                    {
                        int altRewardGold = quest.RewardWeapon.Price / 2; // 대체 보상 (보상 무기의 가격을 나눈 값)
                        Player.Gold += altRewardGold;
                        Console.WriteLine($"이미 {quest.RewardWeapon.Name}을 보유 중이라, 보상 대신 {altRewardGold}G 를 지급합니다.");
                    }
                    else
                    {
                        for (int i = 0; i < quest.RewardWeapon_Count; i++)
                        {
                            Equipment.Inventory.Add(quest.RewardWeapon); // 인벤토리 리스트(무기)에 추가하자
                        }
                        Console.WriteLine($"+ {quest.RewardWeapon.Name} ×{quest.RewardWeapon_Count} 획득!");
                    }
                }
                Console.ResetColor();

                if (quest.IsRepeatable)
                {
                    Quest.CurrentKillCount = 0; // 반복 퀘스트라면 초기화 후 다시 진행 가능
                    quest.Status = QuestStatus.NotAccepted;
                }
                else
                {
                    quest.Status = QuestStatus.Finished;
                }
            }
            else
            {
                Console.WriteLine("\n퀘스트 보상 수락을 취소하였습니다.");
                Console.WriteLine("※ 보상은 퀘스트 상세 보기에서 다시 수령할 수 있습니다.");
                Console.WriteLine("\n계속하려면 Enter를 누르세요...");
                Console.ReadLine();
            }
        }
    }
}
