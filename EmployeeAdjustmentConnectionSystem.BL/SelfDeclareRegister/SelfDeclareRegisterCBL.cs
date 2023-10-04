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

namespace EmployeeAdjustmentConnectionSystem.BL.SelfDeclareRegister {
    /// <summary>
    /// 目標管理入力ビジネスロジック
    /// </summary>
    public class SelfDeclareRegisterCBL {
        #region メンバ変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static NLog.Logger nlog = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public SelfDeclareRegisterCBL() {
        }


        /// <summary>
        /// 目標管理データ取得
        /// </summary>
        /// <param name="id">管理番号</param>
        /// <returns>目標管理モデル</returns>
        public SelfDeclareRegisterCViewModels Select(int? id) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                SelfDeclareRegisterCViewModels model = new SelfDeclareRegisterCViewModels();
                model.Head = new SelfDeclareRegisterCHeaderModel();
                model.Body = new SelfDeclareRegisterCBodyModel();
                //model.Head.JcalYear = DataConv.Date2Jcal(DateTime.Now, "ggy年");

                using(DbManager dm = new DbManager()) {
                    //承認情報取得
                    var signds = SelfDeclareCommonBL.GetSignData(dm, id);

                    //長いので無名関数化
                    Func<string[], string> getSign = (cols) => {
                        return SelfDeclareCommonBL.GetSignName(signds, new string[] { cols[0], cols[1]});
                    };
                    Func<string, string> selectMainte = (val) => {
                        if (val == ":") {
                            return " ";
                        }
                        return val;
                    };


                    var sql = "SELECT * FROM SD_T自己申告書共通基本Data WHERE 管理番号 = @ManageNo ";

                    using(IDbCommand cmd = dm.CreateCommand(sql))
                    using(DataSet ds = new DataSet()) {
                        DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                        ((IDbDataParameter)cmd.Parameters[0]).Value = id;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);
                        var rows = ds.Tables[0].Rows;

                        foreach(DataRow row in rows) {
                            model.Head = new SelfDeclareRegisterCHeaderModel {

                                // 管理番号
                                ManageNo = row["管理番号"].ToString(),

                                //年度
                                SheetYear = DataConv.IntParse(row["年度"].ToString()),

                                //自己申告書種別
                                SelfDeclareCode = row["自己申告書種別Code"].ToString(),

                                // 氏名
                                Name = row["氏名"].ToString(),
								Kana = row["Kana"].ToString(),

                                // 社員番号
                                EmployeeNo = row["社員番号"].ToString(),

                                // 所属
                                DepartmentNo = DataConv.IntParse(row["所属番号"].ToString()),
                                DepartmentName = row["所属名"].ToString(),

                                // 役職
                                PostNo = DataConv.IntParse(row["役職番号"].ToString()),
                                PostName = row["役職名"].ToString(),

                                // 資格
                                CompetencyNo = DataConv.IntParse(row["資格番号"].ToString()),
                                CompetencyName = row["資格名"].ToString(),

                                //職掌
                                DutyNo = DataConv.IntParse(row["職掌番号"].ToString()),
                                DutyName = row["職掌名"].ToString(),

                                //在籍期間
                                HireDate = row["入社年月日"].ToString(),
                                EnrollmentMonths = DataConv.IntParse(row["在籍月数"].ToString()),
                                ExperienceMonths = DataConv.IntParse(row["現職経験月数"].ToString()),

                                //年齢
                                Birthday = row["生年月日"].ToString(),
                                Age = DataConv.IntParse(row["年齢"].ToString()),

                                //住所
                                PostalCode_1 = row["郵便番号_1"].ToString(),
                                PostalCode_2 = row["郵便番号_2"].ToString(),
                                Address = row["住所"].ToString(),
                                AddressType = selectMainte(row["住所形態区分"].ToString() + ":" +row["住所形態内容"].ToString()),
                                AddressTypeContent = row["住所形態内容"].ToString(),

                                // 家族構成
                                FamilyCount = row["家族構成人数"].ToString(),
                                Relationship_1 = selectMainte(row["家族構成続柄区分_1"].ToString() + ":" + row["家族構成続柄内容_1"].ToString()),
                                RelationshipContent_1 = row["家族構成続柄内容_1"].ToString(),
                                RelationshipContentOther_1 = row["家族構成続柄内容_1_Other"].ToString(),
				                FamilyAge_1 = row["家族構成年齢_1"].ToString(),
				                FamilyJob_1 = row["家族構成職業学年_1"].ToString(),
				                FamilyLodger_1 = row["家族構成同居区分_1"].ToString(),
				                FamilyDependent_1 = row["家族構成扶養区分_1"].ToString(),
                                Relationship_2 = selectMainte(row["家族構成続柄区分_2"].ToString() + ":" + row["家族構成続柄内容_2"].ToString()),
                                RelationshipContent_2 = row["家族構成続柄内容_2"].ToString(),
                                RelationshipContentOther_2 = row["家族構成続柄内容_2_Other"].ToString(),
								FamilyAge_2 = row["家族構成年齢_2"].ToString(),
								FamilyJob_2 = row["家族構成職業学年_2"].ToString(),
								FamilyLodger_2 = row["家族構成同居区分_2"].ToString(),
								FamilyDependent_2 = row["家族構成扶養区分_2"].ToString(),
                                Relationship_3 = selectMainte(row["家族構成続柄区分_3"].ToString() + ":" + row["家族構成続柄内容_3"].ToString()),
                                RelationshipContent_3 = row["家族構成続柄内容_3"].ToString(),
                                RelationshipContentOther_3 = row["家族構成続柄内容_3_Other"].ToString(),
								FamilyAge_3 = row["家族構成年齢_3"].ToString(),
								FamilyJob_3 = row["家族構成職業学年_3"].ToString(),
								FamilyLodger_3 = row["家族構成同居区分_3"].ToString(),
								FamilyDependent_3 = row["家族構成扶養区分_3"].ToString(),
                                Relationship_4 = selectMainte(row["家族構成続柄区分_4"].ToString() + ":" + row["家族構成続柄内容_4"].ToString()),
                                RelationshipContent_4 = row["家族構成続柄内容_4"].ToString(),
                                RelationshipContentOther_4 = row["家族構成続柄内容_4_Other"].ToString(),
								FamilyAge_4 = row["家族構成年齢_4"].ToString(),
								FamilyJob_4 = row["家族構成職業学年_4"].ToString(),
								FamilyLodger_4 = row["家族構成同居区分_4"].ToString(),
								FamilyDependent_4 = row["家族構成扶養区分_4"].ToString(),
                                Relationship_5 = selectMainte(row["家族構成続柄区分_5"].ToString() + ":" + row["家族構成続柄内容_5"].ToString()),
                                RelationshipContent_5 = row["家族構成続柄内容_5"].ToString(),
                                RelationshipContentOther_5 = row["家族構成続柄内容_5_Other"].ToString(),
								FamilyAge_5 = row["家族構成年齢_5"].ToString(),
								FamilyJob_5 = row["家族構成職業学年_5"].ToString(),
								FamilyLodger_5 = row["家族構成同居区分_5"].ToString(),
								FamilyDependent_5 = row["家族構成扶養区分_5"].ToString(),
                                Relationship_6 = selectMainte(row["家族構成続柄区分_6"].ToString() + ":" + row["家族構成続柄内容_6"].ToString()),
                                RelationshipContent_6 = row["家族構成続柄内容_6"].ToString(),
                                RelationshipContentOther_6 = row["家族構成続柄内容_6_Other"].ToString(),
								FamilyAge_6 = row["家族構成年齢_6"].ToString(),
								FamilyJob_6 = row["家族構成職業学年_6"].ToString(),
								FamilyLodger_6 = row["家族構成同居区分_6"].ToString(),
								FamilyDependent_6 = row["家族構成扶養区分_6"].ToString(),
                                Relationship_7 = selectMainte(row["家族構成続柄区分_7"].ToString() + ":" + row["家族構成続柄内容_7"].ToString()),
                                RelationshipContent_7 = row["家族構成続柄内容_7"].ToString(),
                                RelationshipContentOther_7 = row["家族構成続柄内容_7_Other"].ToString(),
								FamilyAge_7 = row["家族構成年齢_7"].ToString(),
								FamilyJob_7 = row["家族構成職業学年_7"].ToString(),
								FamilyLodger_7 = row["家族構成同居区分_7"].ToString(),
								FamilyDependent_7 = row["家族構成扶養区分_7"].ToString(),
                                Relationship_8 = selectMainte(row["家族構成続柄区分_8"].ToString() + ":" + row["家族構成続柄内容_8"].ToString()),
                                RelationshipContent_8 = row["家族構成続柄内容_8"].ToString(),
                                RelationshipContentOther_8 = row["家族構成続柄内容_8_Other"].ToString(),
								FamilyAge_8 = row["家族構成年齢_8"].ToString(),
								FamilyJob_8 = row["家族構成職業学年_8"].ToString(),
								FamilyLodger_8 = row["家族構成同居区分_8"].ToString(),
								FamilyDependent_8 = row["家族構成扶養区分_8"].ToString(),
                                Health = selectMainte(row["健康状態区分"].ToString() + ":" + row["健康状態内容"].ToString()),
                                HealthContent = row["健康状態内容"].ToString(),
                                UnHealthContent = row["健康状態不順状態"].ToString(),

                                //承認状態取得
                                AtoCSelfSign = getSign(new string[] { "1", "1" }),  //本人
                                AtoCBossSign = getSign(new string[] { "1", "2" }),  // 上司

                            };
    
                            //表示用入社年月日
                            model.Head.HireDateView = DateTime.ParseExact(model.Head.HireDate, "yyyymmdd", null).ToString("yyyy/m/d");

                            //表示用在籍期間・現職経験
                            int intYear = 0;
                            int intMonth = 0;
                            intYear = (int)(model.Head.EnrollmentMonths/12);
                            intMonth = (int)(model.Head.EnrollmentMonths%12);
                            if (intYear!=0){
                                model.Head.EnrollmentMonthsView = intYear.ToString("0年");
                            }
                            model.Head.EnrollmentMonthsView = model.Head.EnrollmentMonthsView + intMonth.ToString("0ヶ月");
                            
                            intYear = (int)(model.Head.ExperienceMonths/12);
                            intMonth = (int)(model.Head.ExperienceMonths%12);
                            if (intYear!=0){
                                model.Head.ExperienceMonthsView = intYear.ToString("0年");
                            }
                            model.Head.ExperienceMonthsView = model.Head.ExperienceMonthsView + intMonth.ToString("0ヶ月");

                            //表示用生年月日・年齢
                            model.Head.BirthdayView = DateTime.ParseExact(model.Head.Birthday, "yyyymmdd", null).ToString("yyyy/m/d");
                            model.Head.AgeView = ((int)model.Head.Age).ToString("0歳");
                        }
                    }

                    sql = "SELECT * FROM SD_T自己申告書明細Data" + model.Head.SelfDeclareCode + " WHERE 管理番号 = @ManageNo ";

                    using(IDbCommand cmd = dm.CreateCommand(sql))
                    using(DataSet ds = new DataSet()) {
                        DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                        ((IDbDataParameter)cmd.Parameters[0]).Value = id;

                        IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                        da.Fill(ds);
                        var rows = ds.Tables[0].Rows;
                        foreach(DataRow row in rows) {
                            model.Body = new SelfDeclareRegisterCBodyModel {
                                TransferDepartment_1 = row["配置換_1"].ToString(),
                                TransferDepartment_2_1 = selectMainte(row["配置換区分_2_1"].ToString() + ":" + row["配置換内容_2_1"].ToString()),
                                TransferDepartment_1_1_Content = row["配置換内容_2_1"].ToString(),
                                TransferDepartment_2_2 = selectMainte(row["配置換区分_2_2"].ToString() + ":" + row["配置換内容_2_2"].ToString()),
                                TransferDepartment_2_2_Content = row["配置換内容_2_2"].ToString(),
                                TransferDepartment_2_3 = selectMainte(row["配置換区分_2_3"].ToString() + ":" + row["配置換内容_2_3"].ToString()),
                                TransferDepartment_2_3_Content = row["配置換内容_2_3"].ToString(),
                                TransferDepartment_2_Other = row["配置換内容_2_Other"].ToString(),
                                ChargeDuty_1 = row["担当職務_1"].ToString(),
                                ChargeDuty_2 = row["担当職務_2"].ToString(),
                                AptitudeDevelop_1 = row["能力開発_1"].ToString(),
                                OtherComment = row["その他"].ToString(),
                                FreeComment = row["自由意見内容"].ToString(),
                                BossComment = row["上司記入欄内容"].ToString()
                            };

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
        public void SetMode(SelfDeclareRegisterCViewModels model, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                //画面の状態初期設定
                model.Head.InputMode = SelfDeclareMode.ReadOnly;
                model.Head.CancelButton = SelfDeclareMode.ReadOnly;
                model.Head.AuthButton = SelfDeclareMode.ReadOnly;

                using(DbManager dm = new DbManager()) {
                    //承認情報現在地取得
                    var mode = (SelfDeclareMode)Enum.Parse(typeof(SelfDeclareMode), (SelfDeclareCommonBL.GetSignStatus(dm, int.Parse(model.Head.ManageNo),"1", false)).ToString());

                    //自データ時設定
                    if(model.Head.EmployeeNo == lu.UserCode) {
                        //モード設定
                        switch(mode) {
                            //本人入力前
                            case SelfDeclareMode.None:
                                model.Head.InputMode = SelfDeclareMode.AtoCSelfSign;
                                model.Head.AuthButton = SelfDeclareMode.AtoCSelfSign;
                                break;
                            //本人入力後
                            case SelfDeclareMode.AtoCSelfSign:
                                model.Head.CancelButton = SelfDeclareMode.AtoCSelfSign;
                                break;
                            //上記以外
                            default:
                                break;
                        }
                        return;
                    }
                    //他データ時設定
                    //承認者か？ //承認者以外
                    if(!SelfDeclareCommonBL.IsAuthorizer(dm, model.Head.ManageNo, lu.UserCode)) {
                        //一般社員
                        if(lu.IsRoot == false) return;
                        //管理者用キャンセル
                        this.SetRootCancel(dm, model, lu, mode);
                        return;
                    }
                    //承認者時の画面状態設定
                    this.SetInputSignMode(dm, model, lu, mode);
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
        /// 管理者のみキャンセルできる
        /// </summary>
        /// <param name="dm">DBマネージャー</param>
        /// <param name="model">モデル</param>
        /// <param name="lu">ログインユーザー</param>
        /// <param name="mode">承認状態</param>
        private void SetRootCancel(DbManager dm, SelfDeclareRegisterCViewModels model
            , LoginUser lu, SelfDeclareMode mode) {

            //支社管理及び所属配下以外は何もしない
            if(lu.IsRootUser == false && ((int)model.Head.DepartmentNo).ToString("D").Substring(0, 1) != ((int)lu.Permission).ToString()) return;

            var sql = "select"
                    + " 管理番号"
                    + ",年度"
                    + ",所属番号"
                    + ",社員番号"
                    + ",大区分"
                    + ",中区分"
                    + ",小区分"
                    + ",区分"
                    + ",承認社員番号"
                    + ",承認者"
                    + "  from SD_VT目標管理承認権限情報"
                    + " where 管理番号 = @ManageNo"
                    + " order by 区分 desc";
            using(IDbCommand cmd = dm.CreateCommand(sql))
            using(DataSet ds = new DataSet()) {
                DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                ((IDbDataParameter)cmd.Parameters[0]).Value = model.Head.ManageNo;
                IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                da.Fill(ds);

                for(int cnt = 0; cnt < ds.Tables[0].Rows.Count; cnt++) {
                    DataRow row = ds.Tables[0].Rows[cnt];
                    //承認済みの区分時
                    //承認者に該当
                    if(row["承認社員番号"].ToString() != "") {
                        model.Head.CancelButton = (SelfDeclareMode)Enum.Parse(typeof(SelfDeclareMode), row["区分"].ToString());
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 承認者時の画面状態設定
        /// </summary>
        /// <param name="dm">DBマネージャー</param>
        /// <param name="model">モデル</param>
        /// <param name="lu">ログインユーザー</param>
        /// <param name="mode">承認状態</param>
        private void SetInputSignMode(DbManager dm, SelfDeclareRegisterCViewModels model
            , LoginUser lu, SelfDeclareMode mode) {
            var sql = "select"
                    + " 管理番号"
                    + ",年度"
                    + ",所属番号"
                    + ",社員番号"
                    + ",大区分"
                    + ",小区分"
                    + ",区分"
                    + ",承認社員番号"
                    + ",承認者"
                    + "  from SD_VT自己申告書承認権限情報"
                    + " where 管理番号 = @ManageNo"
                    + " order by 区分";
            using(IDbCommand cmd = dm.CreateCommand(sql))
            using(DataSet ds = new DataSet()) {
                DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                ((IDbDataParameter)cmd.Parameters[0]).Value = model.Head.ManageNo;
                IDataAdapter da = dm.CreateSqlDataAdapter(cmd);
                da.Fill(ds);
                //区分の変換
                var kbn = ((int)mode).ToString();

                //対象行取得
                Func<string, string, bool> getSign = (col, uid) => {
                    DataTable dt = ds.Tables[0];
                    var query = from row in dt.AsEnumerable()
                                where row.Field<string>("区分") == col
                                   && row.Field<string>("承認者") == uid
                                select row.Field<Int32>("管理番号");
                    foreach(var name in query) {
                        return true;
                    }
                    return false;
                };
                //モード設定
                Func<SelfDeclareRegisterCViewModels, string, SelfDeclareMode, bool> setMode = (m, u, om) => {
                    if(getSign(((int)om).ToString(), u)) {
                        m.Head.InputMode = om;
                        m.Head.AuthButton = om;
                        return true;
                    }
                    return false;
                };

                for(int cnt = 0; cnt < ds.Tables[0].Rows.Count; cnt++) {
                    DataRow row = ds.Tables[0].Rows[cnt];

                    //承認済か
                    if(mode != SelfDeclareMode.None && row["区分"].ToString() != kbn) continue;

                    //承認済みの区分時
                    //承認者に該当
                    if(row["承認者"].ToString() == lu.UserCode &&
                       row["承認社員番号"].ToString() != "") {
                        model.Head.CancelButton = mode;
                        //return;
                    }

                    switch(mode) {
                        //未記入
                        case SelfDeclareMode.None:
                            break;

                        //本人入力後
                        case SelfDeclareMode.AtoCSelfSign:
                            //自分が上司
                            setMode(model, lu.UserCode, SelfDeclareMode.AtoCBossSign);
                            break;

                        //上司承認後
                        case SelfDeclareMode.AtoCBossSign:
                            break;

                        default:
                            break;
                    }                    
                }
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model">目標管理モデル</param>
        public void Save(SelfDeclareRegisterCViewModels model) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                using(var scope = new TransactionScope()) {
                    using(var dm = new DbManager()) {
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

                        //共通基本情報
                        var sql = "update SD_T自己申告書共通基本Data"
                            + " set "
                            + "  郵便番号_1 = @PostalCode_1"
                            + " ,郵便番号_2 = @PostalCode_2"
                            + " ,住所 = @Address"
                            + " ,住所形態区分 = @AddressType"
                            + " ,住所形態内容 = @AddressTypeContent"
                            + " ,家族構成人数 = @FamilyCount"
                            + " ,家族構成続柄区分_1 = @Relationship_1"
                            + " ,家族構成続柄内容_1 = @RelationshipContent_1"
                            + " ,家族構成続柄内容_1_Other = @RelationshipContentOther_1"
							+ " ,家族構成年齢_1 = @FamilyAge_1"
							+ " ,家族構成職業学年_1 = @FamilyJob_1"
							+ " ,家族構成同居区分_1 = @FamilyLodger_1"
							+ " ,家族構成扶養区分_1 = @FamilyDependent_1"
                            + " ,家族構成続柄区分_2 = @Relationship_2"
                            + " ,家族構成続柄内容_2 = @RelationshipContent_2"
                            + " ,家族構成続柄内容_2_Other = @RelationshipContentOther_2"
							+ " ,家族構成年齢_2 = @FamilyAge_2"
							+ " ,家族構成職業学年_2 = @FamilyJob_2"
							+ " ,家族構成同居区分_2 = @FamilyLodger_2"
							+ " ,家族構成扶養区分_2 = @FamilyDependent_2"
                            + " ,家族構成続柄区分_3 = @Relationship_3"
                            + " ,家族構成続柄内容_3 = @RelationshipContent_3"
                            + " ,家族構成続柄内容_3_Other = @RelationshipContentOther_3"
							+ " ,家族構成年齢_3 = @FamilyAge_3"
							+ " ,家族構成職業学年_3 = @FamilyJob_3"
							+ " ,家族構成同居区分_3 = @FamilyLodger_3"
							+ " ,家族構成扶養区分_3 = @FamilyDependent_3"
                            + " ,家族構成続柄区分_4 = @Relationship_4"
                            + " ,家族構成続柄内容_4 = @RelationshipContent_4"
                            + " ,家族構成続柄内容_4_Other = @RelationshipContentOther_4"
							+ " ,家族構成年齢_4 = @FamilyAge_4"
							+ " ,家族構成職業学年_4 = @FamilyJob_4"
							+ " ,家族構成同居区分_4 = @FamilyLodger_4"
							+ " ,家族構成扶養区分_4 = @FamilyDependent_4"
                            + " ,家族構成続柄区分_5 = @Relationship_5"
                            + " ,家族構成続柄内容_5 = @RelationshipContent_5"
                            + " ,家族構成続柄内容_5_Other = @RelationshipContentOther_5"
							+ " ,家族構成年齢_5 = @FamilyAge_5"
							+ " ,家族構成職業学年_5 = @FamilyJob_5"
							+ " ,家族構成同居区分_5 = @FamilyLodger_5"
							+ " ,家族構成扶養区分_5 = @FamilyDependent_5"
                            + " ,家族構成続柄区分_6 = @Relationship_6"
                            + " ,家族構成続柄内容_6 = @RelationshipContent_6"
                            + " ,家族構成続柄内容_6_Other = @RelationshipContentOther_6"
							+ " ,家族構成年齢_6 = @FamilyAge_6"
							+ " ,家族構成職業学年_6 = @FamilyJob_6"
							+ " ,家族構成同居区分_6 = @FamilyLodger_6"
							+ " ,家族構成扶養区分_6 = @FamilyDependent_6"
                            + " ,家族構成続柄区分_7 = @Relationship_7"
                            + " ,家族構成続柄内容_7 = @RelationshipContent_7"
                            + " ,家族構成続柄内容_7_Other = @RelationshipContentOther_7"
							+ " ,家族構成年齢_7 = @FamilyAge_7"
							+ " ,家族構成職業学年_7 = @FamilyJob_7"
							+ " ,家族構成同居区分_7 = @FamilyLodger_7"
							+ " ,家族構成扶養区分_7 = @FamilyDependent_7"
                            + " ,家族構成続柄区分_8 = @Relationship_8"
                            + " ,家族構成続柄内容_8 = @RelationshipContent_8"
                            + " ,家族構成続柄内容_8_Other = @RelationshipContentOther_8"
							+ " ,家族構成年齢_8 = @FamilyAge_8"
							+ " ,家族構成職業学年_8 = @FamilyJob_8"
							+ " ,家族構成同居区分_8 = @FamilyLodger_8"
							+ " ,家族構成扶養区分_8 = @FamilyDependent_8"
                            + " ,健康状態区分 = @Health"
                            + " ,健康状態内容 = @HealthContent"
                            + " ,健康状態不順状態 = @UnHealthContent"
                            
                            + " where 管理番号 = @ManageNo";
                        //SQL文の型を指定
                        var cmd = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@PostalCode_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@PostalCode_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Address", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@AddressType", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@AddressTypeContent", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@FamilyCount", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_1", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_2", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_3", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_3", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_3", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_3", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_3", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_3", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_3", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_4", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_4", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_4", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_4", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_4", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_4", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_4", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_5", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_5", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_5", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_5", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_5", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_5", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_5", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_6", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_6", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_6", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_6", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_6", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_6", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_6", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_7", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_7", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_7", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_7", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_7", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_7", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_7", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Relationship_8", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContent_8", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@RelationshipContentOther_8", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyAge_8", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyJob_8", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyLodger_8", DbType.String);
						DbHelper.AddDbParameter(cmd, "@FamilyDependent_8", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@Health", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@HealthContent", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@UnHealthContent", DbType.String);

                        //パラメータ設定
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = DataConv.IfNull(model.Head.ManageNo);
                        parameters[1].Value = DataConv.IfNull(model.Head.PostalCode_1);
                        parameters[2].Value = DataConv.IfNull(model.Head.PostalCode_2);
                        parameters[3].Value = DataConv.IfNull(model.Head.Address);
                        parameters[4].Value = DataConv.IfNull(splitPre(model.Head.AddressType));
                        parameters[5].Value = DataConv.IfNull(splitSuf(model.Head.AddressType));
                        parameters[6].Value = DataConv.IfNull(model.Head.FamilyCount);
                        parameters[7].Value = DataConv.IfNull(splitPre(model.Head.Relationship_1));
                        parameters[8].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_1));
                        parameters[9].Value = DataConv.IfNull(model.Head.RelationshipContentOther_1);
						parameters[10].Value = DataConv.IfNull(model.Head.FamilyAge_1);
						parameters[11].Value = DataConv.IfNull(model.Head.FamilyJob_1);
						parameters[12].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_1));
						parameters[13].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_1));
                        parameters[14].Value = DataConv.IfNull(splitPre(model.Head.Relationship_2));
                        parameters[15].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_2));
                        parameters[16].Value = DataConv.IfNull(model.Head.RelationshipContentOther_2);
						parameters[17].Value = DataConv.IfNull(model.Head.FamilyAge_2);
						parameters[18].Value = DataConv.IfNull(model.Head.FamilyJob_2);
						parameters[19].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_2));
						parameters[20].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_2));
                        parameters[21].Value = DataConv.IfNull(splitPre(model.Head.Relationship_3));
                        parameters[22].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_3));
                        parameters[23].Value = DataConv.IfNull(model.Head.RelationshipContentOther_3);
						parameters[24].Value = DataConv.IfNull(model.Head.FamilyAge_3);
						parameters[25].Value = DataConv.IfNull(model.Head.FamilyJob_3);
						parameters[26].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_3));
						parameters[27].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_3));
                        parameters[28].Value = DataConv.IfNull(splitPre(model.Head.Relationship_4));
                        parameters[29].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_4));
                        parameters[30].Value = DataConv.IfNull(model.Head.RelationshipContentOther_4);
						parameters[31].Value = DataConv.IfNull(model.Head.FamilyAge_4);
						parameters[32].Value = DataConv.IfNull(model.Head.FamilyJob_4);
						parameters[33].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_4));
						parameters[34].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_4));
                        parameters[35].Value = DataConv.IfNull(splitPre(model.Head.Relationship_5));
                        parameters[36].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_5));
                        parameters[37].Value = DataConv.IfNull(model.Head.RelationshipContentOther_5);
						parameters[38].Value = DataConv.IfNull(model.Head.FamilyAge_5);
						parameters[39].Value = DataConv.IfNull(model.Head.FamilyJob_5);
						parameters[40].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_5));
						parameters[41].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_5));
                        parameters[42].Value = DataConv.IfNull(splitPre(model.Head.Relationship_6));
                        parameters[43].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_6));
                        parameters[44].Value = DataConv.IfNull(model.Head.RelationshipContentOther_6);
						parameters[45].Value = DataConv.IfNull(model.Head.FamilyAge_6);
						parameters[46].Value = DataConv.IfNull(model.Head.FamilyJob_6);
						parameters[47].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_6));
						parameters[48].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_6));
                        parameters[49].Value = DataConv.IfNull(splitPre(model.Head.Relationship_7));
                        parameters[50].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_7));
                        parameters[51].Value = DataConv.IfNull(model.Head.RelationshipContentOther_7);
						parameters[52].Value = DataConv.IfNull(model.Head.FamilyAge_7);
						parameters[53].Value = DataConv.IfNull(model.Head.FamilyJob_7);
						parameters[54].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_7));
						parameters[55].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_7));
                        parameters[56].Value = DataConv.IfNull(splitPre(model.Head.Relationship_8));
                        parameters[57].Value = DataConv.IfNull(splitSuf(model.Head.Relationship_8));
                        parameters[58].Value = DataConv.IfNull(model.Head.RelationshipContentOther_8);
						parameters[59].Value = DataConv.IfNull(model.Head.FamilyAge_8);
						parameters[60].Value = DataConv.IfNull(model.Head.FamilyJob_8);
						parameters[61].Value = DataConv.IfNull(checkValue(model.Head.FamilyLodger_8));
						parameters[62].Value = DataConv.IfNull(checkValue(model.Head.FamilyDependent_8));
                        parameters[63].Value = DataConv.IfNull(splitPre(model.Head.Health));
                        parameters[64].Value = DataConv.IfNull(splitSuf(model.Head.Health));
                        parameters[65].Value = DataConv.IfNull(model.Head.UnHealthContent);
                        cmd.ExecuteNonQuery();

                        
                        //詳細内容
                        sql = "update SD_T自己申告書明細DataC01"
                            + " set "
                            + "  配置換_1 = @TransferDepartment_1"
                            + " ,配置換区分_2_1 = @TransferDepartment_2_1"
                            + " ,配置換内容_2_1 = @TransferDepartment_1_1_Content"
                            + " ,配置換区分_2_2 = @TransferDepartment_2_2"
                            + " ,配置換内容_2_2 = @TransferDepartment_2_2_Content"
                            + " ,配置換区分_2_3 = @TransferDepartment_2_3"
                            + " ,配置換内容_2_3 = @TransferDepartment_2_3_Content"
                            + " ,配置換内容_2_Other = @TransferDepartment_2_Other"
                            + " ,担当職務_1 = @ChargeDuty_1"
                            + " ,担当職務_2 = @ChargeDuty_2"
                            + " ,能力開発_1 = @AptitudeDevelop_1"
                            + " ,その他 = @OtherComment"
                            + " ,自由意見内容 = @FreeComment"
                            + " ,上司記入欄内容 = @BossComment"

                            + " where 管理番号 = @ManageNo";

                        //SQL文の型を指定
                        cmd = dm.CreateCommand(sql);
                        DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_1_1_Content", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_2_Content", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_3", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_3_Content", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@TransferDepartment_2_Other", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@ChargeDuty_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@ChargeDuty_2", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@AptitudeDevelop_1", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@OtherComment", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@FreeComment", DbType.String);
                        DbHelper.AddDbParameter(cmd, "@BossComment", DbType.String);

                        //パラメータ設定
                        parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = DataConv.IfNull(model.Head.ManageNo);
                        parameters[1].Value = DataConv.IfNull(model.Body.TransferDepartment_1);
                        parameters[2].Value = DataConv.IfNull(splitPre(model.Body.TransferDepartment_2_1));
                        parameters[3].Value = DataConv.IfNull(splitSuf(model.Body.TransferDepartment_2_1));
                        parameters[4].Value = DataConv.IfNull(splitPre(model.Body.TransferDepartment_2_2));
                        parameters[5].Value = DataConv.IfNull(splitSuf(model.Body.TransferDepartment_2_2));
                        parameters[6].Value = DataConv.IfNull(splitPre(model.Body.TransferDepartment_2_3));
                        parameters[7].Value = DataConv.IfNull(splitSuf(model.Body.TransferDepartment_2_3));
                        parameters[8].Value = DataConv.IfNull(model.Body.TransferDepartment_2_Other);
                        parameters[9].Value = DataConv.IfNull(model.Body.ChargeDuty_1);
                        parameters[10].Value = DataConv.IfNull(model.Body.ChargeDuty_2);
                        parameters[11].Value = DataConv.IfNull(model.Body.AptitudeDevelop_1);
                        parameters[12].Value = DataConv.IfNull(model.Body.OtherComment);
                        parameters[13].Value = DataConv.IfNull(model.Body.FreeComment);
                        parameters[14].Value = DataConv.IfNull(model.Body.BossComment);

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
        /// 承認
        /// </summary>
        /// <param name="model">目標管理入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        public void Sign(SelfDeclareRegisterCViewModels model, string value, LoginUser lu, bool isSign = true) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                string[] segments = value.Split(',');
                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);

                //保存
                this.Save(model);

                using(var scope = new TransactionScope()) {
                    var dbm = new DbManager();
                    //目標管理承認
                    var sql = "update SD_T自己申告書承認情報"
                               + " set "
                               + " 承認社員番号 = @SignEmployeeNo"
                               + ",承認日時 = @SignDate"
                               + " where 管理番号 = @ManageNo"
                               + "   and 大区分 = @Large"
                               + "   and 小区分 = @Small";
                    //SQL文の型を指定
                    IDbCommand cmd = dbm.CreateCommand(sql);
                    DbHelper.AddDbParameter(cmd, "@SignEmployeeNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@SignDate", DbType.DateTime);
                    DbHelper.AddDbParameter(cmd, "@ManageNo", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Large", DbType.String);
                    DbHelper.AddDbParameter(cmd, "@Small", DbType.String);
                    //パラメータ設定
                    var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                    parameters[0].Value = isSign ? DataConv.IfNull(lu.UserCode) : System.Data.SqlTypes.SqlString.Null;
                    parameters[1].Value = isSign ? DataConv.IfNull(DateTime.Now.ToString()) : System.Data.SqlTypes.SqlString.Null;
                    parameters[2].Value = DataConv.IfNull(model.Head.ManageNo);
                    parameters[3].Value = DataConv.IfNull(segments[0]);
                    parameters[4].Value = DataConv.IfNull(segments[1]);
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
        /// 前年度コピー
        /// </summary>
        /// <param name="model">自己申告書入力モデル</param>
        /// <param name="value">ボタンのValue</param>
        public void PrevDataCopy(SelfDeclareRegisterCViewModels model, LoginUser lu) {
            try {
                //開始
                nlog.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name + " start");

                DateTime dt = DateTime.Now;
                string logDate = string.Format("{0:d} {1:g}", dt.Date, dt.TimeOfDay);

                ////保存
                //this.Save(model);


                string sql = "SD_SP自己申告書共通基本Data前年度Copy";
                //実行時間取得
                Configuration config = WebConfig.GetConfigFile();
                var timeout = config.AppSettings.Settings["SQL_TIMEOUT"].Value;

                using(var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10.0))) {
                    using(DbManager dm = new DbManager())
                    using(IDbCommand cmd = dm.CreateCommand(sql, string.IsNullOrEmpty(timeout) ? "300" : timeout)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("YearParam", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("EmployeeParam", SqlDbType.NVarChar));
                        var parameters = cmd.Parameters.Cast<IDbDataParameter>().ToArray<IDbDataParameter>();
                        parameters[0].Value = model.Head.SheetYear;
                        parameters[1].Value = model.Head.EmployeeNo;
                        nlog.Debug(sql + " start");
                        int result = cmd.ExecuteNonQuery();
                        nlog.Debug(sql + " end");
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
    
    }
}
