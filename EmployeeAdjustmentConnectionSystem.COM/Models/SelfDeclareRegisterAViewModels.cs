using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdjustmentConnectionSystem.COM.Enum;

namespace EmployeeAdjustmentConnectionSystem.COM.Models {
    /// <summary>
    /// 目標管理入力モデル
    /// </summary>
    public class SelfDeclareRegisterAViewModels {
        /// <summary>
        /// ヘッダ部
        /// </summary>
        public SelfDeclareRegisterAHeaderModel Head { get; set; }

        /// <summary>
        /// ボディ部
        /// </summary>
        public SelfDeclareRegisterABodyModel Body { get; set; }

    }

    /// <summary>
    /// ヘッダ
    /// </summary>
    public class SelfDeclareRegisterAHeaderModel {
        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public SelfDeclareRegisterAHeaderModel() {
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
        public SelfDeclareMode InputMode { get; set; }
        /// <summary>
        /// キャンセルボタン
        /// </summary>
        public SelfDeclareMode CancelButton { get; set; }
        /// <summary>
        /// 承認ボタン
        /// </summary>
        public SelfDeclareMode AuthButton { get; set; }

        /// <summary>
        /// 管理番号
        /// </summary>
        public string ManageNo { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int? SheetYear { get; set; }

        /// <summary>
        /// 自己申告書種別Code
        /// </summary>
        public string SelfDeclareCode { get; set; }

        /// <summary>
        /// 社員番号
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 氏名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// カナ
        /// </summary>
        public string Kana { get; set; }

        /// <summary>
        /// 所属番号
        /// </summary>
        public int? DepartmentNo { get; set; }
        
        /// <summary>
        /// 所属名
        /// </summary>
        public string DepartmentName { get; set; }
        
        /// <summary>
        /// 役職番号
        /// </summary>
        public int? PostNo { get; set; }
        
        /// <summary>
        /// 役職名
        /// </summary>
        public string PostName { get; set; }
        
        /// <summary>
        /// 職掌番号
        /// </summary>
        public int? DutyNo { get; set; }
        
        /// <summary>
        /// 職掌名
        /// </summary>
        public string DutyName { get; set; }
        
        /// <summary>
        /// 資格番号
        /// </summary>
        public int? CompetencyNo { get; set; }
        
        /// <summary>
        /// 資格名
        /// </summary>
        public string CompetencyName { get; set; }
        
        /// <summary>
        /// 入社年月日
        /// </summary>
        public string HireDate { get; set; }

        /// <summary>
        /// 入社年月日 表示用
        /// </summary>
        public string HireDateView { get; set; }
        
        /// <summary>
        /// 在籍月数
        /// </summary>
        public int? EnrollmentMonths { get; set; }
        
        /// <summary>
        /// 在籍月数 表示用
        /// </summary>
        public string EnrollmentMonthsView { get; set; }
        
        /// <summary>
        /// 現職経験月数
        /// </summary>
        public int? ExperienceMonths { get; set; }
        
        /// <summary>
        /// 現職経験月数 表示用
        /// </summary>
        public string ExperienceMonthsView { get; set; }
        
        /// <summary>
        /// 生年月日
        /// </summary>
        public string Birthday { get; set; }
        
        /// <summary>
        /// 生年月日 表示用
        /// </summary>
        public string BirthdayView { get; set; }
        
        /// <summary>
        /// 年齢
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 年齢 表示用
        /// </summary>
        public string AgeView { get; set; }


        
        /// <summary>
        /// A～C表本人
        /// </summary>
        public string AtoCSelfSign { get; set; }
        /// <summary>
        /// A～C表上司
        /// </summary>
        public string AtoCBossSign { get; set; }

        /// <summary>
        /// 郵便番号_1
        /// </summary>
        public string PostalCode_1 { get; set; }
        
        /// <summary>
        /// 郵便番号_2
        /// </summary>
        public string PostalCode_2 { get; set; }
        
        /// <summary>
        /// 住所
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 住所形態区分
        /// </summary>
        public string AddressType { get; set; }
        
        /// <summary>
        /// 住所形態内容
        /// </summary>
        public string AddressTypeContent { get; set; }
        
        /// <summary>
        /// 家族構成人数
        /// </summary>
        public string FamilyCount { get; set; }
        
        /// <summary>
        /// 家族構成続柄区分_1
        /// </summary>
        public string Relationship_1 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_1
        /// </summary>
        public string RelationshipContent_1 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_1_Other
        /// </summary>
        public string RelationshipContentOther_1 { get; set; }
        
        /// <summary>
        /// 家族構成年齢_1
        /// </summary>
        public string FamilyAge_1 { get; set; }

        /// <summary>
        /// 家族構成職業学年_1
        /// </summary>
        public string FamilyJob_1 { get; set; }

        /// <summary>
        /// 家族構成同居区分_1
        /// </summary>
        public string FamilyLodger_1 { get; set; }

        /// <summary>
        /// 家族構成扶養区分_1
        /// </summary>
        public string FamilyDependent_1 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_2
        /// </summary>
        public string Relationship_2 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_2
        /// </summary>
        public string RelationshipContent_2 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_2_Other
        /// </summary>
        public string RelationshipContentOther_2 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_2
		/// </summary>
		public string FamilyAge_2 { get; set; }

		/// <summary>
		/// 家族構成職業学年_2
		/// </summary>
		public string FamilyJob_2 { get; set; }

		/// <summary>
		/// 家族構成同居区分_2
		/// </summary>
		public string FamilyLodger_2 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_2
		/// </summary>
		public string FamilyDependent_2 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_3
        /// </summary>
        public string Relationship_3 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_3
        /// </summary>
        public string RelationshipContent_3 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_3_Other
        /// </summary>
        public string RelationshipContentOther_3 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_3
		/// </summary>
		public string FamilyAge_3 { get; set; }

		/// <summary>
		/// 家族構成職業学年_3
		/// </summary>
		public string FamilyJob_3 { get; set; }

		/// <summary>
		/// 家族構成同居区分_3
		/// </summary>
		public string FamilyLodger_3 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_3
		/// </summary>
		public string FamilyDependent_3 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_4
        /// </summary>
        public string Relationship_4 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_4
        /// </summary>
        public string RelationshipContent_4 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_4_Other
        /// </summary>
        public string RelationshipContentOther_4 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_4
		/// </summary>
		public string FamilyAge_4 { get; set; }

		/// <summary>
		/// 家族構成職業学年_4
		/// </summary>
		public string FamilyJob_4 { get; set; }

		/// <summary>
		/// 家族構成同居区分_4
		/// </summary>
		public string FamilyLodger_4 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_4
		/// </summary>
		public string FamilyDependent_4 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_5
        /// </summary>
        public string Relationship_5 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_5
        /// </summary>
        public string RelationshipContent_5 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_5_Other
        /// </summary>
        public string RelationshipContentOther_5 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_5
		/// </summary>
		public string FamilyAge_5 { get; set; }

		/// <summary>
		/// 家族構成職業学年_5
		/// </summary>
		public string FamilyJob_5 { get; set; }

		/// <summary>
		/// 家族構成同居区分_5
		/// </summary>
		public string FamilyLodger_5 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_5
		/// </summary>
		public string FamilyDependent_5 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_6
        /// </summary>
        public string Relationship_6 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_6
        /// </summary>
        public string RelationshipContent_6 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_6_Other
        /// </summary>
        public string RelationshipContentOther_6 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_6
		/// </summary>
		public string FamilyAge_6 { get; set; }

		/// <summary>
		/// 家族構成職業学年_6
		/// </summary>
		public string FamilyJob_6 { get; set; }

		/// <summary>
		/// 家族構成同居区分_6
		/// </summary>
		public string FamilyLodger_6 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_6
		/// </summary>
		public string FamilyDependent_6 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_7
        /// </summary>
        public string Relationship_7 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_7
        /// </summary>
        public string RelationshipContent_7 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_7_Other
        /// </summary>
        public string RelationshipContentOther_7 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_7
		/// </summary>
		public string FamilyAge_7 { get; set; }

		/// <summary>
		/// 家族構成職業学年_7
		/// </summary>
		public string FamilyJob_7 { get; set; }

		/// <summary>
		/// 家族構成同居区分_7
		/// </summary>
		public string FamilyLodger_7 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_7
		/// </summary>
		public string FamilyDependent_7 { get; set; }

        /// <summary>
        /// 家族構成続柄区分_8
        /// </summary>
        public string Relationship_8 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_8
        /// </summary>
        public string RelationshipContent_8 { get; set; }
        
        /// <summary>
        /// 家族構成続柄内容_8_Other
        /// </summary>
        public string RelationshipContentOther_8 { get; set; }
        
		/// <summary>
		/// 家族構成年齢_8
		/// </summary>
		public string FamilyAge_8 { get; set; }

		/// <summary>
		/// 家族構成職業学年_8
		/// </summary>
		public string FamilyJob_8 { get; set; }

		/// <summary>
		/// 家族構成同居区分_8
		/// </summary>
		public string FamilyLodger_8 { get; set; }

		/// <summary>
		/// 家族構成扶養区分_8
		/// </summary>
		public string FamilyDependent_8 { get; set; }

        /// <summary>
        /// 健康状態区分
        /// </summary>
        public string Health { get; set; }
        
        /// <summary>
        /// 健康状態内容
        /// </summary>
        public string HealthContent { get; set; }
        
        /// <summary>
        /// 健康状態不順状態
        /// </summary>
        public string UnHealthContent { get; set; }
        
    }

    /// <summary>
    /// ボディ
    /// </summary>
    public class SelfDeclareRegisterABodyModel {

        /// <summary>
        /// 担当職務_1
        /// </summary>
        public string ChargeDuty_1 { get; set; }

        /// <summary>
        /// 担当職務_2
        /// </summary>
        public string ChargeDuty_2 { get; set; }

        /// <summary>
        /// 担当職務_3
        /// </summary>
        public string ChargeDuty_3 { get; set; }

        /// <summary>
        /// 担当職務_4
        /// </summary>
        public string ChargeDuty_4 { get; set; }

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
        /// 自由意見内容
        /// </summary>
        public string FreeComment { get; set; }

        /// <summary>
        /// 上司記入欄内容
        /// </summary>
        public string BossComment { get; set; }
    }

}
