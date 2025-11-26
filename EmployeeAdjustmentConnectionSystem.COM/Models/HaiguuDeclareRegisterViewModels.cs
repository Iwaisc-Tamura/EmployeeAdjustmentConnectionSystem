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
    public class HaiguuDeclareRegisterViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public HaiguuDeclareRegisterHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public HaiguuDeclareRegisterBodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class HaiguuDeclareRegisterHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public HaiguuDeclareRegisterHeaderModel() {
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
		/// 基礎控除申告書_給与所得_収入金額
		/// </summary>
		public int? BasicDeduction_Earnings { get; set; }


		/// <summary>
		/// 基礎控除申告書_給与所得_所得金額
		/// </summary>
		public int? BasicDeduction_Income { get; set; }


		/// <summary>
		/// 基礎控除申告書_他_所得金額
		/// </summary>
		public int? BasicDeduction_OtherIncome { get; set; }


		/// <summary>
		/// 基礎控除申告書_合計所得金額見積額
		/// </summary>
		public int? BasicDeduction_TotalEarnings { get; set; }


		/// <summary>
		/// 基礎控除申告書_控除額計算判定
		/// </summary>
		public string BasicDeduction_EarningsType { get; set; }

        //2024-11-19 iwai-tamura upd-str ------
		/// <summary>
		/// 基礎控除申告書_本人定額減税対象
		/// </summary>
		public string BasicDeduction_TaxReductionTarget { get; set; }
        //2024-11-19 iwai-tamura upd-end ------

		/// <summary>
		/// 基礎控除申告書_控除額計算区分
		/// </summary>
		public string BasicDeduction_CalcType { get; set; }


		/// <summary>
		/// 基礎控除申告書_基礎控除額
		/// </summary>
		public int? BasicDeduction_DeductionAmount { get; set; }


		/// <summary>
		/// 配偶者控除申告書_氏名_姓
		/// </summary>
		public string SpouseDeduction_Name1 { get; set; }


		/// <summary>
		/// 配偶者控除申告書_氏名_名
		/// </summary>
		public string SpouseDeduction_Name2 { get; set; }


		/// <summary>
		/// 配偶者控除申告書_Kana_姓
		/// </summary>
		public string SpouseDeduction_Kana1 { get; set; }


		/// <summary>
		/// 配偶者控除申告書_Kana_名
		/// </summary>
		public string SpouseDeduction_Kana2 { get; set; }


		/// <summary>
		/// 配偶者控除申告書_続柄
		/// </summary>
		public string SpouseDeduction_RelationshipType { get; set; }


		/// <summary>
		/// 配偶者控除申告書_生年月日
		/// </summary>
		public string SpouseDeduction_Birthday { get; set; }


		/// <summary>
		/// 配偶者控除申告書_生年月日_年
		/// </summary>
		public string SpouseDeduction_BirthdayYear { get; set; }


		/// <summary>
		/// 配偶者控除申告書_生年月日_月
		/// </summary>
		public string SpouseDeduction_BirthdayMonth { get; set; }


		/// <summary>
		/// 配偶者控除申告書_生年月日_日
		/// </summary>
		public string SpouseDeduction_BirthdayDay { get; set; }


		/// <summary>
		/// 配偶者控除申告書_非居住者
		/// </summary>
		public string SpouseDeduction_ResidentCheck { get; set; }


		/// <summary>
		/// 配偶者控除申告書_住所
		/// </summary>
		public string SpouseDeduction_Address { get; set; }


		/// <summary>
		/// 配偶者控除申告書_給与所得収入金額
		/// </summary>
		public int? SpouseDeduction_Earnings { get; set; }


		/// <summary>
		/// 配偶者控除申告書_給与所得所得金額
		/// </summary>
		public int? SpouseDeduction_Income { get; set; }


		/// <summary>
		/// 配偶者控除申告書_他所得金額
		/// </summary>
		public int? SpouseDeduction_OtherIncome { get; set; }


        //2023-11-20 iwai-tamura upd str -----
		/// <summary>
		/// 参照項：扶養控除申告書_配偶者_給与所得収入金額
		/// </summary>
		public int? SpouseDeduction_Huyou_Earnings { get; set; }


		/// <summary>
		/// 参照項：扶養控除申告書_配偶者_給与所得所得金額
		/// </summary>
		public int? SpouseDeduction_Huyou_Income { get; set; }


		/// <summary>
		/// 参照項：扶養控除申告書_配偶者_他所得金額
		/// </summary>
		public int? SpouseDeduction_Huyou_OtherIncome { get; set; }
        //2023-11-20 iwai-tamura upd end -----




		/// <summary>
		/// 配偶者控除申告書_合計所得金額見積額
		/// </summary>
		public int? SpouseDeduction_TotalEarnings { get; set; }


		/// <summary>
		/// 配偶者控除申告書_控除額計算判定
		/// </summary>
		public string SpouseDeduction_EarningsType { get; set; }


		/// <summary>
		/// 配偶者控除申告書_控除額計算区分
		/// </summary>
		public string SpouseDeduction_CalcType { get; set; }


		/// <summary>
		/// 配偶者控除申告書_配偶者控除額
		/// </summary>
		public int? SpouseDeduction_DeductionAmount { get; set; }


		/// <summary>
		/// 配偶者控除申告書_配偶者特別控除額
		/// </summary>
		public int? SpouseDeduction_SpecialDeductionAmount { get; set; }


        //2024-11-19 iwai-tamura upd-str ------
		/// <summary>
		/// 配偶者控除申告書_配偶者定額減税対象
		/// </summary>
		public string SpouseDeduction_TaxReductionTarget { get; set; }
        //2024-11-19 iwai-tamura upd-end ------


		/// <summary>
		/// 所得金額調整控除申告書_要件区分
		/// </summary>
		public string AdjustmentDeduction_ConditionType { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等氏名_姓
		/// </summary>
		public string AdjustmentDeduction_Name1 { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等氏名_名
		/// </summary>
		public string AdjustmentDeduction_Name2 { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等Kana_姓
		/// </summary>
		public string AdjustmentDeduction_Kana1 { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等Kana_名
		/// </summary>
		public string AdjustmentDeduction_Kana2 { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等生年月日
		/// </summary>
		public string AdjustmentDeduction_Birthday { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等生年月日_年
		/// </summary>
		public string AdjustmentDeduction_BirthdayYear { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等生年月日_月
		/// </summary>
		public string AdjustmentDeduction_BirthdayMonth { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等生年月日_日
		/// </summary>
		public string AdjustmentDeduction_BirthdayDay { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等非居住者
		/// </summary>
		public string AdjustmentDeduction_ResidentCheck { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等住所
		/// </summary>
		public string AdjustmentDeduction_Address { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等続柄
		/// </summary>
		public string AdjustmentDeduction_RelationshipType { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_扶養親族等所得金額
		/// </summary>
		public int? AdjustmentDeduction_TotalEarnings { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_特別障害者該当事実
		/// </summary>
		public string AdjustmentDeduction_Comment { get; set; }


		/// <summary>
		/// 所得金額調整控除申告書_特別障害者申告区分
		/// </summary>
		public string AdjustmentDeduction_ReportType { get; set; }

        //2025-11-18 iwai-tamura upd-str ------
        /// <summary>
        /// 特定親族特別控除申告書01_氏名_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Name1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_氏名_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Name2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_Kana_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Kana1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_Kana_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Kana2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_続柄
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_RelationshipType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_生年月日
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Birthday { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_生年月日_年
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_BirthdayYear { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_生年月日_月
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_BirthdayMonth { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_生年月日_日
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_BirthdayDay { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_住所
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_Address { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_非居住者
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_ResidentCheck { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_給与所得_収入金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction01_Earnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_給与所得_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction01_Income { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_他_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction01_OtherIncome { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_合計所得金額見積額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction01_TotalEarnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_控除額計算判定
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_EarningsType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_控除額計算区分
        /// </summary>
        public string SpecificRelativeSpecialDeduction01_CalcType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書01_特定親族特別控除額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction01_SpecialDeductionAmount { get; set; }



        // -------------------------------------------------------------

        /// <summary>
        /// 特定親族特別控除申告書02_氏名_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Name1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_氏名_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Name2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_Kana_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Kana1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_Kana_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Kana2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_続柄
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_RelationshipType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_生年月日
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Birthday { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_生年月日_年
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_BirthdayYear { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_生年月日_月
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_BirthdayMonth { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_生年月日_日
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_BirthdayDay { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_住所
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_Address { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_非居住者
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_ResidentCheck { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_給与所得_収入金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction02_Earnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_給与所得_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction02_Income { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_他_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction02_OtherIncome { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_合計所得金額見積額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction02_TotalEarnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_控除額計算判定
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_EarningsType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_控除額計算区分
        /// </summary>
        public string SpecificRelativeSpecialDeduction02_CalcType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書02_特定親族特別控除額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction02_SpecialDeductionAmount { get; set; }



        // -------------------------------------------------------------

        /// <summary>
        /// 特定親族特別控除申告書03_氏名_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Name1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_氏名_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Name2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_Kana_姓
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Kana1 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_Kana_名
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Kana2 { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_続柄
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_RelationshipType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_生年月日
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Birthday { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_生年月日_年
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_BirthdayYear { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_生年月日_月
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_BirthdayMonth { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_生年月日_日
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_BirthdayDay { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_住所
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_Address { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_非居住者
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_ResidentCheck { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_給与所得_収入金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction03_Earnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_給与所得_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction03_Income { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_他_所得金額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction03_OtherIncome { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_合計所得金額見積額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction03_TotalEarnings { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_控除額計算判定
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_EarningsType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_控除額計算区分
        /// </summary>
        public string SpecificRelativeSpecialDeduction03_CalcType { get; set; }

        /// <summary>
        /// 特定親族特別控除申告書03_特定親族特別控除額
        /// </summary>
        public int? SpecificRelativeSpecialDeduction03_SpecialDeductionAmount { get; set; }


        //参考にするため扶養控除申告書から特定扶養のデータ取得する用
        /// <summary>
        /// 特定扶養01_続柄（参考表示用）
        /// </summary>
        public string RefSpecificDependent01_RelationshipType { get; set; }

        /// <summary>
        /// 特定扶養01_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent01_Earnings { get; set; }
        /// <summary>
        /// 特定扶養01_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent01_Income { get; set; }
        /// <summary>
        /// 特定扶養01_他_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent01_OtherIncome { get; set; }

        /// <summary>
        /// 特定扶養01_所得見積額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent01_TotalEarnings { get; set; }

        /// <summary>
        /// 特定扶養02_続柄（参考表示用）
        /// </summary>
        public string RefSpecificDependent02_RelationshipType { get; set; }

        /// <summary>
        /// 特定扶養02_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent02_Earnings { get; set; }
        /// <summary>
        /// 特定扶養02_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent02_Income { get; set; }
        /// <summary>
        /// 特定扶養02_他_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent02_OtherIncome { get; set; }

        /// <summary>
        /// 特定扶養02_所得見積額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent02_TotalEarnings { get; set; }

        /// <summary>
        /// 特定扶養03_続柄（参考表示用）
        /// </summary>
        public string RefSpecificDependent03_RelationshipType { get; set; }

        /// <summary>
        /// 特定扶養03_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent03_Earnings { get; set; }
        /// <summary>
        /// 特定扶養03_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent03_Income { get; set; }
        /// <summary>
        /// 特定扶養03_他_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent03_OtherIncome { get; set; }

        /// <summary>
        /// 特定扶養03_所得見積額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent03_TotalEarnings { get; set; }

        /// <summary>
        /// 特定扶養04_続柄（参考表示用）
        /// </summary>
        public string RefSpecificDependent04_RelationshipType { get; set; }

        /// <summary>
        /// 特定扶養04_給与所得_収入金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent04_Earnings { get; set; }
        /// <summary>
        /// 特定扶養04_給与所得_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent04_Income { get; set; }
        /// <summary>
        /// 特定扶養04_他_所得金額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent04_OtherIncome { get; set; }

        /// <summary>
        /// 特定扶養04_所得見積額（参考表示用）
        /// </summary>
        public int? RefSpecificDependent04_TotalEarnings { get; set; }
        //2025-11-18 iwai-tamura upd-end ------
    }

    /// <summary>
    /// ボディ
    /// </summary>
    public class HaiguuDeclareRegisterBodyModel {

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
