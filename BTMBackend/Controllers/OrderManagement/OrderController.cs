using BTMBackend.Data;
using BTMBackend.Data.Repos;
using BTMBackend.Dtos.CustomerDto;
using BTMBackend.Dtos.OrderDto;
using BTMBackend.Dtos.OrderItemDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Dtos.SMSGatewayDto;
using BTMBackend.Models;
using BTMBackend.SyncDataServices.Http.SMGateway;
using BTMBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace BTMBackend.Controllers.OrderManagement
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(ICustomerRepo customerRepo, INotificationRepo notificationRepo , IConfiguration configuration, IEmployeeRepo employeeRepo, ICustomerProductPartRepo customerProductPartRepo, ICustomerProductRepo customerProductRepo, IPartRepo partRepo, IProductRepo productRepo, IOTPMessageRepo oTPMessageRepo, ISMSService sMSService, UploadFileService uploadFileService, IOrderItemRepo orderItemRepo, IOrderRepo orderRepo, IAuthorityRepo authorityRepo, IEncryptRepo encryptRepo, IUserRepo userRepo) : ControllerBase
    {
        private readonly ICustomerRepo _customerRepo = customerRepo;
        private readonly IOrderRepo _orderRepo = orderRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IEncryptRepo _encryptRepo = encryptRepo;
        private readonly IAuthorityRepo _authorityRepo = authorityRepo;
        private readonly IOTPMessageRepo _otpMessageRepo = oTPMessageRepo;
        private readonly IOrderItemRepo _orderItemRepo = orderItemRepo;
        private readonly IProductRepo _productRepo = productRepo;
        private readonly IPartRepo _partRepo = partRepo;
        private readonly IEmployeeRepo _employeeRepo = employeeRepo;
        private readonly ICustomerProductRepo _customerProductRepo = customerProductRepo;
        private readonly ICustomerProductPartRepo _customerProductPartRepo = customerProductPartRepo;
        private readonly UploadFileService _uploadFileService = uploadFileService;
        private readonly ISMSService _sMSService = sMSService;
        private readonly IConfiguration _configuration = configuration;
        private readonly INotificationRepo _notificationRepo = notificationRepo;
        private readonly Messages ms = new();


        [AllowAnonymous]
        [HttpPost("MakeAnOrder")]
        public async Task<IActionResult> MakeAnOrder(MakeOrderRequestDto requestDto)
        {
            var checkCustomerExistince = await _customerRepo.GetByPhoneNumber(requestDto.PhoneNumber);
            int customerId;
            int userId;
            int roleId = await _authorityRepo.GetCustomerRoleId();
            if (roleId == 0)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }
            if (checkCustomerExistince == null)
            {
                var generatedPassword = _encryptRepo.EncryptPassword(RandomCodeGenerator.GeneratePassword());
                var user = new User()
                {
                    Username = requestDto.PhoneNumber,
                    Password = generatedPassword,
                    RoleId = roleId
                };
                await _userRepo.Create(user);
                if (!await _userRepo.SaveChanges())
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
                userId = user.Id;

                var customer = new Customer()
                {
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.PhoneNumber,
                    County = requestDto.CountyId,
                    City = requestDto.CityId,
                    Address = requestDto.Address,
                    UserId = userId

                };
                await _customerRepo.Create(customer);
                if (!await _customerRepo.SaveChanges())
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
                customerId = customer.Id;

                var getMessage = SMSMessages.TempPassword(generatedPassword);
                if (getMessage == null)
                {
                    return BadRequest();
                }

                List<string> listofData = [];
                listofData.Add(requestDto.PhoneNumber);

                var messageData = new SendRequestDto()
                {
                    Source = "BRONZE NET",
                    Message = getMessage,
                    Destination = listofData
                };

                var apiKey = _configuration["SMSAPIKey"];
                if (apiKey == null)
                {
                    return Forbid();
                }

                await _sMSService.SendMessage(messageData, apiKey);

            }
            else
            {
                customerId = checkCustomerExistince.Id;
            }

            await _orderRepo.Create(new Order()
            {
                ServiceType = requestDto.SericeTypeId,
                CountyId = requestDto.CountyId,
                CityId = requestDto.CityId,
                Address = requestDto.Address,
                Message = requestDto.Message,
                Status = 0,
                CustomerId = customerId,
            });

            if (!await _orderRepo.SaveChanges())
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn,
                });
            }

            // Notifiy Supervisors
            var Supervisors = await _userRepo.GetAllSupervisorId();
            if(Supervisors.Count > 0)
            {
                List<Notification> notification = [];
                foreach (var supervisor in Supervisors)
                {
                    notification.Add(new Notification()
                    {
                        ContentAr = " طلب جديد من العميل " + requestDto.FirstName + " " + requestDto.LastName,
                        ContentEn = "New Order From " + requestDto.FirstName + " " + requestDto.LastName,
                        ReceiverId = supervisor
                    });
                }
                await _notificationRepo.Create(notification);
            }



            return Ok(new MessageDto()
            {
                MessageAr = ms.AddedSuccessfullyAr,
                MessageEn = ms.AddedSuccessfullyEn,
            });
        }


        [HttpGet("GetCustomerHistoryOperations")]
        public async Task<ActionResult<GetCustomerOrderHistoryResponseDto>> GetCustomerHistoryOperations(int page = 1, int row = 10)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);
            var customer = await _customerRepo.GetCustomerId(userId);

            if (customer == null)
            {
                return Forbid();
            }

            var result = await _orderRepo.GetCustomerHistoryOperationsById(customer);
            var list = PagedList<GetCustomerOrderHistoryResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [Authorize(Roles = "Supervisor")]
        [HttpGet("GetSupervisorOperations")]
        public async Task<ActionResult<GetSupervisorOperationsResponseDto>> GetSupervisorOperations(int page = 1, int row = 10)
        {
            var result = await _orderRepo.GetSupervisorOperations();

            var list = PagedList<GetSupervisorOperationsResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [HttpGet("GetCallCenterOrders")]
        public async Task<ActionResult<GetCallCenterOrdersResponseDto>> GetCallCenterOrders(int page = 1, int row = 10)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            var result = await _orderRepo.GetCallCenterOrders(fromEmployeeId);

            var list = PagedList<GetCallCenterOrdersResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [Authorize(Roles = "Technician")]
        [HttpGet("GetTechnicianOrders")]
        public async Task<ActionResult<GetTechnicianOrdersResponseDto>> GetTechnicianOrders(int page = 1, int row = 10)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Convert.ToInt32(userIdClaim.Value);

            var employee = await _employeeRepo.GetByUserId(userId);
            if(employee == null)
            {
                return BadRequest();
            }
            var result = await _orderRepo.GetTechnicianOrder(employee.Id);

            var list = PagedList<GetTechnicianOrdersResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }

        [HttpGet("GetCustomerOrders")]
        public async Task<ActionResult<GetCustomerInProgressOrdersResponseDto>> GetCustomerOrders(int page = 1, int row = 10)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var customerId = await _customerRepo.GetCustomerId(userId);

            if (customerId == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var result = await _orderRepo.GetCustomerOrders(userId);

            return Ok(result);
        }

        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<GetAllOrdersResponseDto>> GetAllOrders(int page = 1, int row = 10)
        {
            var result = await _orderRepo.GetAllOrders();

            var list = PagedList<GetAllOrdersResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }


        [HttpGet("GetOrderHistoryByOrderId/{orderId}")]
        public async Task<ActionResult<GetOrderHistoryResponseDto>> GetOrderHistoryByOrderId(int orderId)
        {
            var result = await _orderRepo.GetOrderHistoryByOrderId(orderId);
            return Ok(result);
        }


        [HttpGet("GetOrderItemsByOrderId/{orderId}")]
        public async Task<ActionResult<GetOrderItemsResponseDto>> GetOrderItemsByOrderId(int orderId)
        {
            var result = await _orderRepo.GetOrderItemsByOrderId(orderId);
            return Ok(result);
        }


        [Authorize(Roles = "Supervisor")]
        [HttpPut("SendToCallCenter")]
        public async Task<IActionResult> SendToCallCenter(ListOfOrderIdRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            var updateOrderRequestDto = new UpdateMultiOrderRequestDto
            {
                Id = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = requestDto.EmployeeId,
                Note = requestDto.Note
            };

            var checkResult = await _orderRepo.SendToCallCenter(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [Authorize(Roles = "Call Center")]
        [HttpPut("UpdateItemsToOrder")]
        public async Task<IActionResult> UpdateItemsToOrder(OrderItemsRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            if (getOrder.Status != 1 || fromEmployeeId != getOrder.EmployeeId)
            {
                return Forbid();
            }

            var checkOrder = await _orderRepo.IsExist(requestDto.OrderId);
            if (!checkOrder)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var checkItemOrder = await _orderItemRepo.CheckIfDataExistByOrderId(requestDto.OrderId);
            if (checkItemOrder)
            {
                var deleteExisting = await _orderItemRepo.Delete(requestDto.OrderId);
                if (!deleteExisting)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedToDeleteAr,
                        MessageEn = ms.FailedToDeleteEn
                    });
                }

            }

            var addOrderItems = await _orderItemRepo.Add(requestDto);
            if (!addOrderItems)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto()
            {
                MessageAr = ms.AddedSuccessfullyAr,
                MessageEn = ms.AddedSuccessfullyEn
            });
        }


        [Authorize(Roles = "Call Center")]
        [HttpPut("SendToTechnician")]
        public async Task<IActionResult> SendToTechnician(SendToTechnicianRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            if (getOrder.Status != 1 || fromEmployeeId != getOrder.EmployeeId)
            {
                return Forbid();
            }

            var updateOrderRequestDto = new UpdateToSpecilistRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = requestDto.ToEmployeeId,
                Note = requestDto.Note,
                OperationDateTime = requestDto.OperationDateTime
            };

            var checkResult = await _orderRepo.SendToTechnician(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [HttpPut("CancelOrder")]
        public async Task<IActionResult> CancelOrder(UpdateOrderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            if (getOrder.Status != 2 && getOrder.Status != 4)
            {
                return Forbid();
            }


            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = fromEmployeeId,
                Note = requestDto.Note,
                Status = 5,
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [Authorize(Roles = "Call Center")]
        [HttpPut("PrepaireCancelOrderByCallCenter")]
        public async Task<IActionResult> PrepaireCancelOrderByCallCenter(UpdateOrderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var getSuperVisorId = await _orderRepo.GetSuperVisorIdFromHistory(getOrder.Id);
            if (getSuperVisorId == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            if (getOrder.Status != 1 || fromEmployeeId != getOrder.EmployeeId)
            {
                return Forbid();
            }


            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = getSuperVisorId,
                Note = requestDto.Note,
                Status = 2,
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [HttpPut("PrepaireCancelOrderByTechnician")]
        public async Task<IActionResult> PrepaireCancelOrderByTechnician(UpdateOrderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }


            var getSuperVisorId = await _orderRepo.GetSuperVisorIdFromHistory(getOrder.Id);
            if (getSuperVisorId == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }



            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            if (getOrder.Status != 3 || fromEmployeeId != getOrder.EmployeeId)
            {
                return Forbid();
            }

            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = getSuperVisorId,
                Note = requestDto.Note,
                Status = 4,
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [Authorize(Roles = "Supervisor")]
        [HttpPut("CloseOrderBySupervisor")]
        public async Task<IActionResult> CloseOrderBySupervisor(CloseOrderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            if (getOrder.Status != 7)
            {
                return Forbid();
            }

            var getOrderItems = await _orderItemRepo.GetByOrderId(requestDto.OrderId);
            if (getOrderItems == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var customerProductPartsToCreate = new List<CustomerProductPart>();

            foreach (var orderItem in getOrderItems)
            {
                if (orderItem.ItemType == 1 && orderItem.ServiceType == 1)
                {
                    var product = await _productRepo.GetByIdAsync(orderItem.ItemId);
                    if (product == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    var expirationDate = DateTime.Now.AddDays(product.WarrantyDuration);

                    var customerProduct = new CustomerProduct
                    {
                        ProductId = orderItem.ItemId,
                        CustomerId = getOrder.CustomerId,
                        OrderId = getOrder.Id,
                        ExpirationDate = expirationDate
                    };

                    await _customerProductRepo.Create(customerProduct);

                    var productParts = await _partRepo.GetByProductId(product.Id);
                    if (productParts == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    foreach (var productPart in productParts)
                    {
                        var customerProductPart = new CustomerProductPart
                        {
                            CustomerProductId = customerProduct.Id,
                            MaintenanceDate = DateTime.Now.AddDays(productPart.MaintenanceDuration),
                            OrderId = getOrder.Id,
                            PartId = productPart.Id
                        };

                        customerProductPartsToCreate.Add(customerProductPart);
                    }
                }
                else if (orderItem.ItemType == 2 && orderItem.ServiceType == 2)
                {
                    if (orderItem.CustomerPartId == 0)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    var customerProductPart = await _customerProductPartRepo.GetById(orderItem.CustomerPartId);
                    var productPart = await _partRepo.GetByIdAsync(orderItem.ItemId);
                    if (customerProductPart == null || productPart == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    customerProductPart.MaintenanceDate = customerProductPart.MaintenanceDate.AddDays(productPart.MaintenanceDuration);
                    customerProductPart.UpdatedAt = DateTime.Now;
                    var result = await _customerProductPartRepo.Update(customerProductPart);
                    if (!result)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }
                }
            }

            if (customerProductPartsToCreate.Count > 0)
            {
                var createResult = await _customerProductPartRepo.CreateRange([.. customerProductPartsToCreate]);
                if (!createResult)
                {
                    return BadRequest(new MessageDto
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
            }

            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = fromEmployeeId,
                Note = "تم اغلاق الطلب",
                Status = 8
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }

        [Authorize(Roles = "Technician")]
        [HttpPut("CloseOrderByTechnician")]
        public async Task<IActionResult> CloseOrderByTechnician(CloseByTechnicianRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            if (getOrder.Status != 6 || fromEmployeeId != getOrder.EmployeeId)
            {
                return Forbid();
            }

            var customerPhoneNumber = await _customerRepo.GetCustomerPhoneById(getOrder.CustomerId);
            if (customerPhoneNumber == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var checkOTP = await _otpMessageRepo.CheckIfValid(requestDto.OTPCode, customerPhoneNumber);

            if (!checkOTP)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.WrongOtpAr,
                    MessageEn = ms.WrongOtpEn
                });
            }
            await _otpMessageRepo.Delete(requestDto.OTPCode, customerPhoneNumber);

            var saveChages = await _otpMessageRepo.SaveChanges();
            if (!saveChages)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var getOrderItems = await _orderItemRepo.GetByOrderId(requestDto.OrderId);
            if (getOrderItems == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            var customerProductPartsToCreate = new List<CustomerProductPart>();

            foreach (var orderItem in getOrderItems)
            {
                if (orderItem.ItemType == 1 && orderItem.ServiceType == 1)
                {
                    var product = await _productRepo.GetByIdAsync(orderItem.ItemId);
                    if (product == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    var expirationDate = DateTime.Now.AddDays(product.WarrantyDuration);

                    var customerProduct = new CustomerProduct
                    {
                        ProductId = orderItem.ItemId,
                        CustomerId = getOrder.CustomerId,
                        OrderId = getOrder.Id,
                        ExpirationDate = expirationDate
                    };

                    await _customerProductRepo.Create(customerProduct);

                    var productParts = await _partRepo.GetByProductId(product.Id);
                    if (productParts == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    foreach (var productPart in productParts)
                    {
                        var customerProductPart = new CustomerProductPart
                        {
                            CustomerProductId = customerProduct.Id,
                            MaintenanceDate = DateTime.Now.AddDays(productPart.MaintenanceDuration),
                            OrderId = getOrder.Id,
                            PartId = productPart.Id
                        };

                        customerProductPartsToCreate.Add(customerProductPart);
                    }
                }
                else if (orderItem.ItemType == 2 && orderItem.ServiceType == 2)
                {
                    if (orderItem.CustomerPartId == 0)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    var customerProductPart = await _customerProductPartRepo.GetById(orderItem.CustomerPartId);
                    var productPart = await _partRepo.GetByIdAsync(orderItem.ItemId);
                    if (customerProductPart == null || productPart == null)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }

                    customerProductPart.MaintenanceDate = customerProductPart.MaintenanceDate.AddDays(productPart.MaintenanceDuration);
                    customerProductPart.UpdatedAt = DateTime.Now;
                    var result = await _customerProductPartRepo.Update(customerProductPart);
                    if (!result)
                    {
                        return BadRequest(new MessageDto
                        {
                            MessageAr = ms.FailedAr,
                            MessageEn = ms.FailedEn
                        });
                    }
                }
            }

            if (customerProductPartsToCreate.Count > 0)
            {
                var createResult = await _customerProductPartRepo.CreateRange([.. customerProductPartsToCreate]);
                if (!createResult)
                {
                    return BadRequest(new MessageDto
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
            }

            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = fromEmployeeId,
                Note = "تم اغلاق الطلب",
                Status = 8,
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [HttpPut("PrepaireToCloseOrder")]
        public async Task<IActionResult> PrepaireToCloseOrder(UpdateOrderRequestDto requestDto)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var getOrder = await _orderRepo.GetById(requestDto.OrderId);
            if (getOrder == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }


            var getSuperVisorId = await _orderRepo.GetSuperVisorIdFromHistory(getOrder.Id);
            if (getSuperVisorId == null)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (fromEmployeeId == null)
            {
                return Unauthorized();
            }
            var updateOrderRequestDto = new UpdateSingleOrderRequestDto
            {
                OrderId = requestDto.OrderId,
                FromEmployeeId = fromEmployeeId,
                ToEmployeeId = getSuperVisorId,
                Note = requestDto.Note,
                Status = 7,
            };

            var checkResult = await _orderRepo.UpdateStatus(updateOrderRequestDto);
            if (!checkResult)
            {
                return BadRequest(new MessageDto
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ms.FailedEn
                });
            }

            return Ok(new MessageDto
            {
                MessageAr = ms.UpdatedSuccessfullyAr,
                MessageEn = ms.UpdatedSuccessfullyEn
            });
        }


        [Authorize(Roles = "Technician")]
        [HttpPost("UploadOrderFile")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileByTableIdRequestDto requestDto)
        {
            try
            {
                if (User.Identity is not ClaimsIdentity identity)
                {
                    return Unauthorized();
                }

                var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var fromEmployeeId = await _employeeRepo.GetEmployeeIdByUserId(userId);

                if (fromEmployeeId == null)
                {
                    return Unauthorized();
                }

                var getOrder = await _orderRepo.GetById(requestDto.OrderId);
                if (getOrder == null)
                {
                    return NotFound(new MessageDto()
                    {
                        MessageAr = ms.NotFoundAr,
                        MessageEn = ms.NotFoundEn
                    });
                }


                if (getOrder.Status != 3 || fromEmployeeId != getOrder.EmployeeId)
                {
                    return Forbid();
                }

                var getCustomerPhoneNumber = await _customerRepo.GetCustomerPhoneById(getOrder.CustomerId);
                if (getCustomerPhoneNumber == null)
                {
                    return BadRequest(new MessageDto
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }
                string filePath = await _uploadFileService.UploadPdfFile(requestDto.File);
                getOrder.AttachmentPath = filePath;
                getOrder.Status = 6;
                _orderRepo.Update(getOrder);

                var saveChanges = await _orderRepo.SaveChanges();
                if (!saveChanges)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                var otpCode = RandomCodeGenerator.GenerateOTP();
                if (otpCode == null)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                var getMessage = SMSMessages.SendOTP(otpCode);
                if (getMessage == null)
                {
                    return BadRequest();
                }

                List<string> listofData = [];
                listofData.Add(getCustomerPhoneNumber);

                var messageData = new SendRequestDto()
                {
                    Source = "BRONZE NET",
                    Message = getMessage,
                    Destination = listofData
                };

                var apiKey = _configuration["SMSAPIKey"];
                if (apiKey == null)
                {
                    return Forbid();
                }

                await _sMSService.SendMessage(messageData, apiKey);

                DateTime currentTime = DateTime.Now;
                int minutesToAdd = 5;
                DateTime newTime = currentTime.AddMinutes(minutesToAdd);

                await _otpMessageRepo.DeleteOld();

                await _otpMessageRepo.Create(new OTPMessage()
                {
                    Code = otpCode,
                    ExpirationDatetime = newTime,
                    PhoneNumber = getCustomerPhoneNumber,
                    Status = 0,
                });

                var saveChages = await _otpMessageRepo.SaveChanges();

                if (!saveChages)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                return Ok(new MessageDto()
                {
                    MessageAr = ms.FileUploadedAr,
                    MessageEn = ms.FileUploadedEn
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new MessageDto()
                {
                    MessageAr = ms.FailedAr,
                    MessageEn = ex.Message,
                });
            }
        }



        [Authorize(Roles = "Technician")]
        [HttpPost("ReSendOTP")]
        public async Task<IActionResult> ReSendOTP(GetByOrderIdRequestDto requestDto)
        {
            try
            {
                var getOrder = await _orderRepo.GetById(requestDto.OrderId);
                if (getOrder == null)
                {
                    return NotFound(new MessageDto()
                    {
                        MessageAr = ms.NotFoundAr,
                        MessageEn = ms.NotFoundEn
                    });
                }

                var getCustomerPhoneNumber = await _customerRepo.GetCustomerPhoneById(getOrder.CustomerId);
                if (getCustomerPhoneNumber == null)
                {
                    return BadRequest(new MessageDto
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                var otpCode = RandomCodeGenerator.GenerateOTP();
                if (otpCode == null)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                var getMessage = SMSMessages.SendOTP(otpCode);
                if (getMessage == null)
                {
                    return BadRequest();
                }

                List<string> listofData = [];
                listofData.Add(getCustomerPhoneNumber);

                var messageData = new SendRequestDto()
                {
                    Source = "BRONZE NET",
                    Message = getMessage,
                    Destination = listofData
                };

                var apiKey = _configuration["SMSAPIKey"];
                if (apiKey == null)
                {
                    return Forbid();
                }
                await _sMSService.SendMessage(messageData, apiKey);


                DateTime currentTime = DateTime.Now;
                int minutesToAdd = 5;
                DateTime newTime = currentTime.AddMinutes(minutesToAdd);

                await _otpMessageRepo.DeleteOld();

                await _otpMessageRepo.Create(new OTPMessage()
                {
                    Code = otpCode,
                    ExpirationDatetime = newTime,
                    PhoneNumber = getCustomerPhoneNumber,
                    Status = 0,
                });

                var saveChages = await _otpMessageRepo.SaveChanges();

                if (!saveChages)
                {
                    return BadRequest(new MessageDto()
                    {
                        MessageAr = ms.FailedAr,
                        MessageEn = ms.FailedEn
                    });
                }

                return Ok(new MessageDto()
                {
                    MessageAr = ms.SendSuccessfullyAr,
                    MessageEn = ms.SendSuccessfullyEn
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetCustomerOrderedDevice")]
        public async Task<ActionResult<List<GetCustomerOrderedDeviceResponseDto>>> GetCustomerOrderedDevice()
        {
            try
            {
                if (User.Identity is not ClaimsIdentity identity)
                {
                    return Unauthorized();
                }

                var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var customerId = await _customerRepo.GetCustomerId(userId);
                if (customerId == null)
                {
                    return NotFound(new MessageDto()
                    {
                        MessageAr = ms.NotFoundAr,
                        MessageEn = ms.NotFoundEn
                    });
                }

                var result = await _customerProductRepo.GetCustomerProduct(customerId);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetCustomerPartDetails/{customerProductId}")]
        public async Task<ActionResult<List<GetCustomerPartDetailsResponseDto>>> GetCustomerPartDetails(int customerProductId)
        {
            try
            {
                if (User.Identity is not ClaimsIdentity identity)
                {
                    return Unauthorized();
                }

                var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized();
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized();
                }

                var customerId = await _customerRepo.GetCustomerId(userId);
                if (customerId == null)
                {
                    return NotFound(new MessageDto()
                    {
                        MessageAr = ms.NotFoundAr,
                        MessageEn = ms.NotFoundEn
                    });
                }

                var result = await _customerProductRepo.GetCustomerPartDetails(customerProductId);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GetOrderDetails/{orderId}")]
        public async Task<ActionResult<GetOrderDetailsResponseDto>> GetOrderDetails(int orderId)
        {
            var getOrderContactDetails = await _orderRepo.GetContactDetails(orderId);
            var getOrderDetails = await _orderRepo.GetOrderDetails(orderId);
            if (getOrderDetails == null || getOrderContactDetails == null)
            {
                return NotFound(new MessageDto()
                {
                    MessageAr = ms.NotFoundAr,
                    MessageEn = ms.NotFoundEn
                });
            }

            return Ok(new GetOrderDetailsResponseDto()
            {
                OrderContactDetails = getOrderContactDetails,
                OrderDetails = getOrderDetails
            });
        }



        [HttpGet("GetTechnicianOrdersArchive")]
        public async Task<ActionResult<GetCustomerOrderHistoryResponseDto>> GetTechnicianOrdersArchive(int page = 1, int row = 10)
        {
            if (User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized();
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var technicianId = await _employeeRepo.GetEmployeeIdByUserId(userId);

            if (technicianId == null)
            {
                return Unauthorized();
            }

            var result = await _orderRepo.GetTechnicianArchive(technicianId);
            var list = PagedList<GetCustomerOrderHistoryResponseDto>.ToPagedList(result, page, row);
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(list.Paganation));
            return Ok(list);
        }

    }
}
