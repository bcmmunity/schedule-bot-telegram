delete from Lessons where Id >= 10730
delete from ScheduleDays where Id >=5307
delete from ScheduleWeeks where Id >=759

update Users
set UniversityId = NULL,
FacilityId = NULL,
CourseId = NULL,
GroupId = NULL
where UniversityId = 3


delete from Groups where Id >= 375
delete from Courses where Id >=22
delete from Facilities where Id >=6
delete from Universities where Id=2