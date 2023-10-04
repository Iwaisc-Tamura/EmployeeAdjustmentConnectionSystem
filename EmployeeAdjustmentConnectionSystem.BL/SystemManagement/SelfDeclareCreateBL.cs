using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Transactions;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Data.SqlClient;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    /// <summary>
    /// 自己申告書データ作成ビジネスロジック
    /// </summary>
    public class SelfDeclareCreateBL {

        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 自己申告書データの初期作成
        /// </summary>
        /// <param name="model">システム管理のモデル</param>
        /// <param name="button">押下されたボタンのvalue</param>
        /// <returns>完了メッセージ</returns>
        public string CreateSelfDeclare(SystemManagementModels model, string button,LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //社員番号と所属番号を取得
                DataSet dsBase = new DataSet();

                //個別作成から処理がきた場合は、対象社員番号と対象年度と作成対象をセット。
                string employee = null;
                string year = model.BulkYear;
                string actionType = model.BulkType;

                if(button == "designate") {
                    year = model.DesignateYaer;
                    employee = model.DesignateEmployeeNo;
                    actionType = model.DesignateType;
                }

                int? yearVal = DataConv.IntParse(year, null);

                //ストアド実行
                DoSelfDeclareStored((int)yearVal, employee,actionType,lu);

                return "自己申告書初期データを作成しました。";
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
        /// 自己申告書初期作成のストアド実行
        /// </summary>
        /// <param name="yearParam">対象年度</param>
        /// <param name="employeeParam">対象社員番号</employeeParam>
        private void DoSelfDeclareStored(int yearParam, string employeeParam,string actionType,LoginUser lu) {
            try {

                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");        

                string sql = "SD_SP自己申告書初期作成";

                //実行時間取得
                Configuration config = WebConfig.GetConfigFile();
                var timeout = config.AppSettings.Settings["SQL_TIMEOUT"].Value;

                using(var scope = new TransactionScope()) {
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = yearParam;
                        parameters[1].Value = employeeParam;

                        nlog.Debug(sql + " start");
                        int result = cmd.ExecuteNonQuery();
                        nlog.Debug(sql + " end");
                    }

                    sql = "SD_SP自己申告書決裁権限作成";
                    //自己申告書決裁権限初期データ作成                    
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = yearParam;
                        parameters[1].Value = employeeParam;

                        nlog.Debug(sql + " start");
                        int result = cmd.ExecuteNonQuery();
                        nlog.Debug(sql + " end");
                    }

                    ////bl c#版の決裁権限作成
                    //nlog.Debug("決裁権限初期データ作成 start");
                    //new ApproverInsert().DoInsertApprover(yearParam, employeeParam, actionType, lu,string.Empty);
                    //nlog.Debug("決裁権限初期データ作成 end");
                    
                    scope.Complete();
                }

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
        /// SD_M期間管理の確認
        /// </summary>
        /// <param name="year">対象年度</param>
        /// <returns>SD_M期間管理の対象データの件数</returns>
        private Boolean MasterExists(int year) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                string sql = "select COUNT(*) from SD_M期間管理 where 年度 = @year and 期区分 = '0';";
                Boolean retval = false;

                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql)) {
                    //パラメータ設定
                    DbHelper.AddDbParameter(cmd, "@year", DbType.Int32);
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = year;

                    retval = ((int)cmd.ExecuteScalar()) == 0 ? false : true;
                }
                return retval;
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