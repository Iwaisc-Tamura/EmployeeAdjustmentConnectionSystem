using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Web;   //2017-08-31 iwai-tamura add
using EmployeeAdjustmentConnectionSystem.Log.Common;    //2019-10-02 iwai-tamura add
using System.Reflection;    //2019-10-02 iwai-tamura add


namespace EmployeeAdjustmentConnectionSystem.Bl.Top {
    /// <summary>
    /// TOP画面用ビジネスロジック
    /// </summary>
    public class TopBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <returns>トップ画面用モデル</returns>
        public TopViewModel Index() {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try {
                TopViewModel top = new TopViewModel();
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand("select * from TEM910Information "))
                using(IDataReader reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        top.Announcement = reader["Message"].ToString();
                    }
                }
                //2017-08-31 iwai-tamura upd-str ------
                //未承認データ確認
                //string UserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                //    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();
                //string sql;
                //sql= "select  COUNT(*) from SD_VT目標管理未承認一覧 where 次承認者 = @emp";
                //using(DbManager dm = new DbManager())
                //using (IDbCommand cmd = dm.CreateCommand(sql)) {
                //    DbHelper.AddDbParameter(cmd, "@emp",DbType.String);
                //    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                //    parameters[0].Value = UserCode;
                //    top.ObjectivesWaitingApprover = (int)cmd.ExecuteScalar();
                //}
                //sql = "select  COUNT(*) from SD_VT職能判定未承認一覧 where 次承認者 = @emp";
                //using(DbManager dm = new DbManager())
                //using (IDbCommand cmd = dm.CreateCommand(sql)) {
                //    DbHelper.AddDbParameter(cmd, "@emp",DbType.String);
                //    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                //    parameters[0].Value = UserCode;
                //    top.SkillWaitingApprover = (int)cmd.ExecuteScalar();
                //}
                //2017-08-31 iwai-tamura upd-end ------
                
                return top;
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return new TopViewModel();
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
    }
}
