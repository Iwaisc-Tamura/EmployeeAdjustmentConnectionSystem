using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EmployeeAdjustmentConnectionSystem.BL.Common;
using EmployeeAdjustmentConnectionSystem.COM.Util.Database;
using EmployeeAdjustmentConnectionSystem.COM.Entity.Session;
using EmployeeAdjustmentConnectionSystem.COM.Enum;
using EmployeeAdjustmentConnectionSystem.COM.Models;
using System.Data.SqlClient;
using System.Configuration;
using EmployeeAdjustmentConnectionSystem.COM.Util.Config;
using EmployeeAdjustmentConnectionSystem.COM.Util.Convert;
using System.Web.Mvc;

namespace EmployeeAdjustmentConnectionSystem.BL.HaiguuDeclareRegister {
    /// <summary>
    /// 扶養控除申告書入力ビジネスロジック
    /// </summary>
    public class HaiguuDeclareRegisterBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public HaiguuDeclareRegisterBL() {
        }


        /// <summary>
        /// 扶養控除申告書データ取得
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <returns>扶養控除申告書モデル</returns>
        public HaiguuDeclareRegisterViewModels Select(int? intSheetYear,string strEmployeeNo,bool bolAdminMode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                HaiguuDeclareRegisterViewModels model = new HaiguuDeclareRegisterViewModels();
                model.Head = new HaiguuDeclareRegisterHeaderModel();
                model.Body = new HaiguuDeclareRegisterBodyModel();

                using(DbManager dm = new DbManager()) {
                    //年月日結合用
                    Func<string,DateTime?> changeDate = (val) => {
                        if(val == "") {
                            return null;
                        } else {
                            return DateTime.ParseExact(val, "yyyyMMdd", null);
                        }
                    };
                    
                    //金額セット用
                    Func<string, int?> setMoney = (val1) => {
                        if(!string.IsNullOrEmpty(val1)) {
                            float  i;
                            if (float.TryParse(val1,out i)){
                                return (int)i;
                            }
                        }
                        return null;
                    };

                    //2023-11-20 iwai-tamura upd str -----
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
						//2023-12-15 iwai-tamura upd str -----
                        } else if (value1 == "9" && value2 == "7") {
                            return "システム連携済み";
                        } else if (value1 == "9" && value2 == "8") {
                            return "システム連携後修正";
                        } else if (value1 == "9" && value2 == "9") {
                            return "確定済み";
                        //} else if (value1 == "9" && value2 == "9") {
                        //    return "システム連携済み";
						//2023-12-15 iwai-tamura upd end -----
                        } else {
                            return "システムエラー";
                        }
                    };
                    //2023-11-20 iwai-tamura upd end -----

                    //2023-11-20 iwai-tamura upd str -----
                    var sql = "SELECT T基礎.* ";
                        sql += " ,T扶養.源泉控除対象配偶者給与所得収入金額 ";
                        sql += " ,T扶養.源泉控除対象配偶者給与所得所得金額 ";
                        sql += " ,T扶養.源泉控除対象配偶者他所得金額 ";

                    //2025-99-99 iwai-tamura upd-str ------
                        // 特定扶養親族01
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族01_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族01_続柄";
                        sql += "      ELSE ''";
                        sql += "   END AS 特定扶養01_続柄 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族01_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族01_給与所得_収入金額 ";
                        sql += "      ELSE '' ";
                        sql += "   END AS 特定扶養01_給与所得_収入金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族01_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族01_給与所得_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += "   END AS 特定扶養01_給与所得_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族01_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族01_他_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += "   END AS 特定扶養01_他_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族01_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族01_所得見積額 ";
                        sql += "      ELSE '' ";
                        sql += "   END AS 特定扶養01_所得見積額 ";


                        // 特定扶養親族02
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族02_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族02_続柄";
                        sql += "      ELSE ''";
                        sql += " END AS 特定扶養02_続柄";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族02_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族02_給与所得_収入金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養02_給与所得_収入金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族02_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族02_給与所得_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養02_給与所得_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族02_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族02_他_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養02_他_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族02_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族02_所得見積額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養02_所得見積額 ";

                        // 特定扶養親族03
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族03_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族03_続柄";
                        sql += "      ELSE ''";
                        sql += " END AS 特定扶養03_続柄";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族03_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族03_給与所得_収入金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養03_給与所得_収入金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族03_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族03_給与所得_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養03_給与所得_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族03_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族03_他_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養03_他_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族03_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族03_所得見積額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養03_所得見積額 ";

                        // 特定扶養親族04
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族04_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族04_続柄";
                        sql += "      ELSE ''";
                        sql += " END AS 特定扶養04_続柄";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族04_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族04_給与所得_収入金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養04_給与所得_収入金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族04_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族04_給与所得_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養04_給与所得_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族04_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族04_他_所得金額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養04_他_所得金額 ";
                        sql += " ,CASE WHEN T扶養.控除対象扶養親族04_特定扶養親族区分 = '2' ";
                        sql += "      THEN T扶養.控除対象扶養親族04_所得見積額 ";
                        sql += "      ELSE '' ";
                        sql += " END AS 特定扶養04_所得見積額 ";

                    //2025-99-99 iwai-tamura upd-end ------

                        sql += " FROM TE120基礎控除申告書Data As T基礎  ";
                        sql += "   LEFT JOIN TE100扶養控除申告書Data As T扶養 ";
                        sql += "     ON T基礎.対象年度 = T扶養.対象年度 AND T基礎.社員番号 = T扶養.社員番号 ";
                        sql += " WHERE T基礎.対象年度 = @SheetYear and T基礎.社員番号 = @EmployeeNo ";
                    //var sql = "SELECT * FROM TE120基礎控除申告書Data WHERE 対象年度 = @SheetYear and 社員番号 = @EmployeeNo ";
                    //2023-11-20 iwai-tamura upd end -----

                    using(IDbCommand cmd = dm.CreateCommand(sql))
                    using(DataSet ds = new DataSet()) {
                        DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                        ((IDbDataParameter)cmd.Parameters[0]).Value = intSheetYear;
                        ((IDbDataParameter)cmd.Parameters[1]).Value = strEmployeeNo;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);
                        var rows = ds.Tables[0].Rows;

                        foreach(DataRow row in rows) {
                            model.Head = new HaiguuDeclareRegisterHeaderModel {
								SheetYear = DataConv.IntParse(row["対象年度"].ToString()),
								ApprovalType = row["本人確定区分"].ToString(),
								DecisionType = row["管理者確定区分"].ToString(),
                                //2023-11-20 iwai-tamura upd str -----
                                StatusName = StatusDecision(row["本人確定区分"].ToString(),row["管理者確定区分"].ToString()),
                                //2023-11-20 iwai-tamura upd end -----
								MyNumberCheck = row["個人番号相違確認区分"].ToString(),
								EmployeeNo = row["社員番号"].ToString(),
								DepartmentNo = DataConv.IntParse(row["所属番号"].ToString()),
								Name1 = row["氏名_姓"].ToString(),
								Name2 = row["氏名_名"].ToString(),
								Kana1 = row["Kana_姓"].ToString(),
								Kana2 = row["Kana_名"].ToString(),
								Address = row["住所01"].ToString(),
								BasicDeduction_Earnings = setMoney(row["基礎控除申告書_給与所得_収入金額"].ToString()),
								BasicDeduction_Income = setMoney(row["基礎控除申告書_給与所得_所得金額"].ToString()),
								BasicDeduction_OtherIncome = setMoney(row["基礎控除申告書_他_所得金額"].ToString()),
								BasicDeduction_TotalEarnings = setMoney(row["基礎控除申告書_合計所得金額見積額"].ToString()),
								BasicDeduction_EarningsType = row["基礎控除申告書_控除額計算判定"].ToString(),

                                //2024-11-19 iwai-tamura upd-str ------
								BasicDeduction_TaxReductionTarget = row["基礎控除申告書_本人定額減税対象"].ToString(),
                                //2024-11-19 iwai-tamura upd-end ------

								BasicDeduction_CalcType = row["基礎控除申告書_控除額計算区分"].ToString(),
								BasicDeduction_DeductionAmount = setMoney(row["基礎控除申告書_基礎控除額"].ToString()),
								SpouseDeduction_Name1 = row["配偶者控除申告書_氏名_姓"].ToString(),
								SpouseDeduction_Name2 = row["配偶者控除申告書_氏名_名"].ToString(),
								SpouseDeduction_Kana1 = row["配偶者控除申告書_Kana_姓"].ToString(),
								SpouseDeduction_Kana2 = row["配偶者控除申告書_Kana_名"].ToString(),
								SpouseDeduction_RelationshipType = row["配偶者控除申告書_続柄"].ToString(),
								SpouseDeduction_Birthday = row["配偶者控除申告書_生年月日"].ToString(),
                                SpouseDeduction_BirthdayYear = row["配偶者控除申告書_生年月日"].ToString()=="" ? "":row["配偶者控除申告書_生年月日"].ToString().Substring(0,4),
                                SpouseDeduction_BirthdayMonth = row["配偶者控除申告書_生年月日"].ToString()=="" ? "":row["配偶者控除申告書_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                SpouseDeduction_BirthdayDay = row["配偶者控除申告書_生年月日"].ToString()=="" ? "":row["配偶者控除申告書_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

                                SpouseDeduction_ResidentCheck = row["配偶者控除申告書_非居住者"].ToString(),
								SpouseDeduction_Address = row["配偶者控除申告書_住所"].ToString(),
								SpouseDeduction_Earnings = setMoney(row["配偶者控除申告書_給与所得_収入金額"].ToString()),
								SpouseDeduction_Income = setMoney(row["配偶者控除申告書_給与所得_所得金額"].ToString()),
								SpouseDeduction_OtherIncome = setMoney(row["配偶者控除申告書_他_所得金額"].ToString()),

                                //2023-11-20 iwai-tamura upd str -----
								SpouseDeduction_Huyou_Earnings = setMoney(row["源泉控除対象配偶者給与所得収入金額"].ToString()),
								SpouseDeduction_Huyou_Income = setMoney(row["源泉控除対象配偶者給与所得所得金額"].ToString()),
								SpouseDeduction_Huyou_OtherIncome = setMoney(row["源泉控除対象配偶者他所得金額"].ToString()),
                                //2023-11-20 iwai-tamura upd end -----

								SpouseDeduction_TotalEarnings = setMoney(row["配偶者控除申告書_合計所得金額見積額"].ToString()),
								SpouseDeduction_EarningsType = row["配偶者控除申告書_控除額計算判定"].ToString(),
								SpouseDeduction_CalcType = row["配偶者控除申告書_控除額計算区分"].ToString(),
								SpouseDeduction_DeductionAmount = setMoney(row["配偶者控除申告書_配偶者控除額"].ToString()),
								SpouseDeduction_SpecialDeductionAmount = setMoney(row["配偶者控除申告書_配偶者特別控除額"].ToString()),

                                //2024-11-19 iwai-tamura upd-str ------
								SpouseDeduction_TaxReductionTarget = row["配偶者控除申告書_配偶者定額減税対象"].ToString(),
                                //2024-11-19 iwai-tamura upd-end ------

								AdjustmentDeduction_ConditionType = row["所得金額調整控除申告書_要件区分"].ToString(),
								AdjustmentDeduction_Name1 = row["所得金額調整控除申告書_扶養親族等氏名_姓"].ToString(),
								AdjustmentDeduction_Name2 = row["所得金額調整控除申告書_扶養親族等氏名_名"].ToString(),
								AdjustmentDeduction_Kana1 = row["所得金額調整控除申告書_扶養親族等Kana_姓"].ToString(),
								AdjustmentDeduction_Kana2 = row["所得金額調整控除申告書_扶養親族等Kana_名"].ToString(),
								AdjustmentDeduction_Birthday = row["所得金額調整控除申告書_扶養親族等生年月日"].ToString(),
                                AdjustmentDeduction_BirthdayYear = row["所得金額調整控除申告書_扶養親族等生年月日"].ToString()=="" ? "":row["所得金額調整控除申告書_扶養親族等生年月日"].ToString().Substring(0,4),
                                AdjustmentDeduction_BirthdayMonth = row["所得金額調整控除申告書_扶養親族等生年月日"].ToString()=="" ? "":row["所得金額調整控除申告書_扶養親族等生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                AdjustmentDeduction_BirthdayDay = row["所得金額調整控除申告書_扶養親族等生年月日"].ToString()=="" ? "":row["所得金額調整控除申告書_扶養親族等生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

								AdjustmentDeduction_ResidentCheck = row["所得金額調整控除申告書_扶養親族等同上区分"].ToString(),
								AdjustmentDeduction_Address = row["所得金額調整控除申告書_扶養親族等住所"].ToString(),
								AdjustmentDeduction_RelationshipType = row["所得金額調整控除申告書_扶養親族等続柄"].ToString(),
								AdjustmentDeduction_TotalEarnings = setMoney(row["所得金額調整控除申告書_扶養親族等所得金額"].ToString()),
								AdjustmentDeduction_ReportType = row["所得金額調整控除申告書_特別障害者該当事実"].ToString(),

                                //2025-99-99 iwai-tamura upd-str ------
                                // ===== 特定親族特別控除申告書 01 =====
                                SpecificRelativeSpecialDeduction01_Name1 = row["特定親族特別控除申告書01_氏名_姓"].ToString(),
                                SpecificRelativeSpecialDeduction01_Name2 = row["特定親族特別控除申告書01_氏名_名"].ToString(),
                                SpecificRelativeSpecialDeduction01_Kana1 = row["特定親族特別控除申告書01_Kana_姓"].ToString(),
                                SpecificRelativeSpecialDeduction01_Kana2 = row["特定親族特別控除申告書01_Kana_名"].ToString(),
                                SpecificRelativeSpecialDeduction01_RelationshipType = row["特定親族特別控除申告書01_続柄"].ToString(),
                                SpecificRelativeSpecialDeduction01_Birthday = row["特定親族特別控除申告書01_生年月日"].ToString(),
                                SpecificRelativeSpecialDeduction01_BirthdayYear  = row["特定親族特別控除申告書01_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書01_生年月日"].ToString().Substring(0,4),
                                SpecificRelativeSpecialDeduction01_BirthdayMonth = row["特定親族特別控除申告書01_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書01_生年月日"].ToString().Substring(4,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction01_BirthdayDay   = row["特定親族特別控除申告書01_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書01_生年月日"].ToString().Substring(6,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction01_Address = row["特定親族特別控除申告書01_住所"].ToString(),
                                SpecificRelativeSpecialDeduction01_ResidentCheck = row["特定親族特別控除申告書01_非居住者"].ToString(),
                                SpecificRelativeSpecialDeduction01_Earnings = setMoney(row["特定親族特別控除申告書01_給与所得_収入金額"].ToString()),
                                SpecificRelativeSpecialDeduction01_Income = setMoney(row["特定親族特別控除申告書01_給与所得_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction01_OtherIncome = setMoney(row["特定親族特別控除申告書01_他_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction01_TotalEarnings = setMoney(row["特定親族特別控除申告書01_合計所得金額見積額"].ToString()),
                                SpecificRelativeSpecialDeduction01_EarningsType = row["特定親族特別控除申告書01_控除額計算判定"].ToString(),
                                SpecificRelativeSpecialDeduction01_CalcType = row["特定親族特別控除申告書01_控除額計算区分"].ToString(),
                                SpecificRelativeSpecialDeduction01_SpecialDeductionAmount = setMoney(row["特定親族特別控除申告書01_特定親族特別控除額"].ToString()),

                                // ===== 特定親族特別控除申告書 02 =====
                                SpecificRelativeSpecialDeduction02_Name1 = row["特定親族特別控除申告書02_氏名_姓"].ToString(),
                                SpecificRelativeSpecialDeduction02_Name2 = row["特定親族特別控除申告書02_氏名_名"].ToString(),
                                SpecificRelativeSpecialDeduction02_Kana1 = row["特定親族特別控除申告書02_Kana_姓"].ToString(),
                                SpecificRelativeSpecialDeduction02_Kana2 = row["特定親族特別控除申告書02_Kana_名"].ToString(),
                                SpecificRelativeSpecialDeduction02_RelationshipType = row["特定親族特別控除申告書02_続柄"].ToString(),
                                SpecificRelativeSpecialDeduction02_Birthday = row["特定親族特別控除申告書02_生年月日"].ToString(),
                                SpecificRelativeSpecialDeduction02_BirthdayYear  = row["特定親族特別控除申告書02_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書02_生年月日"].ToString().Substring(0,4),
                                SpecificRelativeSpecialDeduction02_BirthdayMonth = row["特定親族特別控除申告書02_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書02_生年月日"].ToString().Substring(4,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction02_BirthdayDay   = row["特定親族特別控除申告書02_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書02_生年月日"].ToString().Substring(6,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction02_Address = row["特定親族特別控除申告書02_住所"].ToString(),
                                SpecificRelativeSpecialDeduction02_ResidentCheck = row["特定親族特別控除申告書02_非居住者"].ToString(),
                                SpecificRelativeSpecialDeduction02_Earnings = setMoney(row["特定親族特別控除申告書02_給与所得_収入金額"].ToString()),
                                SpecificRelativeSpecialDeduction02_Income = setMoney(row["特定親族特別控除申告書02_給与所得_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction02_OtherIncome = setMoney(row["特定親族特別控除申告書02_他_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction02_TotalEarnings = setMoney(row["特定親族特別控除申告書02_合計所得金額見積額"].ToString()),
                                SpecificRelativeSpecialDeduction02_EarningsType = row["特定親族特別控除申告書02_控除額計算判定"].ToString(),
                                SpecificRelativeSpecialDeduction02_CalcType = row["特定親族特別控除申告書02_控除額計算区分"].ToString(),
                                SpecificRelativeSpecialDeduction02_SpecialDeductionAmount = setMoney(row["特定親族特別控除申告書02_特定親族特別控除額"].ToString()),

                                // ===== 特定親族特別控除申告書 03 =====
                                SpecificRelativeSpecialDeduction03_Name1 = row["特定親族特別控除申告書03_氏名_姓"].ToString(),
                                SpecificRelativeSpecialDeduction03_Name2 = row["特定親族特別控除申告書03_氏名_名"].ToString(),
                                SpecificRelativeSpecialDeduction03_Kana1 = row["特定親族特別控除申告書03_Kana_姓"].ToString(),
                                SpecificRelativeSpecialDeduction03_Kana2 = row["特定親族特別控除申告書03_Kana_名"].ToString(),
                                SpecificRelativeSpecialDeduction03_RelationshipType = row["特定親族特別控除申告書03_続柄"].ToString(),
                                SpecificRelativeSpecialDeduction03_Birthday = row["特定親族特別控除申告書03_生年月日"].ToString(),
                                SpecificRelativeSpecialDeduction03_BirthdayYear  = row["特定親族特別控除申告書03_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書03_生年月日"].ToString().Substring(0,4),
                                SpecificRelativeSpecialDeduction03_BirthdayMonth = row["特定親族特別控除申告書03_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書03_生年月日"].ToString().Substring(4,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction03_BirthdayDay   = row["特定親族特別控除申告書03_生年月日"].ToString()=="" ? "" : row["特定親族特別控除申告書03_生年月日"].ToString().Substring(6,2).TrimStart(new Char[]{'0'}),
                                SpecificRelativeSpecialDeduction03_Address = row["特定親族特別控除申告書03_住所"].ToString(),
                                SpecificRelativeSpecialDeduction03_ResidentCheck = row["特定親族特別控除申告書03_非居住者"].ToString(),
                                SpecificRelativeSpecialDeduction03_Earnings = setMoney(row["特定親族特別控除申告書03_給与所得_収入金額"].ToString()),
                                SpecificRelativeSpecialDeduction03_Income = setMoney(row["特定親族特別控除申告書03_給与所得_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction03_OtherIncome = setMoney(row["特定親族特別控除申告書03_他_所得金額"].ToString()),
                                SpecificRelativeSpecialDeduction03_TotalEarnings = setMoney(row["特定親族特別控除申告書03_合計所得金額見積額"].ToString()),
                                SpecificRelativeSpecialDeduction03_EarningsType = row["特定親族特別控除申告書03_控除額計算判定"].ToString(),
                                SpecificRelativeSpecialDeduction03_CalcType = row["特定親族特別控除申告書03_控除額計算区分"].ToString(),
                                SpecificRelativeSpecialDeduction03_SpecialDeductionAmount = setMoney(row["特定親族特別控除申告書03_特定親族特別控除額"].ToString()),

                                // ===== 参考用：扶養控除申告書データ =====
                                // ===== 特定扶養 01 =====
                                RefSpecificDependent01_RelationshipType = row["特定扶養01_続柄"].ToString(),
                                RefSpecificDependent01_Earnings = setMoney(row["特定扶養01_給与所得_収入金額"].ToString()),
                                RefSpecificDependent01_Income = setMoney(row["特定扶養01_給与所得_所得金額"].ToString()),
                                RefSpecificDependent01_OtherIncome = setMoney(row["特定扶養01_他_所得金額"].ToString()),
                                RefSpecificDependent01_TotalEarnings = setMoney(row["特定扶養01_所得見積額"].ToString()),
                                // ===== 特定扶養 02 =====
                                RefSpecificDependent02_RelationshipType = row["特定扶養02_続柄"].ToString(),
                                RefSpecificDependent02_Earnings = setMoney(row["特定扶養02_給与所得_収入金額"].ToString()),
                                RefSpecificDependent02_Income = setMoney(row["特定扶養02_給与所得_所得金額"].ToString()),
                                RefSpecificDependent02_OtherIncome = setMoney(row["特定扶養02_他_所得金額"].ToString()),
                                RefSpecificDependent02_TotalEarnings = setMoney(row["特定扶養02_所得見積額"].ToString()),
                                // ===== 特定扶養 03 =====
                                RefSpecificDependent03_RelationshipType = row["特定扶養03_続柄"].ToString(),
                                RefSpecificDependent03_Earnings = setMoney(row["特定扶養03_給与所得_収入金額"].ToString()),
                                RefSpecificDependent03_Income = setMoney(row["特定扶養03_給与所得_所得金額"].ToString()),
                                RefSpecificDependent03_OtherIncome = setMoney(row["特定扶養03_他_所得金額"].ToString()),
                                RefSpecificDependent03_TotalEarnings = setMoney(row["特定扶養03_所得見積額"].ToString()),
                                // ===== 特定扶養 04 =====
                                RefSpecificDependent04_RelationshipType = row["特定扶養04_続柄"].ToString(),
                                RefSpecificDependent04_Earnings = setMoney(row["特定扶養04_給与所得_収入金額"].ToString()),
                                RefSpecificDependent04_Income = setMoney(row["特定扶養04_給与所得_所得金額"].ToString()),
                                RefSpecificDependent04_OtherIncome = setMoney(row["特定扶養04_他_所得金額"].ToString()),
                                RefSpecificDependent04_TotalEarnings = setMoney(row["特定扶養04_所得見積額"].ToString())
                                //2025-99-99 iwai-tamura upd-end ------

                            };
                            model.Head.InputMode = ajustMode.SelfInput;
                            model.Head.AdminMode = bolAdminMode;
                        }
                    }
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
        /// 画面モード設定
        /// </summary>
        /// <param name="model">モデル</param>
        /// <param name="lu">ログインユーザー</param>
        public void SetMode(HaiguuDeclareRegisterViewModels model, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //画面の状態初期設定
                model.Head.InputMode = ajustMode.adminConfim;
                model.Head.CancelButton = ajustMode.ReadOnly;
                model.Head.AuthButton = ajustMode.ReadOnly;

                //確定データは変更不可
                if (model.Head.DecisionType == "9"){
                    return;
                }

                if (model.Head.AdminMode) {
                    //管理モード
                    if (model.Head.ApprovalType == "0"){
                        //本人が提出前は変更不可
                        model.Head.InputMode = ajustMode.adminConfim;
                    } else {
                        switch(model.Head.DecisionType){
                            case "0":
                                //管理者入力
                                model.Head.InputMode = ajustMode.adminInput;
                                break;

                            //2023-11-20 iwai-tamura upd str -----
                            case "1":
								switch (lu.IsAdminNo) {
									case "2":
									case "3":
				                        model.Head.InputMode = ajustMode.adminRegist;
										break;
									case "1":
									case "7":
									case "8":
									case "9":
									case "K":
		                                model.Head.InputMode = ajustMode.adminInput;
										break;
								}
                                break;

                            case "5":
                                //管理者登録済み
								switch (lu.IsAdminNo) {
									case "2":
									case "3":
		                                model.Head.InputMode = ajustMode.adminConfim;
										break;
									case "1":
									case "7":
									case "8":
									case "9":
									case "K":
		                                model.Head.InputMode = ajustMode.adminRegist;
										break;
								}
                                break;
                            //case "1":
                            //    //管理者登録済み
                            //    model.Head.InputMode = ajustMode.adminRegist;
                            //    break;
                            //2023-11-20 iwai-tamura upd end -----

							//2023-12-15 iwai-tamura add str -----
                            case "7":
                            case "8":
                                //システム連携済み・システム連携後修正
								switch (lu.IsAdminNo) {
									case "2":
									case "3":
		                                model.Head.InputMode = ajustMode.adminConfim;
										break;
									case "1":
									case "7":
									case "8":
									case "9":
									case "K":
		                                model.Head.InputMode = ajustMode.adminInput;
										break;
								}
                                break;
							//2023-12-15 iwai-tamura add end -----
                            case "9":
                                //管理者確定済み
                                model.Head.InputMode = ajustMode.adminConfim;
                                break;
                        }
                    }
                }else{
                    if (model.Head.DecisionType == "0"){
                        //管理者区分が0のときのみ本人入力可能
                        switch(model.Head.ApprovalType){
                            case "0":
                                //本人入力可能
                                model.Head.InputMode = ajustMode.SelfInput;
                                break;

                            case "1":
                                //本人提出済み
                                model.Head.InputMode = ajustMode.SelfRegist;
                                break;

                            case "9":
                                //本社受理済み
                                model.Head.InputMode = ajustMode.SelfConfim;
                                break;
                        }
                    } else {
                        //本社受理済み
                        model.Head.InputMode = ajustMode.SelfConfim;
                    }
                }

                return;
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
        /// 保存
        /// </summary>
        /// <param name="model">扶養控除申告書モデル</param>
        /// <param name="model">登録モード(1:途中保存/2:承認登録)</param>
        public void Save(HaiguuDeclareRegisterViewModels model,LoginUser lu,string mode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                using(var scope = new TransactionScope()) {
                    using(var dm = new DbManager()) {
                        //年月日結合用
                        Func<string, string, string, string> addYMD = (val1, val2, val3) => {
                            if(!string.IsNullOrEmpty(val1) && !string.IsNullOrEmpty(val2) && !string.IsNullOrEmpty(val3)) {
                                string strYMD = val1+val2.PadLeft(2, '0')+val3.PadLeft(2, '0');
                                return strYMD == "" ? null : strYMD.ToString();
                            }
                            return null;
                        };

                        //評価点計算用
                        Func<string, string, decimal?> calc = (val1, val2) => {
                            if(!string.IsNullOrEmpty(val1) && !string.IsNullOrEmpty(val2)) {
                                decimal? deci = DataConv.Str2Deci(val1, 0) * DataConv.Str2Deci(val2, 0);
                                return deci == 0 ? null : deci;
                            }
                            return null;
                        };

                        //number2string
                        Func<string, string> n2s = (val1) => {
                            if(!string.IsNullOrEmpty(val1)) {
                                decimal? deci = DataConv.Str2Deci(val1, 0);
                                return deci == 0 ? null : deci.ToString();
                            }
                            return null;
                        };

                        //リストボックス、値と内容の分割
                        Func<string, string> splitPre = (val1) => {
                            if(!string.IsNullOrEmpty(val1)) {
                                if (val1.Trim()=="") {
                                    return null;
                                }
                                string strSplit = ((val1 == null)? null: val1.Substring(0,val1.IndexOf(":")));
                                return strSplit == "" ? null : strSplit.ToString();
                            }
                            return null;
                        };
                        Func<string, string> splitSuf = (val1) => {
                            if(!string.IsNullOrEmpty(val1)) {
                                if (val1.Trim()=="") {
                                    return null;
                                }
                                string strSplit = ((val1 == null)? null: val1.Substring(val1.IndexOf(":")+1));
                                return strSplit == "" ? null : strSplit.ToString();
                            }
                            return null;
                        };

                        //チェックボックス
                        Func<string, string> checkValue = (val1) => {
                            if(!string.IsNullOrEmpty(val1)) {
                                if(val1=="0" || val1=="1"){
                                    return val1;
                                }
                                return (val1 == "true")? "1": "0";
                            }
                            return null;
                        };


                        //承認状況 更新内容
                        var strApproval = model.Head.ApprovalType;
                        var strDecision = model.Head.DecisionType;
                        if (mode=="2") {    //承認登録時のみ更新
                            switch (model.Head.InputMode) {
                                case ajustMode.SelfInput:
                                    strApproval = "1";
                                    break;
                                case ajustMode.adminInput:
                                    strApproval = "9";
                                    //2023-11-20 iwai-tamura upd str -----
									switch (lu.IsAdminNo) {
										case "2":
										case "3":
											//2023-12-15 iwai-tamura upd str -----
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
												strDecision = "1";
											}
											//strDecision = "1";
											//2023-12-15 iwai-tamura upd end -----
											break;
										case "1":
										case "7":
										case "8":
										case "9":
											//2023-12-15 iwai-tamura upd str -----
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
			                                    strDecision = "5";
											}
		                                    //strDecision = "5";
											//2023-12-15 iwai-tamura upd end -----
											break;
										case "K":
											//2023-12-15 iwai-tamura upd str -----
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
			                                    strDecision = "5";
											}
		                                    //strDecision = "5";
											//2023-12-15 iwai-tamura upd end -----
											break;
									}
                                    //strDecision = "1";
                                    //2023-11-20 iwai-tamura upd end -----
                                    break;
                            }
                        }

                        //共通基本情報
                        var sql = "update TE120基礎控除申告書Data"
                            + " set "
							+ " 本人確定区分 = @ApprovalType"
							+ " ,管理者確定区分 = @DecisionType"
							+ " ,個人番号相違確認区分 = @MyNumberCheck"
							+ " ,所属番号 = @DepartmentNo"
							+ " ,氏名_姓 = @Name1"
							+ " ,氏名_名 = @Name2"
							+ " ,Kana_姓 = @Kana1"
							+ " ,Kana_名 = @Kana2"
							+ " ,住所01 = @Address"
							+ " ,基礎控除申告書_給与所得_収入金額 = @BasicDeduction_Earnings"
							+ " ,基礎控除申告書_給与所得_所得金額 = @BasicDeduction_Income"
							+ " ,基礎控除申告書_他_所得金額 = @BasicDeduction_OtherIncome"
							+ " ,基礎控除申告書_合計所得金額見積額 = @BasicDeduction_TotalEarnings"
							+ " ,基礎控除申告書_控除額計算判定 = @BasicDeduction_EarningsType"
                            //2024-11-19 iwai-tamura upd-str ------
							+ " ,基礎控除申告書_本人定額減税対象 = @BasicDeduction_TaxReductionTarget"
                            //2024-11-19 iwai-tamura upd-end ------
							+ " ,基礎控除申告書_控除額計算区分 = @BasicDeduction_CalcType"
							+ " ,基礎控除申告書_基礎控除額 = @BasicDeduction_DeductionAmount"
							+ " ,配偶者控除申告書_氏名_姓 = @SpouseDeduction_Name1"
							+ " ,配偶者控除申告書_氏名_名 = @SpouseDeduction_Name2"
							+ " ,配偶者控除申告書_Kana_姓 = @SpouseDeduction_Kana1"
							+ " ,配偶者控除申告書_Kana_名 = @SpouseDeduction_Kana2"
							+ " ,配偶者控除申告書_続柄 = @SpouseDeduction_RelationshipType"
							+ " ,配偶者控除申告書_生年月日 = @SpouseDeduction_Birthday"
							+ " ,配偶者控除申告書_非居住者 = @SpouseDeduction_ResidentCheck"
							+ " ,配偶者控除申告書_住所 = @SpouseDeduction_Address"
							+ " ,配偶者控除申告書_給与所得_収入金額 = @SpouseDeduction_Earnings"
							+ " ,配偶者控除申告書_給与所得_所得金額 = @SpouseDeduction_Income"
							+ " ,配偶者控除申告書_他_所得金額 = @SpouseDeduction_OtherIncome"
							+ " ,配偶者控除申告書_合計所得金額見積額 = @SpouseDeduction_TotalEarnings"
							+ " ,配偶者控除申告書_控除額計算判定 = @SpouseDeduction_EarningsType"
							+ " ,配偶者控除申告書_控除額計算区分 = @SpouseDeduction_CalcType"
							+ " ,配偶者控除申告書_配偶者控除額 = @SpouseDeduction_DeductionAmount"
							+ " ,配偶者控除申告書_配偶者特別控除額 = @SpouseDeduction_SpecialDeductionAmount"
                            //2024-11-19 iwai-tamura upd-str ------
							+ " ,配偶者控除申告書_配偶者定額減税対象 = @SpouseDeduction_TaxReductionTarget"
                            //2024-11-19 iwai-tamura upd-end ------
							+ " ,所得金額調整控除申告書_要件区分 = @AdjustmentDeduction_ConditionType"
							+ " ,所得金額調整控除申告書_扶養親族等氏名_姓 = @AdjustmentDeduction_Name1"
							+ " ,所得金額調整控除申告書_扶養親族等氏名_名 = @AdjustmentDeduction_Name2"
							+ " ,所得金額調整控除申告書_扶養親族等Kana_姓 = @AdjustmentDeduction_Kana1"
							+ " ,所得金額調整控除申告書_扶養親族等Kana_名 = @AdjustmentDeduction_Kana2"
							+ " ,所得金額調整控除申告書_扶養親族等生年月日 = @AdjustmentDeduction_Birthday"
							+ " ,所得金額調整控除申告書_扶養親族等同上区分 = @AdjustmentDeduction_ResidentCheck"
							+ " ,所得金額調整控除申告書_扶養親族等住所 = @AdjustmentDeduction_Address"
							+ " ,所得金額調整控除申告書_扶養親族等続柄 = @AdjustmentDeduction_RelationshipType"
							+ " ,所得金額調整控除申告書_扶養親族等所得金額 = @AdjustmentDeduction_TotalEarnings"
							+ " ,所得金額調整控除申告書_特別障害者該当事実 = @AdjustmentDeduction_ReportType"

                            //2025-99-99 iwai-tamura upd-str ------
                            + " ,特定親族特別控除申告書01_氏名_姓 = @SpecificRelativeSpecialDeduction01_Name1"
                            + " ,特定親族特別控除申告書01_氏名_名 = @SpecificRelativeSpecialDeduction01_Name2"
                            + " ,特定親族特別控除申告書01_Kana_姓 = @SpecificRelativeSpecialDeduction01_Kana1"
                            + " ,特定親族特別控除申告書01_Kana_名 = @SpecificRelativeSpecialDeduction01_Kana2"
                            + " ,特定親族特別控除申告書01_続柄 = @SpecificRelativeSpecialDeduction01_RelationshipType"
                            + " ,特定親族特別控除申告書01_生年月日 = @SpecificRelativeSpecialDeduction01_Birthday"
                            + " ,特定親族特別控除申告書01_非居住者 = @SpecificRelativeSpecialDeduction01_ResidentCheck"
                            + " ,特定親族特別控除申告書01_住所 = @SpecificRelativeSpecialDeduction01_Address"
                            + " ,特定親族特別控除申告書01_給与所得_収入金額 = @SpecificRelativeSpecialDeduction01_Earnings"
                            + " ,特定親族特別控除申告書01_給与所得_所得金額 = @SpecificRelativeSpecialDeduction01_Income"
                            + " ,特定親族特別控除申告書01_他_所得金額 = @SpecificRelativeSpecialDeduction01_OtherIncome"
                            + " ,特定親族特別控除申告書01_合計所得金額見積額 = @SpecificRelativeSpecialDeduction01_TotalEarnings"
                            + " ,特定親族特別控除申告書01_控除額計算判定 = @SpecificRelativeSpecialDeduction01_EarningsType"
                            + " ,特定親族特別控除申告書01_控除額計算区分 = @SpecificRelativeSpecialDeduction01_CalcType"
                            + " ,特定親族特別控除申告書01_特定親族特別控除額 = @SpecificRelativeSpecialDeduction01_SpecialDeductionAmount"

                            + " ,特定親族特別控除申告書02_氏名_姓 = @SpecificRelativeSpecialDeduction02_Name1"
                            + " ,特定親族特別控除申告書02_氏名_名 = @SpecificRelativeSpecialDeduction02_Name2"
                            + " ,特定親族特別控除申告書02_Kana_姓 = @SpecificRelativeSpecialDeduction02_Kana1"
                            + " ,特定親族特別控除申告書02_Kana_名 = @SpecificRelativeSpecialDeduction02_Kana2"
                            + " ,特定親族特別控除申告書02_続柄 = @SpecificRelativeSpecialDeduction02_RelationshipType"
                            + " ,特定親族特別控除申告書02_生年月日 = @SpecificRelativeSpecialDeduction02_Birthday"
                            + " ,特定親族特別控除申告書02_非居住者 = @SpecificRelativeSpecialDeduction02_ResidentCheck"
                            + " ,特定親族特別控除申告書02_住所 = @SpecificRelativeSpecialDeduction02_Address"
                            + " ,特定親族特別控除申告書02_給与所得_収入金額 = @SpecificRelativeSpecialDeduction02_Earnings"
                            + " ,特定親族特別控除申告書02_給与所得_所得金額 = @SpecificRelativeSpecialDeduction02_Income"
                            + " ,特定親族特別控除申告書02_他_所得金額 = @SpecificRelativeSpecialDeduction02_OtherIncome"
                            + " ,特定親族特別控除申告書02_合計所得金額見積額 = @SpecificRelativeSpecialDeduction02_TotalEarnings"
                            + " ,特定親族特別控除申告書02_控除額計算判定 = @SpecificRelativeSpecialDeduction02_EarningsType"
                            + " ,特定親族特別控除申告書02_控除額計算区分 = @SpecificRelativeSpecialDeduction02_CalcType"
                            + " ,特定親族特別控除申告書02_特定親族特別控除額 = @SpecificRelativeSpecialDeduction02_SpecialDeductionAmount"

                            + " ,特定親族特別控除申告書03_氏名_姓 = @SpecificRelativeSpecialDeduction03_Name1"
                            + " ,特定親族特別控除申告書03_氏名_名 = @SpecificRelativeSpecialDeduction03_Name2"
                            + " ,特定親族特別控除申告書03_Kana_姓 = @SpecificRelativeSpecialDeduction03_Kana1"
                            + " ,特定親族特別控除申告書03_Kana_名 = @SpecificRelativeSpecialDeduction03_Kana2"
                            + " ,特定親族特別控除申告書03_続柄 = @SpecificRelativeSpecialDeduction03_RelationshipType"
                            + " ,特定親族特別控除申告書03_生年月日 = @SpecificRelativeSpecialDeduction03_Birthday"
                            + " ,特定親族特別控除申告書03_非居住者 = @SpecificRelativeSpecialDeduction03_ResidentCheck"
                            + " ,特定親族特別控除申告書03_住所 = @SpecificRelativeSpecialDeduction03_Address"
                            + " ,特定親族特別控除申告書03_給与所得_収入金額 = @SpecificRelativeSpecialDeduction03_Earnings"
                            + " ,特定親族特別控除申告書03_給与所得_所得金額 = @SpecificRelativeSpecialDeduction03_Income"
                            + " ,特定親族特別控除申告書03_他_所得金額 = @SpecificRelativeSpecialDeduction03_OtherIncome"
                            + " ,特定親族特別控除申告書03_合計所得金額見積額 = @SpecificRelativeSpecialDeduction03_TotalEarnings"
                            + " ,特定親族特別控除申告書03_控除額計算判定 = @SpecificRelativeSpecialDeduction03_EarningsType"
                            + " ,特定親族特別控除申告書03_控除額計算区分 = @SpecificRelativeSpecialDeduction03_CalcType"
                            + " ,特定親族特別控除申告書03_特定親族特別控除額 = @SpecificRelativeSpecialDeduction03_SpecialDeductionAmount"


                            //2025-99-99 iwai-tamura upd-end ------

							+ " ,最終更新者ID = '" + lu.UserCode + "'"
							+ " ,更新年月日 = GETDATE()"
							+ " ,更新回数 = 更新回数 + 1"

                            + " where 1=1 "
                            + "     AND 対象年度 = @SheetYear"
                            + "     AND 社員番号 = @EmployeeNo"
                            + "";
                        //SQL文の型を指定
                        var cmd = dm.CreateCommand(sql);
						DbHelper.AddDbParameter(cmd, "@ApprovalType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DecisionType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MyNumberCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DepartmentNo", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_TotalEarnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_EarningsType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_CalcType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_ResidentCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_TotalEarnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_EarningsType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_CalcType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_SpecialDeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_ConditionType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_ResidentCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_TotalEarnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@AdjustmentDeduction_ReportType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                        //2024-11-19 iwai-tamura upd-str ------
						DbHelper.AddDbParameter(cmd, "@BasicDeduction_TaxReductionTarget", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseDeduction_TaxReductionTarget", DbType.String);
                        //2024-11-19 iwai-tamura upd-end ------

                        //2025-99-99 iwai-tamura upd-str ------
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Name1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Name2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Kana1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Kana2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_RelationshipType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Birthday", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_ResidentCheck", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Address", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Earnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_Income", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_OtherIncome", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_TotalEarnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_EarningsType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_CalcType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction01_SpecialDeductionAmount", DbType.Int32);

                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Name1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Name2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Kana1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Kana2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_RelationshipType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Birthday", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_ResidentCheck", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Address", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Earnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_Income", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_OtherIncome", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_TotalEarnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_EarningsType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_CalcType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction02_SpecialDeductionAmount", DbType.Int32);

                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Name1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Name2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Kana1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Kana2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_RelationshipType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Birthday", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_ResidentCheck", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Address", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Earnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_Income", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_OtherIncome", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_TotalEarnings", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_EarningsType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_CalcType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SpecificRelativeSpecialDeduction03_SpecialDeductionAmount", DbType.Int32);
                        //2025-99-99 iwai-tamura upd-end ------


                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
						parameters[0].Value = DataConv.IfNull(strApproval);
						parameters[1].Value = DataConv.IfNull(strDecision);
						parameters[2].Value = DataConv.IfNull(checkValue(model.Head.MyNumberCheck));
						parameters[3].Value = model.Head.DepartmentNo;
						parameters[4].Value = DataConv.IfNull(model.Head.Name1);
						parameters[5].Value = DataConv.IfNull(model.Head.Name2);
						parameters[6].Value = DataConv.IfNull(model.Head.Kana1);
						parameters[7].Value = DataConv.IfNull(model.Head.Kana2);
						parameters[8].Value = DataConv.IfNull(model.Head.Address);
						parameters[9].Value = DataConv.IfNull(model.Head.BasicDeduction_Earnings.ToString());
						parameters[10].Value = DataConv.IfNull(model.Head.BasicDeduction_Income.ToString());
						parameters[11].Value = DataConv.IfNull(model.Head.BasicDeduction_OtherIncome.ToString());
						parameters[12].Value = DataConv.IfNull(model.Head.BasicDeduction_TotalEarnings.ToString());
						parameters[13].Value = DataConv.IfNull(model.Head.BasicDeduction_EarningsType);
						parameters[14].Value = DataConv.IfNull(model.Head.BasicDeduction_CalcType);
						parameters[15].Value = DataConv.IfNull(model.Head.BasicDeduction_DeductionAmount.ToString());
						parameters[16].Value = DataConv.IfNull(model.Head.SpouseDeduction_Name1);
						parameters[17].Value = DataConv.IfNull(model.Head.SpouseDeduction_Name2);
						parameters[18].Value = DataConv.IfNull(model.Head.SpouseDeduction_Kana1);
						parameters[19].Value = DataConv.IfNull(model.Head.SpouseDeduction_Kana2);
						parameters[20].Value = DataConv.IfNull(model.Head.SpouseDeduction_RelationshipType);
						parameters[21].Value = DataConv.IfNull(addYMD(model.Head.SpouseDeduction_BirthdayYear,model.Head.SpouseDeduction_BirthdayMonth,model.Head.SpouseDeduction_BirthdayDay));
						parameters[22].Value = DataConv.IfNull(checkValue(model.Head.SpouseDeduction_ResidentCheck));
						parameters[23].Value = DataConv.IfNull(model.Head.SpouseDeduction_Address);
						parameters[24].Value = DataConv.IfNull(model.Head.SpouseDeduction_Earnings.ToString());
						parameters[25].Value = DataConv.IfNull(model.Head.SpouseDeduction_Income.ToString());
						parameters[26].Value = DataConv.IfNull(model.Head.SpouseDeduction_OtherIncome.ToString());
						parameters[27].Value = DataConv.IfNull(model.Head.SpouseDeduction_TotalEarnings.ToString());
						parameters[28].Value = DataConv.IfNull(model.Head.SpouseDeduction_EarningsType);
						parameters[29].Value = DataConv.IfNull(model.Head.SpouseDeduction_CalcType);
						parameters[30].Value = DataConv.IfNull(model.Head.SpouseDeduction_DeductionAmount.ToString());
						parameters[31].Value = DataConv.IfNull(model.Head.SpouseDeduction_SpecialDeductionAmount.ToString());
						parameters[32].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_ConditionType);
						parameters[33].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_Name1);
						parameters[34].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_Name2);
						parameters[35].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_Kana1);
						parameters[36].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_Kana2);
						parameters[37].Value = DataConv.IfNull(addYMD(model.Head.AdjustmentDeduction_BirthdayYear,model.Head.AdjustmentDeduction_BirthdayMonth,model.Head.AdjustmentDeduction_BirthdayDay));
						parameters[38].Value = DataConv.IfNull(checkValue(model.Head.AdjustmentDeduction_ResidentCheck));
						parameters[39].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_Address);
						parameters[40].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_RelationshipType);
						parameters[41].Value = DataConv.IfNull(model.Head.AdjustmentDeduction_TotalEarnings.ToString());
						parameters[42].Value = DataConv.IfNull(checkValue(model.Head.AdjustmentDeduction_ReportType));
                        parameters[43].Value = DataConv.IfNull(model.Head.SheetYear.ToString());
                        parameters[44].Value = DataConv.IfNull(model.Head.EmployeeNo);
                        //2024-11-19 iwai-tamura upd-str ------
						parameters[45].Value = DataConv.IfNull(checkValue(model.Head.BasicDeduction_TaxReductionTarget));
						parameters[46].Value = DataConv.IfNull(checkValue(model.Head.SpouseDeduction_TaxReductionTarget));
                        //2024-11-19 iwai-tamura upd-end ------

                        //2025-99-99 iwai-tamura upd-str ------
                        // ----- 01 -----
                        parameters[47].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Name1);
                        parameters[48].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Name2);
                        parameters[49].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Kana1);
                        parameters[50].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Kana2);
                        parameters[51].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_RelationshipType);
                        parameters[52].Value = DataConv.IfNull(addYMD(model.Head.SpecificRelativeSpecialDeduction01_BirthdayYear, model.Head.SpecificRelativeSpecialDeduction01_BirthdayMonth, model.Head.SpecificRelativeSpecialDeduction01_BirthdayDay));
                        parameters[53].Value = DataConv.IfNull(checkValue(model.Head.SpecificRelativeSpecialDeduction01_ResidentCheck));
                        parameters[54].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Address);
                        parameters[55].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Earnings.ToString());
                        parameters[56].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_Income.ToString());
                        parameters[57].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_OtherIncome.ToString());
                        parameters[58].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_TotalEarnings.ToString());
                        parameters[59].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_EarningsType);
                        parameters[60].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_CalcType);
                        parameters[61].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction01_SpecialDeductionAmount.ToString());

                        // ----- 02 -----
                        parameters[62].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Name1);
                        parameters[63].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Name2);
                        parameters[64].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Kana1);
                        parameters[65].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Kana2);
                        parameters[66].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_RelationshipType);
                        parameters[67].Value = DataConv.IfNull(addYMD(model.Head.SpecificRelativeSpecialDeduction02_BirthdayYear, model.Head.SpecificRelativeSpecialDeduction02_BirthdayMonth, model.Head.SpecificRelativeSpecialDeduction02_BirthdayDay));
                        parameters[68].Value = DataConv.IfNull(checkValue(model.Head.SpecificRelativeSpecialDeduction02_ResidentCheck));
                        parameters[69].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Address);
                        parameters[70].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Earnings.ToString());
                        parameters[71].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_Income.ToString());
                        parameters[72].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_OtherIncome.ToString());
                        parameters[73].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_TotalEarnings.ToString());
                        parameters[74].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_EarningsType);
                        parameters[75].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_CalcType);
                        parameters[76].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction02_SpecialDeductionAmount.ToString());

                        // ----- 03 -----
                        parameters[77].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Name1);
                        parameters[78].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Name2);
                        parameters[79].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Kana1);
                        parameters[80].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Kana2);
                        parameters[81].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_RelationshipType);
                        parameters[82].Value = DataConv.IfNull(addYMD(model.Head.SpecificRelativeSpecialDeduction03_BirthdayYear, model.Head.SpecificRelativeSpecialDeduction03_BirthdayMonth, model.Head.SpecificRelativeSpecialDeduction03_BirthdayDay));
                        parameters[83].Value = DataConv.IfNull(checkValue(model.Head.SpecificRelativeSpecialDeduction03_ResidentCheck));
                        parameters[84].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Address);
                        parameters[85].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Earnings.ToString());
                        parameters[86].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_Income.ToString());
                        parameters[87].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_OtherIncome.ToString());
                        parameters[88].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_TotalEarnings.ToString());
                        parameters[89].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_EarningsType);
                        parameters[90].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_CalcType);
                        parameters[91].Value = DataConv.IfNull(model.Head.SpecificRelativeSpecialDeduction03_SpecialDeductionAmount.ToString());
                        //2025-99-99 iwai-tamura upd-end ------

                        cmd.ExecuteNonQuery();
                    
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

        /// <summary>
        /// 承認登録処理
        /// </summary>
        /// <param name="model">扶養控除申告書入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        public void Sign(HaiguuDeclareRegisterViewModels model, string strDepartmentNo, LoginUser lu, bool isSign = true) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);

                var strApproval = model.Head.ApprovalType;
                var strDecision = model.Head.DecisionType;
                switch (model.Head.InputMode) {
                    case ajustMode.SelfRegist:
                        if(strApproval == "1") strApproval = "0";
                        break;
                    case ajustMode.adminRegist:
                        //2023-11-20 iwai-tamura upd str -----
                        switch (strDecision) {
                            case "1":   //支社確定→本人確定
                                strDecision = "0";
                                break;

                            case "5":   //管理者確定　→　本人確定or支社確定(東京、関東のみ)
								if (strDepartmentNo.Substring(0,1) == "2"||strDepartmentNo.Substring(0,1) == "3") {
		                            strDecision = "1";

								} else {
	                                strDecision = "0";
								}
                                break;

                            default:
                                break;
                        }
                        //if(strDecision == "1") strDecision = "0";
                        //2023-11-20 iwai-tamura upd end -----
                        break;
                }

                using(var scope = new TransactionScope()) {
                    var dbm = new DbManager();
                    //基礎控除申告書承認
                    var sql = "update TE120基礎控除申告書Data"
                                + " set "
                                + " 本人確定区分 = @Approval"
                                + ",管理者確定区分 = @Decision"

                                + " ,最終更新者ID = '" + lu.UserCode + "'"
							    + " ,更新年月日 = GETDATE()"
    						    + " ,更新回数 = 更新回数 + 1"
                                
                                + " where 対象年度 = @SheetYear"
                                + "   and 社員番号 = @EmployeeNo";
                    //SQL文の型を指定
                    IDbCommand cmd = dbm.CreateCommand(sql);
                    DbHelper.AddDbParameter(cmd, "@Approval", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Decision", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = DataConv.IfNull(strApproval);
                    parameters[1].Value = DataConv.IfNull(strDecision);
                    parameters[2].Value = model.Head.SheetYear;
                    parameters[3].Value = DataConv.IfNull(model.Head.EmployeeNo);
                    cmd.ExecuteNonQuery();

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
    }
}
