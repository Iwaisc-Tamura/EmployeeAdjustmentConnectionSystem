using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using System.Data;
using EmployeeAdjustmentConnectionSystem.COM.Util.Encrypt;
using System.Web;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement {
    /// <summary>
    /// パスワードリセットビジネスロジック
    /// </summary>
    public class ResetPasswordBL {
        
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// デフォルトパスワード
        /// </summary>
        private string defaultPwd;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResetPasswordBL() {
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
        /// パスワードリセット
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Reset(SystemManagementModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "リセットできませんでした。";      //画面に表示させるエラーメッセージを保持
                
                //ログインユーザーの社員番号とシステム管理権限を取得
                string strUserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                // 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                //string permission = ((int)((SkillDiscriminantSystem.COM.Entity.Session.LoginUser)
                //    (HttpContext.Current.Session["LoginUser"])).Permission).ToString();
                string permission = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).SYS管理PASSRESET.ToString();

                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加

                //// 2017-03-31 sbc-sagara upd str 支社管理機能の追加
                ////システム管理者(権限=1)以外の場合は、対象社員が権限内であるか確認
                //if(permission != "1") {
                //システム管理者(権限=K)以外の場合は、対象社員が権限内であるか確認
                if (permission != "K") {
                // 2017-03-31 sbc-sagara upd end 支社管理機能の追加
                    //対象社員の事業所番号を取得
                    sql = "select "
                        // 2018-09-20 iwai-tamura upd str ------
                        + " LEFT(m1.所属番号,1) AS 事業所番号 "
                        // + " m1.事業所番号 "
                        // 2018-09-20 iwai-tamura upd end ------
                        + "from "
                        + " SD_Mシステムユーザー t1 "
                        // 2018-09-20 iwai-tamura upd str ------
                        + "  left join SD_M人事Data基本情報 m1 "
                        + "  on t1.社員番号 = m1.社員番号 "
                        + "where "
                        + " t1.社員番号 = @employee ";
                        //+ "  left join SD_VT901社員固定情報 m1 "
                        //+ "  on t1.社員番号 = m1.社員番号 "
                        //+ "where "
                        //+ " m1.年度 = ( select "
                        //+ "              MAX(年度) "
                        //+ "             from "
                        //+ "              SD_VT901社員固定情報 m2 "
                        //+ "             where "
                        //+ "              m1.社員番号 = m2.社員番号 ) "
                        //+ " and m1.期区分 = ( select "
                        //+ "                    MAX(期区分) "
                        //+ "                   from "
                        //+ "                    SD_VT901社員固定情報 m3 "
                        //+ "                   where "
                        //+ "                    m1.社員番号 = m3.社員番号 "
                        //+ "                    and m1.年度 = m3.年度 ) "
                        //+ " and t1.社員番号 = @employee ";
                        // 2018-09-20 iwai-tamura upd str ------

                    string targetNum = "";      //対象者の事業所番号を保持

                    DataSet dataSet = new DataSet();
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql))
                    {
                        //パラメータ設定
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.ResetEmployeeNo;

                        //クエリ実行
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(dataSet);

                        //結果が0件であった場合は、エラーメッセージ？
                        if(dataSet.Tables[0].Rows.Count == 0) return message;

                        //事業所番号を変数に保持
                        targetNum = dataSet.Tables[0].Rows[0]["事業所番号"].ToString();
                    }
                    
                    //ログインユーザーのシステム管理権限と事業所番号が不一致の場合は、作業対象外としてエラー
                    if(permission != targetNum) return message;
                }
                
                //受け取った社員番号でパスワードを書き換える
                sql = "update SD_Mシステムユーザー set パスワード = @pwd "
                    // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                    + " ,PS更新年月日 = @NowDate "
                    // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                    + "where 社員番号 = @employee ";

                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql)){
                    DbHelper.AddDbParameter(cmd, "@pwd", DbType.String);
                    // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
                    DbHelper.AddDbParameter(cmd, "@NowDate", DbType.String);
                    // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                    DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                    parameters[0].Value = EncryptUtil.EncryptString(defaultPwd);
                    parameters[1].Value = DateTime.Today.ToString("yyyyMMdd");
                    parameters[2].Value = model.ResetEmployeeNo;
                    //parameters[0].Value = EncryptUtil.EncryptString(defaultPwd);
                    //parameters[1].Value = model.ResetEmployeeNo;
                    // 2017-06-16 iwai-tamura upd end パスワード有効期限追加
                    
                    cmd.ExecuteNonQuery();
                }
                return "完了しました。";
            } catch (Exception ex) {
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
