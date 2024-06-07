using BTMBackend.Dtos.OrderDto;
using BTMBackend.Dtos.PublicDto;
using BTMBackend.Models;
using BTMBackend.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface IOrderRepo
    {
        Task Create(Order order);
        Task CreateHistory(OrderStatusHistory orderStatusHistory);
        Task<Order?> GetById(int orderId);
        void Update(Order order);
        Task<int?> GetSuperVisorIdFromHistory(int orderId);
        Task<bool> IsExist(int orderId);
        Task<OrderContactDetails?> GetContactDetails(int orderId);
        Task<List<OrderDetails>> GetOrderDetails(int orderId);
        Task<IQueryable<GetAllOrdersResponseDto>> GetAllOrders();
        Task<IQueryable<GetCustomerOrderHistoryResponseDto>> GetCustomerHistoryOperationsById(int? customerId);
        Task<IQueryable<GetSupervisorOperationsResponseDto>> GetSupervisorOperations();
        Task<IQueryable<GetOrderHistoryResponseDto>> GetOrderHistoryByOrderId(int orderId);
        Task<List<GetOrderItemsResponseDto>> GetOrderItemsByOrderId(int orderId);
        Task<IQueryable<GetCallCenterOrdersResponseDto>> GetCallCenterOrders(int? callCenterId);
        Task<IQueryable<GetTechnicianOrdersResponseDto>> GetTechnicianOrder(int technicianId);
        Task<List<GetCustomerInProgressOrdersResponseDto>> GetCustomerOrders(int userId);
        Task<bool> SendToCallCenter(UpdateMultiOrderRequestDto requestDto);
        Task<bool> SendToTechnician(UpdateToSpecilistRequestDto requestDto);
        Task<bool> UpdateStatus(UpdateSingleOrderRequestDto requestDto);
        Task<IQueryable<GetCustomerOrderHistoryResponseDto>> GetTechnicianArchive(int? technicianId);
        Task<bool> SaveChanges();
    }
    public class OrderRepo(DataContext context, UploadFileService fileService) : IOrderRepo
    {
        private readonly DataContext _context = context;
        private readonly UploadFileService _fileService = fileService;

        public async Task Create(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task CreateHistory(OrderStatusHistory orderStatusHistory)
        {
            await _context.OrderStatusHistories.AddAsync(orderStatusHistory);
        }

        public Task<Order?> GetById(int orderId)
        {
            return _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public Task<bool> IsExist(int orderId)
        {
            return _context.Orders.AnyAsync(x => x.Id == orderId);
        }

        public Task<IQueryable<GetAllOrdersResponseDto>> GetAllOrders()
        {
            var result = _context.Orders
                .Join(
                    _context.Customers,
                    order => order.CustomerId,
                    customer => customer.Id,
                    (order, customer) => new { Order = order, Customer = customer }
                )
                .Join(
                    _context.PublicLists,
                    oc => oc.Order.ServiceType,
                    publiclist => publiclist.Id,
                    (oc, publiclist) => new { OrderCustomer = oc, PublicList = publiclist }
                )
                .Select(o => new GetAllOrdersResponseDto()
                {
                    OrderId = o.OrderCustomer.Order.Id,
                    CustomerName = $"{o.OrderCustomer.Customer.FirstName} {o.OrderCustomer.Customer.LastName}",
                    ServiceTypeAr = o.PublicList.NameAR,
                    ServiceTypeEn = o.PublicList.NameEN,
                    StatusAr = GetOrderStatusArText(o.OrderCustomer.Order.Status),
                    StatusEn = GetOrderStatusEnText(o.OrderCustomer.Order.Status)
                })
                .AsQueryable();

            return Task.FromResult(result);
        }

        public Task<IQueryable<GetCustomerOrderHistoryResponseDto>> GetCustomerHistoryOperationsById(int? customerId)
        {
            var result = _context.Orders
                .Where(o => o.CustomerId == customerId && (o.Status == 5 || o.Status == 8))
                .Join(_context.PublicLists, o => o.ServiceType, p => p.Id, (o, p) => new GetCustomerOrderHistoryResponseDto
                {
                    ServiceTypeAr = p.NameAR,
                    ServiceTypeEn = p.NameEN,
                    StatusAr = GetOrderStatusArText(o.Status),
                    StatusEn = GetOrderStatusEnText(o.Status),
                    OrderClosingDAte = o.UpdatedAt.ToShortDateString()
                })
                .AsQueryable();

            return Task.FromResult(result);
        }

        public async Task<IQueryable<GetSupervisorOperationsResponseDto>> GetSupervisorOperations()
        {
            var orders = _context.Orders
                .Where(order => order.Status == 0 || order.Status == 2 || order.Status == 4 || order.Status == 6 || order.Status == 7)
                .OrderBy(order => order.UpdatedAt);

            var result = await (
                from order in orders
                join customer in _context.Customers on order.CustomerId equals customer.Id
                join county in _context.PublicLists on order.CountyId equals county.Id
                join city in _context.PublicLists on order.CityId equals city.Id
                join serviceType in _context.PublicLists on order.ServiceType equals serviceType.Id
                select new GetSupervisorOperationsResponseDto
                {
                    Id = order.Id,
                    CustomerFullName = customer.FirstName + " " + customer.LastName,
                    CountyNameAr = county.NameAR,
                    CountyNameEn = county.NameEN,
                    CityNameAr = city.NameAR,
                    CityNameEn = city.NameEN,
                    CreatedAt = order.CreatedAt,
                    StatusAr = GetOrderStatusArText(order.Status),
                    StatusEn = GetOrderStatusEnText(order.Status),
                }
            ).ToListAsync();

            return result.AsQueryable();
        }




        public Task<IQueryable<GetOrderHistoryResponseDto>> GetOrderHistoryByOrderId(int orderId)
        {
            var result = _context.OrderStatusHistories
                .Where(orderHistory => orderHistory.OrderId == orderId)
                .Join(_context.Employees,
                    orderHistory => orderHistory.EmployeeId,
                    employee => employee.Id,
                    (orderHistory, employee) => new GetOrderHistoryResponseDto
                    {
                        Employee = $"{employee.FirstName} {employee.LastName}",
                        Note = orderHistory.Note,
                        CreatedAt = orderHistory.CreatedAt,
                        StatusAr = GetOrderStatusArText(orderHistory.Status),
                        StatusEn = GetOrderStatusEnText(orderHistory.Status)
                    })
                .AsQueryable();

            return Task.FromResult(result);
        }

        public async Task<List<GetOrderItemsResponseDto>> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(item => item.OrderId == orderId)
                .ToListAsync();

            var productIds = orderItems.Where(item => item.ItemType == 1).Select(item => item.ItemId).ToList();
            var products = await _context.Products.Where(product => productIds.Contains(product.Id)).ToListAsync();

            var partIds = orderItems.Where(item => item.ItemType == 2).Select(item => item.ItemId).ToList();
            var parts = await _context.Parts.Where(part => partIds.Contains(part.Id)).ToListAsync();

            var result = new List<GetOrderItemsResponseDto>();
            foreach (var item in orderItems)
            {
                var newItem = new GetOrderItemsResponseDto();
                if (item.ItemType == 1) // product
                {
                    var product = products.FirstOrDefault(x => x.Id == item.ItemId);
                    if (product != null)
                    {
                        newItem.Id = product.Id;
                        newItem.Image = await _fileService.ConvertFileToByteArrayAsync(product.ImageUrl);
                        newItem.ItemNameAr = product.NameAr;
                        newItem.ItemNameEn = product.NameEn;
                        newItem.ItemTypeAr = "منتج";
                        newItem.ItemTypeEn = "Product";
                    }
                }
                else if (item.ItemType == 2) // Part
                {
                    var part = parts.FirstOrDefault(x => x.Id == item.ItemId);
                    if (part != null)
                    {
                        newItem.Id = part.Id;
                        newItem.ItemNameAr = part.NameAr;
                        newItem.ItemNameEn = part.NameEn;
                        newItem.ItemTypeAr = "قطعة";
                        newItem.ItemTypeEn = "Part";
                    }
                }

                result.Add(newItem);
            }

            return result;

        }

        public async Task<IQueryable<GetCallCenterOrdersResponseDto>> GetCallCenterOrders(int? callCenterId)
        {
            var orders = _context.Orders.AsQueryable();

            if (callCenterId.HasValue)
            {
                orders = orders.Where(order => order.EmployeeId == callCenterId);
            }

            var result = await (
                from order in orders
                join employee in _context.Employees on order.EmployeeId equals employee.Id
                join customer in _context.Customers on order.CustomerId equals customer.Id
                join county in _context.PublicLists on order.CountyId equals county.Id
                join city in _context.PublicLists on order.CityId equals city.Id
                join serviceType in _context.PublicLists on order.ServiceType equals serviceType.Id
                where order.Status == 1
                orderby order.UpdatedAt ascending
                select new GetCallCenterOrdersResponseDto
                {
                    Id = order.Id,
                    CustomerFullName = customer.FirstName + " " + customer.LastName,
                    CountyNameAr = county.NameAR,
                    CountyNameEn = county.NameEN,
                    CityNameAr = city.NameAR,
                    CityNameEn = city.NameEN,
                    CreatedAt = order.CreatedAt,
                }
            ).ToListAsync();

            return result.AsQueryable();
        }


        public async Task<IQueryable<GetTechnicianOrdersResponseDto>> GetTechnicianOrder(int technicianId)
        {
            var result = await (from order in _context.Orders
                                join customer in _context.Customers on order.CustomerId equals customer.Id
                                join pulicList in _context.PublicLists on order.ServiceType equals pulicList.Id
                                where order.EmployeeId == technicianId
                                select new GetTechnicianOrdersResponseDto()
                                {
                                    Id = order.Id,
                                    ServiceTypeAr = pulicList.NameAR,
                                    ServiceTypeEn = pulicList.NameEN,
                                    Date = order.OperationDateTime.HasValue ? order.OperationDateTime.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    Time = order.OperationDateTime.HasValue ? order.OperationDateTime.Value.ToString("hh:mm tt") : string.Empty,
                                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                                    StatusAr = GetOrderStatusArText(order.Status),
                                    StatusEn = GetOrderStatusEnText(order.Status)
                                }).ToListAsync();

            return result.AsQueryable();
        }


        public async Task<List<GetCustomerInProgressOrdersResponseDto>> GetCustomerOrders(int userId)
        {
            var customer = await _context.Customers
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (customer == null)
                return [];

            var result = customer.Orders
                .Where(order => order.Status != 5 && order.Status != 8)
                .Join(_context.PublicLists,
                    order => order.ServiceType,
                    publiclist => publiclist.Id,
                    (order, publiclist) => new GetCustomerInProgressOrdersResponseDto
                    {
                        Id = order.Id,
                        ServiceTypeAr = publiclist.NameAR,
                        ServiceTypeEn = publiclist.NameEN,
                        Date = order.OperationDateTime?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Time = order.OperationDateTime?.ToString("hh:mm tt") ?? string.Empty,
                        StatusAr = GetOrderStatusArText(order.Status),
                        StatusEn = GetOrderStatusEnText(order.Status)
                    })
                .ToList();

            return result;
        }

        public async Task<bool> SendToCallCenter(UpdateMultiOrderRequestDto requestDto)
        {
            var orders = await _context.Orders
                .Where(x => requestDto.Id.Contains(x.Id))
                .ToListAsync();

            if (orders.Count != requestDto.Id.Length)
            {
                return false; // Some orders were not found
            }

            var orderStatusHistories = orders.Select(order => new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = 1,
                Note = "تحويل الى الكول سنتر",
                EmployeeId = requestDto.FromEmployeeId
            }).ToList();

            foreach (var order in orders)
            {
                order.Status = 1;
                order.Note = requestDto.Note;
                order.EmployeeId = requestDto.ToEmployeeId;
                order.UpdatedAt = DateTime.Now;
                Update(order);
            }

            await _context.OrderStatusHistories.AddRangeAsync(orderStatusHistories);

            return await SaveChanges();
        }

        public async Task<bool> UpdateStatus(UpdateSingleOrderRequestDto requestDto)
        {
            var order = await GetById(requestDto.OrderId);

            if (order == null)
            {
                return false;
            }

            order.Status = requestDto.Status;
            order.Note = requestDto.Note;
            order.EmployeeId = requestDto.ToEmployeeId;
            order.UpdatedAt = DateTime.Now;

            var orderStatusHistory = new OrderStatusHistory()
            {
                OrderId = order.Id,
                Status = requestDto.Status,
                Note = requestDto.Note,
                EmployeeId = requestDto.FromEmployeeId
            };

            _context.Update(order);
            _context.OrderStatusHistories.Add(orderStatusHistory);

            return await SaveChanges();
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _context.ChangeTracker.Clear();
                return false;
            }
        }

        public async Task<bool> SendToTechnician(UpdateToSpecilistRequestDto requestDto)
        {
            var order = await GetById(requestDto.OrderId);

            if (order == null)
            {
                return false;
            }

            order.Status = 3;
            order.Note = requestDto.Note;
            order.EmployeeId = requestDto.ToEmployeeId;
            order.UpdatedAt = DateTime.Now;
            order.OperationDateTime = requestDto.OperationDateTime;

            var orderStatusHistory = new OrderStatusHistory()
            {
                OrderId = order.Id,
                Status = 3,
                Note = requestDto.Note,
                EmployeeId = requestDto.FromEmployeeId
            };

            _context.Update(order);
            _context.OrderStatusHistories.Add(orderStatusHistory);

            return await SaveChanges();
        }

        private static string GetOrderStatusArText(int status)
        {
            return status switch
            {
                0 => "تحت المراجعة",
                1 => "تم الارسال للكول سنتر",
                2 => "طلب الغاء من الكول سنتر",
                3 => "تم الارسال للفني",
                4 => "طلب الغاء من الفني",
                5 => "تم الغاء الطلب",
                6 => "تم رفع الملفات",
                7 => "مراجعة اغلاق العملية",
                _ => "تمت العملية"
            };
        }

        private static string GetOrderStatusEnText(int status)
        {
            return status switch
            {
                0 => "Under Review",
                1 => "Sent To Call Center",
                2 => "Cancellation Request From Call Center",
                3 => "Sent To Technician",
                4 => "Cancellation Request From Technician",
                5 => "Request Canceled",
                6 => "Files Uploaded",
                7 => "Process Closure Review",
                _ => "Process Completed"
            };
        }

        public async Task<OrderContactDetails?> GetContactDetails(int orderId)
        {
            var result = await (from order in _context.Orders
                                join customer in _context.Customers on order.CustomerId equals customer.Id
                                join city in _context.PublicLists on order.CityId equals city.Id
                                join county in _context.PublicLists on order.CountyId equals county.Id
                                where order.Id == orderId
                                select new OrderContactDetails()
                                {
                                    FullName = $"{customer.FirstName} {customer.LastName}",
                                    PhoneNumber = customer.PhoneNumber,
                                    InstallationDate = order.OperationDateTime.HasValue ? order.OperationDateTime.Value.ToShortDateString() : string.Empty,
                                    InstallationTime = order.OperationDateTime.HasValue ? order.OperationDateTime.Value.ToString("hh:mm tt") : string.Empty,
                                    AddressAr = $"{city.NameAR} ، {county.NameAR}",
                                    AddressEn = $"{city.NameEN} , {county.NameEN}",
                                    AddressDetails = order.Address
                                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<OrderDetails>> GetOrderDetails(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(item => item.OrderId == orderId)
                .ToListAsync();

            var productIds = orderItems.Where(item => item.ItemType == 1).Select(item => item.ItemId).ToList();
            var products = await _context.Products.Where(product => productIds.Contains(product.Id)).ToListAsync();

            var partIds = orderItems.Where(item => item.ItemType == 2).Select(item => item.ItemId).ToList();
            var parts = await _context.Parts.Where(part => partIds.Contains(part.Id)).ToListAsync();

            var result = new List<OrderDetails>();

            foreach (var item in orderItems)
            {
                var newItem = new OrderDetails();

                if (item.ItemType == 1) // product
                {
                    var product = products.FirstOrDefault(x => x.Id == item.ItemId);
                    if (product != null)
                    {
                        newItem.ItemPhoto = await _fileService.ConvertFileToByteArrayAsync(product.ImageUrl);
                        newItem.ItemNameAr = product.NameAr;
                        newItem.ItemNameEn = product.NameEn;
                        newItem.ItemTypeAr = "منتج";
                        newItem.ItemTypeEn = "Product";
                    }
                }
                else if (item.ItemType == 2) // Part
                {
                    var part = parts.FirstOrDefault(x => x.Id == item.ItemId);
                    if (part != null)
                    {
                        newItem.ItemNameAr = part.NameAr;
                        newItem.ItemNameEn = part.NameEn;
                        newItem.ItemTypeAr = "قطعة";
                        newItem.ItemTypeEn = "Part";
                    }
                }

                if (item.ServiceType == 1) // Purchase a Product
                {
                    newItem.ServiceTypeAr = "شراء منتج";
                    newItem.ServiceTypeEn = "Purchase a Product";
                }
                else if (item.ServiceType == 2) // Periodic Maintenance
                {
                    newItem.ServiceTypeAr = "صيانة دورية";
                    newItem.ServiceTypeEn = "Periodic Maintenance";
                }
                else if (item.ServiceType == 3) // Emergency Maintenance
                {
                    newItem.ServiceTypeAr = "صيانة طارئة";
                    newItem.ServiceTypeEn = "Emergency Maintenance";
                }

                result.Add(newItem);
            }
            return result;
        }

        public async Task<IQueryable<GetCustomerOrderHistoryResponseDto>> GetTechnicianArchive(int? technicianId)
        {
            var result = await (from order in _context.Orders
                                join orderStatusHistory in _context.OrderStatusHistories on order.Id equals orderStatusHistory.OrderId
                                join serviceType in _context.PublicLists on order.ServiceType equals serviceType.Id
                                where orderStatusHistory.EmployeeId == technicianId
                                where order.Status == 5 || order.Status == 8
                                select new GetCustomerOrderHistoryResponseDto()
                                {
                                    ServiceTypeAr = serviceType.NameAR,
                                    ServiceTypeEn = serviceType.NameEN,
                                    StatusAr = GetOrderStatusArText(order.Status),
                                    StatusEn = GetOrderStatusEnText(order.Status),
                                    OrderClosingDAte = order.UpdatedAt.ToShortDateString(),
                                }).ToListAsync();

            return result.AsQueryable();
        }

        public async Task<int?> GetSuperVisorIdFromHistory(int orderId)
        {
            var query = await (from orderHistory in _context.OrderStatusHistories
                               join employee in _context.Employees on orderHistory.EmployeeId equals employee.Id
                               join user in _context.Users on employee.UserId equals user.Id
                               join role in _context.Roles on user.RoleId equals role.Id
                               where orderHistory.OrderId == orderId && role.TitleEn == "Supervisor"
                               select orderHistory).FirstOrDefaultAsync();
            if (query == null)
            {
                return null;
            }

            return query.EmployeeId;

        }
    }
}
