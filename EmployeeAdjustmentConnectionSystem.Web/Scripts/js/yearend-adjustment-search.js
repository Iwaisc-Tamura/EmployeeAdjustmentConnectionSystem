
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
 * 帳票出力ボタンクリック時
 */
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
