using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Enum;

namespace EmployeeAdjustmentConnectionSystem.COM.Models {
    /// <summary>
    /// 保険料控除申告書入力モデル
    /// </summary>
    public class HokenDeclareRegisterViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public HokenDeclareRegisterHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public HokenDeclareRegisterBodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class HokenDeclareRegisterHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HokenDeclareRegisterHeaderModel() {
            //全フィールドに空文字セット リフレクション版
            Type type = this.GetType();
            PropertyInfo[] pInfos = type.GetProperties();
            object value;
            foreach(PropertyInfo pinfo in pInfos) {
                value = null;
                if(pinfo.PropertyType == typeof(string))
                    value = "";
                else if(pinfo.PropertyType == typeof(bool))
                    value = false;

                pinfo.SetValue(this, value, null);
            }
        }

        /// <summary>
        /// 入力モード
        /// </summary>
        public ajustMode InputMode { get; set; }
        /// <summary>
        /// キャンセルボタン
        /// </summary>
        public ajustMode CancelButton { get; set; }
        /// <summary>
        /// 承認ボタン
        /// </summary>
        public ajustMode AuthButton { get; set; }

        /// <summary>
        /// 管理番号
        /// </summary>
        public string ManageNo { get; set; }

        /// <summary>
        /// 管理者モード
        /// </summary>
        public bool AdminMode { get; set; }

        /// <summary>
		/// 対象年度
		/// </summary>
		public int? SheetYear { get; set; }


		/// <summary>
		/// 本人確定区分
		/// </summary>
		public string ApprovalType { get; set; }


		/// <summary>
		/// 管理者確定区分
		/// </summary>
		public string DecisionType { get; set; }

        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// ステータス名
		/// </summary>
		public string StatusName { get; set; }
        //2023-11-20 iwai-tamura upd end -----

		/// <summary>
		/// 社員番号
		/// </summary>
		public string EmployeeNo { get; set; }


		/// <summary>
		/// 所属番号
		/// </summary>
		public int? DepartmentNo { get; set; }


		/// <summary>
		/// 所属名
		/// </summary>
		public string DepartmentName { get; set; }


		/// <summary>
		/// 氏名_姓
		/// </summary>
		public string Name1 { get; set; }


		/// <summary>
		/// 氏名_名
		/// </summary>
		public string Name2 { get; set; }


		/// <summary>
		/// Kana_姓
		/// </summary>
		public string Kana1 { get; set; }


		/// <summary>
		/// Kana_名
		/// </summary>
		public string Kana2 { get; set; }


		/// <summary>
		/// 住所01
		/// </summary>
		public string Address { get; set; }


		/// <summary>
		/// 生命保険料控除額計
		/// </summary>
		public int? AllLifeInsurance_DeductionAmount { get; set; }


		/// <summary>
		/// 一般生命保険料新保険料合計
		/// </summary>
		public int? LifeInsurance_NewTotalAmount { get; set; }


		/// <summary>
		/// 一般生命保険料旧保険料合計
		/// </summary>
		public int? LifeInsurance_OldTotalAmount { get; set; }


		/// <summary>
		/// 一般生命保険料新保険料表計算
		/// </summary>
		public int? LifeInsurance_Calc1 { get; set; }


        //2026-99-99 iwai-miki add str -----
        /// <summary>
        /// 一般生命保険料扶養親族保険料表計算
        /// </summary>
        public int? LifeInsurance_Calc3 { get; set; }
        //2026-99-99 iwai-miki add end -----


        /// <summary>
        /// 一般生命保険料旧保険料表計算
        /// </summary>
        public int? LifeInsurance_Calc2 { get; set; }


		/// <summary>
		/// 一般生命保険料表合計
		/// </summary>
		public int? LifeInsurance_TotalAmount { get; set; }


        //2026-99-99 iwai-miki add str -----
        /// <summary>
        /// 一般生命保険料表扶養親族合計
        /// </summary>
        public int? LifeInsurance_TotalAmount2 { get; set; }
        //2026-99-99 iwai-miki add end -----


        /// <summary>
        /// 一般生命保険料比較
        /// </summary>
        public int? LifeInsurance_DeductionAmount { get; set; }

        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 一般生命保険料01_HostData判定
		/// </summary>
		public string LifeInsurance_1_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----

		/// <summary>
		/// 一般生命保険料01_保険会社等名称
		/// </summary>
		public string LifeInsurance_1_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険等種類
		/// </summary>
		public string LifeInsurance_1_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料01_期間
		/// </summary>
		public string LifeInsurance_1_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_1_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_1_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_1_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_1_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料01_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_1_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料01_新旧
		/// </summary>
		public string LifeInsurance_1_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料01_支払金額
		/// </summary>
		public int? LifeInsurance_1_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 一般生命保険料02_HostData判定
		/// </summary>
		public string LifeInsurance_2_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----

		/// <summary>
		/// 一般生命保険料02_保険会社等名称
		/// </summary>
		public string LifeInsurance_2_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険等種類
		/// </summary>
		public string LifeInsurance_2_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料02_期間
		/// </summary>
		public string LifeInsurance_2_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_2_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_2_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_2_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_2_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料02_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_2_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料02_新旧
		/// </summary>
		public string LifeInsurance_2_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料02_支払金額
		/// </summary>
		public int? LifeInsurance_2_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 一般生命保険料03_HostData判定
		/// </summary>
		public string LifeInsurance_3_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 一般生命保険料03_保険会社等名称
		/// </summary>
		public string LifeInsurance_3_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険等種類
		/// </summary>
		public string LifeInsurance_3_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料03_期間
		/// </summary>
		public string LifeInsurance_3_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_3_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_3_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_3_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_3_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料03_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_3_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料03_新旧
		/// </summary>
		public string LifeInsurance_3_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料03_支払金額
		/// </summary>
		public int? LifeInsurance_3_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 一般生命保険料04_HostData判定
		/// </summary>
		public string LifeInsurance_4_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 一般生命保険料04_保険会社等名称
		/// </summary>
		public string LifeInsurance_4_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険等種類
		/// </summary>
		public string LifeInsurance_4_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料04_期間
		/// </summary>
		public string LifeInsurance_4_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_4_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_4_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_4_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_4_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料04_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_4_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料04_新旧
		/// </summary>
		public string LifeInsurance_4_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料04_支払金額
		/// </summary>
		public int? LifeInsurance_4_InsuranceFee { get; set; }




		//2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 一般生命保険料05_HostData判定
		/// </summary>
		public string LifeInsurance_5_HostDataFlg { get; set; }

		/// <summary>
		/// 一般生命保険料05_保険会社等名称
		/// </summary>
		public string LifeInsurance_5_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険等種類
		/// </summary>
		public string LifeInsurance_5_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料05_期間
		/// </summary>
		public string LifeInsurance_5_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_5_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_5_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_5_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_5_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料05_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_5_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料05_新旧
		/// </summary>
		public string LifeInsurance_5_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料05_支払金額
		/// </summary>
		public int? LifeInsurance_5_InsuranceFee { get; set; }


		/// <summary>
		/// 一般生命保険料06_HostData判定
		/// </summary>
		public string LifeInsurance_6_HostDataFlg { get; set; }

		/// <summary>
		/// 一般生命保険料06_保険会社等名称
		/// </summary>
		public string LifeInsurance_6_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険等種類
		/// </summary>
		public string LifeInsurance_6_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料06_期間
		/// </summary>
		public string LifeInsurance_6_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_6_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_6_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_6_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_6_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料06_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_6_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料06_新旧
		/// </summary>
		public string LifeInsurance_6_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料06_支払金額
		/// </summary>
		public int? LifeInsurance_6_InsuranceFee { get; set; }


		/// <summary>
		/// 一般生命保険料07_HostData判定
		/// </summary>
		public string LifeInsurance_7_HostDataFlg { get; set; }

		/// <summary>
		/// 一般生命保険料07_保険会社等名称
		/// </summary>
		public string LifeInsurance_7_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険等種類
		/// </summary>
		public string LifeInsurance_7_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料07_期間
		/// </summary>
		public string LifeInsurance_7_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_7_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_7_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_7_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_7_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料07_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_7_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料07_新旧
		/// </summary>
		public string LifeInsurance_7_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料07_支払金額
		/// </summary>
		public int? LifeInsurance_7_InsuranceFee { get; set; }



		/// <summary>
		/// 一般生命保険料08_HostData判定
		/// </summary>
		public string LifeInsurance_8_HostDataFlg { get; set; }

		/// <summary>
		/// 一般生命保険料08_保険会社等名称
		/// </summary>
		public string LifeInsurance_8_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険等種類
		/// </summary>
		public string LifeInsurance_8_InsuranceTypeName { get; set; }


		/// <summary>
		/// 一般生命保険料08_期間
		/// </summary>
		public string LifeInsurance_8_InsurancePeriod { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険等契約者氏名_姓
		/// </summary>
		public string LifeInsurance_8_ContractorName1 { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険等契約者氏名_名
		/// </summary>
		public string LifeInsurance_8_ContractorName2 { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険金等受取人氏名_姓
		/// </summary>
		public string LifeInsurance_8_ReceiverName1 { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険金等受取人氏名_名
		/// </summary>
		public string LifeInsurance_8_ReceiverName2 { get; set; }


		/// <summary>
		/// 一般生命保険料08_保険金等受取人続柄
		/// </summary>
		public string LifeInsurance_8_RelationshipType { get; set; }


		/// <summary>
		/// 一般生命保険料08_新旧
		/// </summary>
		public string LifeInsurance_8_OldAndNewType { get; set; }


		/// <summary>
		/// 一般生命保険料08_支払金額
		/// </summary>
		public int? LifeInsurance_8_InsuranceFee { get; set; }

        //2023-11-20 iwai-tamura upd end -----


        //2026-99-99 iwai-miki add end -----
        /// <summary>
        /// 扶養親族23未満_続柄
        /// </summary>
        public string DependentsUnder23_RelationshipType { get; set; }


        /// <summary>
        /// 扶養親族23未満_氏名_姓
        /// </summary>
        public string DependentsUnder23_Name1 { get; set; }


        /// <summary>
        /// 扶養親族23未満_氏名_名
        /// </summary>
        public string DependentsUnder23_Name2 { get; set; }


        /// <summary>
        /// 扶養親族23未満_Kana_姓
        /// </summary>
        public string DependentsUnder23_Kana1 { get; set; }


        /// <summary>
        /// 扶養親族23未満_Kana_名
        /// </summary>
        public string DependentsUnder23_Kana2 { get; set; }


        /// <summary>
        /// 扶養親族23未満_生年月日
        /// </summary>
        public string DependentsUnder23_Birthday { get; set; }


        /// <summary>
        /// 扶養親族23未満_生年月日_年
        /// </summary>
        public string DependentsUnder23_BirthdayYear { get; set; }


        /// <summary>
        /// 扶養親族23未満_生年月日_月
        /// </summary>
        public string DependentsUnder23_BirthdayMonth { get; set; }


        /// <summary>
        /// 扶養親族23未満_生年月日_日
        /// </summary>
        public string DependentsUnder23_BirthdayDay { get; set; }


        /// <summary>
        /// 扶養親族23未満_同上区分
        /// </summary>
        public string DependentsUnder23_AddressSameCheck { get; set; }


        /// <summary>
        /// 扶養親族23未満_住所
        /// </summary>
        public string DependentsUnder23_Address { get; set; }


        /// <summary>
        /// 扶養親族23未満_給与所得_収入金額
        /// </summary>
        public int? DependentsUnder23_Earnings { get; set; }


        /// <summary>
        /// 扶養親族23未満_給与所得_所得金額
        /// </summary>
        public int? DependentsUnder23_Earnings2Income { get; set; }


        /// <summary>
        /// 扶養親族23未満_他_所得金額
        /// </summary>
        public int? DependentsUnder23_OtherIncome { get; set; }


        /// <summary>
        /// 扶養親族23未満_合計所得金額見積額
        /// </summary>
        public int? DependentsUnder23_Income { get; set; }
        //2026-99-99 iwai-miki add end -----


        /// <summary>
        /// 介護医療保険料合計
        /// </summary>
        public int? MedicalInsurance_TotalAmount { get; set; }


		/// <summary>
		/// 介護医療保険料表計算
		/// </summary>
		public int? MedicalInsurance_DeductionAmount { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 介護医療保険料01_HostData判定
		/// </summary>
		public string MedicalInsurance_1_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 介護医療保険料01_保険会社等名称
		/// </summary>
		public string MedicalInsurance_1_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険等種類
		/// </summary>
		public string MedicalInsurance_1_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料01_期間
		/// </summary>
		public string MedicalInsurance_1_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_1_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_1_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_1_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_1_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料01_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_1_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料01_支払金額
		/// </summary>
		public int? MedicalInsurance_1_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 介護医療保険料02_HostData判定
		/// </summary>
		public string MedicalInsurance_2_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 介護医療保険料02_保険会社等名称
		/// </summary>
		public string MedicalInsurance_2_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険等種類
		/// </summary>
		public string MedicalInsurance_2_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料02_期間
		/// </summary>
		public string MedicalInsurance_2_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_2_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_2_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_2_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_2_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料02_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_2_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料02_支払金額
		/// </summary>
		public int? MedicalInsurance_2_InsuranceFee { get; set; }


		//2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 介護医療保険料03_HostData判定
		/// </summary>
		public string MedicalInsurance_3_HostDataFlg { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険会社等名称
		/// </summary>
		public string MedicalInsurance_3_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険等種類
		/// </summary>
		public string MedicalInsurance_3_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料03_期間
		/// </summary>
		public string MedicalInsurance_3_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_3_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_3_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_3_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_3_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料03_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_3_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料03_支払金額
		/// </summary>
		public int? MedicalInsurance_3_InsuranceFee { get; set; }


		/// <summary>
		/// 介護医療保険料04_HostData判定
		/// </summary>
		public string MedicalInsurance_4_HostDataFlg { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険会社等名称
		/// </summary>
		public string MedicalInsurance_4_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険等種類
		/// </summary>
		public string MedicalInsurance_4_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料04_期間
		/// </summary>
		public string MedicalInsurance_4_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_4_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_4_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_4_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_4_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料04_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_4_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料04_支払金額
		/// </summary>
		public int? MedicalInsurance_4_InsuranceFee { get; set; }


		/// <summary>
		/// 介護医療保険料05_HostData判定
		/// </summary>
		public string MedicalInsurance_5_HostDataFlg { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険会社等名称
		/// </summary>
		public string MedicalInsurance_5_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険等種類
		/// </summary>
		public string MedicalInsurance_5_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料05_期間
		/// </summary>
		public string MedicalInsurance_5_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_5_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_5_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_5_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_5_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料05_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_5_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料05_支払金額
		/// </summary>
		public int? MedicalInsurance_5_InsuranceFee { get; set; }


		/// <summary>
		/// 介護医療保険料06_HostData判定
		/// </summary>
		public string MedicalInsurance_6_HostDataFlg { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険会社等名称
		/// </summary>
		public string MedicalInsurance_6_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険等種類
		/// </summary>
		public string MedicalInsurance_6_InsuranceTypeName { get; set; }


		/// <summary>
		/// 介護医療保険料06_期間
		/// </summary>
		public string MedicalInsurance_6_InsurancePeriod { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険等契約者氏名_姓
		/// </summary>
		public string MedicalInsurance_6_ContractorName1 { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険等契約者氏名_名
		/// </summary>
		public string MedicalInsurance_6_ContractorName2 { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険金等受取人氏名_姓
		/// </summary>
		public string MedicalInsurance_6_ReceiverName1 { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険金等受取人氏名_名
		/// </summary>
		public string MedicalInsurance_6_ReceiverName2 { get; set; }


		/// <summary>
		/// 介護医療保険料06_保険金等受取人続柄
		/// </summary>
		public string MedicalInsurance_6_RelationshipType { get; set; }


		/// <summary>
		/// 介護医療保険料06_支払金額
		/// </summary>
		public int? MedicalInsurance_6_InsuranceFee { get; set; }
		//2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 個人年金保険料新保険料合計
		/// </summary>
		public int? PensionInsurance_NewTotalAmount { get; set; }


		/// <summary>
		/// 個人年金保険料旧保険料合計
		/// </summary>
		public int? PensionInsurance_OldTotalAmount { get; set; }


		/// <summary>
		/// 個人年金保険料新保険料表計算
		/// </summary>
		public int? PensionInsurance_Calc1 { get; set; }


		/// <summary>
		/// 個人年金保険料旧保険料表計算
		/// </summary>
		public int? PensionInsurance_Calc2 { get; set; }


		/// <summary>
		/// 個人年金保険料表合計
		/// </summary>
		public int? PensionInsurance_TotalAmount { get; set; }


		/// <summary>
		/// 個人年金保険料比較
		/// </summary>
		public int? PensionInsurance_DeductionAmount { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 個人年金保険料01_HostData判定
		/// </summary>
		public string PensionInsurance_1_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 個人年金保険料01_保険会社等名称
		/// </summary>
		public string PensionInsurance_1_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険等種類
		/// </summary>
		public string PensionInsurance_1_InsuranceTypeName { get; set; }


		/// <summary>
		/// 個人年金保険料01_期間
		/// </summary>
		public string PensionInsurance_1_InsurancePeriod { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険等契約者氏名_姓
		/// </summary>
		public string PensionInsurance_1_ContractorName1 { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険等契約者氏名_名
		/// </summary>
		public string PensionInsurance_1_ContractorName2 { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険金等受取人氏名_姓
		/// </summary>
		public string PensionInsurance_1_ReceiverName1 { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険金等受取人氏名_名
		/// </summary>
		public string PensionInsurance_1_ReceiverName2 { get; set; }


		/// <summary>
		/// 個人年金保険料01_保険金等受取人続柄
		/// </summary>
		public string PensionInsurance_1_RelationshipType { get; set; }


		/// <summary>
		/// 個人年金保険料01_支払開始日
		/// </summary>
		public string PensionInsurance_1_StartPayment { get; set; }


		/// <summary>
		/// 個人年金保険料01_支払開始日_年
		/// </summary>
		public string PensionInsurance_1_StartPaymentYear { get; set; }


		/// <summary>
		/// 個人年金保険料01_支払開始日_月
		/// </summary>
		public string PensionInsurance_1_StartPaymentMonth { get; set; }


		/// <summary>
		/// 個人年金保険料01_支払開始日_日
		/// </summary>
		public string PensionInsurance_1_StartPaymentDay { get; set; }


		/// <summary>
		/// 個人年金保険料01_新旧
		/// </summary>
		public string PensionInsurance_1_OldAndNewType { get; set; }


		/// <summary>
		/// 個人年金保険料01_支払金額
		/// </summary>
		public int? PensionInsurance_1_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 個人年金保険料02_HostData判定
		/// </summary>
		public string PensionInsurance_2_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 個人年金保険料02_保険会社等名称
		/// </summary>
		public string PensionInsurance_2_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険等種類
		/// </summary>
		public string PensionInsurance_2_InsuranceTypeName { get; set; }


		/// <summary>
		/// 個人年金保険料02_期間
		/// </summary>
		public string PensionInsurance_2_InsurancePeriod { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険等契約者氏名_姓
		/// </summary>
		public string PensionInsurance_2_ContractorName1 { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険等契約者氏名_名
		/// </summary>
		public string PensionInsurance_2_ContractorName2 { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険金等受取人氏名_姓
		/// </summary>
		public string PensionInsurance_2_ReceiverName1 { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険金等受取人氏名_名
		/// </summary>
		public string PensionInsurance_2_ReceiverName2 { get; set; }


		/// <summary>
		/// 個人年金保険料02_保険金等受取人続柄
		/// </summary>
		public string PensionInsurance_2_RelationshipType { get; set; }


		/// <summary>
		/// 個人年金保険料02_支払開始日
		/// </summary>
		public string PensionInsurance_2_StartPayment { get; set; }


		/// <summary>
		/// 個人年金保険料02_支払開始日_年
		/// </summary>
		public string PensionInsurance_2_StartPaymentYear { get; set; }


		/// <summary>
		/// 個人年金保険料02_支払開始日_月
		/// </summary>
		public string PensionInsurance_2_StartPaymentMonth { get; set; }


		/// <summary>
		/// 個人年金保険料02_支払開始日_日
		/// </summary>
		public string PensionInsurance_2_StartPaymentDay { get; set; }


		/// <summary>
		/// 個人年金保険料02_新旧
		/// </summary>
		public string PensionInsurance_2_OldAndNewType { get; set; }


		/// <summary>
		/// 個人年金保険料02_支払金額
		/// </summary>
		public int? PensionInsurance_2_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 個人年金保険料03_HostData判定
		/// </summary>
		public string PensionInsurance_3_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 個人年金保険料03_保険会社等名称
		/// </summary>
		public string PensionInsurance_3_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険等種類
		/// </summary>
		public string PensionInsurance_3_InsuranceTypeName { get; set; }


		/// <summary>
		/// 個人年金保険料03_期間
		/// </summary>
		public string PensionInsurance_3_InsurancePeriod { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険等契約者氏名_姓
		/// </summary>
		public string PensionInsurance_3_ContractorName1 { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険等契約者氏名_名
		/// </summary>
		public string PensionInsurance_3_ContractorName2 { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険金等受取人氏名_姓
		/// </summary>
		public string PensionInsurance_3_ReceiverName1 { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険金等受取人氏名_名
		/// </summary>
		public string PensionInsurance_3_ReceiverName2 { get; set; }


		/// <summary>
		/// 個人年金保険料03_保険金等受取人続柄
		/// </summary>
		public string PensionInsurance_3_RelationshipType { get; set; }


		/// <summary>
		/// 個人年金保険料03_支払開始日
		/// </summary>
		public string PensionInsurance_3_StartPayment { get; set; }


		/// <summary>
		/// 個人年金保険料03_支払開始日_年
		/// </summary>
		public string PensionInsurance_3_StartPaymentYear { get; set; }


		/// <summary>
		/// 個人年金保険料03_支払開始日_月
		/// </summary>
		public string PensionInsurance_3_StartPaymentMonth { get; set; }


		/// <summary>
		/// 個人年金保険料03_支払開始日_日
		/// </summary>
		public string PensionInsurance_3_StartPaymentDay { get; set; }


		/// <summary>
		/// 個人年金保険料03_新旧
		/// </summary>
		public string PensionInsurance_3_OldAndNewType { get; set; }


		/// <summary>
		/// 個人年金保険料03_支払金額
		/// </summary>
		public int? PensionInsurance_3_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 個人年金保険料04_HostData判定
		/// </summary>
		public string PensionInsurance_4_HostDataFlg { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険会社等名称
		/// </summary>
		public string PensionInsurance_4_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険等種類
		/// </summary>
		public string PensionInsurance_4_InsuranceTypeName { get; set; }


		/// <summary>
		/// 個人年金保険料04_期間
		/// </summary>
		public string PensionInsurance_4_InsurancePeriod { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険等契約者氏名_姓
		/// </summary>
		public string PensionInsurance_4_ContractorName1 { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険等契約者氏名_名
		/// </summary>
		public string PensionInsurance_4_ContractorName2 { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険金等受取人氏名_姓
		/// </summary>
		public string PensionInsurance_4_ReceiverName1 { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険金等受取人氏名_名
		/// </summary>
		public string PensionInsurance_4_ReceiverName2 { get; set; }


		/// <summary>
		/// 個人年金保険料04_保険金等受取人続柄
		/// </summary>
		public string PensionInsurance_4_RelationshipType { get; set; }


		/// <summary>
		/// 個人年金保険料04_支払開始日
		/// </summary>
		public string PensionInsurance_4_StartPayment { get; set; }


		/// <summary>
		/// 個人年金保険料04_支払開始日_年
		/// </summary>
		public string PensionInsurance_4_StartPaymentYear { get; set; }


		/// <summary>
		/// 個人年金保険料04_支払開始日_月
		/// </summary>
		public string PensionInsurance_4_StartPaymentMonth { get; set; }


		/// <summary>
		/// 個人年金保険料04_支払開始日_日
		/// </summary>
		public string PensionInsurance_4_StartPaymentDay { get; set; }


		/// <summary>
		/// 個人年金保険料04_新旧
		/// </summary>
		public string PensionInsurance_4_OldAndNewType { get; set; }


		/// <summary>
		/// 個人年金保険料04_支払金額
		/// </summary>
		public int? PensionInsurance_4_InsuranceFee { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 地震保険料控除地震保険料合計
		/// </summary>
		public int? QuakeInsurance_QuakeAmount { get; set; }


		/// <summary>
		/// 地震保険料控除旧長期損害保険料合計
		/// </summary>
		public int? QuakeInsurance_DamageTotalAmount { get; set; }


		/// <summary>
		/// 地震保険料控除額金額01
		/// </summary>
		public int? QuakeInsurance_Calc1 { get; set; }


		/// <summary>
		/// 地震保険料控除額金額02
		/// </summary>
		public int? QuakeInsurance_Calc2 { get; set; }


		/// <summary>
		/// 地震保険料控除額金額合計
		/// </summary>
		public int? QuakeInsurance_DeductionAmount { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 地震保険料控除01_HostData判定
		/// </summary>
		public string QuakeInsurance_1_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 地震保険料控除01_保険会社等名称
		/// </summary>
		public string QuakeInsurance_1_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等種類
		/// </summary>
		public string QuakeInsurance_1_InsuranceTypeName { get; set; }


		/// <summary>
		/// 地震保険料控除01_期間
		/// </summary>
		public string QuakeInsurance_1_InsurancePeriod { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等契約者氏名_姓
		/// </summary>
		public string QuakeInsurance_1_ContractorName1 { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等契約者氏名_名
		/// </summary>
		public string QuakeInsurance_1_ContractorName2 { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等対象氏名_姓
		/// </summary>
		public string QuakeInsurance_1_ReceiverName1 { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等対象氏名_名
		/// </summary>
		public string QuakeInsurance_1_ReceiverName2 { get; set; }


		/// <summary>
		/// 地震保険料控除01_保険等対象続柄
		/// </summary>
		public string QuakeInsurance_1_RelationshipType { get; set; }


		/// <summary>
		/// 地震保険料控除01_地震旧長期
		/// </summary>
		public string QuakeInsurance_1_QuakeAndDamageType { get; set; }


		/// <summary>
		/// 地震保険料控除01_支払保険料
		/// </summary>
		public int? QuakeInsurance_1_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 地震保険料控除02_HostData判定
		/// </summary>
		public string QuakeInsurance_2_HostDataFlg { get; set; }
        //2023-11-20 iwai-tamura upd end -----


		/// <summary>
		/// 地震保険料控除02_保険会社等名称
		/// </summary>
		public string QuakeInsurance_2_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等種類
		/// </summary>
		public string QuakeInsurance_2_InsuranceTypeName { get; set; }


		/// <summary>
		/// 地震保険料控除02_期間
		/// </summary>
		public string QuakeInsurance_2_InsurancePeriod { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等契約者氏名_姓
		/// </summary>
		public string QuakeInsurance_2_ContractorName1 { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等契約者氏名_名
		/// </summary>
		public string QuakeInsurance_2_ContractorName2 { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等対象氏名_姓
		/// </summary>
		public string QuakeInsurance_2_ReceiverName1 { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等対象氏名_名
		/// </summary>
		public string QuakeInsurance_2_ReceiverName2 { get; set; }


		/// <summary>
		/// 地震保険料控除02_保険等対象続柄
		/// </summary>
		public string QuakeInsurance_2_RelationshipType { get; set; }


		/// <summary>
		/// 地震保険料控除02_地震旧長期
		/// </summary>
		public string QuakeInsurance_2_QuakeAndDamageType { get; set; }


		/// <summary>
		/// 地震保険料控除02_支払保険料
		/// </summary>
		public int? QuakeInsurance_2_InsuranceFee { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 地震保険料控除03_HostData判定
		/// </summary>
		public string QuakeInsurance_3_HostDataFlg { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険会社等名称
		/// </summary>
		public string QuakeInsurance_3_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等種類
		/// </summary>
		public string QuakeInsurance_3_InsuranceTypeName { get; set; }


		/// <summary>
		/// 地震保険料控除03_期間
		/// </summary>
		public string QuakeInsurance_3_InsurancePeriod { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等契約者氏名_姓
		/// </summary>
		public string QuakeInsurance_3_ContractorName1 { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等契約者氏名_名
		/// </summary>
		public string QuakeInsurance_3_ContractorName2 { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等対象氏名_姓
		/// </summary>
		public string QuakeInsurance_3_ReceiverName1 { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等対象氏名_名
		/// </summary>
		public string QuakeInsurance_3_ReceiverName2 { get; set; }


		/// <summary>
		/// 地震保険料控除03_保険等対象続柄
		/// </summary>
		public string QuakeInsurance_3_RelationshipType { get; set; }


		/// <summary>
		/// 地震保険料控除03_地震旧長期
		/// </summary>
		public string QuakeInsurance_3_QuakeAndDamageType { get; set; }


		/// <summary>
		/// 地震保険料控除03_支払保険料
		/// </summary>
		public int? QuakeInsurance_3_InsuranceFee { get; set; }


		/// <summary>
		/// 地震保険料控除04_HostData判定
		/// </summary>
		public string QuakeInsurance_4_HostDataFlg { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険会社等名称
		/// </summary>
		public string QuakeInsurance_4_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等種類
		/// </summary>
		public string QuakeInsurance_4_InsuranceTypeName { get; set; }


		/// <summary>
		/// 地震保険料控除04_期間
		/// </summary>
		public string QuakeInsurance_4_InsurancePeriod { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等契約者氏名_姓
		/// </summary>
		public string QuakeInsurance_4_ContractorName1 { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等契約者氏名_名
		/// </summary>
		public string QuakeInsurance_4_ContractorName2 { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等対象氏名_姓
		/// </summary>
		public string QuakeInsurance_4_ReceiverName1 { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等対象氏名_名
		/// </summary>
		public string QuakeInsurance_4_ReceiverName2 { get; set; }


		/// <summary>
		/// 地震保険料控除04_保険等対象続柄
		/// </summary>
		public string QuakeInsurance_4_RelationshipType { get; set; }


		/// <summary>
		/// 地震保険料控除04_地震旧長期
		/// </summary>
		public string QuakeInsurance_4_QuakeAndDamageType { get; set; }


		/// <summary>
		/// 地震保険料控除04_支払保険料
		/// </summary>
		public int? QuakeInsurance_4_InsuranceFee { get; set; }
        //2023-11-20 iwai-tamura upd end -----



		/// <summary>
		/// 社会保険料控除合計
		/// </summary>
		public int? SocialInsurance_DeductionAmount { get; set; }


		/// <summary>
		/// 社会保険料控除01_社会保険種類
		/// </summary>
		public string SocialInsurance_1_InsuranceTypeName { get; set; }


		/// <summary>
		/// 社会保険料控除01_支払先名称
		/// </summary>
		public string SocialInsurance_1_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 社会保険料控除01_負担者氏名_姓
		/// </summary>
		public string SocialInsurance_1_ContractorName1 { get; set; }


		/// <summary>
		/// 社会保険料控除01_負担者氏名_名
		/// </summary>
		public string SocialInsurance_1_ContractorName2 { get; set; }


		/// <summary>
		/// 社会保険料控除01_負担者続柄
		/// </summary>
		public string SocialInsurance_1_RelationshipType { get; set; }


		/// <summary>
		/// 社会保険料控除01_支払保険料
		/// </summary>
		public int? SocialInsurance_1_InsuranceFee { get; set; }


		/// <summary>
		/// 社会保険料控除02_社会保険種類
		/// </summary>
		public string SocialInsurance_2_InsuranceTypeName { get; set; }


		/// <summary>
		/// 社会保険料控除02_支払先名称
		/// </summary>
		public string SocialInsurance_2_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 社会保険料控除02_負担者氏名_姓
		/// </summary>
		public string SocialInsurance_2_ContractorName1 { get; set; }


		/// <summary>
		/// 社会保険料控除02_負担者氏名_名
		/// </summary>
		public string SocialInsurance_2_ContractorName2 { get; set; }


		/// <summary>
		/// 社会保険料控除02_負担者続柄
		/// </summary>
		public string SocialInsurance_2_RelationshipType { get; set; }


		/// <summary>
		/// 社会保険料控除02_支払保険料
		/// </summary>
		public int? SocialInsurance_2_InsuranceFee { get; set; }

        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 社会保険料控除03_社会保険種類
		/// </summary>
		public string SocialInsurance_3_InsuranceTypeName { get; set; }


		/// <summary>
		/// 社会保険料控除03_支払先名称
		/// </summary>
		public string SocialInsurance_3_InsuranceCompanyName { get; set; }


		/// <summary>
		/// 社会保険料控除03_負担者氏名_姓
		/// </summary>
		public string SocialInsurance_3_ContractorName1 { get; set; }


		/// <summary>
		/// 社会保険料控除03_負担者氏名_名
		/// </summary>
		public string SocialInsurance_3_ContractorName2 { get; set; }


		/// <summary>
		/// 社会保険料控除03_負担者続柄
		/// </summary>
		public string SocialInsurance_3_RelationshipType { get; set; }


		/// <summary>
		/// 社会保険料控除03_支払保険料
		/// </summary>
		public int? SocialInsurance_3_InsuranceFee { get; set; }
        //2023-11-20 iwai-tamura upd end -----

		/// <summary>
		/// 共済契約掛金
		/// </summary>
		public int? SmallScaleMutualAid_MutualAidCost { get; set; }


		/// <summary>
		/// 企業型年金加入者掛金
		/// </summary>
		public int? SmallScaleMutualAid_CorporatePensionCost { get; set; }


		/// <summary>
		/// 個人型年金加入者掛金
		/// </summary>
		public int? SmallScaleMutualAid_PersonalPensionCost { get; set; }


		/// <summary>
		/// 心身障害者扶養共済制度契約掛金
		/// </summary>
		public int? SmallScaleMutualAid_HandicappedMutualAidCost { get; set; }


		/// <summary>
		/// 小規模企業共済等掛金控除合計
		/// </summary>
		public int? SmallScaleMutualAid_DeductionAmount { get; set; }


        //2026-99-99 iwai-miki add str -----
        //参考にするため扶養控除申告書から扶養者のデータを取得する用
        /// <summary>
        /// 23歳未満の扶養親族01_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld01_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld01_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族01_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld01_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族01_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld01_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族01_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld01_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld02_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld02_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族02_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld02_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族02_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld02_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族02_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld02_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld03_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld03_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族03_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld03_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族03_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld03_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族03_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld03_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld04_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld04_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族04_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld04_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族04_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld04_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族04_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld04_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld05_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld05_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族05_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld05_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族05_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld05_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族05_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld05_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld06_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld06_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族06_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld06_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族06_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld06_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族06_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld06_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld07_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld07_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族07_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld07_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族07_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld07_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族07_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld07_TotalEarnings { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_続柄（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_RelationshipType { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_氏名(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Name01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_氏名(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Name02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_カナ(氏)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Kana01 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_カナ(名)（参考表示用）
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Kana02 { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_生年月日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Birthday { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_生年月日_年
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_BirthdayYear { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_生年月日_月
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_BirthdayMonth { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_生年月日_日
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_BirthdayDay { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_別居
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_AddressSameCheck { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_住所
        /// </summary>
        public string RefDependentFamilyMemberUnder23YearsOld08_Address { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld08_Earnings { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族08_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld08_Income { get; set; }
        /// <summary>
        /// 23歳未満の扶養親族08_他_所得金額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld08_OtherIncome { get; set; }

        /// <summary>
        /// 23歳未満の扶養親族08_所得見積額（参考表示用）
        /// </summary>
        public int? RefDependentFamilyMemberUnder23YearsOld08_TotalEarnings { get; set; }
        //2026-99-99 iwai-miki add end -----

    }

	/// <summary>
	/// ボディ
	/// </summary>
	public class HokenDeclareRegisterBodyModel {

        /// <summary>
        /// 担当職務_1
        /// </summary>
        public string ChargeDuty_1 { get; set; }

        /// <summary>
        /// 担当職務_2
        /// </summary>
        public string ChargeDuty_2 { get; set; }

        /// <summary>
        /// 担当職務区分_2_1
        /// </summary>
        public string ChargeDuty_2_1 { get; set; }

        /// <summary>
        /// 担当職務内容_2_1
        /// </summary>
        public string ChargeDuty_2_1_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_2
        /// </summary>
        public string ChargeDuty_2_2 { get; set; }

        /// <summary>
        /// 担当職務内容_2_2
        /// </summary>
        public string ChargeDuty_2_2_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_3
        /// </summary>
        public string ChargeDuty_2_3 { get; set; }

        /// <summary>
        /// 担当職務内容_2_3
        /// </summary>
        public string ChargeDuty_2_3_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_4
        /// </summary>
        public string ChargeDuty_2_4 { get; set; }

        /// <summary>
        /// 担当職務内容_2_4
        /// </summary>
        public string ChargeDuty_2_4_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_5
        /// </summary>
        public string ChargeDuty_2_5 { get; set; }

        /// <summary>
        /// 担当職務内容_2_5
        /// </summary>
        public string ChargeDuty_2_5_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_6
        /// </summary>
        public string ChargeDuty_2_6 { get; set; }

        /// <summary>
        /// 担当職務内容_2_6
        /// </summary>
        public string ChargeDuty_2_6_Content { get; set; }

        /// <summary>
        /// 担当職務区分_2_7
        /// </summary>
        public string ChargeDuty_2_7 { get; set; }

        /// <summary>
        /// 担当職務内容_2_7
        /// </summary>
        public string ChargeDuty_2_7_Content { get; set; }

        //2021/12/24 iwai-tamura add-str ------
        /// <summary>
        /// 担当職務区分_2_8
        /// </summary>
        public string ChargeDuty_2_8 { get; set; }

        /// <summary>
        /// 担当職務内容_2_8
        /// </summary>
        public string ChargeDuty_2_8_Content { get; set; }
        //2021/12/24 iwai-tamura add-end ------

        /// <summary>
        /// 担当職務_3
        /// </summary>
        public string ChargeDuty_3 { get; set; }

        /// <summary>
        /// 担当職務_4
        /// </summary>
        public string ChargeDuty_4 { get; set; }

        /// <summary>
        /// 担当職務_5
        /// </summary>
        public string ChargeDuty_5 { get; set; }

        /// <summary>
        /// 担当職務_6
        /// </summary>
        public string ChargeDuty_6 { get; set; }

        /// <summary>
        /// 能力開発_1
        /// </summary>
        public string AptitudeDevelop_1 { get; set; }

        /// <summary>
        /// 適性能力開発区分_1_1_1
        /// </summary>
        public string AptitudeDevelop_1_1_1 { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_1_1
        /// </summary>
        public string AptitudeDevelop_1_1_1_Content { get; set; }

        /// <summary>
        /// 適性能力開発区分_1_1_2
        /// </summary>
        public string AptitudeDevelop_1_1_2 { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_1_2
        /// </summary>
        public string AptitudeDevelop_1_1_2_Content { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_1_Other
        /// </summary>
        public string AptitudeDevelop_1_1_Other { get; set; }

        /// <summary>
        /// 適性能力開発区分_1_2_1
        /// </summary>
        public string AptitudeDevelop_1_2_1 { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_2_1
        /// </summary>
        public string AptitudeDevelop_1_2_1_Content { get; set; }

        /// <summary>
        /// 適性能力開発区分_1_2_2
        /// </summary>
        public string AptitudeDevelop_1_2_2 { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_2_2
        /// </summary>
        public string AptitudeDevelop_1_2_2_Content { get; set; }

        /// <summary>
        /// 適性能力開発内容_1_2_Other
        /// </summary>
        public string AptitudeDevelop_1_2_Other { get; set; }

        /// <summary>
        /// 適性能力開発内容_2
        /// </summary>
        public string AptitudeDevelop_2_Content { get; set; }

        /// <summary>
        /// 適性区分_1_1_1
        /// </summary>
        public string Aptitude_1_1_1 { get; set; }

        /// <summary>
        /// 適性内容_1_1_1
        /// </summary>
        public string Aptitude_1_1_1_Content { get; set; }

        /// <summary>
        /// 適性区分_1_1_2
        /// </summary>
        public string Aptitude_1_1_2 { get; set; }

        /// <summary>
        /// 適性内容_1_1_2
        /// </summary>
        public string Aptitude_1_1_2_Content { get; set; }

        /// <summary>
        /// 適性内容_1_1_Other
        /// </summary>
        public string Aptitude_1_1_Other { get; set; }

        /// <summary>
        /// 適性区分_1_2_1
        /// </summary>
        public string Aptitude_1_2_1 { get; set; }

        /// <summary>
        /// 適性内容_1_2_1
        /// </summary>
        public string Aptitude_1_2_1_Content { get; set; }

        /// <summary>
        /// 適性区分_1_2_2
        /// </summary>
        public string Aptitude_1_2_2 { get; set; }

        /// <summary>
        /// 適性内容_1_2_2
        /// </summary>
        public string Aptitude_1_2_2_Content { get; set; }

        /// <summary>
        /// 適性内容_1_2_Other
        /// </summary>
        public string Aptitude_1_2_Other { get; set; }
        
         /// <summary>
        /// 適性職務内容_2_1
        /// </summary>
        public string Aptitude_2_1_Duty { get; set; }

        /// <summary>
        /// 適性資格免許_2_1
        /// </summary>
        public string Aptitude_2_1_License { get; set; }

        /// <summary>
        /// 適性遂行Level区分_2_1
        /// </summary>
        public string Aptitude_2_1_Level { get; set; }

        /// <summary>
        /// 適性遂行Level内容_2_1
        /// </summary>
        public string Aptitude_2_1_Level_Content { get; set; }

        /// <summary>
        /// 適性職務内容_2_2
        /// </summary>
        public string Aptitude_2_2_Duty { get; set; }

        /// <summary>
        /// 適性資格免許_2_2
        /// </summary>
        public string Aptitude_2_2_License { get; set; }

        /// <summary>
        /// 適性遂行Level区分_2_2
        /// </summary>
        public string Aptitude_2_2_Level { get; set; }

        /// <summary>
        /// 適性遂行Level内容_2_2
        /// </summary>
        public string Aptitude_2_2_Level_Content { get; set; }

        /// <summary>
        /// 適性職務内容_2_3
        /// </summary>
        public string Aptitude_2_3_Duty { get; set; }

        /// <summary>
        /// 適性資格免許_2_3
        /// </summary>
        public string Aptitude_2_3_License { get; set; }

        /// <summary>
        /// 適性遂行Level区分_2_3
        /// </summary>
        public string Aptitude_2_3_Level { get; set; }

        /// <summary>
        /// 適性遂行Level内容_2_3
        /// </summary>
        public string Aptitude_2_3_Level_Content { get; set; }

        /// <summary>
        /// 適性職務内容_2_4
        /// </summary>
        public string Aptitude_2_4_Duty { get; set; }

        /// <summary>
        /// 適性資格免許_2_4
        /// </summary>
        public string Aptitude_2_4_License { get; set; }

        /// <summary>
        /// 適性遂行Level区分_2_4
        /// </summary>
        public string Aptitude_2_4_Level { get; set; }

        /// <summary>
        /// 適性遂行Level内容_2_4
        /// </summary>
        public string Aptitude_2_4_Level_Content { get; set; }

        /// <summary>
        /// 適性職務内容_2_5
        /// </summary>
        public string Aptitude_2_5_Duty { get; set; }

        /// <summary>
        /// 適性資格免許_2_5
        /// </summary>
        public string Aptitude_2_5_License { get; set; }

        /// <summary>
        /// 適性遂行Level区分_2_5
        /// </summary>
        public string Aptitude_2_5_Level { get; set; }

        /// <summary>
        /// 適性遂行Level内容_2_5
        /// </summary>
        public string Aptitude_2_5_Level_Content { get; set; }

        /// <summary>
        /// 適性_3
        /// </summary>
        public string Aptitude_3 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_1
        /// </summary>
        public string TransferDutyDepartment_1_Content { get; set; }

        /// <summary>
        /// 職務変更配置換区分_1_1
        /// </summary>
        public string TransferDutyDepartment_1_1 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_1_1
        /// </summary>
        public string TransferDutyDepartment_1_1_Content { get; set; }

        /// <summary>
        /// 職務変更配置換内容_1_1_Other
        /// </summary>
        public string TransferDutyDepartment_1_1_Other { get; set; }

        /// <summary>
        /// 職務変更配置換区分_1_2
        /// </summary>
        public string TransferDutyDepartment_1_2 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_1_2
        /// </summary>
        public string TransferDutyDepartment_1_2_Content { get; set; }

        /// <summary>
        /// 職務変更配置換内容_1_2_Other
        /// </summary>
        public string TransferDutyDepartment_1_2_Other { get; set; }

        /// <summary>
        /// 職務変更配置換区分_2_1
        /// </summary>
        public string TransferDutyDepartment_2_1 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_2_1
        /// </summary>
        public string TransferDutyDepartment_2_1_Content { get; set; }

        /// <summary>
        /// 職務変更配置換区分_2_2
        /// </summary>
        public string TransferDutyDepartment_2_2 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_2_2
        /// </summary>
        public string TransferDutyDepartment_2_2_Content { get; set; }

        /// <summary>
        /// 職務変更配置換区分_2_3
        /// </summary>
        public string TransferDutyDepartment_2_3 { get; set; }

        /// <summary>
        /// 職務変更配置換内容_2_3
        /// </summary>
        public string TransferDutyDepartment_2_3_Content { get; set; }

        /// <summary>
        /// 職務変更配置換内容_2_Other
        /// </summary>
        public string TransferDutyDepartment_2_Other { get; set; }

        /// <summary>
        /// 配置換_1
        /// </summary>
        public string TransferDepartment_1 { get; set; }

        /// <summary>
        /// 配置換区分_1_1
        /// </summary>
        public string TransferDepartment_1_1 { get; set; }

        /// <summary>
        /// 配置換内容_1_1
        /// </summary>
        public string TransferDepartment_1_1_Content { get; set; }

        /// <summary>
        /// 配置換内容_1_1_Other
        /// </summary>
        public string TransferDepartment_1_1_Other { get; set; }

        /// <summary>
        /// 配置換区分_1_2
        /// </summary>
        public string TransferDepartment_1_2 { get; set; }

        /// <summary>
        /// 配置換内容_1_2
        /// </summary>
        public string TransferDepartment_1_2_Content { get; set; }

        /// <summary>
        /// 配置換内容_1_2_Other
        /// </summary>
        public string TransferDepartment_1_2_Other { get; set; }

        /// <summary>
        /// 配置換区分_2_1
        /// </summary>
        public string TransferDepartment_2_1 { get; set; }

        /// <summary>
        /// 配置換内容_2_1
        /// </summary>
        public string TransferDepartment_2_1_Content { get; set; }

        /// <summary>
        /// 配置換区分_2_2
        /// </summary>
        public string TransferDepartment_2_2 { get; set; }

        /// <summary>
        /// 配置換内容_2_2
        /// </summary>
        public string TransferDepartment_2_2_Content { get; set; }

        /// <summary>
        /// 配置換区分_2_3
        /// </summary>
        public string TransferDepartment_2_3 { get; set; }

        /// <summary>
        /// 配置換内容_2_3
        /// </summary>
        public string TransferDepartment_2_3_Content { get; set; }

        /// <summary>
        /// 配置換内容_2_Other
        /// </summary>
        public string TransferDepartment_2_Other { get; set; }

        /// <summary>
        /// 能力開発_1_1
        /// </summary>
        public string AptitudeDevelop_1_1 { get; set; }

        /// <summary>
        /// 能力開発区分_1_2
        /// </summary>
        public string AptitudeDevelop_1_2 { get; set; }

        /// <summary>
        /// 能力開発内容_1_2
        /// </summary>
        public string AptitudeDevelop_1_2_Content { get; set; }

        /// <summary>
        /// 能力開発_1_3
        /// </summary>
        public string AptitudeDevelop_1_3 { get; set; }

        /// <summary>
        /// 能力開発_2
        /// </summary>
        public string AptitudeDevelop_2 { get; set; }

        /// <summary>
        /// 能力開発_3
        /// </summary>
        public string AptitudeDevelop_3 { get; set; }

        /// <summary>
        /// その他
        /// </summary>
        public string OtherComment { get; set; }

        //2021/12/24 iwai-tamura add-str ------
        /// <summary>
        /// 定年退職後の生活設計
        /// </summary>
        public string OldLifeComment1 { get; set; }
        //2021/12/24 iwai-tamura add-end ------

        /// <summary>
        /// 自由意見内容
        /// </summary>
        public string FreeComment { get; set; }

        /// <summary>
        /// 上司記入欄内容
        /// </summary>
        public string BossComment { get; set; }
    }

}
