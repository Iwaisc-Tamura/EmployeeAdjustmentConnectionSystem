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

//2020-03-31 iwai-tamura add str ------
/*
 * 検索ボタンクリック時
 */
$('#dmysearchD').click(function () {
    //ローディングパネル表示
    showLoading();
    //ボタンクリック
    $('#searchbuttonD').trigger('click');
});
//2020-03-31 iwai-tamura add end ------

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
