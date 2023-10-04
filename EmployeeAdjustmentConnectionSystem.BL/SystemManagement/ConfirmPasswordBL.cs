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
using System.Web;

namespace EmployeeAdjustmentConnectionSystem.BL.SystemManagement
{
    /// <summary>
    /// パスワード確認ビジネスロジック
    /// </summary>
    public class ConfirmPasswordBL
    {
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
        public ConfirmPasswordBL() {
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
        /// パスワード確認
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Confirm(SystemManagementModels model)
        {
            try
            {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string sql = "";                                //実行するクエリを保持
                string message = "確認できませんでした。";      //画面に表示させるエラーメッセージを保持
                string ConfirmPwd = "";                         //対象パスワード
                // 2017-06-16 iwai-tamura upd str 確認情報の追加
                string strInfo = "";                         //表示情報
                // 2017-06-16 iwai-tamura upd end 確認情報の追加
                //ログインユーザーの社員番号とシステム管理権限を取得
                string strUserCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).UserCode.ToString();

                string permission = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
                    (HttpContext.Current.Session["LoginUser"])).SYS管理PASS確認.ToString();

                //システム管理者(権限=K)以外の場合は、対象社員が権限内であるか確認
                if (permission != "K")
                {
                    //対象社員の事業所番号を取得
                    // 2017-07-12 iwai-tamura upd str 参照先テーブルの変更
                    sql = "select "
                        + " left(m1.所属番号,1) as 事業所番号 "
                        + "from "
                        + " SD_Mシステムユーザー t1 "
                        + "  left join SD_M人事Data基本情報 m1 "
                        + "  on t1.社員番号 = m1.社員番号 "
                        + "where "
                        + "  t1.社員番号 = @employee ";
                    //sql = "select "
                    //    + " m1.事業所番号 "
                    //    + "from "
                    //    + " SD_Mシステムユーザー t1 "
                    //    + "  left join SD_VT901社員固定情報 m1 "
                    //    + "  on t1.社員番号 = m1.社員番号 "
                    //    + "where "
                    //    + " m1.年度 = ( select "
                    //    + "              MAX(年度) "
                    //    + "             from "
                    //    + "              SD_VT901社員固定情報 m2 "
                    //    + "             where "
                    //    + "              m1.社員番号 = m2.社員番号 ) "
                    //    + " and m1.期区分 = ( select "
                    //    + "                    MAX(期区分) "
                    //    + "                   from "
                    //    + "                    SD_VT901社員固定情報 m3 "
                    //    + "                   where "
                    //    + "                    m1.社員番号 = m3.社員番号 "
                    //    + "                    and m1.年度 = m3.年度 ) "
                    //    + " and t1.社員番号 = @employee ";
                    // 2017-07-12 iwai-tamura upd end 参照先テーブルの変更

                    string targetNum = "";      //対象者の事業所番号を保持

                    DataSet dataSet = new DataSet();
                    using (DbManager dm = new DbManager())
                    using (IDbCommand cmd = dm.CreateCommand(sql))
                    {
                        //パラメータ設定
                        DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.ResetEmployeeNo;

                        //クエリ実行
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(dataSet);

                        //結果が0件であった場合は、エラーメッセージ？
                        if (dataSet.Tables[0].Rows.Count == 0) return message;

                        //事業所番号を変数に保持
                        targetNum = dataSet.Tables[0].Rows[0]["事業所番号"].ToString();
                    }
                    //ログインユーザーのシステム管理権限と事業所番号が不一致の場合は、作業対象外としてエラー
                    if (permission != targetNum) return message;
                }

                //受け取った社員番号でパスワードを取得する
                // 2017-06-16 iwai-tamura upd str 確認情報の追加
                sql = "select sms.パスワード"
                    + "  ,smj.社員番号"
                    + "  ,smj.氏名"
                    + "  ,smj.所属番号"
                    + "  ,smj.所属名"
                    + "  from SD_Mシステムユーザー sms"
                    + "  Left join SD_M人事Data基本情報 smj on sms.社員番号 = smj.社員番号"
                    + " where sms.社員番号 = @employee";
                //sql = "select パスワード"
                //    + "  from SD_Mシステムユーザー"
                //    + " where 社員番号 = @employee";
                // 2017-06-16 iwai-tamura upd end 確認情報の追加

                using (DbManager dm = new DbManager())
                using (IDbCommand cmd = dm.CreateCommand(sql))
                {
                    DbHelper.AddDbParameter(cmd, "@employee", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = model.ResetEmployeeNo;

                    //クエリ実行
                    DataSet dataSet = new DataSet();
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    da.Fill(dataSet);

                    if (dataSet.Tables[0].Rows.Count == 0) return message;
                    ConfirmPwd = EncryptUtil.DecryptString(dataSet.Tables[0].Rows[0]["パスワード"].ToString());
                    // 2017-06-16 iwai-tamura upd str 確認情報の追加
                    strInfo = string.Format("パスワード:{0}<br>"
                                            + "<br>"
                                            + "社員番号：{1} 氏名：{2}<br>"
                                            + "所属番号：{3}<br>"
                                            + "所属名：{4}<br>"
                                            ,ConfirmPwd
                                            ,dataSet.Tables[0].Rows[0]["社員番号"].ToString()
                                            ,dataSet.Tables[0].Rows[0]["氏名"].ToString()
                                            ,dataSet.Tables[0].Rows[0]["所属番号"].ToString()
                                            ,dataSet.Tables[0].Rows[0]["所属名"].ToString()
                                            );
                    // 2017-06-16 iwai-tamura upd end 確認情報の追加

                }
                // 2017-06-16 iwai-tamura upd str 確認情報の追加
                return strInfo;
                //return "社員番号：" + model.ResetEmployeeNo.ToString() + " パスワード：" + ConfirmPwd;
                // 2017-06-16 iwai-tamura upd end 確認情報の追加
            }
            catch (Exception ex)
            {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            }
            finally
            {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

    }
}
