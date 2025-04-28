using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static XVIBE_TextRPG.Enemy;
using static XVIBE_TextRPG.GameData;

namespace XVIBE_TextRPG
{
    internal class Player
    {
        public static string Name { get; set; } = "플레이어"; // 플레이어 이름

        public static string Job { get; set; } = "전사"; // 전사, 도적, 마법사 중 선택 가능

        public static int Exp { get; set; } = 0; // 플레이어 경험치

        public static int Level { get; set; } = 1; // 플레이어 레벨 = 기본레벨 1 + 경헙치 100당 레벨업

        public static int Gold { get; set; } = 600; // 플레이어 골드

        public static int MaxHP { get; set; } = 100; // 직업에 따라 결정되도록 수정 필요
        public static int CurrentHP { get; set; } = MaxHP;
        public static int MaxMP { get; set; } = 50; // 직업에 따라 결정되도록 수정 필요
        public static int CurrentMP { get; set; } = MaxMP;

        public static int TotalATK { get; set; } = 10; // 장비와 레벨에 따라 결정되도록 수정 필요
        public static int TotalDEF { get; set; } = 5; // 장비와 레벨에 따라 결정되도록 수정 필요
        

        public static float AdditionalEvasionRate { get; set; } = 0; // 추가 회피율
        public static float BaseEvasionRate { get; set; } = 0; // 직업별 기본 회피율


        // 전투 턴 동안 추가 공격력
        private static int TemporaryATKBoost { get; set; } = 0;

        // 직업별 능력치 계산 메서드
        public static void UpdateStats()
        {

            switch (Job)
            {
                case "전사":
                    MaxHP = 130;
                    TotalATK = 10 + Equipment.ATKBonus + (Level - 1);
                    TotalDEF = 10 + Equipment.DEFBonus + ((Level - 1) * 2);
                    MaxMP = 20;
                    BaseEvasionRate = 0.00f;
                    Quest.CheckQuestConditions();
                    break;
                case "마법사":
                    MaxHP = 80;
                    TotalATK = 17 + Equipment.ATKBonus + (Level - 1);
                    TotalDEF = 3 + Equipment.DEFBonus + ((Level - 1) * 2);
                    MaxMP = 80;
                    BaseEvasionRate = 0.05f;
                    Quest.CheckQuestConditions();
                    break;
                case "도적":
                    MaxHP = 100;
                    TotalATK = 13 + Equipment.ATKBonus + (Level - 1);
                    TotalDEF = 5 + Equipment.DEFBonus + ((Level - 1) * 2);
                    MaxMP = 40;
                    BaseEvasionRate = 0.15f;
                    Quest.CheckQuestConditions();
                    break;
                default:
                    MaxHP = 100;
                    TotalATK = 10;
                    TotalDEF = 5;
                    break;
            }

            // 현재 HP와 MP가 최대값을 초과하지 않도록 조정
            CurrentHP = Math.Min(CurrentHP, MaxHP);
            CurrentMP = Math.Min(CurrentMP, MaxMP);

            Console.WriteLine($"[스탯 업데이트] 직업: {Job}, HP: {MaxHP}, ATK: {TotalATK}, DEF: {TotalDEF}");
        }

        // 전사 스킬: 전투 턴 동안 공격력 증가
        public static void WarriorSkill()
        {
            if (CurrentHP >= 6 && CurrentMP >= 20) // 전사 스킬로 버프걸면 치명타 계산식에 반영되어 추가 코드 생략
            {
                CurrentHP -= 5;
                CurrentMP -= 20;
                TemporaryATKBoost = 10; // 턴 동안 추가 공격력
                Console.WriteLine("전사의 분노를 사용했습니다! 이번 턴 동안 공격력이 10 증가합니다.");
            }
            else
            {
                Console.WriteLine("스킬을 사용할 수 있는 HP 또는 MP가 부족합니다.");
            }
        }

        // 현재 공격력 계산 메서드
        public static float GetCurrentATK()
        {
            return TotalATK + TemporaryATKBoost;
        }

        // 턴 종료 시 추가 공격력, 회피율 초기화
        public static void EndTurn()
        {
            TemporaryATKBoost = 0; // 턴 종료 시 공격력 초기화
            AdditionalEvasionRate = 0; // 턴 종료 시 회피율 초기화
        }

        // 도적 스킬: 단일 대상에게 1.5배 피해
        public static void ThiefSkill(Enemy target)
        {
            if (CurrentMP >= 10)
            {
                int damage = (int)(TotalATK * 1.5);
                if (Combat.IsCriticalHit())
                {
                    CurrentMP -= 10;
                    int criticalDamage = target.TakeCriticalDamage(damage); // 단일적에게 도적 스킬로 강화된 공격에 크리티컬 데미지 입힘
                    Console.WriteLine($"도적의 스킬을 사용했습니다! {target.Name}에게 {criticalDamage}의 [치명타]피해를 입혔습니다!!!");
                    Console.WriteLine($"남은 MP: {CurrentMP}");
                }
                else
                {
                    CurrentMP -= 10;
                    target.TakeDamage(damage);
                    Console.WriteLine($"도적의 스킬을 사용했습니다! {target.Name}에게 {damage}의 피해를 입혔습니다.");
                    Console.WriteLine($"남은 MP: {CurrentMP}");
                }
            }
            else
            {
                Console.WriteLine("스킬을 사용할 수 있는 MP가 부족합니다.");
            }
        }

        // 마법사 스킬: 전체 대상에게 공격력만큼 피해
        public static void MageSkill(Enemy[] targets)
        {
            if (CurrentMP >= 20)
            {
                CurrentMP -= 20;
                if (Combat.IsCriticalHit())
                {
                    foreach (var target in targets)
                    {
                        int criticalDamage = target.TakeCriticalDamage(TotalATK); // 마법사 스킬로 모든 적에게 강려크한 크리티컬 데미지 입힘
                        Console.WriteLine($"마법사의 스킬을 사용했습니다! {target.Name}에게 {criticalDamage}의 [치명타]피해를 입혔습니다!!!");
                    }
                }
                else
                {
                    foreach (var target in targets)
                    {
                        target.TakeDamage(TotalATK);
                        Console.WriteLine($"마법사의 스킬을 사용했습니다! {target.Name}에게 {TotalATK}의 피해를 입혔습니다.");
                    }
                }
                Console.WriteLine($"남은 MP: {CurrentMP}");
            }
            else
            {
                Console.WriteLine("스킬을 사용할 수 있는 MP가 부족합니다.");
            }
        }
        public static void ShowStatus()
        {
            Console.Clear();
            Console.WriteLine("[캐릭터 상태 보기]");
            Console.WriteLine($"이름     : {Name}");
            Console.WriteLine($"직업     : {Job}");
            Console.WriteLine($"레벨     : {Level}");
            Console.WriteLine($"경험치   : {Exp}");
            Console.WriteLine($"체력     : {CurrentHP} / {MaxHP}");
            Console.WriteLine($"공격력   : {TotalATK} (+{Equipment.ATKBonus + (Level - 1)})");
            Console.WriteLine($"방어력   : {TotalDEF} (+{Equipment.DEFBonus + ((Level - 1) * 2)})");
            Console.WriteLine($"보유 골드: {Gold} G");
            Console.WriteLine("\nEnter 키를 누르면 이전 화면으로 돌아갑니다.");
            Console.ReadLine();
        }

        // 플레이어 정보 data 에 저장
        public static void SavePlayerData()
        {
            GameData data = new GameData
            {
                Name = Player.Name,
                Job = Player.Job,
                Exp = Player.Exp,
                Level = Player.Level,
                Gold = Player.Gold,
                MaxHP = Player.MaxHP,
                CurrentHP = Player.CurrentHP,
                MaxMP = Player.MaxMP,
                CurrentMP = Player.CurrentMP,
                TotalATK = (int)Player.TotalATK,
                TotalDEF = Player.TotalDEF,

                //무기 인벤토리
                Inventory = Equipment.Inventory.Select(w => new WeaponData
                {
                    Name = w.Name,
                    Type = w.Type.ToString(),
                    ATK = w.ATK,
                    Price = w.Price
                }).ToList(),

                // 장착한 물건이 있다면 WeponData에 저장, 없으면 null값
                EquippedWeapon = Equipment.EquippedWeapon != null ? new WeaponData
                {
                    Name = Equipment.EquippedWeapon.Name,
                    Type = Equipment.EquippedWeapon.Type.ToString(),
                    ATK = Equipment.EquippedWeapon.ATK,
                    Price = Equipment.EquippedWeapon.Price
                } : null,

                // 방어구 인벤토리
                ArmorInventory = Equipment.ArmorInventory.Select(a => new ArmorData
                {
                    Name = a.Name,
                    Type = a.Type.ToString(),
                    DEF = a.DEF,
                    Price = a.Price
                }).ToList(),

                // 방어구도 똑같은 방식으로 저장
                EquippedArmor = Equipment.EquippedArmor != null ? new ArmorData
                {
                    Name = Equipment.EquippedArmor.Name,
                    Type = Equipment.EquippedArmor.Type.ToString(),
                    DEF = Equipment.EquippedArmor.DEF,
                    Price = Equipment.EquippedArmor.Price
                } : null,

                Quests = Quest.questList.Select(q => new GameData.QuestData
                {
                    Name = q.QuestName,
                    IsAccepted = q.Status == QuestStatus.InProgress,
                    IsCompleted = q.Status == QuestStatus.Completed,
                    IsRewardReceived = q.Status == QuestStatus.Finished
                    
                }).ToList(),

                Consumables = Consumable.consumable.Select(c => new ConsumableData
                {
                    Name = c.Name,
                    HealHP = c.HealHP,
                    HealMP = c.HealMP,
                    Amount = c.Amount,
                    Type = c.Type.ToString()
                }).ToList(),

                CurrentKillCount = Quest.CurrentKillCount,
            };

            SaveSystem.Save(data);
        }

        // 저장된 게임 데이터 삭제
        public static void DeletePlayerData()
        {
            SaveSystem.DeleteSave();
            Console.WriteLine("Enter 키를 눌러주세요.");
            Console.ReadLine();
        }

        // data 에 있는 정보 플레이어에게 전달
        public static void LoadPlayerData(GameData data)
        {
            // 플레이어 캐릭터 스텟 관련
            Name = data.Name;
            Job = data.Job;
            Exp = data.Exp;
            Level = data.Level;
            Gold = data.Gold;
            MaxHP = data.MaxHP;
            CurrentHP = data.CurrentHP;
            MaxMP = data.MaxMP;
            CurrentMP = data.CurrentMP;
            TotalATK = data.TotalATK;
            TotalDEF = data.TotalDEF;

            // 플레이어 무기 관련
            Equipment.Inventory = data.Inventory.Select(w => new Equipment.Weapon(
                w.Name,
                Enum.Parse<Equipment.WeaponType>(w.Type),
                w.ATK,
                w.Price
                )).ToList();

            // 플레이어 무기 작착 관련
            if (data.EquippedWeapon != null)
            {
                var loadedWeapon = new Equipment.Weapon(
                    data.EquippedWeapon.Name,
                    Enum.Parse<Equipment.WeaponType>(data.EquippedWeapon.Type),
                    data.EquippedWeapon.ATK,
                    data.EquippedWeapon.Price
                );

                Equipment.Equip(loadedWeapon);
            }
            else
            {
                Equipment.EquippedWeapon = null;
                Equipment.ATKBonus = 0;
            }

            // 플레이어 방어구 관련
            Equipment.ArmorInventory = data.ArmorInventory.Select(a => new Equipment.Armor(
                a.Name,
                Enum.Parse<Equipment.ArmorType>(a.Type),
                a.DEF,
                a.Price
            )).ToList();

            // 플레이어 방어구 장착 관련
            if (data.EquippedArmor != null)
            {
                var loadedArmor = new Equipment.Armor(
                    data.EquippedArmor.Name,
                    Enum.Parse<Equipment.ArmorType>(data.EquippedArmor.Type),
                    data.EquippedArmor.DEF,
                    data.EquippedArmor.Price
                );

                Equipment.Equip(loadedArmor);
            }
            else
            {
                Equipment.EquippedArmor = null;
                Equipment.DEFBonus = 0;
            }

            // 플레이어 포션 관련
            foreach (var c in data.Consumables)
            {
                var existing = Consumable.consumable.FirstOrDefault(x =>
                    x.Name == c.Name &&
                    x.HealHP == c.HealHP &&
                    x.HealMP == c.HealMP &&
                    x.Type.ToString() == c.Type);

                if (existing != null)
                {
                    existing.Amount = c.Amount;
                }
            }

            // 플레이어 퀘스트 관련
            if (data.Quests != null && data.Quests.Count > 0)
            {
                foreach (var q in data.Quests)
                {
                    var quest = Quest.questList.FirstOrDefault(x => x.QuestName == q.Name);
                    if (quest != null)
                    {
                        if (q.IsRewardReceived)
                            quest.Status = QuestStatus.Finished;
                        else if (q.IsCompleted)
                            quest.Status = QuestStatus.Completed;
                        else if (q.IsAccepted)
                            quest.Status = QuestStatus.InProgress;
                        else
                            quest.Status = QuestStatus.NotAccepted;
                    }
                }
                Quest.CurrentKillCount = data.CurrentKillCount;
            }
            UpdateStats();
        }

        public static void ResetAfterDeath()
        {
            // 레벨, 경험치, 골드 초기화
            Level = 1;
            Exp = 0;
            Gold = 600;

            // 장비 인벤토리 초기화
            Equipment.Inventory.Clear();
            Equipment.EquippedWeapon = null;
            Equipment.ATKBonus = 0;
            Equipment.ArmorInventory.Clear();
            Equipment.EquippedArmor = null;
            Equipment.DEFBonus = 0;

            // 포션 갯수 초기화
            foreach (var c in Consumable.consumable)
            {
                c.Amount = 0;
            }

            // 퀘스트 초기화
            foreach (var quest in Quest.questList)
            {
                quest.Status = QuestStatus.NotAccepted;
            }
            Quest.CurrentKillCount = 0;

            Player.EndTurn(); // 전투 종료 시 추가 능력치 초기화

            // 직업에 따른 능력치 초기화
            UpdateStats();

            // 체력, 마나는 최대치로 회복
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
        }

        // 레벨업

        // ▒▒▒ [레벨업 밸런스 기준 주석] ▒▒▒
        //
        // ▶ 성장 구조
        //    - 레벨업 필요 EXP: 100으로 설정
        //      → 평균 던전 2~4회, 퀘스트 2~3개 클리어로 1레벨 업 가능
        //      → 성장이 지체되지 않도록 템포를 맞춘 수치
        //
        // ▶ 능력치 상승폭
        //    - 공격력: +1 / 방어력: +2
        //      → 고급 던전 평균 몬스터 ATK 30~45 수준을 감안해,
        //         플레이어가 일정 레벨 이상이면 장비 없이도 최소 생존 가능하도록 설계
        //
        // ▶ 밸런스 방향성
        //    - 장비가 핵심 성장 수단이긴 하지만,
        //      순수 레벨업만으로도 스탯이 확실히 오르도록 구성해 **성장 보람 체감 보장**
        //    - 성장 난이도 완화를 통해 유저 이탈 방지
        //      → 특히 초반 구간에서 빠른 성취감을 주기 위한 조치
        //
        //  전체 밸런스는 "EXP 100 기준 + 능력치 직접 성장 + 장비 구매"의 삼각구조로 설계됨
        public static void LvUp()
        {
            while (Exp >= 100)
            {
                int remainderExp = Exp - 100;
                Level++;
                MaxHP += 2;
                TotalATK += 1;
                TotalDEF += 2;
                Exp = remainderExp;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[캐릭터 정보] Lv.{Level - 1} -> Lv.{Level}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"최대 체력: {MaxHP - 2} -> {MaxHP}");
                Console.WriteLine($"공격력: {TotalATK - 1} -> {TotalATK}");
                Console.WriteLine($"방어력: {TotalDEF - 2} -> {TotalDEF}");
                Console.WriteLine($"Exp: {Exp + 100} -> {remainderExp}\n");
                Console.ResetColor();
            }
            Quest.CheckQuestConditions(); // 퀘스트 조건 즉시 확인
            Console.WriteLine("아무 키나 누르세요...");
            Console.ReadLine();
        }
    }
}
