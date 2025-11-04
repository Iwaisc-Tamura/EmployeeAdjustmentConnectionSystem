
/*エンターキー無効化*/
$(document).keypress(function (e) {
    // エンターキーだったら無効にする
    if (e.key === 'Enter') {
        return false;
    }
});

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


//社員番号FromTo入力補助
$('#Search_EmployeeNoFrom').change(function () {
    document.getElementById('Search_EmployeeNoTo').value = this.value;
});

//所属番号FromTo入力補助
$('#Search_DepartmentFrom').change(function () {
    document.getElementById('Search_DepartmentTo').value = this.value;
});


/*
 * 全選択・全解除をクリック時
 */
$('.allCheck input').click(function () {
  //チェックボックス取得
  var items = $('#items').find('input');
  //全選択チェック時 true・・・全チェック、false・・・未チェック
  var isCheck = $(this).is(':checked') ? true : false;
  //置き換え
  $(items).prop('checked', isCheck);
});

/*
 * 検索ボタンクリック時
 */
$('#dmysearch').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    if (document.getElementById('Search_Year').value == '') {
        showMessage('警告', '対象となる年度が入力されていません。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---

    //ローディングパネル表示
    showLoading();
    //ボタンクリック
    $('#searchbutton').trigger('click');
});

/*
 * 検索ボタンクリック時
 */
$('#dmysearchD').click(function () {
    //ローディングパネル表示
    showLoading();
    //ボタンクリック
    $('#searchbuttonD').trigger('click');
});

/*
 * 本人入力ボタンクリック時
 */
$('#dmyselfinput').click(function () {
  //ローディングパネル表示
  //showLoading();
  //ボタンクリック
  $('#selfinputbutton').trigger('click');
});


/*
 * 一括確定ボタンクリック時
 */
$('#dmysignbatch_huyou').click(function () {

    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---

    showMessage('一括確定', '確定しますか？', 'signbatch_huyou_button', false);
});
$('#dmysignbatch_hoken').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---
    showMessage('一括確定', '確定しますか？', 'signbatch_hoken_button', false);
});
$('#dmysignbatch_haiguu').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---
    showMessage('一括確定', '確定しますか？', 'signbatch_haiguu_button', false);
});

//2025-99-99 iwai-tamura upd-str ------
$('#dmysignbatch_jutaku').click(function () {
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    showMessage('一括確定', '確定しますか？', 'signbatch_jutaku_button', false);
});
$('#dmysignbatch_zenshoku').click(function () {
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    showMessage('一括確定', '確定しますか？', 'signbatch_zenshoku_button', false);
});
//2025-99-99 iwai-tamura upd-end ------


/*
 * 一括帳票出力ボタンクリック時
 */
$('#dmyprintbatch_huyou').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---
    showMessage('帳票出力', '出力しますか？', 'printbatch_huyou_button', false);
});
$('#dmyprintbatch_hoken').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---
    showMessage('帳票出力', '出力しますか？', 'printbatch_hoken_button', false);
});
$('#dmyprintbatch_haiguu').click(function () {
    //2025-03-21 iwai-tamura add-str ---
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---
    showMessage('帳票出力', '出力しますか？', 'printbatch_haiguu_button', false);
});

//2025-99-99 iwai-tamura upd-str ------
//住宅控除EXCEL出力
$('#dmyprintxls_jutaku').click(function () {
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    showMessage('一括出力', 'エクセルデータを出力しますか？', 'printxls_jutaku_button', false);
});

//前職源泉EXCEL出力
$('#dmyprintxls_zenshoku').click(function () {
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    showMessage('一括出力', 'エクセルデータをしますか？', 'printxls_zenshoku_button', false);
});
//2025-99-99 iwai-tamura upd-end ------

$('#dmyprintatoc').click(function () {
    showMessage('帳票出力', '出力しますか？', 'printatocbutton', false);
});
$('#dmyprintd').click(function () {
    showMessage('帳票出力', '出力しますか？', 'printdbutton', false);
});
$('#dmyprintcareer').click(function () {
    showMessage('帳票出力', '出力しますか？', 'printcareerbutton', false);
});

/*
 * 部下表示ボタンクリック時
 */
$('#dmysubview').click(function () {
  //ローディングパネル表示
  showLoading();
  //ボタンクリック
  $('#subviewbutton').trigger('click');
});

/*
 * 一括承認ボタンクリック時
 */
$('#dmysign').click(function () {
  //ボタンクリック
  showMessage('一括承認', '承認しますか？', 'signbutton', true);
});


//2024-12-24 iwai-tamura add-str ---
/*
 * メール配信ボタンクリック時
 */
$('#dmysendmailbutton').click(function () {
    // チェックボックスの情報取得
    const checkboxes = document.querySelectorAll('input[name="selPrint"]:checked');

    // チェックが付いている数を取得
    const checkedCount = checkboxes.length;

    //2025-03-21 iwai-tamura add-str ---
    if (checkedCount == 0) {
        showMessage('選択', '対象者が１件も選択されていません。<br>対象者を選択してください。');
        return false;
    }
    //2025-03-21 iwai-tamura add-end ---

    if (checkedCount > 10) {
        showMessage('メール配信', '同時に配信できるのは10件までです。<br>選択を減らしてください。');
        return false;
    }

    //ボタンクリック
    showMessage('メール配信', '選択された対象者に催促メールを配信します。<br>よろしいですか？', 'sendmailbutton', true);
});
//2024-12-24 iwai-tamura add-end ---

//2017-03-31 sbc-sagara add str 一括Excel出力ボタン追加
// 
/*
 * 一括Excelボタンクリック時
 */
$('#dmyprintaxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printaxlsbutton', false);
});
$('#dmyprintbxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printbxlsbutton', false);
});
$('#dmyprintcxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printcxlsbutton', false);
});
$('#dmyprintdxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printdxlsbutton', false);
});
$('#dmyprintcareerxls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printcareerxlsbutton', false);
});
//2017-03-31 sbc-sagara add end 一括Excel出力ボタン追加

//2020-04-10 iwai-tamura add str 一括Excel出力ボタン追加
$('#dmyprinta11xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printa11xlsbutton', false);
});
$('#dmyprinta12xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printa12xlsbutton', false);
});
$('#dmyprinta13xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printa13xlsbutton', false);
});
$('#dmyprintb11xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printb11xlsbutton', false);
});
$('#dmyprintb12xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printb12xlsbutton', false);
});
$('#dmyprintc11xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printc11xlsbutton', false);
});
$('#dmyprintc12xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printc12xlsbutton', false);
});
//2020-04-10 iwai-tamura add end 一括Excel出力ボタン追加
//2021-12-24 iwai-tamura add str Excel出力ボタン対応
$('#dmyprinta20xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printa20xlsbutton', false);
});
$('#dmyprintb20xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printb20xlsbutton', false);
});
$('#dmyprintc20xls').click(function () {
    showMessage('Excel出力', '出力しますか？', 'printc20xlsbutton', false);
});
//2021-12-24 iwai-tamura add end Excel出力ボタン対応
