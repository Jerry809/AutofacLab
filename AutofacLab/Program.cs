using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacLab
{
    class Program
    {
        //在記憶體中建立一個IQueryable<Memo>資料來源
        static IQueryable<Memo> GenSomeMemos()
        {
            IQueryable<Memo> memos = new List<Memo>() {
                new Memo { Title = "Release Autofac 1.0",
                           DueAt = new DateTime(2007, 12, 14) },
                new Memo { Title = "Write CodeProject Article",
                           DueAt = DateTime.Now },
                new Memo { Title = "End of The World",
                           DueAt = new DateTime(2012, 12, 21) }
            }.AsQueryable();
            return memos;
        }

        static ContainerBuilder builder;

        static void Main(string[] args)
        {
            builder = new ContainerBuilder();

            //註冊MemoChecker，建構式的參數也由Container產生
            builder.Register(c => new MemoChecker(
               c.Resolve<IQueryable<Memo>>(),
               c.Resolve<IMemoDueNotifier>())
               );

            //將PrintingNotifier註冊成IMemoDueNotifier的預設來源
            builder.Register(c => new PrintingNotifier(
                c.Resolve<TextWriter>())).As<IMemoDueNotifier>();

            IQueryable<Memo> memos = GenSomeMemos();
            //直接註冊現有物件是很方便的玩法，在此註冊IQueryable<Memo>的Instance
            //當需要IQueryable<Memo>時，就會用它
            builder.RegisterInstance(memos);
            //Console.Out有實作IDisposable，但不應該由Container來終結
            //所以要加上ExternallyOwned
            builder.RegisterInstance(Console.Out).As<TextWriter>().ExternallyOwned();

            InAutofacWay();

            //NoDI();
            //Console.WriteLine("Done! press any key to exit...");
            //Console.Read();
        }


        static void NoDI()
        {
            //傳統寫法，物件的產生是寫死的
            MemoChecker chkr = new MemoChecker(GenSomeMemos(), new PrintingNotifier(Console.Out));
            chkr.CheckNow();
        }

        static void InAutofacWay()
        {
            //使用using包住Container
            //當程式結束時Autofac會自動處理需要Dispose的物件
            using (var container = builder.Build())
            {
                container.Resolve<MemoChecker>().CheckNow();
            }
        }
    }


}
