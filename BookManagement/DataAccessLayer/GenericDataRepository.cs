using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class GenericDataRepository<T> : IGenericDataRepository<T> where T : class
    {
        // Add
        public virtual async Task Add(params T[] items)
        {
            try
            {
                using (var context = new WPFBookManagementEntities())
                {
                    foreach (T item in items)
                    {
                        context.Entry(item).State = EntityState.Added;
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public async Task<IList<T>> GetAllAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list = new List<T>();
            await Task.Run(() =>
            {
                using (var context = new WPFBookManagementEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    list = dbQuery
                        .AsNoTracking()
                        .ToList<T>();

                }
            });
            return list;

        }


        public async Task<IList<T>> GetListAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list = new List<T>();
            await Task.Run(() =>
            {
                using (var context = new WPFBookManagementEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    list = dbQuery
                        .AsNoTracking()
                        .Where(where)
                        .ToList<T>();
                }
            });
            return list;
        }


        public async Task<T> GetSingleAsync(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            await Task.Run(() =>
            {
                using (var context = new WPFBookManagementEntities())
                {
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    item = dbQuery
                        .AsNoTracking() //Don't track any changes for the selected item
                        .FirstOrDefault(where); //Apply where clause
                }
            });
            return item;
        }

        public async Task Remove(params T[] items)
        {

            using (var context = new WPFBookManagementEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = EntityState.Deleted;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(params T[] items)
        {
            using (var context = new WPFBookManagementEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }
    }
}