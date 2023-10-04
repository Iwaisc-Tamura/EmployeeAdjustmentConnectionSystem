using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Util.Encrypt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement
{
    public class AddAccountBL
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        
        /// デフォルトパスワード
        /// </summary>
        private string defaultPwd;

        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddAccountBL() {
            try {
                //帳票作成ディレクトリを取得
                Configuration config = WebConfig.GetConfigFile();
                defaultPwd = config.AppSettings.Settings["DFP"].Value;
            } catch (Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }


        /// <summary>
        /// アカウント追加処理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Add(SystemManagementModels model)
        {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "アカウントを追加できませんでした。";      //画面に表示させるエラーメッセージを保持
                
                //ログインユーザーの社員番号とシステム管理権限を取得
                string strUserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                string permission = ((int)((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).Permission).ToString();

                //システム管理者(権限=1)以外の場合は、エラー
                if(permission != "1") {
                    message = "アカウントを追加に必要な管理権限が設定されていません。";
                    return message;
                }
                
                using(var scope = new TransactionScope())
                {
                    //社員番号使用可否チェック
                    sql = "SELECT 社員番号"
                        + "  FROM SD_Mシステムユーザー"
                        + " WHERE 社員番号 = @employee";
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet())
                    {
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.AddEmployeeNo;
                    
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);        // データセットに設定する
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            message = "入力された社員番号は、既にアカウント追加済の社員番号です。";
                            return message;
                        }
                    }

                    //社員番号有効チェック
                    sql = "SELECT 社員番号"
                        + "  FROM SD_VT人事Data基本情報"
                        + " WHERE 社員番号 = @employee"
                        + "   AND 所属番号 IS NOT NULL";
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet())
                    {
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.AddEmployeeNo;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);        // データセットに設定する
                        if (!(ds.Tables[0].Rows.Count > 0))
                        {
                            message = "入力された社員番号は、無効な社員番号です。";
                            return message;
                        }
                    }

                    //受け取った社員番号とパスワードでユーザーを作成する
                    // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                    sql = "insert SD_Mシステムユーザー"
                        + " (社員番号,パスワード,システム管理アクセス権,PS更新年月日)"
                        + " values(@employee,@pwd,0,@nowdate)";
                    //sql = "insert SD_Mシステムユーザー"
                    //    + " (社員番号,パスワード,システム管理アクセス権)"
                    //    + " values(@employee,@pwd,0)";
                    // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    {

                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@pwd", DbType.String);
                        // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                        DbHelper.AddDbParameter(cmd, "@nowdate", DbType.String);
                        // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        // 2017-06-16 iwai-tamura upd str 大文字変換
                        parameters[0].Value = model.AddEmployeeNo.ToUpper();
                        //parameters[0].Value = model.AddEmployeeNo;
                        // 2017-06-16 iwai-tamura upd end 大文字変換
                        parameters[1].Value = EncryptUtil.EncryptString(model.AddPassword);
                        // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                        parameters[2].Value = DateTime.Today.ToString("yyyyMMdd");
                        // 2017-06-16 iwai-tamura upd end パスワード有効期限追加

                        cmd.ExecuteNonQuery();                        
                    }

                    scope.Complete();
                }
                return "アカウントを追加しました。<br />社員番号:" + model.AddEmployeeNo + " Password:" + model.AddPassword;
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        // 2017-06-16 iwai-tamura upd str アカウント参照追加機能
        /// <summary>
        /// 参照元社員情報取得
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public string GetReferenceEmployeeName(string EmpNo) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "";
                
                //社員番号有効チェック
                sql = "SELECT 氏名"
                    + "  FROM SD_VT人事Data基本情報"
                    + " WHERE 社員番号 = @employee"
                    + "   AND 所属番号 IS NOT NULL";
                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                using (DataSet ds = new DataSet()) {
                    DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = EmpNo;

                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(ds);        // データセットに設定する
                    if (ds.Tables[0].Rows.Count > 0) {
                        message = "社員番号:" + EmpNo + "<br />氏名：" + ds.Tables[0].Rows[0]["氏名"].ToString();
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

        /// <summary>
        /// アカウント参照追加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string ReferenceAdd(SystemManagementModels model)
        {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "アカウントを追加できませんでした。";      //画面に表示させるエラーメッセージを保持
                string password = ""; //パスワード
                string accessright = ""; //システム管理アクセス権
                
                //ログインユーザーの社員番号とシステム管理権限を取得
                string strUserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                string permission = ((int)((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).Permission).ToString();

                //システム管理者(権限=1)以外の場合は、エラー
                if(permission != "1") {
                    message = "アカウントを追加に必要な管理権限が設定されていません。";
                    return message;
                }
                
                using(var scope = new TransactionScope())
                {
                    //社員番号使用可否チェック
                    sql = "SELECT 社員番号"
                        + "  FROM SD_Mシステムユーザー"
                        + " WHERE 社員番号 = @employee";
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet())
                    {
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.AddEmployeeNo;
                    
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);        // データセットに設定する
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            message = "入力された社員番号は、既にアカウント追加済の社員番号です。";
                            return message;
                        }
                    }

                    //社員番号有効チェック
                    sql = "SELECT 社員番号"
                        + "  FROM SD_VT人事Data基本情報"
                        + " WHERE 社員番号 = @employee"
                        + "   AND 所属番号 IS NOT NULL";
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet())
                    {
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.AddEmployeeNo;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);        // データセットに設定する
                        if (!(ds.Tables[0].Rows.Count > 0))
                        {
                            message = "入力された社員番号は、無効な社員番号です。";
                            return message;
                        }
                    }


                    //参照元社員情報取得
                    sql = "SELECT パスワード,システム管理アクセス権"
                        + "  FROM SD_Mシステムユーザー"
                        + " WHERE 社員番号 = @employee";
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    using (DataSet ds = new DataSet())
                    {
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.ReferenceEmployeeNo;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);        // データセットに設定する
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            password = ds.Tables[0].Rows[0]["パスワード"].ToString();
                            accessright = ds.Tables[0].Rows[0]["システム管理アクセス権"].ToString();
                        }
                    }

                    //受け取った社員番号とパスワードでユーザーを作成する
                    sql = "insert SD_Mシステムユーザー"
                        + " (社員番号,パスワード,システム管理アクセス権,PS更新年月日)"
                        + " values(@employee,@pwd,@accessright,@nowdate)";

                    
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    {

                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@pwd", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@accessright", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@nowdate", DbType.String);
                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.AddEmployeeNo.ToUpper();
                        parameters[1].Value = password;
                        parameters[2].Value = accessright;
                        parameters[3].Value = DateTime.Today.ToString("yyyyMMdd");

                        cmd.ExecuteNonQuery();                        
                    }

                    scope.Complete();
                }
                return "アカウントを追加しました。<br />社員番号:" + model.AddEmployeeNo + " Password:" + EncryptUtil.DecryptString(password);
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }    
        // 2017-06-16 iwai-tamura upd end アカウント参照追加機能

    }
}
