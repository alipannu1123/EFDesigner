//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor v2.1.0.0
//     Source:                    https://github.com/msawczyn/EFDesigner
//     Visual Studio Marketplace: https://marketplace.visualstudio.com/items?itemName=michaelsawczyn.EFDesigner
//     Documentation:             https://msawczyn.github.io/EFDesigner/
//     License (MIT):             https://github.com/msawczyn/EFDesigner/blob/master/LICENSE
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Testing
{
   public partial class BackingFieldTester
   {
      partial void Init();

      /// <summary>
      /// Default constructor
      /// </summary>
      public BackingFieldTester()
      {
         Init();
      }

      /*************************************************************************
       * Properties
       *************************************************************************/

      /// <summary>
      /// Identity, Indexed, Required
      /// </summary>
      [Key]
      [Required]
      public int Id { get; protected set; }

      /// <summary>
      /// Backing field for Property1_FDC
      /// </summary>
      protected int? _property1_FDC;
      /// <summary>
      /// When provided in a partial class, allows value of Property1_FDC to be changed before setting.
      /// </summary>
      partial void SetProperty1_FDC(int? oldValue, ref int? newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Property1_FDC to be changed before returning.
      /// </summary>
      partial void GetProperty1_FDC(ref int? result);

      public int? Property1_FDC
      {
         get
         {
            int? value = _property1_FDC;
            GetProperty1_FDC(ref value);
            return (_property1_FDC = value);
         }
         set
         {
            int? oldValue = _property1_FDC;
            SetProperty1_FDC(oldValue, ref value);
            if (oldValue != value)
            {
               _property1_FDC = value;
            }
         }
      }

      public int? Property2_FA { get; set; }

      public int? Property3_PA { get; set; }

      /*************************************************************************
       * Navigation properties
       *************************************************************************/

      protected global::Testing.BackingFieldTesterChild _toBackingFieldOnConstruction;
      partial void SetToBackingFieldOnConstruction(global::Testing.BackingFieldTesterChild oldValue, ref global::Testing.BackingFieldTesterChild newValue);
      partial void GetToBackingFieldOnConstruction(ref global::Testing.BackingFieldTesterChild result);

      public virtual global::Testing.BackingFieldTesterChild ToBackingFieldOnConstruction
      {
         get
         {
            global::Testing.BackingFieldTesterChild value = _toBackingFieldOnConstruction;
            GetToBackingFieldOnConstruction(ref value);
            return (_toBackingFieldOnConstruction = value);
         }
         set
         {
            global::Testing.BackingFieldTesterChild oldValue = _toBackingFieldOnConstruction;
            SetToBackingFieldOnConstruction(oldValue, ref value);
            if (oldValue != value)
            {
               _toBackingFieldOnConstruction = value;
            }
         }
      }

      protected global::Testing.BackingFieldTesterChild _toBackingFieldAlways;
      partial void SetToBackingFieldAlways(global::Testing.BackingFieldTesterChild oldValue, ref global::Testing.BackingFieldTesterChild newValue);
      partial void GetToBackingFieldAlways(ref global::Testing.BackingFieldTesterChild result);

      public virtual global::Testing.BackingFieldTesterChild ToBackingFieldAlways
      {
         get
         {
            global::Testing.BackingFieldTesterChild value = _toBackingFieldAlways;
            GetToBackingFieldAlways(ref value);
            return (_toBackingFieldAlways = value);
         }
         set
         {
            global::Testing.BackingFieldTesterChild oldValue = _toBackingFieldAlways;
            SetToBackingFieldAlways(oldValue, ref value);
            if (oldValue != value)
            {
               _toBackingFieldAlways = value;
            }
         }
      }

      protected global::Testing.BackingFieldTesterChild _toProperty;
      partial void SetToProperty(global::Testing.BackingFieldTesterChild oldValue, ref global::Testing.BackingFieldTesterChild newValue);
      partial void GetToProperty(ref global::Testing.BackingFieldTesterChild result);

      public virtual global::Testing.BackingFieldTesterChild ToProperty
      {
         get
         {
            global::Testing.BackingFieldTesterChild value = _toProperty;
            GetToProperty(ref value);
            return (_toProperty = value);
         }
         set
         {
            global::Testing.BackingFieldTesterChild oldValue = _toProperty;
            SetToProperty(oldValue, ref value);
            if (oldValue != value)
            {
               _toProperty = value;
            }
         }
      }

   }
}
