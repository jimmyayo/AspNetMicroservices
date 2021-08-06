﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
   public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
   {

      private readonly IOrderRepository _orderRepository;
      private readonly IMapper _mapper;
      private readonly ILogger<UpdateOrderCommandHandler> _logger;

      public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<UpdateOrderCommandHandler> logger)
      {
         _orderRepository = orderRepository;
         _mapper = mapper;
         _logger = logger;
      }

      public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
      {
         var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
         if (orderToUpdate == null)
         {
            _logger.LogError("Order does not exist in database.");
            throw new NotFoundException(nameof(Order), request.Id);
         }

         // copy values over from our request DTO to entity orderToUpdate
         _mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));
         await _orderRepository.UpdateAsync(orderToUpdate);
         _logger.LogInformation($"OrderId {orderToUpdate} has been successfully updated.");

         return Unit.Value;
      }
   }
}
