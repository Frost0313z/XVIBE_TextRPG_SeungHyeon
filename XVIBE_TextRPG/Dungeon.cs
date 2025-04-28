using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static XVIBE_TextRPG.Enemy;
using static XVIBE_TextRPG.Equipment;
using static XVIBE_TextRPG.WeaponReward;

namespace XVIBE_TextRPG
{
    public class Dungeon // 부모 클래스 이녀석을 자식 클래스인 초급 중급 고급 던전 클래스에 상속시킬거임
    {
        protected List<Enemy> monsters; // 몬스터 리스트
        protected List<string> battleLog; // 배틀 로그 리스트
        protected List<Reward> rewards = new List<Reward>(); // 던전 클리어 보상 리스트
        private int expBeforeBattle; // 전투 시작 시 경험치 저장

        public Dungeon() // 생성자에서 보상 설정
        {
            rewards.Add(new GoldReward(GetGoldReward())); // 던전 클리어 골드 보상 rewards 리스트에 추가
        }

        public virtual List<Enemy> GenerateMonsters() // 자식이 정의할 수 있도록 가상 메서드 만들기
        {
            var random = new Random();
            var monsterList = new List<Enemy>();

            for (int i = 0; i < 3; i++) // 3마리 생성
            {
                int type = random.Next(0, 3); // 0~2 타입 랜덤 생성
                int level = random.Next(1, 4); // 1~3 레벨 랜덤 생성
                monsterList.Add(new Enemy(type, level));
            }

            return monsterList;
        }

        public virtual void ShowEnterMessage() // 자식이 정의함
        {
            Console.WriteLine("초급 던전에 입장합니다...");
        }

        public void Enter()
        {
            if (monsters == null) // 몬스터 생성이 안된다고 오류나서 방어코드 작성
                monsters = GenerateMonsters();
            if (battleLog == null)
                battleLog = new List<string>();

            ShowEnterMessage();
            StartDungeon();
        }

        // 던전 시작 메서드
        public void StartDungeon()
        {
            while (true)
            {
                // 콘솔 클리어 후 플레이어 상태 출력
                Console.Clear();
                DisplayPlayerStatus();

                // 몬스터 상태 출력
                DisplayMonsters();
                Console.WriteLine("===== 행동 선택 =====");
                Console.WriteLine("1. 공격하기");
                Console.WriteLine("2. 소모품 사용");
                Console.WriteLine("0. 후퇴");
                Console.Write(">> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": // 공격하기
                        bool endBattle = TargetEnemy(); // 기존의 StartDungeon 메서드 호출
                        if (endBattle)
                            return;
                        break;
                    case "2": // 소모품 사용
                              // 콘솔 클리어 후 플레이어 상태 출력
                        Console.Clear();
                        DisplayPlayerStatus();

                        // 몬스터 상태 출력
                        DisplayMonsters();
                        Consumable.UseConsumable();
                        break;
                    case "0": // 후퇴
                        Console.WriteLine("던전에서 후퇴합니다.");
                        Player.EndTurn(); // 전투 종료 시 초기화
                        return; // 메서드 종료
                    default:
                        Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
                        break;
                }

                Console.WriteLine("\nEnter를 눌러 계속...");
                Console.ReadLine();
            }
        }
        public bool TargetEnemy()
        {
            // 전투 시작 직전 경험치 저장
            expBeforeBattle = Player.Exp;

            if (monsters == null || monsters.Count == 0)
            {
                Console.WriteLine("에러: 몬스터가 생성되지 않았습니다.");
                return false;
            }

            while (true)
            {
                // 콘솔 클리어 후 플레이어 상태 출력
                Console.Clear();
                DisplayPlayerStatus();

                // 몬스터 상태 출력
                DisplayMonsters();

                // 공격 대상 선택
                Console.WriteLine("공격할 몬스터를 선택하세요 (1-4). 0을 누르면 행동 선택으로 되돌아갑니다.");
                Console.Write(">> ");
                string targetInput = Console.ReadLine();
                if (targetInput == "0")
                {
                    Console.WriteLine("행동 선택으로 되돌아갑니다.");
                    break;
                }

                if (!int.TryParse(targetInput, out int targetIndex) || targetIndex < 1 || targetIndex > monsters.Count)
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
                    continue;
                }

                Enemy target = monsters[targetIndex - 1];

                // 행동 선택
                Console.WriteLine("행동을 선택하세요:");
                Console.WriteLine("1. 기본 공격");
                Console.WriteLine("2. 스킬 사용");
                Console.WriteLine("0. 대상 재지정");
                Console.Write(">> ");
                string action = Console.ReadLine();

                if (action == "1")
                {
                    BasicAttack(target);
                }
                else if (action == "2")
                {
                    UseSkill(target);
                }
                else if (action == "0")
                {
                    Console.WriteLine("대상을 재지정합니다.");
                    continue;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.");
                    continue;
                }

                // 몬스터 공격 처리
                MonsterAttack();

                // 배틀 로그 출력
                DisplayBattleLog();

                // 경험치 획득
                foreach (var monster in monsters)
                {
                    if (monster.Dead) // 적이 죽었을 경우
                    {
                        if (!monster.NotGetExperience) // 경험치 지급을 안 했을경우
                        {
                            GetExperience(monster);
                            monster.NotGetExperience = true; // 경험치 지급 -완-
                        }
                    }
                }

                // 레벨업
                Player.LvUp();

                // 전투 결과 확인
                if (Player.CurrentHP <= 0 && monsters.TrueForAll(m => m.IsDead()))
                {
                    Console.WriteLine("플레이어와 몬스터가 동시에 쓰러졌습니다. 플레이어의 승리로 간주됩니다!");
                    BattleVictory();
                    Player.EndTurn(); // 전투 종료 시 공격력 초기화
                    return true;
                }
                else if (Player.CurrentHP <= 0)
                {
                    BattleDefeat();
                    Player.EndTurn(); // 전투 종료 시 공격력 초기화
                    return true;
                }
                else if (monsters.TrueForAll(m => m.IsDead()))
                {
                    BattleVictory();
                    return true;
                }
            }
            return false;
        }

        // 플레이어 상태 출력
        public void DisplayPlayerStatus()
        {
            Console.WriteLine($"플레이어 상태: HP: {Player.CurrentHP}/{Player.MaxHP}, MP: {Player.CurrentMP}/{Player.MaxMP}");
            Console.WriteLine($"직업: {Player.Job}, 레벨: {Player.Level}, 경험치: {Player.Exp}");
            Console.WriteLine($"공격력: {Player.GetCurrentATK()}, 방어력: {Player.TotalDEF}, 추가 회피율: {Player.AdditionalEvasionRate*100}%");
            Console.WriteLine(new string('-', 40)); // 구분선
        }

        // 몬스터 상태 출력
        public void DisplayMonsters()
        {
            Console.WriteLine("현재 몬스터 상태:");
            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];
                Console.WriteLine($"{i + 1}. {monster.Name} (레벨: {monster.Level}, HP: {monster.CurrentHP}/{monster.MaxHP}, DEF: {monster.DEF})");
            }
            Console.WriteLine(new string('-', 40)); // 구분선
        }

        // 기본 공격 메서드
        private void BasicAttack(Enemy target)
        {
            float damage = Player.GetCurrentATK();

            if (target.IsDead())
            {
                Console.WriteLine($"{target.Name}은(는) 이미 쓰러졌습니다.");
                return;
            }
            else if (Combat.IsMiss(false)) // 몬스터를 공격할 때
            {
                battleLog.Add($"{target.Name}을(를) 공격했지만 아무일도 일어나지 않았습니다....");
            }
            else if (Combat.IsCriticalHit()) // 조건문 걸어서 치명타 터지는 상황 아닌 상황 나누기
            {
                int criticalDamage = target.TakeCriticalDamage((int)damage);// 몬스터에게 치명타 데미지 피해
                battleLog.Add($"플레이어가 {target.Name}에게 {criticalDamage}의 [치명타] 피해를 입혔습니다!!!");

                if (target.IsDead()) 
                { 
                    Quest.CurrentKillCount += 1; 
                }
                Quest.CheckQuestConditions();
            }
            else
            {
                target.TakeDamage((int)damage); // 일반 공격
                battleLog.Add($"플레이어가 {target.Name}에게 {damage}의 피해를 입혔습니다.");

                if (target.IsDead()) 
                {
                    Quest.CurrentKillCount += 1;
                }
                Quest.CheckQuestConditions();
            }

            Player.EndTurn(); // 전투 종료 시 공격력 버프 초기화
        }

        // 스킬 사용 메서드
        private void UseSkill(Enemy target)
        {
            if (Player.Job == "전사")
            {
                Player.WarriorSkill();
                battleLog.Add("플레이어가 전사의 스킬을 사용하여 이번 턴 동안 공격력을 증가시켰습니다!");
            }
            else if (Player.Job == "도적")
            {
                Player.ThiefSkill(target);
                battleLog.Add($"플레이어가 도적의 스킬을 사용하여 {target.Name}에게 큰 피해를 입혔습니다!");

                if (target.IsDead())
                {
                    Quest.CurrentKillCount += 1;
                }
                Quest.CheckQuestConditions();
            }
            else if (Player.Job == "마법사")
            {
                Player.MageSkill(monsters.ToArray());
                battleLog.Add("플레이어가 마법사의 스킬을 사용하여 모든 적에게 피해를 입혔습니다!");

                foreach (var monster in monsters)
                {
                    if (target.IsDead())
                    {
                        Quest.CurrentKillCount += 1;
                    }
                }
                Quest.CheckQuestConditions();
            }
            else
            {
                Console.WriteLine("스킬 없음");
            }
        }

        // 몬스터 공격 처리
        private void MonsterAttack()
        {
            int totalDamage = 0;

            foreach (var monster in monsters)
            {
                if (monster.IsDead())
                {
                    continue;   // 몬스터가 죽으면 그냥 진행해라
                }
                else if (Combat.IsMiss()) // 10퍼센트 확률로 회피하는 경우 회피 판정해라
                {
                    battleLog.Add($"플레이어가 {monster.Name}의 공격을 [회피]했습니다!");
                }
                else if (!monster.IsDead()) // 죽지도 않고 회피 판정도 필요없으면 데미지를 추가해라!
                {
                    totalDamage += monster.ATK;
                }
            }
            Player.CurrentHP = Math.Max(Player.CurrentHP - totalDamage, 0);
            battleLog.Add($"몬스터들이 공격하여 플레이어가 {totalDamage}의 피해를 입었습니다. 남은 HP: {Player.CurrentHP}");
        }

        // 배틀 로그 출력
        private void DisplayBattleLog()
        {
            Console.WriteLine(new string('-', 40)); // 구분선
            Console.WriteLine("배틀 로그:");
            foreach (var log in battleLog)
            {
                Console.WriteLine(log);
            }
            battleLog.Clear(); // 로그 초기화
            Console.WriteLine(new string('-', 40)); // 구분선
            Console.WriteLine("아무 키나 눌러 다음 턴으로 진행하세요...");
            Console.ReadLine();
        }


        // 전투 시작 시 경험치 저장
        private int totalExpGained = 0;

        // 전투 승리 메서드
        public void BattleVictory()
        {
            Console.WriteLine(new string('-', 40)); // 구분선
            Console.WriteLine(@"
 ___      ___ ___  ________ _________  ________  ________      ___    ___ ___       
|\  \    /  /|\  \|\   ____\\___   ___\\   __  \|\   __  \    |\  \  /  /|\  \      
\ \  \  /  / | \  \ \  \___\|___ \  \_\ \  \|\  \ \  \|\  \   \ \  \/  / | \  \     
 \ \  \/  / / \ \  \ \  \       \ \  \ \ \  \\\  \ \   _  _\   \ \    / / \ \  \    
  \ \    / /   \ \  \ \  \____   \ \  \ \ \  \\\  \ \  \\  \|   \/  /  /   \ \__\   
   \ \__/ /     \ \__\ \_______\  \ \__\ \ \_______\ \__\\ _\ __/  / /      \|__|   
    \|__|/       \|__|\|_______|   \|__|  \|_______|\|__|\|__|\___/ /           ___ 
                                                             \|___|/           |\__\
                                                                               \|__|
                                                                                    
");
            Console.WriteLine();
            GetRewards(); // 클리어 보상 호출
            Console.WriteLine($"보상으로 {GetGoldReward()} G를 획득했습니다.");
            Console.WriteLine($"총 경험치 {totalExpGained}를 획득했습니다.");
            Quest.CheckQuestConditions();
            Player.SavePlayerData();
            Console.WriteLine();
            Console.WriteLine("Enter 키를 눌러주세요.");
            Console.ReadLine();
        }

        // 전투 패배 메서드
        public void BattleDefeat()
        {
            Thread.Sleep(500);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
 ________  ________  _____ ______   _______           ________  ___      ___ _______   ________     
|\   ____\|\   __  \|\   _ \  _   \|\  ___ \         |\   __  \|\  \    /  /|\  ___ \ |\   __  \    
\ \  \___|\ \  \|\  \ \  \\\__\ \  \ \   __/|        \ \  \|\  \ \  \  /  / | \   __/|\ \  \|\  \   
 \ \  \  __\ \   __  \ \  \\|__| \  \ \  \_|/__       \ \  \\\  \ \  \/  / / \ \  \_|/_\ \   _  _\  
  \ \  \|\  \ \  \ \  \ \  \    \ \  \ \  \_|\ \       \ \  \\\  \ \    / /   \ \  \_|\ \ \  \\  \| 
   \ \_______\ \__\ \__\ \__\    \ \__\ \_______\       \ \_______\ \__/ /     \ \_______\ \__\\ _\ 
    \|_______|\|__|\|__|\|__|     \|__|\|_______|        \|_______|\|__|/       \|_______|\|__|\|__|                                                                                                    
");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"{Player.Name}님은 힘이 다해 쓰러졌습니다.");
            Console.WriteLine($"처음 마을에 도착했을 때로 회귀합니다.");
            Console.WriteLine();
            Console.WriteLine("Enter 키를 눌러 계속하세요.");
            Console.ReadLine();
            Player.ResetAfterDeath();
        }

        // 경험치 획득 메서드
        private void GetExperience(Enemy Deadmonster)
        {
            Player.Exp += Deadmonster.Exp;
            totalExpGained += Deadmonster.Exp; // 경험치 누적 저장
            Quest.CheckQuestConditions();
        }

        // 각 던전별로 골드 보상액을 다르게 설정
        protected virtual int GetGoldReward()
        {
            return 500; // 500G
        }

        //보상 받기 메서드
        private void GetRewards()
        {
            foreach (var reward in rewards) // rewards 리스트에 있는 보상 목록
            {
                reward.GetReward(); // Reward.cs에 있는 GetReward에서 보상을 받음
            }
            DropItem(); // 아이템 보상
        }

        // 던전 클리어 후 아이템 보상
        private void DropItem()
        {
            Random rand = new Random();

            //인벤토리에 없는 아이템 리스트 - 상점의 모든 아이템 중에서, 플레이어의 인벤토리에 아직 없는 아이템 목록
            List<Equipment.Weapon> NoWeapon = Shop.storeWeapons.Where(weapon => !Equipment.Inventory.Any(owned => owned.Name == weapon.Name)).ToList();
            List<Equipment.Armor> NoArmor = Shop.storeArmors.Where(armor => !Equipment.ArmorInventory.Any(owned => owned.Name == armor.Name)).ToList();

            if (NoWeapon.Count > 0) // 소지하지 않은 무기의 수 > 0
            {
                int randDrop = rand.Next(1, 101); // 무기 드랍 확률
                int itemDrop = rand.Next(1, 101); // 무기와 방어구 중 하나 드랍 확률

                if (randDrop <= 20) // 아이템 드랍률 20%
                {
                    if (itemDrop > 50) // 무기 드랍률 50%
                    {
                        int randnum = rand.Next(0, NoWeapon.Count); // 인벤토리에 없는 무기 개수 만큼
                        Equipment.Weapon reWeapon = NoWeapon[randnum]; // 보상 무기 = 내가 소지하고 있지 않은 무기
                        rewards.Add(new WeaponReward(reWeapon)); // 내가 소지하지 않은 무기를 보상 리스트에 추가
                        new WeaponReward(reWeapon).GetReward();

                        Console.WriteLine($"보상으로 '{reWeapon.Name}'을 획득했습니다!");
                    }
                    else // 방어구 드랍률 50%
                    {
                        int randnum = rand.Next(0, NoArmor.Count); // 인벤토리에 없는 방어구 개수 만큼
                        Equipment.Armor reArmor = NoArmor[randnum]; // 보상 방어구 = 내가 소지하고 있지 않은 방어구
                        rewards.Add(new ArmorReward(reArmor)); // 내가 소지하지 않은 방어구를 보상 리스트에 추가
                        new ArmorReward(reArmor).GetReward();

                        Console.WriteLine($"보상으로 '{reArmor.Name}'을 획득했습니다!");
                    }
                }
            }
        }
    }

    public class EasyDungeon : Dungeon // 초급 던전
    {
        public override void ShowEnterMessage()
        {
            Console.WriteLine("초급 던전에 입장합니다!");
        }

        public override List<Enemy> GenerateMonsters()
        {
            var random = new Random();
            var monsterList = new List<Enemy>();

            for (int i = 0; i < 3; i++) // 3마리 생성
            {
                int type = random.Next(0, 3); // 0~2 타입 랜덤 생성
                int level = random.Next(1, 4); 
                monsterList.Add(new Enemy(type, level));
            }

            return monsterList;
        }

        public EasyDungeon() // 출력 메시지와 상속받은 던전 입장 메서드 실행
        {
            Enter();
        }
    }

    public class NormalDungeon : Dungeon // 중급 던전
    {
        public override void ShowEnterMessage()
        {
            Console.WriteLine("중급 던전에 입장합니다!");
        }

        public override List<Enemy> GenerateMonsters()
        {
            var random = new Random();
            var monsterList = new List<Enemy>();

            for (int i = 0; i < 4; i++) // 4마리 생성
            {
                int type = random.Next(1, 4);
                int level = random.Next(3, 6); 
                monsterList.Add(new Enemy(type, level));
            }

            return monsterList;
        }

        protected override int GetGoldReward() // override로 중급 던전 골드 보상 재정의
        {
            return 390; // 390G 골드 보상 공식은 Exp * 15G
        }

        public NormalDungeon() // 출력 메시지와 상속받은 던전 입장 메서드 실행
        {
            Enter();
        }
    }

    public class HardDungeon : Dungeon // 고급 던전
    {
        public override void ShowEnterMessage()
        {
            Console.WriteLine("고급 던전에 입장합니다!");
        }

        public override List<Enemy> GenerateMonsters()
        {
            var random = new Random();
            var monsterList = new List<Enemy>();

            for (int i = 0; i < 5; i++) 
            {
                int type = random.Next(2, 5); 
                int level = random.Next(4, 7); 
                monsterList.Add(new Enemy(type, level));
            }

            return monsterList;
        }

        protected override int GetGoldReward() // override로 고급 던전 골드 보상 재정의
        {
            return 645; // 645G 골드 보상 공식은 Exp * 15G
        }

        public HardDungeon() // 출력 메시지와 상속받은 던전 입장 메서드 실행
        {
            Enter();
        }
    }
}
