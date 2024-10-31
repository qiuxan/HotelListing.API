﻿using HotelListing.API.Models;

namespace HotelListing.API.Contracts;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetAsync(int? id);
    Task<TResult?> GetAsync<TResult>(int? id);

    Task<List<T>> GetAllAsync();
    Task<List<TResult>> GetAllAsync<TResult>();

    Task<PageResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);

    Task<T> AddAsync(T entity);
    Task<TResult> AddAsync<TResult, TSource>(TSource source);

    Task UpdateAsync(T entity);
    Task UpdateAsync<TSource>(int id, TSource source);

    Task DeleteAsync(int id);

    Task<bool> Exists(int id);

}
