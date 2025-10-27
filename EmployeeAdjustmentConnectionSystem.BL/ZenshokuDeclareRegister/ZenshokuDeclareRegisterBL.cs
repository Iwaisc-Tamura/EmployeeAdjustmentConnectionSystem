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
using System.Web.Mvc;

namespace EmployeeAdjustmentConnectionSystem.BL.ZenshokuDeclareRegister {
    /// <summary>
    /// 前職源泉徴収票入力ビジネスロジック
    /// </summary>
    public class ZenshokuDeclareRegisterBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ZenshokuDeclareRegisterBL() {
        }


        /// <summary>
        /// 前職源泉徴収票データ取得
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <returns>前職源泉徴収票モデル</returns>
        public ZenshokuDeclareRegisterViewModels Select(int? intSheetYear,string strEmployeeNo,bool bolAdminMode) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                ZenshokuDeclareRegisterViewModels model = new ZenshokuDeclareRegisterViewModels();
                model.Head = new ZenshokuDeclareRegisterHeaderModel();
                model.Body = new ZenshokuDeclareRegisterBodyModel();

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
                        } else if (value1 == "9" && value2 == "7") {
                            return "システム連携済み";
                        } else if (value1 == "9" && value2 == "8") {
                            return "システム連携後修正";
                        } else if (value1 == "9" && value2 == "9") {
                            return "確定済み";
                        } else {
                            return "システムエラー";
                        }
                    };

                    var sql = "SELECT T前職.* ";
                        sql += " FROM TE160前職源泉徴収票Data As T前職  ";
                        sql += " WHERE T前職.対象年度 = @SheetYear and T前職.社員番号 = @EmployeeNo ";

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
                            model.Head = new ZenshokuDeclareRegisterHeaderModel {
								SheetYear = DataConv.IntParse(row["対象年度"].ToString()),
								CreateType = row["作成区分"].ToString(),
								ApprovalType = row["本人確定区分"].ToString(),
								DecisionType = row["管理者確定区分"].ToString(),
                                StatusName = StatusDecision(row["本人確定区分"].ToString(),row["管理者確定区分"].ToString()),
								EmployeeNo = row["社員番号"].ToString(),
								DepartmentNo = DataConv.IntParse(row["所属番号"].ToString()),
								Name1 = row["氏名_姓"].ToString(),
								Name2 = row["氏名_名"].ToString(),
								Kana1 = row["Kana_姓"].ToString(),
								Kana2 = row["Kana_名"].ToString(),
                                PaymentAmount = setMoney(row["支払金額"].ToString()),
                                SocialInsuranceAmount = setMoney(row["社会保険料等金額"].ToString()),
                                WithholdingTaxAmount = setMoney(row["源泉徴収税額"].ToString()),
								CompanyName = row["会社名"].ToString(),
								CompanyAddress = row["会社住所"].ToString(),
                                // 退職日
                                RetirementDate = row["退職日"].ToString(),
                                RetirementDateYear  = row["退職日"].ToString() == "" ? "" : row["退職日"].ToString().Substring(0, 4),
                                RetirementDateMonth = row["退職日"].ToString() == "" ? "" : row["退職日"].ToString().Substring(4, 2).TrimStart(new Char[] { '0' }),
                                RetirementDateDay   = row["退職日"].ToString() == "" ? "" : row["退職日"].ToString().Substring(6, 2).TrimStart(new Char[] { '0' }),
                                Remarks = row["備考"].ToString(),
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
        public void SetMode(ZenshokuDeclareRegisterViewModels model, LoginUser lu) {
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
        /// <param name="model">前職源泉徴収票モデル</param>
        /// <param name="model">登録モード(1:途中保存/2:承認登録)</param>
        public void Save(ZenshokuDeclareRegisterViewModels model,LoginUser lu,string mode) {
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
                        var strCreate = "1";    //登録時に作成区分をたてる
                        var strApproval = model.Head.ApprovalType;
                        var strDecision = model.Head.DecisionType;
                        if (mode=="2") {    //承認登録時のみ更新
                            switch (model.Head.InputMode) {
                                case ajustMode.SelfInput:
                                    strApproval = "1";
                                    break;
                                case ajustMode.adminInput:
                                    strApproval = "9";
									switch (lu.IsAdminNo) {
										case "2":
										case "3":
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
												strDecision = "1";
											}
											break;
										case "1":
										case "7":
										case "8":
										case "9":
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
			                                    strDecision = "5";
											}
											break;
										case "K":
											if (strDecision == "7"|| strDecision == "8") {
												strDecision = "8";
											} else {
			                                    strDecision = "5";
											}
											break;
									}
                                    break;
                            }
                        }

                        //共通基本情報
                        var sql = "update TE160前職源泉徴収票Data"
                            + " set "
							+ " 作成区分 = @CreateType"
							+ " ,本人確定区分 = @ApprovalType"
							+ " ,管理者確定区分 = @DecisionType"
							+ " ,所属番号 = @DepartmentNo"
							+ " ,氏名_姓 = @Name1"
							+ " ,氏名_名 = @Name2"
							+ " ,Kana_姓 = @Kana1"
							+ " ,Kana_名 = @Kana2"
                            + " ,支払金額 = @PaymentAmount"
                            + " ,社会保険料等金額 = @SocialInsuranceAmount"
                            + " ,源泉徴収税額 = @WithholdingTaxAmount"
                            + " ,会社名 = @CompanyName"
                            + " ,会社住所 = @CompanyAddress"
                            + " ,退職日 = @RetirementDate"
                            + " ,備考 = @Remarks"

							+ " ,最終更新者ID = '" + lu.UserCode + "'"
							+ " ,更新年月日 = GETDATE()"
							+ " ,更新回数 = 更新回数 + 1"

                            + " where 1=1 "
                            + "     AND 対象年度 = @SheetYear"
                            + "     AND 社員番号 = @EmployeeNo"
                            + "";
                        //SQL文の型を指定
                        var cmd = dm.CreateCommand(sql);
						DbHelper.AddDbParameter(cmd, "@CreateType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@ApprovalType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DecisionType", DbType.String);
						DbHelper.AddDbParameter(cmd, "@DepartmentNo", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@Name1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Name2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@Kana2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@PaymentAmount", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@SocialInsuranceAmount", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@WithholdingTaxAmount", DbType.Int32);
                        DbHelper.AddDbParameter(cmd, "@CompanyName", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@CompanyAddress", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RetirementDate", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Remarks", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
						DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);


                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
						parameters[0].Value = DataConv.IfNull(strCreate);
						parameters[1].Value = DataConv.IfNull(strApproval);
						parameters[2].Value = DataConv.IfNull(strDecision);
						parameters[3].Value = model.Head.DepartmentNo;
						parameters[4].Value = DataConv.IfNull(model.Head.Name1);
						parameters[5].Value = DataConv.IfNull(model.Head.Name2);
						parameters[6].Value = DataConv.IfNull(model.Head.Kana1);
						parameters[7].Value = DataConv.IfNull(model.Head.Kana2);
                        parameters[8].Value  = DataConv.IfNull(model.Head.PaymentAmount?.ToString());
                        parameters[9].Value  = DataConv.IfNull(model.Head.SocialInsuranceAmount?.ToString());
                        parameters[10].Value = DataConv.IfNull(model.Head.WithholdingTaxAmount?.ToString());
                        parameters[11].Value = DataConv.IfNull(model.Head.CompanyName);
                        parameters[12].Value = DataConv.IfNull(model.Head.CompanyAddress);
                        parameters[13].Value = DataConv.IfNull(
                            addYMD(model.Head.RetirementDateYear, model.Head.RetirementDateMonth, model.Head.RetirementDateDay));
                        parameters[14].Value = DataConv.IfNull(model.Head.Remarks);
                        parameters[15].Value = DataConv.IfNull(model.Head.SheetYear.ToString());
                        parameters[16].Value = DataConv.IfNull(model.Head.EmployeeNo);

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
        /// 承認取消処理
        /// </summary>
        /// <param name="model">前職源泉徴収票入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        public void Sign(ZenshokuDeclareRegisterViewModels model, string strDepartmentNo, LoginUser lu, bool isSign = true) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);
                var strCreate = model.Head.CreateType;
                var strApproval = model.Head.ApprovalType;
                var strDecision = model.Head.DecisionType;
                switch (model.Head.InputMode) {
                    case ajustMode.SelfRegist:
                        if(strApproval == "1") strApproval = "0";
                        break;
                    case ajustMode.adminRegist:
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
                        break;
                }

                using(var scope = new TransactionScope()) {
                    var dbm = new DbManager();
                    //前職源泉徴収票承認取り消し
                    var sql = "update TE160前職源泉徴収票Data"
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

        /// <summary>
        /// データ削除
        /// </summary>
        /// <param name="model">前職源泉徴収票入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        public void Delete(ZenshokuDeclareRegisterViewModels model, string strDepartmentNo, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);
                var strCreate = model.Head.CreateType;
                var strApproval = model.Head.ApprovalType;
                var strDecision = model.Head.DecisionType;
                switch (model.Head.InputMode) {
                    case ajustMode.SelfRegist:
                        if(strApproval == "1") strApproval = "0";
                        break;
                    case ajustMode.adminRegist:
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
                        break;
                }

                using(var scope = new TransactionScope()) {
                    var dbm = new DbManager();
                    //前職源泉徴収票
                    var sql = "update TE160前職源泉徴収票Data"
                                + " set "
                                + " 作成区分 = '0'"
                                + ", 本人確定区分 = '0'"
                                + ",管理者確定区分 = '0'"

                                + " ,最終更新者ID = '" + lu.UserCode + "'"
							    + " ,更新年月日 = GETDATE()"
    						    + " ,更新回数 = 更新回数 + 1"
                                
                                + " where 対象年度 = @SheetYear"
                                + "   and 社員番号 = @EmployeeNo";
                    //SQL文の型を指定
                    IDbCommand cmd = dbm.CreateCommand(sql);
                    DbHelper.AddDbParameter(cmd, "@SheetYear", DbType.Int32);
                    DbHelper.AddDbParameter(cmd, "@EmployeeNo", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = model.Head.SheetYear;
                    parameters[1].Value = DataConv.IfNull(model.Head.EmployeeNo);
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
