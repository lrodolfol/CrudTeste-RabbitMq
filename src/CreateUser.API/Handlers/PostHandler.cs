using CreateUser.API.Model.Request;
using CreateUser.API.Model.Response;
using CreateUser.Core.Entities;
using CreateUser.Core.Exceptions;
using CreateUser.Infrastructure;
using CreateUser.Repository;
using MySql.Data.MySqlClient;

namespace CreateUser.API.Handlers;

public class PostHandler
{
    private readonly IUserPostRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly Serilog.ILogger _logger;

    public PostHandler(IUserPostRepository repository, IConfiguration configuration, Serilog.ILogger logger) =>
        (_repository, _configuration, _logger) = (repository, configuration, logger);

    public async Task<ViewResponse> Bootstrap(UserRequest request)
    {
        var viewResponse = new ViewResponse();

        try
        {
            User user = request;
            await SetPassword(user);
            _repository.Execute(user);

            UserResponse userResponse = user;
            viewResponse.AddData(userResponse);

            _logger.Information("user saved successfully");
        }
        catch (MessageBrockerException ex)
        {
            _logger.Error($"{ex.Message}");
            viewResponse.AddErros("Oops, we have a problem. Try again -> CODE 9b2e822b", 500);
        }
        catch (EmailException ex)
        {
            _logger.Error($"Invalid e-mail. {ex.Message}");
            viewResponse.AddErros("Check your e-mail!");
        }
        catch (DocumentException ex)
        {
            _logger.Error($"Invalid document. {ex.Message}");
            viewResponse.AddErros("Check your e-mail!");
        }
        catch (PhoneException ex)
        {
            _logger.Error($"Invalid phone. {ex.Message}");
            viewResponse.AddErros("Check your e-mail!");
        } 
        catch (MySqlException ex)
        {
            _logger.Error($"Database error! {ex.Message}");
            viewResponse.AddErros("Oops, we have a problem. Try again. -> CODE 2f3b087b", 500);
        }

        return viewResponse;
    }

    private async Task SetPassword(User user)
    {
        var rabbit = new RabbitMqClientePublish(_configuration, _logger);

        var userPassword = await rabbit.SetPassWord(user);
        user.SetPassword(userPassword);
    }
}