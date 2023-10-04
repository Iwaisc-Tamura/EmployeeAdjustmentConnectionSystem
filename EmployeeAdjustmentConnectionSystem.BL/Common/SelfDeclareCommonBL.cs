using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;

namespace EmployeeAdjustmentConnectionSystem.BL.Common {
    /// <summary>
    /// 目標管理全般の共通クラス
    /// </summary>
    public class SelfDeclareCommonBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// 承認情報取得
        /// </summary>
        /// <param name="dm">DBマネージャ</param>
        /// <param name="id">管理番号</param>
        /// <returns></returns>
        public static DataSet GetSignData(DbManager dm, int? id) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                var sql = "select distinct "
                        + "  ms.管理番号"
                        + " ,ms.年度"
                        + " ,ms.所属番号"
                        + " ,ms.社員番号"
                        + " ,ms.大区分"
                        + " ,ms.小区分"
                        + " ,ms.承認社員番号"
                        + " ,ms.承認日時"
                        + " ,vsk.氏名"
                        + " from SD_T自己申告書承認情報 ms"
                        + " left join SD_M自己申告書基本情報 vsk"
                        + "   on ms.年度 = vsk.年度"
                        + "  and ms.承認社員番号 = vsk.社員番号 "
                        + "where 管理番号= @ManageNo";

                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = id;
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);
                    return ds;
                }
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                //TODO:エラーの戻りは検討中
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認社員名取得
        /// </summary>
        /// <param name="ds">データセット</param>
        /// <param name="cols">区分</param>
        /// <returns>社員名</returns>
        public static String GetSignName(DataSet ds, string[] cols) {
            DataTable dt = ds.Tables[0];
            var query = from row in dt.AsEnumerable()
                        where row.Field<string>("大区分") == cols[0]
                          && row.Field<string>("小区分") == cols[1]
                        select row.Field<string>("氏名");

            foreach(var name in query) {
                return name;
            }
            return "";
        }

        /// <summary>
        /// 承認情報取得
        /// </summary>
        /// <param name="dm">DBマネージャ</param>
        /// <param name="id">管理番号</param>
        /// <returns></returns>
        public static int? GetSignStatus(DbManager dm, int? id,string pattern, bool isRireki) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                var sql = "select max(区分) as 区分 from ("
                        + "    select 大区分+小区分 as 区分"
                        + "      from  SD_T自己申告書承認情報"
                        + "     where 管理番号 = @ManageNo"
                        + "       and (承認社員番号 is not null or 承認社員番号 !='')"
                        + string.Format("       and (大区分 = '{0}')",pattern)     //A~C表orD表
                        + "     group by 管理番号, 年度, 所属番号, 社員番号, 大区分, 小区分"
                        + ") mks";
                //履歴の場合は履歴テーブルから取得
                sql = string.Format(sql, isRireki ? "履歴" : "");

                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = id;
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        return DataConv.Str2Int(row["区分"].ToString(), 0);
                    }
                }

                return 999;
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                //TODO:エラーの戻りは検討中
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        /// <summary>
        /// 承認者確認
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsAuthorizer(DbManager dm, string id, string uid) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                var sql = "select 管理番号"
                        + "  from SD_VT自己申告書承認権限情報"
                        + "  where 管理番号 = @ManageNo"
                        + "    and 承認者 = @EmployeeNo";
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = id;
                    ((IDbDataParameter)cmd.Parameters[1]).Value = uid;
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);
                    return ds.Tables[0].Rows.Count == 0 ? false : true;
                }
            } catch(Exception ex) {
                //エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                //TODO:エラーの戻りは検討中
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }




    }
}
