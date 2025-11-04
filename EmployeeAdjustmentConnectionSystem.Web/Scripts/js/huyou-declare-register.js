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

    //2025-99-99 iwai-tamura upd-str ------
    // ★ 初期住所コードがあればここで逆引きして選択
    initFromAddressCode(); 
    document.getElementById('Pref').addEventListener('change', fillCity);
    document.getElementById('City').addEventListener('change', updateAddressCode);
    //2025-99-99 iwai-tamura upd-end ------
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



//2025-99-99 iwai-tamura upd-str ------

//function fillPref() {
//    const prefSel = document.getElementById('Pref');

//    // 先頭(placeholder)以外を消す
//    prefSel.length = 1;

//    const prefs = [...new Set(addressMaster.map(x => x.Prefecture))];
//    prefs.forEach(p => prefSel.add(new Option(p, p)));

//    // 既存選択があれば復元、なければ未選択のまま（placeholder表示）
//    const selectedPref = '@(Model.SelectedPref ?? "")';
//    if (selectedPref) {
//        prefSel.value = selectedPref;
//    } else {
//        prefSel.value = ""; // 未選択＝:invalid で薄表示
//    }
//}
// 都道府県の重複を排除してDDLに投入
function fillPref() {
    const prefSel = document.getElementById('Pref');
    prefSel.innerHTML = '';
    const prefs = [...new Set(addressMaster.map(x => x.Prefecture))];
    prefSel.add(new Option('都道府県', ''));
    prefs.forEach(p => prefSel.add(new Option(p, p)));
}

// 都道府県に応じて市区町村DDLを更新
function fillCity() {
    const pref = document.getElementById('Pref').value;
    const citySel = document.getElementById('City');
    citySel.innerHTML = '';
    const cities = addressMaster.filter(x => x.Prefecture === pref)
        .map(x => x.City);
    const uniq = [...new Set(cities)];
    citySel.add(new Option('市区町村', ''));
    uniq.forEach(c => citySel.add(new Option(c, c)));
    ////if ('@Model.SelectedCity') citySel.value = '@Model.SelectedCity';
    updateAddressCode(); // 初期反映
}

// 市区町村選択に応じて住所区分コードを表示
function updateAddressCode() {
    const pref = document.getElementById('Pref').value;
    const city = document.getElementById('City').value;
    const match = addressMaster.find(x => x.Prefecture === pref && x.City === city);
    document.getElementById('Head_AddressType').value = match ? match.Code : '';
}

// 住所区分コードから都道府県/市区町村を逆引きして選択する
function initFromAddressCode() {
    var initCode = $("#Head_AddressType").val();
    fillPref();
    fillCity();                 // これで City の候補が Pref に合わせて並ぶ
    if (!initCode) return;

    // マスタから該当1件を探す
    const hit = addressMaster.find(x => x.Code === initCode);
    if (!hit) return;

    const prefSel = document.getElementById('Pref');
    const citySel = document.getElementById('City');

    // 1) 都道府県リストを構築 → 都道府県を選択
    prefSel.value = hit.Prefecture;

    // 2) 市区町村リストを構築（選ばれた都道府県で絞り込み）→ 市区町村を選択
    fillCity();                 // これで City の候補が Pref に合わせて並ぶ
    citySel.value = hit.City;

    // 3) テキストボックスへコードを反映
    updateAddressCode();
}

// イベント
document.addEventListener('DOMContentLoaded', function () {

});
//2025-99-99 iwai-tamura upd-end ------

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

//2023-11-20 iwai-tamura upd str -----
// 収入金額の変更時に所得金額を自動計算
// 共通フィールド更新
function updateIncomeFields(earningsId, incomeId, otherIncomeId, estimateId) {

    // 収入から所得金額を計算する
    var earningsValue = parseFloat(document.getElementById(earningsId).value) || 0;
    var incomeValue = updateEarnings2Income(earningsValue);
    document.getElementById(incomeId).value = incomeValue;

    // 所得とその他所得を合計し見積額を算出
    var otherIncomeValue = parseFloat(document.getElementById(otherIncomeId).value) || 0;
    var estimateValue = incomeValue + otherIncomeValue;
    document.getElementById(estimateId).value = estimateValue;
}

// 収入から所得金額を計算するロジック
function updateEarnings2Income(earnings) {
    if (earnings <= 550999) {
        return 0;
    } else if (earnings <= 1618999) {
        return Math.floor(earnings - 550000);
    } else if (earnings <= 1619999) {
        return 1069000;
    } else if (earnings <= 1621999) {
        return 1070000;
    } else if (earnings <= 1623999) {
        return 1072000;
    } else if (earnings <= 1627999) {
        return 1074000;
    } else if (earnings <= 1799999) {
        return Math.floor(Math.floor(earnings / 4 / 1000) * 1000 * 2.4 + 100000);
    } else if (earnings <= 3599999) {
        return Math.floor(Math.floor(earnings / 4 / 1000) * 1000 * 2.8 - 80000);
    } else if (earnings <= 6599999) {
        return Math.floor(Math.floor(earnings / 4 / 1000) * 1000 * 3.2 - 440000);
    } else if (earnings <= 8499999) {
        return Math.floor(earnings * 0.9 - 1100000);
    } else {
        return earnings - 1950000;
    }
}


//源泉控除対象配偶者給与
//  所得
document.getElementById('Head_TaxWithholding_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_TaxWithholding_Earnings', 'Head_TaxWithholding_Earnings2Income', 'Head_TaxWithholding_OtherIncome', 'Head_TaxWithholding_Income');
    checkMoney('#Head_TaxWithholding_Income', 950000)
});
//  その他所得
document.getElementById('Head_TaxWithholding_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_TaxWithholding_Earnings', 'Head_TaxWithholding_Earnings2Income', 'Head_TaxWithholding_OtherIncome', 'Head_TaxWithholding_Income');
    checkMoney('#Head_TaxWithholding_Income', 950000)
});

//控除対象扶養親族給与1
//  所得
document.getElementById('Head_DependentsOver16_1_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_1_Earnings', 'Head_DependentsOver16_1_Earnings2Income', 'Head_DependentsOver16_1_OtherIncome', 'Head_DependentsOver16_1_Income');
    //2025-99-99 iwai-tamura upd-str ------
    //checkMoney('#Head_DependentsOver16_1_Income', 1230000)
    checkDependentsOver16('DependentsOver16_1')
    //checkMoney('#Head_DependentsOver16_1_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});
//  その他所得
document.getElementById('Head_DependentsOver16_1_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_1_Earnings', 'Head_DependentsOver16_1_Earnings2Income', 'Head_DependentsOver16_1_OtherIncome', 'Head_DependentsOver16_1_Income');
    //2025-99-99 iwai-tamura upd-str ------
    //checkMoney('#Head_DependentsOver16_1_Income', 1230000)
    checkDependentsOver16('DependentsOver16_1')
    //checkMoney('#Head_DependentsOver16_1_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});

//控除対象扶養親族給与2
//  所得
document.getElementById('Head_DependentsOver16_2_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_2_Earnings', 'Head_DependentsOver16_2_Earnings2Income', 'Head_DependentsOver16_2_OtherIncome', 'Head_DependentsOver16_2_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_2_Income', 1230000)
    checkDependentsOver16('DependentsOver16_2')
    //checkMoney('#Head_DependentsOver16_2_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});
//  その他所得
document.getElementById('Head_DependentsOver16_2_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_2_Earnings', 'Head_DependentsOver16_2_Earnings2Income', 'Head_DependentsOver16_2_OtherIncome', 'Head_DependentsOver16_2_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_2_Income', 1230000)
    checkDependentsOver16('DependentsOver16_2')
    //checkMoney('#Head_DependentsOver16_2_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});

//控除対象扶養親族給与3
//  所得
document.getElementById('Head_DependentsOver16_3_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_3_Earnings', 'Head_DependentsOver16_3_Earnings2Income', 'Head_DependentsOver16_3_OtherIncome', 'Head_DependentsOver16_3_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_3_Income', 1230000)
    checkDependentsOver16('DependentsOver16_3')
    //checkMoney('#Head_DependentsOver16_3_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});
//  その他所得
document.getElementById('Head_DependentsOver16_3_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_3_Earnings', 'Head_DependentsOver16_3_Earnings2Income', 'Head_DependentsOver16_3_OtherIncome', 'Head_DependentsOver16_3_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_3_Income', 1230000)
    checkDependentsOver16('DependentsOver16_3')
    //checkMoney('#Head_DependentsOver16_3_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});

//控除対象扶養親族給与4
//  所得
document.getElementById('Head_DependentsOver16_4_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_4_Earnings', 'Head_DependentsOver16_4_Earnings2Income', 'Head_DependentsOver16_4_OtherIncome', 'Head_DependentsOver16_4_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_4_Income', 1230000)
    checkDependentsOver16('DependentsOver16_4')
    //checkMoney('#Head_DependentsOver16_4_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});
//  その他所得
document.getElementById('Head_DependentsOver16_4_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsOver16_4_Earnings', 'Head_DependentsOver16_4_Earnings2Income', 'Head_DependentsOver16_4_OtherIncome', 'Head_DependentsOver16_4_Income');
    //2025-99-99 iwai-tamura upd-str ------
    checkMoney('#Head_DependentsOver16_4_Income', 1230000)
    checkDependentsOver16('DependentsOver16_4')
    //checkMoney('#Head_DependentsOver16_4_Income', 480000)
    //2025-99-99 iwai-tamura upd-end ------
});

//16歳未満の扶養親族給与1
//  所得
document.getElementById('Head_DependentsUnder16_1_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_1_Earnings', 'Head_DependentsUnder16_1_Earnings2Income', 'Head_DependentsUnder16_1_OtherIncome', 'Head_DependentsUnder16_1_Income');
    checkMoney('#Head_DependentsUnder16_1_Income', 480000)
});
//  その他所得
document.getElementById('Head_DependentsUnder16_1_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_1_Earnings', 'Head_DependentsUnder16_1_Earnings2Income', 'Head_DependentsUnder16_1_OtherIncome', 'Head_DependentsUnder16_1_Income');
    checkMoney('#Head_DependentsUnder16_1_Income', 480000)
});

//16歳未満の扶養親族給与2
//  所得
document.getElementById('Head_DependentsUnder16_2_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_2_Earnings', 'Head_DependentsUnder16_2_Earnings2Income', 'Head_DependentsUnder16_2_OtherIncome', 'Head_DependentsUnder16_2_Income');
    checkMoney('#Head_DependentsUnder16_2_Income', 480000)
});
//  その他所得
document.getElementById('Head_DependentsUnder16_2_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_2_Earnings', 'Head_DependentsUnder16_2_Earnings2Income', 'Head_DependentsUnder16_2_OtherIncome', 'Head_DependentsUnder16_2_Income');
    checkMoney('#Head_DependentsUnder16_2_Income', 480000)
});

//16歳未満の扶養親族給与3
//  所得
document.getElementById('Head_DependentsUnder16_3_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_3_Earnings', 'Head_DependentsUnder16_3_Earnings2Income', 'Head_DependentsUnder16_3_OtherIncome', 'Head_DependentsUnder16_3_Income');
    checkMoney('#Head_DependentsUnder16_3_Income', 480000)
});
//  その他所得
document.getElementById('Head_DependentsUnder16_3_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_3_Earnings', 'Head_DependentsUnder16_3_Earnings2Income', 'Head_DependentsUnder16_3_OtherIncome', 'Head_DependentsUnder16_3_Income');
    checkMoney('#Head_DependentsUnder16_3_Income', 480000)
});

//16歳未満の扶養親族給与4
//  所得
document.getElementById('Head_DependentsUnder16_4_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_4_Earnings', 'Head_DependentsUnder16_4_Earnings2Income', 'Head_DependentsUnder16_4_OtherIncome', 'Head_DependentsUnder16_4_Income');
    checkMoney('#Head_DependentsUnder16_4_Income', 480000)
});
//  その他所得
document.getElementById('Head_DependentsUnder16_4_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder16_4_Earnings', 'Head_DependentsUnder16_4_Earnings2Income', 'Head_DependentsUnder16_4_OtherIncome', 'Head_DependentsUnder16_4_Income');
    checkMoney('#Head_DependentsUnder16_4_Income', 480000)
});

//2023-11-20 iwai-tamura upd end -----


//2023-11-20 iwai-tamura upd str -----
//老人扶養チェック
function checkDependentsOver16(id) {
    var birthdayYearID = "#Head_" + id + "_BirthdayYear";
    var birthdayMonthID = "#Head_" + id + "_BirthdayMonth";
    var birthdayDayID = "#Head_" + id + "_BirthdayDay";
    var birthdayMesID = "#Head_" + id + "_Birthday_mes";

    var oldmanTypeName = "Head." + id + "_OldmanType";
    var oldmanTypeMesID = "#Head_" + id + "_OldmanType_mes";
    var specificTypeName = "Head." + id + "_SpecificType";
    var specificTypeMesID = "#Head_" + id + "_SpecificType_mes";
    var residentTypeName = "Head." + id + "_ResidentType";
    var residentTypeMesID = "#Head_" + id + "_ResidentType_mes";

    //2025-99-99 iwai-tamura upd-str ------
    var incomeID = "#Head_" + id + "_Income";
    var incomeMesID = "#Head_" + id + "_Income_mes";
    //2025-99-99 iwai-tamura upd-end ------

    var strYear = $(birthdayYearID).val();
    var strMonth = $(birthdayMonthID).val();
    var strDay = $(birthdayDayID).val();
    var cYear = $("#Head_SheetYear").val(); 
    var cDate16 = String(cYear - 15) + "0101";
    var cDate19 = String(cYear - 18) + "0101";
    var cDate23 = String(cYear - 22) + "0101";
    var cDate70 = String(cYear - 69) + "0101";
    //2025-99-99 iwai-tamura upd-str ------
    var cIncome = $(incomeID).val();
    //2025-99-99 iwai-tamura upd-end ------

    var strMessage = "";
    var bolReturn = false;

    $(birthdayMesID).text("");
    $(oldmanTypeMesID).text("");
    $(specificTypeMesID).text("");


    //2025-99-99 iwai-tamura upd-str ------
    //特定扶養親族区分は自動選択に変更
    $('input:radio[name="' + specificTypeName + '"]').prop("checked", false);
    //2025-99-99 iwai-tamura upd-end ------


    if (strYear == "" && strMonth == "" && strDay == "") return true;
    if (!strYear || !strMonth || !strDay) {
        strMessage = "年月日が正しく入力されていません。";
        $(birthdayMesID).text(strMessage);
        return false;
    } else if (!String(strYear).match(/^[0-9]{4}$/) || !String(strMonth).match(/^[0-9]{1,2}$/) || !String(strDay).match(/^[0-9]{1,2}$/)) {
        strMessage = "年月日が正しく入力されていません。";
        $(birthdayMesID).text(strMessage);
        return false;
    } else {
        var dateObj = new Date(strYear, strMonth - 1, strDay),
            dateObjStr = dateObj.getFullYear() + '' + (dateObj.getMonth() + 1) + '' + dateObj.getDate(),
            checkDateStr = strYear + '' + strMonth + '' + strDay;
        if (dateObjStr != checkDateStr) {
            strMessage = "年月日が正しく入力されていません。";
            $(birthdayMesID).text(strMessage);
            return false;
        } else {
            bolReturn = true;
        }
    }

    //年齢チェック
    if (strMonth.length < 2) {
        strMonth = "0" + strMonth;
    }
    if (strDay.length < 2) {
        strDay = "0" + strDay;
    }

    var strDate = strYear + strMonth + strDay;
    if (strDate > cDate16) {
        strMessage = "基準日の年齢が16歳未満です。";
        $(birthdayMesID).text(strMessage);
        return false;
    }

    //老人扶養区分チェック
    strMessage = "";
    if (strDate <= cDate70) {
        if ($('input:radio[name="' + oldmanTypeName + '"]:checked').val() == "0") {
            strMessage = "70歳以上は該当する区分を選択してください。";
        }
    }
    if (strDate > cDate70) {
        if ($('input:radio[name="' + oldmanTypeName + '"]:checked').val() != "0") {
            strMessage = "70歳未満は選択できません。";
        }
    }
    $(oldmanTypeMesID).text(strMessage);

    //特定扶養親族区分チェック
    //2025-99-99 iwai-tamura upd-str ------
    //特定扶養親族区分は自動選択に変更
    //金額と年齢にて判別
    strMessage = "";
    if (cIncome != "") {
        if (cIncome > 580000) {
            //所得の見積額が58万円超の場合、対象外
            strMessage = "58万円超は対象外です。";
        }
        $('input:radio[name="' + specificTypeName + '"][value="0"]').prop("checked", true);
        if ((strDate <= cDate19) && (strDate > cDate23)) {
            strMessage = "";    //特定親族の場合は上限が代わる。
            if (cIncome > 1000000) {
                //所得の見積額が100万円超の場合、対象外
                strMessage = "100万円超は対象外です。";
                $(incomeMesID).text(strMessage);
            }
            if (cIncome <= 580000) {
                //58万円以下の場合、特定扶養親族
                $('input:radio[name="' + specificTypeName + '"][value="1"]').prop("checked", true);
            } else if ((cIncome > 580000) && (cIncome <= 1230000)) {
                //58万円超123万円以下の場合、特定親族
                $('input:radio[name="' + specificTypeName + '"][value="2"]').prop("checked", true);
            }
        }
    }
    $(incomeMesID).text(strMessage);
    //strMessage = "";
    //if ((strDate <= cDate19) && (strDate > cDate23)) {
    //    if ($('input:radio[name="' + specificTypeName + '"]:checked').val() == "0") {
    //        strMessage = "19歳～23歳は特定扶養親族を選択してください。";
    //    }
    //} else {
    //    if ($('input:radio[name="' + specificTypeName + '"]:checked').val() != "0") {
    //        strMessage = "19歳～23歳以外は選択できません。";
    //    }
    //}
    //$(specificTypeMesID).text(strMessage);
    //2025-99-99 iwai-tamura upd-end ------

    strMessage = "";
    if ($('input:radio[name="' + oldmanTypeName + '"]:checked').val() == "1") {
        if ($('input:radio[name="' + residentTypeName + '"]:checked').val() != "0") {
            strMessage = "同居老親等の場合は、該当しないを選択してください。";
        }
    } else if ($('input:radio[name="' + oldmanTypeName + '"]:checked').val() == "2") {
        if (($('input:radio[name="' + residentTypeName + '"]:checked').val() != "1") && ($('input:radio[name="' + residentTypeName + '"]:checked').val() != "2")) {
            strMessage = "老人扶養親族でその他を選択している場合は、該当する区分を選択してください。";
        }
    } else {
    }


    $(residentTypeMesID).text(strMessage);

    return bolReturn;
}


//扶養者の入力必須チェック
//var checkRequiredFields = function () {
//    // 氏名のフィールドがいずれかに入力されているかどうかをチェック
//    var isNameEntered = document.getElementById('TaxWithholding_Name1').value.trim() !== '' || document.getElementById('TaxWithholding_Name2').value.trim() !== '';

//    // 必要なフィールドを選択
//    var requiredFields = document.querySelectorAll('.form-inline input, .form-inline select, .form-inline textarea');

//    // 氏名が入力されている場合、全てのフィールドにrequired属性を追加
//    requiredFields.forEach(function (field) {
//        if (isNameEntered) {
//            field.setAttribute('required', 'required');
//        } else {
//            field.removeAttribute('required');
//        }
//    });
//};

//// 氏名のフィールドにイベントリスナーを設定
//document.getElementById('TaxWithholding_Name1').addEventListener('change', checkRequiredFields);
//document.getElementById('TaxWithholding_Name2').addEventListener('change', checkRequiredFields);

//// 初期ロード時にチェック
//checkRequiredFields();
//2023-11-20 iwai-tamura upd end -----




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

    //2023-11-20 iwai-tamura upd str -----
    var convertedValue = zenkana2Hankana(hira2Kana($(this).val()));
    $(this).val(convertedValue);
    //2023-11-20 iwai-tamura upd end -----
    
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
        strMessage = maxMoney/10000 + "万円以上は対象外です。";
    } else {
        bolReturn = true;
    }
    $(id + "_mes").text(strMessage);
    return bolReturn;
}


//2024-11-19 iwai-tamura upd-str ------
function checkFamilyCount() {
    message = '';
    var bolCheckAddress = false;
    var bolCheckFamily = false;


    //本人住所変更チェック
    if (document.getElementById("Head_AddressBefore").value != document.getElementById("Head_Address").value) {
        bolCheckAddress = true;
    }

    //扶養人数の増減チェック
    var aryCheckHuyou = "";
    aryCheckHuyou = [
        { id: "TaxWithholding", name: "源泉控除対象配偶者" }
        , { id: "DependentsOver16_1", name: "控除対象扶養親族(16歳以上) 1" }
        , { id: "DependentsOver16_2", name: "控除対象扶養親族(16歳以上) 2" }
        , { id: "DependentsOver16_3", name: "控除対象扶養親族(16歳以上) 3" }
        , { id: "DependentsOver16_4", name: "控除対象扶養親族(16歳以上) 4" }
        , { id: "DependentsUnder16_1", name: "控除対象扶養親族(16歳未満) 1" }
        , { id: "DependentsUnder16_2", name: "控除対象扶養親族(16歳未満) 2" }
        , { id: "DependentsUnder16_3", name: "控除対象扶養親族(16歳未満) 3" }
        , { id: "DependentsUnder16_4", name: "控除対象扶養親族(16歳未満) 4" }
    ];
    let familyCount = 0;

    $.each(aryCheckHuyou, function (index, value) {
        // 氏名が入力されている場合、扶養人数を増やす
        if (document.getElementById("Head_" + value.id + "_Name1").value != "") {
            familyCount++;
        }
        // 対象外区分がチェックされている場合、扶養人数を減らす
        if (document.getElementById("Head_" + value.id + "_notSubject").checked == true) {
            familyCount--;
        }
    });
    //開いた時の扶養人数と比較
    let originalFamilyCount = parseInt(document.getElementById('Head_FamilyCount').value);
    if (familyCount !== originalFamilyCount) {
        bolCheckFamily = true;
    }


    if (bolCheckFamily && bolCheckAddress) {
        message = '※本人情報の住所と扶養人数に変更がありました。<br/>　グループウェアにおいて人事関係申請の提出も<br/>　お願いします。';
    } else if (bolCheckFamily) {
        message = '※扶養人数の変更がありました。<br/>　グループウェアにおいて人事関係申請の提出も<br/>　お願いします。';
    } else if (bolCheckAddress) {
        message = '※本人情報の住所に変更がありました。<br/>　グループウェアにおいて人事関係申請の提出も<br/>　お願いします。';
    }

    return message;
}
//2024-11-19 iwai-tamura upd-end ------


//一括チェック
function checkAll() {
    message = '';
    var aryCheckInput = "";
    var aryCheckSelect = "";
    var aryCheckHuyou = "";


    //2024-11-19 iwai-tamura upd-str ------
    ///本人情報チェック
    let missingFields = [];

    //個人番号相違チェック
    if (document.getElementById("Head_MyNumberCheck").checked == false) {
        missingFields.push("本人情報:提出済み相違チェック");
    }

    //入力項目チェック
    aryCheckInput = [
        { id: "Head_DepartmentNo", name: "本人情報：所属番号" },
        { id: "Head_Name1", name: "本人情報：氏名_氏" },
        { id: "Head_Name2", name: "本人情報：氏名_名" },
        { id: "Head_Kana1", name: "本人情報：ﾌﾘｶﾞﾅ_氏" },
        { id: "Head_Kana2", name: "本人情報：ﾌﾘｶﾞﾅ_名" },
        { id: "Head_BirthdayYear", name: "本人情報：生年月日_年" },
        { id: "Head_BirthdayMonth", name: "本人情報：生年月日_月" },
        { id: "Head_BirthdayDay", name: "本人情報：生年月日_日" },
        { id: "Head_HouseholdName1", name: "本人情報：世帯主_氏" },
        { id: "Head_HouseholdName2", name: "本人情報：世帯主_名" },
        { id: "Head_RelationshipType", name: "本人情報：世帯主_続柄" },
        //2025-99-99 iwai-tamura upd-str ------
        { id: "Head_AddressType", name: "本人情報：住所区分" },
        //2025-99-99 iwai-tamura upd-end ------
        { id: "Head_PostalCode_1", name: "本人情報：郵便番号1" },
        { id: "Head_PostalCode_2", name: "本人情報：郵便番号2" },
        { id: "Head_Address", name: "本人情報：住所又は居所" }
    ];

    $.each(aryCheckInput, function (index, item) {
        if (document.getElementById(item.id).value.trim() == "") {
            missingFields.push(item.name);
        }else if (item.id === "Head_PostalCode_1" && document.getElementById(item.id).value.length !== 3) {
            missingFields.push(item.name);
        }else if (item.id === "Head_PostalCode_2" && document.getElementById(item.id).value.length !== 4) {
            missingFields.push(item.name);
        }
    });


    //選択項目チェック
    aryCheckSelect = [{ id: "Head.SpouseCheck", name: "本人情報：配偶者の有無" }];
    $.each(aryCheckSelect, function (index, item) {
        if ($("input[name='" + item.id + "']:checked").length === 0) {
            missingFields.push(item.name);
        }
    })

    if (missingFields.length > 0) {
        message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
    }



    /////本人情報チェック
    ////チェック項目チェック
    //if (document.getElementById("Head_MyNumberCheck").checked == false) {
    //    message = '必須項目が入力されていません。<br/>確認してください。';
    //}
    //aryCheckInput = ["Head_DepartmentNo"
    //                , "Head_Name1"
    //                , "Head_Name2"
    //                , "Head_BirthdayYear"
    //                , "Head_BirthdayMonth"
    //                , "Head_BirthdayDay"
    //                , "Head_HouseholdName1"
    //                , "Head_HouseholdName2"
    //                , "Head_RelationshipType"
    //                , "Head_PostalCode_1"
    //                , "Head_PostalCode_2"
    //                , "Head_Address"
    //];
    //$.each(aryCheckInput, function (index, value) {
    //    if (document.getElementById(value).value == "") {
    //        message = '必須項目が入力されていません。<br/>確認してください。';
    //    }

    //})

    ////選択項目チェック
    //aryCheckSelect = ["Head.SpouseCheck"
    //                , "Head.SpouseCheck"
    //];
    //$.each(aryCheckSelect, function (index, value) {
    //    if ($("input[name='" + value + "']:checked").length === 0) {
    //    message = '必須項目が入力されていません。<br/>確認してください。';
    //    }
    //})
    //2024-11-19 iwai-tamura upd-end ------


    //2024-11-19 iwai-tamura upd-str ------
    //源泉控除対象配偶者データチェック
    aryCheckHuyou = [
        { id: "TaxWithholding", name: "源泉控除対象配偶者" }
    ];
    $.each(aryCheckHuyou, function (index, value) {
        if (document.getElementById("Head_" + value.id + "_notSubject").checked == false) {
            aryCheckInput = [
                { type: "txt", id: "RelationshipType", name: "続柄" },
                { type: "txt", id: "Name1", name: "氏名_氏" },
                { type: "txt", id: "Name2", name: "氏名_名" },
                { type: "txt", id: "Kana1", name: "ﾌﾘｶﾞﾅ_氏" },
                { type: "txt", id: "Kana2", name: "ﾌﾘｶﾞﾅ_名" },
                { type: "txt", id: "BirthdayYear", name: "生年月日_年" },
                { type: "txt", id: "BirthdayMonth", name: "生年月日_月" },
                { type: "txt", id: "BirthdayDay", name: "生年月日_日" },
                { type: "txt", id: "Earnings", name: "収入金額" },
                { type: "txt", id: "Earnings2Income", name: "所得金額" },
                { type: "chk", id: "ResidentType", name: "居住者区分" },
                { type: "Address", id: "Address", name: "住所又は居所" }
            ];
            if (document.getElementById("Head_" + value.id + "_Name1").value != "" || document.getElementById("Head_" + value.id + "_Name2").value != "") {
                $.each(aryCheckInput, function (index, item) {
                    switch (item.type) {
                        case "txt": 
                            if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;

                        case "chk":
                            if ($('input:radio[name="Head.' + value.id + '_' + item.id + '"]:checked').length === 0) {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;
                        case "Address": //住所または居所 居住者区分が該当しない場合はチェック不要
                            if ($('input:radio[name="Head.' + value.id + '_ResidentType"]:checked').val() != "0") {
                                if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                    if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                        missingFields.push(value.name + "：" + item.name);
                                    }
                                }
                            }
                            break;
                    } 
                })
                if (missingFields.length > 0) {
                    message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
                }
            }
        } else {
            aryCheckInput = [
                { type: "txt", id: "TransferDate", name: "異動年月日" },
                { type: "txt", id: "TransferComment", name: "事由" }
            ];
            $.each(aryCheckInput, function (index, item) {
                switch (item.type) {
                    case "txt":
                        if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                            missingFields.push(value.name + "：" + item.name);
                        }
                        break;
                }
            })
            if (missingFields.length > 0) {
                message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
            }
        }
    })


    //控除対象扶養親族(16歳以上)データチェック
    aryCheckHuyou = [
        { id: "DependentsOver16_1", name: "控除対象扶養親族(16歳以上) 1" }
        , { id: "DependentsOver16_2", name: "控除対象扶養親族(16歳以上) 2" }
        , { id: "DependentsOver16_3", name: "控除対象扶養親族(16歳以上) 3" }
        , { id: "DependentsOver16_4", name: "控除対象扶養親族(16歳以上) 4" }
    ];

    $.each(aryCheckHuyou, function (index, value) {
        if (document.getElementById("Head_" + value.id + "_notSubject").checked == false) {
            aryCheckInput = [
                { type: "txt", id: "RelationshipType", name: "続柄" }
                , { type: "txt", id: "Name1", name: "氏名_氏" }
                , { type: "txt", id: "Name2", name: "氏名_名" }
                , { type: "txt", id: "Kana1", name: "ﾌﾘｶﾞﾅ_氏" }
                , { type: "txt", id: "Kana2", name: "ﾌﾘｶﾞﾅ_名" }
                , { type: "txt", id: "BirthdayYear", name: "生年月日" }
                , { type: "chk", id: "OldmanType", name: "老人扶養親族" }
                , { type: "chk", id: "SpecificType", name: "特定扶養親族" }
                , { type: "txt", id: "Earnings", name: "収入金額" }
                , { type: "txt", id: "Earnings2Income", name: "所得金額" }
                , { type: "chk", id: "ResidentType", name: "居住者区分" }
                , { type: "Address", id: "Address", name: "住所又は居所" }
            ];
            if (document.getElementById("Head_" + value.id + "_Name1").value != "" || document.getElementById("Head_" + value.id + "_Name2").value != "") {
                $.each(aryCheckInput, function (index, item) {
                    switch (item.type) {
                        case "txt":
                            if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;

                        case "chk":
                            if ($('input:radio[name="Head.' + value.id + '_' + item.id + '"]:checked').length === 0) {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;
                        case "Address": //住所または居所 居住者区分が該当しない場合はチェック不要
                            if ($('input:radio[name="Head.' + value.id + '_ResidentType"]:checked').val() != "0") {
                                if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                    if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                        missingFields.push(value.name + "：" + item.name);
                                    }
                                }
                            }
                            break;
                    }
                })
                if (missingFields.length > 0) {
                    message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
                }
            }
        } else {
            aryCheckInput = [
                { type: "txt", id: "TransferDate", name: "異動年月日" },
                { type: "txt", id: "TransferComment", name: "事由" }
            ];
            $.each(aryCheckInput, function (index, item) {
                switch (item.type) {
                    case "txt":
                        if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                            missingFields.push(value.name + "：" + item.name);
                        }
                        break;
                }
            })
            if (missingFields.length > 0) {
                message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
            }
        }
    })


    //16歳未満の扶養親族データチェック
    aryCheckHuyou = [
        { id: "DependentsUnder16_1", name: "控除対象扶養親族(16歳未満) 1" }
        , { id: "DependentsUnder16_2", name: "控除対象扶養親族(16歳未満) 2" }
        , { id: "DependentsUnder16_3", name: "控除対象扶養親族(16歳未満) 3" }
        , { id: "DependentsUnder16_4", name: "控除対象扶養親族(16歳未満) 4" }
    ];

    $.each(aryCheckHuyou, function (index, value) {
        if (document.getElementById("Head_" + value.id + "_notSubject").checked == false) {
            aryCheckInput = [
                { type: "txt", id: "RelationshipType", name: "続柄" }
                , { type: "txt", id: "Name1", name: "氏名_氏" }
                , { type: "txt", id: "Name2", name: "氏名_名" }
                , { type: "txt", id: "Kana1", name: "ﾌﾘｶﾞﾅ_氏" }
                , { type: "txt", id: "Kana2", name: "ﾌﾘｶﾞﾅ_名" }
                , { type: "txt", id: "BirthdayYear", name: "生年月日" }
                , { type: "txt", id: "Earnings", name: "収入金額" }
                , { type: "txt", id: "Earnings2Income", name: "所得金額" }
                , { type: "Address", id: "Address", name: "住所又は居所" }
            ];
            if (document.getElementById("Head_" + value.id + "_Name1").value != "" || document.getElementById("Head_" + value.id + "_Name2").value != "") {
                $.each(aryCheckInput, function (index, item) {
                    switch (item.type) {
                        case "txt":
                            if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;

                        case "chk":
                            if ($('input:radio[name="Head.' + value.id + '_' + item.id + '"]:checked').length === 0) {
                                missingFields.push(value.name + "：" + item.name);
                            }
                            break;
                        case "Address": //住所または居所 居住者区分が該当しない場合はチェック不要
                            if ($('input:radio[name="Head.' + value.id + '_AddressSameCheck"]:checked').val() == true) {
                                if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                                    missingFields.push(value.name + "：" + item.name);
                                }
                            }
                            break;
                    }
                })
                if (missingFields.length > 0) {
                    message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
                }
            }
        } else {
            aryCheckInput = [
                { type: "txt", id: "TransferDate", name: "異動年月日" },
                { type: "txt", id: "TransferComment", name: "事由" }
            ];
            $.each(aryCheckInput, function (index, item) {
                switch (item.type) {
                    case "txt":
                        if (document.getElementById("Head_" + value.id + "_" + item.id).value.trim() == "") {
                            missingFields.push(value.name + "：" + item.name);
                        }
                        break;
                }
            })
            if (missingFields.length > 0) {
                message = '以下の必須項目が入力されていません。<br/>' + missingFields.join('<br/>');
            }
        }
    })

    ////源泉控除対象配偶者データチェック
    //aryCheckHuyou = ["TaxWithholding"];
    //$.each(aryCheckHuyou, function (index, value) {
    //    if (document.getElementById("Head_" + value + "_notSubject").checked == false) {
    //        if (document.getElementById("Head_" + value + "_Name1").value != "" || document.getElementById("Head_" + value + "_Name2").value != "") {
    //            if (document.getElementById("Head_" + value + "_RelationshipType").value == ""
    //                || document.getElementById("Head_" + value + "_Name1").value == ""
    //                || document.getElementById("Head_" + value + "_Name2").value == ""
    //                || document.getElementById("Head_" + value + "_Kana1").value == ""
    //                || document.getElementById("Head_" + value + "_Kana2").value == ""
    //                || document.getElementById("Head_" + value + "_BirthdayYear").value == ""
    //                || document.getElementById("Head_" + value + "_Earnings").value == ""
    //                || document.getElementById("Head_" + value + "_Earnings2Income").value == ""
    //                || $('input:radio[name="Head.' + value + '_ResidentType"]:checked').val() == ""
    //                || (document.getElementById("Head_" + value + "_Address").value == "" && $('input:radio[name="Head.' + value + '_ResidentType"]:checked').val() != "0")
    //            ) {
    //                message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //            }
    //        }
    //    } else {
    //        if (document.getElementById("Head_" + value + "_TransferDate").value == ""
    //            || document.getElementById("Head_" + value + "_TransferComment").value == ""
    //        ) {
    //            message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //        }
    //    }
    //})
    ////控除対象扶養親族(16歳以上)データチェック
    //aryCheckHuyou = ["DependentsOver16_1"
    //    , "DependentsOver16_2"
    //    , "DependentsOver16_3"
    //    , "DependentsOver16_4"
    //];
    //$.each(aryCheckHuyou, function (index, value) {
    //    if (document.getElementById("Head_" + value + "_notSubject").checked == false) {
    //        if (document.getElementById("Head_" + value + "_Name1").value != "" || document.getElementById("Head_" + value + "_Name2").value != "") {
    //            if (document.getElementById("Head_" + value + "_RelationshipType").value == ""
    //                || document.getElementById("Head_" + value + "_Name1").value == ""
    //                || document.getElementById("Head_" + value + "_Name2").value == ""
    //                || document.getElementById("Head_" + value + "_Kana1").value == ""
    //                || document.getElementById("Head_" + value + "_Kana2").value == ""
    //                || document.getElementById("Head_" + value + "_BirthdayYear").value == ""
    //                || $('input:radio[name="Head.' + value + '_OldmanType"]:checked').val() == ""
    //                || $('input:radio[name="Head.' + value + '_SpecificType"]:checked').val() == ""
    //                || document.getElementById("Head_" + value + "_Earnings").value == ""
    //                || document.getElementById("Head_" + value + "_Earnings2Income").value == ""
    //                || $('input:radio[name="Head.' + value + '_ResidentType"]:checked').val() == ""
    //                || (document.getElementById("Head_" + value + "_Address").value == "" && $('input:radio[name="Head.' + value + '_ResidentType"]:checked').val() != "0")
    //            ) {
    //                message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //            }
    //        }
    //    } else {
    //        if (document.getElementById("Head_" + value + "_TransferDate").value == ""
    //            || document.getElementById("Head_" + value + "_TransferComment").value == ""
    //        ) {
    //            message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //        }
    //    }
    //})

    ////16歳未満の扶養親族データチェック
    //aryCheckHuyou = ["DependentsUnder16_1"
    //    , "DependentsUnder16_2"
    //    , "DependentsUnder16_3"
    //    , "DependentsUnder16_4"
    //];
    //$.each(aryCheckHuyou, function (index, value) {
    //    if (document.getElementById("Head_" + value + "_notSubject").checked == false) {
    //        if (document.getElementById("Head_" + value + "_Name1").value != "" || document.getElementById("Head_" + value + "_Name2").value != "") {
    //            if (document.getElementById("Head_" + value + "_RelationshipType").value == ""
    //                || document.getElementById("Head_" + value + "_BirthdayYear").value == ""
    //                || document.getElementById("Head_" + value + "_Name1").value == ""
    //                || document.getElementById("Head_" + value + "_Name2").value == ""
    //                || document.getElementById("Head_" + value + "_Kana1").value == ""
    //                || document.getElementById("Head_" + value + "_Kana2").value == ""
    //                || document.getElementById("Head_" + value + "_Earnings").value == ""
    //                || document.getElementById("Head_" + value + "_Earnings2Income").value == ""
    //                || (document.getElementById("Head_" + value + "_Address").value == "" && document.getElementById("Head_" + value + "_AddressSameCheck").checked == true)
    //            ) {
    //                message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //            }
    //        }
    //    } else {
    //        if (document.getElementById("Head_" + value + "_TransferDate").value == ""
    //            || document.getElementById("Head_" + value + "_TransferComment").value == ""
    //        ) {
    //            message = '扶養者を入力する際の必須項目が入力されていません。<br/>確認してください。';
    //        }
    //    }
    //})

    //2024-11-19 iwai-tamura upd-end ------



    //画面のエラー項目全チェック
    //2024-11-19 iwai-tamura upd-str ------
    let errorMessages = [];

    $(".check-comment").each(function (i, e) {
        let errorText = $(e).text().trim();

        if (errorText.length > 0) {
            // エラーメッセージが存在する場合
            let fieldName = $(e).data("fieldname"); // data-fieldname属性から項目名を取得

            if (fieldName) {
                errorMessages.push(`${fieldName}`);
            } else {
                errorMessages.push(errorText);
            }
        }
    });

    if (errorMessages.length > 0) {
        message = '以下の入力項目に誤りがあります。<br/>' + errorMessages.join('<br/>');
    }

    //$(".check-comment").each(function (i, e) {
    //    if ($(e).text().length > 1) {
    //        message = '入力に誤りがあります。<br/>確認してください。';
    //    }
    //});
    //2024-11-19 iwai-tamura upd-end ------

    return message;
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

    //2024-11-19 iwai-tamura upd-str ------
    //扶養人数チェック
    message = '';
    message = checkFamilyCount();
    let additionalMessage = message ? '<br><br>' + message : '';
    //2024-11-19 iwai-tamura upd-end ------

    //ボタンクリック
    //2024-11-19 iwai-tamura upd-str ------
    showMessageEx('途中保存確認', '保存しますか？' + additionalMessage, 'keepbutton', true);
    //showMessageEx('途中保存確認', '保存しますか？', 'keepbutton', true);
    //2024-11-19 iwai-tamura upd-end ------
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

    //2024-11-19 iwai-tamura upd-str ------
    //扶養人数チェック
    message = '';
    message = checkFamilyCount();
    let additionalMessage = message ? '<br><br>' + message : '';
    //2024-11-19 iwai-tamura upd-end ------

    //ボタンクリック
    //2023-11-20 iwai-tamura upd str -----
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
	    //2023-12-15 iwai-tamura upd str -----
        if ($('#Head_DecisionType').val() <= '5') {
            //2024-11-19 iwai-tamura upd-str ------
            showMessageEx('確定確認', '確定しますか？' + additionalMessage, 'savebutton', true);
            //showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
            //2024-11-19 iwai-tamura upd-end ------
        } else {
            //2024-11-19 iwai-tamura upd-str ------
            showMessageEx('修正確認', '修正しますか？ <br><br> ※既に連携済みデータの為、連携先システムの修正も同様に行ってください。' + additionalMessage, 'savebutton', true);
            //showMessageEx('修正確認', '修正しますか？ <br><br> ※既に連携済みデータの為、連携先システムの修正も同様に行ってください。', 'savebutton', true);
            //2024-11-19 iwai-tamura upd-end ------
        }
        //showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
	    //2023-12-15 iwai-tamura upd end -----
    } else {
        //2024-11-19 iwai-tamura upd-str ------
        showMessageEx('提出確認', '提出しますか？' + additionalMessage, 'savebutton', true);
        //showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
        //2024-11-19 iwai-tamura upd-end ------
    }
    //showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
    //2023-11-20 iwai-tamura upd end -----
});

/*
 * 承認キャンセルボタンクリック時
 */
$('#dmySignCancel').click(function () {
    //ボタンクリック
    //2023-11-20 iwai-tamura upd str -----
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        showMessageEx('取消確認', '確定状態を取消しますか？', 'signcancel', true);
    } else {
        showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
    }
    //showMessageEx('取消確認', '提出状態を取消しますか？', 'signcancel', true);
    //2023-11-20 iwai-tamura upd end -----
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
