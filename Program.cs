using HospitalSystem.Data;
using HospitalSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("HospitalDb"));

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Doctors.Any())
    {
        context.Doctors.AddRange(
            new Doctor { Name = "Dr. Ahmed Hassan", Specialization = "Cardiology", ImageUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=300" },
            new Doctor { Name = "Dr. Sara Ali", Specialization = "Pediatrics", ImageUrl = "https://images.unsplash.com/photo-1594824813566-78817730e719?w=300" },
            new Doctor { Name = "Dr. Mohamed Omar", Specialization = "Cardiology", ImageUrl = "https://images.unsplash.com/photo-1537368910025-700350fe46c7?w=300" },
            new Doctor { Name = "Dr. Mona Mahmoud", Specialization = "Dermatology", ImageUrl = "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=300" },
            new Doctor { Name = "Dr. Khaled Youssef", Specialization = "Orthopedics", ImageUrl = "https://images.unsplash.com/photo-1622253692010-333f2da6031d?w=300" },
            new Doctor { Name = "Dr. Layla Ibrahim", Specialization = "Neurology", ImageUrl = "https://images.unsplash.com/photo-1594824813566-78817730e719?w=300" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
