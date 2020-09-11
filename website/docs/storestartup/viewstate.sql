--------
-- in order to create ASPState Database please run following:
-- aspnet_regsql.exe -U "username" -P "passworod" -S "servername" -ssadd -p
--------

USE ASPState
GO

DECLARE @jobID BINARY(16) 

SELECT @jobID = job_id FROM msdb.dbo.sysjobs WHERE name = N'ASPState_Job_ExpireViewState'

IF (@JobID IS NOT NULL) BEGIN  
       IF EXISTS(SELECT * FROM msdb.dbo.sysjobservers WHERE job_id = @JobID AND server_id <> 0) BEGIN
              RAISERROR (N'Unable to import job ''ASPNET_Job_ExpireViewState'' since there is already a multi-server job with this name.', 16, 1)
       END ELSE
              EXECUTE msdb.dbo.sp_delete_job @job_name = N'ASPState_Job_ExpireViewState'
END
GO

IF OBJECTPROPERTY(OBJECT_ID(N'dbo.ExpireViewState'), N'IsProcedure') = 1
       DROP PROCEDURE dbo.ExpireViewState
IF OBJECTPROPERTY(OBJECT_ID(N'dbo.SetViewState'), N'IsProcedure') = 1
       DROP PROCEDURE dbo.SetViewState
IF OBJECTPROPERTY(OBJECT_ID(N'dbo.GetViewState'), N'IsProcedure') = 1
       DROP PROCEDURE dbo.GetViewState
IF OBJECTPROPERTY(OBJECT_ID(N'dbo.ViewState'), N'IsUserTable') = 1
       DROP TABLE dbo.ViewState
GO

CREATE TABLE dbo.ViewState (
       ViewStateId UNIQUEIDENTIFIER NOT NULL,
       Value IMAGE NOT NULL,
       LastAccessed DATETIME NOT NULL,
       Timeout INT NOT NULL CONSTRAINT CK_ViewState_Timeout CHECK(Timeout > 0),
       CONSTRAINT PK_ViewState PRIMARY KEY CLUSTERED (ViewStateId),
)
GO

CREATE PROCEDURE dbo.GetViewState (@viewStateId UNIQUEIDENTIFIER) AS
       SET NOCOUNT ON
       DECLARE @textPtr VARBINARY(16)
       DECLARE @length INT

       UPDATE dbo.ViewState
          SET LastAccessed = GETUTCDATE(),
              @textPtr = TEXTPTR(Value),
              @length = DATALENGTH(Value)
        WHERE ViewStateId = @viewStateId

       IF @length IS NOT NULL BEGIN
              SELECT @length AS Length
              READTEXT ViewState.Value @textPtr 0 @length
       END
       RETURN 0
GO

CREATE PROCEDURE dbo.SetViewState (@viewStateId UNIQUEIDENTIFIER, @value IMAGE, @timeout INT = 20) AS
       SET NOCOUNT ON

       IF @viewStateId IS NULL BEGIN
              RETURN -1
       END ELSE IF @timeout < 1 BEGIN
              RETURN -2
       END ELSE IF @value IS NULL BEGIN
              RETURN -3
       END

       IF EXISTS(SELECT * FROM ViewState WHERE ViewStateId = @viewStateID) BEGIN 
              UPDATE dbo.ViewState
                 SET LastAccessed = GETUTCDATE() ,Value = @value WHERE ViewStateID = @viewStateId
       END ELSE BEGIN
              INSERT INTO dbo.ViewState (ViewStateId, Value, LastAccessed, Timeout) VALUES (@viewStateId, @value, GETUTCDATE(), @timeout)
       END
       RETURN 0
GO

CREATE PROCEDURE dbo.ExpireViewState AS
       SET NOCOUNT ON
       DELETE FROM dbo.ViewState WHERE GETUTCDATE() > DATEADD(minute, Timeout, LastAccessed)
GO

BEGIN TRANSACTION            
       DECLARE @jobID BINARY(16)
       DECLARE @returnCode INT

       SET @returnCode = 0

       IF NOT EXISTS(SELECT * FROM msdb.dbo.syscategories WHERE name = N'[Uncategorized (Local)]')
              EXEC msdb.dbo.sp_add_category @name=N'[Uncategorized (Local)]'

       EXECUTE @returnCode = msdb.dbo.sp_add_job
                     @job_id = @jobID OUTPUT,
                     @job_name = N'ASPState_Job_ExpireViewState',
                     @owner_login_name = NULL,
                     @description = N'Deletes expired view state information.',
                     @category_name = N'[Uncategorized (Local)]',
                     @enabled = 1,
                     @notify_level_email = 0,
                     @notify_level_page = 0,
                     @notify_level_netsend = 0,
                     @notify_level_eventlog = 0,
                     @delete_level = 0

       IF @@ERROR <> 0 OR @returnCode <> 0 GOTO QuitWithRollback

       EXECUTE @returnCode = msdb.dbo.sp_add_jobstep
            @job_id = @jobID,
            @step_id = 1,
            @step_name = N'ASPState_JobStep_ExpireViewState',
            @command = N'EXECUTE ExpireViewState',
            @database_name = N'ASPState',
            @server = N'',
            @database_user_name = N'',
            @subsystem = N'TSQL',
            @cmdexec_success_code = 0,
            @flags = 0,
            @retry_attempts = 0,
            @retry_interval = 1,
            @output_file_name = N'',
            @on_success_step_id = 0,
            @on_success_action = 1,
            @on_fail_step_id = 0,
            @on_fail_action = 2

    IF @@ERROR <> 0 OR @ReturnCode <> 0 GOTO QuitWithRollback

    EXECUTE @returnCode = msdb.dbo.sp_update_job
                     @job_id = @jobID,
                     @start_step_id = 1

    IF @@ERROR <> 0 OR @ReturnCode <> 0 GOTO QuitWithRollback

    EXECUTE @returnCode = msdb.dbo.sp_add_jobschedule
            @job_id = @jobID,
            @name = N'ASPState_JobSchedule_ExpireViewState',
            @enabled = 1,
            @freq_type = 4,
            @active_start_date = 20001016,
            @active_start_time = 0,
            @freq_interval = 1,
            @freq_subday_type = 4,
            @freq_subday_interval = 1,
            @freq_relative_interval = 0,
            @freq_recurrence_factor = 0,
            @active_end_date = 99991231,
            @active_end_time = 235959

    IF @@ERROR <> 0 OR @ReturnCode <> 0	GOTO QuitWithRollback

    EXECUTE @returnCode = msdb.dbo.sp_add_jobserver
                     @job_id = @jobID,
                     @server_name = N'(local)'
    
    IF @@ERROR <> 0 OR @ReturnCode <> 0 GOTO QuitWithRollback

    COMMIT TRANSACTION          
    GOTO   EndSave

QuitWithRollback:
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
              
EndSave:

  