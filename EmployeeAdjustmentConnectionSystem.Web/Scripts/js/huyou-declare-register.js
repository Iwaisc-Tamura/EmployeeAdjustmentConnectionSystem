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
    $('#Head_HouseholdSelfCheck').change()
    $('#Head_TaxWithholding_notSubject').change();
    $('#Head_DependentsOver16_1_notSubject').change();
    $('#Head_DependentsOver16_2_notSubject').change();
    $('#Head_DependentsOver16_3_notSubject').change();
    $('#Head_DependentsOver16_4_notSubject').change();
    $('#Head_DependentsOther_Subject').change();
    $('#Head_DependentsOther_GeneralHandicappedDependentsCheck').change();
    $('#Head_DependentsOther_SpecialHandicappedDependentsCheck').change();
    $('#Head_DependentsOther_LivingHandicappedDependentsCheck').change();
    $('input:radio[name="Head.DependentsOther_WidowType"]').change()
    $('#Head_DependentsUnder16_1_notSubject').change();
    $('#Head_DependentsUnder16_2_notSubject').change();
    $('#Head_DependentsUnder16_3_notSubject').change();
    $('#Head_DependentsUnder16_4_notSubject').change();

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
            $("button.input-control").remove();
            break;

        default:
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $("button.input-control").remove();

            break;
    }
});

//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});


//本人情報 入力制御
$('#Head_HouseholdSelfCheck').change(function () {
    if (document.getElementById("Head_HouseholdSelfCheck").checked == true) {
        $("#Head_HouseholdName1").attr('disabled', true);
        $("#Head_HouseholdName2").attr('disabled', true);
        $("#Head_RelationshipType").attr('disabled', true);
        $("#Head_HouseholdName1").val(document.getElementById("Head_Name1").value);
        $("#Head_HouseholdName2").val(document.getElementById("Head_Name2").value);
        $("#Head_RelationshipType").val('10');
    } else {
        $("#Head_HouseholdName1").removeAttr("disabled");
        $("#Head_HouseholdName2").removeAttr("disabled");
        $("#Head_RelationshipType").removeAttr("disabled");
    }
});




/*源泉控除対象配偶者 対象者入力制御*/
$('#Head_TaxWithholding_notSubject').change(function () {
    if (document.getElementById("Head_TaxWithholding_notSubject").checked == true) {
        $(".TaxWithholding_isRead").attr('disabled', true);
        $(".TaxWithholdingAddress_isRead").attr('disabled', true);
    } else {
        $(".TaxWithholding_isRead").removeAttr("disabled");
        if ($('input:radio[name="Head.TaxWithholding_ResidentType"]:checked').val() > "0") {
            $(".TaxWithholdingAddress_isRead").removeAttr("disabled");
        } else {
            $(".TaxWithholdingAddress_isRead").attr('disabled', true);
            $("#Head_TaxWithholding_Address").val('');
        }
    }
});
$('input:radio[name="Head.TaxWithholding_ResidentType"]').change(function () {
    if ($('input:radio[name="Head.TaxWithholding_ResidentType"]:checked').val() > "0") {
        $(".TaxWithholdingAddress_isRead").removeAttr("disabled");
    } else {
        $(".TaxWithholdingAddress_isRead").attr('disabled', true);
        $("#Head_TaxWithholding_Address").val('');
    }
});




/*B1 控除対象扶養親族(16歳以上) 対象者入力制御*/
$('#Head_DependentsOver16_1_notSubject').change(function () {
    if (document.getElementById("Head_DependentsOver16_1_notSubject").checked == true) {
        $(".DependentsOver16_1_isRead").attr('disabled', true);
        $(".DependentsOver16_1Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsOver16_1_isRead").removeAttr("disabled");
        if ($('input:radio[name="Head.DependentsOver16_1_ResidentType"]:checked').val() > "0") {
            $(".DependentsOver16_1Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsOver16_1Address_isRead").attr('disabled', true);
        }
    }
});
$('input:radio[name="Head.DependentsOver16_1_ResidentType"]').change(function () {
    if ($('input:radio[name="Head.DependentsOver16_1_ResidentType"]:checked').val() > "0") {
        $(".DependentsOver16_1Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsOver16_1Address_isRead").attr('disabled', true);
        $("#Head_DependentsOver16_1_Address").val('');
    }
});

/*B2 控除対象扶養親族(16歳以上) 対象者入力制御*/
$('#Head_DependentsOver16_2_notSubject').change(function () {
    if (document.getElementById("Head_DependentsOver16_2_notSubject").checked == true) {
        $(".DependentsOver16_2_isRead").attr('disabled', true);
        $(".DependentsOver16_2Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsOver16_2_isRead").removeAttr("disabled");
        if ($('input:radio[name="Head.DependentsOver16_2_ResidentType"]:checked').val() > "0") {
            $(".DependentsOver16_2Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsOver16_2Address_isRead").attr('disabled', true);
        }
    }
});
$('input:radio[name="Head.DependentsOver16_2_ResidentType"]').change(function () {
    if ($('input:radio[name="Head.DependentsOver16_2_ResidentType"]:checked').val() > "0") {
        $(".DependentsOver16_2Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsOver16_2Address_isRead").attr('disabled', true);
        $("#Head_DependentsOver16_2_Address").val('');
    }
});

/*B3 控除対象扶養親族(16歳以上) 対象者入力制御*/
$('#Head_DependentsOver16_3_notSubject').change(function () {
    if (document.getElementById("Head_DependentsOver16_3_notSubject").checked == true) {
        $(".DependentsOver16_3_isRead").attr('disabled', true);
        $(".DependentsOver16_3Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsOver16_3_isRead").removeAttr("disabled");
        if ($('input:radio[name="Head.DependentsOver16_3_ResidentType"]:checked').val() > "0") {
            $(".DependentsOver16_3Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsOver16_3Address_isRead").attr('disabled', true);
        }
    }
});
$('input:radio[name="Head.DependentsOver16_3_ResidentType"]').change(function () {
    if ($('input:radio[name="Head.DependentsOver16_3_ResidentType"]:checked').val() > "0") {
        $(".DependentsOver16_3Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsOver16_3Address_isRead").attr('disabled', true);
        $("#Head_DependentsOver16_3_Address").val('');
    }
});

/*B4 控除対象扶養親族(16歳以上) 対象者入力制御*/
$('#Head_DependentsOver16_4_notSubject').change(function () {
    if (document.getElementById("Head_DependentsOver16_4_notSubject").checked == true) {
        $(".DependentsOver16_4_isRead").attr('disabled', true);
        $(".DependentsOver16_4Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsOver16_4_isRead").removeAttr("disabled");
        if ($('input:radio[name="Head.DependentsOver16_4_ResidentType"]:checked').val() > "0") {
            $(".DependentsOver16_4Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsOver16_4Address_isRead").attr('disabled', true);
        }
    }
});
$('input:radio[name="Head.DependentsOver16_4_ResidentType"]').change(function () {
    if ($('input:radio[name="Head.DependentsOver16_4_ResidentType"]:checked').val() > "0") {
        $(".DependentsOver16_4Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsOver16_4Address_isRead").attr('disabled', true);
        $("#Head_DependentsOver16_4_Address").val('');
    }
});









//C 障碍者、寡婦、ひとり親又は勤労学生 入力制御
//障害者チェック
$('#Head_DependentsOther_Subject').change(function () {
    if (document.getElementById("Head_DependentsOther_Subject").checked == true) {
        $(".DependentsOther_isRead").removeAttr("disabled");
    } else {
        $(".DependentsOther_isRead").each(function (i, e) {
            $(e).prop("checked", false);
        });
        $(".DependentsOther_isRead").attr('disabled', true);
    }
    $('#Head_DependentsOther_GeneralHandicappedDependentsCheck').change();
    $('#Head_DependentsOther_SpecialHandicappedDependentsCheck').change();
    $('#Head_DependentsOther_LivingHandicappedDependentsCheck').change();
});


//一般の障害者チェック_扶養家族
$('#Head_DependentsOther_GeneralHandicappedDependentsCheck').change(function () {
    if (document.getElementById("Head_DependentsOther_GeneralHandicappedDependentsCheck").checked == true) {
        $("#Head_DependentsOther_GeneralHandicappedDependentsNumber").removeAttr("disabled");
    } else {
        $("#Head_DependentsOther_GeneralHandicappedDependentsNumber").val('');
        $("#Head_DependentsOther_GeneralHandicappedDependentsNumber").attr('disabled', true);
    }
});

//特別障害者_扶養親族
$('#Head_DependentsOther_SpecialHandicappedDependentsCheck').change(function () {
    if (document.getElementById("Head_DependentsOther_SpecialHandicappedDependentsCheck").checked == true) {
        $("#Head_DependentsOther_SpecialHandicappedDependentsNumber").removeAttr("disabled");
    } else {
        $("#Head_DependentsOther_SpecialHandicappedDependentsNumber").val('');
        $("#Head_DependentsOther_SpecialHandicappedDependentsNumber").attr('disabled', true);
    }
});

//同居特別障害者_扶養親族
$('#Head_DependentsOther_LivingHandicappedDependentsCheck').change(function () {
    if (document.getElementById("Head_DependentsOther_LivingHandicappedDependentsCheck").checked == true) {
        $("#Head_DependentsOther_LivingHandicappedDependentsNumber").removeAttr("disabled");
    } else {
        $("#Head_DependentsOther_LivingHandicappedDependentsNumber").val('');
        $("#Head_DependentsOther_LivingHandicappedDependentsNumber").attr('disabled', true);
    }
});

//寡婦、ひとり親制御
$('input:radio[name="Head.DependentsOther_WidowType"]').change(function () {
    if ($('input:radio[name="Head.DependentsOther_WidowType"]:checked').val() > "1") {
        $(".Widow_isRead").removeAttr("disabled");
    } else {
        $(".Widow_isRead").attr('disabled', true);
        $("#Head_DependentsOther_WidowReasonType").val('');
        $("#Head_DependentsOther_WidowOccurrenceDate").val('');
    }
});



/*1 16歳未満の扶養親族 対象者入力制御*/
$('#Head_DependentsUnder16_1_notSubject').change(function () {
    if (document.getElementById("Head_DependentsUnder16_1_notSubject").checked == true) {
        $(".DependentsUnder16_1_isRead").attr('disabled', true);
        $(".DependentsUnder16_1Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsUnder16_1_isRead").removeAttr("disabled");
        if (document.getElementById("Head_DependentsUnder16_1_AddressSameCheck").checked == true) {
            $(".DependentsUnder16_1Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsUnder16_1Address_isRead").attr('disabled', true);
        }
    }
});
$('#Head_DependentsUnder16_1_AddressSameCheck').change(function () {
    if (document.getElementById("Head_DependentsUnder16_1_AddressSameCheck").checked == true) {
        $(".DependentsUnder16_1Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsUnder16_1Address_isRead").attr('disabled', true);
        $("#Head_DependentsUnder16_1_Address").val('');
    }
});

/*2 16歳未満の扶養親族 対象者入力制御*/
$('#Head_DependentsUnder16_2_notSubject').change(function () {
    if (document.getElementById("Head_DependentsUnder16_2_notSubject").checked == true) {
        $(".DependentsUnder16_2_isRead").attr('disabled', true);
        $(".DependentsUnder16_2Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsUnder16_2_isRead").removeAttr("disabled");
        if (document.getElementById("Head_DependentsUnder16_2_AddressSameCheck").checked == true) {
            $(".DependentsUnder16_2Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsUnder16_2Address_isRead").attr('disabled', true);
        }
    }
});
$('#Head_DependentsUnder16_2_AddressSameCheck').change(function () {
    if (document.getElementById("Head_DependentsUnder16_2_AddressSameCheck").checked == true) {
        $(".DependentsUnder16_2Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsUnder16_2Address_isRead").attr('disabled', true);
        $("#Head_DependentsUnder16_2_Address").val('');
    }
});

/*3 16歳未満の扶養親族 対象者入力制御*/
$('#Head_DependentsUnder16_3_notSubject').change(function () {
    if (document.getElementById("Head_DependentsUnder16_3_notSubject").checked == true) {
        $(".DependentsUnder16_3_isRead").attr('disabled', true);
        $(".DependentsUnder16_3Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsUnder16_3_isRead").removeAttr("disabled");
        if (document.getElementById("Head_DependentsUnder16_3_AddressSameCheck").checked == true) {
            $(".DependentsUnder16_3Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsUnder16_3Address_isRead").attr('disabled', true);
        }
    }
});
$('#Head_DependentsUnder16_3_AddressSameCheck').change(function () {
    if (document.getElementById("Head_DependentsUnder16_3_AddressSameCheck").checked == true) {
        $(".DependentsUnder16_3Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsUnder16_3Address_isRead").attr('disabled', true);
        $("#Head_DependentsUnder16_3_Address").val('');
    }
});

/*4 16歳未満の扶養親族 対象者入力制御*/
$('#Head_DependentsUnder16_4_notSubject').change(function () {
    if (document.getElementById("Head_DependentsUnder16_4_notSubject").checked == true) {
        $(".DependentsUnder16_4_isRead").attr('disabled', true);
        $(".DependentsUnder16_4Address_isRead").attr('disabled', true);
    } else {
        $(".DependentsUnder16_4_isRead").removeAttr("disabled");
        if (document.getElementById("Head_DependentsUnder16_4_AddressSameCheck").checked == true) {
            $(".DependentsUnder16_4Address_isRead").removeAttr("disabled");
        } else {
            $(".DependentsUnder16_4Address_isRead").attr('disabled', true);
        }
    }
});
$('#Head_DependentsUnder16_4_AddressSameCheck').change(function () {
    if (document.getElementById("Head_DependentsUnder16_4_AddressSameCheck").checked == true) {
        $(".DependentsUnder16_4Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsUnder16_4Address_isRead").attr('disabled', true);
        $("#Head_DependentsUnder16_4_Address").val('');
    }
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
function checkDate(id, proc, cYear) {
    var strYear = $(id + 'Year').val();
    var strMonth = $(id + 'Month').val();
    var strDay = $(id + 'Day').val();
    var strMessage = "";
    var bolReturn = false;
    var cDate16 = String(cYear - 15) + "0101";
    var cDate19 = String(cYear - 18) + "0101";
    var cDate23 = String(cYear - 22) + "0101";

    $(id + "_mes").text("");

    if (strYear == "" && strMonth == "" && strDay == "") return true;
    if (!strYear || !strMonth || !strDay) {
        strMessage = "年月日が正しく入力されていません。";
        $(id + "_mes").text(strMessage);
        return false;
    } else if (!String(strYear).match(/^[0-9]{4}$/) || !String(strMonth).match(/^[0-9]{1,2}$/) || !String(strDay).match(/^[0-9]{1,2}$/)) {
        strMessage = "年月日が正しく入力されていません。";
        $(id + "_mes").text(strMessage);
        return false;
    } else {
        var dateObj = new Date(strYear, strMonth - 1, strDay),
            dateObjStr = dateObj.getFullYear() + '' + (dateObj.getMonth() + 1) + '' + dateObj.getDate(),
            checkDateStr = strYear + '' + strMonth + '' + strDay;
        if (dateObjStr != checkDateStr) {
            strMessage = "年月日が正しく入力されていません。";
            $(id + "_mes").text(strMessage);
            return false;
        } else {
            bolReturn = true;
        }
    }

    //年齢チェック
    if (strMonth.length<2){
        strMonth = "0" + strMonth;
    }
    if (strDay.length < 2) {
        strDay = "0" + strDay;
    }
    switch (proc) {
        case "1":
            var strDate = strYear + strMonth + strDay;
            if (strDate > cDate16) {
                strMessage = "基準日の年齢が16歳未満です。";
                $(id + "_mes").text(strMessage);
                return false;
            } else {
            }
            break;
        case "2":
            var strDate = strYear + strMonth + strDay;
            if (strDate <= cDate16) {
                strMessage = "基準日の年齢が16歳を超えてます。";
                $(id + "_mes").text(strMessage);
                return false;
            }
            break;
    }

    //指定箇所制御
    if (id == "#Head_DependentsOver16_1_Birthday") {
        if ((strDate <= cDate19) && (strDate > cDate23)) {
            $('input:radio[name="Head.DependentsOver16_1_SpecificType"]').val(["1"]);
        } else {
            $('input:radio[name="Head.DependentsOver16_1_SpecificType"]').val(["0"]);
        }
    }
    if (id == "#Head_DependentsOver16_2_Birthday") {
        if ((strDate <= cDate19) && (strDate > cDate23)) {
            $('input:radio[name="Head.DependentsOver16_2_SpecificType"]').val(["1"]);
        } else {
            $('input:radio[name="Head.DependentsOver16_2_SpecificType"]').val(["0"]);
        }
    }
    if (id == "#Head_DependentsOver16_3_Birthday") {
        if ((strDate <= cDate19) && (strDate > cDate23)) {
            $('input:radio[name="Head.DependentsOver16_3_SpecificType"]').val(["1"]);
        } else {
            $('input:radio[name="Head.DependentsOver16_3_SpecificType"]').val(["0"]);
        }
    }
    if (id == "#Head_DependentsOver16_4_Birthday") {
        if ((strDate <= cDate19) && (strDate > cDate23)) {
            $('input:radio[name="Head.DependentsOver16_4_SpecificType"]').val(["1"]);
        } else {
            $('input:radio[name="Head.DependentsOver16_4_SpecificType"]').val(["0"]);
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


//金額
function checkMoney(id, maxMoney) {
    var strVal = $(id).val();
    var strMessage = "";
    var bolReturn = false;
    strVal = strVal.replace(/[^0-9]+/i, '');    //数値のみ入力可能
    $(id).val(strVal);

    if (strVal == "") {
        bolReturn = true;
    } else if ($.isNumeric(strVal) == false) {
        strMessage = "数値で入力してください。";
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
    var aryCheckHuyou = "";

    ///本人情報チェック
    //チェック項目チェック
    if (document.getElementById("Head_MyNumberCheck").checked == false) {
        message = '必須項目が入力されていません。<br/>確認してください。';
    }
    //入力項目チェック
    aryCheckInput = ["Head_DepartmentNo"
                    , "Head_Name1"
                    , "Head_Name2"
                    , "Head_BirthdayYear"
                    , "Head_BirthdayMonth"
                    , "Head_BirthdayDay"
                    , "Head_HouseholdName1"
                    , "Head_HouseholdName2"
                    , "Head_RelationshipType"
                    , "Head_PostalCode_1"
                    , "Head_PostalCode_2"
                    , "Head_Address"
    ];
    $.each(aryCheckInput, function (index, value) {
        if (document.getElementById(value).value == "") {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }

    })

    //選択項目チェック
    aryCheckSelect = ["Head.SpouseCheck"
                    , "Head.SpouseCheck"
    ];
    $.each(aryCheckSelect, function (index, value) {
        if ($("input[name='" + value + "']:checked").length === 0) {
            message = '必須項目が入力されていません。<br/>確認してください。';
        }
    })

    //扶養データチェック
    aryCheckHuyou = ["Head_TaxWithholding"
                    , "Head_DependentsOver16_1"
                    , "Head_DependentsOver16_2"
                    , "Head_DependentsOver16_3"
                    , "Head_DependentsOver16_4"
                    , "Head_DependentsUnder16_1"
                    , "Head_DependentsUnder16_2"
                    , "Head_DependentsUnder16_3"
                    , "Head_DependentsUnder16_4"
    ];
    $.each(aryCheckHuyou, function (index, value) {
        if (document.getElementById(value + "_Name1").value != "" || document.getElementById(value + "_Name2").value != "") {
            if (document.getElementById(value + "_RelationshipType").value == ""
                || document.getElementById(value + "_BirthdayYear").value == "") {
                message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。<br/>(必須項目:続柄、生年月日)';
            }
        }
    })


    //画面のエラー項目全チェック
    $(".check-comment").each(function (i, e) {
        if ($(e).text().length > 1) {
            message = '入力に誤りがあります。<br/>確認してください。';
        }
    });


    return message
}


//保存直前にdisabled解除(値が送られないため)
function removeDisabled() {
    $('input').each(function (i, elem) {
        elem.disabled = false;
    });
    $('select').each(function (i, elem) {
        elem.disabled = false;
    });
}

/*
 * 途中保存ボタンクリック時
 */
$('#dmykeep').click(function () {
    message = '';
    //入力チェック
    message = checkAll();


    if (message != '') {
        showAlert('確認',message)
        return;
    }
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

    //ボタンクリック
    showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
});

/*
 * 承認キャンセルボタンクリック時
 */
$('#dmySignCancel').click(function () {
    //ボタンクリック
    showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
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
          $('#' + buttonid).trigger('click');
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

/*
 * 承認ボタンクリック時
 */
$(function () {
  $('#dmy111, #dmy112, #dmy113, #dmy114, #dmy211, #dmy212, #dmy213, #dmy214, #dmy221, #dmy223, #dmy224').click(function () {

    var msg = '承認画面　承認しますか？';
    switch (this.id) {
      case 'dmy111':
      case 'dmy211':
        msg = '登録確認画面　登録しますか？';
        break;
      default:
        break;
    }

    if ($('#Head_IsRireki').val() === 'True') {
        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
        //alert('履歴データの為、承認出来ません');
        showAlert('エラー', '履歴データの為、承認出来ません');
        // 2017-03-31 sbc-sagara upd end
      return;
    }

    //入力チェック(目標)
    if (this.id === 'dmy111' ||
        this.id === 'dmy112') {
        if (itemCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('目標は最低限３つ入力してください');
            showAlert('エラー', '目標は最低限３つ入力してください');
            // 2017-03-31 sbc-sagara upd end
          return;
        }
    }

    //2016-05-23 iwai-tamura upd str  -----
    //ウェイトチェック(本人)
    if (this.id === 'dmy111' ||
        this.id === 'dmy112') {
        if (selfWeightCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('ウェイトは合計１００%になるように入力してください');
            showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
            // 2017-03-31 sbc-sagara upd end
            return;
        }
    }
      //ウェイトチェック(上司)
    if (this.id === 'dmy112') {
        if (bossWeightCheck() == false) {
            // 2017-03-31 sbc-sagara upd str アラート表示形式統一
            //alert('ウェイトは合計１００%になるように入力してください');
            showAlert('エラー', 'ウェイトは合計１００%になるように入力してください');
            // 2017-03-31 sbc-sagara upd end
            return;
        }
    }
    //2016-05-23 iwai-tamura upd end  -----

    //目標面談
    if (this.id === 'dmy112') {
      if (objDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('目標設定面談実施日に正しい日付を入力してください');
          showAlert('エラー', '目標設定面談実施日に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }
    //達成面談
    if (this.id === 'dmy212') {
      if (achvDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('達成度評価面談実施日に正しい日付を入力してください');
          showAlert('エラー', '達成度評価面談実施日に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }
    //異動
    if (this.id === 'dmy211' ||
        this.id === 'dmy212') {
      if (trnsDateCheck() == false) {
          // 2017-03-31 sbc-sagara upd str アラート表示形式統一
          //alert('期中の人事異動に正しい日付を入力してください');
          showAlert('エラー', '期中の人事異動に正しい日付を入力してください');
          // 2017-03-31 sbc-sagara upd end
        return;
      }
    }

    //2017-04-11 iwai-tamura upd str  -----
    ////達成度・プロセスチェック(本人)
    //if (this.id === 'dmy211') {
    //    if (selfAchvCheck() == false) {
    //        showAlert('エラー', '入力されていない達成度があります。確認してください。');
    //        return;
    //    }
    //    if (selfProcessCheck() == false) {
    //        showAlert('エラー', '入力されていないプロセスがあります。確認してください。');
    //        return;
    //    }
    //}
    //  //達成度・プロセスチェック(上司)
    //if (this.id === 'dmy212') {
    //    if (bossAchvCheck() == false) {
    //        showAlert('エラー', '入力されていない達成度があります。確認してください。');
    //        return;
    //    }
    //    if (bossProcessCheck() == false) {
    //        showAlert('エラー', '入力されていないプロセスがあります。確認してください。');
    //        return;
    //    }
    //}
      //2017-04-11 iwai-tamura upd end  -----







    //2017-03-31 sbc-sagara add str
    //入力チェック(目標/達成水準、行数)
    if (!$('#Head_BusinessPlanning').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_BusinessPlanning').val()), 48, 16) == false) {
        message = '会社経営計画';
        showSaveMessageAndAlert(message, msg, this.value);
        return;
    } else if (!$('#Head_DepartmentPolicy').prop('readonly') && itemlineCheck(jQuery.trim($('#Head_DepartmentPolicy').val()), 48, 16) == false) {
        message = '所属部門';
        showSaveMessageAndAlert(message, msg, this.value);
        return;
    }
    // 目標項目の数繰り返し
    for (i = 0; $('#ObjList_' + i + '__ObjItem').length == 1; i++) {
        if (!$('#ObjList_' + i + '__ObjItem').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjItem').val()), 20, 12) == false) {
            message = '区分' + (i + 1) + 'の目標項目';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__AchvLevel').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__AchvLevel').val()), 20, 12) == false) {
            message = '区分' + (i + 1) + 'の達成水準・期限';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__ObjPolicys').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__ObjPolicys').val()), 30, 12) == false) {
            message = '区分' + (i + 1) + 'の目標達成のための施策・手段';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BestMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BestMetrics').val()), 15, 5) == false) {
            message = '区分' + (i + 1) + 'の大幅達成';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BetterMetrics').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BetterMetrics').val()), 15, 5) == false) {
            message = '区分' + (i + 1) + 'の達成';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__SelfComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__SelfComment').val()), 18, 11) == false) {
            message = '区分' + (i + 1) + 'の本人評価コメント';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        } else if (!$('#ObjList_' + i + '__BossComment').prop('readonly') && itemlineCheck(jQuery.trim($('#ObjList_' + i + '__BossComment').val()), 18, 11) == false) {
            message = '区分' + (i + 1) + 'の上司評価コメント';
            showSaveMessageAndAlert(message, msg, this.value);
            return;
        }
    }
    //2017-03-31 sbc-sagara add end


    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});

/*
 * 承認キャンセルボタンクリック時
 */
$(function () {
  $('#dmyc111, #dmyc112, #dmyc113, #dmyc114, #dmyc211, #dmyc212, #dmyc213, #dmyc214, #dmyc221, #dmyc223, #dmyc224').click(function () {

    var msg = '承認画面　承認をキャンセルしますか？';
    switch (this.id) {
      case 'dmyc111':
      case 'dmyc211':
        msg = '登録確認画面　登録をキャンセルしますか？';
        break;
      default:
        break;
    }

    if ($('#Head_IsRireki').val() === 'True') {
        // 2017-03-31 sbc-sagara upd str アラート表示形式統一
        //alert('履歴データの為、承認キャンセル出来ません');
        showAlert('エラー', '履歴データの為、承認キャンセル出来ません');
        // 2017-03-31 sbc-sagara upd end
      return;
    }

    //ボタンクリック
    showMessage('確認', msg, this.value, true);
  });
});



/*
 * 目標入力チェック
 */
function itemCheck() {
  for (i = 0; i < 3;i++){
    if (jQuery.trim($('#ObjList_' + i + '__ObjItem').val()) === '') {
      return false;
    }

  }
  return true;
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
    if (!(value === 100)){
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
        if(Number(jQuery.trim($('#ObjList_' + i + '__SelfWeight').val())) > 0){
            if(jQuery.trim($('#ObjList_' + i + '__SelfAchv').val()) === ""){
                return false;
            }
        }
    }
    return true;
}

function bossAchvCheck() {
    for (i = 0; i < 5; i++) {
        if(Number(jQuery.trim($('#ObjList_' + i + '__BossWeight').val())) > 0){
            if(jQuery.trim($('#ObjList_' + i + '__BossAchv').val()) === ""){
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
