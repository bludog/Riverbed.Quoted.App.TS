using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Riverbed.Pricing.Paint.Controllers.Utils;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using Riverbed.Pricing.Paint.Shared.EmailService;
using EmailLogDto = Riverbed.Pricing.Paint.Controllers.Utils.EmailLogDto;
using Xunit;

namespace Riverbed.Pricing.Paint.Tests
{
    public class EmailServiceControllerTests : IDisposable
    {
        private readonly PricingDbContext _db;
        private readonly Mock<IEmailService> _mockEmailService;

        public EmailServiceControllerTests()
        {
            var options = new DbContextOptionsBuilder<PricingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new PricingDbContext(options);
            _mockEmailService = new Mock<IEmailService>();
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }

        private EmailServiceController CreateController()
            => new EmailServiceController(_db, _mockEmailService.Object);

        // ───────────────────────────────────────────────
        //  Send endpoint
        // ───────────────────────────────────────────────

        [Theory]
        [InlineData(null, "Subject", "Body")]
        [InlineData("", "Subject", "Body")]
        [InlineData("to@test.com", null, "Body")]
        [InlineData("to@test.com", "", "Body")]
        [InlineData("to@test.com", "Subject", null)]
        [InlineData("to@test.com", "Subject", "")]
        [InlineData("   ", "Subject", "Body")]
        public async Task Send_ReturnsBadRequest_WhenRequiredFieldsMissing(string to, string subject, string body)
        {
            var controller = CreateController();
            var request = new EmailRequest { ToAddress = to, Subject = subject, Body = body };

            var result = await controller.Send(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Missing required fields.", ((BadRequestObjectResult)result).Value);
        }

        [Fact]
        public async Task Send_Returns500_WhenEmailServiceFails()
        {
            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Error sending email: SMTP connection refused");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "to@test.com",
                Subject = "Test",
                Body = "Body",
                senderName = "Tester",
                senderEmail = "sender@test.com"
            };

            var result = await controller.Send(request);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task Send_ReturnsOk_AndLogsEmail_WhenSuccessful()
        {
            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var projectGuid = Guid.NewGuid();
            var companyGuid = Guid.NewGuid();
            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "to@test.com",
                Subject = "Test Subject",
                Body = "Test Body",
                senderName = "Tester",
                senderEmail = "sender@test.com",
                ProjectGuid = projectGuid.ToString(),
                CompanyGuid = companyGuid.ToString(),
                CompanyReportTypeId = 1
            };

            var result = await controller.Send(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email sent successfully.", okResult.Value);

            // Verify the email was logged
            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("to@test.com", log.ToEmail);
            Assert.Equal("Test Subject", log.Subject);
            Assert.Equal("Test Body", log.EmailBody);
            Assert.Equal(projectGuid, log.ProjectGuid);
            Assert.Equal(companyGuid, log.CompanyGuid);
            Assert.Equal("sender@test.com", log.FromEmail);
            Assert.Equal(1, log.CompanyReportTypeId);
        }

        [Fact]
        public async Task Send_UsesDefaultFromEmail_WhenSenderEmailIsEmpty()
        {
            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "to@test.com",
                Subject = "Test",
                Body = "Body",
                senderName = "Tester",
                senderEmail = ""
            };

            await controller.Send(request);

            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("quotes@riverbed-software.com", log.FromEmail);
        }

        [Fact]
        public async Task Send_HandlesInvalidGuids_Gracefully()
        {
            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "to@test.com",
                Subject = "Test",
                Body = "Body",
                senderName = "Tester",
                senderEmail = "sender@test.com",
                ProjectGuid = "not-a-guid",
                CompanyGuid = "also-not-a-guid"
            };

            var result = await controller.Send(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Null(log.ProjectGuid);
            Assert.Null(log.CompanyGuid);
        }

        // ───────────────────────────────────────────────
        //  SendProject endpoint
        // ───────────────────────────────────────────────

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SendProject_ReturnsBadRequest_WhenProjectGuidEmpty(string projectGuid)
        {
            var controller = CreateController();
            var request = new EmailRequest { Subject = "S", Body = "B" };

            var result = await controller.SendProject(projectGuid, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SendProject_ReturnsBadRequest_WhenSubjectOrBodyMissing()
        {
            var controller = CreateController();
            var request = new EmailRequest { Subject = "", Body = "Body" };

            var result = await controller.SendProject(Guid.NewGuid().ToString(), request);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Subject and Body are required.", badResult.Value);
        }

        [Fact]
        public async Task SendProject_ReturnsBadRequest_WhenGuidInvalid()
        {
            var controller = CreateController();
            var request = new EmailRequest { Subject = "S", Body = "B" };

            var result = await controller.SendProject("not-a-guid", request);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid project guid.", badResult.Value);
        }

        [Fact]
        public async Task SendProject_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            var controller = CreateController();
            var request = new EmailRequest { Subject = "S", Body = "B" };

            var result = await controller.SendProject(Guid.NewGuid().ToString(), request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Project not found.", notFound.Value);
        }

        [Fact]
        public async Task SendProject_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            var projectGuid = Guid.NewGuid();
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = Guid.NewGuid(), // No matching customer
                ProjectName = "Test",
                CustomerEmail = "c@test.com",
                CustomerPhone = "555-0000",
                Address1 = "123 Main",
                City = "Test",
                StateCode = "TX",
                ZipCode = "75001",
                Summary = "Test",
                ScopeOfWork = "Test"
            });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var request = new EmailRequest { Subject = "S", Body = "B" };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Customer not found for project.", notFound.Value);
        }

        [Fact]
        public async Task SendProject_ReturnsBadRequest_WhenNoToAddressResolvable()
        {
            var companyGuid = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var projectGuid = Guid.NewGuid();

            _db.CompanyCustomers.Add(new CompanyCustomer
            {
                CustomerId = customerId,
                CompanyId = companyGuid,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            });
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = customerId,
                ProjectName = "Test",
                CustomerEmail = "",
                CustomerPhone = "555-0000",
                Address1 = "123 Main",
                City = "Test",
                StateCode = "TX",
                ZipCode = "75001",
                Summary = "Test",
                ScopeOfWork = "Test"
            });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = null, // No explicit ToAddress either
                Subject = "S",
                Body = "B"
            };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ToAddress is required", badResult.Value?.ToString());
        }

        [Fact]
        public async Task SendProject_ReturnsOk_AndLogsWithResolvedGuids()
        {
            var companyGuid = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var projectGuid = Guid.NewGuid();

            _db.CompanyCustomers.Add(new CompanyCustomer
            {
                CustomerId = customerId,
                CompanyId = companyGuid,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@test.com"
            });
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = customerId,
                ProjectName = "Test Project",
                CustomerEmail = "project-email@test.com",
                CustomerPhone = "555-0001",
                Address1 = "456 Oak",
                City = "Dallas",
                StateCode = "TX",
                ZipCode = "75002",
                Summary = "Test",
                ScopeOfWork = "Test"
            });
            await _db.SaveChangesAsync();

            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "explicit@test.com",
                Subject = "Quote",
                Body = "<p>Hello</p>",
                senderName = "Sales",
                senderEmail = "sales@company.com",
                BccEmail = "bcc@test.com"
            };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email sent successfully.", okResult.Value);

            // Verify email was sent with explicit ToAddress
            _mockEmailService.Verify(s => s.SendEmail(
                "explicit@test.com", "Quote", "<p>Hello</p>",
                "Sales", "sales@company.com", null, "bcc@test.com", true), Times.Once);

            // Verify log has resolved company guid
            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal(projectGuid, log.ProjectGuid);
            Assert.Equal(companyGuid, log.CompanyGuid);
            Assert.Equal("explicit@test.com", log.ToEmail);
            Assert.Equal("bcc@test.com", log.BccEmail);
        }

        [Fact]
        public async Task SendProject_UsesProjectEmail_WhenToAddressNotProvided()
        {
            var companyGuid = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var projectGuid = Guid.NewGuid();

            _db.CompanyCustomers.Add(new CompanyCustomer
            {
                CustomerId = customerId,
                CompanyId = companyGuid,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@test.com"
            });
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = customerId,
                ProjectName = "Test",
                CustomerEmail = "fallback@test.com",
                CustomerPhone = "555-0002",
                Address1 = "789 Pine",
                City = "Austin",
                StateCode = "TX",
                ZipCode = "73301",
                Summary = "Test",
                ScopeOfWork = "Test"
            });
            await _db.SaveChangesAsync();

            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = null, // Not provided — should fall back to project.CustomerEmail
                Subject = "S",
                Body = "B"
            };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            Assert.IsType<OkObjectResult>(result);
            _mockEmailService.Verify(s => s.SendEmail(
                "fallback@test.com", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true), Times.Once);
        }

        [Fact]
        public async Task SendProject_Returns500_WhenEmailFails()
        {
            var companyGuid = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var projectGuid = Guid.NewGuid();

            _db.CompanyCustomers.Add(new CompanyCustomer
            {
                CustomerId = customerId,
                CompanyId = companyGuid,
                FirstName = "X",
                LastName = "Y",
                Email = "x@test.com"
            });
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = customerId,
                ProjectName = "T",
                CustomerEmail = "e@test.com",
                CustomerPhone = "555-0003",
                Address1 = "1",
                City = "C",
                StateCode = "TX",
                ZipCode = "00000",
                Summary = "T",
                ScopeOfWork = "T"
            });
            await _db.SaveChangesAsync();

            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Error: SMTP timeout");

            var controller = CreateController();
            var request = new EmailRequest { ToAddress = "t@t.com", Subject = "S", Body = "B" };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        // ───────────────────────────────────────────────
        //  Multiple recipients
        // ───────────────────────────────────────────────

        [Fact]
        public async Task Send_ReturnsOk_AndLogsEmail_WhenMultipleToAndCcRecipients()
        {
            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "alice@test.com;bob@test.com;carol@test.com",
                Subject = "Multi-recipient test",
                Body = "Hello everyone",
                senderName = "Sender",
                senderEmail = "sender@test.com",
                CcEmail = "manager@test.com;director@test.com"
            };

            var result = await controller.Send(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email sent successfully.", okResult.Value);

            _mockEmailService.Verify(s => s.SendEmail(
                "alice@test.com;bob@test.com;carol@test.com",
                "Multi-recipient test",
                "Hello everyone",
                "Sender",
                "sender@test.com",
                "manager@test.com;director@test.com",
                It.IsAny<string>(),
                true), Times.Once);

            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("alice@test.com;bob@test.com;carol@test.com", log.ToEmail);
            Assert.Equal("manager@test.com;director@test.com", log.CcEmail);
        }

        [Fact]
        public async Task SendProject_ReturnsOk_AndLogsEmail_WhenMultipleToAndCcRecipients()
        {
            var companyGuid = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var projectGuid = Guid.NewGuid();

            _db.CompanyCustomers.Add(new CompanyCustomer
            {
                CustomerId = customerId,
                CompanyId = companyGuid,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@test.com"
            });
            _db.Projects.Add(new ProjectData
            {
                ProjectGuid = projectGuid,
                CompanyCustomerGuidId = customerId,
                ProjectName = "Multi-CC Project",
                CustomerEmail = "customer@test.com",
                CustomerPhone = "555-0010",
                Address1 = "100 Main",
                City = "Dallas",
                StateCode = "TX",
                ZipCode = "75001",
                Summary = "Test",
                ScopeOfWork = "Test"
            });
            await _db.SaveChangesAsync();

            _mockEmailService
                .Setup(s => s.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns("Email sent successfully.");

            var controller = CreateController();
            var request = new EmailRequest
            {
                ToAddress = "alice@test.com;bob@test.com",
                Subject = "Project Quote",
                Body = "<p>Please review</p>",
                senderName = "Sales",
                senderEmail = "sales@company.com",
                CcEmail = "pm@test.com;finance@test.com;legal@test.com",
                BccEmail = "archive@test.com"
            };

            var result = await controller.SendProject(projectGuid.ToString(), request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email sent successfully.", okResult.Value);

            _mockEmailService.Verify(s => s.SendEmail(
                "alice@test.com;bob@test.com",
                "Project Quote",
                "<p>Please review</p>",
                "Sales",
                "sales@company.com",
                "pm@test.com;finance@test.com;legal@test.com",
                "archive@test.com",
                true), Times.Once);

            var log = await _db.EmailLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("alice@test.com;bob@test.com", log.ToEmail);
            Assert.Equal("pm@test.com;finance@test.com;legal@test.com", log.CcEmail);
            Assert.Equal("archive@test.com", log.BccEmail);
            Assert.Equal(projectGuid, log.ProjectGuid);
            Assert.Equal(companyGuid, log.CompanyGuid);
        }

        // ───────────────────────────────────────────────
        //  GetProjectEmailLogs endpoint
        // ───────────────────────────────────────────────

        [Fact]
        public async Task GetProjectEmailLogs_ReturnsBadRequest_WhenGuidInvalid()
        {
            var controller = CreateController();

            var result = await controller.GetProjectEmailLogs("not-valid");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetProjectEmailLogs_ReturnsEmptyList_WhenNoLogs()
        {
            var controller = CreateController();

            var result = await controller.GetProjectEmailLogs(Guid.NewGuid().ToString());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsAssignableFrom<List<EmailLogDto>>(okResult.Value);
            Assert.Empty(logs);
        }

        [Fact]
        public async Task GetProjectEmailLogs_ReturnsLogs_OrderedByTimeSentDescending()
        {
            var projectGuid = Guid.NewGuid();
            _db.EmailLogs.AddRange(
                new EmailLog
                {
                    TimeSent = new DateTime(2025, 1, 1),
                    Subject = "First",
                    EmailBody = "B1",
                    ToEmail = "a@test.com",
                    FromEmail = "f@test.com",
                    ProjectGuid = projectGuid
                },
                new EmailLog
                {
                    TimeSent = new DateTime(2025, 6, 1),
                    Subject = "Second",
                    EmailBody = "B2",
                    ToEmail = "b@test.com",
                    FromEmail = "f@test.com",
                    ProjectGuid = projectGuid
                },
                new EmailLog
                {
                    TimeSent = new DateTime(2025, 3, 1),
                    Subject = "Third",
                    EmailBody = "B3",
                    ToEmail = "c@test.com",
                    FromEmail = "f@test.com",
                    ProjectGuid = projectGuid
                });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var result = await controller.GetProjectEmailLogs(projectGuid.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsAssignableFrom<List<EmailLogDto>>(okResult.Value);
            Assert.Equal(3, logs.Count);
            Assert.Equal("Second", logs[0].Subject);  // June (most recent)
            Assert.Equal("Third", logs[1].Subject);   // March
            Assert.Equal("First", logs[2].Subject);   // January
        }

        [Fact]
        public async Task GetProjectEmailLogs_DoesNotReturnOtherProjectLogs()
        {
            var targetGuid = Guid.NewGuid();
            var otherGuid = Guid.NewGuid();

            _db.EmailLogs.AddRange(
                new EmailLog { Subject = "Mine", EmailBody = "B", ToEmail = "a@t.com", FromEmail = "f@t.com", ProjectGuid = targetGuid },
                new EmailLog { Subject = "Other", EmailBody = "B", ToEmail = "b@t.com", FromEmail = "f@t.com", ProjectGuid = otherGuid });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var result = await controller.GetProjectEmailLogs(targetGuid.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsAssignableFrom<List<EmailLogDto>>(okResult.Value);
            Assert.Single(logs);
            Assert.Equal("Mine", logs[0].Subject);
        }

        [Fact]
        public async Task GetProjectEmailLogs_ResolvesEmailTypeName_FromCompanyReportType()
        {
            var projectGuid = Guid.NewGuid();
            _db.CompanyReportTypes.Add(new CompanyReportType { Id = 10, ReportTypeName = "Quote", ReportTypeDescription = "Quote report" });
            _db.EmailLogs.Add(new EmailLog
            {
                Subject = "Your Quote",
                EmailBody = "B",
                ToEmail = "c@t.com",
                FromEmail = "f@t.com",
                ProjectGuid = projectGuid,
                CompanyReportTypeId = 10
            });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var result = await controller.GetProjectEmailLogs(projectGuid.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsAssignableFrom<List<EmailLogDto>>(okResult.Value);
            Assert.Single(logs);
            Assert.Equal("Quote", logs[0].EmailTypeName);
        }

        [Fact]
        public async Task GetProjectEmailLogs_EmailTypeName_IsNull_WhenNoReportTypeId()
        {
            var projectGuid = Guid.NewGuid();
            _db.EmailLogs.Add(new EmailLog
            {
                Subject = "No Type",
                EmailBody = "B",
                ToEmail = "c@t.com",
                FromEmail = "f@t.com",
                ProjectGuid = projectGuid,
                CompanyReportTypeId = null
            });
            await _db.SaveChangesAsync();

            var controller = CreateController();
            var result = await controller.GetProjectEmailLogs(projectGuid.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var logs = Assert.IsAssignableFrom<List<EmailLogDto>>(okResult.Value);
            Assert.Single(logs);
            Assert.Null(logs[0].EmailTypeName);
        }
    }
}