﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Modeling;

using Sawczyn.EFDesigner.EFModel.Extensions;

namespace Sawczyn.EFDesigner.EFModel
{
   [RuleOn(typeof(ModelClass), FireTime = TimeToFire.TopLevelCommit)]
   internal class ModelClassChangeRules : ChangeRule
   {
      public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
      {
         base.ElementPropertyChanged(e);

         ModelClass element = (ModelClass)e.ModelElement;
         ModelRoot modelRoot = element.ModelRoot;

         if (element.IsDeleted)
            return;

         Store store = element.Store;
         Transaction current = store.TransactionManager.CurrentTransaction;

         if (current.IsSerializing || ModelRoot.BatchUpdating)
            return;

         if (Equals(e.NewValue, e.OldValue))
            return;

         List<string> errorMessages = new List<string>();

         switch (e.DomainProperty.Name)
         {
            case "BaseClass":
               {
                  if (element.IsDependentType)
                     errorMessages.Add($"Can't give {element.Name} a base class since it's a dependent type");

                  break;
               }

            case "DbSetName":
               {
                  string newDbSetName = (string)e.NewValue;

                  if (element.IsDependentType)
                  {
                     if (!string.IsNullOrEmpty(newDbSetName))
                        element.DbSetName = string.Empty;
                  }
                  else
                  {
                     if (string.IsNullOrEmpty(newDbSetName))
                        element.DbSetName = MakeDefaultTableAndSetName(element.Name);

                     if (current.Name.ToLowerInvariant() != "paste" &&
                         (string.IsNullOrWhiteSpace(newDbSetName) || !CodeGenerator.IsValidLanguageIndependentIdentifier(newDbSetName)))
                        errorMessages.Add($"DbSet name '{newDbSetName}' isn't a valid .NET identifier.");
                     else if (store.GetAll<ModelClass>()
                                   .Except(new[] { element })
                                   .Any(x => x.DbSetName == newDbSetName))
                        errorMessages.Add($"DbSet name '{newDbSetName}' already in use");
                  }

                  break;
               }

            case "ImplementNotify":
               {
                  bool newImplementNotify = (bool)e.NewValue;

                  if (newImplementNotify)
                  {
                     List<string> nameList = element.Attributes.Where(x => x.AutoProperty).Select(x => x.Name).ToList();
                     if (nameList.Any())
                     {
                        string names = nameList.Count > 1
                                          ? string.Join(", ", nameList.Take(nameList.Count - 1)) + " and " + nameList.Last()
                                          : nameList.First();

                        string verb = nameList.Count > 1
                                         ? "is an autoproperty"
                                         : "are autoproperties";

                        WarningDisplay.Show($"{names} {verb}, so will not participate in INotifyPropertyChanged messages");
                     }
                  }

                  PresentationHelper.UpdateClassDisplay(element);

                  break;
               }

            case "IsAbstract":
               {
                  bool newIsAbstract = (bool)e.NewValue;

                  if (newIsAbstract && element.IsDependentType)
                  {
                     errorMessages.Add($"Can't make {element.Name} abstract since it's a dependent type");

                     break;
                  }

                  PresentationHelper.UpdateClassDisplay(element);

                  break;
               }

            case "IsDependentType":
               {
                  bool newIsDependentType = (bool)e.NewValue;

                  if (newIsDependentType)
                  {
                     if (element.BaseClass != null)
                     {
                        errorMessages.Add($"Can't make {element.Name} a dependent class since it has a base class");

                        break;
                     }

                     string subclasses = string.Join(", ", store.GetAll<Generalization>().Where(g => g.Superclass == element).Select(g => g.Subclass.Name));
                     if (!string.IsNullOrEmpty(subclasses))
                     {
                        errorMessages.Add($"Can't make {element.Name} a dependent class since it has subclass(es) {subclasses}");

                        break;
                     }

                     if (element.IsAbstract)
                     {
                        errorMessages.Add($"Can't make {element.Name} a dependent class since it's abstract");

                        break;
                     }

                     List<Association> principalAssociations = store.GetAll<Association>().Where(a => a.Principal == element).ToList();

                     if (principalAssociations.Any())
                     {
                        string badAssociations = string.Join(", ", principalAssociations.Select(a => a.GetDisplayText()));
                        errorMessages.Add($"Can't make {element.Name} a dependent class since it's the principal end in: {badAssociations}");

                        break;
                     }

                     if (!modelRoot.IsEFCore5Plus && store.GetAll<BidirectionalAssociation>().Any(a => a.Source == element || a.Target == element))
                     {
                        errorMessages.Add($"Can't make {element.Name} a dependent class since it's in a bidirectional association");

                        break;
                     }

                     if (element.ModelRoot.EntityFrameworkVersion == EFVersion.EF6 || element.ModelRoot.GetEntityFrameworkPackageVersionNum() < 2.2)
                     {
                        if (store.GetAll<Association>().Any(a => a.Target == element && a.TargetMultiplicity == Multiplicity.ZeroMany))
                        {
                           errorMessages.Add($"Can't make {element.Name} a dependent class since it's the target of a 0..* association");

                           break;
                        }

                        foreach (UnidirectionalAssociation association in Association.GetLinksToTargets(element).OfType<UnidirectionalAssociation>())
                        {
                           if (association.SourceMultiplicity == Multiplicity.ZeroMany)
                              association.SourceMultiplicity = Multiplicity.ZeroOne;

                           if (association.TargetMultiplicity == Multiplicity.ZeroMany)
                              association.TargetMultiplicity = Multiplicity.ZeroOne;

                           association.TargetRole = EndpointRole.Dependent;
                        }

                        element.TableName = string.Empty;
                     }

                     foreach (ModelAttribute modelAttribute in element.AllAttributes.Where(a => a.IsIdentity))
                        modelAttribute.IsIdentity = false;

                     element.DbSetName = string.Empty;
                  }
                  else
                  {
                     element.DbSetName = MakeDefaultTableAndSetName(element.Name);
                     element.TableName = MakeDefaultTableAndSetName(element.Name);
                  }

                  PresentationHelper.UpdateClassDisplay(element);

                  break;
               }

            case "IsQueryType":
               {
                  if ((bool)e.NewValue)
                  {
                     if (modelRoot.EntityFrameworkVersion == EFVersion.EF6)
                        element.IsQueryType = false;
                     else if (modelRoot.IsEFCore5Plus)
                     {
                        // TODO: Find definitive documentation on query type restrictions in EFCore5+
                        // Restrictions:
                        // =================================
                        // Cannot have a key defined.
                        List<string> allIdentityAttributeNames = element.AllIdentityAttributeNames.ToList();

                        if (allIdentityAttributeNames.Any())
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has identity attribute(s) {string.Join(", ", allIdentityAttributeNames)}. Set their 'Is Identity' property to false first.");

                        // Only support a subset of navigation mapping capabilities, specifically:
                        //    - They may never act as the principal end of a relationship.
                        string badAssociations = string.Join(", "
                                                           , store.ElementDirectory.AllElements
                                                                  .OfType<Association>()
                                                                  .Where(a => a.Principal == element)
                                                                  .Select(a => a.GetDisplayText()));

                        if (!string.IsNullOrEmpty(badAssociations))
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it is the principal end of association(s) {badAssociations}.");

                        //    - They may not have navigations to owned entities 
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.Target.IsDependentType)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.Target.IsDependentType)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.Source.IsDependentType))
                                                           .Select(a => a.GetDisplayText()));

                        if (!string.IsNullOrEmpty(badAssociations))
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has association(s) to dependent type(s) in {badAssociations}.");

                        //    - Entities cannot contain navigation properties to query types.
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.Target.IsQueryType)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.Target.IsQueryType)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.Source.IsQueryType))
                                                           .Select(a => a.GetDisplayText()));

                        errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has association to sql-mapped type(s) in {badAssociations}.");

                        //    - They can only contain reference navigation properties pointing to regular entities.
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.TargetMultiplicity == Multiplicity.ZeroMany)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.TargetMultiplicity == Multiplicity.ZeroMany)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.SourceMultiplicity == Multiplicity.ZeroMany))
                                                           .Select(a => a.GetDisplayText()));

                        errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has zero-to-many association(s) in {badAssociations}. Only to-one or to-zero-or-one associations are allowed. ");
                     }
                     else
                     {
                        // Restrictions:
                        // =================================
                        // Cannot have a key defined.
                        List<string> allIdentityAttributeNames = element.AllIdentityAttributeNames.ToList();

                        if (allIdentityAttributeNames.Any())
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has identity attribute(s) {string.Join(", ", allIdentityAttributeNames)}. Set their 'Is Identity' property to false first.");

                        // Only support a subset of navigation mapping capabilities, specifically:
                        //    - They may never act as the principal end of a relationship.
                        string badAssociations = string.Join(", "
                                                           , store.ElementDirectory.AllElements
                                                                  .OfType<Association>()
                                                                  .Where(a => a.Principal == element)
                                                                  .Select(a => a.GetDisplayText()));

                        if (!string.IsNullOrEmpty(badAssociations))
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it is the principal end of association(s) {badAssociations}.");

                        //    - They may not have navigations to owned entities 
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.Target.IsDependentType)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.Target.IsDependentType)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.Source.IsDependentType))
                                                           .Select(a => a.GetDisplayText()));

                        if (!string.IsNullOrEmpty(badAssociations))
                           errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has association(s) to dependent type(s) in {badAssociations}.");

                        //    - Entities cannot contain navigation properties to query types.
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.Target.IsQueryType)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.Target.IsQueryType)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.Source.IsQueryType))
                                                           .Select(a => a.GetDisplayText()));

                        errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has association to sql-mapped type(s) in {badAssociations}.");

                        //    - They can only contain reference navigation properties pointing to regular entities.
                        badAssociations = string.Join(", "
                                                    , store.ElementDirectory.AllElements
                                                           .OfType<Association>()
                                                           .Where(a => (a is UnidirectionalAssociation && a.Source == element && a.TargetMultiplicity == Multiplicity.ZeroMany)
                                                                    || (a is BidirectionalAssociation b && b.Source == element && b.TargetMultiplicity == Multiplicity.ZeroMany)
                                                                    || (a is BidirectionalAssociation c && c.Target == element && c.SourceMultiplicity == Multiplicity.ZeroMany))
                                                           .Select(a => a.GetDisplayText()));

                        errorMessages.Add($"{element.Name} can't be mapped to a Sql query since it has zero-to-many association(s) in {badAssociations}. Only to-one or to-zero-or-one associations are allowed. ");
                     }
                  }

                  break;
               }
            case "Name":
               {
                  string newName = (string)e.NewValue;

                  if (current.Name.ToLowerInvariant() != "paste" &&
                      (string.IsNullOrWhiteSpace(newName) || !CodeGenerator.IsValidLanguageIndependentIdentifier(newName)))
                     errorMessages.Add($"Class name '{newName}' isn't a valid .NET identifier.");

                  else if (store.ElementDirectory
                                .AllElements
                                .OfType<ModelClass>()
                                .Except(new[] { element })
                                .Any(x => x.Name == newName))
                     errorMessages.Add($"Class name '{newName}' already in use by another class");

                  else if (store.ElementDirectory
                                .AllElements
                                .OfType<ModelEnum>()
                                .Any(x => x.Name == newName))
                     errorMessages.Add($"Class name '{newName}' already in use by an enum");

                  else if (!string.IsNullOrEmpty((string)e.OldValue))
                  {
                     string oldDefaultName = MakeDefaultTableAndSetName((string)e.OldValue);
                     string newDefaultName = MakeDefaultTableAndSetName(newName);

                     if (element.DbSetName == oldDefaultName)
                        element.DbSetName = newDefaultName;

                     if (element.TableName == oldDefaultName)
                        element.TableName = newDefaultName;
                  }

                  break;
               }

            case "Namespace":
               {
                  string newNamespace = (string)e.NewValue;

                  if (current.Name.ToLowerInvariant() != "paste")
                     errorMessages.Add(CommonRules.ValidateNamespace(newNamespace, CodeGenerator.IsValidLanguageIndependentIdentifier));

                  break;
               }

            case "TableName":
               {
                  string newTableName = (string)e.NewValue;

                  if (element.IsDependentType)
                  {
                     if (!string.IsNullOrEmpty(newTableName))
                        element.TableName = string.Empty;
                  }
                  else
                  {
                     if (string.IsNullOrEmpty(newTableName))
                        element.TableName = MakeDefaultTableAndSetName(element.Name);

                     if (store.GetAll<ModelClass>()
                              .Except(new[] { element })
                              .Any(x => x.TableName == newTableName))
                        errorMessages.Add($"Table name '{newTableName}' already in use");
                  }

                  break;
               }
         }

         errorMessages = errorMessages.Where(m => m != null).ToList();

         if (errorMessages.Any())
         {
            current.Rollback();
            ErrorDisplay.Show(store, string.Join("\n", errorMessages));
         }
      }

      private string MakeDefaultTableAndSetName(string root)
      {
         return ModelRoot.PluralizationService?.IsSingular(root) == true
                   ? ModelRoot.PluralizationService.Pluralize(root)
                   : root;
      }
   }
}
