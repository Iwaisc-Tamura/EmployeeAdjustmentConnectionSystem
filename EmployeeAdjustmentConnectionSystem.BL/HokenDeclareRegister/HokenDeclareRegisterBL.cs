﻿using System;
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

namespace EmployeeAdjustmentConnectionSystem.BL.HokenDeclareRegister {
    /// <summary>
    /// 扶養控除申告書入力ビジネスロジック
    /// </summary>
    public class HokenDeclareRegisterBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public HokenDeclareRegisterBL() {
        }


        /// <summary>
        /// 扶養控除申告書データ取得
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <returns>扶養控除申告書モデル</returns>
        public HokenDeclareRegisterViewModels Select(int? intSheetYear,string strEmployeeNo,bool bolAdminMode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                HokenDeclareRegisterViewModels model = new HokenDeclareRegisterViewModels();
                model.Head = new HokenDeclareRegisterHeaderModel();
                model.Body = new HokenDeclareRegisterBodyModel();

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

                    var sql = "SELECT * FROM TE110保険料控除申告書Data WHERE 対象年度 = @SheetYear and 社員番号 = @EmployeeNo ";

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
                            model.Head = new HokenDeclareRegisterHeaderModel {
								SheetYear = DataConv.IntParse(row["対象年度"].ToString()),
								ApprovalType = row["本人確定区分"].ToString(),
								DecisionType = row["管理者確定区分"].ToString(),
								EmployeeNo = row["社員番号"].ToString(),
								DepartmentNo = DataConv.IntParse(row["所属番号"].ToString()),
								Name1 = row["氏名_姓"].ToString(),
								Name2 = row["氏名_名"].ToString(),
								Kana1 = row["Kana_姓"].ToString(),
								Kana2 = row["Kana_名"].ToString(),
								Address = row["住所01"].ToString(),
                                AllLifeInsurance_DeductionAmount = setMoney(row["生命保険料控除額計"].ToString()),
								LifeInsurance_NewTotalAmount = setMoney(row["一般生命保険料新保険料合計"].ToString()),
								LifeInsurance_OldTotalAmount = setMoney(row["一般生命保険料旧保険料合計"].ToString()),
								LifeInsurance_Calc1 = setMoney(row["一般生命保険料新保険料表計算"].ToString()),
								LifeInsurance_Calc2 = setMoney(row["一般生命保険料旧保険料表計算"].ToString()),
								LifeInsurance_TotalAmount = setMoney(row["一般生命保険料表合計"].ToString()),
								LifeInsurance_DeductionAmount = setMoney(row["一般生命保険料比較"].ToString()),
								LifeInsurance_1_InsuranceCompanyName = row["一般生命保険料01_保険会社等名称"].ToString(),
								LifeInsurance_1_InsuranceTypeName = row["一般生命保険料01_保険等種類"].ToString(),
								LifeInsurance_1_InsurancePeriod = row["一般生命保険料01_期間"].ToString(),
								LifeInsurance_1_ContractorName1 = row["一般生命保険料01_保険等契約者氏名_姓"].ToString(),
								LifeInsurance_1_ContractorName2 = row["一般生命保険料01_保険等契約者氏名_名"].ToString(),
								LifeInsurance_1_ReceiverName1 = row["一般生命保険料01_保険金等受取人氏名_姓"].ToString(),
								LifeInsurance_1_ReceiverName2 = row["一般生命保険料01_保険金等受取人氏名_名"].ToString(),
								LifeInsurance_1_RelationshipType = row["一般生命保険料01_保険金等受取人続柄"].ToString(),
								LifeInsurance_1_OldAndNewType = row["一般生命保険料01_新旧"].ToString(),
								LifeInsurance_1_InsuranceFee = setMoney(row["一般生命保険料01_支払金額"].ToString()),
								LifeInsurance_2_InsuranceCompanyName = row["一般生命保険料02_保険会社等名称"].ToString(),
								LifeInsurance_2_InsuranceTypeName = row["一般生命保険料02_保険等種類"].ToString(),
								LifeInsurance_2_InsurancePeriod = row["一般生命保険料02_期間"].ToString(),
								LifeInsurance_2_ContractorName1 = row["一般生命保険料02_保険等契約者氏名_姓"].ToString(),
								LifeInsurance_2_ContractorName2 = row["一般生命保険料02_保険等契約者氏名_名"].ToString(),
								LifeInsurance_2_ReceiverName1 = row["一般生命保険料02_保険金等受取人氏名_姓"].ToString(),
								LifeInsurance_2_ReceiverName2 = row["一般生命保険料02_保険金等受取人氏名_名"].ToString(),
								LifeInsurance_2_RelationshipType = row["一般生命保険料02_保険金等受取人続柄"].ToString(),
								LifeInsurance_2_OldAndNewType = row["一般生命保険料02_新旧"].ToString(),
								LifeInsurance_2_InsuranceFee = setMoney(row["一般生命保険料02_支払金額"].ToString()),
								LifeInsurance_3_InsuranceCompanyName = row["一般生命保険料03_保険会社等名称"].ToString(),
								LifeInsurance_3_InsuranceTypeName = row["一般生命保険料03_保険等種類"].ToString(),
								LifeInsurance_3_InsurancePeriod = row["一般生命保険料03_期間"].ToString(),
								LifeInsurance_3_ContractorName1 = row["一般生命保険料03_保険等契約者氏名_姓"].ToString(),
								LifeInsurance_3_ContractorName2 = row["一般生命保険料03_保険等契約者氏名_名"].ToString(),
								LifeInsurance_3_ReceiverName1 = row["一般生命保険料03_保険金等受取人氏名_姓"].ToString(),
								LifeInsurance_3_ReceiverName2 = row["一般生命保険料03_保険金等受取人氏名_名"].ToString(),
								LifeInsurance_3_RelationshipType = row["一般生命保険料03_保険金等受取人続柄"].ToString(),
								LifeInsurance_3_OldAndNewType = row["一般生命保険料03_新旧"].ToString(),
								LifeInsurance_3_InsuranceFee = setMoney(row["一般生命保険料03_支払金額"].ToString()),
								LifeInsurance_4_InsuranceCompanyName = row["一般生命保険料04_保険会社等名称"].ToString(),
								LifeInsurance_4_InsuranceTypeName = row["一般生命保険料04_保険等種類"].ToString(),
								LifeInsurance_4_InsurancePeriod = row["一般生命保険料04_期間"].ToString(),
								LifeInsurance_4_ContractorName1 = row["一般生命保険料04_保険等契約者氏名_姓"].ToString(),
								LifeInsurance_4_ContractorName2 = row["一般生命保険料04_保険等契約者氏名_名"].ToString(),
								LifeInsurance_4_ReceiverName1 = row["一般生命保険料04_保険金等受取人氏名_姓"].ToString(),
								LifeInsurance_4_ReceiverName2 = row["一般生命保険料04_保険金等受取人氏名_名"].ToString(),
								LifeInsurance_4_RelationshipType = row["一般生命保険料04_保険金等受取人続柄"].ToString(),
								LifeInsurance_4_OldAndNewType = row["一般生命保険料04_新旧"].ToString(),
								LifeInsurance_4_InsuranceFee = setMoney(row["一般生命保険料04_支払金額"].ToString()),
								MedicalInsurance_TotalAmount = setMoney(row["介護医療保険料合計"].ToString()),
								MedicalInsurance_DeductionAmount = setMoney(row["介護医療保険料表計算"].ToString()),
								MedicalInsurance_1_InsuranceCompanyName = row["介護医療保険料01_会社等名称"].ToString(),
								MedicalInsurance_1_InsuranceTypeName = row["介護医療保険料01_保険等種類"].ToString(),
								MedicalInsurance_1_InsurancePeriod = row["介護医療保険料01_期間"].ToString(),
								MedicalInsurance_1_ContractorName1 = row["介護医療保険料01_保険等契約者氏名_姓"].ToString(),
								MedicalInsurance_1_ContractorName2 = row["介護医療保険料01_保険等契約者氏名_名"].ToString(),
								MedicalInsurance_1_ReceiverName1 = row["介護医療保険料01_保険金等受取人氏名_姓"].ToString(),
								MedicalInsurance_1_ReceiverName2 = row["介護医療保険料01_保険金等受取人氏名_名"].ToString(),
								MedicalInsurance_1_RelationshipType = row["介護医療保険料01_保険金等受取人続柄"].ToString(),
								MedicalInsurance_1_InsuranceFee = setMoney(row["介護医療保険料01_支払金額"].ToString()),
								MedicalInsurance_2_InsuranceCompanyName = row["介護医療保険料02_会社等名称"].ToString(),
								MedicalInsurance_2_InsuranceTypeName = row["介護医療保険料02_保険等種類"].ToString(),
								MedicalInsurance_2_InsurancePeriod = row["介護医療保険料02_期間"].ToString(),
								MedicalInsurance_2_ContractorName1 = row["介護医療保険料02_保険等契約者氏名_姓"].ToString(),
								MedicalInsurance_2_ContractorName2 = row["介護医療保険料02_保険等契約者氏名_名"].ToString(),
								MedicalInsurance_2_ReceiverName1 = row["介護医療保険料02_保険金等受取人氏名_姓"].ToString(),
								MedicalInsurance_2_ReceiverName2 = row["介護医療保険料02_保険金等受取人氏名_名"].ToString(),
								MedicalInsurance_2_RelationshipType = row["介護医療保険料02_保険金等受取人続柄"].ToString(),
								MedicalInsurance_2_InsuranceFee = setMoney(row["介護医療保険料02_支払金額"].ToString()),
								PensionInsurance_NewTotalAmount = setMoney(row["個人年金保険料新保険料合計"].ToString()),
								PensionInsurance_OldTotalAmount = setMoney(row["個人年金保険料旧保険料合計"].ToString()),
								PensionInsurance_Calc1 = setMoney(row["個人年金保険料新保険料表計算"].ToString()),
								PensionInsurance_Calc2 = setMoney(row["個人年金保険料旧保険料表計算"].ToString()),
								PensionInsurance_TotalAmount = setMoney(row["個人年金保険料表合計"].ToString()),
								PensionInsurance_DeductionAmount = setMoney(row["個人年金保険料比較"].ToString()),
								PensionInsurance_1_InsuranceCompanyName = row["個人年金保険料01_会社等名称"].ToString(),
								PensionInsurance_1_InsuranceTypeName = row["個人年金保険料01_保険等種類"].ToString(),
								PensionInsurance_1_InsurancePeriod = row["個人年金保険料01_期間"].ToString(),
								PensionInsurance_1_ContractorName1 = row["個人年金保険料01_保険等契約者氏名_姓"].ToString(),
								PensionInsurance_1_ContractorName2 = row["個人年金保険料01_保険等契約者氏名_名"].ToString(),
								PensionInsurance_1_ReceiverName1 = row["個人年金保険料01_保険金等受取人氏名_姓"].ToString(),
								PensionInsurance_1_ReceiverName2 = row["個人年金保険料01_保険金等受取人氏名_名"].ToString(),
								PensionInsurance_1_RelationshipType = row["個人年金保険料01_保険金等受取人続柄"].ToString(),
								PensionInsurance_1_StartPayment = row["個人年金保険料01_支払開始日"].ToString(),
                                PensionInsurance_1_StartPaymentYear = row["個人年金保険料01_支払開始日"].ToString()=="" ? "":row["個人年金保険料01_支払開始日"].ToString().Substring(0,4),
                                PensionInsurance_1_StartPaymentMonth = row["個人年金保険料01_支払開始日"].ToString()=="" ? "":row["個人年金保険料01_支払開始日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                PensionInsurance_1_StartPaymentDay = row["個人年金保険料01_支払開始日"].ToString()=="" ? "":row["個人年金保険料01_支払開始日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

                                PensionInsurance_1_OldAndNewType = row["個人年金保険料01_新旧"].ToString(),
								PensionInsurance_1_InsuranceFee = setMoney(row["個人年金保険料01_支払金額"].ToString()),
								PensionInsurance_2_InsuranceCompanyName = row["個人年金保険料02_会社等名称"].ToString(),
								PensionInsurance_2_InsuranceTypeName = row["個人年金保険料02_保険等種類"].ToString(),
								PensionInsurance_2_InsurancePeriod = row["個人年金保険料02_期間"].ToString(),
								PensionInsurance_2_ContractorName1 = row["個人年金保険料02_保険等契約者氏名_姓"].ToString(),
								PensionInsurance_2_ContractorName2 = row["個人年金保険料02_保険等契約者氏名_名"].ToString(),
								PensionInsurance_2_ReceiverName1 = row["個人年金保険料02_保険金等受取人氏名_姓"].ToString(),
								PensionInsurance_2_ReceiverName2 = row["個人年金保険料02_保険金等受取人氏名_名"].ToString(),
								PensionInsurance_2_RelationshipType = row["個人年金保険料02_保険金等受取人続柄"].ToString(),
								PensionInsurance_2_StartPayment = row["個人年金保険料02_支払開始日"].ToString(),
                                PensionInsurance_2_StartPaymentYear = row["個人年金保険料02_支払開始日"].ToString()=="" ? "":row["個人年金保険料02_支払開始日"].ToString().Substring(0,4),
                                PensionInsurance_2_StartPaymentMonth = row["個人年金保険料02_支払開始日"].ToString()=="" ? "":row["個人年金保険料02_支払開始日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                PensionInsurance_2_StartPaymentDay = row["個人年金保険料02_支払開始日"].ToString()=="" ? "":row["個人年金保険料02_支払開始日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

								PensionInsurance_2_OldAndNewType = row["個人年金保険料02_新旧"].ToString(),
								PensionInsurance_2_InsuranceFee = setMoney(row["個人年金保険料02_支払金額"].ToString()),
								PensionInsurance_3_InsuranceCompanyName = row["個人年金保険料03_会社等名称"].ToString(),
								PensionInsurance_3_InsuranceTypeName = row["個人年金保険料03_保険等種類"].ToString(),
								PensionInsurance_3_InsurancePeriod = row["個人年金保険料03_期間"].ToString(),
								PensionInsurance_3_ContractorName1 = row["個人年金保険料03_保険等契約者氏名_姓"].ToString(),
								PensionInsurance_3_ContractorName2 = row["個人年金保険料03_保険等契約者氏名_名"].ToString(),
								PensionInsurance_3_ReceiverName1 = row["個人年金保険料03_保険金等受取人氏名_姓"].ToString(),
								PensionInsurance_3_ReceiverName2 = row["個人年金保険料03_保険金等受取人氏名_名"].ToString(),
								PensionInsurance_3_RelationshipType = row["個人年金保険料03_保険金等受取人続柄"].ToString(),
								PensionInsurance_3_StartPayment = row["個人年金保険料03_支払開始日"].ToString(),
                                PensionInsurance_3_StartPaymentYear = row["個人年金保険料03_支払開始日"].ToString()=="" ? "":row["個人年金保険料03_支払開始日"].ToString().Substring(0,4),
                                PensionInsurance_3_StartPaymentMonth = row["個人年金保険料03_支払開始日"].ToString()=="" ? "":row["個人年金保険料03_支払開始日"].ToString().Substring(4,2).TrimStart(new Char[] { '0' } ),
                                PensionInsurance_3_StartPaymentDay = row["個人年金保険料03_支払開始日"].ToString()=="" ? "":row["個人年金保険料03_支払開始日"].ToString().Substring(6,2).TrimStart(new Char[] { '0' } ),

								PensionInsurance_3_OldAndNewType = row["個人年金保険料03_新旧"].ToString(),
								PensionInsurance_3_InsuranceFee = setMoney(row["個人年金保険料03_支払金額"].ToString()),
								QuakeInsurance_QuakeAmount = setMoney(row["地震保険料控除地震保険料合計"].ToString()),
								QuakeInsurance_DamageTotalAmount = setMoney(row["地震保険料控除旧長期損害保険料合計"].ToString()),
								QuakeInsurance_Calc1 = setMoney(row["地震保険料控除額金額01"].ToString()),
								QuakeInsurance_Calc2 = setMoney(row["地震保険料控除額金額02"].ToString()),
								QuakeInsurance_DeductionAmount = setMoney(row["地震保険料控除額金額合計"].ToString()),
								QuakeInsurance_1_InsuranceCompanyName = row["地震保険料控除01_会社等名称"].ToString(),
								QuakeInsurance_1_InsuranceTypeName = row["地震保険料控除01_保険等種類"].ToString(),
								QuakeInsurance_1_InsurancePeriod = row["地震保険料控除01_期間"].ToString(),
								QuakeInsurance_1_ContractorName1 = row["地震保険料控除01_保険等契約者氏名_姓"].ToString(),
								QuakeInsurance_1_ContractorName2 = row["地震保険料控除01_保険等契約者氏名_名"].ToString(),
								QuakeInsurance_1_ReceiverName1 = row["地震保険料控除01_保険等対象氏名_姓"].ToString(),
								QuakeInsurance_1_ReceiverName2 = row["地震保険料控除01_保険等対象氏名_名"].ToString(),
								QuakeInsurance_1_RelationshipType = row["地震保険料控除01_保険等対象続柄"].ToString(),
								QuakeInsurance_1_QuakeAndDamageType = row["地震保険料控除01_地震旧長期"].ToString(),
								QuakeInsurance_1_InsuranceFee = setMoney(row["地震保険料控除01_支払保険料"].ToString()),
								QuakeInsurance_2_InsuranceCompanyName = row["地震保険料控除02_会社等名称"].ToString(),
								QuakeInsurance_2_InsuranceTypeName = row["地震保険料控除02_保険等種類"].ToString(),
								QuakeInsurance_2_InsurancePeriod = row["地震保険料控除02_期間"].ToString(),
								QuakeInsurance_2_ContractorName1 = row["地震保険料控除02_保険等契約者氏名_姓"].ToString(),
								QuakeInsurance_2_ContractorName2 = row["地震保険料控除02_保険等契約者氏名_名"].ToString(),
								QuakeInsurance_2_ReceiverName1 = row["地震保険料控除02_保険等対象氏名_姓"].ToString(),
								QuakeInsurance_2_ReceiverName2 = row["地震保険料控除02_保険等対象氏名_名"].ToString(),
								QuakeInsurance_2_RelationshipType = row["地震保険料控除02_保険等対象続柄"].ToString(),
								QuakeInsurance_2_QuakeAndDamageType = row["地震保険料控除02_地震旧長期"].ToString(),
								QuakeInsurance_2_InsuranceFee = setMoney(row["地震保険料控除02_支払保険料"].ToString()),
								SocialInsurance_DeductionAmount = setMoney(row["社会保険料控除合計"].ToString()),
								SocialInsurance_1_InsuranceTypeName = row["社会保険料控除01_社会保険種類"].ToString(),
								SocialInsurance_1_InsuranceCompanyName = row["社会保険料控除01_支払先名称"].ToString(),
								SocialInsurance_1_ContractorName1 = row["社会保険料控除01_負担者氏名_姓"].ToString(),
								SocialInsurance_1_ContractorName2 = row["社会保険料控除01_負担者氏名_名"].ToString(),
								SocialInsurance_1_RelationshipType = row["社会保険料控除01_負担者続柄"].ToString(),
								SocialInsurance_1_InsuranceFee = setMoney(row["社会保険料控除01_支払保険料"].ToString()),
								SocialInsurance_2_InsuranceTypeName = row["社会保険料控除02_社会保険種類"].ToString(),
								SocialInsurance_2_InsuranceCompanyName = row["社会保険料控除02_支払先名称"].ToString(),
								SocialInsurance_2_ContractorName1 = row["社会保険料控除02_負担者氏名_姓"].ToString(),
								SocialInsurance_2_ContractorName2 = row["社会保険料控除02_負担者氏名_名"].ToString(),
								SocialInsurance_2_RelationshipType = row["社会保険料控除02_負担者続柄"].ToString(),
								SocialInsurance_2_InsuranceFee = setMoney(row["社会保険料控除02_支払保険料"].ToString()),
								SmallScaleMutualAid_MutualAidCost = setMoney(row["共済契約掛金"].ToString()),
								SmallScaleMutualAid_CorporatePensionCost = setMoney(row["企業型年金加入者掛金"].ToString()),
								SmallScaleMutualAid_PersonalPensionCost = setMoney(row["個人型年金加入者掛金"].ToString()),
								SmallScaleMutualAid_HandicappedMutualAidCost = setMoney(row["心身障害者扶養共済制度契約掛金"].ToString()),
								SmallScaleMutualAid_DeductionAmount = setMoney(row["小規模企業共済等掛金控除合計"].ToString()),

                            };
                            model.Head.InputMode = ajustMode.SelfInput;
                            model.Head.AdminMode = bolAdminMode;
                            ////表示用入社年月日
                            //model.Head.HireDateView = DateTime.ParseExact(model.Head.HireDate, "yyyymmdd", null).ToString("yyyy/m/d");

                            ////表示用在籍期間・現職経験
                            //int intYear = 0;
                            //int intMonth = 0;
                            //intYear = (int)(model.Head.EnrollmentMonths/12);
                            //intMonth = (int)(model.Head.EnrollmentMonths%12);
                            //if (intYear!=0){
                            //    model.Head.EnrollmentMonthsView = intYear.ToString("0年");
                            //}
                            //model.Head.EnrollmentMonthsView = model.Head.EnrollmentMonthsView + intMonth.ToString("0ヶ月");
                            
                            //intYear = (int)(model.Head.ExperienceMonths/12);
                            //intMonth = (int)(model.Head.ExperienceMonths%12);
                            //if (intYear!=0){
                            //    model.Head.ExperienceMonthsView = intYear.ToString("0年");
                            //}
                            //model.Head.ExperienceMonthsView = model.Head.ExperienceMonthsView + intMonth.ToString("0ヶ月");

                            ////表示用生年月日・年齢
                            //model.Head.BirthdayView = DateTime.ParseExact(model.Head.Birthday, "yyyymmdd", null).ToString("yyyy/m/d");
                            //model.Head.AgeView = ((int)model.Head.Age).ToString("0歳");
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
        public void SetMode(HokenDeclareRegisterViewModels model, LoginUser lu) {
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
                                //管理者登録済み
                                model.Head.InputMode = ajustMode.adminRegist;
                                break;

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
        public void Save(HokenDeclareRegisterViewModels model,LoginUser lu,string mode) {
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
                                    strDecision = "1";
                                    break;
                            }
                        }

                        //共通基本情報
                        var sql = "update TE110保険料控除申告書Data"
                            + " set "
							+ " 本人確定区分 = @ApprovalType"
							+ " ,管理者確定区分 = @DecisionType"
							+ " ,所属番号 = @DepartmentNo"
							+ " ,氏名_姓 = @Name1"
							+ " ,氏名_名 = @Name2"
							+ " ,Kana_姓 = @Kana1"
							+ " ,Kana_名 = @Kana2"
							+ " ,住所01 = @Address"
							+ " ,生命保険料控除額計 = @AllLifeInsurance_DeductionAmount"
							+ " ,一般生命保険料新保険料合計 = @LifeInsurance_NewTotalAmount"
							+ " ,一般生命保険料旧保険料合計 = @LifeInsurance_OldTotalAmount"
							+ " ,一般生命保険料新保険料表計算 = @LifeInsurance_Calc1"
							+ " ,一般生命保険料旧保険料表計算 = @LifeInsurance_Calc2"
							+ " ,一般生命保険料表合計 = @LifeInsurance_TotalAmount"
							+ " ,一般生命保険料比較 = @LifeInsurance_DeductionAmount"
							+ " ,一般生命保険料01_保険会社等名称 = @LifeInsurance_1_InsuranceCompanyName"
							+ " ,一般生命保険料01_保険等種類 = @LifeInsurance_1_InsuranceTypeName"
							+ " ,一般生命保険料01_期間 = @LifeInsurance_1_InsurancePeriod"
							+ " ,一般生命保険料01_保険等契約者氏名_姓 = @LifeInsurance_1_ContractorName1"
							+ " ,一般生命保険料01_保険等契約者氏名_名 = @LifeInsurance_1_ContractorName2"
							+ " ,一般生命保険料01_保険金等受取人氏名_姓 = @LifeInsurance_1_ReceiverName1"
							+ " ,一般生命保険料01_保険金等受取人氏名_名 = @LifeInsurance_1_ReceiverName2"
							+ " ,一般生命保険料01_保険金等受取人続柄 = @LifeInsurance_1_RelationshipType"
							+ " ,一般生命保険料01_新旧 = @LifeInsurance_1_OldAndNewType"
							+ " ,一般生命保険料01_支払金額 = @LifeInsurance_1_InsuranceFee"
							+ " ,一般生命保険料02_保険会社等名称 = @LifeInsurance_2_InsuranceCompanyName"
							+ " ,一般生命保険料02_保険等種類 = @LifeInsurance_2_InsuranceTypeName"
							+ " ,一般生命保険料02_期間 = @LifeInsurance_2_InsurancePeriod"
							+ " ,一般生命保険料02_保険等契約者氏名_姓 = @LifeInsurance_2_ContractorName1"
							+ " ,一般生命保険料02_保険等契約者氏名_名 = @LifeInsurance_2_ContractorName2"
							+ " ,一般生命保険料02_保険金等受取人氏名_姓 = @LifeInsurance_2_ReceiverName1"
							+ " ,一般生命保険料02_保険金等受取人氏名_名 = @LifeInsurance_2_ReceiverName2"
							+ " ,一般生命保険料02_保険金等受取人続柄 = @LifeInsurance_2_RelationshipType"
							+ " ,一般生命保険料02_新旧 = @LifeInsurance_2_OldAndNewType"
							+ " ,一般生命保険料02_支払金額 = @LifeInsurance_2_InsuranceFee"
							+ " ,一般生命保険料03_保険会社等名称 = @LifeInsurance_3_InsuranceCompanyName"
							+ " ,一般生命保険料03_保険等種類 = @LifeInsurance_3_InsuranceTypeName"
							+ " ,一般生命保険料03_期間 = @LifeInsurance_3_InsurancePeriod"
							+ " ,一般生命保険料03_保険等契約者氏名_姓 = @LifeInsurance_3_ContractorName1"
							+ " ,一般生命保険料03_保険等契約者氏名_名 = @LifeInsurance_3_ContractorName2"
							+ " ,一般生命保険料03_保険金等受取人氏名_姓 = @LifeInsurance_3_ReceiverName1"
							+ " ,一般生命保険料03_保険金等受取人氏名_名 = @LifeInsurance_3_ReceiverName2"
							+ " ,一般生命保険料03_保険金等受取人続柄 = @LifeInsurance_3_RelationshipType"
							+ " ,一般生命保険料03_新旧 = @LifeInsurance_3_OldAndNewType"
							+ " ,一般生命保険料03_支払金額 = @LifeInsurance_3_InsuranceFee"
							+ " ,一般生命保険料04_保険会社等名称 = @LifeInsurance_4_InsuranceCompanyName"
							+ " ,一般生命保険料04_保険等種類 = @LifeInsurance_4_InsuranceTypeName"
							+ " ,一般生命保険料04_期間 = @LifeInsurance_4_InsurancePeriod"
							+ " ,一般生命保険料04_保険等契約者氏名_姓 = @LifeInsurance_4_ContractorName1"
							+ " ,一般生命保険料04_保険等契約者氏名_名 = @LifeInsurance_4_ContractorName2"
							+ " ,一般生命保険料04_保険金等受取人氏名_姓 = @LifeInsurance_4_ReceiverName1"
							+ " ,一般生命保険料04_保険金等受取人氏名_名 = @LifeInsurance_4_ReceiverName2"
							+ " ,一般生命保険料04_保険金等受取人続柄 = @LifeInsurance_4_RelationshipType"
							+ " ,一般生命保険料04_新旧 = @LifeInsurance_4_OldAndNewType"
							+ " ,一般生命保険料04_支払金額 = @LifeInsurance_4_InsuranceFee"
							+ " ,介護医療保険料合計 = @MedicalInsurance_TotalAmount"
							+ " ,介護医療保険料表計算 = @MedicalInsurance_DeductionAmount"
							+ " ,介護医療保険料01_会社等名称 = @MedicalInsurance_1_InsuranceCompanyName"
							+ " ,介護医療保険料01_保険等種類 = @MedicalInsurance_1_InsuranceTypeName"
							+ " ,介護医療保険料01_期間 = @MedicalInsurance_1_InsurancePeriod"
							+ " ,介護医療保険料01_保険等契約者氏名_姓 = @MedicalInsurance_1_ContractorName1"
							+ " ,介護医療保険料01_保険等契約者氏名_名 = @MedicalInsurance_1_ContractorName2"
							+ " ,介護医療保険料01_保険金等受取人氏名_姓 = @MedicalInsurance_1_ReceiverName1"
							+ " ,介護医療保険料01_保険金等受取人氏名_名 = @MedicalInsurance_1_ReceiverName2"
							+ " ,介護医療保険料01_保険金等受取人続柄 = @MedicalInsurance_1_RelationshipType"
							+ " ,介護医療保険料01_支払金額 = @MedicalInsurance_1_InsuranceFee"
							+ " ,介護医療保険料02_会社等名称 = @MedicalInsurance_2_InsuranceCompanyName"
							+ " ,介護医療保険料02_保険等種類 = @MedicalInsurance_2_InsuranceTypeName"
							+ " ,介護医療保険料02_期間 = @MedicalInsurance_2_InsurancePeriod"
							+ " ,介護医療保険料02_保険等契約者氏名_姓 = @MedicalInsurance_2_ContractorName1"
							+ " ,介護医療保険料02_保険等契約者氏名_名 = @MedicalInsurance_2_ContractorName2"
							+ " ,介護医療保険料02_保険金等受取人氏名_姓 = @MedicalInsurance_2_ReceiverName1"
							+ " ,介護医療保険料02_保険金等受取人氏名_名 = @MedicalInsurance_2_ReceiverName2"
							+ " ,介護医療保険料02_保険金等受取人続柄 = @MedicalInsurance_2_RelationshipType"
							+ " ,介護医療保険料02_支払金額 = @MedicalInsurance_2_InsuranceFee"
							+ " ,個人年金保険料新保険料合計 = @PensionInsurance_NewTotalAmount"
							+ " ,個人年金保険料旧保険料合計 = @PensionInsurance_OldTotalAmount"
							+ " ,個人年金保険料新保険料表計算 = @PensionInsurance_Calc1"
							+ " ,個人年金保険料旧保険料表計算 = @PensionInsurance_Calc2"
							+ " ,個人年金保険料表合計 = @PensionInsurance_TotalAmount"
							+ " ,個人年金保険料比較 = @PensionInsurance_DeductionAmount"
							+ " ,個人年金保険料01_会社等名称 = @PensionInsurance_1_InsuranceCompanyName"
							+ " ,個人年金保険料01_保険等種類 = @PensionInsurance_1_InsuranceTypeName"
							+ " ,個人年金保険料01_期間 = @PensionInsurance_1_InsurancePeriod"
							+ " ,個人年金保険料01_保険等契約者氏名_姓 = @PensionInsurance_1_ContractorName1"
							+ " ,個人年金保険料01_保険等契約者氏名_名 = @PensionInsurance_1_ContractorName2"
							+ " ,個人年金保険料01_保険金等受取人氏名_姓 = @PensionInsurance_1_ReceiverName1"
							+ " ,個人年金保険料01_保険金等受取人氏名_名 = @PensionInsurance_1_ReceiverName2"
							+ " ,個人年金保険料01_保険金等受取人続柄 = @PensionInsurance_1_RelationshipType"
							+ " ,個人年金保険料01_支払開始日 = @PensionInsurance_1_StartPayment"
							+ " ,個人年金保険料01_新旧 = @PensionInsurance_1_OldAndNewType"
							+ " ,個人年金保険料01_支払金額 = @PensionInsurance_1_InsuranceFee"
							+ " ,個人年金保険料02_会社等名称 = @PensionInsurance_2_InsuranceCompanyName"
							+ " ,個人年金保険料02_保険等種類 = @PensionInsurance_2_InsuranceTypeName"
							+ " ,個人年金保険料02_期間 = @PensionInsurance_2_InsurancePeriod"
							+ " ,個人年金保険料02_保険等契約者氏名_姓 = @PensionInsurance_2_ContractorName1"
							+ " ,個人年金保険料02_保険等契約者氏名_名 = @PensionInsurance_2_ContractorName2"
							+ " ,個人年金保険料02_保険金等受取人氏名_姓 = @PensionInsurance_2_ReceiverName1"
							+ " ,個人年金保険料02_保険金等受取人氏名_名 = @PensionInsurance_2_ReceiverName2"
							+ " ,個人年金保険料02_保険金等受取人続柄 = @PensionInsurance_2_RelationshipType"
							+ " ,個人年金保険料02_支払開始日 = @PensionInsurance_2_StartPayment"
							+ " ,個人年金保険料02_新旧 = @PensionInsurance_2_OldAndNewType"
							+ " ,個人年金保険料02_支払金額 = @PensionInsurance_2_InsuranceFee"
							+ " ,個人年金保険料03_会社等名称 = @PensionInsurance_3_InsuranceCompanyName"
							+ " ,個人年金保険料03_保険等種類 = @PensionInsurance_3_InsuranceTypeName"
							+ " ,個人年金保険料03_期間 = @PensionInsurance_3_InsurancePeriod"
							+ " ,個人年金保険料03_保険等契約者氏名_姓 = @PensionInsurance_3_ContractorName1"
							+ " ,個人年金保険料03_保険等契約者氏名_名 = @PensionInsurance_3_ContractorName2"
							+ " ,個人年金保険料03_保険金等受取人氏名_姓 = @PensionInsurance_3_ReceiverName1"
							+ " ,個人年金保険料03_保険金等受取人氏名_名 = @PensionInsurance_3_ReceiverName2"
							+ " ,個人年金保険料03_保険金等受取人続柄 = @PensionInsurance_3_RelationshipType"
							+ " ,個人年金保険料03_支払開始日 = @PensionInsurance_3_StartPayment"
							+ " ,個人年金保険料03_新旧 = @PensionInsurance_3_OldAndNewType"
							+ " ,個人年金保険料03_支払金額 = @PensionInsurance_3_InsuranceFee"
							+ " ,地震保険料控除地震保険料合計 = @QuakeInsurance_QuakeAmount"
							+ " ,地震保険料控除旧長期損害保険料合計 = @QuakeInsurance_DamageTotalAmount"
							+ " ,地震保険料控除額金額01 = @QuakeInsurance_Calc1"
							+ " ,地震保険料控除額金額02 = @QuakeInsurance_Calc2"
							+ " ,地震保険料控除額金額合計 = @QuakeInsurance_DeductionAmount"
							+ " ,地震保険料控除01_会社等名称 = @QuakeInsurance_1_InsuranceCompanyName"
							+ " ,地震保険料控除01_保険等種類 = @QuakeInsurance_1_InsuranceTypeName"
							+ " ,地震保険料控除01_期間 = @QuakeInsurance_1_InsurancePeriod"
							+ " ,地震保険料控除01_保険等契約者氏名_姓 = @QuakeInsurance_1_ContractorName1"
							+ " ,地震保険料控除01_保険等契約者氏名_名 = @QuakeInsurance_1_ContractorName2"
							+ " ,地震保険料控除01_保険等対象氏名_姓 = @QuakeInsurance_1_ReceiverName1"
							+ " ,地震保険料控除01_保険等対象氏名_名 = @QuakeInsurance_1_ReceiverName2"
							+ " ,地震保険料控除01_保険等対象続柄 = @QuakeInsurance_1_RelationshipType"
							+ " ,地震保険料控除01_地震旧長期 = @QuakeInsurance_1_QuakeAndDamageType"
							+ " ,地震保険料控除01_支払保険料 = @QuakeInsurance_1_InsuranceFee"
							+ " ,地震保険料控除02_会社等名称 = @QuakeInsurance_2_InsuranceCompanyName"
							+ " ,地震保険料控除02_保険等種類 = @QuakeInsurance_2_InsuranceTypeName"
							+ " ,地震保険料控除02_期間 = @QuakeInsurance_2_InsurancePeriod"
							+ " ,地震保険料控除02_保険等契約者氏名_姓 = @QuakeInsurance_2_ContractorName1"
							+ " ,地震保険料控除02_保険等契約者氏名_名 = @QuakeInsurance_2_ContractorName2"
							+ " ,地震保険料控除02_保険等対象氏名_姓 = @QuakeInsurance_2_ReceiverName1"
							+ " ,地震保険料控除02_保険等対象氏名_名 = @QuakeInsurance_2_ReceiverName2"
							+ " ,地震保険料控除02_保険等対象続柄 = @QuakeInsurance_2_RelationshipType"
							+ " ,地震保険料控除02_地震旧長期 = @QuakeInsurance_2_QuakeAndDamageType"
							+ " ,地震保険料控除02_支払保険料 = @QuakeInsurance_2_InsuranceFee"
							+ " ,社会保険料控除合計 = @SocialInsurance_DeductionAmount"
							+ " ,社会保険料控除01_社会保険種類 = @SocialInsurance_1_InsuranceTypeName"
							+ " ,社会保険料控除01_支払先名称 = @SocialInsurance_1_InsuranceCompanyName"
							+ " ,社会保険料控除01_負担者氏名_姓 = @SocialInsurance_1_ContractorName1"
							+ " ,社会保険料控除01_負担者氏名_名 = @SocialInsurance_1_ContractorName2"
							+ " ,社会保険料控除01_負担者続柄 = @SocialInsurance_1_RelationshipType"
							+ " ,社会保険料控除01_支払保険料 = @SocialInsurance_1_InsuranceFee"
							+ " ,社会保険料控除02_社会保険種類 = @SocialInsurance_2_InsuranceTypeName"
							+ " ,社会保険料控除02_支払先名称 = @SocialInsurance_2_InsuranceCompanyName"
							+ " ,社会保険料控除02_負担者氏名_姓 = @SocialInsurance_2_ContractorName1"
							+ " ,社会保険料控除02_負担者氏名_名 = @SocialInsurance_2_ContractorName2"
							+ " ,社会保険料控除02_負担者続柄 = @SocialInsurance_2_RelationshipType"
							+ " ,社会保険料控除02_支払保険料 = @SocialInsurance_2_InsuranceFee"
							+ " ,共済契約掛金 = @SmallScaleMutualAid_MutualAidCost"
							+ " ,企業型年金加入者掛金 = @SmallScaleMutualAid_CorporatePensionCost"
							+ " ,個人型年金加入者掛金 = @SmallScaleMutualAid_PersonalPensionCost"
							+ " ,心身障害者扶養共済制度契約掛金 = @SmallScaleMutualAid_HandicappedMutualAidCost"
							+ " ,小規模企業共済等掛金控除合計 = @SmallScaleMutualAid_DeductionAmount"

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
						DbHelper.AddDbParameter(cmd, "@DepartmentNo", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Address", DbType.String);
						DbHelper.AddDbParameter(cmd, "@AllLifeInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_NewTotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_OldTotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_Calc1", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_Calc2", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_TotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_1_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_2_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_3_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@LifeInsurance_4_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_TotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_1_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@MedicalInsurance_2_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_NewTotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_OldTotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_Calc1", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_Calc2", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_TotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_StartPayment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_1_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_StartPayment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_2_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_StartPayment", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_OldAndNewType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@PensionInsurance_3_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_QuakeAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_DamageTotalAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_Calc1", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_Calc2", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_QuakeAndDamageType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_1_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_InsurancePeriod", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_ReceiverName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_ReceiverName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_QuakeAndDamageType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@QuakeInsurance_2_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_DeductionAmount", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_1_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_InsuranceTypeName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_InsuranceCompanyName", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_ContractorName1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_ContractorName2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_RelationshipType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@SocialInsurance_2_InsuranceFee", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SmallScaleMutualAid_MutualAidCost", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SmallScaleMutualAid_CorporatePensionCost", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SmallScaleMutualAid_PersonalPensionCost", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SmallScaleMutualAid_HandicappedMutualAidCost", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@SmallScaleMutualAid_DeductionAmount", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);


                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = DataConv.IfNull(strApproval);
						parameters[1].Value = DataConv.IfNull(strDecision);
						parameters[2].Value = model.Head.DepartmentNo;
						parameters[3].Value = DataConv.IfNull(model.Head.Name1);
						parameters[4].Value = DataConv.IfNull(model.Head.Name2);
						parameters[5].Value = DataConv.IfNull(model.Head.Kana1);
						parameters[6].Value = DataConv.IfNull(model.Head.Kana2);
						parameters[7].Value = DataConv.IfNull(model.Head.Address);
						parameters[8].Value = DataConv.IfNull(model.Head.AllLifeInsurance_DeductionAmount.ToString());
						parameters[9].Value = DataConv.IfNull(model.Head.LifeInsurance_NewTotalAmount.ToString());
						parameters[10].Value = DataConv.IfNull(model.Head.LifeInsurance_OldTotalAmount.ToString());
						parameters[11].Value = DataConv.IfNull(model.Head.LifeInsurance_Calc1.ToString());
						parameters[12].Value = DataConv.IfNull(model.Head.LifeInsurance_Calc2.ToString());
						parameters[13].Value = DataConv.IfNull(model.Head.LifeInsurance_TotalAmount.ToString());
						parameters[14].Value = DataConv.IfNull(model.Head.LifeInsurance_DeductionAmount.ToString());
						parameters[15].Value = DataConv.IfNull(model.Head.LifeInsurance_1_InsuranceCompanyName);
						parameters[16].Value = DataConv.IfNull(model.Head.LifeInsurance_1_InsuranceTypeName);
						parameters[17].Value = DataConv.IfNull(model.Head.LifeInsurance_1_InsurancePeriod);
						parameters[18].Value = DataConv.IfNull(model.Head.LifeInsurance_1_ContractorName1);
						parameters[19].Value = DataConv.IfNull(model.Head.LifeInsurance_1_ContractorName2);
						parameters[20].Value = DataConv.IfNull(model.Head.LifeInsurance_1_ReceiverName1);
						parameters[21].Value = DataConv.IfNull(model.Head.LifeInsurance_1_ReceiverName2);
						parameters[22].Value = DataConv.IfNull(model.Head.LifeInsurance_1_RelationshipType);
						parameters[23].Value = DataConv.IfNull(model.Head.LifeInsurance_1_OldAndNewType);
						parameters[24].Value = DataConv.IfNull(model.Head.LifeInsurance_1_InsuranceFee.ToString());
						parameters[25].Value = DataConv.IfNull(model.Head.LifeInsurance_2_InsuranceCompanyName);
						parameters[26].Value = DataConv.IfNull(model.Head.LifeInsurance_2_InsuranceTypeName);
						parameters[27].Value = DataConv.IfNull(model.Head.LifeInsurance_2_InsurancePeriod);
						parameters[28].Value = DataConv.IfNull(model.Head.LifeInsurance_2_ContractorName1);
						parameters[29].Value = DataConv.IfNull(model.Head.LifeInsurance_2_ContractorName2);
						parameters[30].Value = DataConv.IfNull(model.Head.LifeInsurance_2_ReceiverName1);
						parameters[31].Value = DataConv.IfNull(model.Head.LifeInsurance_2_ReceiverName2);
						parameters[32].Value = DataConv.IfNull(model.Head.LifeInsurance_2_RelationshipType);
						parameters[33].Value = DataConv.IfNull(model.Head.LifeInsurance_2_OldAndNewType);
						parameters[34].Value = DataConv.IfNull(model.Head.LifeInsurance_2_InsuranceFee.ToString());
						parameters[35].Value = DataConv.IfNull(model.Head.LifeInsurance_3_InsuranceCompanyName);
						parameters[36].Value = DataConv.IfNull(model.Head.LifeInsurance_3_InsuranceTypeName);
						parameters[37].Value = DataConv.IfNull(model.Head.LifeInsurance_3_InsurancePeriod);
						parameters[38].Value = DataConv.IfNull(model.Head.LifeInsurance_3_ContractorName1);
						parameters[39].Value = DataConv.IfNull(model.Head.LifeInsurance_3_ContractorName2);
						parameters[40].Value = DataConv.IfNull(model.Head.LifeInsurance_3_ReceiverName1);
						parameters[41].Value = DataConv.IfNull(model.Head.LifeInsurance_3_ReceiverName2);
						parameters[42].Value = DataConv.IfNull(model.Head.LifeInsurance_3_RelationshipType);
						parameters[43].Value = DataConv.IfNull(model.Head.LifeInsurance_3_OldAndNewType);
						parameters[44].Value = DataConv.IfNull(model.Head.LifeInsurance_3_InsuranceFee.ToString());
						parameters[45].Value = DataConv.IfNull(model.Head.LifeInsurance_4_InsuranceCompanyName);
						parameters[46].Value = DataConv.IfNull(model.Head.LifeInsurance_4_InsuranceTypeName);
						parameters[47].Value = DataConv.IfNull(model.Head.LifeInsurance_4_InsurancePeriod);
						parameters[48].Value = DataConv.IfNull(model.Head.LifeInsurance_4_ContractorName1);
						parameters[49].Value = DataConv.IfNull(model.Head.LifeInsurance_4_ContractorName2);
						parameters[50].Value = DataConv.IfNull(model.Head.LifeInsurance_4_ReceiverName1);
						parameters[51].Value = DataConv.IfNull(model.Head.LifeInsurance_4_ReceiverName2);
						parameters[52].Value = DataConv.IfNull(model.Head.LifeInsurance_4_RelationshipType);
						parameters[53].Value = DataConv.IfNull(model.Head.LifeInsurance_4_OldAndNewType);
						parameters[54].Value = DataConv.IfNull(model.Head.LifeInsurance_4_InsuranceFee.ToString());
						parameters[55].Value = DataConv.IfNull(model.Head.MedicalInsurance_TotalAmount.ToString());
						parameters[56].Value = DataConv.IfNull(model.Head.MedicalInsurance_DeductionAmount.ToString());
						parameters[57].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_InsuranceCompanyName);
						parameters[58].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_InsuranceTypeName);
						parameters[59].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_InsurancePeriod);
						parameters[60].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_ContractorName1);
						parameters[61].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_ContractorName2);
						parameters[62].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_ReceiverName1);
						parameters[63].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_ReceiverName2);
						parameters[64].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_RelationshipType);
						parameters[65].Value = DataConv.IfNull(model.Head.MedicalInsurance_1_InsuranceFee.ToString());
						parameters[66].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_InsuranceCompanyName);
						parameters[67].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_InsuranceTypeName);
						parameters[68].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_InsurancePeriod);
						parameters[69].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_ContractorName1);
						parameters[70].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_ContractorName2);
						parameters[71].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_ReceiverName1);
						parameters[72].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_ReceiverName2);
						parameters[73].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_RelationshipType);
						parameters[74].Value = DataConv.IfNull(model.Head.MedicalInsurance_2_InsuranceFee.ToString());
						parameters[75].Value = DataConv.IfNull(model.Head.PensionInsurance_NewTotalAmount.ToString());
						parameters[76].Value = DataConv.IfNull(model.Head.PensionInsurance_OldTotalAmount.ToString());
						parameters[77].Value = DataConv.IfNull(model.Head.PensionInsurance_Calc1.ToString());
						parameters[78].Value = DataConv.IfNull(model.Head.PensionInsurance_Calc2.ToString());
						parameters[79].Value = DataConv.IfNull(model.Head.PensionInsurance_TotalAmount.ToString());
						parameters[80].Value = DataConv.IfNull(model.Head.PensionInsurance_DeductionAmount.ToString());
						parameters[81].Value = DataConv.IfNull(model.Head.PensionInsurance_1_InsuranceCompanyName);
						parameters[82].Value = DataConv.IfNull(model.Head.PensionInsurance_1_InsuranceTypeName);
						parameters[83].Value = DataConv.IfNull(model.Head.PensionInsurance_1_InsurancePeriod);
						parameters[84].Value = DataConv.IfNull(model.Head.PensionInsurance_1_ContractorName1);
						parameters[85].Value = DataConv.IfNull(model.Head.PensionInsurance_1_ContractorName2);
						parameters[86].Value = DataConv.IfNull(model.Head.PensionInsurance_1_ReceiverName1);
						parameters[87].Value = DataConv.IfNull(model.Head.PensionInsurance_1_ReceiverName2);
						parameters[88].Value = DataConv.IfNull(model.Head.PensionInsurance_1_RelationshipType);
                        parameters[89].Value = DataConv.IfNull(addYMD(model.Head.PensionInsurance_1_StartPaymentYear,model.Head.PensionInsurance_1_StartPaymentMonth,model.Head.PensionInsurance_1_StartPaymentDay));
						parameters[90].Value = DataConv.IfNull(model.Head.PensionInsurance_1_OldAndNewType);
						parameters[91].Value = DataConv.IfNull(model.Head.PensionInsurance_1_InsuranceFee.ToString());
						parameters[92].Value = DataConv.IfNull(model.Head.PensionInsurance_2_InsuranceCompanyName);
						parameters[93].Value = DataConv.IfNull(model.Head.PensionInsurance_2_InsuranceTypeName);
						parameters[94].Value = DataConv.IfNull(model.Head.PensionInsurance_2_InsurancePeriod);
						parameters[95].Value = DataConv.IfNull(model.Head.PensionInsurance_2_ContractorName1);
						parameters[96].Value = DataConv.IfNull(model.Head.PensionInsurance_2_ContractorName2);
						parameters[97].Value = DataConv.IfNull(model.Head.PensionInsurance_2_ReceiverName1);
						parameters[98].Value = DataConv.IfNull(model.Head.PensionInsurance_2_ReceiverName2);
						parameters[99].Value = DataConv.IfNull(model.Head.PensionInsurance_2_RelationshipType);
                        parameters[100].Value = DataConv.IfNull(addYMD(model.Head.PensionInsurance_2_StartPaymentYear,model.Head.PensionInsurance_2_StartPaymentMonth,model.Head.PensionInsurance_2_StartPaymentDay));
						parameters[101].Value = DataConv.IfNull(model.Head.PensionInsurance_2_OldAndNewType);
						parameters[102].Value = DataConv.IfNull(model.Head.PensionInsurance_2_InsuranceFee.ToString());
						parameters[103].Value = DataConv.IfNull(model.Head.PensionInsurance_3_InsuranceCompanyName);
						parameters[104].Value = DataConv.IfNull(model.Head.PensionInsurance_3_InsuranceTypeName);
						parameters[105].Value = DataConv.IfNull(model.Head.PensionInsurance_3_InsurancePeriod);
						parameters[106].Value = DataConv.IfNull(model.Head.PensionInsurance_3_ContractorName1);
						parameters[107].Value = DataConv.IfNull(model.Head.PensionInsurance_3_ContractorName2);
						parameters[108].Value = DataConv.IfNull(model.Head.PensionInsurance_3_ReceiverName1);
						parameters[109].Value = DataConv.IfNull(model.Head.PensionInsurance_3_ReceiverName2);
						parameters[110].Value = DataConv.IfNull(model.Head.PensionInsurance_3_RelationshipType);
                        parameters[111].Value = DataConv.IfNull(addYMD(model.Head.PensionInsurance_3_StartPaymentYear,model.Head.PensionInsurance_3_StartPaymentMonth,model.Head.PensionInsurance_3_StartPaymentDay));
						parameters[112].Value = DataConv.IfNull(model.Head.PensionInsurance_3_OldAndNewType);
						parameters[113].Value = DataConv.IfNull(model.Head.PensionInsurance_3_InsuranceFee.ToString());
						parameters[114].Value = DataConv.IfNull(model.Head.QuakeInsurance_QuakeAmount.ToString());
						parameters[115].Value = DataConv.IfNull(model.Head.QuakeInsurance_DamageTotalAmount.ToString());
						parameters[116].Value = DataConv.IfNull(model.Head.QuakeInsurance_Calc1.ToString());
						parameters[117].Value = DataConv.IfNull(model.Head.QuakeInsurance_Calc2.ToString());
						parameters[118].Value = DataConv.IfNull(model.Head.QuakeInsurance_DeductionAmount.ToString());
						parameters[119].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_InsuranceCompanyName);
						parameters[120].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_InsuranceTypeName);
						parameters[121].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_InsurancePeriod);
						parameters[122].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_ContractorName1);
						parameters[123].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_ContractorName2);
						parameters[124].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_ReceiverName1);
						parameters[125].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_ReceiverName2);
						parameters[126].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_RelationshipType);
						parameters[127].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_QuakeAndDamageType);
						parameters[128].Value = DataConv.IfNull(model.Head.QuakeInsurance_1_InsuranceFee.ToString());
						parameters[129].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_InsuranceCompanyName);
						parameters[130].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_InsuranceTypeName);
						parameters[131].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_InsurancePeriod);
						parameters[132].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_ContractorName1);
						parameters[133].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_ContractorName2);
						parameters[134].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_ReceiverName1);
						parameters[135].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_ReceiverName2);
						parameters[136].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_RelationshipType);
						parameters[137].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_QuakeAndDamageType);
						parameters[138].Value = DataConv.IfNull(model.Head.QuakeInsurance_2_InsuranceFee.ToString());
						parameters[139].Value = DataConv.IfNull(model.Head.SocialInsurance_DeductionAmount.ToString());
						parameters[140].Value = DataConv.IfNull(model.Head.SocialInsurance_1_InsuranceTypeName);
						parameters[141].Value = DataConv.IfNull(model.Head.SocialInsurance_1_InsuranceCompanyName);
						parameters[142].Value = DataConv.IfNull(model.Head.SocialInsurance_1_ContractorName1);
						parameters[143].Value = DataConv.IfNull(model.Head.SocialInsurance_1_ContractorName2);
						parameters[144].Value = DataConv.IfNull(model.Head.SocialInsurance_1_RelationshipType);
						parameters[145].Value = DataConv.IfNull(model.Head.SocialInsurance_1_InsuranceFee.ToString());
						parameters[146].Value = DataConv.IfNull(model.Head.SocialInsurance_2_InsuranceTypeName);
						parameters[147].Value = DataConv.IfNull(model.Head.SocialInsurance_2_InsuranceCompanyName);
						parameters[148].Value = DataConv.IfNull(model.Head.SocialInsurance_2_ContractorName1);
						parameters[149].Value = DataConv.IfNull(model.Head.SocialInsurance_2_ContractorName2);
						parameters[150].Value = DataConv.IfNull(model.Head.SocialInsurance_2_RelationshipType);
						parameters[151].Value = DataConv.IfNull(model.Head.SocialInsurance_2_InsuranceFee.ToString());
						parameters[152].Value = DataConv.IfNull(model.Head.SmallScaleMutualAid_MutualAidCost.ToString());
						parameters[153].Value = DataConv.IfNull(model.Head.SmallScaleMutualAid_CorporatePensionCost.ToString());
						parameters[154].Value = DataConv.IfNull(model.Head.SmallScaleMutualAid_PersonalPensionCost.ToString());
						parameters[155].Value = DataConv.IfNull(model.Head.SmallScaleMutualAid_HandicappedMutualAidCost.ToString());
						parameters[156].Value = DataConv.IfNull(model.Head.SmallScaleMutualAid_DeductionAmount.ToString());
                        parameters[157].Value = DataConv.IfNull(model.Head.SheetYear.ToString());
                        parameters[158].Value = DataConv.IfNull(model.Head.EmployeeNo);


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
        public void Sign(HokenDeclareRegisterViewModels model, string value, LoginUser lu, bool isSign = true) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string[] segments = value.Split(',');
                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);

                var strApproval = model.Head.ApprovalType;
                var strDecision = model.Head.DecisionType;
                switch (model.Head.InputMode) {
                    case ajustMode.SelfRegist:
                        if(strApproval == "1") strApproval = "0";
                        break;
                    case ajustMode.adminRegist:
                        if(strDecision == "1") strDecision = "0";
                        break;
                }

                using(var scope = new TransactionScope()) {
                    var dbm = new DbManager();
                    //保険料控除控除申告書承認
                    var sql = "update TE110保険料控除申告書Data"
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
