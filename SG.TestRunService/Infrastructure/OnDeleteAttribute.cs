using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EF = Microsoft.EntityFrameworkCore;

namespace SG.TestRunService.Infrastructure
{
    /// <summary>
    /// Specifies `DeleteBehavior` on foreign keys
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OnDeleteAttribute : Attribute
    {
        public DeleteBehavior Behavior { get; set; }

        public OnDeleteAttribute(DeleteBehavior behavior)
        {
            Behavior = behavior;
        }

        public EF.DeleteBehavior EFDeleteBehavior =>
            Behavior switch
            {
                DeleteBehavior.Cascade => EF.DeleteBehavior.Cascade,
                DeleteBehavior.Restrict => EF.DeleteBehavior.Restrict,
                _ => throw new NotSupportedException()
            };

        /// <summary>
        /// Searches for all specified `OnDelete` attributes on properties related to foreign keys,
        /// and applies them on given `ModelBuilder`.
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void Apply(ModelBuilder modelBuilder)
        {
            var fks = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var fk in fks)
            {
                var allOnDeleteAttributes = FindAllOnDeleteAttributes(fk);
                if (allOnDeleteAttributes.Count > 0)
                {
                    ValidateConsistency(allOnDeleteAttributes);

                    fk.DeleteBehavior = allOnDeleteAttributes[0].Attrib.EFDeleteBehavior;
                }
            }
        }

        private static void ValidateConsistency(List<(PropertyInfo Property, OnDeleteAttribute Attrib)> allOnDeleteAttributes)
        {
            var behavior = allOnDeleteAttributes[0].Attrib.Behavior;
            var (inconsistentProperty, _) = allOnDeleteAttributes.FirstOrDefault(a => a.Attrib.Behavior != behavior);
            if (inconsistentProperty != null)
            {
                var property1 = allOnDeleteAttributes[0].Property;
                var property2 = inconsistentProperty;
                var message = "Inconsistent DeleteBehavior: `OnDelete` attribute specified" +
                    $" on `{GetName(property1)}`, does not math the one specified on `{GetName(property2)}`.";
                throw new Exception(message);
            }
        }

        private static List<(PropertyInfo Property, OnDeleteAttribute Attrib)>
            FindAllOnDeleteAttributes(EF.Metadata.IMutableForeignKey fk)
        {
            var participatingProperties = Enumerable.Empty<PropertyInfo>()
                    .Concat(fk.Properties.Select(p => p.PropertyInfo))
                    .Append(fk.DependentToPrincipal?.PropertyInfo)
                    .Append(fk.PrincipalToDependent?.PropertyInfo)
                    .Where(p => p != null);

            var allOnDeleteAttributes = participatingProperties
                .Select(p => (Property: p, Attrib: p.GetCustomAttribute<OnDeleteAttribute>()))
                .Where(a => a.Attrib != null).ToList();
            return allOnDeleteAttributes;
        }

        private static string GetName(PropertyInfo pi)
        {
            return pi.DeclaringType.Name + "." + pi.Name;
        }
    }

    public enum DeleteBehavior
    {
        Restrict = 1,
        Cascade = 3
    }
}
