using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Transactions;
using System.Collections;
using System.Data.SqlClient;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;


namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    /// <summary>
    /// 職能判定データ作成ビジネスロジック
    /// </summary>
    public class SkillCreateBL {

        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 職能判定データ作成
        /// </summary>
        /// <param name="model">システム管理画面モデル</param>
        /// <param name="button">システム管理画面ボタンValue</param>
        /// <returns></returns>
        public string CreateSkill(SystemManagementModels model, string button,LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //社員番号と所属番号を取得
                DataSet dsBase = new DataSet();

                //個別作成から処理がきた場合は、対象社員番号と対象年度をセット。
                string employee = null;                       //社員番号
                string duration = model.BulkDuration;       //期間
                string year = model.BulkYear;               //年度
                string actionType = model.BulkType;         //作成対象

                if(button == "designate") {
                    year = model.DesignateYaer;
                    duration = model.DesignateDuration;
                    employee = model.DesignateEmployeeNo;
                    actionType = model.DesignateType;
                }


                //2016-01-21 iwai-tamura upd str -----
                //設定データ修正(上期下期の期区分を入替、下期の対象期間を修正)
                //上期(年末)の場合は4月から9月末までを対象期間とする
                string hanteiFrom = year + "/04/01";
                string hanteiTo = year + "/09/30";

                //下期(夏季)の場合は前年10月から3月末までを対象期間とする
                if (duration == "1"){
                    int y;
                    int.TryParse(year, out y);
                    hanteiFrom = (y - 1) + "/10/01";
                    hanteiTo = year + "/03/31";
                };
                ////上期にあわせて判定対象期間FromToを設定
                //string hanteiFrom = year + "/04/01";
                //string hanteiTo = year + "/09/30";

                ////下期の場合は下期に合わせて判定対象期間FromToを設定
                //if (duration == "2")
                //{
                //    hanteiFrom = year + "/10/01";
                //    int y;
                //    int.TryParse(year, out y);
                //    hanteiTo = (y + 1) + "/03/31";
                //};
                //2016-01-21 iwai-tamura upd end -----
                //ストアドでデータ作成
                DoSkillStored(int.Parse(year), duration, hanteiFrom, hanteiTo, employee, actionType, lu);

                //2017-08-31 iwai-tamura upd str -----
                //前回データを履歴へ移動する
                int moveyear;
                string moveduration;
                if (duration == "1"){
                    moveyear = int.Parse(year) - 1;
                    moveduration = "2";
                }else{
                    moveyear = int.Parse(year);
                    moveduration = "1";
                };
                DoSkillMoveStored(moveyear, moveduration,lu);
                //2017-08-31 iwai-tamura upd end -----
                
                return "職能職務初期データを作成しました。";

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
        /// 職能職務作成のストアド実行
        /// </summary>
        /// <param name="yearParam">対象年度</param>
        /// <param name="durationParam">対象期区分</param>
        /// <param name="fromDurationParam">判定対象期間_自</param>
        /// <param name="toDurationParam">判定対象期間_至</param>
        /// <param name="employeeParam">対象社員番号</param>
        /// <param name="actionType">作成対象</employeeParam>
        private void DoSkillStored(int yearParam, string durationParam, string fromDurationParam, string toDurationParam, string employeeParam,string actionType,LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");        
                string sql = "SD_SP職能職務初期作成";
                //実行時間取得
                Configuration config = WebConfig.GetConfigFile();
                var timeout = config.AppSettings.Settings["SQL_TIMEOUT"].Value;

                using(var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10.0))) {
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("DurationParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("FromDurationParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("ToDurationParam", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = yearParam;
                        parameters[1].Value = durationParam;
                        parameters[2].Value = fromDurationParam;
                        parameters[3].Value = toDurationParam;
                        parameters[4].Value = employeeParam;
                        
                        nlog.Debug(sql + " start");
                        int result = cmd.ExecuteNonQuery();
                        nlog.Debug(sql + " end");
                    }

                    #region 決裁権限作成 ストアド版コード


                    ////--決裁権限のストアド実行
                    //sql = "SD_SP決裁権限初期作成";
                    ////決裁権限初期データ作成                    
                    //using(DbManager dm = new DbManager())
                    //using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                    //    cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                    //    cmd.Parameters.Add(new SqlParameter("ActionType", SqlDbType.NVarChar));
                    //    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    //    parameters[0].Value = yearParam;
                    //    parameters[1].Value = employeeParam;
                    //    parameters[2].Value = actionType;

                    //    nlog.Debug(sql + " start");
                    //    int result = cmd.ExecuteNonQuery();
                    //    nlog.Debug(sql + " end");
                    //}


                    #endregion

                    //bl c#版の決裁権限作成
                    nlog.Debug("決裁権限初期データ作成 start");
                    new ApproverInsert().DoInsertApprover(yearParam, employeeParam, actionType, lu, durationParam);
                    nlog.Debug("決裁権限初期データ作成 end");

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
        /// 職能職務履歴データ移動のストアド実行
        /// </summary>
        /// <param name="yearParam">対象年度</param>
        /// <param name="durationParam">対象期区分</param>
        private void DoSkillMoveStored(int yearParam, string durationParam,LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");        
                string sql = "SD_SP職能職務履歴移動";
                //実行時間取得
                Configuration config = WebConfig.GetConfigFile();
                var timeout = config.AppSettings.Settings["SQL_TIMEOUT"].Value;

                using(var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10.0))) {
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("DurationParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = yearParam;
                        parameters[1].Value = durationParam;
                        
                        nlog.Debug(sql + " start");
                        int result = cmd.ExecuteNonQuery();
                        nlog.Debug(sql + " end");
                    }
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

        // 2017-10-31 iwai-tamura upd str 職能判定 処理状態取得
        /// <summary>
        /// 職能判定 処理状態取得
        /// </summary>
        /// <param name="yearParam">対象年度</param>
        /// <param name="durationParam">対象期区分</param>
        /// <returns></returns>
        public string GetSkillStatus(string yearParam, string durationParam) {
            try {
                
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "";
                
                //社員番号有効チェック
                sql = "SELECT *"
                    + "  FROM SD_VT職能判定Access処理状態";
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet()) {
                    //DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                    //var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    //parameters[0].Value = EmpNo;

                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);        // データセットに設定する

                    if((ds.Tables[0].Rows[0]["社員年度"].ToString() == yearParam) && (ds.Tables[0].Rows[0]["社員期区分"].ToString() == durationParam) ){
                        //状態確認
                        if(int.Parse(ds.Tables[0].Rows[0]["社員処理状態区分"].ToString()) < 18){
                            message = "Accessシステムでの初期処理が終わっていません。確認してください。";
                            return message;
                        } else if(int.Parse(ds.Tables[0].Rows[0]["社員処理状態区分"].ToString()) > 18){
                            message = "既にAccessシステムに取り込まれています。確認してください。";
                            return message;
                        }
                    } else {
                        message = "対象の期間が不正です。確認してください。";
                        return message;
                    }

                    if((ds.Tables[0].Rows[0]["嘱託年度"].ToString() == yearParam) && (ds.Tables[0].Rows[0]["嘱託期区分"].ToString() == durationParam) ){
                        //状態確認
                        if(int.Parse(ds.Tables[0].Rows[0]["嘱託処理状態区分"].ToString()) < 18){
                            message = "Accessシステム(嘱託)での初期処理が終わっていません。確認してください。";
                            return message;
                        } else if(int.Parse(ds.Tables[0].Rows[0]["嘱託処理状態区分"].ToString()) > 18){
                            message = "既にAccessシステム(嘱託)に取り込まれています。確認してください。";
                            return message;
                        }
                    } else {
                        message = "対象の期間が不正です。確認してください。";
                        return message;
                    }
                }
                return message;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }   
        // 2017-10-31 iwai-tamura upd end 職能判定 処理状態取得


    
    
    }
}
