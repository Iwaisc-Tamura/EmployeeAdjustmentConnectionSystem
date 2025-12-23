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
using System.Web;

namespace EmployeeAdjustmentConnectionSystem.BL.HuyouDeclareRegister {
    /// <summary>
    /// 扶養控除申告書入力ビジネスロジック
    /// </summary>
    public class HuyouDeclareRegisterBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public HuyouDeclareRegisterBL() {
        }


        /// <summary>
        /// 扶養控除申告書データ取得
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <returns>扶養控除申告書モデル</returns>
        public HuyouDeclareRegisterViewModels Select(int? intSheetYear,string strEmployeeNo,bool bolAdminMode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                HuyouDeclareRegisterViewModels model = new HuyouDeclareRegisterViewModels();
                model.Head = new HuyouDeclareRegisterHeaderModel();
                model.Body = new HuyouDeclareRegisterBodyModel();

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

                    var sql = "SELECT * FROM TE100扶養控除申告書Data WHERE 対象年度 = @SheetYear and 社員番号 = @EmployeeNo ";

					//2025-12-19 iwai-tamura upd str ------
					string userAdminNo = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
						(HttpContext.Current.Session["LoginUser"])).IsAdminNo.ToString();
					string userCode = ((EmployeeAdjustmentConnectionSystem.COM.Entity.Session.LoginUser)
						(HttpContext.Current.Session["LoginUser"])).UserCode.ToString();
					switch (userAdminNo) {
						case "K":
							break;

						case "1":
							sql += " AND (";
							sql += "   (社員番号 = '" + userCode + "' )";
							sql += "   Or( LEFT(所属番号,1) in('1','2','3'))" ;
							sql += " )";
							break;

						case "2":
						case "3":
						case "7":
						case "8":
						case "9":
							sql += " AND (";
							sql += "   (社員番号 = '" + userCode + "' )";
							sql += "   Or( LEFT(所属番号,1) = '" + userAdminNo +"')" ;
							sql += " )";
								break;

						default:
							sql += " AND (";
							sql += "   (社員番号 = '" + userCode + "' )";
							sql += " )";
							break;
					}
					//2025-12-19 iwai-tamura upd end ------

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
                            model.Head = new HuyouDeclareRegisterHeaderModel {
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
								Birthday = row["生年月日"].ToString(),
                                BirthdayYear = row["生年月日"].ToString()=="" ? "":row["生年月日"].ToString().Substring(0,4),
                                BirthdayMonth = row["生年月日"].ToString()=="" ? "":row["生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                BirthdayDay = row["生年月日"].ToString()=="" ? "":row["生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								HouseholdSelfCheck = row["本人区分"].ToString(),
								HouseholdName1 = row["世帯主氏名_姓"].ToString(),
								HouseholdName2 = row["世帯主氏名_名"].ToString(),
								RelationshipType = row["世帯主続柄"].ToString(),
								PostalCode_1 = row["郵便番号_前"].ToString(),
								PostalCode_2 = row["郵便番号_後"].ToString(),
								Address = row["住所01"].ToString(),
								//2024-11-19 iwai-tamura upd-str ------
								AddressBefore = row["住所01"].ToString(),
								//2024-11-19 iwai-tamura upd-end ------
								//2025-11-18 iwai-tamura upd-str ------
								Address02 = row["住所02"].ToString(),
								Address02Before = row["住所02"].ToString(),
								AddressType = row["住所区分"].ToString(),
								//2025-11-18 iwai-tamura upd-end ------
								SpouseCheck = row["配偶者有無"].ToString(),
								TaxWithholding_notSubject = row["源泉控除対象配偶者対象外区分"].ToString(),
								TaxWithholding_Name1 = row["源泉控除対象配偶者氏名_姓"].ToString(),
								TaxWithholding_Name2 = row["源泉控除対象配偶者氏名_名"].ToString(),
								TaxWithholding_Kana1 = row["源泉控除対象配偶者Kana_姓"].ToString(),
								TaxWithholding_Kana2 = row["源泉控除対象配偶者Kana_名"].ToString(),
                                TaxWithholding_RelationshipType = row["源泉控除対象配偶者続柄"].ToString(),
								TaxWithholding_Birthday = row["源泉控除対象配偶者生年月日"].ToString(),
                                TaxWithholding_BirthdayYear = row["源泉控除対象配偶者生年月日"].ToString()=="" ? "":row["源泉控除対象配偶者生年月日"].ToString().Substring(0,4),
                                TaxWithholding_BirthdayMonth = row["源泉控除対象配偶者生年月日"].ToString()=="" ? "":row["源泉控除対象配偶者生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                TaxWithholding_BirthdayDay = row["源泉控除対象配偶者生年月日"].ToString()=="" ? "":row["源泉控除対象配偶者生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

								//2023-11-20 iwai-tamura upd str -----
								TaxWithholding_Earnings = setMoney(row["源泉控除対象配偶者給与所得収入金額"].ToString()),
								TaxWithholding_Earnings2Income = setMoney(row["源泉控除対象配偶者給与所得所得金額"].ToString()),
								TaxWithholding_OtherIncome = setMoney(row["源泉控除対象配偶者他所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----

								TaxWithholding_Income = setMoney(row["源泉控除対象配偶者所得見積額"].ToString()),

								TaxWithholding_ResidentType = row["源泉控除対象配偶者非居住者"].ToString(),
								TaxWithholding_Address = row["源泉控除対象配偶者住所"].ToString(),
								TaxWithholding_TransferDate = changeDate(row["源泉控除対象配偶者異動月日"].ToString()),
								TaxWithholding_TransferComment = row["源泉控除対象配偶者事由"].ToString(),
								DependentsOver16_1_notSubject = row["控除対象扶養親族01_対象外区分"].ToString(),
								DependentsOver16_1_Name1 = row["控除対象扶養親族01_氏名_姓"].ToString(),
								DependentsOver16_1_Name2 = row["控除対象扶養親族01_氏名_名"].ToString(),
								DependentsOver16_1_Kana1 = row["控除対象扶養親族01_Kana_姓"].ToString(),
								DependentsOver16_1_Kana2 = row["控除対象扶養親族01_Kana_名"].ToString(),
								DependentsOver16_1_RelationshipType = row["控除対象扶養親族01_続柄"].ToString(),
								DependentsOver16_1_Birthday = row["控除対象扶養親族01_生年月日"].ToString(),
                                DependentsOver16_1_BirthdayYear = row["控除対象扶養親族01_生年月日"].ToString()=="" ? "":row["控除対象扶養親族01_生年月日"].ToString().Substring(0,4),
                                DependentsOver16_1_BirthdayMonth = row["控除対象扶養親族01_生年月日"].ToString()=="" ? "":row["控除対象扶養親族01_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsOver16_1_BirthdayDay = row["控除対象扶養親族01_生年月日"].ToString()=="" ? "":row["控除対象扶養親族01_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsOver16_1_OldmanType = row["控除対象扶養親族01_老人扶養親族区分"].ToString(),
								DependentsOver16_1_SpecificType = row["控除対象扶養親族01_特定扶養親族区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsOver16_1_Earnings = setMoney(row["控除対象扶養親族01_給与所得_収入金額"].ToString()),
								DependentsOver16_1_Earnings2Income = setMoney(row["控除対象扶養親族01_給与所得_所得金額"].ToString()),
								DependentsOver16_1_OtherIncome = setMoney(row["控除対象扶養親族01_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsOver16_1_Income = setMoney(row["控除対象扶養親族01_所得見積額"].ToString()),
								DependentsOver16_1_ResidentType = row["控除対象扶養親族01_非居住者"].ToString(),
								DependentsOver16_1_Address = row["控除対象扶養親族01_住所"].ToString(),
								DependentsOver16_1_TransferDate = changeDate(row["控除対象扶養親族01_異動月日"].ToString()),
								DependentsOver16_1_TransferComment = row["控除対象扶養親族01_事由"].ToString(),
								DependentsOver16_2_notSubject = row["控除対象扶養親族02_対象外区分"].ToString(),
								DependentsOver16_2_Name1 = row["控除対象扶養親族02_氏名_姓"].ToString(),
								DependentsOver16_2_Name2 = row["控除対象扶養親族02_氏名_名"].ToString(),
								DependentsOver16_2_Kana1 = row["控除対象扶養親族02_Kana_姓"].ToString(),
								DependentsOver16_2_Kana2 = row["控除対象扶養親族02_Kana_名"].ToString(),
								DependentsOver16_2_RelationshipType = row["控除対象扶養親族02_続柄"].ToString(),
								DependentsOver16_2_Birthday = row["控除対象扶養親族02_生年月日"].ToString(),
                                DependentsOver16_2_BirthdayYear = row["控除対象扶養親族02_生年月日"].ToString()=="" ? "":row["控除対象扶養親族02_生年月日"].ToString().Substring(0,4),
                                DependentsOver16_2_BirthdayMonth = row["控除対象扶養親族02_生年月日"].ToString()=="" ? "":row["控除対象扶養親族02_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsOver16_2_BirthdayDay = row["控除対象扶養親族02_生年月日"].ToString()=="" ? "":row["控除対象扶養親族02_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsOver16_2_OldmanType = row["控除対象扶養親族02_老人扶養親族区分"].ToString(),
								DependentsOver16_2_SpecificType = row["控除対象扶養親族02_特定扶養親族区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsOver16_2_Earnings = setMoney(row["控除対象扶養親族02_給与所得_収入金額"].ToString()),
								DependentsOver16_2_Earnings2Income = setMoney(row["控除対象扶養親族02_給与所得_所得金額"].ToString()),
								DependentsOver16_2_OtherIncome = setMoney(row["控除対象扶養親族02_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsOver16_2_Income = setMoney(row["控除対象扶養親族02_所得見積額"].ToString()),
								DependentsOver16_2_ResidentType = row["控除対象扶養親族02_非居住者"].ToString(),
								DependentsOver16_2_Address = row["控除対象扶養親族02_住所"].ToString(),
								DependentsOver16_2_TransferDate = changeDate(row["控除対象扶養親族02_異動月日"].ToString()),
								DependentsOver16_2_TransferComment = row["控除対象扶養親族02_事由"].ToString(),
								DependentsOver16_3_notSubject = row["控除対象扶養親族03_対象外区分"].ToString(),
								DependentsOver16_3_Name1 = row["控除対象扶養親族03_氏名_姓"].ToString(),
								DependentsOver16_3_Name2 = row["控除対象扶養親族03_氏名_名"].ToString(),
								DependentsOver16_3_Kana1 = row["控除対象扶養親族03_Kana_姓"].ToString(),
								DependentsOver16_3_Kana2 = row["控除対象扶養親族03_Kana_名"].ToString(),
								DependentsOver16_3_RelationshipType = row["控除対象扶養親族03_続柄"].ToString(),
								DependentsOver16_3_Birthday = row["控除対象扶養親族03_生年月日"].ToString(),
                                DependentsOver16_3_BirthdayYear = row["控除対象扶養親族03_生年月日"].ToString()=="" ? "":row["控除対象扶養親族03_生年月日"].ToString().Substring(0,4),
                                DependentsOver16_3_BirthdayMonth = row["控除対象扶養親族03_生年月日"].ToString()=="" ? "":row["控除対象扶養親族03_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsOver16_3_BirthdayDay = row["控除対象扶養親族03_生年月日"].ToString()=="" ? "":row["控除対象扶養親族03_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsOver16_3_OldmanType = row["控除対象扶養親族03_老人扶養親族区分"].ToString(),
								DependentsOver16_3_SpecificType = row["控除対象扶養親族03_特定扶養親族区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsOver16_3_Earnings = setMoney(row["控除対象扶養親族03_給与所得_収入金額"].ToString()),
								DependentsOver16_3_Earnings2Income = setMoney(row["控除対象扶養親族03_給与所得_所得金額"].ToString()),
								DependentsOver16_3_OtherIncome = setMoney(row["控除対象扶養親族03_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsOver16_3_Income = setMoney(row["控除対象扶養親族03_所得見積額"].ToString()),
								DependentsOver16_3_ResidentType = row["控除対象扶養親族03_非居住者"].ToString(),
								DependentsOver16_3_Address = row["控除対象扶養親族03_住所"].ToString(),
								DependentsOver16_3_TransferDate = changeDate(row["控除対象扶養親族03_異動月日"].ToString()),
								DependentsOver16_3_TransferComment = row["控除対象扶養親族03_事由"].ToString(),
								DependentsOver16_4_notSubject = row["控除対象扶養親族04_対象外区分"].ToString(),
								DependentsOver16_4_Name1 = row["控除対象扶養親族04_氏名_姓"].ToString(),
								DependentsOver16_4_Name2 = row["控除対象扶養親族04_氏名_名"].ToString(),
								DependentsOver16_4_Kana1 = row["控除対象扶養親族04_Kana_姓"].ToString(),
								DependentsOver16_4_Kana2 = row["控除対象扶養親族04_Kana_名"].ToString(),
								DependentsOver16_4_RelationshipType = row["控除対象扶養親族04_続柄"].ToString(),
								DependentsOver16_4_Birthday = row["控除対象扶養親族04_生年月日"].ToString(),
                                DependentsOver16_4_BirthdayYear = row["控除対象扶養親族04_生年月日"].ToString()=="" ? "":row["控除対象扶養親族04_生年月日"].ToString().Substring(0,4),
                                DependentsOver16_4_BirthdayMonth = row["控除対象扶養親族04_生年月日"].ToString()=="" ? "":row["控除対象扶養親族04_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsOver16_4_BirthdayDay = row["控除対象扶養親族04_生年月日"].ToString()=="" ? "":row["控除対象扶養親族04_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsOver16_4_OldmanType = row["控除対象扶養親族04_老人扶養親族区分"].ToString(),
								DependentsOver16_4_SpecificType = row["控除対象扶養親族04_特定扶養親族区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsOver16_4_Earnings = setMoney(row["控除対象扶養親族04_給与所得_収入金額"].ToString()),
								DependentsOver16_4_Earnings2Income = setMoney(row["控除対象扶養親族04_給与所得_所得金額"].ToString()),
								DependentsOver16_4_OtherIncome = setMoney(row["控除対象扶養親族04_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsOver16_4_Income = setMoney(row["控除対象扶養親族04_所得見積額"].ToString()),
								DependentsOver16_4_ResidentType = row["控除対象扶養親族04_非居住者"].ToString(),
								DependentsOver16_4_Address = row["控除対象扶養親族04_住所"].ToString(),
								DependentsOver16_4_TransferDate = changeDate(row["控除対象扶養親族04_異動月日"].ToString()),
								DependentsOver16_4_TransferComment = row["控除対象扶養親族04_事由"].ToString(),
								DependentsOther_Subject = row["障害者"].ToString(),
								DependentsOther_GeneralHandicappedSelfCheck = row["一般障害_本人"].ToString(),
								DependentsOther_GeneralHandicappedSpouseCheck = row["一般障害_同一生計配偶者"].ToString(),
								DependentsOther_GeneralHandicappedDependentsCheck = row["一般障害_扶養親族"].ToString(),
								DependentsOther_GeneralHandicappedDependentsNumber = row["一般障害_扶養親族人数"].ToString(),
								DependentsOther_SpecialHandicappedSelfCheck = row["特別障害者_本人"].ToString(),
								DependentsOther_SpecialHandicappedSpouseCheck = row["特別障害者_同一生計配偶者"].ToString(),
								DependentsOther_SpecialHandicappedDependentsCheck = row["特別障害者_扶養親族"].ToString(),
								DependentsOther_SpecialHandicappedDependentsNumber = row["特別障害者_扶養親族人数"].ToString(),
								DependentsOther_LivingHandicappedSpouseCheck = row["同居特別障害者_同一生計配偶者"].ToString(),
								DependentsOther_LivingHandicappedDependentsCheck = row["同居特別障害者_扶養親族"].ToString(),
								DependentsOther_LivingHandicappedDependentsNumber = row["同居特別障害者_扶養親族人数"].ToString(),
								DependentsOther_WidowType = row["寡婦一人親区分"].ToString(),
								DependentsOther_WidowReasonType = row["理由区分"].ToString(),
								DependentsOther_WidowOccurrenceDate = changeDate(row["発生年月日"].ToString()),
								DependentsOther_StudentCheck = row["勤労学生"].ToString(),
								DependentsOther_TransferDate = changeDate(row["障害異動月日"].ToString()),
								DependentsOther_TransferComment = row["障害事由"].ToString(),
								DependentsUnder16_1_notSubject = row["扶養親族16未満01_対象外区分"].ToString(),
								DependentsUnder16_1_Name1 = row["扶養親族16未満01_氏名_姓"].ToString(),
								DependentsUnder16_1_Name2 = row["扶養親族16未満01_氏名_名"].ToString(),
								DependentsUnder16_1_Kana1 = row["扶養親族16未満01_Kana_姓"].ToString(),
								DependentsUnder16_1_Kana2 = row["扶養親族16未満01_Kana_名"].ToString(),
								DependentsUnder16_1_RelationshipType = row["扶養親族16未満01_続柄"].ToString(),
								DependentsUnder16_1_Birthday = row["扶養親族16未満01_生年月日"].ToString(),
                                DependentsUnder16_1_BirthdayYear = row["扶養親族16未満01_生年月日"].ToString()=="" ? "":row["扶養親族16未満01_生年月日"].ToString().Substring(0,4),
                                DependentsUnder16_1_BirthdayMonth = row["扶養親族16未満01_生年月日"].ToString()=="" ? "":row["扶養親族16未満01_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsUnder16_1_BirthdayDay = row["扶養親族16未満01_生年月日"].ToString()=="" ? "":row["扶養親族16未満01_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsUnder16_1_AddressSameCheck = row["扶養親族16未満01_同上区分"].ToString(),
								DependentsUnder16_1_Address = row["扶養親族16未満01_住所"].ToString(),
								DependentsUnder16_1_AbroadCheck = row["扶養親族16未満01_国外区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsUnder16_1_Earnings = setMoney(row["扶養親族16未満01_給与所得_収入金額"].ToString()),
								DependentsUnder16_1_Earnings2Income = setMoney(row["扶養親族16未満01_給与所得_所得金額"].ToString()),
								DependentsUnder16_1_OtherIncome = setMoney(row["扶養親族16未満01_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsUnder16_1_Income = setMoney(row["扶養親族16未満01_所得見積額"].ToString()),
								DependentsUnder16_1_TransferDate = changeDate(row["扶養親族16未満01_異動月日"].ToString()),
								DependentsUnder16_1_TransferComment = row["扶養親族16未満01_事由"].ToString(),
								DependentsUnder16_2_notSubject = row["扶養親族16未満02_対象外区分"].ToString(),
								DependentsUnder16_2_Name1 = row["扶養親族16未満02_氏名_姓"].ToString(),
								DependentsUnder16_2_Name2 = row["扶養親族16未満02_氏名_名"].ToString(),
								DependentsUnder16_2_Kana1 = row["扶養親族16未満02_Kana_姓"].ToString(),
								DependentsUnder16_2_Kana2 = row["扶養親族16未満02_Kana_名"].ToString(),
								DependentsUnder16_2_RelationshipType = row["扶養親族16未満02_続柄"].ToString(),
								DependentsUnder16_2_Birthday = row["扶養親族16未満02_生年月日"].ToString(),
                                DependentsUnder16_2_BirthdayYear = row["扶養親族16未満02_生年月日"].ToString()=="" ? "":row["扶養親族16未満02_生年月日"].ToString().Substring(0,4),
                                DependentsUnder16_2_BirthdayMonth = row["扶養親族16未満02_生年月日"].ToString()=="" ? "":row["扶養親族16未満02_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsUnder16_2_BirthdayDay = row["扶養親族16未満02_生年月日"].ToString()=="" ? "":row["扶養親族16未満02_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsUnder16_2_AddressSameCheck = row["扶養親族16未満02_同上区分"].ToString(),
								DependentsUnder16_2_Address = row["扶養親族16未満02_住所"].ToString(),
								DependentsUnder16_2_AbroadCheck = row["扶養親族16未満02_国外区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsUnder16_2_Earnings = setMoney(row["扶養親族16未満02_給与所得_収入金額"].ToString()),
								DependentsUnder16_2_Earnings2Income = setMoney(row["扶養親族16未満02_給与所得_所得金額"].ToString()),
								DependentsUnder16_2_OtherIncome = setMoney(row["扶養親族16未満02_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsUnder16_2_Income = setMoney(row["扶養親族16未満02_所得見積額"].ToString()),
								DependentsUnder16_2_TransferDate = changeDate(row["扶養親族16未満02_異動月日"].ToString()),
								DependentsUnder16_2_TransferComment = row["扶養親族16未満02_事由"].ToString(),
								DependentsUnder16_3_notSubject = row["扶養親族16未満03_対象外区分"].ToString(),
								DependentsUnder16_3_Name1 = row["扶養親族16未満03_氏名_姓"].ToString(),
								DependentsUnder16_3_Name2 = row["扶養親族16未満03_氏名_名"].ToString(),
								DependentsUnder16_3_Kana1 = row["扶養親族16未満03_Kana_姓"].ToString(),
								DependentsUnder16_3_Kana2 = row["扶養親族16未満03_Kana_名"].ToString(),
								DependentsUnder16_3_RelationshipType = row["扶養親族16未満03_続柄"].ToString(),
								DependentsUnder16_3_Birthday = row["扶養親族16未満03_生年月日"].ToString(),
                                DependentsUnder16_3_BirthdayYear = row["扶養親族16未満03_生年月日"].ToString()=="" ? "":row["扶養親族16未満03_生年月日"].ToString().Substring(0,4),
                                DependentsUnder16_3_BirthdayMonth = row["扶養親族16未満03_生年月日"].ToString()=="" ? "":row["扶養親族16未満03_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsUnder16_3_BirthdayDay = row["扶養親族16未満03_生年月日"].ToString()=="" ? "":row["扶養親族16未満03_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsUnder16_3_AddressSameCheck = row["扶養親族16未満03_同上区分"].ToString(),
								DependentsUnder16_3_Address = row["扶養親族16未満03_住所"].ToString(),
								DependentsUnder16_3_AbroadCheck = row["扶養親族16未満03_国外区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsUnder16_3_Earnings = setMoney(row["扶養親族16未満03_給与所得_収入金額"].ToString()),
								DependentsUnder16_3_Earnings2Income = setMoney(row["扶養親族16未満03_給与所得_所得金額"].ToString()),
								DependentsUnder16_3_OtherIncome = setMoney(row["扶養親族16未満03_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsUnder16_3_Income = setMoney(row["扶養親族16未満03_所得見積額"].ToString()),
								DependentsUnder16_3_TransferDate = changeDate(row["扶養親族16未満03_異動月日"].ToString()),
								DependentsUnder16_3_TransferComment = row["扶養親族16未満03_事由"].ToString(),
								DependentsUnder16_4_notSubject = row["扶養親族16未満04_対象外区分"].ToString(),
								DependentsUnder16_4_Name1 = row["扶養親族16未満04_氏名_姓"].ToString(),
								DependentsUnder16_4_Name2 = row["扶養親族16未満04_氏名_名"].ToString(),
								DependentsUnder16_4_Kana1 = row["扶養親族16未満04_Kana_姓"].ToString(),
								DependentsUnder16_4_Kana2 = row["扶養親族16未満04_Kana_名"].ToString(),
								DependentsUnder16_4_RelationshipType = row["扶養親族16未満04_続柄"].ToString(),
								DependentsUnder16_4_Birthday = row["扶養親族16未満04_生年月日"].ToString(),
                                DependentsUnder16_4_BirthdayYear = row["扶養親族16未満04_生年月日"].ToString()=="" ? "":row["扶養親族16未満04_生年月日"].ToString().Substring(0,4),
                                DependentsUnder16_4_BirthdayMonth = row["扶養親族16未満04_生年月日"].ToString()=="" ? "":row["扶養親族16未満04_生年月日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                DependentsUnder16_4_BirthdayDay = row["扶養親族16未満04_生年月日"].ToString()=="" ? "":row["扶養親族16未満04_生年月日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),
								DependentsUnder16_4_AddressSameCheck = row["扶養親族16未満04_同上区分"].ToString(),
								DependentsUnder16_4_Address = row["扶養親族16未満04_住所"].ToString(),
								DependentsUnder16_4_AbroadCheck = row["扶養親族16未満04_国外区分"].ToString(),
								//2023-11-20 iwai-tamura upd str -----
								DependentsUnder16_4_Earnings = setMoney(row["扶養親族16未満04_給与所得_収入金額"].ToString()),
								DependentsUnder16_4_Earnings2Income = setMoney(row["扶養親族16未満04_給与所得_所得金額"].ToString()),
								DependentsUnder16_4_OtherIncome = setMoney(row["扶養親族16未満04_他_所得金額"].ToString()),
								//2023-11-20 iwai-tamura upd end -----
								DependentsUnder16_4_Income = setMoney(row["扶養親族16未満04_所得見積額"].ToString()),
								DependentsUnder16_4_TransferDate = changeDate(row["扶養親族16未満04_異動月日"].ToString()),
								DependentsUnder16_4_TransferComment = row["扶養親族16未満04_事由"].ToString(),


                            };


							//2024-11-19 iwai-tamura upd-str ------
							// 家族の人数を計算する
							int countFamilyNames = 0;
							int countFamilyNotSubjects = 0;

							// 氏名が入っているか確認
							if (!string.IsNullOrEmpty(row["源泉控除対象配偶者氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["控除対象扶養親族01_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["控除対象扶養親族02_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["控除対象扶養親族03_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["控除対象扶養親族04_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["扶養親族16未満01_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["扶養親族16未満02_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["扶養親族16未満03_氏名_姓"].ToString())){
								countFamilyNames++;
							}
							if (!string.IsNullOrEmpty(row["扶養親族16未満04_氏名_姓"].ToString())){
								countFamilyNames++;
							}

							// 対象外区分が入っているか確認
							if (row["源泉控除対象配偶者対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["控除対象扶養親族01_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["控除対象扶養親族02_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["控除対象扶養親族03_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["控除対象扶養親族04_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["扶養親族16未満01_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["扶養親族16未満02_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["扶養親族16未満03_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							if (row["扶養親族16未満04_対象外区分"].ToString() == "1"){
								countFamilyNotSubjects++;
							}
							// 家族の人数を計算
							model.Head.FamilyCount = countFamilyNames - countFamilyNotSubjects;
							//2024-11-19 iwai-tamura upd-end ------

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
        public void SetMode(HuyouDeclareRegisterViewModels model, LoginUser lu) {
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


                //using(DbManager dm = new DbManager()) {
                //    //承認情報現在地取得
                //    //var mode = (SelfDeclareMode)Enum.Parse(typeof(SelfDeclareMode), (SelfDeclareCommonBL.GetSignStatus(dm, int.Parse(model.Head.ManageNo),"1", false)).ToString());
                //    var mode =ajustMode.None;
                //    //自データ時設定
                //    if(model.Head.EmployeeNo == lu.UserCode) {
                //        //モード設定
                //        switch(mode) {
                //            //本人入力前
                //            case ajustMode.None:
                //                model.Head.InputMode = ajustMode.AtoCSelfSign;
                //                model.Head.AuthButton = ajustMode.AtoCSelfSign;
                //                break;
                //            //本人入力後
                //            case ajustMode.AtoCSelfSign:
                //                model.Head.CancelButton = ajustMode.AtoCSelfSign;
                //                break;
                //            //上記以外
                //            default:
                //                break;
                //        }
                //        return;
                //    }
                //    //他データ時設定
                //    //承認者か？ //承認者以外
                //    if(!SelfDeclareCommonBL.IsAuthorizer(dm, model.Head.ManageNo, lu.UserCode)) {
                //        //一般社員
                //        if(lu.IsRoot == false) return;
                //        //管理者用キャンセル
                //        this.SetRootCancel(dm, model, lu, mode);
                //        return;
                //    }
                //    //承認者時の画面状態設定
                //    this.SetInputSignMode(dm, model, lu, mode);
                //}
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
        public void Save(HuyouDeclareRegisterViewModels model, LoginUser lu,string mode) {
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

                        Func<DateTime?,string> changeDate = (val) => {
                            if(val == null) {
                                return null;
                            } else {
                                return String.Format("{0:yyyyMMdd}",val);
                            }
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

						//勤労学生用
                        Func<string, string> checkValueKin = (val1) => {
                            if(!string.IsNullOrEmpty(val1)) {
                                if(val1=="0" || val1=="1"){
                                    return val1;
                                }
                                return (val1 == "true")? "1": "0";
                            }
                            return "0";
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
                        var sql = "update TE100扶養控除申告書Data"
                            + " set "
                            
                            + " 本人確定区分 = @ApprovalType"
							+ " ,管理者確定区分 = @DecisionType"
							+ " ,個人番号相違確認区分 = @MyNumberCheck"
							+ " ,所属番号 = @DepartmentNo"
							+ " ,氏名_姓 = @Name1"
							+ " ,氏名_名 = @Name2"
							+ " ,Kana_姓 = @Kana1"
							+ " ,Kana_名 = @Kana2"
							+ " ,生年月日 = @Birthday"
							+ " ,本人区分 = @HouseholdSelfCheck"
							+ " ,世帯主氏名_姓 = @HouseholdName1"
							+ " ,世帯主氏名_名 = @HouseholdName2"
							+ " ,世帯主続柄 = @RelationshipType"
							+ " ,郵便番号_前 = @PostalCode_1"
							+ " ,郵便番号_後 = @PostalCode_2"
							+ " ,住所01 = @Address"
							//2025-11-18 iwai-tamura upd-str ------
							+ " ,住所02 = @Address02"
							+ " ,住所区分 = @AddressType"
							//2025-11-18 iwai-tamura upd-end ------
							+ " ,配偶者有無 = @SpouseCheck"
							+ " ,源泉控除対象配偶者対象外区分 = @TaxWithholding_notSubject"
							+ " ,源泉控除対象配偶者氏名_姓 = @TaxWithholding_Name1"
							+ " ,源泉控除対象配偶者氏名_名 = @TaxWithholding_Name2"
							+ " ,源泉控除対象配偶者Kana_姓 = @TaxWithholding_Kana1"
							+ " ,源泉控除対象配偶者Kana_名 = @TaxWithholding_Kana2"
							+ " ,源泉控除対象配偶者生年月日 = @TaxWithholding_Birthday"
							+ " ,源泉控除対象配偶者所得見積額 = @TaxWithholding_Income"
							+ " ,源泉控除対象配偶者非居住者 = @TaxWithholding_ResidentType"
							+ " ,源泉控除対象配偶者住所 = @TaxWithholding_Address"
							+ " ,源泉控除対象配偶者異動月日 = @TaxWithholding_TransferDate"
							+ " ,源泉控除対象配偶者事由 = @TaxWithholding_TransferComment"
							+ " ,控除対象扶養親族01_対象外区分 = @DependentsOver16_1_notSubject"
							+ " ,控除対象扶養親族01_氏名_姓 = @DependentsOver16_1_Name1"
							+ " ,控除対象扶養親族01_氏名_名 = @DependentsOver16_1_Name2"
							+ " ,控除対象扶養親族01_Kana_姓 = @DependentsOver16_1_Kana1"
							+ " ,控除対象扶養親族01_Kana_名 = @DependentsOver16_1_Kana2"
							+ " ,控除対象扶養親族01_続柄 = @DependentsOver16_1_RelationshipType"
							+ " ,控除対象扶養親族01_生年月日 = @DependentsOver16_1_Birthday"
							+ " ,控除対象扶養親族01_老人扶養親族区分 = @DependentsOver16_1_OldmanType"
							+ " ,控除対象扶養親族01_特定扶養親族区分 = @DependentsOver16_1_SpecificType"
							+ " ,控除対象扶養親族01_所得見積額 = @DependentsOver16_1_Income"
							+ " ,控除対象扶養親族01_非居住者 = @DependentsOver16_1_ResidentType"
							+ " ,控除対象扶養親族01_住所 = @DependentsOver16_1_Address"
							+ " ,控除対象扶養親族01_異動月日 = @DependentsOver16_1_TransferDate"
							+ " ,控除対象扶養親族01_事由 = @DependentsOver16_1_TransferComment"
							+ " ,控除対象扶養親族02_対象外区分 = @DependentsOver16_2_notSubject"
							+ " ,控除対象扶養親族02_氏名_姓 = @DependentsOver16_2_Name1"
							+ " ,控除対象扶養親族02_氏名_名 = @DependentsOver16_2_Name2"
							+ " ,控除対象扶養親族02_Kana_姓 = @DependentsOver16_2_Kana1"
							+ " ,控除対象扶養親族02_Kana_名 = @DependentsOver16_2_Kana2"
							+ " ,控除対象扶養親族02_続柄 = @DependentsOver16_2_RelationshipType"
							+ " ,控除対象扶養親族02_生年月日 = @DependentsOver16_2_Birthday"
							+ " ,控除対象扶養親族02_老人扶養親族区分 = @DependentsOver16_2_OldmanType"
							+ " ,控除対象扶養親族02_特定扶養親族区分 = @DependentsOver16_2_SpecificType"
							+ " ,控除対象扶養親族02_所得見積額 = @DependentsOver16_2_Income"
							+ " ,控除対象扶養親族02_非居住者 = @DependentsOver16_2_ResidentType"
							+ " ,控除対象扶養親族02_住所 = @DependentsOver16_2_Address"
							+ " ,控除対象扶養親族02_異動月日 = @DependentsOver16_2_TransferDate"
							+ " ,控除対象扶養親族02_事由 = @DependentsOver16_2_TransferComment"
							+ " ,控除対象扶養親族03_対象外区分 = @DependentsOver16_3_notSubject"
							+ " ,控除対象扶養親族03_氏名_姓 = @DependentsOver16_3_Name1"
							+ " ,控除対象扶養親族03_氏名_名 = @DependentsOver16_3_Name2"
							+ " ,控除対象扶養親族03_Kana_姓 = @DependentsOver16_3_Kana1"
							+ " ,控除対象扶養親族03_Kana_名 = @DependentsOver16_3_Kana2"
							+ " ,控除対象扶養親族03_続柄 = @DependentsOver16_3_RelationshipType"
							+ " ,控除対象扶養親族03_生年月日 = @DependentsOver16_3_Birthday"
							+ " ,控除対象扶養親族03_老人扶養親族区分 = @DependentsOver16_3_OldmanType"
							+ " ,控除対象扶養親族03_特定扶養親族区分 = @DependentsOver16_3_SpecificType"
							+ " ,控除対象扶養親族03_所得見積額 = @DependentsOver16_3_Income"
							+ " ,控除対象扶養親族03_非居住者 = @DependentsOver16_3_ResidentType"
							+ " ,控除対象扶養親族03_住所 = @DependentsOver16_3_Address"
							+ " ,控除対象扶養親族03_異動月日 = @DependentsOver16_3_TransferDate"
							+ " ,控除対象扶養親族03_事由 = @DependentsOver16_3_TransferComment"
							+ " ,控除対象扶養親族04_対象外区分 = @DependentsOver16_4_notSubject"
							+ " ,控除対象扶養親族04_氏名_姓 = @DependentsOver16_4_Name1"
							+ " ,控除対象扶養親族04_氏名_名 = @DependentsOver16_4_Name2"
							+ " ,控除対象扶養親族04_Kana_姓 = @DependentsOver16_4_Kana1"
							+ " ,控除対象扶養親族04_Kana_名 = @DependentsOver16_4_Kana2"
							+ " ,控除対象扶養親族04_続柄 = @DependentsOver16_4_RelationshipType"
							+ " ,控除対象扶養親族04_生年月日 = @DependentsOver16_4_Birthday"
							+ " ,控除対象扶養親族04_老人扶養親族区分 = @DependentsOver16_4_OldmanType"
							+ " ,控除対象扶養親族04_特定扶養親族区分 = @DependentsOver16_4_SpecificType"
							+ " ,控除対象扶養親族04_所得見積額 = @DependentsOver16_4_Income"
							+ " ,控除対象扶養親族04_非居住者 = @DependentsOver16_4_ResidentType"
							+ " ,控除対象扶養親族04_住所 = @DependentsOver16_4_Address"
							+ " ,控除対象扶養親族04_異動月日 = @DependentsOver16_4_TransferDate"
							+ " ,控除対象扶養親族04_事由 = @DependentsOver16_4_TransferComment"
							+ " ,障害者 = @DependentsOther_Subject"
							+ " ,一般障害_本人 = @DependentsOther_GeneralHandicappedSelfCheck"
							+ " ,一般障害_同一生計配偶者 = @DependentsOther_GeneralHandicappedSpouseCheck"
							+ " ,一般障害_扶養親族 = @DependentsOther_GeneralHandicappedDependentsCheck"
							+ " ,一般障害_扶養親族人数 = @DependentsOther_GeneralHandicappedDependentsNumber"
							+ " ,特別障害者_本人 = @DependentsOther_SpecialHandicappedSelfCheck"
							+ " ,特別障害者_同一生計配偶者 = @DependentsOther_SpecialHandicappedSpouseCheck"
							+ " ,特別障害者_扶養親族 = @DependentsOther_SpecialHandicappedDependentsCheck"
							+ " ,特別障害者_扶養親族人数 = @DependentsOther_SpecialHandicappedDependentsNumber"
							+ " ,同居特別障害者_同一生計配偶者 = @DependentsOther_LivingHandicappedSpouseCheck"
							+ " ,同居特別障害者_扶養親族 = @DependentsOther_LivingHandicappedDependentsCheck"
							+ " ,同居特別障害者_扶養親族人数 = @DependentsOther_LivingHandicappedDependentsNumber"
							+ " ,寡婦一人親区分 = @DependentsOther_WidowType"
							+ " ,理由区分 = @DependentsOther_WidowReasonType"
							+ " ,発生年月日 = @DependentsOther_WidowOccurrenceDate"
							+ " ,勤労学生 = @DependentsOther_StudentCheck"
							+ " ,障害異動月日 = @DependentsOther_TransferDate"
							+ " ,障害事由 = @DependentsOther_TransferComment"
							+ " ,扶養親族16未満01_対象外区分 = @DependentsUnder16_1_notSubject"
							+ " ,扶養親族16未満01_氏名_姓 = @DependentsUnder16_1_Name1"
							+ " ,扶養親族16未満01_氏名_名 = @DependentsUnder16_1_Name2"
							+ " ,扶養親族16未満01_Kana_姓 = @DependentsUnder16_1_Kana1"
							+ " ,扶養親族16未満01_Kana_名 = @DependentsUnder16_1_Kana2"
							+ " ,扶養親族16未満01_続柄 = @DependentsUnder16_1_RelationshipType"
							+ " ,扶養親族16未満01_生年月日 = @DependentsUnder16_1_Birthday"
							+ " ,扶養親族16未満01_同上区分 = @DependentsUnder16_1_AddressSameCheck"
							+ " ,扶養親族16未満01_住所 = @DependentsUnder16_1_Address"
							+ " ,扶養親族16未満01_国外区分 = @DependentsUnder16_1_AbroadCheck"
							+ " ,扶養親族16未満01_所得見積額 = @DependentsUnder16_1_Income"
							+ " ,扶養親族16未満01_異動月日 = @DependentsUnder16_1_TransferDate"
							+ " ,扶養親族16未満01_事由 = @DependentsUnder16_1_TransferComment"
							+ " ,扶養親族16未満02_対象外区分 = @DependentsUnder16_2_notSubject"
							+ " ,扶養親族16未満02_氏名_姓 = @DependentsUnder16_2_Name1"
							+ " ,扶養親族16未満02_氏名_名 = @DependentsUnder16_2_Name2"
							+ " ,扶養親族16未満02_Kana_姓 = @DependentsUnder16_2_Kana1"
							+ " ,扶養親族16未満02_Kana_名 = @DependentsUnder16_2_Kana2"
							+ " ,扶養親族16未満02_続柄 = @DependentsUnder16_2_RelationshipType"
							+ " ,扶養親族16未満02_生年月日 = @DependentsUnder16_2_Birthday"
							+ " ,扶養親族16未満02_同上区分 = @DependentsUnder16_2_AddressSameCheck"
							+ " ,扶養親族16未満02_住所 = @DependentsUnder16_2_Address"
							+ " ,扶養親族16未満02_国外区分 = @DependentsUnder16_2_AbroadCheck"
							+ " ,扶養親族16未満02_所得見積額 = @DependentsUnder16_2_Income"
							+ " ,扶養親族16未満02_異動月日 = @DependentsUnder16_2_TransferDate"
							+ " ,扶養親族16未満02_事由 = @DependentsUnder16_2_TransferComment"
							+ " ,扶養親族16未満03_対象外区分 = @DependentsUnder16_3_notSubject"
							+ " ,扶養親族16未満03_氏名_姓 = @DependentsUnder16_3_Name1"
							+ " ,扶養親族16未満03_氏名_名 = @DependentsUnder16_3_Name2"
							+ " ,扶養親族16未満03_Kana_姓 = @DependentsUnder16_3_Kana1"
							+ " ,扶養親族16未満03_Kana_名 = @DependentsUnder16_3_Kana2"
							+ " ,扶養親族16未満03_続柄 = @DependentsUnder16_3_RelationshipType"
							+ " ,扶養親族16未満03_生年月日 = @DependentsUnder16_3_Birthday"
							+ " ,扶養親族16未満03_同上区分 = @DependentsUnder16_3_AddressSameCheck"
							+ " ,扶養親族16未満03_住所 = @DependentsUnder16_3_Address"
							+ " ,扶養親族16未満03_国外区分 = @DependentsUnder16_3_AbroadCheck"
							+ " ,扶養親族16未満03_所得見積額 = @DependentsUnder16_3_Income"
							+ " ,扶養親族16未満03_異動月日 = @DependentsUnder16_3_TransferDate"
							+ " ,扶養親族16未満03_事由 = @DependentsUnder16_3_TransferComment"
							+ " ,扶養親族16未満04_対象外区分 = @DependentsUnder16_4_notSubject"
							+ " ,扶養親族16未満04_氏名_姓 = @DependentsUnder16_4_Name1"
							+ " ,扶養親族16未満04_氏名_名 = @DependentsUnder16_4_Name2"
							+ " ,扶養親族16未満04_Kana_姓 = @DependentsUnder16_4_Kana1"
							+ " ,扶養親族16未満04_Kana_名 = @DependentsUnder16_4_Kana2"
							+ " ,扶養親族16未満04_続柄 = @DependentsUnder16_4_RelationshipType"
							+ " ,扶養親族16未満04_生年月日 = @DependentsUnder16_4_Birthday"
							+ " ,扶養親族16未満04_同上区分 = @DependentsUnder16_4_AddressSameCheck"
							+ " ,扶養親族16未満04_住所 = @DependentsUnder16_4_Address"
							+ " ,扶養親族16未満04_国外区分 = @DependentsUnder16_4_AbroadCheck"
							+ " ,扶養親族16未満04_所得見積額 = @DependentsUnder16_4_Income"
							+ " ,扶養親族16未満04_異動月日 = @DependentsUnder16_4_TransferDate"
							+ " ,扶養親族16未満04_事由 = @DependentsUnder16_4_TransferComment"
							+ " ,源泉控除対象配偶者続柄 = @TaxWithholding_RelationshipType"

							//2023-11-20 iwai-tamura upd str -----
							+ " ,源泉控除対象配偶者給与所得収入金額 = @TaxWithholding_Earnings"
							+ " ,源泉控除対象配偶者給与所得所得金額 = @TaxWithholding_Earnings2Income"
							+ " ,源泉控除対象配偶者他所得金額 = @TaxWithholding_OtherIncome"
							+ " ,控除対象扶養親族01_給与所得_収入金額 = @DependentsOver16_1_Earnings"
							+ " ,控除対象扶養親族01_給与所得_所得金額 = @DependentsOver16_1_Earnings2Income"
							+ " ,控除対象扶養親族01_他_所得金額 = @DependentsOver16_1_OtherIncome"
							+ " ,控除対象扶養親族02_給与所得_収入金額 = @DependentsOver16_2_Earnings"
							+ " ,控除対象扶養親族02_給与所得_所得金額 = @DependentsOver16_2_Earnings2Income"
							+ " ,控除対象扶養親族02_他_所得金額 = @DependentsOver16_2_OtherIncome"
							+ " ,控除対象扶養親族03_給与所得_収入金額 = @DependentsOver16_3_Earnings"
							+ " ,控除対象扶養親族03_給与所得_所得金額 = @DependentsOver16_3_Earnings2Income"
							+ " ,控除対象扶養親族03_他_所得金額 = @DependentsOver16_3_OtherIncome"
							+ " ,控除対象扶養親族04_給与所得_収入金額 = @DependentsOver16_4_Earnings"
							+ " ,控除対象扶養親族04_給与所得_所得金額 = @DependentsOver16_4_Earnings2Income"
							+ " ,控除対象扶養親族04_他_所得金額 = @DependentsOver16_4_OtherIncome"
							+ " ,扶養親族16未満01_給与所得_収入金額 = @DependentsUnder16_1_Earnings"
							+ " ,扶養親族16未満01_給与所得_所得金額 = @DependentsUnder16_1_Earnings2Income"
							+ " ,扶養親族16未満01_他_所得金額 = @DependentsUnder16_1_OtherIncome"
							+ " ,扶養親族16未満02_給与所得_収入金額 = @DependentsUnder16_2_Earnings"
							+ " ,扶養親族16未満02_給与所得_所得金額 = @DependentsUnder16_2_Earnings2Income"
							+ " ,扶養親族16未満02_他_所得金額 = @DependentsUnder16_2_OtherIncome"
							+ " ,扶養親族16未満03_給与所得_収入金額 = @DependentsUnder16_3_Earnings"
							+ " ,扶養親族16未満03_給与所得_所得金額 = @DependentsUnder16_3_Earnings2Income"
							+ " ,扶養親族16未満03_他_所得金額 = @DependentsUnder16_3_OtherIncome"
							+ " ,扶養親族16未満04_給与所得_収入金額 = @DependentsUnder16_4_Earnings"
							+ " ,扶養親族16未満04_給与所得_所得金額 = @DependentsUnder16_4_Earnings2Income"
							+ " ,扶養親族16未満04_他_所得金額 = @DependentsUnder16_4_OtherIncome"
							//2023-11-20 iwai-tamura upd end -----

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
						DbHelper.AddDbParameter(cmd, "@Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@HouseholdSelfCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@HouseholdName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@HouseholdName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PostalCode_1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PostalCode_2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SpouseCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_ResidentType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_OldmanType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_SpecificType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_ResidentType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_OldmanType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_SpecificType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_ResidentType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_OldmanType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_SpecificType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_ResidentType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_OldmanType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_SpecificType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_ResidentType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_Subject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_GeneralHandicappedSelfCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_GeneralHandicappedSpouseCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_GeneralHandicappedDependentsCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_GeneralHandicappedDependentsNumber", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_SpecialHandicappedSelfCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_SpecialHandicappedSpouseCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_SpecialHandicappedDependentsCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_SpecialHandicappedDependentsNumber", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_LivingHandicappedSpouseCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_LivingHandicappedDependentsCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_LivingHandicappedDependentsNumber", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_WidowType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_WidowReasonType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_WidowOccurrenceDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_StudentCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsOther_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_AddressSameCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_AbroadCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_AddressSameCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_AbroadCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_AddressSameCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_AbroadCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_TransferComment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_notSubject", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Birthday", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_AddressSameCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_AbroadCheck", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_TransferDate", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_TransferComment", DbType.String);

						//2023-11-20 iwai-tamura upd str -----
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_1_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_2_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_3_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsOver16_4_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_1_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_2_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_3_OtherIncome", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Earnings", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_Earnings2Income", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@DependentsUnder16_4_OtherIncome", DbType.Int32);
						//2023-11-20 iwai-tamura upd end -----
                        DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
						DbHelper.AddDbParameter(cmd, "@TaxWithholding_RelationshipType", DbType.String);
						//2025-11-18 iwai-tamura upd-str ------
						DbHelper.AddDbParameter(cmd, "@Address02", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AddressType", DbType.String);
						//2025-11-18 iwai-tamura upd-end ------
                        
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
                        parameters[8].Value = DataConv.IfNull(addYMD(model.Head.BirthdayYear,model.Head.BirthdayMonth,model.Head.BirthdayDay));
						parameters[9].Value = DataConv.IfNull(checkValue(model.Head.HouseholdSelfCheck));
						parameters[10].Value = DataConv.IfNull(model.Head.HouseholdName1);
						parameters[11].Value = DataConv.IfNull(model.Head.HouseholdName2);
						parameters[12].Value = DataConv.IfNull(model.Head.RelationshipType);
						parameters[13].Value = DataConv.IfNull(model.Head.PostalCode_1);
						parameters[14].Value = DataConv.IfNull(model.Head.PostalCode_2);
						parameters[15].Value = DataConv.IfNull(model.Head.Address);
						parameters[16].Value = DataConv.IfNull(model.Head.SpouseCheck);
						parameters[17].Value = DataConv.IfNull(checkValue(model.Head.TaxWithholding_notSubject));
						parameters[18].Value = DataConv.IfNull(model.Head.TaxWithholding_Name1);
						parameters[19].Value = DataConv.IfNull(model.Head.TaxWithholding_Name2);
						parameters[20].Value = DataConv.IfNull(model.Head.TaxWithholding_Kana1);
						parameters[21].Value = DataConv.IfNull(model.Head.TaxWithholding_Kana2);
                        parameters[22].Value = DataConv.IfNull(addYMD(model.Head.TaxWithholding_BirthdayYear,model.Head.TaxWithholding_BirthdayMonth,model.Head.TaxWithholding_BirthdayDay));
						parameters[23].Value = DataConv.IfNull(model.Head.TaxWithholding_Income.ToString());
						parameters[24].Value = DataConv.IfNull(model.Head.TaxWithholding_ResidentType);
						parameters[25].Value = DataConv.IfNull(model.Head.TaxWithholding_Address);
						parameters[26].Value = DataConv.IfNull(changeDate(model.Head.TaxWithholding_TransferDate));
						parameters[27].Value = DataConv.IfNull(model.Head.TaxWithholding_TransferComment);
						parameters[28].Value = DataConv.IfNull(checkValue(model.Head.DependentsOver16_1_notSubject));
						parameters[29].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Name1);
						parameters[30].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Name2);
						parameters[31].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Kana1);
						parameters[32].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Kana2);
						parameters[33].Value = DataConv.IfNull(model.Head.DependentsOver16_1_RelationshipType);
                        parameters[34].Value = DataConv.IfNull(addYMD(model.Head.DependentsOver16_1_BirthdayYear,model.Head.DependentsOver16_1_BirthdayMonth,model.Head.DependentsOver16_1_BirthdayDay));
						parameters[35].Value = DataConv.IfNull(model.Head.DependentsOver16_1_OldmanType);
						parameters[36].Value = DataConv.IfNull(model.Head.DependentsOver16_1_SpecificType);
						parameters[37].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Income.ToString());
						parameters[38].Value = DataConv.IfNull(model.Head.DependentsOver16_1_ResidentType);
						parameters[39].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Address);
						parameters[40].Value = DataConv.IfNull(changeDate(model.Head.DependentsOver16_1_TransferDate));
						parameters[41].Value = DataConv.IfNull(model.Head.DependentsOver16_1_TransferComment);
						parameters[42].Value = DataConv.IfNull(checkValue(model.Head.DependentsOver16_2_notSubject));
						parameters[43].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Name1);
						parameters[44].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Name2);
						parameters[45].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Kana1);
						parameters[46].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Kana2);
						parameters[47].Value = DataConv.IfNull(model.Head.DependentsOver16_2_RelationshipType);
                        parameters[48].Value = DataConv.IfNull(addYMD(model.Head.DependentsOver16_2_BirthdayYear,model.Head.DependentsOver16_2_BirthdayMonth,model.Head.DependentsOver16_2_BirthdayDay));
						parameters[49].Value = DataConv.IfNull(model.Head.DependentsOver16_2_OldmanType);
						parameters[50].Value = DataConv.IfNull(model.Head.DependentsOver16_2_SpecificType);
						parameters[51].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Income.ToString());
						parameters[52].Value = DataConv.IfNull(model.Head.DependentsOver16_2_ResidentType);
						parameters[53].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Address);
						parameters[54].Value = DataConv.IfNull(changeDate(model.Head.DependentsOver16_2_TransferDate));
						parameters[55].Value = DataConv.IfNull(model.Head.DependentsOver16_2_TransferComment);
						parameters[56].Value = DataConv.IfNull(checkValue(model.Head.DependentsOver16_3_notSubject));
						parameters[57].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Name1);
						parameters[58].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Name2);
						parameters[59].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Kana1);
						parameters[60].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Kana2);
						parameters[61].Value = DataConv.IfNull(model.Head.DependentsOver16_3_RelationshipType);
                        parameters[62].Value = DataConv.IfNull(addYMD(model.Head.DependentsOver16_3_BirthdayYear,model.Head.DependentsOver16_3_BirthdayMonth,model.Head.DependentsOver16_3_BirthdayDay));
						parameters[63].Value = DataConv.IfNull(model.Head.DependentsOver16_3_OldmanType);
						parameters[64].Value = DataConv.IfNull(model.Head.DependentsOver16_3_SpecificType);
						parameters[65].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Income.ToString());
						parameters[66].Value = DataConv.IfNull(model.Head.DependentsOver16_3_ResidentType);
						parameters[67].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Address);
						parameters[68].Value = DataConv.IfNull(changeDate(model.Head.DependentsOver16_3_TransferDate));
						parameters[69].Value = DataConv.IfNull(model.Head.DependentsOver16_3_TransferComment);
						parameters[70].Value = DataConv.IfNull(checkValue(model.Head.DependentsOver16_4_notSubject));
						parameters[71].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Name1);
						parameters[72].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Name2);
						parameters[73].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Kana1);
						parameters[74].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Kana2);
						parameters[75].Value = DataConv.IfNull(model.Head.DependentsOver16_4_RelationshipType);
                        parameters[76].Value = DataConv.IfNull(addYMD(model.Head.DependentsOver16_4_BirthdayYear,model.Head.DependentsOver16_4_BirthdayMonth,model.Head.DependentsOver16_4_BirthdayDay));
						parameters[77].Value = DataConv.IfNull(model.Head.DependentsOver16_4_OldmanType);
						parameters[78].Value = DataConv.IfNull(model.Head.DependentsOver16_4_SpecificType);
						parameters[79].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Income.ToString());
						parameters[80].Value = DataConv.IfNull(model.Head.DependentsOver16_4_ResidentType);
						parameters[81].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Address);
						parameters[82].Value = DataConv.IfNull(changeDate(model.Head.DependentsOver16_4_TransferDate));
						parameters[83].Value = DataConv.IfNull(model.Head.DependentsOver16_4_TransferComment);
						parameters[84].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_Subject));
						parameters[85].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_GeneralHandicappedSelfCheck));
						parameters[86].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_GeneralHandicappedSpouseCheck));
						parameters[87].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_GeneralHandicappedDependentsCheck));
						parameters[88].Value = DataConv.IfNull(model.Head.DependentsOther_GeneralHandicappedDependentsNumber);
						parameters[89].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_SpecialHandicappedSelfCheck));
						parameters[90].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_SpecialHandicappedSpouseCheck));
						parameters[91].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_SpecialHandicappedDependentsCheck));
						parameters[92].Value = DataConv.IfNull(model.Head.DependentsOther_SpecialHandicappedDependentsNumber);
						parameters[93].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_LivingHandicappedSpouseCheck));
						parameters[94].Value = DataConv.IfNull(checkValue(model.Head.DependentsOther_LivingHandicappedDependentsCheck));
						parameters[95].Value = DataConv.IfNull(model.Head.DependentsOther_LivingHandicappedDependentsNumber);
						parameters[96].Value = DataConv.IfNull(model.Head.DependentsOther_WidowType);
						parameters[97].Value = DataConv.IfNull(model.Head.DependentsOther_WidowReasonType);
						parameters[98].Value = DataConv.IfNull(changeDate(model.Head.DependentsOther_WidowOccurrenceDate));
						parameters[99].Value = DataConv.IfNull(checkValueKin(model.Head.DependentsOther_StudentCheck));
						parameters[100].Value = DataConv.IfNull(changeDate(model.Head.DependentsOther_TransferDate));
						parameters[101].Value = DataConv.IfNull(model.Head.DependentsOther_TransferComment);
						parameters[102].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_1_notSubject));
						parameters[103].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Name1);
						parameters[104].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Name2);
						parameters[105].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Kana1);
						parameters[106].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Kana2);
						parameters[107].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_RelationshipType);
                        parameters[108].Value = DataConv.IfNull(addYMD(model.Head.DependentsUnder16_1_BirthdayYear,model.Head.DependentsUnder16_1_BirthdayMonth,model.Head.DependentsUnder16_1_BirthdayDay));
						parameters[109].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_1_AddressSameCheck));
						parameters[110].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Address);
						parameters[111].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_1_AbroadCheck));
						parameters[112].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Income.ToString());
						parameters[113].Value = DataConv.IfNull(changeDate(model.Head.DependentsUnder16_1_TransferDate));
						parameters[114].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_TransferComment);
						parameters[115].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_2_notSubject));
						parameters[116].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Name1);
						parameters[117].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Name2);
						parameters[118].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Kana1);
						parameters[119].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Kana2);
						parameters[120].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_RelationshipType);
                        parameters[121].Value = DataConv.IfNull(addYMD(model.Head.DependentsUnder16_2_BirthdayYear,model.Head.DependentsUnder16_2_BirthdayMonth,model.Head.DependentsUnder16_2_BirthdayDay));
						parameters[122].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_2_AddressSameCheck));
						parameters[123].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Address);
						parameters[124].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_2_AbroadCheck));
						parameters[125].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Income.ToString());
						parameters[126].Value = DataConv.IfNull(changeDate(model.Head.DependentsUnder16_2_TransferDate));
						parameters[127].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_TransferComment);
						parameters[128].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_3_notSubject));
						parameters[129].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Name1);
						parameters[130].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Name2);
						parameters[131].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Kana1);
						parameters[132].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Kana2);
						parameters[133].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_RelationshipType);
                        parameters[134].Value = DataConv.IfNull(addYMD(model.Head.DependentsUnder16_3_BirthdayYear,model.Head.DependentsUnder16_3_BirthdayMonth,model.Head.DependentsUnder16_3_BirthdayDay));
						parameters[135].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_3_AddressSameCheck));
						parameters[136].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Address);
						parameters[137].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_3_AbroadCheck));
						parameters[138].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Income.ToString());
						parameters[139].Value = DataConv.IfNull(changeDate(model.Head.DependentsUnder16_3_TransferDate));
						parameters[140].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_TransferComment);
						parameters[141].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_4_notSubject));
						parameters[142].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Name1);
						parameters[143].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Name2);
						parameters[144].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Kana1);
						parameters[145].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Kana2);
						parameters[146].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_RelationshipType);
                        parameters[147].Value = DataConv.IfNull(addYMD(model.Head.DependentsUnder16_4_BirthdayYear,model.Head.DependentsUnder16_4_BirthdayMonth,model.Head.DependentsUnder16_4_BirthdayDay));
						parameters[148].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_4_AddressSameCheck));
						parameters[149].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Address);
						parameters[150].Value = DataConv.IfNull(checkValue(model.Head.DependentsUnder16_4_AbroadCheck));
						parameters[151].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Income.ToString());
						parameters[152].Value = DataConv.IfNull(changeDate(model.Head.DependentsUnder16_4_TransferDate));
						parameters[153].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_TransferComment);

						//2023-11-20 iwai-tamura upd str -----
						parameters[154].Value = DataConv.IfNull(model.Head.TaxWithholding_Earnings.ToString());
						parameters[155].Value = DataConv.IfNull(model.Head.TaxWithholding_Earnings2Income.ToString());
						parameters[156].Value = DataConv.IfNull(model.Head.TaxWithholding_OtherIncome.ToString());
						parameters[157].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Earnings.ToString());
						parameters[158].Value = DataConv.IfNull(model.Head.DependentsOver16_1_Earnings2Income.ToString());
						parameters[159].Value = DataConv.IfNull(model.Head.DependentsOver16_1_OtherIncome.ToString());
						parameters[160].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Earnings.ToString());
						parameters[161].Value = DataConv.IfNull(model.Head.DependentsOver16_2_Earnings2Income.ToString());
						parameters[162].Value = DataConv.IfNull(model.Head.DependentsOver16_2_OtherIncome.ToString());
						parameters[163].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Earnings.ToString());
						parameters[164].Value = DataConv.IfNull(model.Head.DependentsOver16_3_Earnings2Income.ToString());
						parameters[165].Value = DataConv.IfNull(model.Head.DependentsOver16_3_OtherIncome.ToString());
						parameters[166].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Earnings.ToString());
						parameters[167].Value = DataConv.IfNull(model.Head.DependentsOver16_4_Earnings2Income.ToString());
						parameters[168].Value = DataConv.IfNull(model.Head.DependentsOver16_4_OtherIncome.ToString());
						parameters[169].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Earnings.ToString());
						parameters[170].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_Earnings2Income.ToString());
						parameters[171].Value = DataConv.IfNull(model.Head.DependentsUnder16_1_OtherIncome.ToString());
						parameters[172].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Earnings.ToString());
						parameters[173].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_Earnings2Income.ToString());
						parameters[174].Value = DataConv.IfNull(model.Head.DependentsUnder16_2_OtherIncome.ToString());
						parameters[175].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Earnings.ToString());
						parameters[176].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_Earnings2Income.ToString());
						parameters[177].Value = DataConv.IfNull(model.Head.DependentsUnder16_3_OtherIncome.ToString());
						parameters[178].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Earnings.ToString());
						parameters[179].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_Earnings2Income.ToString());
						parameters[180].Value = DataConv.IfNull(model.Head.DependentsUnder16_4_OtherIncome.ToString());
						//2023-11-20 iwai-tamura upd end -----

                        parameters[181].Value = DataConv.IfNull(model.Head.SheetYear.ToString());
                        parameters[182].Value = DataConv.IfNull(model.Head.EmployeeNo);
                        parameters[183].Value = DataConv.IfNull(model.Head.TaxWithholding_RelationshipType);
                        
						//2025-11-18 iwai-tamura upd-str ------
                        parameters[184].Value = DataConv.IfNull(model.Head.Address02);
                        parameters[185].Value = DataConv.IfNull(model.Head.AddressType);
						//2025-11-18 iwai-tamura upd-end ------
                        

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
        public void Sign(HuyouDeclareRegisterViewModels model, string strDepartmentNo, LoginUser lu, bool isSign = true) {
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
                    //扶養控除申告書承認
                    var sql = "update TE100扶養控除申告書Data"
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

        //2025-11-18 iwai-tamura upd-str ------
        /// <summary>
        /// 住所区分一覧取得
        /// </summary>
        public IList<AddressEntry> GetAddressMaster() {
			var list = new List<AddressEntry>();
			//従業員の当日の勤務状況データを取得
			var sql = "";
			sql = " SELECT "
				+ "     TM住所.住所区分Code"
				+ "     ,TM住所.都道府県"
				+ "     ,TM住所.市区町村"
				+ "     ,TM住所.自治体番号"
				+ " FROM TM901住所区分Master TM住所 "
				+ " ORDER BY TM住所.住所区分Code   ";

			IList<SelectListItem> itemList = new List<SelectListItem>();
			using (DbManager dm = new DbManager())
			using (IDbCommand cmd = dm.CreateCommand(sql))
			using (DataSet ds = new DataSet()) {
				IDataAdapter da = dm.CreateSqlDataAdapter(cmd);

				// データセットに設定する
				da.Fill(ds);
				foreach (DataRow row in ds.Tables[0].Rows) {
					list.Add(new AddressEntry {
						Code = row["住所区分Code"].ToString(),
						Prefecture = row["都道府県"].ToString(),
						City = row["市区町村"].ToString(),
						JichitaiNo = row["自治体番号"].ToString()
					});
				}
			}
			return list;
        }
        //2025-11-18 iwai-tamura upd-end ------
    }
}
