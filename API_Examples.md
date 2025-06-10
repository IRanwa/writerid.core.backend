# WriterID API Examples - New Interactive Workflow

## Task Creation Workflow

### Step 1: Get Dataset Analysis (Writers List)

First, get the list of available writers from a dataset analysis:

```json
GET /api/tasks/dataset/{datasetId}/analysis
```

**Response Example:**
```json
{
  "datasetId": "123e4567-e89b-12d3-a456-426614174000",
  "datasetName": "Historical Documents Dataset",
  "status": "Completed",
  "analyzedAt": "2024-06-10T15:30:00Z",
  "writers": [
    {
      "writerId": "writer_001",
      "writerName": "Writer 1", 
      "sampleCount": 25,
      "confidence": 0.95
    },
    {
      "writerId": "writer_002", 
      "writerName": "Writer 2",
      "sampleCount": 18,
      "confidence": 0.92
    },
    {
      "writerId": "writer_003",
      "writerName": "Writer 3", 
      "sampleCount": 30,
      "confidence": 0.98
    }
  ]
}
```

#### **Azure Storage Analysis Results Format**

The system retrieves analysis results from Azure Storage. The analysis results should be stored as `analysis-results.json` in the dataset's container. The system extracts only the writer names from the following JSON format:

**Expected Format:**
```json
{
  "num_writers": 15,
  "writer_names": [
    "091",
    "092", 
    "093",
    "094",
    "095",
    "096",
    "097",
    "098",
    "099",
    "100",
    "101",
    "102",
    "103",
    "104",
    "105"
  ],
  "min_samples": 54,
  "max_samples": 123,
  "writer_counts": {
    "091": 105,
    "092": 100,
    "093": 100,
    "094": 123,
    "095": 107,
    "096": 111,
    "097": 115,
    "098": 113,
    "099": 115,
    "100": 115,
    "101": 111,
    "102": 54,
    "103": 112,
    "104": 114,
    "105": 98
  }
}
```

The system will:
1. Extract only the `writer_names` array
2. Create writer entries using the name as both ID and display name
3. Set sample count to 0 and confidence to 1.0 for all writers

**Resulting API Response:**
```json
{
  "datasetId": "123e4567-e89b-12d3-a456-426614174000",
  "datasetName": "Historical Documents Dataset",
  "status": "Completed", 
  "analyzedAt": "2024-06-10T15:30:00Z",
  "writers": [
    {
      "writerId": "091",
      "writerName": "Writer 091",
      "sampleCount": 0,
      "confidence": 1.0
    },
    {
      "writerId": "092",
      "writerName": "Writer 092", 
      "sampleCount": 0,
      "confidence": 1.0
    }
    // ... more writers
  ]
}
```

If no analysis results are found in storage, the system will return empty results.

### Step 2: Create Task with Selected Writers and Query Image

**Endpoint:** `POST /api/tasks`

**Description:** Creates a new writer identification task with selected writers and base64 encoded query image, then immediately executes the prediction.

**Request Body:**
```json
{
  "name": "Historical Document Analysis",
  "description": "Identify writer of historical document sample",
  "datasetId": "123e4567-e89b-12d3-a456-426614174000",
  "selectedWriters": ["091", "092", "093"],
  "queryImageBase64": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
  "useDefaultModel": true,
  "modelId": null
}
```

**Success Response:** `201 Created`
```json
{
  "message": "Task created and prediction started successfully"
}
```

**Failure Response:** `400 Bad Request`
```json
{
  "message": "Task created but failed to start prediction execution"
}
```

**Important Notes:**
- **Query Image**: Send the image as base64 encoded string (with or without data URL prefix)
- **Container Creation**: System automatically creates container named `task-{taskId}`
- **Image Storage**: Base64 image is converted and stored as `query.png` in the task container
- **Success**: Returns `201 Created` with simple success message
- **Failure**: Returns `400 Bad Request` if prediction execution fails to start
- Task is always created in database, but execution may fail
- The prediction API is called automatically with the task ID
- Results will be populated via the callback endpoint when prediction completes

**Workflow:**
1. Task record is created in database with status "Created"
2. Task container `task-{taskId}` is created in Azure Storage
3. Base64 query image is converted and uploaded as `query.png`
4. Task is updated with query image path (`task-{taskId}/query.png`)
5. Task status is updated to "Processing"
6. Prediction API is called at the configured endpoint (default: localhost:5000/predict)
7. **If successful**: Returns `201 Created` with success message
8. **If failed**: Task status set to "Failed", returns `400 Bad Request`
9. When prediction completes, results are sent back via callback to `POST /api/tasks/{id}/results`

### Step 3: Execute Task

**Note:** Task execution now happens automatically when creating a task. This section is maintained for reference but the manual execution endpoint is no longer needed in the typical workflow.

**Endpoint:** `POST /api/tasks/{id}/execute`

**Description:** Manually starts execution of a previously created task (optional, since execution is automatic).

**Response:** `200 OK`
```json
{
  "message": "Task execution started successfully",
  "taskId": "987fcdeb-51a2-43b1-9f4e-123456789abc",
  "status": "Processing"
}
```

## View Tasks List

Get all tasks for the current user:

```json
GET /api/tasks
```

**Response Example:**
```json
[
  {
    "id": "789e1234-e89b-12d3-a456-426614174002",
    "name": "Identify Writer in Historical Document",
    "description": "Compare query document against selected historical writers", 
    "useDefaultModel": true,
    "modelId": null,
    "datasetId": "123e4567-e89b-12d3-a456-426614174000",
    "selectedWriters": ["writer_001", "writer_003", "writer_002"],
    "queryImagePath": "https://storage.example.com/query-images/document123.jpg",
    "status": "Completed",
    "resultsJson": "{\"predictions\": [{\"writerId\": \"writer_001\", \"confidence\": 0.87}, {\"writerId\": \"writer_003\", \"confidence\": 0.82}]}",
    "userId": 1,
    "createdAt": "2024-06-10T21:02:19.123Z",
    "updatedAt": "2024-06-10T21:05:30.456Z"
  }
]
```

## Task Executor Integration

### HTTP Call to Executor

When a task is executed, this call is made:

**URL:** `http://localhost:5000/predict` (configurable)

**Request Body:**
```json
{
  "task_id": "789e1234-e89b-12d3-a456-426614174002"
}
```

### Executor Gets Task Details

The executor can get full task details:

```json
GET /api/tasks/{taskId}/execution-details
```

**Response Example:**
```json
{
  "taskId": "789e1234-e89b-12d3-a456-426614174002",
  "useDefaultModel": true,
  "modelId": null,
  "datasetId": "123e4567-e89b-12d3-a456-426614174000",
  "selectedWriters": ["writer_001", "writer_003", "writer_002"],
  "queryImagePath": "https://storage.example.com/query-images/document123.jpg",
  "status": "Processing",
  "createdAt": "2024-06-10T21:02:19.123Z",
  "name": "Identify Writer in Historical Document",
  "description": "Compare query document against selected historical writers"
}
```

### Executor Returns Results

The executor updates the task with results:

```json
POST /api/tasks/{taskId}/results
{
  "resultsJson": "{\"predictions\": [{\"writerId\": \"writer_001\", \"confidence\": 0.87}, {\"writerId\": \"writer_003\", \"confidence\": 0.82}, {\"writerId\": \"writer_002\", \"confidence\": 0.45}], \"processing_time\": 2.3, \"model_used\": \"default_model.pth\"}",
  "status": 2
}
```

**Status Values:**
- `0` = Created
- `1` = Processing  
- `2` = Completed
- `3` = Failed

## New Workflow Benefits

- **🎯 Selective Analysis**: Choose specific writers instead of analyzing all
- **📸 Query-Based**: Upload specific images for comparison
- **🔄 Interactive**: Step-by-step workflow with dataset exploration
- **📊 Rich Results**: Detailed confidence scores and processing metrics
- **⚡ Flexible Models**: Default or custom model selection
- **🔗 API Integration**: Seamless integration with task executor
- **📋 Task Management**: Complete task lifecycle management
- **☁️ Cloud Storage**: Analysis results retrieved from Azure Storage

## Implementation Flow

1. **Frontend**: User selects dataset → shows available writers
2. **Frontend**: User selects specific writers + uploads query image
3. **Frontend**: User creates task with selections
4. **Frontend**: User clicks execute → API calls task executor
5. **Executor**: Receives task_id → gets full details via API
6. **Executor**: Processes query image against selected writers
7. **Executor**: Returns results via API callback
8. **Frontend**: Shows results to user 