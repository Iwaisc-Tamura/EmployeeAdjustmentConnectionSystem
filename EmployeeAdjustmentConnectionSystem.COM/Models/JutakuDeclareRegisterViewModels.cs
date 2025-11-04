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
    public class JutakuDeclareRegisterViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public JutakuDeclareRegisterHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public JutakuDeclareRegisterBodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class JutakuDeclareRegisterHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public JutakuDeclareRegisterHeaderModel() {
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
		/// 作成区分
		/// </summary>
		public string CreateType { get; set; }

		/// <summary>
		/// 本人確定区分
		/// </summary>
		public string ApprovalType { get; set; }

		/// <summary>
		/// 管理者確定区分
		/// </summary>
		public string DecisionType { get; set; }

		/// <summary>
		/// ステータス名
		/// </summary>
		public string StatusName { get; set; }

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
        /// 住宅借入金等特別控除適用数
        /// </summary>
        public string HousingLoanSpecialDeduction_ApplyCount { get; set; }

        /// <summary>
        /// 居住開始年月日_1回目
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart1Date { get; set; }

        /// <summary>
        /// 居住開始年月日_1回目_年
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart1DateYear { get; set; }

        /// <summary>
        /// 居住開始年月日_1回目_月
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart1DateMonth { get; set; }

        /// <summary>
        /// 居住開始年月日_1回目_日
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart1DateDay { get; set; }

        /// <summary>
        /// 居住開始年月日_2回目
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart2Date { get; set; }

        /// <summary>
        /// 居住開始年月日_2回目_年
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart2DateYear { get; set; }

        /// <summary>
        /// 居住開始年月日_2回目_月
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart2DateMonth { get; set; }

        /// <summary>
        /// 居住開始年月日_2回目_日
        /// </summary>
        public string HousingLoanSpecialDeduction_ResidenceStart2DateDay { get; set; }

        /// <summary>
        /// 住宅借入金等特別控除区分_1回目
        /// </summary>
        public string HousingLoanSpecialDeduction_Type1 { get; set; }

        /// <summary>
        /// 住宅借入金等特別控除区分_2回目
        /// </summary>
        public string HousingLoanSpecialDeduction_Type2 { get; set; }

        /// <summary>
        /// 住宅借入金等年末残高_1回目
        /// </summary>
        public int? HousingLoanSpecialDeduction_YearEndBalance1 { get; set; }

        /// <summary>
        /// 住宅借入金等年末残高_2回目
        /// </summary>
        public int? HousingLoanSpecialDeduction_YearEndBalance2 { get; set; }
    }

    /// <summary>
    /// ボディ
    /// </summary>
    public class JutakuDeclareRegisterBodyModel {

    }

}
