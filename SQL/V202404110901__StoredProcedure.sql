use AutoReviewerDB;
go

create procedure ViewUsersWithSkillInRepository 
	@skillID int,
	@repositoryID int
as
	select u.gitHubUsername, s.skillName, r.repositoryName
	from UserSkills as us
	inner join Skill as s on s.skillID = us.skillID
	inner join GitHubUser as u on u.gitHubUserID = us.gitHubUserID
	inner join Registration as reg on reg.gitHubUserID = us.gitHubUserID
	inner join Repository as r on r.repositoryID = reg.repositoryID
	where 
		s.skillID = @skillID and
		r.repositoryID = @repositoryID;
go
