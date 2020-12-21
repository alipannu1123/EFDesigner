//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor v3.0.1.1
//     Source:                    https://github.com/msawczyn/EFDesigner
//     Visual Studio Marketplace: https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner
//     Documentation:             https://msawczyn.github.io/EFDesigner/
//     License (MIT):             https://github.com/msawczyn/EFDesigner/blob/master/LICENSE
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Testing
{
   public class EFModel1Factory: IDesignTimeDbContextFactory<EFModel1>
   {
      /// <summary>Creates a new instance of a derived context.</summary>
      /// <param name="args"> Arguments provided by the design-time service. </param>
      /// <returns> An instance of <typeparamref name="Testing.EFModel1" />.</returns>
      public EFModel1 CreateDbContext(string[] args)
      {
         DbContextOptionsBuilder<EFModel1> optionsBuilder = new DbContextOptionsBuilder<EFModel1>();

         // Please provide the EFModel1.ConfigureOptions(optionsBuilder) in the partial class as
         //    public static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder) {{ ... }}
         // If you have custom initialization for the context, you can then consolidate the code by defining the CustomInit partial as
         //    partial void CustomInit(DbContextOptionsBuilder optionsBuilder) => ConfigureOptions(optionsBuilder);
         EFModel1.ConfigureOptions(optionsBuilder);
         return new EFModel1(optionsBuilder.Options);
      }
   }
}