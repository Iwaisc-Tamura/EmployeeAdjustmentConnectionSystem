/*エンターキー無効化*/
$(document).keypress(function (e) {
    // エンターキーだったら無効にする
    if (e.key === 'Enter') {
        return false;
    }
});

/* ラジオチェック戻し */
$('input:radio').dblclick(function () {
    var nombre = $(this).attr('id');
    var checked = $(this).is(":checked");
    if (checked) {
        $("input[id=" + nombre + "]:radio").prop("checked", false);
    }
});


$(function () {
    //初回 入力制御
    //$('#Head_HouseholdSelfCheck').change()
    //$('#Head_TaxWithholding_notSubject').change();
    //$('#Head_DependentsOver16_1_notSubject').change();
    //$('#Head_DependentsOther_GeneralHandicappedDependentsCheck').change();
    //$('#Head_DependentsOther_SpecialHandicappedDependentsCheck').change();
    //$('#Head_DependentsOther_LivingHandicappedDependentsCheck').change();
    //$('#Head_DependentsUnder16_1_notSubject').change();

    //datepicker設定
    $.datepicker.setDefaults($.datepicker.regional['ja']);
    $('.ymd-control').datepicker();

    //入力モード制御
    switch ($('#Head_InputMode').val()) {
        //本人入力,管理入力
        case 'SelfInput':
        case 'adminInput':
            //$(".form-control").attr('disabled', true);
            //$(".input-control").attr('disabled', true);
            //$("button.input-control").remove();
            break;

            //本人確定
        case 'SelfConfim':
        case 'adminConfim':
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();
            break;

        default:
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            $("button.input-control").remove();

            break;
    }
});



//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});





//生命保険入力制御
//計算ボタン
function calcLifeInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""
    var id2 = ""

    $("#Head_LifeInsurance_NewTotalAmount").val(null);
    $("#Head_LifeInsurance_OldTotalAmount").val(null);
    $("#Head_LifeInsurance_Calc1").val(null);
    $("#Head_LifeInsurance_Calc2").val(null);
    $("#Head_LifeInsurance_TotalAmount").val(null);
    $("#Head_LifeInsurance_DeductionAmount").val(null);

    //新旧区分チェック
    for (var i = 1; i < 5; i++) {
        id1 = "Head_LifeInsurance_" + i + "_OldAndNewType"
        id2 = "Head_LifeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        alert(strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intNewTotalAmount = 0;
    var intOldTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 5; i++) {
        id1 = "Head_LifeInsurance_" + i + "_OldAndNewType"
        id2 = "Head_LifeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id1).value == "1") {
            intNewTotalAmount += Number(document.getElementById(id2).value);
        }
        if (document.getElementById(id1).value == "2") {
            intOldTotalAmount += Number(document.getElementById(id2).value);
        }
    }

    if (intNewTotalAmount <= 20000) {
        intCalc1 = intNewTotalAmount
    } else if (intNewTotalAmount <= 40000) {
        intCalc1 = Math.ceil(intNewTotalAmount / 2) + 10000

    } else if (intNewTotalAmount <= 80000) {
        intCalc1 = Math.ceil(intNewTotalAmount / 4) + 20000
    } else {
        intCalc1 = 40000
    }

    if (intOldTotalAmount <= 25000) {
        intCalc2 = intOldTotalAmount
    } else if (intOldTotalAmount <= 50000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 2) + 12500

    } else if (intOldTotalAmount <= 100000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 4) + 25000
    } else {
        intCalc2 = 50000
    }

    intTotalAmount = intCalc1 + intCalc2;
    if (intTotalAmount > 40000) {
        intTotalAmount = 40000;
    }
    if (intTotalAmount > intCalc2) {
        intDeductionAmount = intTotalAmount;
    } else {
        intDeductionAmount = intCalc2;
    }
    $("#Head_LifeInsurance_NewTotalAmount").val(intNewTotalAmount);
    $("#Head_LifeInsurance_OldTotalAmount").val(intOldTotalAmount);
    $("#Head_LifeInsurance_Calc1").val(intCalc1);
    $("#Head_LifeInsurance_Calc2").val(intCalc2);
    $("#Head_LifeInsurance_TotalAmount").val(intTotalAmount);
    $("#Head_LifeInsurance_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}

$('#btn-lifeInsurance-calc').click(function () {
    calcLifeInsurance();
});


//介護医療保険入力制御
//計算ボタン
function calcMedicalInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""


    $("#Head_MedicalInsurance_TotalAmount").val(null);
    $("#Head_MedicalInsurance_DeductionAmount").val(null);

    //金額計算処理
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 3; i++) {
        id1 = "Head_MedicalInsurance_" + i + "_InsuranceFee"
        intTotalAmount += Number(document.getElementById(id1).value);
    }

    if (intTotalAmount <= 20000) {
        intCalc1 = intTotalAmount
    } else if (intTotalAmount <= 40000) {
        intCalc1 = Math.ceil(intTotalAmount / 2) + 10000

    } else if (intTotalAmount <= 80000) {
        intCalc1 = Math.ceil(intTotalAmount / 4) + 20000
    } else {
        intCalc1 = 40000
    }

    intDeductionAmount = intCalc1
    if (intDeductionAmount > 40000) {
        intDeductionAmount = 40000;
    }
    $("#Head_MedicalInsurance_TotalAmount").val(intTotalAmount);
    $("#Head_MedicalInsurance_DeductionAmount").val(intDeductionAmount);

    bolReturn = true;
    return bolReturn;
}
$('#btn-medicalInsurance-calc').click(function () {
    calcMedicalInsurance();
});


//個人年金保険入力制御
//計算ボタン
function calcPensionInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""
    var id2 = ""


    $("#Head_PensionInsurance_NewTotalAmount").val(null);
    $("#Head_PensionInsurance_OldTotalAmount").val(null);
    $("#Head_PensionInsurance_Calc1").val(null);
    $("#Head_PensionInsurance_Calc2").val(null);
    $("#Head_PensionInsurance_TotalAmount").val(null);
    $("#Head_PensionInsurance_DeductionAmount").val(null);


    //新旧区分チェック
    for (var i = 1; i < 4; i++) {
        id1 = "Head_PensionInsurance_" + i + "_OldAndNewType"
        id2 = "Head_PensionInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        alert(strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intNewTotalAmount = 0;
    var intOldTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 4; i++) {
        id1 = "Head_PensionInsurance_" + i + "_OldAndNewType"
        id2 = "Head_PensionInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id1).value == "1") {
            intNewTotalAmount += Number(document.getElementById(id2).value);
        }
        if (document.getElementById(id1).value == "2") {
            intOldTotalAmount += Number(document.getElementById(id2).value);
        }
    }

    if (intNewTotalAmount <= 20000) {
        intCalc1 = intNewTotalAmount
    } else if (intNewTotalAmount <= 40000) {
        intCalc1 = Math.ceil(intNewTotalAmount / 2) + 10000

    } else if (intNewTotalAmount <= 80000) {
        intCalc1 = Math.ceil(intNewTotalAmount / 4) + 20000
    } else {
        intCalc1 = 40000
    }

    if (intOldTotalAmount <= 25000) {
        intCalc2 = intOldTotalAmount
    } else if (intOldTotalAmount <= 50000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 2) + 12500

    } else if (intOldTotalAmount <= 100000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 4) + 25000
    } else {
        intCalc2 = 50000
    }

    intTotalAmount = intCalc1 + intCalc2;
    if (intTotalAmount > 40000) {
        intTotalAmount = 40000;
    }
    if (intTotalAmount > intCalc2) {
        intDeductionAmount = intTotalAmount;
    } else {
        intDeductionAmount = intCalc2;
    }
    $("#Head_PensionInsurance_NewTotalAmount").val(intNewTotalAmount);
    $("#Head_PensionInsurance_OldTotalAmount").val(intOldTotalAmount);
    $("#Head_PensionInsurance_Calc1").val(intCalc1);
    $("#Head_PensionInsurance_Calc2").val(intCalc2);
    $("#Head_PensionInsurance_TotalAmount").val(intTotalAmount);
    $("#Head_PensionInsurance_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}

$('#btn-PensionInsurance-calc').click(function () {
    calcPensionInsurance()
});

//生命保険料控除額入力制御
//計算ボタン
function calcAllLifeInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""

    $("#Head_AllLifeInsurance_DeductionAmount").val(null);

    //金額計算処理
    var intLifeAmount = 0;
    var intMedicalAmount = 0;
    var intPensionIAmount = 0;
    var intDeductionAmount = 0;

    intLifeAmount = $("#Head_LifeInsurance_DeductionAmount").val();
    intMedicalAmount = $("#Head_MedicalInsurance_DeductionAmount").val();
    intPensionIAmount = $("#Head_PensionInsurance_DeductionAmount").val();

    intDeductionAmount = Number(intLifeAmount) + Number(intMedicalAmount) + Number(intPensionIAmount);
    if (intDeductionAmount > 120000) {
        intDeductionAmount = 120000;
    }
    $("#Head_AllLifeInsurance_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}

$('#btn-AllLifeInsurance-calc').click(function () {
    calcAllLifeInsurance()
});


//地震保険入力制御
//計算ボタン
function calcQuakeInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""
    var id2 = ""


    $("#Head_QuakeInsurance_QuakeAmount").val(null);
    $("#Head_QuakeInsurance_DamageTotalAmount").val(null);
    $("#Head_QuakeInsurance_Calc1").val(null);
    $("#Head_QuakeInsurance_Calc2").val(null);
    $("#Head_QuakeInsurance_DeductionAmount").val(null);


    //新旧区分チェック
    for (var i = 1; i < 3; i++) {
        id1 = "Head_QuakeInsurance_" + i + "_QuakeAndDamageType"
        id2 = "Head_QuakeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "地震または旧長期区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        alert(strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intQuakeTotalAmount = 0;
    var intDamageTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 3; i++) {
        id1 = "Head_QuakeInsurance_" + i + "_QuakeAndDamageType"
        id2 = "Head_QuakeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id1).value == "1") {
            intQuakeTotalAmount += Number(document.getElementById(id2).value);
        }
        if (document.getElementById(id1).value == "2") {
            intDamageTotalAmount += Number(document.getElementById(id2).value);
        }
    }

    intCalc1 = intQuakeTotalAmount;
    if (intQuakeTotalAmount > 50000) {
        intCalc1 = 50000;
    }

    if (intDamageTotalAmount >= 10000) {
        intCalc2 = Math.ceil(intDamageTotalAmount / 2) + 5000
    } else {
        intCalc2 = intDamageTotalAmount;
    }
    if (intDamageTotalAmount > 15000) {
        intCalc2 = 15000;
    }

    intDeductionAmount = intCalc1 + intCalc2;
    if (intDeductionAmount > 50000) {
        intDeductionAmount = 50000;
    }
    $("#Head_QuakeInsurance_QuakeAmount").val(intQuakeTotalAmount);
    $("#Head_QuakeInsurance_DamageTotalAmount").val(intDamageTotalAmount);
    $("#Head_QuakeInsurance_Calc1").val(intCalc1);
    $("#Head_QuakeInsurance_Calc2").val(intCalc2);
    $("#Head_QuakeInsurance_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}
$('#btn-QuakeInsurance-calc').click(function () {
    calcQuakeInsurance()
});

//社会保険入力制御
//計算ボタン
function calcSocialInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""


    $("#Head_SocialInsurance_DeductionAmount").val(null);

    //金額計算処理
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 3; i++) {
        id1 = "Head_SocialInsurance_" + i + "_InsuranceFee"
        intTotalAmount += Number(document.getElementById(id1).value);
    }

    intDeductionAmount = intTotalAmount
    $("#Head_SocialInsurance_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}

$('#btn-SocialInsurance-calc').click(function () {
    calcSocialInsurance()
});

//小規模企業共済等入力制御
//計算ボタン
function calcSmallScaleMutualAid() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""


    $("#Head_SmallScaleMutualAid_DeductionAmount").val(null);

    //金額計算処理
    var intTotalAmount = 0;
    var intDeductionAmount = 0;
    intTotalAmount += Number(document.getElementById("Head_SmallScaleMutualAid_MutualAidCost").value);
    intTotalAmount += Number(document.getElementById("Head_SmallScaleMutualAid_CorporatePensionCost").value);
    intTotalAmount += Number(document.getElementById("Head_SmallScaleMutualAid_PersonalPensionCost").value);
    intTotalAmount += Number(document.getElementById("Head_SmallScaleMutualAid_HandicappedMutualAidCost").value);

    intDeductionAmount = intTotalAmount
    $("#Head_SmallScaleMutualAid_DeductionAmount").val(intDeductionAmount);
    bolReturn = true;
    return bolReturn;
}

$('#btn-SmallScaleMutualAid-calc').click(function () {
    calcSmallScaleMutualAid()
});







/*###### 入力チェック ######*/
/*所属番号*/
$('#Head_DepartmentNo').change(function () {
    var strVal = $('#Head_DepartmentNo').val()
    var strMessage = ""
    var bolReturn = false
    if (strVal.length != 5) {
        strMessage = "5文字で入力してください。";
    } else if ($.isNumeric(strVal) == false) {
        strMessage = "数値でで入力してください。";
    } else {
        bolReturn = true
    }
    $("#Head_DepartmentNo_mes").text(strMessage);
    return bolReturn;
});

/*チェック関数*/
/*日付*/
function checkDate(id) {
    var strYear = $(id + 'Year').val();
    var strMonth = $(id + 'Month').val();
    var strDay = $(id + 'Day').val();
    var strMessage = "";
    var bolReturn = false;

    $(id + "_mes").text("");

    if (strYear == "" && strMonth == "" && strDay == "") return true;
    if (!strYear || !strMonth || !strDay) {
        strMessage = "年月日が正しく入力されていません。";
        bolReturn = false;
    } else if (!String(strYear).match(/^[0-9]{4}$/) || !String(strMonth).match(/^[0-9]{1,2}$/) || !String(strDay).match(/^[0-9]{1,2}$/)) {
        strMessage = "年月日が正しく入力されていません。";
        bolReturn = false;
    } else {
        var dateObj = new Date(strYear, strMonth - 1, strDay),
            dateObjStr = dateObj.getFullYear() + '' + (dateObj.getMonth() + 1) + '' + dateObj.getDate(),
            checkDateStr = strYear + '' + strMonth + '' + strDay;
        if (dateObjStr != checkDateStr) {
            strMessage = "年月日が正しく入力されていません。";
            bolReturn = false;
        } else {
            return true;
        }
    }

    $(id + "_mes").text(strMessage);
    return bolReturn;
}

//半角カナチェック
$('.checkKana').change(function () {
    var strId = "#" + this.id.slice(0, -1)  //共通利用するため一旦変換(Kana_1,Kana_2⇒"Kana_)"
    var strMessage = "";
    var bolReturn = false;

    var reg = new RegExp(/^[ｦ-ﾟ]*$/);   //使用可能文字指定(半角カナのみ)

    //2023-99-99 iwai-tamura upd str -----
    var convertedValue = zenkana2Hankana(hira2Kana($(this).val()));
    $(this).val(convertedValue);
    //2023-99-99 iwai-tamura upd end -----

    //Kana_1とKana_2に半角カナ以外が入力されていないかチェック
    if (reg.test($(strId + "1").val()) && reg.test($(strId + "2").val())) {
        bolReturn = true;
    } else {
        strMessage = "半角カナ以外は入力できません。"
        bolReturn = false;
    }
    $(strId + "_mes").text(strMessage);
    return bolReturn;
});


//金額チェック
function checkMoney(id, maxMoney) {
    var strVal = $(id).val();
    var strMessage = "";
    var bolReturn = false;
    strVal = strVal.replace(/[^0-9]+/i, '');    //数値のみ入力可能
    $(id).val(strVal);

    if (strVal == "") {
        bolReturn = true;
    } else if ($.isNumeric(strVal) == false) {
        strMessage = "数値でで入力してください。";
    } else if ((strVal) > maxMoney) {
        strMessage = maxMoney + "円以上は入力できません。";
    } else {
        bolReturn = true;
    }
    $(id + "_mes").text(strMessage);
    return bolReturn;
}


//一括チェック
function checkAll() {
    message = '';
    var aryCheckInput = "";
    var aryCheckSelect = "";

    ///本人情報チェック
    //チェック項目チェック

    //入力項目チェック
    aryCheckInput = ["Head_DepartmentNo"
                    , "Head_Name1"
                    , "Head_Name2"
                    , "Head_Kana1"
                    , "Head_Kana2"
                    , "Head_Address"
    ];
    $.each(aryCheckInput, function (index, value) {
        if (document.getElementById(value).value == "") {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }

    })
    //選択項目チェック
    //aryCheckSelect = ["Head.SpouseCheck"
    //                , "Head.SpouseCheck"
    //];
    //$.each(aryCheckSelect, function (index, value) {
    //    if ($("input[name='" + value + "']:checked").length === 0) {
    //        message = '必須項目が入力されていません。<br/>確認してください。';
    //    }
    //})


    //画面のエラー項目全チェック
    $(".check-comment").each(function (i, e) {
        if ($(e).text().length > 1) {
            message = '入力に誤りがあります。<br/>確認してください。';
        }
    });




    //if (!checkMoney('#Head_TaxWithholding_Income', 950000)) {
    //    message = 'Ａ 源泉控除対象配偶者　所得の見積額が不正です。';
    //}
    return message
}

//計算チェック
function checkAllCalc() {
    if (!calcLifeInsurance()) { return false }
    if (!calcMedicalInsurance()) { return false }
    if (!calcPensionInsurance()) { return false }
    if (!calcAllLifeInsurance()) { return false }
    if (!calcQuakeInsurance()) { return false }
    if (!calcSocialInsurance()) { return false }
    if (!calcSmallScaleMutualAid()) { return false }
    return true;
}




/*
 * 途中保存ボタンクリック時
 */
$('#dmykeep').click(function () {
    message = '';

    //入力チェック
    message = checkAll();

    if (message != '') {
        showAlert('確認', message)
        return;
    }
    if (!checkAllCalc()){return;}

    //ボタンクリック
    showMessageEx('途中保存確認', '保存しますか？', 'keepbutton', true);
});

/*
 * 承認保存ボタンクリック時
 */
$('#dmysave').click(function () {
    message = '';
    //入力チェック
    message = checkAll();

    if (message != '') {
        showAlert('確認', message)
        return;
    }
    if (!checkAllCalc()){return;}

    //ボタンクリック
    //2023-99-99 iwai-tamura upd str -----
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
    } else {
        showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
    }
    //showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
    //2023-99-99 iwai-tamura upd end -----
});

/*
 * 承認キャンセルボタンクリック時
 */
$('#dmySignCancel').click(function () {
    //ボタンクリック
    //2023-99-99 iwai-tamura upd str -----
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        showMessageEx('取消確認', '確定状態を取消しますか？', 'signcancel', true);
    } else {
        showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
    }
    //showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
    //2023-99-99 iwai-tamura upd end -----
});




/*
 * ダイアログメッセージ表示(OkCancel)
 */
function showMessageEx(title, message, buttonid, isLoding) {

    // ダイアログのメッセージを設定
    $('#show_dialog').html(message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: title,
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                //2016-05-23 iwai-tamura upd str 途中保存のPDF出力をやめる -----
                $('#' + buttonid).trigger('click');
                //showPrintMessage('PDF出力確認', 'PDFを出力しますか？', buttonid, 'save2printbutton');
                //2016-05-23 iwai-tamura upd str -----
            },
            'キャンセル': function () {
                $(this).dialog('close');
                return;
            }
        },
        open: function () {						// キャンセルボタンにフォーカスをあてる
            $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
        }
    });
}

/*
 * ダイアログメッセージ表示(PDF)
 */
function showPrintMessage(title, message, buttonid1, buttonid2) {

    // ダイアログのメッセージを設定
    $('#show_dialog').html(message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: title,
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                $('#' + buttonid2).trigger('click');
                return;
            },
            'キャンセル': function () {
                $(this).dialog('close');
                $('#' + buttonid1).trigger('click');
                return;
            }
        },
        open: function () {						// キャンセルボタンにフォーカスをあてる
            $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
        }
    });
}


//目標面談日チェック
function objDateCheck() {
    var value = $('#Head_ObjYear').val();
    if (jQuery.trim(value) === '') {
        return false;
    }
    var ds = Date.parse(value);
    var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
    var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
    //var fds = new Date(ds);
    //var mind = new Date(min);
    //var maxd = new Date(max);
    if (!(min <= ds && ds <= max)) {
        return false;
    }
    return true;
}

//達成面談日チェック
function achvDateCheck() {
    var value = $('#Head_AchvYear').val();
    if (jQuery.trim(value) === '') {
        return false;
    }
    var ds = Date.parse(value);
    var min = Date.parse(parseInt($('#Head_JcalYear').val()) - 1 + '/4/1');
    var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/12/31');
    if (!(min <= ds && ds <= max)) {
        return false;
    }
    return true;
}

//異動日チェック
function trnsDateCheck() {
    var value = $('#Head_TransferYear').val();
    if (jQuery.trim(value) === '') {
        return true;
    }
    var ds = Date.parse(value);
    var min = Date.parse(parseInt($('#Head_JcalYear').val()) + '/4/1');
    var max = Date.parse(parseInt($('#Head_JcalYear').val()) + 1 + '/3/31');
    if (!(min <= ds && ds <= max)) {
        return false;
    }
    return true;
}


//2016-05-23 iwai-tamura upd str  -----
//ウェイトチェック
function selfWeightCheck() {
    var value = 0;
    for (i = 0; i < 5; i++) {
        value += Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val()))
    }
    if (!(value === 100)) {
        return false;
    }
    return true;
}

function bossWeightCheck() {
    var value = 0;
    for (i = 0; i < 5; i++) {
        value += Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val()))
    }
    if (!(value === 100)) {
        return false;
    }
    return true;
}
//2016-05-23 iwai-tamura upd end -----

//2017-04-11 iwai-tamura upd str  -----
//達成度・プロセスチェック
function selfAchvCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__SelfAchv').val()) === "") {
                return false;
            }
        }
    }
    return true;
}

function bossAchvCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__BossAchv').val()) === "") {
                return false;
            }
        }
    }
    return true;
}
function selfProcessCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__SelfProcess').val()) === "") {
                return false;
            }
        }
    }
    return true;
}

function bossProcessCheck() {
    for (i = 0; i < 5; i++) {
        if (Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0) {
            if (jQuery.trim($('#ObjList_' + i + '__BossProcess').val()) === "") {
                return false;
            }
        }
    }
    return true;
}
//2017-04-11 iwai-tamura upd end  -----
