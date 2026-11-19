USE [JsAjusStfDB]
GO

--※※※※※※※※※※※※※※※※※
--※※項目位置変更すること！！！※※
--※※※※※※※※※※※※※※※※※

ALTER TABLE [dbo].[TE110保険料控除申告書Data]
ADD
	[扶養親族23歳未満_氏名_姓] [nvarchar](50) NULL,
	[扶養親族23歳未満_氏名_名] [nvarchar](50) NULL,
	[扶養親族23歳未満_Kana_姓] [nvarchar](50) NULL,
	[扶養親族23歳未満_Kana_名] [nvarchar](50) NULL,
	[扶養親族23歳未満_続柄] [varchar](2) NULL,
	[扶養親族23歳未満_生年月日] [nvarchar](8) NULL,
	[扶養親族23歳未満_同上区分] [varchar](1) NULL,
	[扶養親族23歳未満_住所] [nvarchar](50) NULL,
	[扶養親族23歳未満_給与所得_収入金額] [money] NULL,
	[扶養親族23歳未満_給与所得_所得金額] [money] NULL,
	[扶養親族23歳未満_他_所得金額] [money] NULL,
	[扶養親族23歳未満_所得見積額] [money] NULL,
	[一般生命保険料23歳未満新保険料表計算] [money] NULL,
	[一般生命保険料表23歳未満合計] [money] NULL
	
go