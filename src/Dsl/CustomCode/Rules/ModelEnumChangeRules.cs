﻿using System.CodeDom.Compiler;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling;
using Sawczyn.EFDesigner.EFModel.CustomCode.Rules;

namespace Sawczyn.EFDesigner.EFModel
{
   [RuleOn(typeof(ModelEnum), FireTime = TimeToFire.TopLevelCommit)]
   internal class ModelEnumChangeRules : ChangeRule
   {
      public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
      {
         base.ElementPropertyChanged(e);

         ModelEnum element = (ModelEnum)e.ModelElement;
         Store store = element.Store;
         Transaction current = store.TransactionManager.CurrentTransaction;

         if (current.IsSerializing)
            return;

         string errorMessage = null;

         switch (e.DomainProperty.Name)
         {
            case "Name":
               string newName = (string)e.NewValue;
               if (current.Name.ToLowerInvariant() == "paste")
                  return;

               if (current.Name.ToLowerInvariant() != "paste" && (string.IsNullOrWhiteSpace(newName) || !CodeGenerator.IsValidLanguageIndependentIdentifier(newName)))
                  errorMessage = "Name must be a valid .NET identifier";
               else if (store.ElementDirectory
                             .AllElements
                             .OfType<ModelClass>()
                             .Any(x => x.Name == newName))
                  errorMessage = "Enum name already in use";
               else if (store.ElementDirectory
                             .AllElements
                             .OfType<ModelEnum>()
                             .Except(new[] {element})
                             .Any(x => x.Name == newName))
                  errorMessage = "Enum name already in use";

               break;

            case "Namespace":
               if (current.Name.ToLowerInvariant() != "paste")
                  errorMessage = CommonRules.ValidateNamespace((string)e.NewValue, CodeGenerator.IsValidLanguageIndependentIdentifier);
               break;
         }

         if (errorMessage != null)
         {
            current.Rollback();
            MessageBox.Show(errorMessage);
         }
      }
   }
}
