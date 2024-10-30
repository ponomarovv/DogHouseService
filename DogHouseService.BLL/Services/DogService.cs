﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DogHouseService.BLL.Interfaces;
using DogHouseService.BLL.Models;
using DogHouseService.DAL.Data;
using DogHouseService.DAL.Models;

namespace DogHouseService.BLL.Services
{
    public class DogService : IDogService
    {
        private readonly DogHouseContext _context;
        private readonly IMapper _mapper;

        public DogService(DogHouseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DogModel>> GetDogsAsync(string attribute = null, string order = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Dogs.AsQueryable();

            if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
            {
                if (attribute == "name" || attribute == "color" || attribute == "tail_length" || attribute == "weight")
                {
                    query = order.ToLower() == "desc" ? query.OrderByDescending(attribute) : query.OrderBy(attribute);
                }
            }

            var dogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DogModel>>(dogs);
        }

        public async Task<DogModel> CreateDogAsync(DogModel dogModel)
        {
            if (await _context.Dogs.AnyAsync(d => d.Name == dogModel.Name))
            {
                throw new InvalidOperationException("A dog with the same name already exists.");
            }

            if (dogModel.TailLength < 0)
            {
                throw new InvalidOperationException("Tail length cannot be a negative number.");
            }

            var dog = _mapper.Map<Dog>(dogModel);
            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();

            return _mapper.Map<DogModel>(dog);
        }
    }
}
