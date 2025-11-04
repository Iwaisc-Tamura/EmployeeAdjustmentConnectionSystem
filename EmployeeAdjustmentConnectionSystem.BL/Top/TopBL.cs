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
using Microsoft.Office.Interop.Excel;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;

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
                //ログイン情報取得
                var lu = (LoginUser)HttpContext.Current.Session["LoginUser"];
                //2025-99-99 iwai-tamura upd-str ------
                top.HuyouDecisionType = GetHuyouStatus(lu.IsYear,lu.UserCode);
                top.HuyouAttachmentFilePath = GetHuyouAttachmentFilePath(lu.IsYear,lu.UserCode);
                //2025-99-99 iwai-tamura upd-end ------


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

        //2025-99-99 iwai-tamura upd-str ------
        /// <summary>
        /// 扶養控除申告書 管理者確定区分取得
        /// </summary>
        public string GetHuyouStatus(int? intSheetYear,string strEmployeeNo,bool bolAdminMode = false) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try {
                var strStatus = "";

			    //添付ファイルデータを取得
			    var sql = "";
			    sql = " SELECT  "
				    + "   管理者確定区分 "
				    + " FROM TE100扶養控除申告書Data "
				    + " WHERE 1 = 1"
				    + "   AND 対象年度 = " + intSheetYear
				    + "   AND 社員番号 = '" + strEmployeeNo +"'";
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(IDataReader reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        strStatus = reader["管理者確定区分"].ToString();
                    }
                }
                return strStatus;
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return "";
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 扶養控除申告書 添付ファイルデータ取得
        /// </summary>
        public string GetHuyouAttachmentFilePath(int? intSheetYear,string strEmployeeNo,bool bolAdminMode = false) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try {
                var strPath = "";

			    //添付ファイルデータを取得
			    var sql = "";
			    sql = " SELECT  "
				    + "   添付FileName "
				    + "   ,本人確定区分 "
				    + "   ,管理者確定区分 "
				    + " FROM TE100扶養控除申告書Data "
				    + " WHERE 1 = 1"
				    + "   AND 対象年度 = " + intSheetYear
				    + "   AND 社員番号 = '" + strEmployeeNo +"'";
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(IDataReader reader = cmd.ExecuteReader()) {
                    while(reader.Read()) {
                        strPath = reader["添付FileName"].ToString();
                    }
                }
                return strPath;
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return "";
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }

        }

        /// <summary>
        /// 扶養控除申告書 添付ファイルデータ更新
        /// </summary>
        public bool UploadHuyouAttachmentFilePath(int? intSheetYear,string strEmployeeNo,string strFileName, bool bolAdminMode = false) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

            try {
                // 添付ファイルデータを更新
			    var sql = "";
			    sql = " UPDATE TE100扶養控除申告書Data "
				    + " SET 添付FileName = @FileName "
				    + " WHERE 1 = 1"
				    + "   AND 対象年度 = @SheetYear"
				    + "   AND 社員番号 = @EmployeeNo";

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@FileName", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = strFileName;
                    ((IDbDataParameter)cmd.Parameters[1]).Value = intSheetYear;
                    ((IDbDataParameter)cmd.Parameters[2]).Value = strEmployeeNo;

                    // UPDATE実行
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                return false;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }

        }
        //2025-99-99 iwai-tamura upd-end ------

    }
}
