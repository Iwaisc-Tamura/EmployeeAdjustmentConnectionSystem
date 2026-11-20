/*
 * 確定(上)ボタンクリック時
 */
$('#DmyDoCommit').click(function () {

    if ($('#Search_ExclusionType').val() == "") {
        showAlert('エラー', '判定除外区分が未選択です。');
        return;
    }

    if ($('#Search_EmployeeNo').val() == "") {
        showAlert('エラー', '社員番号が未入力です。');
        return;
    }

    $('#DoCommit').trigger('click');
});

/*class='calender' 入力欄クリック時カレンダー表示*/
$(function () {
    $('.calender').datepicker();
});

/*
 * 戻るボタンクリック時
 */
$('#DmyBack').click(function () {
    $('#Back').trigger('click');
});

/*
 * 取消ボタンクリック時
 */
$('#DmyCancel').click(function () {
    $('#Cancel').trigger('click');
});

/*
 * 結果表示ボタンクリック時
 */
$('#DmyShowResult').click(function () {
    /*入力エラーチェック*/

    $('#ShowResult').trigger('click');
});

/*
 * 確定(下)ボタンクリック時
 */
$('#DmyDoExecute').click(function () {
    /*入力エラーチェック*/

    $('#DoExecute').trigger('click');
});
