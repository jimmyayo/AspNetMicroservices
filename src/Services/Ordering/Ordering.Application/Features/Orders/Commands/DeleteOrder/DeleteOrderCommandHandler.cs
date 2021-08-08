using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
   public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
   {

      private readonly IOrderRepository _orderRepository;
      private readonly IMapper _mapper;
      private readonly ILogger<DeleteOrderCommandHandler> _logger;

      public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
      {
         _orderRepository = orderRepository;
         _mapper = mapper;
         _logger = logger;
      }

      public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
      {
         var order = await _orderRepository.GetByIdAsync(request.Id);
         if (order == null)
         {
            _logger.LogError($"OrderId {request.Id} does not exist in database.");
            throw new NotFoundException(nameof(Order), request.Id);
         }
         await _orderRepository.DeleteAsync(order);
         _logger.LogInformation($"OrderId {order.Id} has been successfully deleted.");

         return Unit.Value;
      }
   }
}
