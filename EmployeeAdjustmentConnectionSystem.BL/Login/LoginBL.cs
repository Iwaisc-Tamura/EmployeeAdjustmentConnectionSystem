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
                //////////var sql = "select"
                //////////        + "  sms.社員番号"
                //////////        + " ,sms.システム管理アクセス権"
                //////////        // 2017-06-16 iwai-tamura add str パスワード有効期限追加
                //////////        + " ,sms.PS更新年月日"
                //////////        + " ,tmk.PS有効日数"
                //////////        // 2017-06-16 iwai-tamura add end パスワード有効期限追加
                //////////        + " ,svs.氏名"
                //////////        + " ,svs.DB区分"
                //////////        + " ,svs.資格番号"
                //////////        + " ,svs.所属番号"
                //////////        + " ,svs.役職番号"
                //////////        + " ,tms.所属名称"
                //////////        // 2018-07-27 iwai-tamura add str ------
                //////////        + " ,tmn.管理区分"
                //////////        // 2018-07-27 iwai-tamura add end ------
                //////////        // 2017-03-31 sbc-sagara add str 支社管理機能の追加
                //////////        + " ,smk.SYS管理MENU,smk.SYS管理初期処理,smk.SYS管理確定処理"
                //////////        + " ,smk.SYS管理USER追加,smk.SYS管理PASS確認,smk.SYS管理PASSRESET"
                //////////        // 2017-09-15 iwai-tamura add str ------
                //////////        + " ,smk.SYS管理決裁権限出力,smk.SYS管理決裁権限取込"
                //////////        // 2017-09-15 iwai-tamura add end ------
                //////////        // 2018-03-20 iwai-tamura upd str ------
                //////////        + " ,smk.SYS管理異動DATA引継処理,smk.SYS管理組編DATA引継処理"
                //////////        // 2018-03-20 iwai-tamura upd end ------
                //////////        + " ,smk.SYS管理連絡事項,smk.目標検索対象,smk.目標DATA出力"
                //////////        + " ,smk.職能検索対象,smk.職能DATA出力,smk.職能一括入力"
                //////////        + " ,smk.職能集計表MENU,smk.職能集計表検索対象,smk.職能判定除外"
                //////////        + " ,smk.決裁権限検索対象,smk.決裁権限登録制限"
                //////////        // 2017-03-31 sbc-sagara add end 支社管理機能の追加

                //////////        // 2018-99-99 iwai-tamura add str 自己申告書関連の管理機能追加
                //////////        + " ,smk.自己申告書検索対象"
                //////////        + " ,smk.自己申告書DATA出力"
                //////////        + " ,smk.自己申告書決裁権限検索対象"
                //////////        + " ,smk.自己申告書決裁権限登録制限"
                //////////        // 2018-99-99 iwai-tamura add end 自己申告書関連の管理機能追加
                        
                //////////        + "  from SD_Mシステムユーザー sms "
                //////////        + " inner join "
                //////////        //2016-05-26 iwai-tamura upd str -----
                //////////        + "    (select "
                //////////        + "        CASE "
                //////////        + "            WHEN 身分ｺｰﾄﾞ < '20' THEN 'H'"
                //////////        + "            ELSE 'S'"
                //////////        + "        END DB区分"
                //////////        + "      ,t1.氏名"
                //////////        + "      ,t1.社員番号"
                //////////        + "      ,t1.資格番号"
                //////////        + "      ,t1.所属番号"
                //////////        + "      ,t1.役職番号"
                //////////        + "    from SD_VT人事Data基本情報 t1"
                //////////        + "    ) svs"

                //////////        //    + "    (select "
                //////////        //    + "       t1.DB区分"
                //////////        //    + "      ,t1.年度"
                //////////        //    + "      ,t1.期区分"
                //////////        //    + "      ,t1.氏名"
                //////////        //    + "      ,t1.社員番号"
                //////////        //    + "      ,t1.資格番号"
                //////////        //    + "      ,t1.所属番号"
                //////////        //    + "      ,t1.役職番号"
                //////////        //    + "       from SD_VT901社員固定情報 t1"
                //////////        ////+ "       left join SD_M期間管理 t2"
                //////////        ////+ "         on t1.年度 = t2.年度"
                //////////        ////+ "      where t2.期区分 = '0'"
                //////////        ////+ "        and t1.期区分 = (select max(期区分)"
                //////////        ////+ "                          from SD_VT901社員固定情報 as t3"
                //////////        ////+ "                         where t3.年度 = t2.年度"
                //////////        ////+ "                           and t3.社員番号 = t1.社員番号)) svs"
                //////////        //    + "        where t1.TBL区分 ='G' ) svs"
                //////////        //2016-05-26 iwai-tamura upd end -----
                //////////        + "    on sms.社員番号 = svs.社員番号"
                //////////        //2017-09-15 iwai-tamura upd-str ------
                //////////        + "  left join SD_M所属Master tms"
                //////////        //+ "  left join TM911所属Master tms"
                //////////        //2017-09-15 iwai-tamura upd-end ------
                //////////        + "    on svs.所属番号 = tms.所属番号"
                //////////        // 2017-03-31 sbc-sagara add str 支社管理機能の追加
                //////////        + "  left join TM990入力担当者 tmn on sms.社員番号 = tmn.入力担当社員番号"
                //////////        + "  left join SD_M管理区分権限Master smk on tmn.管理区分 = smk.管理区分"
                //////////        // 2017-03-31 sbc-sagara add end 支社管理機能の追加
                //////////        // 2017-06-16 iwai-tamura add str パスワード有効期限追加
                //////////        + "  left join TM991管理情報 tmk on 1=1"
                //////////        // 2017-06-16 iwai-tamura add end パスワード有効期限追加
                //////////        + " where sms.社員番号 = @EmployeeNo and パスワード = @Password";
                ////////////sql += string.Format(" where sms.社員番号 ='{0}' and パスワード ='{1}'", uid, password);

                var sql = "select"
                        + "  TMLogin.社員番号"

                        + " ,TMLogin.管理区分"
                        + " ,TMLogin.PS更新年月日"
                        + " ,TM基本.戸籍名字 "
                        + " ,TM基本.戸籍名前 "
                        + " ,TM基本.所属番号 "
                        + "  from TEM900LoginPassword as TMLogin "
                        + "     inner join TEM100社員基本情報Data as TM基本 on TMLogin.社員番号 = TM基本.社員番号"
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
