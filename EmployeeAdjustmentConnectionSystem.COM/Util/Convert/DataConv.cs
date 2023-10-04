using System;
using System.Collections;
using System.Security.Cryptography;
using System.Data;
using EmployeeAdjustmentConnectionSystem.COM.Enum;
using System.Globalization;

namespace EmployeeAdjustmentConnectionSystem.COM.Util.Convert {
    /// <summary>
    /// ============================================================================== 
    /// Iwai Project                                  CopyRight(C)�@SBC 
    /// ------------------------------------------------------------------------------ 
    /// �T�C�g�@�@:�@�E�\�V�X�e�� 
    /// �@�\���@�@:�@�f�[�^�ϊ�
    /// �t�@�C����:�@DataConv.cs
    /// �����敪�@:�@���ʃ��[�e�B���e�B
    /// �쐬���@�@:�@2015/02/26 
    /// �쐬�ҁ@�@:�@SBC Katoh
    /// �o�[�W����:�@1.0 
    /// �@�\�T�v�@:  �f�[�^�ϊ����s��
    ///  
    /// ============================================================================== 
    /// </summary>
    public static class DataConv {
        /// <summary>
        /// Int�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <returns>���l</returns>
        /// <remarks>
        /// ������𐔒l�ϊ����܂��A�ϊ����s����NULL
        /// </remarks>
        public static int? IntParse(string value) {
            return IntParse(value, null);
        }

        /// <summary>
        /// Int�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <param name="nullConv">�k�������l</param>
        /// <returns>���l</returns>
        /// <remarks>
        /// ������𐔒l�ϊ����܂��A�ϊ����s�������NULL���̓k�������l��Ԃ�
        /// </remarks>
        public static int? IntParse(string value, int? nullConv) {
            int result;
            int? rtValue = (nullConv == null) ? null : nullConv;
            if(int.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// long�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <returns>���l</returns>
        /// <remarks>
        /// ������𐔒l�ϊ����܂��A�ϊ����s����NULL
        /// </remarks>
        public static long? LongParse(string value) {
            return LongParse(value, null);
        }

        /// <summary>
        /// long�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <param name="nullConv">�k�������l</param>
        /// <returns>���l</returns>
        /// <remarks>
        /// ������𐔒l�ϊ����܂��A�ϊ����s�������NULL���̓k�������l��Ԃ�
        /// </remarks>
        public static long? LongParse(string value, long? nullConv) {
            long result;
            long? rtValue = (nullConv == null) ? null : nullConv;
            if(long.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// DateTime�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <returns>����</returns>
        /// <remarks>
        /// �����������ϊ����܂��A�ϊ����s����NULL
        /// </remarks>
        public static DateTime? DateTimeParse(string value) {
            return DateTimeParse(value, null);
        }

        /// <summary>
        /// DateTime�ϊ�
        /// </summary>
        /// <param name="value">�ϊ��������f�[�^</param>
        /// <param name="nullConv">�k�����l</param>
        /// <returns>����</returns>
        /// <remarks>
        /// �����������ϊ����܂��A�ϊ����s�������NULL���̓k�����l��Ԃ�
        /// </remarks>
        public static DateTime? DateTimeParse(string value, DateTime? nullConv) {
            DateTime result;
            DateTime? rtValue = (nullConv == null) ? null : nullConv;
            if(DateTime.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }

        /// <summary>
        /// �N��������
        /// </summary>
        /// <param name="value">���t�̕�����</param>
        /// <param name="dEnum">���t�pEnum</param>
        /// <returns>������������</returns>
        public static string Date2String(string value, DateEnum dEnum) {
            DateTime? dt = DateTimeParse(value, null);
            if(dt == null) return "";

            var retval = "";
            switch(dEnum) {
                case DateEnum.YEAR:
                    retval = dt.Value.Year.ToString();
                    break;
                case DateEnum.MONTH:
                    retval = dt.Value.Month.ToString();
                    break;
                case DateEnum.DAY:
                    retval = dt.Value.Day.ToString();
                    break;
                case DateEnum.YMD:
                    retval = string.Format("{0}/{1}/{2}", dt.Value.Year.ToString(), dt.Value.Month.ToString(), dt.Value.Day.ToString());
                    break;
                case DateEnum.ALL:
                default:
                    retval = dt.ToString();
                    break;
            }
            return retval;
        }

        /// <summary>
        /// �a��ϊ�
        /// </summary>
        /// <param name="dt">���t</param>
        /// <param name="format">�t�H�[�}�b�g</param>
        /// <returns>�a��</returns>
        public static string Date2Jcal(DateTime dt, string format) {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();
            return dt.ToString(format, culture);
        }


        /// <summary>
        /// ���s���폜
        /// </summary>
        /// <param name="val">�Ώە���</param>
        /// <returns>�ϊ���</returns>
        public static String EraseCrLf(string val) {
            //Null�ŋ󕶎�
            return DataConv.EraseCrLf(val, null);
        }

        /// <summary>
        /// ���s���폜���Ēu��
        /// </summary>
        /// <param name="val">�Ώە���</param>
        /// <param name="repVal">�ϊ��p����</param>
        /// <returns>�ϊ���</returns>
        public static String EraseCrLf(string val, string repVal) {
            return val.Replace("\r", "").Replace("\n", repVal == null ? "" : repVal);
        }

        /// <summary>
        /// �^�u�N���A
        /// </summary>
        /// <param name="val">�Ώە���</param>
        /// <returns>�ϊ���</returns>
        public static String EraseTab(string val) {
            //Null�ŋ󕶎�
            return DataConv.EraseTab(val, null);
        }

        /// <summary>
        /// �^�u�N���A���Ēu��
        /// </summary>
        /// <param name="val">�Ώە���</param>
        /// <param name="repVal">�ϊ��p����</param>
        /// <returns>�ϊ���</returns>
        public static String EraseTab(string val, string repVal) {
            return val.Replace("\t", repVal == null ? "" : repVal);
        }

        /// <summary>
        /// �o�^�G���e�B�e�B���o��
        /// </summary>
        /// <param name="entityList">�o�^</param>
        /// <returns>�o�^�G���e�B�e�B</returns>
        public static T GetEntity<T>(IList eList) {
            //���o��
            foreach(T iEntry in eList) { return iEntry; }
            //���Ԃ�
            return default(T);
        }

        /// <summary>
        /// ���t�ɕϊ��ł��邩���肵�A�ϊ��ł��Ȃ��ꍇ�͓��t�̍ő�l��Ԃ�
        /// </summary>
        /// <param name="val">�ϊ���</param>
        /// <returns>����</returns>
        public static string DateTimeTostring(string val) {
            DateTime dt;
            return !DateTime.TryParse(val, out dt) ? DateTime.MaxValue.ToString() : val;
        }

        /// <summary>
        /// ���t�ɕϊ��ł��邩���肵�A�ϊ��o�����ꍇ�͐��`���Ă�������B
        /// </summary>
        /// <param name="val">�ϊ���</param>
        /// <returns>����</returns>
        public static string DateTimeTostring(DateTime? val, string format) {
            return string.IsNullOrEmpty(val.ToString()) ? "" : ((DateTime)val).ToString(format);
        }

        /// <summary>
        /// DateTime�^�ϊ�
        /// </summary>
        /// <param name="value">�ϊ���</param>
        /// <remarks>�l���󔒂̏ꍇ�G���[�ŗ�����̂Œǉ�</remarks>
        /// <returns>����</returns>
        public static string DateTimeToLongTimeString(string value) {
            DateTime dt;
            //��̏ꍇ
            if(value.Trim() == "") { return ""; }
            //�ϊ��ł��Ȃ��ꍇ
            if(!DateTime.TryParse(value, out dt)) { return ""; }
            //�ϊ��ł���                
            return dt.ToString("HH:mm:ss");
        }

        /// <summary>
        /// DateTime�^�ϊ�
        /// </summary>
        /// <param name="value">�ϊ���</param>
        /// <remarks>�l���󔒂̏ꍇ�G���[�ŗ�����̂Œǉ�</remarks>
        /// <returns>����</returns>
        public static string DateTimeToStringEx(string value) {
            string fmt = "{0} {1}";

            if(value == null) return "";

            if(value == "") return value;

            //���t
            string[] date = value.Split(' ');
            string datetimemax = DateTime.MaxValue.ToString();
            DateTime? dval = DataConv.DateTimeParse(date[0]);
            date[0] = dval != null ? ((DateTime)dval).ToString("yyyy/MM/dd") : DateTime.MaxValue.ToString("yyyy/MM/dd");
            dval = DataConv.DateTimeParse(string.Format(fmt, DateTime.MaxValue.ToString("yyyy/MM/dd"), date[1]));
            date[1] = dval != null ? ((DateTime)dval).ToString("HH:mm:ss") : DateTime.MaxValue.ToString("HH:mm:ss");
            return string.Format(fmt, date[0], date[1]);
        }

        /// <summary>
        /// �Í���
        /// </summary>
        /// <param name="value">�Í���������������</param>
        /// <returns>�Í�����������</returns>
        public static string CreateSha256Crypt(string value) {
            try {
                SHA256 shaM = new SHA256Managed();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(value.Trim());
                byte[] bs = shaM.ComputeHash(data);
                //byte�^�z���16�i���̕�����ɕϊ�
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                foreach(byte b in bs) {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString().Trim();
            } catch(Exception) {   //�G���[����NULL��Ԃ��܂��B
                return "";
            }
        }

        /// <summary>
        /// URL�R���o�C��
        /// </summary>
        /// <param name="baseUrl">�x�[�X</param>
        /// <param name="relativeUrl">���������</param>
        /// <returns>�����������ʂȂ�</returns>
        public static string CombineUrl(string baseUrl, string relativeUrl) {
            //�x�[�X����H
            if(string.IsNullOrEmpty(baseUrl))
                return relativeUrl;
            //�x�[�X���ꕶ�����u/�v�̂�
            if(baseUrl.Trim().Length == 1 && baseUrl.Trim().Equals("/"))
                return relativeUrl;

            return baseUrl + relativeUrl;
        }

        /// <summary>
        /// expr1��null�ł�Empty������ł��Ȃ��ꍇ��expr1��Ԃ��A����ȊO�̏ꍇ��DBNull.Value��Ԃ��܂��B
        /// </summary>
        /// <param name="expr1">�l�P</param>
        /// <returns>expr1��null�ł�Empty������ł��Ȃ��ꍇ��expr1�A����ȊO��DBNull.Value�B</returns>
        public static object IfNull(string expr1) {
            return IfNull(expr1, DBNull.Value);
        }

        /// <summary>
        /// expr1��null�ł�Empty������ł��Ȃ��ꍇ��expr1��Ԃ��A����ȊO�̏ꍇ��expr2��Ԃ��܂��B
        /// </summary>
        /// <param name="expr1">�l�P</param>
        /// <param name="expr2">�l�Q</param>
        /// <returns>expr1��null�ł�Empty������ł��Ȃ��ꍇ��expr1�A����ȊO��expr2�B</returns>
        public static object IfNull(string expr1, object expr2) {
            object result = expr1;
            if(string.IsNullOrEmpty(expr1)) {
                result = expr2;
            }
            return result;
        }

        /// <summary>
        /// decimal�ϊ�
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullConv"></param>
        /// <returns></returns>
        public static decimal? Str2Deci(string value, decimal? nullConv) {
            decimal result;
            decimal? rtValue = (nullConv == null) ? null : nullConv;
            if(decimal.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }
        /// <summary>
        /// decimal�ϊ�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal? Str2Deci(string value) {
            return Str2Deci(value, null);
        }

        /// <summary>
        /// int�ϊ�
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nullConv"></param>
        /// <returns></returns>
        public static int? Str2Int(string value, int? nullConv) {
            int result;
            int? rtValue = (nullConv == null) ? null : nullConv;
            if(int.TryParse(value, out result)) rtValue = result;
            return rtValue;
        }
        /// <summary>
        /// int�ϊ�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? Str2Int(string value) {
            return Str2Int(value, null);
        }
    }
}
