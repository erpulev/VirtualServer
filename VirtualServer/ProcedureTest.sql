--ERROR MULTI SELECT

CREATE PROCEDURE [dbo].[ProcedureTest]
	@workWas bit = false,
	@workNow bit = false
AS
BEGIN
--DECLARE @workWas BIT;
--DECLARE @workNow BIT;

	SELECT @workWas =  Worked FROM IsWorkeds WHERE Id = 1 

	IF((SELECT TOP 1 * FROM VirtualServers WHERE RemoveDateTime IS NULL) > 0)
		SET @workNow = true
	ELSE
		SET @workNow = false

	IF(@workWas = TRUE AND @workNow = TRUE)
		PRINT('true and true');
	ELSE IF (@workWas = TRUE AND @workNow = FALSE)
		PRINT('true and false');
	ELSE IF (@workWas = FALSE AND @workNow = TRUE)
		PRINT('false and true');
	ELSE IF (@workWas = FALSE AND @workNow = FALSE)
		PRINT('false and false');
END