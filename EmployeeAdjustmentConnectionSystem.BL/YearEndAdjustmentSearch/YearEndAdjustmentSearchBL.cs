using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;

namespace EmployeeAdjustmentConnectionSystem.BL.YearEndAdjustmentSearch {
    /// <summary>
    /// 目標管理照会
    /// </summary>
    public class YearEndAdjustmentSearchBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="model">年末調整検索モデル</param>
        /// <returns>検索モデル</returns>
        public YearEndAdjustmentSearchViewModels Search(YearEndAdjustmentSearchViewModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //上限値取得
                Configuration config = WebConfig.GetConfigFile();
                var limit = config.AppSettings.Settings["SEARCH_LIMIT"].Value;

                //条件作成
                Func<string, string, string> condition = (format, value) => {
                    return string.IsNullOrEmpty(value) ? "" :
                        value.Trim() == "" ? "" : string.Format(format, value.Trim());
                };
                var sql = "  SELECT T基本.*";
                
                //確定区分取得
                sql += ",T扶養控除.本人確定区分 As 扶養控除_本人確定区分 ";
                sql += ",T扶養控除.管理者確定区分 As 扶養控除_管理者確定区分 ";
                sql += ",T保険控除.本人確定区分 As 保険控除_本人確定区分 ";
                sql += ",T保険控除.管理者確定区分 As 保険控除_管理者確定区分 ";
                sql += ",T基礎控除.本人確定区分 As 基礎控除_本人確定区分 ";
                sql += ",T基礎控除.管理者確定区分 As 基礎控除_管理者確定区分 ";
                         
                sql += " FROM TEM100社員基本情報Data as T基本 ";
                sql += "    LEFT JOIN TE100扶養控除申告書Data as T扶養控除";
                sql +=  string.Format(" 	ON T基本.社員番号 = T扶養控除.社員番号 And T扶養控除.対象年度 = {0}", model.Search.Year);
                sql += "    LEFT JOIN TE110保険料控除申告書Data as T保険控除";
                sql +=  string.Format(" 	ON T基本.社員番号 = T保険控除.社員番号 And T保険控除.対象年度 = {0}", model.Search.Year);
                sql += "    LEFT JOIN TE120基礎控除申告書Data as T基礎控除";
                sql +=  string.Format(" 	ON T基本.社員番号 = T基礎控除.社員番号 And T基礎控除.対象年度 = {0}", model.Search.Year);
                sql += " {0} ";

                var sqlc = "where 1=1";

                //年調データ
                sqlc += (" and ((T扶養控除.社員番号 is not null) or (T保険控除.社員番号 is not null) or (T基礎控除.社員番号 is not null))");


                //所属
                sqlc += condition(" and left(T基本.所属番号,4) >=left('{0}',4)", model.Search.DepartmentFrom);
                sqlc += condition(" and left(T基本.所属番号,4) <=left('{0}',4)", model.Search.DepartmentTo);

                //社員番号
                string empsql = "";
                empsql += condition(" and T基本.社員番号 >='{0}'", model.Search.EmployeeNoFrom);
                empsql += condition(" and T基本.社員番号 <='{0}'", model.Search.EmployeeNoTo);
                sqlc += empsql;
                //氏名
                sqlc += condition(" and (T基本.戸籍名字 + T基本.戸籍名前) like '%{0}%'", model.Search.EmployeeName);

                //氏名カナ
                sqlc += condition(" and (T基本.戸籍名字Kana + T基本.戸籍名前Kana) like '%{0}%'", KanaEx.ToHankakuKana(model.Search.EmployeeNameKana));


                //ステータス
                //扶養控除
                switch (model.Search.HuyouDeclareStatus) {
                    case "00":  //本人未提出
                        sqlc += " and (T扶養控除.本人確定区分 = '0' and T扶養控除.管理者確定区分 = '0') ";
                        break;

                    case "90":  //本人提出済み
                        sqlc += " and ((T扶養控除.本人確定区分 = '1' and T扶養控除.管理者確定区分 = '0') ";
                        sqlc += "       or(T扶養控除.本人確定区分 = '9' and T扶養控除.管理者確定区分 = '0')) ";
                        break;

                    case "91":  //支社確定済み
                        sqlc += " and (T扶養控除.本人確定区分 = '9' and T扶養控除.管理者確定区分 = '1') ";
                        break;

                    case "95":  //管理者確定済み
                        sqlc += " and (T扶養控除.本人確定区分 = '9' and T扶養控除.管理者確定区分 = '5') ";
                        break;

                    case "99":  //システム連携済み
                        sqlc += " and (T扶養控除.本人確定区分 = '9' and T扶養控除.管理者確定区分 = '9') ";
                        break;

                    default: break;
                }
                
                //保険料控除
                switch (model.Search.HokenDeclareStatus) {
                    case "00":  //本人未提出
                        sqlc += " and (T保険控除.本人確定区分 = '0' and T保険控除.管理者確定区分 = '0') ";
                        break;

                    case "90":  //本人提出済み
                        sqlc += " and ((T保険控除.本人確定区分 = '1' and T保険控除.管理者確定区分 = '0') ";
                        sqlc += "       or(T保険控除.本人確定区分 = '9' and T保険控除.管理者確定区分 = '0')) ";
                        break;

                    case "91":  //支社確定済み
                        sqlc += " and (T保険控除.本人確定区分 = '9' and T保険控除.管理者確定区分 = '1') ";
                        break;

                    case "95":  //管理者確定済み
                        sqlc += " and (T保険控除.本人確定区分 = '9' and T保険控除.管理者確定区分 = '5') ";
                        break;

                    case "99":  //システム連携済み
                        sqlc += " and (T保険控除.本人確定区分 = '9' and T保険控除.管理者確定区分 = '9') ";
                        break;

                    default: break;
                }

                //基礎控除
                switch (model.Search.HaiguuDeclareStatus) {
                    case "00":  //本人未提出
                        sqlc += " and (T基礎控除.本人確定区分 = '0' and T基礎控除.管理者確定区分 = '0') ";
                        break;

                    case "90":  //本人提出済み
                        sqlc += " and ((T基礎控除.本人確定区分 = '1' and T基礎控除.管理者確定区分 = '0') ";
                        sqlc += "       or(T基礎控除.本人確定区分 = '9' and T基礎控除.管理者確定区分 = '0')) ";
                        break;

                    case "91":  //支社確定済み
                        sqlc += " and (T基礎控除.本人確定区分 = '9' and T基礎控除.管理者確定区分 = '1') ";
                        break;

                    case "95":  //管理者確定済み
                        sqlc += " and (T基礎控除.本人確定区分 = '9' and T基礎控除.管理者確定区分 = '5') ";
                        break;

                    case "99":  //システム連携済み
                        sqlc += " and (T基礎控除.本人確定区分 = '9' and T基礎控除.管理者確定区分 = '9') ";
                        break;

                    default: break;
                }

                //ログインユーザー抽出可能範囲
                sqlc += " And (";

                //所属検索条件追加
                if (lu.IsAdminNo == "K")
                {
                    sqlc += " T基本.所属番号 BETWEEN 10000 AND 99999";
                }
                else if (lu.IsAdminNo != "0")
                {
                    sqlc += string.Format(" LEFT(T基本.所属番号,1) = '{0}'", lu.IsAdminNo);
                }                

                sqlc += " )";

                ////1000件まで
                //sqlc += string.Format(" and レコード番号 between 1 and {0}", string.IsNullOrEmpty(limit) ? "1000" : limit);

                sql = string.Format(sql, sqlc);


                Func<string, string, string> StatusDecision = (value1, value2) => {
                    if (value1 == "0" && value2 == "0") {
                        return "本人未提出";
                    } else if (value1 == "1" && value2 == "0") {
                        return "本人提出済み";
                    } else if (value1 == "9" && value2 == "0") {
                        return "本人提出済み";
                    } else if (value1 == "9" && value2 == "1") {
                        return "支社確定済み";
                    } else if (value1 == "9" && value2 == "5") {
                        return "管理者確定済み";
                    } else if (value1 == "9" && value2 == "9") {
                        return "システム連携済み";
                    } else {
                        return "システムエラー";
                    }
                };

                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);

                    List<YearEndAdjustmentSearchListModel> resultList = new List<YearEndAdjustmentSearchListModel>();
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        YearEndAdjustmentSearchListModel result = new YearEndAdjustmentSearchListModel {
                            Selected = false,
                            Year = model.Search.Year,
                            EmployeeNumber = row["社員番号"].ToString(),
                            EmployeeName = row["戸籍名字"].ToString() +" "+ row["戸籍名前"].ToString(),
                            EmployeeNameKana = KanaEx.ToZenkakuKana(row["戸籍名字Kana"].ToString() + " "+ row["戸籍名前Kana"].ToString()),
                            Department = row["所属番号"].ToString(),
                            HuyouDeclareStatus =StatusDecision(row["扶養控除_本人確定区分"].ToString(),row["扶養控除_管理者確定区分"].ToString()),
                            HokenDeclareStatus =StatusDecision(row["保険控除_本人確定区分"].ToString(),row["保険控除_管理者確定区分"].ToString()),
                            HaiguuDeclareStatus =StatusDecision(row["基礎控除_本人確定区分"].ToString(),row["基礎控除_管理者確定区分"].ToString()),

                        };
                        resultList.Add(result);
                    }
                    model.SearchResult = resultList;
                }
                return model;
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }

        // 2020-03-31 iwai-tamura upd str -----
        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="model">年末調整検索モデル</param>
        /// <returns>検索モデル</returns>
        public YearEndAdjustmentSearchViewModels SearchD(YearEndAdjustmentSearchViewModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //上限値取得
                Configuration config = WebConfig.GetConfigFile();
                var limit = config.AppSettings.Settings["SEARCH_LIMIT"].Value;

                //条件作成
                Func<string, string, string> condition = (format, value) => {
                    return string.IsNullOrEmpty(value) ? "" :
                        value.Trim() == "" ? "" : string.Format(format, value.Trim());
                };
                var sql = "  SELECT 基本.*";

                if (lu.目標検索対象 != "0"){
                    sql += "    ,'1' AS AtoCButtonView";

                    //sql += string.Format("   ,CASE WHEN 基本.社員番号 = '{0}' ",lu.UserCode);
                    //sql += "      THEN '1' ";
                    //sql += "      ELSE ";
                    //sql += "        CASE WHEN D権限.社員番号 is Null THEN '0' ";  //本人以外は承認者のみ表示
                    //sql += "          ELSE CASE WHEN D本人承認.承認社員番号 is null THEN '0' ELSE '1' END ";
                    //sql += "        END ";
                    //sql += "    END DButtonView";
                    
                    sql += string.Format("   ,CASE WHEN 基本.社員番号 = '{0}' ",lu.UserCode);
                    sql += "      THEN '1' ";
                    sql += "      ELSE ";
                    sql += "        CASE WHEN D本人承認.承認社員番号 is null THEN '0' ELSE '1' END ";
                    sql += "    END DButtonView";
                    sql += "    ,CASE WHEN CareerSheet.社員番号 is null THEN '0' ELSE '1' END CareerButtonView ";
                }else{
                    sql += string.Format("   ,CASE WHEN 基本.社員番号 = '{0}' ",lu.UserCode);
                    sql += "      THEN '1' ";
                    sql += "      ELSE ";
                    sql += "        CASE WHEN AtoC権限.社員番号 is Null THEN '0' ELSE '1' END ";  //本人以外は承認者のみ表示
                    sql += "    END AtoCButtonView";

                    sql += string.Format("   ,CASE WHEN 基本.社員番号 = '{0}' ",lu.UserCode);
                    sql += "      THEN '1' ";
                    sql += "      ELSE ";
                    sql += "        CASE WHEN D権限.社員番号 is Null THEN '0' ";  //本人以外は承認者のみ表示
                    sql += "          ELSE CASE WHEN D本人承認.承認社員番号 is null THEN '0' ELSE '1' END ";
                    sql += "        END ";
                    sql += "    END DButtonView";

                    sql += "    ,CASE WHEN  CareerSheet.社員番号 is null ";
                    sql += "      THEN '0' ";
                    sql += "      ELSE ";
                    sql += string.Format("     CASE WHEN 基本.社員番号 = '{0}' ",lu.UserCode);
                    sql += "          THEN '1' ";
                    sql += "          ELSE ";
                    sql += "            CASE WHEN Career権限.社員番号 is Null THEN '0' ELSE '1' END ";  //本人以外は承認者のみ表示
                    sql += "        END";
                    sql += "    END CareerButtonView";
                }                
                sql += " FROM SD_T自己申告書共通基本Data as 基本 ";
                sql += " LEFT JOIN SD_VM自己申告書決裁権限検索 as AtoC権限";
                sql +=  string.Format(" 	ON 基本.社員番号 = AtoC権限.社員番号 And 基本.年度 = AtoC権限.年度 And AtoC権限.大区分 = '1' And AtoC権限.承認者 = '{0}'", lu.UserCode);
                sql += " LEFT JOIN SD_VM自己申告書決裁権限検索 as D権限";
                sql +=  string.Format(" 	ON 基本.社員番号 = D権限.社員番号 And 基本.年度 = D権限.年度 And D権限.大区分 = '2' And D権限.承認者 = '{0}'", lu.UserCode);
                sql += " LEFT JOIN SD_T自己申告書承認情報 as D本人承認";
                sql += " 	ON 基本.社員番号 = D本人承認.社員番号 And 基本.年度 = D本人承認.年度 And D本人承認.大区分 = '2'  And D本人承認.小区分 = '1' ";
                sql += " LEFT JOIN SD_VMCareerSheet決裁権限検索 as Career権限";
                sql +=  string.Format(" 	ON 基本.社員番号 = Career権限.社員番号 And 基本.年度 = Career権限.年度 And Career権限.承認者 = '{0}'", lu.UserCode);
                sql += " LEFT JOIN SD_TCareerSheet01 as CareerSheet";
                sql += " 	ON 基本.社員番号 = CareerSheet.社員番号 And 基本.年度 = CareerSheet.年度";
                sql += " {0} ";

                var sqlc = "where 1=1";

                //年度
                sqlc += condition(" and 基本.年度 ='{0}'", model.Search.Year);

                //所属
                sqlc += condition(" and left(基本.所属番号,4) >=left('{0}',4)", model.Search.DepartmentFrom);
                sqlc += condition(" and left(基本.所属番号,4) <=left('{0}',4)", model.Search.DepartmentTo);

                //社員番号
                string empsql = "";
                empsql += condition(" and 基本.社員番号 >='{0}'", model.Search.EmployeeNoFrom);
                empsql += condition(" and 基本.社員番号 <='{0}'", model.Search.EmployeeNoTo);
                empsql = !model.Search.DesignatedFlag ? empsql :
                          condition(" and 基本.社員番号 ='{0}'", model.Search.EmployeeNoFrom);
                sqlc += empsql;
                //氏名
                sqlc += condition(" and 基本.氏名 like '%{0}%'", model.Search.EmployeeName);

                //氏名カナ
                sqlc += condition(" and 基本.Kana like '%{0}%'", KanaEx.ToHankakuKana(model.Search.EmployeeNameKana));


                //管理職以外の場合、自分自身のみ抽出
                if (!lu.IsPost){
                    sqlc += condition(" and 基本.社員番号 = '{0}'", lu.UserCode);
                }

                //ログインユーザー抽出可能範囲
                sqlc += " And (";

                sqlc += "   (not(AtoC権限.承認者 is null and D権限.承認者 is null and Career権限.承認者 is null))";
                sqlc += string.Format(" or (基本.社員番号 ='{0}')", lu.UserCode);
                
                //所属検索条件追加
                if (lu.自己申告書検索対象 == "K")
                {
                    sqlc += " or 基本.所属番号 BETWEEN 10000 AND 99999";
                }
                else if (lu.自己申告書検索対象 != "0")
                {
                    sqlc += string.Format(" or LEFT(基本.所属番号,1) = '{0}'", lu.自己申告書検索対象);
                }                

                sqlc += " )";

                ////1000件まで
                //sqlc += string.Format(" and レコード番号 between 1 and {0}", string.IsNullOrEmpty(limit) ? "1000" : limit);

                sql = string.Format(sql, sqlc);

                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);

                    List<YearEndAdjustmentSearchListModel> resultList = new List<YearEndAdjustmentSearchListModel>();
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        if (row["DButtonView"].ToString() == "1") {
                            YearEndAdjustmentSearchListModel result = new YearEndAdjustmentSearchListModel {
                                Selected = false,
                                ManageNo = row["管理番号"].ToString(),
                                Year = row["年度"].ToString(),
                                SelfDecType = row["自己申告書種別Code"].ToString(),
                                EmployeeNumber = row["社員番号"].ToString(),
                                EmployeeName = row["氏名"].ToString(),
                                EmployeeNameKana = KanaEx.ToZenkakuKana(row["Kana"].ToString()),
                                Department = row["所属番号"].ToString(),
                                CompetencyNo = row["資格番号"].ToString(),
                                Competency = row["資格名"].ToString(),
                                DutyNo = row["職掌番号"].ToString(),
                                Duty = row["職掌名"].ToString(),
                                SelfDecAtoCButtonView = row["AtoCButtonView"].ToString(),
                                SelfDecDButtonView = row["DButtonView"].ToString(),
                                CareerButtonView = row["CareerButtonView"].ToString(),
                                //ObjectivesAceept = row["設定"].ToString(),
                                //AattainmentAccept = row["達成"].ToString(),
                                //ViewLink = row["管理番号"].ToString(),
                                //TableType = row["TBL区分"].ToString(),
                                //AchvTotal = row["達成度"].ToString(),
                                //ProcessTotal = row["プロセス"].ToString()
                            };
                            resultList.Add(result);
                        }
                    }
                    model.SearchResult = resultList;
                }
                return model;
            } catch(Exception ex) {
                // エラー
                nlog.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + " error " + ex.ToString());
                throw;
            } finally {
                //終了
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " end");
            }
        }
        // 2020-03-31 iwai-tamura upd str -----



        /// <summary>
        /// 部下検索
        /// </summary>
        /// <param name="model">目標管理検索モデル</param>
        /// <returns>検索モデル</returns>
        public YearEndAdjustmentSearchViewModels SubSearch(YearEndAdjustmentSearchViewModels model, LoginUser lu) {
            //開始
            nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");
                //上限値取得
                Configuration config = WebConfig.GetConfigFile();
                var limit = config.AppSettings.Settings["SEARCH_LIMIT"].Value;

                var sql = "select"
                        + " レコード番号"
                        + ",TBL区分"
                        + ",管理番号"
                        + ",年度"
                        + ",所属番号"
                        + ",社員番号"
                        + ",氏名"
                        + ",フリガナ"
                        + ",設定"
                        + ",達成"
                        + "  from ("
                        + "select"
                        //ソート変更:年度(降順) > 所属番号 > 役職番号 > 資格 > 社員番号>
                        + " row_number() over (order by k.年度 desc,k.所属番号,s.役職番号,s.資格番号,k.社員番号) as レコード番号"
                        + ",k.TBL区分"
                        + ",k.管理番号"
                        + ",k.年度"
                        + ",k.所属番号"
                        + ",k.社員番号"
                        + ",s.氏名"
                        + ",s.フリガナ"
                        + ",CASE k.TBL区分 WHEN 'G' THEN mm.設定 WHEN 'R' THEN mmr.設定 END  as 設定コード"
                        + ",CASE k.TBL区分 WHEN 'G' THEN tm.達成 WHEN 'R' THEN tmr.達成 END  as 達成コード"
                        + ",CASE k.TBL区分 WHEN 'G' THEN mmm.内容 WHEN 'R' THEN mmmr.内容 END  as 設定"
                        + ",CASE k.TBL区分 WHEN 'G' THEN tmm.内容 WHEN 'R' THEN tmmr.内容 END  as 達成"
                        + ",vmm.目標面談者 as 目面"
                        + ",vmm.目標面談上位者 as 目面上"
                        + ",vmm.目標面談部長支店長 as 目面部"
                        + ",vmm.達成評価者 as 達面"
                        + ",vmm.達成評価上位者 as 達面上"
                        + ",vmm.達成評価部長支店長 as 達面部"
                        + ",vmm.人事担当課長 as人事 "
                        + ",vmm.総務部長 as 総務"
                        + ",vmm.支社長担当役員 as 支社長"
                        + " from SD_VT目標管理検索キー k "
                        + "inner join (select "
                        + "             t1.所属番号"
                        + "             ,t1.社員番号"
                        + "             ,t1.氏名"
                        + "             ,t1.フリガナ"
                        + "             ,t1.役職番号"
                        + "             ,LEFT(t1.所属番号,1) as 事業所番号"
                        + "             ,t1.資格番号"
                        + "            from SD_VT人事Data基本情報 t1) s"
                        + "       on k.社員番号 = s.社員番号"
                        + " left join (select max(大区分+中区分+小区分) as 設定,管理番号"
                        + "              from SD_T目標管理承認情報"
                        + "             where 大区分 = '1'"
                        + "               and 承認社員番号 is not null"
                        + "             group by 管理番号) as mm "
                        + "        on mm.管理番号 = k.管理番号 "
                        + " left join (select max(大区分+中区分+小区分) as 達成,管理番号"
                        + "              from SD_T目標管理承認情報"
                        + "             where 大区分 = '2'"
                        + "               and 承認社員番号 is not null"
                        + "             group by 管理番号) as tm "
                        + "        on tm.管理番号 = k.管理番号"
                        + " left join ( select 内容, キーID from SD_Mコード where コードタイプ ='OBJ_SIGN') mmm"
                        + "        on mmm.キーID = mm.設定"
                        + " left join ( select 内容, キーID from SD_Mコード where コードタイプ ='OBJ_SIGN') tmm"
                        + "        on tmm.キーID = tm.達成"

                        //履歴テーブル情報追加
                        + " left join (select max(大区分+中区分+小区分) as 設定,管理番号"
                        + "              from SD_T目標管理承認情報履歴"
                        + "             where 大区分 = '1'"
                        + "               and 承認社員番号 is not null"
                        + "             group by 管理番号) as mmr "
                        + "        on mmr.管理番号 = k.管理番号 "
                        + " left join (select max(大区分+中区分+小区分) as 達成,管理番号"
                        + "              from SD_T目標管理承認情報履歴"
                        + "             where 大区分 = '2'"
                        + "               and 承認社員番号 is not null"
                        + "             group by 管理番号) as tmr "
                        + "        on tmr.管理番号 = k.管理番号"
                        + " left join ( select 内容, キーID from SD_Mコード where コードタイプ ='OBJ_SIGN') mmmr"
                        + "        on mmmr.キーID = mmr.設定"
                        + " left join ( select 内容, キーID from SD_Mコード where コードタイプ ='OBJ_SIGN') tmmr"
                        + "        on tmmr.キーID = tmr.達成"

                        + " inner join SD_VM目標管理決裁権限 vmm"
                        + "        on k.年度 = vmm.年度"
                        + "       and k.所属番号 = vmm.所属番号"
                        + "       and k.社員番号 = vmm.社員番号"
                        + " {0}"
                        + ") sch";

                //条件作成
                Func<string, string, string> condition = (format, value) => {
                    return string.IsNullOrEmpty(value) ? "" :
                        value.Trim() == "" ? "" : string.Format(format, value.Trim());
                };
                var sqlc = "where 1=1";
                //年度
                sqlc += " and k.TBL区分 ='G'";
                //決裁権限
                sqlc += string.Format(" and (vmm.目標面談者 ='{0}'"
                                    + " or vmm.目標面談上位者 ='{0}'"
                                    + " or vmm.目標面談部長支店長 ='{0}'"
                                    + " or vmm.達成評価者 ='{0}'"
                                    + " or vmm.達成評価上位者 ='{0}'"
                                    + " or vmm.達成評価部長支店長 ='{0}'"
                                    + " or vmm.人事担当課長 ='{0}'"
                                    + " or vmm.総務部長 ='{0}'"
                                    + " or vmm.支社長担当役員 ='{0}')", lu.UserCode);

                sql = string.Format(sql, sqlc);
                //1000件まで
                sql += string.Format(" where レコード番号 between 1 and {0}", string.IsNullOrEmpty(limit) ? "1000" : limit);

                using(DbManager dm = new DbManager())
                using(IDbCommand cmd = dm.CreateCommand(sql))
                using(DataSet ds = new DataSet()) {
                    IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                    // データセットに設定する
                    da.Fill(ds);

                    List<YearEndAdjustmentSearchListModel> resultList = new List<YearEndAdjustmentSearchListModel>();
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        YearEndAdjustmentSearchListModel result = new YearEndAdjustmentSearchListModel {
                            Selected = false,
                            ManageNo = row["管理番号"].ToString(),
                            Year = row["年度"].ToString(),
                            EmployeeNumber = row["社員番号"].ToString(),
                            EmployeeName = row["氏名"].ToString(),
                            EmployeeNameKana = KanaEx.ToZenkakuKana(row["フリガナ"].ToString()),
                            Department = row["所属番号"].ToString(),
                            ObjectivesAceept = row["設定"].ToString(),
                            AattainmentAccept = row["達成"].ToString(),
                            ViewLink = row["管理番号"].ToString(),
                            TableType = row["TBL区分"].ToString()
                        };
                        resultList.Add(result);
                    }
                    model.SearchResult = resultList;
                }
                return model;
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
        /// 承認
        /// </summary>
        /// <param name="manageNos">管理番号配列</param>
        /// <param name="lu">ログインユーザー</param>
        /// <returns>承認件数</returns>
        //2016-01-21 iwai-tamura upd-str ------
        //承認した件数を戻すよう変更
        public int Sign(string[] manageNos, LoginUser lu) {
        //public void Sign(string[] manageNos, LoginUser lu) {
        //2016-01-21 iwai-tamura upd-end ------
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //管理番号抽出
                IList<string> mnos = new List<string>();
                foreach(string mno in manageNos) {
                    var sp = mno.Split(',');
                    mnos.Add(sp[0]);
                }

                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);

                //2016-01-21 iwai-tamura add-str ------
                int retCount = 0;   //承認した件数カウント用
                //2016-01-21 iwai-tamura add-end ------

                using(var scope = new TransactionScope()) {
                    //目標承認取得用
                    var sql = "select"
                            + " 管理番号"
                            + ",社員番号"
                            + ",達成部長"
                            + ",達成支社"
                            + ",人事"
                            + "  from ("
                            + "  select"
                            + "   管理番号"
                            + "  ,社員番号"
                            + "  ,isnull((select 承認社員番号 as 達成部長"
                            + "             from SD_T目標管理承認情報 msb"
                            + "            where 大区分 = '2' and 中区分 ='1' and  小区分 ='3'"
                            + "              and ms.管理番号 = msb.管理番号),'') as 達成部長"
                            + "  ,isnull((select 承認社員番号 as 達成支社"
                            + "             from SD_T目標管理承認情報 mss"
                            + "            where 大区分 = '2' and 中区分 ='1' and  小区分 ='4'"
                            + "              and ms.管理番号 = mss.管理番号),'') as 達成支社"
                            + "  ,isnull((select 承認社員番号 as 人事"
                            + "             from SD_T目標管理承認情報 mss"
                            + "            where 大区分 = '2' and 中区分 ='2' and  小区分 ='1'"
                            + "              and ms.管理番号 = mss.管理番号),'') as 人事"
                            + "    from SD_T目標管理承認情報 ms"
                            + "   group by  管理番号,社員番号) bluk"
                            + "   where (達成部長 != '' or 達成支社 !='')"
                            + "     and 人事 = ''";
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql))
                    using(DataSet ds = new DataSet()) {
                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        // データセットに設定する
                        da.Fill(ds);

                        //目標管理承認
                        sql = "update SD_T目標管理承認情報"
                                   + " set "
                                   + " 承認社員番号 = @SignEmployeeNo"
                                   + ",承認日付 = @SignDate"
                                   + " where 管理番号 = @ManageNo"
                                   + "   and 大区分 = '2'"
                                   + "   and 中区分 = '2'"
                                   + "   and 小区分 = '1'";
                        //SQL文の型を指定
                        IDbCommand ucmd = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(ucmd, "@SignEmployeeNo", DbType.String);
                        DbHelper.AddDbParameter(ucmd, "@SignDate", DbType.DateTime);
                        DbHelper.AddDbParameter(ucmd, "@ManageNo", DbType.String);
                        //パラメータ設定
                        var parameters = ucmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = DataConv.IfNull(lu.UserCode);
                        parameters[1].Value = DataConv.IfNull(DateTime.Now.ToString());
                        
                        //抽出した管理番号を更新
                        var query = from row in ds.Tables[0].AsEnumerable()
                                    where mnos.Contains(row.Field<Int32>("管理番号").ToString())
                                    select row.Field<Int32>("管理番号").ToString();
                        foreach(var manageNo in query) {
                            parameters[2].Value = DataConv.IfNull(manageNo);
                            ucmd.ExecuteNonQuery();
                            //2016-01-21 iwai-tamura add-str ------
                            //承認件数カウント
                            retCount++;
                            //2016-01-21 iwai-tamura add-upd ------
                        }
                    }

                    scope.Complete();
                    //2016-01-21 iwai-tamura add-str ------
                    //承認した件数を返す
                    return retCount;
                    //2016-01-21 iwai-tamura add-end ------
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
        /// 管理番号取得
        /// </summary>
        /// <param name="lu">ログイン情報</param>
        /// <returns>管理番号</returns>
        public int? GetManageNo(LoginUser lu) {
            var sql = "select 管理番号 "
                    + "  from SD_VT目標管理検索キー"
                    + " where 社員番号 = @EmployeeNo"
                    + "   and TBL区分 ='G'"
                    + " order by 年度 desc, 管理番号 desc";
            using(DbManager dm = new DbManager())
            using(IDbCommand cmd = dm.CreateCommand(sql))
            using(DataSet ds = new DataSet()) {
                DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                ((IDbDataParameter)cmd.Parameters[0]).Value = lu.UserCode;
                IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                da.Fill(ds);
                foreach(DataRow row in ds.Tables[0].Rows) {
                    return DataConv.IntParse(row["管理番号"].ToString(), null);
                }

            }
            return null;
        }
    }
}
