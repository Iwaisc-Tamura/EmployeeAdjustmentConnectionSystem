using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    /// <summary>
    /// 確定処理ビジネスロジック
    /// </summary>
    public class DataCommitBL {

        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 目標管理処理の種別の値
        /// </summary>
        private const string OBJECTIVE_FLAG = "Objective";
        /// <summary>
        /// 職能職務処理の種別の値
        /// </summary>
        private const string SKILL_FLAG = "Skill";

        //2020-11-16 iwai-tamura upd-str ------
        /// <summary>
        /// 自己申告書処理の種別の値
        /// </summary>
        private const string SELF_FLAG = "Self";
        //2020-11-16 iwai-tamura upd-end ------
        /// <summary>
        /// 確定処理
        /// </summary>
        /// <returns></returns>
        public string DoCommit(SystemManagementModels model) {
            try {

                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                
                int? yearVal = DataConv.IntParse(model.CommitYear, null);

                //ストアドでデータ作成
                return DoCommitStored((int)yearVal, model.CommitDuration, model.CommitType, model.CommitArea);

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }

        }

        /// <summary>
        /// 確定処理のストアド実行
        /// </summary>
        /// <param name="yearParam">確定処理年度</param>
        /// <param name="durationParam">確定処理期間区分</param>
        /// <param name="type">確定処理対象</param>
        /// <param name="commitAreaParam">確定区分リストのvalue(All(全体確定) or システム管理権限(事業所番号))</param>
        private string DoCommitStored(int yearParam, string durationParam, string type,string commitAreaParam) {
            try {

                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //処理種別によって実行ストアドを切替
                //2020-11-16 iwai-tamura upd-str ------
                string sql = type == SKILL_FLAG ? "SD_SP職能判定確定" : (type == OBJECTIVE_FLAG ? "SD_SP目標管理確定":"SD_SP自己申告書確定");
                //string sql = type == SKILL_FLAG ? "SD_SP職能判定確定" : "SD_SP目標管理確定";
                //2020-11-16 iwai-tamura upd-end ------

                //実行時間取得
                Configuration config = WebConfig.GetConfigFile();
                var timeout = config.AppSettings.Settings["SQL_TIMEOUT"].Value;

                string ResultMsg = "";      //ストアドからのメッセージを受け取る

                using(var scope = new TransactionScope()) {
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("DurationParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("CommitArea", SqlDbType.NVarChar));

                        SqlParameter ResultMsgPara = new SqlParameter("ResultMsg", SqlDbType.NVarChar,-1);
                        ResultMsgPara.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(ResultMsgPara);

                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();

                        parameters[0].Value = yearParam;
                        parameters[1].Value = durationParam;
                        parameters[2].Value = commitAreaParam;

                        int result = cmd.ExecuteNonQuery();
                        ResultMsg = ResultMsgPara.Value.ToString();

                    }
                    //全体確定後の場合のみ決裁権限履歴化ストアド実行
                    if(commitAreaParam == "All") {
                        sql = "SD_SP決裁権限確定";
                        using(DbManager dm = new DbManager())
                        using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                            cmd.Parameters.Add(new SqlParameter("DurationParam", SqlDbType.NVarChar));
                            cmd.Parameters.Add(new SqlParameter("CommitTypeParam", SqlDbType.NVarChar));

                            var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                            parameters[0].Value = yearParam;
                            parameters[1].Value = durationParam;
                            parameters[2].Value = type;

                            int result = cmd.ExecuteNonQuery();
                        }
                    }
                    scope.Complete();
                }

                //空でもなく末尾がカンマの場合は、末尾のカンマを削除する。
                if(!(string.IsNullOrEmpty(ResultMsg)) && (ResultMsg.EndsWith(","))) { 
                    ResultMsg = ResultMsg.Remove(ResultMsg.Length - 1);
                    ResultMsg = ResultMsg.Replace(",",System.Environment.NewLine);
                }

                return ResultMsg;

            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

    }
}
