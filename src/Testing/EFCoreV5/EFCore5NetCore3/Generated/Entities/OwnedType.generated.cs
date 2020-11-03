//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
//
//     Produced by Entity Framework Visual Editor v2.1.0.4
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
   public partial class OwnedType
   {
      partial void Init();

      /// <summary>
      /// Default constructor
      /// </summary>
      public OwnedType()
      {
         // NOTE: This class has one-to-one associations with OwnedType.
         // One-to-one associations are not validated in constructors since this causes a scenario where each one must be constructed before the other.

         Init();
      }

      /// <summary>
      /// Public constructor with required data
      /// </summary>
      /// <param name="id"></param>
      /// <param name="_allpropertytypesoptional0"></param>
      /// <param name="_allpropertytypesoptional2"></param>
      public OwnedType(int id, global::Testing.AllPropertyTypesOptional _allpropertytypesoptional0, global::Testing.AllPropertyTypesOptional _allpropertytypesoptional2)
      {
         // NOTE: This class has one-to-one associations with OwnedType.
         // One-to-one associations are not validated in constructors since this causes a scenario where each one must be constructed before the other.

         this.Id = id;

         if (_allpropertytypesoptional0 == null) throw new ArgumentNullException(nameof(_allpropertytypesoptional0));
         _allpropertytypesoptional0.OwnedTypeOptional = this;

         if (_allpropertytypesoptional2 == null) throw new ArgumentNullException(nameof(_allpropertytypesoptional2));
         _allpropertytypesoptional2.OwnedTypeCollection.Add(this);


         Init();
      }

      /// <summary>
      /// Static create function (for use in LINQ queries, etc.)
      /// </summary>
      /// <param name="id"></param>
      /// <param name="_allpropertytypesoptional0"></param>
      /// <param name="_allpropertytypesoptional2"></param>
      public static OwnedType Create(int id, global::Testing.AllPropertyTypesOptional _allpropertytypesoptional0, global::Testing.AllPropertyTypesOptional _allpropertytypesoptional2)
      {
         return new OwnedType(id, _allpropertytypesoptional0, _allpropertytypesoptional2);
      }

      /*************************************************************************
       * Properties
       *************************************************************************/

      /// <summary>
      /// Backing field for SingleAttr
      /// </summary>
      protected Single? _singleAttr;
      /// <summary>
      /// When provided in a partial class, allows value of SingleAttr to be changed before setting.
      /// </summary>
      partial void SetSingleAttr(Single? oldValue, ref Single? newValue);
      /// <summary>
      /// When provided in a partial class, allows value of SingleAttr to be changed before returning.
      /// </summary>
      partial void GetSingleAttr(ref Single? result);

      public Single? SingleAttr
      {
         get
         {
            Single? value = _singleAttr;
            GetSingleAttr(ref value);
            return (_singleAttr = value);
         }
         set
         {
            Single? oldValue = _singleAttr;
            SetSingleAttr(oldValue, ref value);
            if (oldValue != value)
            {
               _singleAttr = value;
            }
         }
      }

      /// <summary>
      /// Backing field for Id
      /// </summary>
      internal int _id;
      /// <summary>
      /// When provided in a partial class, allows value of Id to be changed before setting.
      /// </summary>
      partial void SetId(int oldValue, ref int newValue);
      /// <summary>
      /// When provided in a partial class, allows value of Id to be changed before returning.
      /// </summary>
      partial void GetId(ref int result);

      /// <summary>
      /// Indexed, Required
      /// </summary>
      [Required]
      public int Id
      {
         get
         {
            int value = _id;
            GetId(ref value);
            return (_id = value);
         }
         set
         {
            int oldValue = _id;
            SetId(oldValue, ref value);
            if (oldValue != value)
            {
               _id = value;
            }
         }
      }

      /*************************************************************************
       * Navigation properties
       *************************************************************************/

   }
}

