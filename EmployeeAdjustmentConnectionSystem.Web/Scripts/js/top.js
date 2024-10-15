/*
 * ログアウトボタンクリック時
 */
$('#dmylogin').click(function () {
  //ボタンクリック
  showLogout(false, 'loginbutton');
});

$(function () {
    var status = document.getElementById("ServerStatus").value;

    // 扶養控除等申告書ボタンとPDFボタンを取得
    var btnHuyouReg = document.querySelector('[name="Transition"][value="HuyouDeclareRegister"]');
    var btnHokenReg = document.querySelector('[name="Transition"][value="HokenDeclareRegister"]');
    var btnHaiguuReg = document.querySelector('[name="Transition"][value="HaiguuDeclareRegister"]');
    var btnHuyouPdf = document.querySelector('[name="PrintHuyou"][value="HuyouDeclareReport"]');
    var btnHokenPdf = document.querySelector('[name="PrintHoken"][value="HokenDeclareReport"]');
    var btnHaiguuPdf = document.querySelector('[name="PrintHaiguu"][value="HaiguuDeclareReport"]');

    // Statusに基づいてボタンの使用可否を設定
    if (status === "1") {
        // [1:通常運用]すべてのボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = false;
        btnHaiguuReg.disabled = false;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = false;
        btnHaiguuPdf.disabled = false;
    } else if (status === "2") {
        // [2:通常運用]扶養控除と扶養控除のPDFボタンを有効にする
        btnHuyouReg.disabled = false;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    } else if (status === "3") {
        // [3:通常運用]扶養控除のPDFボタンのみ有効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = false;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    } else {
        // それ以外の場合、すべてのボタンを無効にする
        btnHuyouReg.disabled = true;
        btnHokenReg.disabled = true;
        btnHaiguuReg.disabled = true;
        btnHuyouPdf.disabled = true;
        btnHokenPdf.disabled = true;
        btnHaiguuPdf.disabled = true;
    }


});