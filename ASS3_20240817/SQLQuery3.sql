-- 创建DoctorsInformation表
CREATE TABLE DoctorsInformation (
    DoctorId NVARCHAR(128) PRIMARY KEY,
    Name NVARCHAR(100),
    Major NVARCHAR(100),
    Description NVARCHAR(255),
    PhoneNumber NVARCHAR(20),
    FOREIGN KEY (DoctorId) REFERENCES AspNetUsers(Id)
);
GO

-- 创建DoctorRatings表
CREATE TABLE DoctorRatings (
    RatingId INT PRIMARY KEY IDENTITY,
    DoctorId NVARCHAR(128),
    UserId NVARCHAR(128),
    Score INT,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DoctorId) REFERENCES DoctorsInformation(DoctorId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
GO

-- 创建DoctorRatingsView视图
CREATE VIEW DoctorRatingsView AS
SELECT 
    DoctorId, 
    AVG(CAST(Score AS FLOAT)) AS AverageScore
FROM 
    DoctorRatings
GROUP BY 
    DoctorId;
GO