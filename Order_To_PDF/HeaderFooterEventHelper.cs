using iTextSharp.text.pdf;
using iTextSharp.text;
using System;

public class HeaderFooterEventHelper : PdfPageEventHelper
{
    private string _companyName;
    private string _authorName;

    // 생성자에서 회사명과 작성자명을 전달받습니다.
    public HeaderFooterEventHelper(string companyName, string authorName)
    {
        _companyName = companyName;
        _authorName = authorName;
    }

    // OnEndPage 이벤트에서 헤더와 푸터를 추가합니다.
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        PdfContentByte cb = writer.DirectContent;

        // 폰트 설정 (Helvetica 사용)
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        cb.SetFontAndSize(bf, 10);

        // 헤더 텍스트 설정 (회사명, 작성자명)
        string headerText = $"회사명{_companyName} | {_authorName}";

        // 중앙 정렬로 상단에 텍스트 추가
        float headerX = (document.PageSize.Width / 2);
        float headerY = document.PageSize.Height - 20; // 상단에서 20포인트 아래
        cb.BeginText();
        cb.ShowTextAligned(Element.ALIGN_CENTER, headerText, headerX, headerY, 0);
        cb.EndText();

        // 푸터 텍스트 설정 (현재 시간)
        string footerText = $"생성시간 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"; //여기도 영어만됨

        // 중앙 정렬로 하단에 텍스트 추가
        float footerX = (document.PageSize.Width / 2);
        float footerY = document.BottomMargin / 2; // 하단에서 여백/2 포인트 위
        cb.BeginText();
        cb.ShowTextAligned(Element.ALIGN_CENTER, footerText, footerX, footerY, 0);
        cb.EndText();
    }
}