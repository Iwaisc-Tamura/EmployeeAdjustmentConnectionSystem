USE [JsAjusStfDB]
GO

--※※※※※※※※※※※※※※※※※
--※※項目位置変更すること！！！※※
--※※※※※※※※※※※※※※※※※

ALTER TABLE [dbo].[TE160前職源泉徴収票Data]
ADD
	[添付FileName] [nvarchar](100) NULL	--[管理者確定区分]のした
	
go