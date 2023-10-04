using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Enum;

namespace EmployeeAdjustmentConnectionSystem.COM.Models {
    /// <summary>
    /// 扶養控除申告書入力モデル
    /// </summary>
    public class HuyouDeclareRegisterViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public HuyouDeclareRegisterHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public HuyouDeclareRegisterBodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class HuyouDeclareRegisterHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HuyouDeclareRegisterHeaderModel() {
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
        /// 管理者モード
        /// </summary>
        public bool AdminMode { get; set; }

		/// <summary>
		/// 年度
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


		/// <summary>
		/// 個人番号相違確認区分
		/// </summary>
		public string MyNumberCheck { get; set; }


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
		/// 生年月日
		/// </summary>
		public string Birthday { get; set; }


		/// <summary>
		/// 生年月日_年
		/// </summary>
		public string BirthdayYear { get; set; }


		/// <summary>
		/// 生年月日_月
		/// </summary>
		public string BirthdayMonth { get; set; }


		/// <summary>
		/// 生年月日_日
		/// </summary>
		public string BirthdayDay { get; set; }


		/// <summary>
		/// 本人区分
		/// </summary>
		public string HouseholdSelfCheck { get; set; }


		/// <summary>
		/// 世帯主氏名_姓
		/// </summary>
		public string HouseholdName1 { get; set; }


		/// <summary>
		/// 世帯主氏名_名
		/// </summary>
		public string HouseholdName2 { get; set; }


		/// <summary>
		/// 世帯主続柄
		/// </summary>
		public string RelationshipType { get; set; }


		/// <summary>
		/// 世帯主続柄名
		/// </summary>
		public string RelationshipName { get; set; }


		/// <summary>
		/// 郵便番号_前
		/// </summary>
		public string PostalCode_1 { get; set; }


		/// <summary>
		/// 郵便番号_後
		/// </summary>
		public string PostalCode_2 { get; set; }


		/// <summary>
		/// 住所01
		/// </summary>
		public string Address { get; set; }


		/// <summary>
		/// 配偶者有無
		/// </summary>
		public string SpouseCheck { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者対象外区分
		/// </summary>
		public string TaxWithholding_notSubject { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者氏名_姓
		/// </summary>
		public string TaxWithholding_Name1 { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者氏名_名
		/// </summary>
		public string TaxWithholding_Name2 { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者Kana_姓
		/// </summary>
		public string TaxWithholding_Kana1 { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者Kana_名
		/// </summary>
		public string TaxWithholding_Kana2 { get; set; }

		/// <summary>
		/// 源泉控除対象配偶者続柄
		/// </summary>
		public string TaxWithholding_RelationshipType { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者生年月日
		/// </summary>
		public string TaxWithholding_Birthday { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者生年月日_年
		/// </summary>
		public string TaxWithholding_BirthdayYear { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者生年月日_月
		/// </summary>
		public string TaxWithholding_BirthdayMonth { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者生年月日_日
		/// </summary>
		public string TaxWithholding_BirthdayDay { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者所得見積額
		/// </summary>
		public int? TaxWithholding_Income { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者非居住者
		/// </summary>
		public string TaxWithholding_ResidentType { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者住所
		/// </summary>
		public string TaxWithholding_Address { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者異動月日
		/// </summary>
		public DateTime? TaxWithholding_TransferDate { get; set; }


		/// <summary>
		/// 源泉控除対象配偶者事由
		/// </summary>
		public string TaxWithholding_TransferComment { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_対象外区分
		/// </summary>
		public string DependentsOver16_1_notSubject { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_氏名_姓
		/// </summary>
		public string DependentsOver16_1_Name1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_氏名_名
		/// </summary>
		public string DependentsOver16_1_Name2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_Kana_姓
		/// </summary>
		public string DependentsOver16_1_Kana1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_Kana_名
		/// </summary>
		public string DependentsOver16_1_Kana2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_続柄
		/// </summary>
		public string DependentsOver16_1_RelationshipType { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_生年月日
		/// </summary>
		public string DependentsOver16_1_Birthday { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_生年月日_年
		/// </summary>
		public string DependentsOver16_1_BirthdayYear { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_生年月日_月
		/// </summary>
		public string DependentsOver16_1_BirthdayMonth { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_生年月日_日
		/// </summary>
		public string DependentsOver16_1_BirthdayDay { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_老人扶養親族区分
		/// </summary>
		public string DependentsOver16_1_OldmanType { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_特定扶養親族区分
		/// </summary>
		public string DependentsOver16_1_SpecificType { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_所得見積額
		/// </summary>
		public int? DependentsOver16_1_Income { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_非居住者
		/// </summary>
		public string DependentsOver16_1_ResidentType { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_住所
		/// </summary>
		public string DependentsOver16_1_Address { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_異動月日
		/// </summary>
		public DateTime? DependentsOver16_1_TransferDate { get; set; }


		/// <summary>
		/// 控除対象扶養親族01_事由
		/// </summary>
		public string DependentsOver16_1_TransferComment { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_対象外区分
		/// </summary>
		public string DependentsOver16_2_notSubject { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_氏名_姓
		/// </summary>
		public string DependentsOver16_2_Name1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_氏名_名
		/// </summary>
		public string DependentsOver16_2_Name2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_Kana_姓
		/// </summary>
		public string DependentsOver16_2_Kana1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_Kana_名
		/// </summary>
		public string DependentsOver16_2_Kana2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_続柄
		/// </summary>
		public string DependentsOver16_2_RelationshipType { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_生年月日
		/// </summary>
		public string DependentsOver16_2_Birthday { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_生年月日_年
		/// </summary>
		public string DependentsOver16_2_BirthdayYear { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_生年月日_月
		/// </summary>
		public string DependentsOver16_2_BirthdayMonth { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_生年月日_日
		/// </summary>
		public string DependentsOver16_2_BirthdayDay { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_老人扶養親族区分
		/// </summary>
		public string DependentsOver16_2_OldmanType { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_特定扶養親族区分
		/// </summary>
		public string DependentsOver16_2_SpecificType { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_所得見積額
		/// </summary>
		public int? DependentsOver16_2_Income { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_非居住者
		/// </summary>
		public string DependentsOver16_2_ResidentType { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_住所
		/// </summary>
		public string DependentsOver16_2_Address { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_異動月日
		/// </summary>
		public DateTime? DependentsOver16_2_TransferDate { get; set; }


		/// <summary>
		/// 控除対象扶養親族02_事由
		/// </summary>
		public string DependentsOver16_2_TransferComment { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_対象外区分
		/// </summary>
		public string DependentsOver16_3_notSubject { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_氏名_姓
		/// </summary>
		public string DependentsOver16_3_Name1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_氏名_名
		/// </summary>
		public string DependentsOver16_3_Name2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_Kana_姓
		/// </summary>
		public string DependentsOver16_3_Kana1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_Kana_名
		/// </summary>
		public string DependentsOver16_3_Kana2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_続柄
		/// </summary>
		public string DependentsOver16_3_RelationshipType { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_生年月日
		/// </summary>
		public string DependentsOver16_3_Birthday { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_生年月日_年
		/// </summary>
		public string DependentsOver16_3_BirthdayYear { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_生年月日_月
		/// </summary>
		public string DependentsOver16_3_BirthdayMonth { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_生年月日_日
		/// </summary>
		public string DependentsOver16_3_BirthdayDay { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_老人扶養親族区分
		/// </summary>
		public string DependentsOver16_3_OldmanType { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_特定扶養親族区分
		/// </summary>
		public string DependentsOver16_3_SpecificType { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_所得見積額
		/// </summary>
		public int? DependentsOver16_3_Income { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_非居住者
		/// </summary>
		public string DependentsOver16_3_ResidentType { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_住所
		/// </summary>
		public string DependentsOver16_3_Address { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_異動月日
		/// </summary>
		public DateTime? DependentsOver16_3_TransferDate { get; set; }


		/// <summary>
		/// 控除対象扶養親族03_事由
		/// </summary>
		public string DependentsOver16_3_TransferComment { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_対象外区分
		/// </summary>
		public string DependentsOver16_4_notSubject { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_氏名_姓
		/// </summary>
		public string DependentsOver16_4_Name1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_氏名_名
		/// </summary>
		public string DependentsOver16_4_Name2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_Kana_姓
		/// </summary>
		public string DependentsOver16_4_Kana1 { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_Kana_名
		/// </summary>
		public string DependentsOver16_4_Kana2 { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_続柄
		/// </summary>
		public string DependentsOver16_4_RelationshipType { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_生年月日
		/// </summary>
		public string DependentsOver16_4_Birthday { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_生年月日_年
		/// </summary>
		public string DependentsOver16_4_BirthdayYear { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_生年月日_月
		/// </summary>
		public string DependentsOver16_4_BirthdayMonth { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_生年月日_日
		/// </summary>
		public string DependentsOver16_4_BirthdayDay { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_老人扶養親族区分
		/// </summary>
		public string DependentsOver16_4_OldmanType { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_特定扶養親族区分
		/// </summary>
		public string DependentsOver16_4_SpecificType { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_所得見積額
		/// </summary>
		public int? DependentsOver16_4_Income { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_非居住者
		/// </summary>
		public string DependentsOver16_4_ResidentType { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_住所
		/// </summary>
		public string DependentsOver16_4_Address { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_異動月日
		/// </summary>
		public DateTime? DependentsOver16_4_TransferDate { get; set; }


		/// <summary>
		/// 控除対象扶養親族04_事由
		/// </summary>
		public string DependentsOver16_4_TransferComment { get; set; }


		/// <summary>
		/// 障害者
		/// </summary>
		public string DependentsOther_Subject { get; set; }


		/// <summary>
		/// 一般障害_本人
		/// </summary>
		public string DependentsOther_GeneralHandicappedSelfCheck { get; set; }


		/// <summary>
		/// 一般障害_同一生計配偶者
		/// </summary>
		public string DependentsOther_GeneralHandicappedSpouseCheck { get; set; }


		/// <summary>
		/// 一般障害_扶養親族
		/// </summary>
		public string DependentsOther_GeneralHandicappedDependentsCheck { get; set; }


		/// <summary>
		/// 一般障害_扶養親族人数
		/// </summary>
		public string DependentsOther_GeneralHandicappedDependentsNumber { get; set; }


		/// <summary>
		/// 特別障害者_本人
		/// </summary>
		public string DependentsOther_SpecialHandicappedSelfCheck { get; set; }


		/// <summary>
		/// 特別障害者_同一生計配偶者
		/// </summary>
		public string DependentsOther_SpecialHandicappedSpouseCheck { get; set; }


		/// <summary>
		/// 特別障害者_扶養親族
		/// </summary>
		public string DependentsOther_SpecialHandicappedDependentsCheck { get; set; }


		/// <summary>
		/// 特別障害者_扶養親族人数
		/// </summary>
		public string DependentsOther_SpecialHandicappedDependentsNumber { get; set; }


		/// <summary>
		/// 同居特別障害者_同一生計配偶者
		/// </summary>
		public string DependentsOther_LivingHandicappedSpouseCheck { get; set; }


		/// <summary>
		/// 同居特別障害者_扶養親族
		/// </summary>
		public string DependentsOther_LivingHandicappedDependentsCheck { get; set; }


		/// <summary>
		/// 同居特別障害者_扶養親族人数
		/// </summary>
		public string DependentsOther_LivingHandicappedDependentsNumber { get; set; }


		/// <summary>
		/// 寡婦一人親区分
		/// </summary>
		public string DependentsOther_WidowType { get; set; }


		/// <summary>
		/// 理由区分
		/// </summary>
		public string DependentsOther_WidowReasonType { get; set; }


		/// <summary>
		/// 発生年月日
		/// </summary>
		public DateTime? DependentsOther_WidowOccurrenceDate { get; set; }


		/// <summary>
		/// 勤労学生
		/// </summary>
		public string DependentsOther_StudentCheck { get; set; }


		/// <summary>
		/// 障害異動月日
		/// </summary>
		public DateTime? DependentsOther_TransferDate { get; set; }


		/// <summary>
		/// 障害事由
		/// </summary>
		public string DependentsOther_TransferComment { get; set; }


		/// <summary>
		/// 扶養親族16未満01_対象外区分
		/// </summary>
		public string DependentsUnder16_1_notSubject { get; set; }


		/// <summary>
		/// 扶養親族16未満01_氏名_姓
		/// </summary>
		public string DependentsUnder16_1_Name1 { get; set; }


		/// <summary>
		/// 扶養親族16未満01_氏名_名
		/// </summary>
		public string DependentsUnder16_1_Name2 { get; set; }


		/// <summary>
		/// 扶養親族16未満01_Kana_姓
		/// </summary>
		public string DependentsUnder16_1_Kana1 { get; set; }


		/// <summary>
		/// 扶養親族16未満01_Kana_名
		/// </summary>
		public string DependentsUnder16_1_Kana2 { get; set; }


		/// <summary>
		/// 扶養親族16未満01_続柄
		/// </summary>
		public string DependentsUnder16_1_RelationshipType { get; set; }


		/// <summary>
		/// 扶養親族16未満01_生年月日
		/// </summary>
		public string DependentsUnder16_1_Birthday { get; set; }


		/// <summary>
		/// 扶養親族16未満01_生年月日_年
		/// </summary>
		public string DependentsUnder16_1_BirthdayYear { get; set; }


		/// <summary>
		/// 扶養親族16未満01_生年月日_月
		/// </summary>
		public string DependentsUnder16_1_BirthdayMonth { get; set; }


		/// <summary>
		/// 扶養親族16未満01_生年月日_日
		/// </summary>
		public string DependentsUnder16_1_BirthdayDay { get; set; }


		/// <summary>
		/// 扶養親族16未満01_同上区分
		/// </summary>
		public string DependentsUnder16_1_AddressSameCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満01_住所
		/// </summary>
		public string DependentsUnder16_1_Address { get; set; }


		/// <summary>
		/// 扶養親族16未満01_国外区分
		/// </summary>
		public string DependentsUnder16_1_AbroadCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満01_所得見積額
		/// </summary>
		public int? DependentsUnder16_1_Income { get; set; }


		/// <summary>
		/// 扶養親族16未満01_異動月日
		/// </summary>
		public DateTime? DependentsUnder16_1_TransferDate { get; set; }


		/// <summary>
		/// 扶養親族16未満01_事由
		/// </summary>
		public string DependentsUnder16_1_TransferComment { get; set; }


		/// <summary>
		/// 扶養親族16未満02_対象外区分
		/// </summary>
		public string DependentsUnder16_2_notSubject { get; set; }


		/// <summary>
		/// 扶養親族16未満02_氏名_姓
		/// </summary>
		public string DependentsUnder16_2_Name1 { get; set; }


		/// <summary>
		/// 扶養親族16未満02_氏名_名
		/// </summary>
		public string DependentsUnder16_2_Name2 { get; set; }


		/// <summary>
		/// 扶養親族16未満02_Kana_姓
		/// </summary>
		public string DependentsUnder16_2_Kana1 { get; set; }


		/// <summary>
		/// 扶養親族16未満02_Kana_名
		/// </summary>
		public string DependentsUnder16_2_Kana2 { get; set; }


		/// <summary>
		/// 扶養親族16未満02_続柄
		/// </summary>
		public string DependentsUnder16_2_RelationshipType { get; set; }


		/// <summary>
		/// 扶養親族16未満02_生年月日
		/// </summary>
		public string DependentsUnder16_2_Birthday { get; set; }


		/// <summary>
		/// 扶養親族16未満02_生年月日_年
		/// </summary>
		public string DependentsUnder16_2_BirthdayYear { get; set; }


		/// <summary>
		/// 扶養親族16未満02_生年月日_月
		/// </summary>
		public string DependentsUnder16_2_BirthdayMonth { get; set; }


		/// <summary>
		/// 扶養親族16未満02_生年月日_日
		/// </summary>
		public string DependentsUnder16_2_BirthdayDay { get; set; }


		/// <summary>
		/// 扶養親族16未満02_同上区分
		/// </summary>
		public string DependentsUnder16_2_AddressSameCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満02_住所
		/// </summary>
		public string DependentsUnder16_2_Address { get; set; }


		/// <summary>
		/// 扶養親族16未満02_国外区分
		/// </summary>
		public string DependentsUnder16_2_AbroadCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満02_所得見積額
		/// </summary>
		public int? DependentsUnder16_2_Income { get; set; }


		/// <summary>
		/// 扶養親族16未満02_異動月日
		/// </summary>
		public DateTime? DependentsUnder16_2_TransferDate { get; set; }


		/// <summary>
		/// 扶養親族16未満02_事由
		/// </summary>
		public string DependentsUnder16_2_TransferComment { get; set; }


		/// <summary>
		/// 扶養親族16未満03_対象外区分
		/// </summary>
		public string DependentsUnder16_3_notSubject { get; set; }


		/// <summary>
		/// 扶養親族16未満03_氏名_姓
		/// </summary>
		public string DependentsUnder16_3_Name1 { get; set; }


		/// <summary>
		/// 扶養親族16未満03_氏名_名
		/// </summary>
		public string DependentsUnder16_3_Name2 { get; set; }


		/// <summary>
		/// 扶養親族16未満03_Kana_姓
		/// </summary>
		public string DependentsUnder16_3_Kana1 { get; set; }


		/// <summary>
		/// 扶養親族16未満03_Kana_名
		/// </summary>
		public string DependentsUnder16_3_Kana2 { get; set; }


		/// <summary>
		/// 扶養親族16未満03_続柄
		/// </summary>
		public string DependentsUnder16_3_RelationshipType { get; set; }


		/// <summary>
		/// 扶養親族16未満03_生年月日
		/// </summary>
		public string DependentsUnder16_3_Birthday { get; set; }


		/// <summary>
		/// 扶養親族16未満03_生年月日_年
		/// </summary>
		public string DependentsUnder16_3_BirthdayYear { get; set; }


		/// <summary>
		/// 扶養親族16未満03_生年月日_月
		/// </summary>
		public string DependentsUnder16_3_BirthdayMonth { get; set; }


		/// <summary>
		/// 扶養親族16未満03_生年月日_日
		/// </summary>
		public string DependentsUnder16_3_BirthdayDay { get; set; }


		/// <summary>
		/// 扶養親族16未満03_同上区分
		/// </summary>
		public string DependentsUnder16_3_AddressSameCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満03_住所
		/// </summary>
		public string DependentsUnder16_3_Address { get; set; }


		/// <summary>
		/// 扶養親族16未満03_国外区分
		/// </summary>
		public string DependentsUnder16_3_AbroadCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満03_所得見積額
		/// </summary>
		public int? DependentsUnder16_3_Income { get; set; }


		/// <summary>
		/// 扶養親族16未満03_異動月日
		/// </summary>
		public DateTime? DependentsUnder16_3_TransferDate { get; set; }


		/// <summary>
		/// 扶養親族16未満03_事由
		/// </summary>
		public string DependentsUnder16_3_TransferComment { get; set; }


		/// <summary>
		/// 扶養親族16未満04_対象外区分
		/// </summary>
		public string DependentsUnder16_4_notSubject { get; set; }


		/// <summary>
		/// 扶養親族16未満04_氏名_姓
		/// </summary>
		public string DependentsUnder16_4_Name1 { get; set; }


		/// <summary>
		/// 扶養親族16未満04_氏名_名
		/// </summary>
		public string DependentsUnder16_4_Name2 { get; set; }


		/// <summary>
		/// 扶養親族16未満04_Kana_姓
		/// </summary>
		public string DependentsUnder16_4_Kana1 { get; set; }


		/// <summary>
		/// 扶養親族16未満04_Kana_名
		/// </summary>
		public string DependentsUnder16_4_Kana2 { get; set; }


		/// <summary>
		/// 扶養親族16未満04_続柄
		/// </summary>
		public string DependentsUnder16_4_RelationshipType { get; set; }


		/// <summary>
		/// 扶養親族16未満04_生年月日
		/// </summary>
		public string DependentsUnder16_4_Birthday { get; set; }


		/// <summary>
		/// 扶養親族16未満04_生年月日_年
		/// </summary>
		public string DependentsUnder16_4_BirthdayYear { get; set; }


		/// <summary>
		/// 扶養親族16未満04_生年月日_月
		/// </summary>
		public string DependentsUnder16_4_BirthdayMonth { get; set; }


		/// <summary>
		/// 扶養親族16未満04_生年月日_日
		/// </summary>
		public string DependentsUnder16_4_BirthdayDay { get; set; }


		/// <summary>
		/// 扶養親族16未満04_同上区分
		/// </summary>
		public string DependentsUnder16_4_AddressSameCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満04_住所
		/// </summary>
		public string DependentsUnder16_4_Address { get; set; }


		/// <summary>
		/// 扶養親族16未満04_国外区分
		/// </summary>
		public string DependentsUnder16_4_AbroadCheck { get; set; }


		/// <summary>
		/// 扶養親族16未満04_所得見積額
		/// </summary>
		public int? DependentsUnder16_4_Income { get; set; }


		/// <summary>
		/// 扶養親族16未満04_異動月日
		/// </summary>
		public DateTime? DependentsUnder16_4_TransferDate { get; set; }


		/// <summary>
		/// 扶養親族16未満04_事由
		/// </summary>
		public string DependentsUnder16_4_TransferComment { get; set; }




    }

    /// <summary>
    /// ボディ
    /// </summary>
    public class HuyouDeclareRegisterBodyModel {

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
