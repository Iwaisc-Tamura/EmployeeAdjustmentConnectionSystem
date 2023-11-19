using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Encrypt;
using EmployeeAdjustmentConnectionSystem.COM.Enum;

namespace EmployeeAdjustmentConnectionSystem.BL.Login {

    /// <summary>
    /// ログイン用ビジネスロジック
    /// </summary>
    public class LoginBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoginBL() {
        }

        /// <summary>
        /// サーバ状態チェック
        /// </summary>
        public String ServerStatusCheck() {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                var strStatus = "";
                var sql = "select"
                        + "  ServerStatus"
                        + "  from TEM991管理情報 ";
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                    // データセットに設定する
                    da.Fill(ds);
                    StringBuilder sb = new StringBuilder();
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        strStatus = row["ServerStatus"].ToString();
                        HttpContext.Current.Session["ServerStatus"] = strStatus;
                        HttpContext.Current.Session["MaintenanceMessage"] = "2022/11/21 16:00まで使用できません。";
                        return strStatus;
                    }
                }
                return strStatus;
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
        /// ログイン処理
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string uid, string password, LoginViewModelSd vm) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                var sql = "select"
                        + "  TMLogin.社員番号"

                        + " ,TMLogin.管理区分"
                        + " ,TMLogin.PS更新年月日"
                        + " ,TM基本.戸籍名字 "
                        + " ,TM基本.戸籍名前 "
                        + " ,TM基本.所属番号 "
                        //2023-11-20 iwai-tamura add str -----
                        + " ,TM管理.対象年度 "
                        //2023-11-20 iwai-tamura add end -----
                        + "  from TEM900LoginPassword as TMLogin "
                        + "     inner join TEM100社員基本情報Data as TM基本 on TMLogin.社員番号 = TM基本.社員番号"
                        //2023-11-20 iwai-tamura add str -----
                        + "     ,TEM991管理情報 as TM管理"
                        //2023-11-20 iwai-tamura add end -----
                        + " where TMLogin.社員番号 = @EmployeeNo and TMLogin.Password = @Password";
                
                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Password", DbType.String);
                    ((IDbDataParameter)cmd.Parameters[0]).Value = uid;
                    ((IDbDataParameter)cmd.Parameters[1]).Value = DataConv.IfNull(EncryptUtil.EncryptString(password));
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

                    // データセットに設定する
                    da.Fill(ds);

                    StringBuilder sb = new StringBuilder();
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        LoginUser luser = new LoginUser {
                            UserCode = KanaEx.ToHankaku(uid.ToUpper()),     //全角を半角、小文字を大文字に変換
                            UserName = row["戸籍名字"].ToString() + " " +  row["戸籍名前"].ToString(),
                            IsRoot = (String.IsNullOrEmpty(row["管理区分"].ToString()) ? "0" : row["管理区分"].ToString()) == "K" ? true : false,
                            IsRootUser = (String.IsNullOrEmpty(row["管理区分"].ToString()) ? "0" : row["管理区分"].ToString()) == "K" ? true : false,
                            Permission = (Permissions)Enum.Parse(typeof(Permissions), (String.IsNullOrEmpty(row["管理区分"].ToString()) ? "0" : row["管理区分"].ToString()) == "K" ? "1" :"0"),
                            DepartmentNo = row["所属番号"].ToString(),
                            //2023-11-20 iwai-tamura add str -----
                            IsAdminNo=row["管理区分"].ToString(),
                            IsYear = Convert.ToInt32(row["対象年度"]),
                            //2023-11-20 iwai-tamura add end -----

                            //DepartmentName = row["所属名称"].ToString(),
                            //PostNo = row["役職番号"].ToString(),
                            //IsPost = DataConv.IntParse(row["役職番号"].ToString()) < 600 ? true : false,
                            //IsPost2 = DataConv.IntParse(row["役職番号"].ToString()) < 400 ? 
                            //             (DataConv.IntParse(row["役職番号"].ToString()) != 280 ? 
                            //                (DataConv.IntParse(row["役職番号"].ToString()) != 281 ? true : false) : false) : false,
                        };
                        //ログイン状態
                        HttpContext.Current.Session["IsLogin"] = true;
                        HttpContext.Current.Session["LoginUser"] = luser;
                        // パスワード有効期限
                        HttpContext.Current.Session["IsPassExpiration"] = false;    //ok
                        //////////DateTime dt1 = new DateTime(2000,1,1);  //初期値
                        //////////if (String.IsNullOrEmpty(row["PS更新年月日"].ToString())){
                        //////////    //PS更新年月日がNullの場合は初期値で計算(必ず変更)
                        //////////}else{
                        //////////    dt1 = DateTime.ParseExact(row["PS更新年月日"].ToString(), "yyyyMMdd", null);
                        //////////}
                        //////////if (double.Parse(row["PS有効日数"].ToString())==0){
                        //////////    //有効日数が0の場合は無視する
                        //////////    HttpContext.Current.Session["IsPassExpiration"] = false;
                        //////////}else{
                        //////////    //最後に更新した日付から有効日数を足した日付でPC日付と比較
                        //////////    if (dt1.AddDays(double.Parse(row["PS有効日数"].ToString())) < DateTime.Today){
                        //////////        HttpContext.Current.Session["IsPassExpiration"] = true; //有効日数超過
                        //////////    }else{
                        //////////        HttpContext.Current.Session["IsPassExpiration"] = false;    //ok
                        //////////    }
                        //////////}
                        return true;
                    }
                }
                return false;
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
        /// ログイン状態か判定
        /// </summary>
        /// <returns>判定結果</returns>
        public bool IsLogin() {
            return HttpContext.Current.Session["LoginUser"] == null ? false : true;
        }

        // 2017-06-16 iwai-tamura upd str パスワード有効期限追加
        /// <summary>
        /// パスワードの有効期限が過ぎているか判定
        /// </summary>
        /// <returns>判定結果</returns>
        public bool IsPassExpiration() {
            return (bool)HttpContext.Current.Session["IsPassExpiration"] == true ? true:false;
        }            
        // 2017-06-16 iwai-tamura upd end パスワード有効期限追加

    }
}
