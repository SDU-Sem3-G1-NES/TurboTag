using MongoDB.Bson;
using API.DataAccess;
using API.Repositories;
using API.DTOs;

namespace API.Tests.Repositories
{
    public class LessonRepositoryTests
    {
        private readonly LessonRepository _repository;
        private readonly Mock<IMongoDataAccess> _mockDb;

        public LessonRepositoryTests()
        {
            _mockDb = new Mock<IMongoDataAccess>();
            _repository = new LessonRepository(_mockDb.Object);
        }

        [Fact]
        public void AddLesson_AddsLessonSuccessfully()
        {
            // Arrange
            var lesson = new LessonDto(
                1,
                new LessonDetailsDto(1, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                new List<FileMetadataDto>(),
                1,
                "Sample MongoId"
            );

            // Act
            _repository.AddLesson(lesson);

            // Assert
            _mockDb.Verify(db => db.Insert("lesson", It.IsAny<BsonDocument>()), Times.Once);
        }

        [Fact]
        public void GetAllLessons_ReturnsAllLessons()
        {
            // Arrange
            var lessons = new List<LessonDto>
            {
                new LessonDto(
                    1,
                    new LessonDetailsDto(1, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                    new List<FileMetadataDto>(),
                    1,
                    "Sample MongoId"
                )
            };
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", "{}", null, null)).Returns(lessons);

            // Act
            var result = _repository.GetAllLessons();

            // Assert
            Assert.Equal(lessons, result);
        }

        [Fact]
        public void GetLessonsByTags_ReturnsLessonsWithTags()
        {
            // Arrange
            var tags = new[] { "tag1", "tag2" };
            var lessons = new List<LessonDto>
            {
                new LessonDto(
                    1,
                    new LessonDetailsDto(1, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                    new List<FileMetadataDto>(),
                    1,
                    "Sample MongoId"
                )
            };
            var filter = $"{{\"lesson_details.tags\": {{$all: [{string.Join(",", tags.Select(tag => $"\"{tag}\""))}]}}}}";
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", filter, null, null)).Returns(lessons);

            // Act
            var result = _repository.GetLessonsByTags(tags);

            // Assert
            Assert.Equal(lessons, result);
        }

        [Fact]
        public void GetLessonsByTitle_ReturnsLessonsWithTitle()
        {
            // Arrange
            var title = "Sample Title";
            var lessons = new List<LessonDto>
            {
                new LessonDto(
                    1,
                    new LessonDetailsDto(1, title, "Description", new List<string> { "tag1", "tag2" }),
                    new List<FileMetadataDto>(),
                    1,
                    "Sample MongoId"
                )
            };
            var filter = $"{{\"lesson_details.title\": {{$regex: \"{title}\", $options: \"i\"}}}}";
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", filter, null, null)).Returns(lessons);

            // Act
            var result = _repository.GetLessonsByTitle(title);

            // Assert
            Assert.Equal(lessons, result);
        }

        [Fact]
        public void GetLessonByObjectId_ReturnsLessonWithValidObjectId()
        {
            // Arrange
            var objectId = ObjectId.GenerateNewId().ToString();
            var lesson = new LessonDto(
                1,
                new LessonDetailsDto(1, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                new List<FileMetadataDto>(),
                1,
                objectId
            );
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", $"{{\"_id\": ObjectId(\"{objectId}\")}}", null, null)).Returns(new List<LessonDto> { lesson });

            // Act
            var result = _repository.GetLessonByObjectId(objectId);

            // Assert
            Assert.Equal(lesson, result);
        }

        [Fact]
        public void GetLessonById_ReturnsLessonWithValidId()
        {
            // Arrange
            var lessonId = 1;
            var lesson = new LessonDto(
                1,
                new LessonDetailsDto(lessonId, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                new List<FileMetadataDto>(),
                1,
                "Sample MongoId"
            );
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", $"{{\"lesson_details._id\": {lessonId}}}", null, null)).Returns(new List<LessonDto> { lesson });

            // Act
            var result = _repository.GetLessonById(lessonId);

            // Assert
            Assert.Equal(lesson, result);
        }

        [Fact]
        public void GetLessonByUploadId_ReturnsLessonWithValidUploadId()
        {
            // Arrange
            var uploadId = 1;
            var lesson = new LessonDto(
                1,
                new LessonDetailsDto(1, "Sample Lesson", "Description", new List<string> { "tag1", "tag2" }),
                new List<FileMetadataDto>(),
                1,
                "Sample MongoId"
            );
            _mockDb.Setup(db => db.Find<LessonDto>("lesson", $"{{upload_id: {uploadId}}}", null, null)).Returns(new List<LessonDto> { lesson });

            // Act
            var result = _repository.GetLessonByUploadId(uploadId);

            // Assert
            Assert.Equal(lesson, result);
        }

        [Fact]
        public void UpdateLesson_UpdatesLessonSuccessfully()
        {
            // Arrange
            var lesson = new LessonDto(
                1,
                new LessonDetailsDto(1, "Updated Lesson", "Description", new List<string> { "tag1", "tag2" }),
                new List<FileMetadataDto>(),
                1,
                ObjectId.GenerateNewId().ToString()
            );

            // Act
            _repository.UpdateLesson(lesson);

            // Assert
            _mockDb.Verify(db => db.Replace("lesson", $"{{\"_id\": ObjectId(\"{lesson.MongoId}\")}}", lesson), Times.Once);
        }
    }
}