/*
 * 全選択・全解除をクリック時
 */
$('#allCheck').click(function () {
    //チェックボックス取得
    var items = $('.items').find('input');
    //全選択チェック時 true・・・全チェック、false・・・未チェック
    var isCheck = $(this).is(':checked') ? true : false;
    //置き換え
    $(items).prop('checked', isCheck);
});


$(function ($) {
    $('.Seiseki_in').change(function () {
        //変更アイテム対応欄のデフォルト値を取得
        var val = $(this).parent("td").parent("tr").children(".Seiseki_out").children("input")[0].value;
        //変更アイテム値が空でなければ値を更新
        if ($(this).val() != "") val = $(this).val();
        //変更アイテム対応欄の値を更新
        $(this).parent("td").parent("tr").children(".Seiseki_out").children("div")[0].innerHTML = val;
    });
    $('.Jyoui_in').change(function () {
        var val = $(this).parent("td").parent("tr").children(".Jyoui_out").children("input")[0].value;
        if ($(this).val() != "") val = $(this).val();
        $(this).parent("td").parent("tr").children(".Jyoui_out").children("div")[0].innerHTML = val;
    });
    $('.Hoyuu_in').change(function () {
        var val = $(this).parent("td").parent("tr").children(".Hoyuu_out").children("input")[0].value;
        if ($(this).val() != "") val = $(this).val();
        $(this).parent("td").parent("tr").children(".Hoyuu_out").children("div")[0].innerHTML = val;
    });
    $('.Mokusui_in').change(function () {
        var val = $(this).parent("td").parent("tr").children(".Mokusui_out").children("input")[0].value;
        if ($(this).val() != "") val = $(this).val();
        $(this).parent("td").parent("tr").children(".Mokusui_out").children("div")[0].innerHTML = val;
    });
});


/*
 * 登録ボタンクリック時
 */
$('#dmydoregist').click(function () {
    // 2018-05-09 iwai-tamura upd str
    //判定項目入力チェック
    if (itemCheck() == false) {
        showAlert('エラー', '未入力の判定項目があります。確認してください。');
        return;
    }
    // 2018-05-09 iwai-tamura upd end

    //ボタンクリック
    showMessage('登録', '登録しますか？', 'DoRegist', true);
});


// 2018-05-09 iwai-tamura upd str
/*
 * 判定項目入力チェック
 */
function itemCheck() {
    // 表示件数繰り返し
    for (i = 0; $('#ResultItems_' + i + '__Selected').length == 1; i++) {
        //承認チェックされているデータのみ対象とする
        if ($('#ResultItems_' + i + '__Selected').prop('checked') === true) {
            if ($('#ResultItems_' + i + '__Seiseki_BranchAdjustment').length > 0) {
                if ((!$('#ResultItems_' + i + '__Seiseki_BranchAdjustment').prop('readonly')) && (jQuery.trim($('#ResultItems_' + i + '__Seiseki_BranchAdjustment').val()) === '')) {
                    return false;
                }
            }
            if ($('#ResultItems_' + i + '__Jyoui_BranchAdjustment').length > 0) {
                if ((!$('#ResultItems_' + i + '__Jyoui_BranchAdjustment').prop('readonly')) && (jQuery.trim($('#ResultItems_' + i + '__Jyoui_BranchAdjustment').val()) === '')) {
                    return false;
                }
            }
        }
    }

    return true;
}
// 2018-05-09 iwai-tamura upd end

