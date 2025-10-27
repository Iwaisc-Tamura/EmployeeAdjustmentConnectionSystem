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
    public class ZenshokuDeclareRegisterViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public ZenshokuDeclareRegisterHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public ZenshokuDeclareRegisterBodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class ZenshokuDeclareRegisterHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ZenshokuDeclareRegisterHeaderModel() {
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
        /// 支払金額
        /// </summary>
        public int? PaymentAmount { get; set; }

        /// <summary>
        /// 社会保険料等金額
        /// </summary>
        public int? SocialInsuranceAmount { get; set; }

        /// <summary>
        /// 源泉徴収税額
        /// </summary>
        public int? WithholdingTaxAmount { get; set; }

        /// <summary>
        /// 会社名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 会社住所
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// 退職日
        /// </summary>
        public string RetirementDate { get; set; }

        /// <summary>
        /// 退職日_Date_年
        /// </summary>
        public string RetirementDateYear { get; set; }

        /// <summary>
        /// 退職日_Date_月
        /// </summary>
        public string RetirementDateMonth { get; set; }

        /// <summary>
        /// 退職日_Date_日
        /// </summary>
        public string RetirementDateDay { get; set; }

        /// <summary>
        /// 備考
        /// </summary>
        public string Remarks { get; set; }

    }

    /// <summary>
    /// ボディ
    /// </summary>
    public class ZenshokuDeclareRegisterBodyModel {
    }

}
