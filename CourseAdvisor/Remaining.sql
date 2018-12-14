Select * from SequentialToStudent S1
inner join  Program_Course P1 on S1.SequentialID = P1.SequentialID 
where not exists (Select * from Grade G1 where P1.Course_ID=G1.Course_ID);