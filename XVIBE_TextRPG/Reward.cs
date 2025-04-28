using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static XVIBE_TextRPG.Equipment;
using System.Xml.Linq;

namespace XVIBE_TextRPG
{
    public abstract class Reward // 보상 추상 클래스
    {
        public abstract void GetReward();
    }

    public class GoldReward : Reward // 골드 보상
    {
        public int GetGold { get; }

        public GoldReward(int getGold) // 생성자
        {
            GetGold = getGold;
        }

        public override void GetReward() // 보상 받기 메서드
        {
            Player.Gold += GetGold; // 던전 클리어 골드를 플레이어 골드에 합침
        }
    }

    public class WeaponReward : Reward // 무기 보상
    {
        public Weapon Weapon { get; }

        public WeaponReward(Weapon weapon) // 생성자
        {
            Weapon = weapon;
        }

        public override void GetReward() // 보상 받기 메서드
        {
            Equipment.Inventory.Add(Weapon); // 인벤토리에 보상 추가
        }

        public class ArmorReward : Reward // 방어구 보상
        {
            public Armor Armor { get; }

            public ArmorReward(Armor armor) // 생성자
            {
                Armor = armor;
            }

            public override void GetReward() // 보상 받기 메서드
            {
                Equipment.ArmorInventory.Add(Armor); // 인벤토리에 보상 추가
            }
        }
    }
}
