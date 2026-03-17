using BookingTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Data.SeedData
{
    public static class SeedLocations
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Provinces.AnyAsync()) return;

            // 1. Dữ liệu Tỉnh (Provinces)
            var hanoi = new Provinces { ProvinceName = "Hà Nội", IsActive = true };
            var nghean = new Provinces { ProvinceName = "Nghệ An", IsActive = true };
            
            context.Provinces.AddRange(hanoi, nghean);
            await context.SaveChangesAsync();

            // 2. Dữ liệu Xã/Phường (Wards) - Seed một số đơn vị hành chính tiêu biểu
            var wards = new List<Ward>();

            // Hà Nội - Một số khu vực trọng điểm xe khách
            wards.Add(new Ward { WardName = "Quận Cầu Giấy", ProvinceId = hanoi.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Quận Hoàng Mai", ProvinceId = hanoi.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Quận Hà Đông", ProvinceId = hanoi.ProvinceId, IsActive = true });

            // Nghệ An - Một số đơn vị tại TP. Vinh và các huyện dọc QL1A
            wards.Add(new Ward { WardName = "Xã Vĩnh Tường", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Nhân Hòa", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Anh Sơn", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Yên Xuân", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Đô Lương", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Văn Hiến", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Hợp Minh", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Minh Châu", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Diễn Châu", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Hải Châu", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Quỳnh Phú", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Quỳnh Anh", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Quỳnh Mai", ProvinceId = nghean.ProvinceId, IsActive = true });
            wards.Add(new Ward { WardName = "Xã Tân Mai", ProvinceId = nghean.ProvinceId, IsActive = true });

            context.Wards.AddRange(wards);
            await context.SaveChangesAsync();
            
            // Seed thêm phương thức thanh toán cơ bản (Dữ liệu Master)
            if (!await context.PaymentMethods.AnyAsync())
            {
                context.PaymentMethods.AddRange(
                    new PaymentMethods { PaymentType = "Tiền mặt" },
                    new PaymentMethods { PaymentType = "Chuyển khoản / App" },
                    new PaymentMethods { PaymentType = "VNPay" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
