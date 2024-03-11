using PwdGen.Models;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PwdGen
{
    public partial class App : Application
    {
        public static string _fileName_UserAcct = Path.Combine(FileSystem.AppDataDirectory, "UserAcct.json");
        public static JsonSerializerOptions _jso = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        public static List<AcctData> AcctDatas { get; set; }

        public static Action? QuitMethod { get; private set; } = null;

        public App()
        {
            if (QuitMethod == null) QuitMethod = Quit;
            else throw new Exception("App class has been Inst");

            InitializeComponent();
#if DEBUG
            Debug.WriteLine(_fileName_UserAcct);
#endif
            MainPage = new MainPage();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Title = "PwdGen";

            return window;
        }
        protected override void OnStart()
        {
            base.OnStart();
            if (!File.Exists(_fileName_UserAcct))
            {
                AcctDatas = new();
                File.WriteAllBytes(_fileName_UserAcct, JsonSerializer.SerializeToUtf8Bytes(AcctDatas, _jso));
            }
            else AcctDatas = JsonSerializer.Deserialize<List<AcctData>>(File.ReadAllBytes(_fileName_UserAcct), _jso);
        }
    }
}
