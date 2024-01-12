using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using SYS_PR_Acceso.CS.Ayuda;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using SYS_PR_Nucleo.Clases.DTO;

namespace SYS_PR_Zermat.CS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //De aquí en adelante configuración de documentación de nuestra API
            services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("SYS_CO_Acceso", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Acceso",
                    Version = "1.0.0",
                    Description = "Backend Acceso",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "bernardo.tg94@gmail.com",
                        Name = "Bernardo Trejo Gonzalez",
                        Url = new Uri("http://www.google.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "BTG",
                        Url = new Uri("http://www.google.com")
                    }
                });

                options.SwaggerDoc("SYS_CO_Movimientos", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Movimientos",
                    Version = "1.0.0",
                    Description = "Backend Acceso",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "bernardo.tg94@gmail.com",
                        Name = "Bernardo Trejo Gonzalez",
                        Url = new Uri("http://www.google.com")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "BTG",
                        Url = new Uri("http://www.google.com")
                    }
                });

                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                options.IncludeXmlComments(rutaApiComentarios);


                //Primero definir el esquema de seguridad
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticación JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                  });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { "readAccess", "writeAccess" }
                    }
                });

            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddControllers();
            services.Configure<SYS_DTO_Conexion>(Configuration.GetSection("Conexion"));
            /*Damos soporte para CORS*/
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AgregarErrorAplicacion(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }
            app.UseHttpsRedirection();

            //Línea para documentación api
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/SYS_CO_Acceso/swagger.json", "Login");
                options.SwaggerEndpoint("/swagger/SYS_CO_Movimientos/swagger.json", "Movimientos");

                //Para la publicación en IIS descomentar estas líneas y comentar las de arriba                
                //options.SwaggerEndpoint("/Zermatusa/swagger/SYS_CO_Acceso/swagger.json", "Login");

                options.RoutePrefix = "";

            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            /*Damos soporte para CORS*/
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
