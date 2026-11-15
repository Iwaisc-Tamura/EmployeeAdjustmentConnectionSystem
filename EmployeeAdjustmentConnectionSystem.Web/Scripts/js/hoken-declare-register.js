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
    //2023-11-20 iwai-tamura upd str -----
    $('#Head_LifeInsurance_1_HostDataFlg').change();
    $('#Head_LifeInsurance_2_HostDataFlg').change();
    $('#Head_LifeInsurance_3_HostDataFlg').change();
    $('#Head_LifeInsurance_4_HostDataFlg').change();
    $('#Head_LifeInsurance_5_HostDataFlg').change();
    $('#Head_LifeInsurance_6_HostDataFlg').change();
    $('#Head_LifeInsurance_7_HostDataFlg').change();
    $('#Head_LifeInsurance_8_HostDataFlg').change();
    $('#Head_MedicalInsurance_1_HostDataFlg').change();
    $('#Head_MedicalInsurance_2_HostDataFlg').change();
    $('#Head_MedicalInsurance_3_HostDataFlg').change();
    $('#Head_MedicalInsurance_4_HostDataFlg').change();
    $('#Head_MedicalInsurance_5_HostDataFlg').change();
    $('#Head_MedicalInsurance_6_HostDataFlg').change();
    $('#Head_PensionInsurance_1_HostDataFlg').change();
    $('#Head_PensionInsurance_2_HostDataFlg').change();
    $('#Head_PensionInsurance_3_HostDataFlg').change();
    $('#Head_PensionInsurance_4_HostDataFlg').change();
    $('#Head_QuakeInsurance_1_HostDataFlg').change();
    $('#Head_QuakeInsurance_2_HostDataFlg').change();
    $('#Head_QuakeInsurance_3_HostDataFlg').change();
    $('#Head_QuakeInsurance_4_HostDataFlg').change();
    //2023-11-20 iwai-tamura upd end -----

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
            //2024-11-19 iwai-tamura upd str -----
            $(".btn-clear").attr('disabled', true);
            //2024-11-19 iwai-tamura upd end -----
            $("button.input-control").remove();

            break;

        default:
            $(".form-control").attr('disabled', true);
            $(".input-control").attr('disabled', true);
            $(".btn-calc").attr('disabled', true);
            //2024-11-19 iwai-tamura upd str -----
            $(".btn-clear").attr('disabled', true);
            //2024-11-19 iwai-tamura upd end -----
            $("button.input-control").remove();

            break;
    }
});



//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});



//2026-99-99 iwai-miki add str -----
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
    if (earnings <= 740999) {
        return 0;
    } else if (earnings <= 2190999) {
        return Math.floor(earnings - 740000);
    } else if (earnings <= 2192999) {
        return 1451000;
    } else if (earnings <= 2195999) {
        return 1453000;
    } else if (earnings <= 2199999) {
        return 1456000;
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

//2023-11-20 iwai-tamura upd str -----
///*HostData判定 入力制御*///
$('#Head_LifeInsurance_1_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_1_HostDataFlg").checked == true) {
        $(".LifeInsurance_1_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_1_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_1_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_1_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_2_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_2_HostDataFlg").checked == true) {
        $(".LifeInsurance_2_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_2_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_2_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_2_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_3_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_3_HostDataFlg").checked == true) {
        $(".LifeInsurance_3_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_3_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_3_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_3_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_4_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_4_HostDataFlg").checked == true) {
        $(".LifeInsurance_4_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_4_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_4_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_4_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_5_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_5_HostDataFlg").checked == true) {
        $(".LifeInsurance_5_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_5_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_5_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_5_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_6_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_6_HostDataFlg").checked == true) {
        $(".LifeInsurance_6_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_6_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_6_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_6_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_7_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_7_HostDataFlg").checked == true) {
        $(".LifeInsurance_7_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_7_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_7_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_7_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_LifeInsurance_8_HostDataFlg').change(function () {
    if (document.getElementById("Head_LifeInsurance_8_HostDataFlg").checked == true) {
        $(".LifeInsurance_8_isRead").attr('disabled', true);
    } else {
        $(".LifeInsurance_8_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.LifeInsurance_8_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_LifeInsurance_8_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_1_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_1_HostDataFlg").checked == true) {
        $(".MedicalInsurance_1_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_1_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_1_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_1_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_2_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_2_HostDataFlg").checked == true) {
        $(".MedicalInsurance_2_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_2_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_2_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_2_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_3_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_3_HostDataFlg").checked == true) {
        $(".MedicalInsurance_3_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_3_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_3_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_3_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_4_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_4_HostDataFlg").checked == true) {
        $(".MedicalInsurance_4_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_4_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_4_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_4_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_5_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_5_HostDataFlg").checked == true) {
        $(".MedicalInsurance_5_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_5_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_5_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_5_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_MedicalInsurance_6_HostDataFlg').change(function () {
    if (document.getElementById("Head_MedicalInsurance_6_HostDataFlg").checked == true) {
        $(".MedicalInsurance_6_isRead").attr('disabled', true);
    } else {
        $(".MedicalInsurance_6_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.MedicalInsurance_6_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_MedicalInsurance_6_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});


$('#Head_PensionInsurance_1_HostDataFlg').change(function () {
    if (document.getElementById("Head_PensionInsurance_1_HostDataFlg").checked == true) {
        $(".PensionInsurance_1_isRead").attr('disabled', true);
    } else {
        $(".PensionInsurance_1_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.PensionInsurance_1_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_PensionInsurance_1_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_PensionInsurance_2_HostDataFlg').change(function () {
    if (document.getElementById("Head_PensionInsurance_2_HostDataFlg").checked == true) {
        $(".PensionInsurance_2_isRead").attr('disabled', true);
    } else {
        $(".PensionInsurance_2_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.PensionInsurance_2_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_PensionInsurance_2_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_PensionInsurance_3_HostDataFlg').change(function () {
    if (document.getElementById("Head_PensionInsurance_3_HostDataFlg").checked == true) {
        $(".PensionInsurance_3_isRead").attr('disabled', true);
    } else {
        $(".PensionInsurance_3_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.PensionInsurance_3_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_PensionInsurance_3_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_PensionInsurance_4_HostDataFlg').change(function () {
    if (document.getElementById("Head_PensionInsurance_4_HostDataFlg").checked == true) {
        $(".PensionInsurance_4_isRead").attr('disabled', true);
    } else {
        $(".PensionInsurance_4_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.PensionInsurance_4_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_PensionInsurance_4_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_QuakeInsurance_1_HostDataFlg').change(function () {
    if (document.getElementById("Head_QuakeInsurance_1_HostDataFlg").checked == true) {
        $(".QuakeInsurance_1_isRead").attr('disabled', true);
    } else {
        $(".QuakeInsurance_1_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.QuakeInsurance_1_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_QuakeInsurance_1_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_QuakeInsurance_2_HostDataFlg').change(function () {
    if (document.getElementById("Head_QuakeInsurance_2_HostDataFlg").checked == true) {
        $(".QuakeInsurance_2_isRead").attr('disabled', true);
    } else {
        $(".QuakeInsurance_2_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.QuakeInsurance_2_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_QuakeInsurance_2_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_QuakeInsurance_3_HostDataFlg').change(function () {
    if (document.getElementById("Head_QuakeInsurance_3_HostDataFlg").checked == true) {
        $(".QuakeInsurance_3_isRead").attr('disabled', true);
    } else {
        $(".QuakeInsurance_3_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.QuakeInsurance_3_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_QuakeInsurance_3_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});

$('#Head_QuakeInsurance_4_HostDataFlg').change(function () {
    if (document.getElementById("Head_QuakeInsurance_4_HostDataFlg").checked == true) {
        $(".QuakeInsurance_4_isRead").attr('disabled', true);
    } else {
        $(".QuakeInsurance_4_isRead").removeAttr("disabled");
    }
});

// 各要素に対してイベントリスナーを追加します。
document.querySelectorAll('.QuakeInsurance_4_isRead').forEach(function (input) {
    input.addEventListener('change', function () {
        var hostDataFlagElement = document.getElementById('Head_QuakeInsurance_4_HostDataFlg');
        if (hostDataFlagElement) {
            hostDataFlagElement.disabled = true;
        }
    });
});
//2023-11-20 iwai-tamura upd end -----


//2026-99-99 iwai-miki add str -----
//扶養控除申告書_配偶者データ取得ボタン
//document.getElementById('btn-spouseDeduction-get1').addEventListener('click', function () {

//    var strMessage = ""
//    var bolReturn = false
//    if (typeof varArt === 'undefined') varArt = true;

//    var relationshipTypeID = "#Head_SpecificRelativeSpecialDeduction" + id + "_RelationshipType";
//    var earningsID = "#Head_SpecificRelativeSpecialDeduction" + id + "_Earnings";
//    var incomeID = "#Head_SpecificRelativeSpecialDeduction" + id + "_Income";
//    var otherIncomeID = "#Head_SpecificRelativeSpecialDeduction" + id + "_OtherIncome";

//    var strRelationshipType = $(relationshipTypeID).val();
//    var intEarnings;
//    var intIncome;
//    var intOtherIncome;





//    DependentsUnder23_Name1
//    DependentsUnder23_Name2

//    DependentsUnder23_Kana1
//    DependentsUnder23_Kana2

//    DependentsUnder23_BirthdayYear
//    DependentsUnder23_BirthdayMonth
//    DependentsUnder23_BirthdayDay

//    DependentsUnder23_AddressSameCheck
//    DependentsUnder23_Address

//    DependentsUnder23_Earnings
//    DependentsUnder23_Earnings2Income
//    DependentsUnder23_OtherIncome
//    DependentsUnder23_Income







//    RefDependentFamilyMemberUnder23YearsOld01_RelationshipType = row["23歳未満扶養親族01_続柄"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Name01 = row["23歳未満扶養親族01_氏名_姓"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Name02 = row["23歳未満扶養親族01_氏名_名"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Kana01 = row["23歳未満扶養親族01_Kana_姓"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Kana02 = row["23歳未満扶養親族01_Kana_名"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Birthday = row["23歳未満扶養親族01_生年月日"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_BirthdayYear = row["23歳未満扶養親族01_生年月日"].ToString() == "" ? "" : row["23歳未満扶養親族01_生年月日"].ToString().Substring(0, 4),
//        RefDependentFamilyMemberUnder23YearsOld01_BirthdayMonth = row["23歳未満扶養親族01_生年月日"].ToString() == "" ? "" : row["23歳未満扶養親族01_生年月日"].ToString().Substring(4, 2).TrimStart(new Char[] { '0' }),
//        RefDependentFamilyMemberUnder23YearsOld01_BirthdayDay = row["23歳未満扶養親族01_生年月日"].ToString() == "" ? "" : row["23歳未満扶養親族01_生年月日"].ToString().Substring(6, 2).TrimStart(new Char[] { '0' }),
//        RefDependentFamilyMemberUnder23YearsOld01_AddressSameCheck = row["23歳未満扶養親族01_別居"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Address = row["23歳未満扶養親族01_住所"].ToString(),
//        RefDependentFamilyMemberUnder23YearsOld01_Earnings = setMoney(row["23歳未満扶養親族01_給与所得_収入金額"].ToString()),
//        RefDependentFamilyMemberUnder23YearsOld01_Income = setMoney(row["23歳未満扶養親族01_給与所得_所得金額"].ToString()),
//        RefDependentFamilyMemberUnder23YearsOld01_OtherIncome = setMoney(row["23歳未満扶養親族01_他_所得金額"].ToString()),
//        RefDependentFamilyMemberUnder23YearsOld01_TotalEarnings = setMoney(row["23歳未満扶養親族01_所得見積額"].ToString()),









    ////続柄チェック
    //if (strRelationshipType == "") {
    //    strMessage = "続柄が選択されていないので取得できません。";
    //    if (varArt) { alert(strMessage) };
    //    return true
    //}

    //if ($("#Head_RefSpecificDependent01_RelationshipType").val() == strRelationshipType) {
    //    intEarnings = $("#Head_RefSpecificDependent01_Earnings").val();
    //    intOtherIncome = $("#Head_RefSpecificDependent01_OtherIncome").val();

    //} else if ($("#Head_RefSpecificDependent02_RelationshipType").val() == strRelationshipType) {
    //    intEarnings = $("#Head_RefSpecificDependent02_Earnings").val();
    //    intOtherIncome = $("#Head_RefSpecificDependent02_OtherIncome").val();

    //} else if ($("#Head_RefSpecificDependent03_RelationshipType").val() == strRelationshipType) {
    //    intEarnings = $("#Head_RefSpecificDependent03_Earnings").val();
    //    intOtherIncome = $("#Head_RefSpecificDependent03_OtherIncome").val();

    //} else if ($("#Head_RefSpecificDependent04_RelationshipType").val() == strRelationshipType) {
    //    intEarnings = $("#Head_RefSpecificDependent04_Earnings").val();
    //    intOtherIncome = $("#Head_RefSpecificDependent04_OtherIncome").val();

    //} else {
    //    strMessage = "取得できる扶養控除申告書の入力がありません。";
    //    if (varArt) { alert(strMessage) };
    //    return true
    //}
    //$(earningsID).val(intEarnings);
    //$(otherIncomeID).val(intOtherIncome);
    //$(earningsID).trigger('change');

    //bolReturn = true;
    //return bolReturn;








    //var aaa = $('#Head_SpouseDeduction_Huyou_Income').val();

    //$('#Head.DependentsUnder23_Name1').val($('#Head.DependentsOver16_1_Name1').val());
    //$('#Head.DependentsUnder23_Name2').val($('#Head.DependentsOver16_1_Name2').val());
    //$('#Head.DependentsUnder23_Kana1').val($('#Head.DependentsOver16_1_Kana1').val());
    //$('#Head.DependentsUnder23_Kana2').val($('#Head.DependentsOver16_1_Kana2').val());


    //$('#Head_SpouseDeduction_OtherIncome').val($('#Head_SpouseDeduction_Huyou_OtherIncome').val());

//});
//2026-99-99 iwai-miki add end -----


//計算ボタン
function calcLifeInsurance() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""
    var id2 = ""
    //2026-99-99 iwai-miki add str -----
    var Under23Judgment = "0"
    //2026-99-99 iwai-miki add end -----

    //2026-99-99 iwai-miki add str -----
    var Under23Name = "#Head_DependentsUnder23_Name1";
    var Under23birthdayID = "#Head_DependentsUnder23_Birthday";
    var Under23birthdayYearID = "#Head_DependentsUnder23_BirthdayYear";
    var Under23birthdayMonthID = "#Head_DependentsUnder23_BirthdayMonth";
    var Under23birthdayDayID = "#Head_DependentsUnder23_BirthdayDay";

    if ($(Under23Name).val() != "") {
        //年齢チェック
        var cYear = $("#Head_SheetYear").val();
        var cDate23 = String(cYear - 22) + "0101";
        var strYear = $(Under23birthdayYearID).val();
        var strMonth = $(Under23birthdayMonthID).val();
        var strDay = $(Under23birthdayDayID).val();
        if (strMonth.length < 2) {
            strMonth = "0" + strMonth;
        }
        if (strDay.length < 2) {
            strDay = "0" + strDay;
        }
        if (!checkDate(Under23birthdayID, '0', '')) {
            strMessage = "年齢23歳未満の扶養親族の生年月日が不正です。確認してください。";
            alert(strMessage);

            return bolReturn;
        }

        if (strYear == "") {
            strMessage = "年齢23歳未満の扶養親族の生年月日が入力されていないので計算できません。確認してください。";
            if (varArt) { alert(strMessage) };
            return true;
        }

        var strDate = strYear + strMonth + strDay;
        if (strDate <= cDate23) {
            strMessage = "年齢23歳未満の扶養親族の基準日の年齢が23歳を超えてます。";
            alert(strMessage);
            return bolReturn;
        }




        Under23Judgment = "1";
        
    }
    //2026-99-99 iwai-miki add end -----

    $("#Head_LifeInsurance_NewTotalAmount").val(null);
    $("#Head_LifeInsurance_OldTotalAmount").val(null);
    $("#Head_LifeInsurance_Calc1").val(null);
    //2026-99-99 iwai-miki add str -----
    $("#Head_LifeInsurance_Calc3").val(null);
    //2026-99-99 iwai-miki add end -----
    $("#Head_LifeInsurance_Calc2").val(null);
    $("#Head_LifeInsurance_TotalAmount").val(null);
    //2026-99-99 iwai-miki add str -----
    $("#Head_LifeInsurance_TotalAmount2").val(null);
    //2026-99-99 iwai-miki add end -----
    $("#Head_LifeInsurance_DeductionAmount").val(null);

    //新旧区分チェック
    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 9; i++) {
    //for (var i = 1; i < 5; i++) {
    //2023-11-20 iwai-tamura upd end -----
        id1 = "Head_LifeInsurance_" + i + "_OldAndNewType"
        id2 = "Head_LifeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intNewTotalAmount = 0;
    //2026-99-99 iwai-miki add str -----
    var int23NewTotalAmount = 0;
    //2026-99-99 iwai-miki add end -----
    var intOldTotalAmount = 0;
    var intCalc1 = 0;
    //2026-99-99 iwai-miki add str -----
    var intCalc3 = 0;
    //2026-99-99 iwai-miki add end -----
    var intCalc2 = 0;
    var intTotalAmount = 0;
    //2026-99-99 iwai-miki add str -----
    var intTotalAmount2 = 0;
    //2026-99-99 iwai-miki add end -----

    var intDeductionAmount = 0;

    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 9; i++) {
    //for (var i = 1; i < 5; i++) {
    //2023-11-20 iwai-tamura upd end -----
        id1 = "Head_LifeInsurance_" + i + "_OldAndNewType"
        id2 = "Head_LifeInsurance_" + i + "_InsuranceFee"

        //2026-99-99 iwai-miki upd str -----
        if (document.getElementById(id1).value == "1") {
            if (Under23Judgment == "1") {
                //年齢23歳未満の扶養親族有り
                int23NewTotalAmount += Number(document.getElementById(id2).value);
            } else {
                intNewTotalAmount += Number(document.getElementById(id2).value);
            }
        }
        //if (document.getElementById(id1).value == "1") {
        //    intNewTotalAmount += Number(document.getElementById(id2).value);
        //}
        //2026-99-99 iwai-miki upd end -----
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

    //2026-99-99 iwai-miki add str -----
    if (int23NewTotalAmount <= 30000) {
        intCalc3 = int23NewTotalAmount
    } else if (int23NewTotalAmount <= 60000) {
        intCalc3 = Math.ceil(int23NewTotalAmount / 2) + 15000

    } else if (int23NewTotalAmount <= 120000) {
        intCalc3 = Math.ceil(int23NewTotalAmount / 4) + 30000
    } else {
        intCalc3 = 60000
    }
    //2026-99-99 iwai-miki add end -----

    if (intOldTotalAmount <= 25000) {
        intCalc2 = intOldTotalAmount
    } else if (intOldTotalAmount <= 50000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 2) + 12500

    } else if (intOldTotalAmount <= 100000) {
        intCalc2 = Math.ceil(intOldTotalAmount / 4) + 25000
    } else {
        intCalc2 = 50000
    }

    //2026-99-99 iwai-miki upd str -----
    intTotalAmount = intCalc1 + intCalc2;
    if (intTotalAmount > 40000) {
        intTotalAmount = 40000;
    }
    intTotalAmount2 = intCalc3 + intCalc2;
    if (intTotalAmount2 > 60000) {
        intTotalAmount2 = 60000;
    }
    var intDeductionAmount = Math.max(intTotalAmount, intTotalAmount2, intCalc2);
    //intTotalAmount = intCalc1 + intCalc2;
    //if (intTotalAmount > 40000) {
    //    intTotalAmount = 40000;
    //}
    //if (intTotalAmount > intCalc2) {
    //    intDeductionAmount = intTotalAmount;
    //} else {
    //    intDeductionAmount = intCalc2;
    //}
    //2026-99-99 iwai-miki upd end -----

    //2026-99-99 iwai-miki upd str -----
    if (Under23Judgment == "1") {
        $("#Head_LifeInsurance_NewTotalAmount").val(int23NewTotalAmount);
    } else {
        $("#Head_LifeInsurance_NewTotalAmount").val(intNewTotalAmount);
    }
    //$("#Head_LifeInsurance_NewTotalAmount").val(intNewTotalAmount);
    //2026-99-99 iwai-miki upd end -----
    $("#Head_LifeInsurance_OldTotalAmount").val(intOldTotalAmount);
    $("#Head_LifeInsurance_Calc1").val(intCalc1);
    //2026-99-99 iwai-miki add str -----
    $("#Head_LifeInsurance_Calc3").val(intCalc3);
    //2026-99-99 iwai-miki add end -----
    $("#Head_LifeInsurance_Calc2").val(intCalc2);
    $("#Head_LifeInsurance_TotalAmount").val(intTotalAmount);
    //2026-99-99 iwai-miki add str -----
    $("#Head_LifeInsurance_TotalAmount2").val(intTotalAmount2);
    //2026-99-99 iwai-miki add end -----
    $("#Head_LifeInsurance_DeductionAmount").val(intDeductionAmount);

    bolReturn = true;
    return bolReturn;
}

//2024-11-19 iwai-tamura upd-str ------
//一般の生命保険料クリア処理
function clearLifeInsuranceValue(index) {
    if ($('#Head_LifeInsurance_' + index + '_HostDataFlg').is(':checked')) {
        alert("クリアする場合は、給与天引き団体保険料のチェックを外してください。");
    } else {
        $('#Head_LifeInsurance_' + index + '_InsuranceFee').val('');
        $('#Head_LifeInsurance_' + index + '_OldAndNewType').val('');
        $('#Head_LifeInsurance_' + index + '_InsuranceCompanyName').val('');
        $('#Head_LifeInsurance_' + index + '_InsuranceTypeName').val('');
        $('#Head_LifeInsurance_' + index + '_InsurancePeriod').val('');
        $('#Head_LifeInsurance_' + index + '_ContractorName1').val('');
        $('#Head_LifeInsurance_' + index + '_ContractorName2').val('');
        $('#Head_LifeInsurance_' + index + '_ReceiverName1').val('');
        $('#Head_LifeInsurance_' + index + '_ReceiverName2').val('');
        $('#Head_LifeInsurance_' + index + '_RelationshipType').val('');
    }
}

// 介護医療保険のデータをクリア
function clearMedicalInsuranceValue(index) {
    if ($('#Head_MedicalInsurance_' + index + '_HostDataFlg').is(':checked')) {
        alert("クリアする場合は、給与天引き団体保険料のチェックを外してください。");
    } else {
        $('#Head_MedicalInsurance_' + index + '_InsuranceCompanyName').val('');
        $('#Head_MedicalInsurance_' + index + '_InsuranceTypeName').val('');
        $('#Head_MedicalInsurance_' + index + '_InsurancePeriod').val('');
        $('#Head_MedicalInsurance_' + index + '_ContractorName1').val('');
        $('#Head_MedicalInsurance_' + index + '_ContractorName2').val('');
        $('#Head_MedicalInsurance_' + index + '_ReceiverName1').val('');
        $('#Head_MedicalInsurance_' + index + '_ReceiverName2').val('');
        $('#Head_MedicalInsurance_' + index + '_RelationshipType').val('');
        $('#Head_MedicalInsurance_' + index + '_InsuranceFee').val('');
    }
}

// 個人年金保険のデータをクリア
function clearPensionInsuranceValue(index) {
    if ($('#Head_PensionInsurance_' + index + '_HostDataFlg').is(':checked')) {
        alert("クリアする場合は、給与天引き団体保険料のチェックを外してください。");
    } else {
        $('#Head_PensionInsurance_' + index + '_InsuranceCompanyName').val('');
        $('#Head_PensionInsurance_' + index + '_InsuranceTypeName').val('');
        $('#Head_PensionInsurance_' + index + '_InsurancePeriod').val('');
        $('#Head_PensionInsurance_' + index + '_ContractorName1').val('');
        $('#Head_PensionInsurance_' + index + '_ContractorName2').val('');
        $('#Head_PensionInsurance_' + index + '_ReceiverName1').val('');
        $('#Head_PensionInsurance_' + index + '_ReceiverName2').val('');
        $('#Head_PensionInsurance_' + index + '_RelationshipType').val('');
        $('#Head_PensionInsurance_' + index + '_StartPaymentYear').val('');
        $('#Head_PensionInsurance_' + index + '_StartPaymentMonth').val('');
        $('#Head_PensionInsurance_' + index + '_StartPaymentDay').val('');
        $('#Head_PensionInsurance_' + index + '_OldAndNewType').val('');
        $('#Head_PensionInsurance_' + index + '_InsuranceFee').val('');
    }
}

// 地震保険のデータをクリア
function clearQuakeInsuranceValue(index) {
    if ($('#Head_QuakeInsurance_' + index + '_HostDataFlg').is(':checked')) {
        alert("クリアする場合は、給与天引き団体保険料のチェックを外してください。");
    } else {
        $('#Head_QuakeInsurance_' + index + '_InsuranceCompanyName').val('');
        $('#Head_QuakeInsurance_' + index + '_InsuranceTypeName').val('');
        $('#Head_QuakeInsurance_' + index + '_InsurancePeriod').val('');
        $('#Head_QuakeInsurance_' + index + '_ContractorName1').val('');
        $('#Head_QuakeInsurance_' + index + '_ContractorName2').val('');
        $('#Head_QuakeInsurance_' + index + '_ReceiverName1').val('');
        $('#Head_QuakeInsurance_' + index + '_ReceiverName2').val('');
        $('#Head_QuakeInsurance_' + index + '_RelationshipType').val('');
        $('#Head_QuakeInsurance_' + index + '_QuakeAndDamageType').val('');
        $('#Head_QuakeInsurance_' + index + '_InsuranceFee').val('');
    }
}

// 社会保険のデータをクリア
function clearSocialInsuranceValue(index) {
    $('#Head_SocialInsurance_' + index + '_InsuranceTypeName').val('');
    $('#Head_SocialInsurance_' + index + '_InsuranceCompanyName').val('');
    $('#Head_SocialInsurance_' + index + '_ContractorName1').val('');
    $('#Head_SocialInsurance_' + index + '_ContractorName2').val('');
    $('#Head_SocialInsurance_' + index + '_RelationshipType').val('');
    $('#Head_SocialInsurance_' + index + '_InsuranceFee').val('');
}

//発火イベント設定
$(document).ready(function () {
    //一般の生命保険料イベント設定
    for (let i = 1; i <= 8; i++) {
        //新旧区分
        $('#Head_LifeInsurance_' + i + '_OldAndNewType').on('change', function () {
            checkAndCalcLifeInsurance(i);
        });
        //保険料
        $('#Head_LifeInsurance_' + i + '_InsuranceFee').on('change', function () {
            checkAndCalcLifeInsurance(i);
        });

        //クリアボタンのクリックイベント
        $('#btn-LifeInsurance-clear_' + i).on('click', function () {
            clearLifeInsuranceValue(i);
        });
    }

    //介護医療保険料イベント設定
    for (let i = 1; i <= 6; i++) {
        //保険料
        $('#Head_MedicalInsurance_' + i + '_InsuranceFee').on('change', function () {
            calcMedicalInsuranceCheck();
        });

        //クリアボタンのクリックイベント
        $('#btn-MedicalInsurance-clear_' + i).on('click', function () {
            clearMedicalInsuranceValue(i);
        });
    }

    //個人年金保険料イベント設定
    for (let i = 1; i <= 4; i++) {
        //新旧区分
        $('#Head_PensionInsurance_' + i + '_OldAndNewType').on('change', function () {
            checkAndCalcPensionInsurance(i);
        });
        //保険料
        $('#Head_PensionInsurance_' + i + '_InsuranceFee').on('change', function () {
            checkAndCalcPensionInsurance(i);
        });

        //クリアボタンのクリックイベント
        $('#btn-PensionInsurance-clear_' + i).on('click', function () {
            clearPensionInsuranceValue(i);
        });
    }

    //地震保険料イベント設定
    for (let i = 1; i <= 4; i++) {
        //地震旧長期区分
        $('#Head_QuakeInsurance_' + i + '_QuakeAndDamageType').on('change', function () {
            checkAndCalcQuakeInsurance(i);
        });
        //保険料
        $('#Head_QuakeInsurance_' + i + '_InsuranceFee').on('change', function () {
            checkAndCalcQuakeInsurance(i);
        });

        //クリアボタンのクリックイベント
        $('#btn-QuakeInsurance-clear_' + i).on('click', function () {
            clearQuakeInsuranceValue(i);
        });
    }

    //社会保険料イベント設定
    for (let i = 1; i <= 3; i++) {
        //クリアボタンのクリックイベント
        $('#btn-SocialInsurance-clear_' + i).on('click', function () {
            clearSocialInsuranceValue(i);
        });
    }
});

//一般生命保険 新旧区分と金額の入力チェック　チェック後上限チェックを行う。
function checkAndCalcLifeInsurance(index) {
    let insuranceFee = $('#Head_LifeInsurance_' + index + '_InsuranceFee').val();
    let oldAndNewType = $('#Head_LifeInsurance_' + index + '_OldAndNewType').val();

    if (insuranceFee && oldAndNewType) {
        calcLifeInsuranceCheck();
    }
}

//個人年金保険 新旧区分と金額の入力チェック　チェック後上限チェックを行う。
function checkAndCalcPensionInsurance(index) {
    let insuranceFee = $('#Head_PensionInsurance_' + index + '_InsuranceFee').val();
    let oldAndNewType = $('#Head_PensionInsurance_' + index + '_OldAndNewType').val();

    if (insuranceFee && oldAndNewType) {
        calcPensionInsuranceCheck();
    }
}

//地震保険 地震旧長期区分と金額の入力チェック　チェック後上限チェックを行う。
function checkAndCalcQuakeInsurance(index) {
    let insuranceFee = $('#Head_QuakeInsurance_' + index + '_InsuranceFee').val();
    let oldAndNewType = $('#Head_QuakeInsurance_' + index + '_QuakeAndDamageType').val();

    if (insuranceFee && oldAndNewType) {
        calcQuakeInsuranceCheck();
    }
}




//一般生命保険料上限チェック
function calcLifeInsuranceCheck() {
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
    for (var i = 1; i < 9; i++) {
        id1 = "Head_LifeInsurance_" + i + "_OldAndNewType"
        id2 = "Head_LifeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intNewTotalAmount = 0;
    var intOldTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;

    for (var i = 1; i < 9; i++) {
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

    strMessage = "";
    if (intCalc2>=50000){
        strMessage = "※生命保険料の合計額が上限に達しました。<br/>生命保険料はこれ以上入力する必要はありません。";
    } else if(intCalc1 + intCalc2>=40000){
        strMessage = "※新制度の生命保険料の合計額が上限に達しました。<br/>新制度の生命保険料はこれ以上入力する必要はありません。";
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    } else {
        bolReturn = true;
    }
    return bolReturn;
}

//介護医療保険料上限チェック
function calcMedicalInsuranceCheck() {
    var strMessage = ""
    var bolReturn = false
    var id1 = ""


    $("#Head_MedicalInsurance_TotalAmount").val(null);
    $("#Head_MedicalInsurance_DeductionAmount").val(null);

    //金額計算処理
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 7; i++) {
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
    strMessage = "";
    if (intCalc1 >= 40000) {
        strMessage = "※介護保険料の合計額が上限に達しました。<br/>介護保険料はこれ以上入力する必要はありません。";
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    } else {
        bolReturn = true;
    }
    return bolReturn;
}


//個人年金保険料上限チェック
function calcPensionInsuranceCheck() {
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
    for (var i = 1; i < 5; i++) {
        id1 = "Head_PensionInsurance_" + i + "_OldAndNewType"
        id2 = "Head_PensionInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
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

    strMessage = "";
    if (intCalc2 >= 50000) {
        strMessage = "※個人年金保険料の合計額が上限に達しました。<br/>個人年金保険料はこれ以上入力する必要はありません。";
    } else if (intCalc1 + intCalc2 >= 40000) {
        strMessage = "※新制度の個人年金保険料の合計額が上限に達しました。<br/>新制度の個人年金保険料はこれ以上入力する必要はありません。";
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    } else {
        bolReturn = true;
    }
    return bolReturn;
}

//地震保険料上限チェック
function calcQuakeInsuranceCheck() {
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
    for (var i = 1; i < 5; i++) {
        id1 = "Head_QuakeInsurance_" + i + "_QuakeAndDamageType"
        id2 = "Head_QuakeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "地震または旧長期区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intQuakeTotalAmount = 0;
    var intDamageTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    for (var i = 1; i < 5; i++) {
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
    if (intCalc2 > 15000) {
        intCalc2 = 15000;
    }

    intDeductionAmount = intCalc1 + intCalc2;
    if (intDeductionAmount > 50000) {
        intDeductionAmount = 50000;
    }

    strMessage = "";
    if (intCalc1 + intCalc2 >= 50000) {
        strMessage = "※地震保険料保険料の合計額が上限に達しました。<br/>地震保険料保険料はこれ以上入力する必要はありません。";
    } else if (intCalc2 >= 15000) {
        strMessage = "※旧長期損害保険料の合計額が上限に達しました。<br/>旧長期損害保険料はこれ以上入力する必要はありません。";
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    } else {
        bolReturn = true;
    }
    return bolReturn;



    bolReturn = true;
    return bolReturn;
}
//2024-11-19 iwai-tamura upd-end ------


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

    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 7; i++) {
    //for (var i = 1; i < 3; i++) {
    //2023-11-20 iwai-tamura upd end -----
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
    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 5; i++) {
    //for (var i = 1; i < 4; i++) {
    //2023-11-20 iwai-tamura upd end -----
        id1 = "Head_PensionInsurance_" + i + "_OldAndNewType"
        id2 = "Head_PensionInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "新旧区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intNewTotalAmount = 0;
    var intOldTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 5; i++) {
    //for (var i = 1; i < 4; i++) {
    //2023-11-20 iwai-tamura upd end -----
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
    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 5; i++) {
    //for (var i = 1; i < 3; i++) {
    //2023-11-20 iwai-tamura upd end -----
        id1 = "Head_QuakeInsurance_" + i + "_QuakeAndDamageType"
        id2 = "Head_QuakeInsurance_" + i + "_InsuranceFee"
        if (document.getElementById(id2).value != "") {
            if (document.getElementById(id1).value == "") {
                strMessage = "地震または旧長期区分が正しく入力されていない箇所があります。";
            }
        }
    }
    if (strMessage != "") {
        showAlert('確認', strMessage);
        return bolReturn;
    }

    //金額計算処理
    var intQuakeTotalAmount = 0;
    var intDamageTotalAmount = 0;
    var intCalc1 = 0;
    var intCalc2 = 0;
    var intTotalAmount = 0;
    var intDeductionAmount = 0;

    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 5; i++) {
    //for (var i = 1; i < 3; i++) {
    //2023-11-20 iwai-tamura upd end -----
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
    //2024-11-19 iwai-tamura upd-str ------
    if (intCalc2 > 15000) {
        intCalc2 = 15000;
    }
    //if (intDamageTotalAmount > 15000) {
    //    intCalc2 = 15000;
    //}
    //2024-11-19 iwai-tamura upd-end ------

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

    //2023-11-20 iwai-tamura upd str -----
    for (var i = 1; i < 4; i++) {
    //for (var i = 1; i < 3; i++) {
    //2023-11-20 iwai-tamura upd end -----
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
    //var cDate23 = String(cYear - 22) + "0101";

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

    ////年齢チェック
    //if (strMonth.length < 2) {
    //    strMonth = "0" + strMonth;
    //}
    //if (strDay.length < 2) {
    //    strDay = "0" + strDay;
    //}
    //var strDate = strYear + strMonth + strDay;
    //if (strDate <= cDate23) {
    //    strMessage = "基準日の年齢が23歳を超えてます。";
    //    $(id + "_mes").text(strMessage);
    //    return false;
    //}

    //$(id + "_mes").text(strMessage);
    //return bolReturn;
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
        strMessage = "数値で入力してください。";
    } else if ((strVal) > maxMoney) {
        strMessage = maxMoney / 10000 + "万円以上は対象外です。";
    } else {
        bolReturn = true;
    }
    $(id + "_mes").text(strMessage);
    return bolReturn;
}

//2026-99-99 iwai-miki add str -----
//年齢23歳未満の扶養親族
//  所得
document.getElementById('Head_DependentsUnder23_Earnings').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder23_Earnings', 'Head_DependentsUnder23_Earnings2Income', 'Head_DependentsUnder23_OtherIncome', 'Head_DependentsUnder23_Income');
    checkMoney('#Head_DependentsUnder23_Income', 620000)
});
//  その他所得
document.getElementById('Head_DependentsUnder23_OtherIncome').addEventListener('change', function () {
    updateIncomeFields('Head_DependentsUnder23_Earnings', 'Head_DependentsUnder23_Earnings2Income', 'Head_DependentsUnder23_OtherIncome', 'Head_DependentsUnder23_Income');
    checkMoney('#Head_DependentsUnder23_Income', 620000)
});

//年齢23歳未満の扶養親族 住所又は居所の入力制御
$('#Head_DependentsUnder23_AddressSameCheck').change(function () {
    if (document.getElementById("Head_DependentsUnder23_AddressSameCheck").checked == true) {
        $(".DependentsUnder23Address_isRead").removeAttr("disabled");
    } else {
        $(".DependentsUnder23Address_isRead").attr('disabled', true);
        $("#Head_DependentsUnder23_Address").val('');
    }
});
//2026-99-99 iwai-miki add end -----

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
    //2023-11-20 iwai-tamura upd str -----
    var isAdminMode = $('#Head_AdminMode').val().toLowerCase() === 'true';
    if (isAdminMode) {
        //2023-12-15 iwai-tamura upd str -----
        if ($('#Head_DecisionType').val() <= '5') {
            showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
        } else {
            showMessageEx('修正確認', '修正しますか？ <br><br> ※既に連携済みデータの為、連携先システムの修正も同様に行ってください。', 'savebutton', true);
        }
        //showMessageEx('確定確認', '確定しますか？', 'savebutton', true);
	    //2023-12-15 iwai-tamura upd end -----
    } else {
        showMessageEx('提出確認', '提出しますか？', 'savebutton', true);
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
