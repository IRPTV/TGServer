using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TGServer.MyDBTableAdapters;

namespace TGServer
{

    class DAL
    {




        public static BAL.TGREQ SelectJob()
        {
            DATATableAdapter Ta = new DATATableAdapter();
            MyDB.DATADataTable Dt = Ta.Select_TopJob();

            BAL.TGREQ Rq = new BAL.TGREQ();
            if (Dt.Rows.Count > 0)
            {
                Rq.Title1 = Dt[0]["Title1"].ToString();
                Rq.Title2 = Dt[0]["Title2"].ToString();
                Rq.TEMPLATE = Dt[0]["TEMPLATE"].ToString();
                Rq.ENDINGKIND = Dt[0]["ENDINGKIND"].ToString();

                Rq.T1 = Dt[0]["T1"].ToString();
                Rq.T2 = Dt[0]["T2"].ToString();
                Rq.T3 = Dt[0]["T3"].ToString();
                Rq.T4 = Dt[0]["T4"].ToString();
                Rq.T5 = Dt[0]["T5"].ToString();
                Rq.T6 = Dt[0]["T6"].ToString();
                Rq.T7 = Dt[0]["T7"].ToString();
                Rq.T8 = Dt[0]["T8"].ToString();
                Rq.T9 = Dt[0]["T9"].ToString();
                Rq.T10 = Dt[0]["T10"].ToString();

                Rq.REPORT = Dt[0]["REPORT"].ToString();

                Rq.ISDONE = bool.Parse(Dt[0]["ISDONE"].ToString());
                Rq.SENT = bool.Parse(Dt[0]["SENT"].ToString());
                Rq.VIDEODURTION = Dt[0]["VIDEODURTION"].ToString();

                Rq.DATETIME_INSERT = DateTime.Parse(Dt[0]["DATETIME_INSERT"].ToString());
                Rq.DATETIME_DONE = DateTime.Parse(Dt[0]["DATETIME_DONE"].ToString());
                Rq.DATETIME_ORDER = DateTime.Parse(Dt[0]["DATETIME_ORDER"].ToString());
                Rq.DATETIME_START = DateTime.Parse(Dt[0]["DATETIME_START"].ToString());

                Rq.FinalFile = Dt[0]["FinalFile"].ToString();

                Rq.Id = int.Parse(Dt[0]["ID"].ToString());

                return Rq;
            }
            else
            {
                return null;
            }
        }

        public static BAL.TGREQ SelectById(int Id)
        {

            DATATableAdapter Ta = new DATATableAdapter();
            MyDB.DATADataTable Dt = Ta.Data_Select_ById(Id);

            BAL.TGREQ Rq = new BAL.TGREQ();
            Rq.Title1 = Dt[0]["Title1"].ToString();
            Rq.Title2 = Dt[0]["Title2"].ToString();
            Rq.TEMPLATE = Dt[0]["TEMPLATE"].ToString();
            Rq.ENDINGKIND = Dt[0]["ENDINGKIND"].ToString();

            Rq.T1 = Dt[0]["T1"].ToString();
            Rq.T2 = Dt[0]["T2"].ToString();
            Rq.T3 = Dt[0]["T3"].ToString();
            Rq.T4 = Dt[0]["T4"].ToString();
            Rq.T5 = Dt[0]["T5"].ToString();
            Rq.T6 = Dt[0]["T6"].ToString();
            Rq.T7 = Dt[0]["T7"].ToString();
            Rq.T8 = Dt[0]["T8"].ToString();
            Rq.T9 = Dt[0]["T9"].ToString();
            Rq.T10 = Dt[0]["T10"].ToString();

            Rq.REPORT = Dt[0]["REPORT"].ToString();

            Rq.ISDONE = bool.Parse(Dt[0]["ISDONE"].ToString());
            Rq.SENT = bool.Parse(Dt[0]["SENT"].ToString());
            Rq.VIDEODURTION = Dt[0]["VIDEODURTION"].ToString();

            Rq.DATETIME_INSERT = DateTime.Parse(Dt[0]["DATETIME_INSERT"].ToString());
            Rq.DATETIME_DONE = DateTime.Parse(Dt[0]["DATETIME_DONE"].ToString());
            Rq.DATETIME_ORDER = DateTime.Parse(Dt[0]["DATETIME_ORDER"].ToString());
            Rq.DATETIME_START = DateTime.Parse(Dt[0]["DATETIME_START"].ToString());


            Rq.FinalFile = Dt[0]["FinalFile"].ToString();

            Rq.Id = int.Parse(Dt[0]["ID"].ToString());


            return Rq;
        }

        public static void Update(BAL.TGREQ Obj)
        {
            DATATableAdapter Ta = new DATATableAdapter();
            int ReturnID = Ta.Update_Data(Obj.Title1,
                Obj.Title2,
                Obj.DATETIME_INSERT,
                Obj.DATETIME_DONE,
                Obj.DATETIME_START,
                Obj.DATETIME_ORDER,
                Obj.SENT,
                Obj.ISDONE,
                Obj.TEMPLATE,
                Obj.REPORT,
                Obj.ENDINGKIND,
                Obj.VIDEODURTION,
                Obj.T1,
                Obj.T2,
                Obj.T3,
                Obj.T4,
                Obj.T5,
                Obj.T6,
                Obj.T7,
                Obj.T8,
                Obj.T9,
                Obj.T10,
                Obj.Id
                );
        }
        public static void UpdateIsDone(int Id, bool Sent,string FinalFile)
        {
            DATATableAdapter Ta = new DATATableAdapter();
            int ReturnID = Ta.Update_Done(Sent, DateTime.Now,FinalFile, Id);
        }

    }
}
