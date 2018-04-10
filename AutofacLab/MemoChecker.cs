using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacLab
{
    class Memo
    {
        public string Title;
        public DateTime DueAt;
    }

    #region 通知備忘事項
    //產生通知動作的呼叫介面
    interface IMemoDueNotifier
    {
        void MemoIsDue(Memo memo);
    }

    //實作由TextWriter輸出的通知服務
    class PrintingNotifier : IMemoDueNotifier
    {
        TextWriter _writer;
        // 傳入TextWriter的建構式
        public PrintingNotifier(TextWriter writer)
        {
            _writer = writer;
        }
        // 輸出通知訊息到TextWriter
        public void MemoIsDue(Memo memo)
        {
            _writer.WriteLine("Memo '{0}' is due!", memo.Title);
        }
    }
    #endregion


    #region 檢查備忘錄
    class MemoChecker
    {
        IQueryable<Memo> _memos;
        IMemoDueNotifier _notifier;

        // 建立備忘錄檢查器，顯示到期待辦事項
        public MemoChecker(IQueryable<Memo> memos, IMemoDueNotifier notifier)
        {
            _memos = memos;
            _notifier = notifier;
        }

        // 依目前日期找出已到期項目
        public void CheckNow()
        {
            var overdueMemos = _memos.Where(memo => memo.DueAt < DateTime.Now);
            
            foreach (var memo in overdueMemos)
                _notifier.MemoIsDue(memo);
        }
    }
    #endregion
}
