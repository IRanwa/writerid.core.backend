# WriterID External API Documentation

This document describes how to use the external API endpoints for integrating with external services.

## Authentication

The external API uses API Key authentication. You need to include the API key in the request headers.

### Headers Required:
```
X-API-Key: WID-API-2024-SecureKey-XYZ789
Content-Type: application/json
```

## Endpoints

### 1. Update Dataset Status

**Endpoint:** `PUT /api/external/datasets/status`

**Description:** Updates the status of a specific dataset.

**Request Body:**
```json
{
  "datasetId": "550e8400-e29b-41d4-a716-446655440000",
  "status": 2,
  "message": "Processing completed successfully"
}
```

**Status Values:**
- `0` = Created
- `1` = Processing  
- `2` = Completed
- `3` = Failed

**Response (Success):**
```json
{
  "message": "Dataset status updated successfully",
  "datasetId": "550e8400-e29b-41d4-a716-446655440000",
  "status": "Completed",
  "updatedAt": "2024-12-10T15:30:00Z"
}
```

**Example cURL:**
```bash
curl -X PUT "https://localhost:5001/api/external/datasets/status" \
  -H "X-API-Key: WID-API-2024-SecureKey-XYZ789" \
  -H "Content-Type: application/json" \
  -d '{
    "datasetId": "550e8400-e29b-41d4-a716-446655440000",
    "status": 2,
    "message": "Processing completed"
  }'
```

### 2. Get Dataset Status

**Endpoint:** `GET /api/external/datasets/{id}/status`

**Description:** Retrieves the current status of a dataset.

**Response:**
```json
{
  "datasetId": "550e8400-e29b-41d4-a716-446655440000",
  "status": "Processing",
  "containerName": "dataset-550e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-12-10T10:00:00Z",
  "updatedAt": "2024-12-10T15:30:00Z"
}
```

**Example cURL:**
```bash
curl -X GET "https://localhost:5001/api/external/datasets/550e8400-e29b-41d4-a716-446655440000/status" \
  -H "X-API-Key: WID-API-2024-SecureKey-XYZ789"
```



## Error Responses

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 404 Not Found
```json
{
  "message": "Dataset not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "Internal server error occurred while updating dataset status"
}
```

## Configuration

The API key is configured in `appsettings.json`:

```json
{
  "Authentication": {
    "ApiKey": "WID-API-2024-SecureKey-XYZ789"
  }
}
```

**Important:** Change the API key in production to a secure, randomly generated value!

## Usage Notes

1. All dataset IDs must be valid GUIDs
2. The API key must be included in every request
3. Dataset status updates will trigger automatic processing workflows
4. All timestamps are in UTC format
5. The API includes comprehensive logging for monitoring and debugging

## Swagger Documentation

When running in development mode, you can access the Swagger UI at:
- https://localhost:5001/swagger
- http://localhost:5000/swagger

The external endpoints will be documented alongside the regular API endpoints with API Key authentication options. 