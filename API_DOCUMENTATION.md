# Hướng Dẫn Sử Dụng API

## Mục Lục
1. [Cấu trúc Response](#cấu-trúc-response)
2. [WorkShifts API](#workshifts-api)
3. [Role API](#role-api)
4. [Lưu ý chung](#lưu-ý-chung)

---

## Cấu trúc Response

Tất cả các API đều trả về format chuẩn với cấu trúc:

```json
{
  "code": 200,
  "data": {...},
  "message": "Thành công"
}
```

### Các mã code (ResponseCode):
- **200**: Thành công (Success)
- **201**: Tạo mới thành công (Created)
- **400**: Dữ liệu không hợp lệ (BadRequest)
- **404**: Không tìm thấy (NotFound)
- **409**: Xung đột dữ liệu (Conflict)
- **500**: Lỗi server (InternalServerError)

### Xử lý Response trong Frontend:

```javascript
// Ví dụ hàm xử lý response
async function callAPI(url, options) {
  try {
    const response = await fetch(url, options);
    const result = await response.json();
    
    // Kiểm tra code trong response
    if (result.code === 200 || result.code === 201) {
      // Thành công
      return {
        success: true,
        data: result.data,
        message: result.message
      };
    } else {
      // Có lỗi
      return {
        success: false,
        code: result.code,
        message: result.message,
        data: result.data
      };
    }
  } catch (error) {
    return {
      success: false,
      code: 500,
      message: 'Lỗi kết nối đến server',
      error: error.message
    };
  }
}
```

---

## WorkShifts API

Base URL: `/api/WorkShifts`

### 1. Lấy danh sách ca làm việc (có phân trang)

**Endpoint:** `POST /api/WorkShifts/get-all`

**Mô tả:** Lấy danh sách ca làm việc có phân trang, tìm kiếm và sắp xếp

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 10,
  "keyword": "ca sáng",
  "columnFilters": [
    {
      "columnName": "WorkShiftName",
      "filterValue": "ca sáng",
      "operator": "contains"
    }
  ],
  "columnSorts": [
    {
      "columnName": "WorkShiftCode",
      "sortDirection": "asc"
    }
  ]
}
```

**Response (200 OK):**
```json
{
  "code": 200,
  "data": {
    "items": [
      {
        "workShiftId": "123e4567-e89b-12d3-a456-426614174000",
        "workShiftCode": "CA001",
        "workShiftName": "Ca sáng",
        "beginShiftTime": "08:00:00",
        "endShiftTime": "12:00:00",
        "beginBreakTime": "10:00:00",
        "endBreakTime": "10:15:00",
        "workingTime": 240,
        "breakTime": 15,
        "isActive": 1,
        "createdByName": "Nguyễn Văn A",
        "createdDate": "2025-01-01T08:00:00",
        "modifiedByName": null,
        "modifiedDate": null
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "message": "Thành công"
}
```

**Response (400 Bad Request):**
```json
{
  "code": 400,
  "data": null,
  "message": "Dữ liệu không hợp lệ"
}
```

**Ví dụ sử dụng (JavaScript/Fetch):**
```javascript
const response = await fetch('/api/WorkShifts/get-all', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    page: 1,
    pageSize: 10,
    keyword: 'ca sáng'
  })
});

const result = await response.json();

if (result.code === 200) {
  console.log('Danh sách ca làm việc:', result.data.items);
  console.log('Tổng số:', result.data.total);
} else {
  console.error('Lỗi:', result.message);
}
```

**Ví dụ sử dụng (Axios):**
```javascript
try {
  const response = await axios.post('/api/WorkShifts/get-all', {
    page: 1,
    pageSize: 10,
    keyword: 'ca sáng'
  });
  
  if (response.data.code === 200) {
    console.log('Danh sách:', response.data.data.items);
  }
} catch (error) {
  console.error('Lỗi:', error.response?.data?.message || error.message);
}
```

---

### 2. Lấy thông tin chi tiết ca làm việc

**Endpoint:** `GET /api/WorkShifts/{id}`

**Mô tả:** Lấy thông tin chi tiết của một ca làm việc theo ID

**Path Parameters:**
- `id` (Guid, required): ID của ca làm việc

**Response (200 OK):**
```json
{
  "code": 200,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "workShiftCode": "CA001",
    "workShiftName": "Ca sáng",
    "beginShiftTime": "08:00:00",
    "endShiftTime": "12:00:00",
    "beginBreakTime": "10:00:00",
    "endBreakTime": "10:15:00",
    "description": "Ca làm việc buổi sáng",
    "isActive": 1,
    "createdBy": "123e4567-e89b-12d3-a456-426614174000",
    "createdDate": "2025-01-01T08:00:00",
    "modifiedBy": null,
    "modifiedDate": null
  },
  "message": "Thành công"
}
```

**Response (404 Not Found):**
```json
{
  "code": 404,
  "data": null,
  "message": "Không tìm thấy dữ liệu"
}
```

**Ví dụ sử dụng:**
```javascript
const workShiftId = '123e4567-e89b-12d3-a456-426614174000';
const response = await fetch(`/api/WorkShifts/${workShiftId}`);
const result = await response.json();

if (result.code === 200) {
  console.log('Chi tiết ca làm việc:', result.data);
} else if (result.code === 404) {
  console.error('Không tìm thấy ca làm việc');
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 3. Kiểm tra mã ca làm việc đã tồn tại

**Endpoint:** `GET /api/WorkShifts/check-code`

**Mô tả:** Kiểm tra mã ca làm việc đã tồn tại trong hệ thống chưa (dùng để validate trước khi tạo/cập nhật)

**Query Parameters:**
- `code` (string, required): Mã ca làm việc cần kiểm tra
- `excludeId` (Guid, optional): ID ca làm việc cần loại trừ (dùng khi cập nhật để không kiểm tra chính nó)

**Response (200 OK - Mã đã tồn tại):**
```json
{
  "code": 200,
  "data": true,
  "message": "Mã đã tồn tại trong hệ thống"
}
```

**Response (200 OK - Mã chưa tồn tại):**
```json
{
  "code": 200,
  "data": false,
  "message": "Mã chưa tồn tại trong hệ thống"
}
```

**Ví dụ sử dụng:**
```javascript
// Kiểm tra khi tạo mới
const code = 'CA001';
const response = await fetch(
  `/api/WorkShifts/check-code?code=${encodeURIComponent(code)}`
);
const result = await response.json();

if (result.code === 200) {
  if (result.data === true) {
    console.log('Mã đã tồn tại, không thể tạo mới');
  } else {
    console.log('Mã chưa tồn tại, có thể tạo mới');
  }
}

// Kiểm tra khi cập nhật (loại trừ ID hiện tại)
const excludeId = '123e4567-e89b-12d3-a456-426614174000';
const response2 = await fetch(
  `/api/WorkShifts/check-code?code=${encodeURIComponent(code)}&excludeId=${excludeId}`
);
const result2 = await response2.json();
```

---

### 4. Tạo mới ca làm việc

**Endpoint:** `POST /api/WorkShifts/create`

**Mô tả:** Tạo mới một ca làm việc

**Request Body:**
```json
{
  "workShiftCode": "CA001",
  "workShiftName": "Ca sáng",
  "beginShiftTime": "08:00:00",
  "endShiftTime": "12:00:00",
  "beginBreakTime": "10:00:00",
  "endBreakTime": "10:15:00",
  "description": "Ca làm việc buổi sáng",
  "createdBy": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Lưu ý:**
- `beginShiftTime`, `endShiftTime`: Format `HH:mm:ss` hoặc `HH:mm` (TimeSpan)
- `beginBreakTime`, `endBreakTime`: Có thể null (TimeSpan?)
- `createdBy`: ID của người tạo (Guid)

**Response (201 Created):**
```json
{
  "code": 201,
  "data": "123e4567-e89b-12d3-a456-426614174000",
  "message": "Tạo mới thành công"
}
```

**Response (400 Bad Request):**
```json
{
  "code": 400,
  "data": null,
  "message": "Dữ liệu không hợp lệ"
}
```

**Response (409 Conflict):**
```json
{
  "code": 409,
  "data": null,
  "message": "Mã đã tồn tại trong hệ thống"
}
```

**Ví dụ sử dụng:**
```javascript
const response = await fetch('/api/WorkShifts/create', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    workShiftCode: 'CA001',
    workShiftName: 'Ca sáng',
    beginShiftTime: '08:00:00',
    endShiftTime: '12:00:00',
    beginBreakTime: '10:00:00',
    endBreakTime: '10:15:00',
    description: 'Ca làm việc buổi sáng',
    createdBy: '123e4567-e89b-12d3-a456-426614174000'
  })
});

const result = await response.json();

if (result.code === 201) {
  console.log('Ca làm việc đã được tạo với ID:', result.data);
  console.log('Thông báo:', result.message);
} else if (result.code === 409) {
  console.error('Mã đã tồn tại:', result.message);
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 5. Cập nhật ca làm việc

**Endpoint:** `PUT /api/WorkShifts/{id}`

**Mô tả:** Cập nhật thông tin của một ca làm việc

**Path Parameters:**
- `id` (Guid, required): ID của ca làm việc cần cập nhật

**Request Body:**
```json
{
  "workShiftCode": "CA001",
  "workShiftName": "Ca sáng (đã cập nhật)",
  "beginShiftTime": "08:00:00",
  "endShiftTime": "12:00:00",
  "beginBreakTime": "10:00:00",
  "endBreakTime": "10:15:00",
  "description": "Ca làm việc buổi sáng",
  "isActive": 1,
  "modifiedBy": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Lưu ý:**
- `isActive`: 1 = Sử dụng, 0 = Ngừng sử dụng
- `modifiedBy`: ID của người cập nhật (Guid)

**Response (200 OK):**
```json
{
  "code": 200,
  "data": 1,
  "message": "Cập nhật thành công"
}
```

**Response (404 Not Found):**
```json
{
  "code": 404,
  "data": null,
  "message": "Không tìm thấy dữ liệu"
}
```

**Response (409 Conflict):**
```json
{
  "code": 409,
  "data": null,
  "message": "Mã đã tồn tại trong hệ thống"
}
```

**Ví dụ sử dụng:**
```javascript
const workShiftId = '123e4567-e89b-12d3-a456-426614174000';
const response = await fetch(`/api/WorkShifts/${workShiftId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    workShiftCode: 'CA001',
    workShiftName: 'Ca sáng (đã cập nhật)',
    beginShiftTime: '08:00:00',
    endShiftTime: '12:00:00',
    beginBreakTime: '10:00:00',
    endBreakTime: '10:15:00',
    description: 'Ca làm việc buổi sáng',
    isActive: 1,
    modifiedBy: '123e4567-e89b-12d3-a456-426614174000'
  })
});

const result = await response.json();

if (result.code === 200) {
  console.log('Đã cập nhật', result.data, 'bản ghi');
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 6. Xóa một ca làm việc

**Endpoint:** `DELETE /api/WorkShifts/{id}`

**Mô tả:** Xóa một ca làm việc theo ID

**Path Parameters:**
- `id` (Guid, required): ID của ca làm việc cần xóa

**Response (200 OK):**
```json
{
  "code": 200,
  "data": 1,
  "message": "Xóa thành công"
}
```

**Response (404 Not Found):**
```json
{
  "code": 404,
  "data": null,
  "message": "Không tìm thấy dữ liệu"
}
```

**Ví dụ sử dụng:**
```javascript
const workShiftId = '123e4567-e89b-12d3-a456-426614174000';
const response = await fetch(`/api/WorkShifts/${workShiftId}`, {
  method: 'DELETE'
});

const result = await response.json();

if (result.code === 200) {
  console.log('Đã xóa', result.data, 'bản ghi');
  console.log('Thông báo:', result.message);
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 7. Xóa nhiều ca làm việc

**Endpoint:** `DELETE /api/WorkShifts/multiple-delete`

**Mô tả:** Xóa nhiều ca làm việc cùng lúc

**Request Body:**
```json
{
  "ids": [
    "123e4567-e89b-12d3-a456-426614174000",
    "223e4567-e89b-12d3-a456-426614174001",
    "323e4567-e89b-12d3-a456-426614174002"
  ]
}
```

**Response (200 OK):**
```json
{
  "code": 200,
  "data": 3,
  "message": "Xóa nhiều bản ghi thành công"
}
```

**Response (400 Bad Request):**
```json
{
  "code": 400,
  "data": null,
  "message": "Dữ liệu không hợp lệ"
}
```

**Ví dụ sử dụng:**
```javascript
const response = await fetch('/api/WorkShifts/multiple-delete', {
  method: 'DELETE',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    ids: [
      '123e4567-e89b-12d3-a456-426614174000',
      '223e4567-e89b-12d3-a456-426614174001',
      '323e4567-e89b-12d3-a456-426614174002'
    ]
  })
});

const result = await response.json();

if (result.code === 200) {
  console.log('Đã xóa', result.data, 'bản ghi');
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 8. Cập nhật trạng thái nhiều ca làm việc

**Endpoint:** `PUT /api/WorkShifts/status`

**Mô tả:** Cập nhật trạng thái (sử dụng/ngừng sử dụng) cho nhiều ca làm việc cùng lúc

**Request Body:**
```json
{
  "ids": [
    "123e4567-e89b-12d3-a456-426614174000",
    "223e4567-e89b-12d3-a456-426614174001"
  ],
  "isActive": 1,
  "modifiedBy": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Lưu ý:**
- `isActive`: 1 = Sử dụng, 0 = Ngừng sử dụng
- `modifiedBy`: ID của người cập nhật (Guid)

**Response (200 OK):**
```json
{
  "code": 200,
  "data": 2,
  "message": "Cập nhật trạng thái thành công"
}
```

**Ví dụ sử dụng:**
```javascript
const response = await fetch('/api/WorkShifts/status', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    ids: [
      '123e4567-e89b-12d3-a456-426614174000',
      '223e4567-e89b-12d3-a456-426614174001'
    ],
    isActive: 1,
    modifiedBy: '123e4567-e89b-12d3-a456-426614174000'
  })
});

const result = await response.json();

if (result.code === 200) {
  console.log('Đã cập nhật trạng thái cho', result.data, 'bản ghi');
} else {
  console.error('Lỗi:', result.message);
}
```

---

### 9. Xuất danh sách ca làm việc ra file CSV

**Endpoint:** `POST /api/WorkShifts/export`

**Mô tả:** Xuất danh sách ca làm việc ra file CSV theo điều kiện lọc

**Request Body:**
```json
{
  "page": 1,
  "pageSize": 1000,
  "keyword": "ca sáng",
  "columnFilters": [],
  "columnSorts": []
}
```

**Response (200 OK):**
- Content-Type: `text/csv`
- File download với tên: `work-shifts-{yyyyMMddHHmmss}.csv`

**Response (400 Bad Request):**
```json
{
  "code": 400,
  "data": null,
  "message": "Dữ liệu không hợp lệ"
}
```

**Ví dụ sử dụng:**
```javascript
const response = await fetch('/api/WorkShifts/export', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    page: 1,
    pageSize: 1000,
    keyword: 'ca sáng'
  })
});

if (response.ok && response.headers.get('content-type')?.includes('text/csv')) {
  // Xử lý file CSV
  const blob = await response.blob();
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `work-shifts-${new Date().toISOString().replace(/[:.]/g, '-')}.csv`;
  document.body.appendChild(a);
  a.click();
  window.URL.revokeObjectURL(url);
  document.body.removeChild(a);
} else {
  // Xử lý lỗi
  const result = await response.json();
  console.error('Lỗi:', result.message);
}
```

---

## Role API

Base URL: `/api/Role`

### 1. Lấy danh sách vai trò

**Endpoint:** `GET /api/Role`

**Mô tả:** Lấy danh sách tất cả vai trò trong hệ thống

**Response (200 OK):**
```json
{
  "code": 200,
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "roleName": "Admin",
      "description": "Quản trị viên hệ thống"
    },
    {
      "id": "223e4567-e89b-12d3-a456-426614174001",
      "roleName": "User",
      "description": "Người dùng thông thường"
    }
  ],
  "message": "Thành công"
}
```

**Response (500 Internal Server Error):**
```json
{
  "code": 500,
  "data": null,
  "message": "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau."
}
```

**Ví dụ sử dụng:**
```javascript
const response = await fetch('/api/Role');
const result = await response.json();

if (result.code === 200) {
  console.log('Danh sách vai trò:', result.data);
} else {
  console.error('Lỗi:', result.message);
}
```

---

## Lưu ý chung

### Cấu trúc Response chuẩn

Tất cả API đều trả về format:
```json
{
  "code": number,
  "data": any,
  "message": string
}
```

### Xử lý lỗi

Tất cả các API có thể trả về các mã lỗi sau:
- **200**: Thành công
- **201**: Tạo mới thành công
- **400**: Dữ liệu request không hợp lệ
- **404**: Không tìm thấy resource
- **409**: Xung đột dữ liệu (ví dụ: mã đã tồn tại)
- **500**: Lỗi server

**Ví dụ xử lý lỗi tổng quát:**
```javascript
async function handleAPIResponse(response) {
  const result = await response.json();
  
  switch (result.code) {
    case 200:
    case 201:
      return {
        success: true,
        data: result.data,
        message: result.message
      };
    
    case 400:
      return {
        success: false,
        error: 'Dữ liệu không hợp lệ',
        message: result.message
      };
    
    case 404:
      return {
        success: false,
        error: 'Không tìm thấy',
        message: result.message
      };
    
    case 409:
      return {
        success: false,
        error: 'Dữ liệu bị trùng',
        message: result.message
      };
    
    case 500:
      return {
        success: false,
        error: 'Lỗi server',
        message: result.message
      };
    
    default:
      return {
        success: false,
        error: 'Lỗi không xác định',
        message: result.message
      };
  }
}

// Sử dụng
try {
  const response = await fetch('/api/WorkShifts/get-all', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  });
  
  const result = await handleAPIResponse(response);
  
  if (result.success) {
    console.log('Thành công:', result.data);
  } else {
    console.error('Lỗi:', result.error, '-', result.message);
  }
} catch (error) {
  console.error('Lỗi kết nối:', error);
}
```

### Format TimeSpan

Khi gửi TimeSpan trong request body, sử dụng format:
- `"HH:mm:ss"` (ví dụ: `"08:00:00"`)
- `"HH:mm"` (ví dụ: `"08:00"`)

### Format Guid

Guid phải ở dạng chuỗi với format: `"123e4567-e89b-12d3-a456-426614174000"`

### Headers

Tất cả các request cần có header:
```javascript
{
  'Content-Type': 'application/json'
}
```

Nếu có authentication, thêm header:
```javascript
{
  'Content-Type': 'application/json',
  'Authorization': 'Bearer {token}'
}
```

### Helper Function cho Frontend

```javascript
// File: apiHelper.js

const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Gọi API với xử lý response chuẩn
 */
async function callAPI(endpoint, options = {}) {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      }
    });

    const result = await response.json();

    // Kiểm tra code
    if (result.code === 200 || result.code === 201) {
      return {
        success: true,
        data: result.data,
        message: result.message,
        code: result.code
      };
    } else {
      return {
        success: false,
        data: result.data,
        message: result.message,
        code: result.code
      };
    }
  } catch (error) {
    return {
      success: false,
      message: 'Lỗi kết nối đến server',
      error: error.message
    };
  }
}

// Export để sử dụng
export { callAPI };
```

**Sử dụng:**
```javascript
import { callAPI } from './apiHelper';

// GET request
const roles = await callAPI('/Role');

// POST request
const workShifts = await callAPI('/WorkShifts/get-all', {
  method: 'POST',
  body: JSON.stringify({
    page: 1,
    pageSize: 10
  })
});

if (workShifts.success) {
  console.log('Danh sách:', workShifts.data);
} else {
  console.error('Lỗi:', workShifts.message);
}
```
