use AutoReviewerDB;
go

alter table GitHubUser
drop column gitHubUserEmail;
go