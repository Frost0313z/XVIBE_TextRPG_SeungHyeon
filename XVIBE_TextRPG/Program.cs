namespace XVIBE_TextRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logo.LogoPrint(); // 로고 출력

            //세이브 파일이 있는지 확인, 없으면 새로운 게임 시작
            SaveSystem.LoadOrNewGame();

            MainMenu.ShowMainMenu(); // 메인 메뉴 표시
        }
    }
}
