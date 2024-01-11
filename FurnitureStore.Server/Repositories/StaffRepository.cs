﻿using FurnitureStore.Server.Utils;
using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels.PasswordModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using System.Security.Authentication;

namespace FurnitureStore.Server.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly Container _staffContainer;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public StaffRepository(CosmosClient cosmosClient, IMapper mapper, IMemoryCache memoryCache)
        {
            var databaseName = cosmosClient.ClientOptions.ApplicationName;
            var containerName = "staffs";

            _staffContainer = cosmosClient.GetContainer(databaseName, containerName);
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task AddStaffDocumentAsync(StaffDocument item)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.ModifiedAt = item.CreatedAt;


            await _staffContainer.UpsertItemAsync(
                item: item,
                partitionKey: new PartitionKey(item.StaffId)
            );
        }

        public async Task AddStaffDTOAsync(StaffDTO staffDTO)
        {
            var staffDoc = _mapper.Map<StaffDocument>(staffDTO);

            await AddStaffDocumentAsync(staffDoc);
        }

        public async Task UpdateStaffAsync(StaffDTO staffDTO)
        {
            var staffToUpdate = _mapper.Map<StaffDocument>(staffDTO);

            await _staffContainer.UpsertItemAsync(
                item: staffToUpdate,
                partitionKey: new PartitionKey(staffToUpdate.StaffId)
            );
        }

        public async Task<IEnumerable<StaffDTO>> GetStaffDTOsAsync()
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM po"
            );

            var staffDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<StaffDocument>(_staffContainer, queryDef);
            var staffDTOs = staffDocs.Select(staffDoc =>
            {
                return _mapper.Map<StaffDTO>(staffDoc);
            }).ToList();

            return staffDTOs;
        }

        public async Task<string> GetNewStaffIdAsync()
        {
            if (_memoryCache.TryGetValue("LastestStaffId", out string? lastestId))
            {
                if (!String.IsNullOrEmpty(lastestId))
                    return lastestId;
            }

            // Query the database to get the latest product ID
            QueryDefinition queryDef = new QueryDefinition(
                query:
                "SELECT TOP 1 po.id " +
                "FROM po " +
                "ORDER BY po.id DESC"
            );

            string currLastestId = (await CosmosDbUtils.GetDocumentByQueryDefinition<ResponseToGetId>(_staffContainer, queryDef))!.Id;
            string newId = IdUtils.IncreaseId(currLastestId);

            _memoryCache.Set("LastestStaffId", newId);
            return newId;
        }

        public async Task<StaffDTO> GetStaffDTOByIdAsync(string id)
        {
            var staffDoc = await GetStaffDocumentByIdAsync(id);

            var staffDTO = _mapper.Map<StaffDTO>(staffDoc);

            return staffDTO;
        }

        public async Task DeleteStaffAsync(string id)
        {
            //var staffDoc = await GetStaffDocumentByIdAsync(id);

            //if (staffDoc == null)
            //{
            //    throw new Exception("Staff Not found!");
            //}

            //List<PatchOperation> patchOperations = new List<PatchOperation>()
            //{
            //    PatchOperation.Replace("/isDeleted", true)
            //};

            //await _staffContainer.PatchItemAsync<StaffDocument>(id, new PartitionKey(staffDoc.StaffId), patchOperations);
        }

        private async Task<StaffDocument?> GetStaffDocumentByIdAsync(string id)
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM po " +
                    "WHERE po.id = @id"
            ).WithParameter("@id", id);

            var staff = await CosmosDbUtils.GetDocumentByQueryDefinition<StaffDocument>(_staffContainer, queryDef);

            return staff;
        }

        public async Task UpdatePasswordAsync(UpdatePasswordModel data)
        {
            LoginModel loginModel = new()
            {
                Email = data.Email,
                Password = data.OldPassword
            };
            try
            {
                StaffDocument staffDoc = await GetStaffDocByCredentials(loginModel);

                var hashedAndSaltedPassword = SecretHasher.Hash(data.NewPassword);
                staffDoc.ModifiedAt = DateTime.UtcNow;
                staffDoc.HashedAndSaltedPassword = hashedAndSaltedPassword;
                staffDoc.DefaultPassword = null;

                await _staffContainer.UpsertItemAsync(
                    item: staffDoc,
                    partitionKey: new PartitionKey(staffDoc.StaffId)
                );
            }
            catch (DocumentNotFoundException) 
            {
                throw;
            }
            catch (InvalidCredentialException)
            {
                throw;
            }
        }

        public async Task<StaffDocument?> GetStaffDocumentByEmailAsync(string email)
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM c " +
                    "WHERE c.isDeleted = false AND STRINGEQUALS(c.contact.email, @email, true)"
            ).WithParameter("@email", email);

            
                var staff = await CosmosDbUtils.GetDocumentByQueryDefinition<StaffDocument>(_staffContainer, queryDef);
                return staff;
            
        }

        public async Task<StaffDTO> GetStaffDTOByCredentials(LoginModel data)
        {
            try
            {
                var staffDoc = await GetStaffDocByCredentials(data);
                return _mapper.Map<StaffDTO>(staffDoc);
            }
            catch (DocumentNotFoundException)
            {
                throw;
            }
            catch(InvalidCredentialException)
            {
                throw;
            }
        }

        private async Task<StaffDocument> GetStaffDocByCredentials(LoginModel data)
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM s " +
                    "WHERE s.contact.email = @email "
            ).WithParameter("@email", data.Email);

            var staffDoc = await CosmosDbUtils.GetDocumentByQueryDefinition<StaffDocument>(_staffContainer, queryDef)
                ?? throw new DocumentNotFoundException($"Staff with email {data.Email} not found.");

            var isValidByMainPassword = false;
            var isValidByDefaultPassword = false;

            if (!string.IsNullOrEmpty(staffDoc.DefaultPassword))
            {
                isValidByDefaultPassword = staffDoc.DefaultPassword == data.Password;
            }

            if (!string.IsNullOrEmpty(staffDoc.HashedAndSaltedPassword))
            {
                isValidByMainPassword = SecretHasher.Verify(data.Password, staffDoc.HashedAndSaltedPassword);
            }

            if (isValidByMainPassword || isValidByDefaultPassword)
            {
                return staffDoc;
            }

            throw new InvalidCredentialException();
        }

    }
}
