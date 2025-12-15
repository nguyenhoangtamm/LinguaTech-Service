# Admin Dashboard API Documentation

## Overview

The Admin Dashboard API provides comprehensive system statistics and analytics for administrators. It allows admins to view overview information about the entire system including user counts, course statistics, enrollment data, materials, and other key metrics.

## Base URL
```
/api/v1/admindashboard
```

## Authentication
- **Required**: Yes
- **Type**: Bearer Token (JWT)
- **Role Required**: Admin

All endpoints require the user to be authenticated and have the "Admin" role.

## API Endpoints

### 1. Get Dashboard Statistics
Retrieve comprehensive system overview statistics.

**Endpoint**: `GET /api/v1/admindashboard/stats`

**Authorization**: Required (Admin role)

**Response**: 
```json
{
  "succeeded": true,
  "message": "Dashboard statistics retrieved successfully",
  "data": {
    "totalUsers": 150,
    "totalCourses": 45,
    "publishedCourses": 38,
    "totalLessons": 250,
    "totalMaterials": 500,
    "totalAssignments": 85,
    "totalEnrollments": 600,
    "activeUsers": 120,
    "activeCourses": 35,
    "totalMaterialsSize": 5368709120,
    "usersByRole": {
      "Active Learners": 120,
      "Total Registered": 150
    },
    "coursesByStatus": {
      "Active": 35,
      "Draft": 7,
      "Archived": 3
    },
    "systemOverview": {
      "lastUpdated": "2024-01-15T10:30:00Z",
      "averageEnrollmentPerCourse": 13.33,
      "totalStorageUsedMB": 5120.5,
      "courseCompletionRate": 84.44,
      "averageCourseRating": 4.2
    }
  },
  "code": 200
}
```

**Response Fields**:
- `totalUsers`: Total number of registered users
- `totalCourses`: Total number of courses in the system
- `publishedCourses`: Number of published courses
- `totalLessons`: Total number of lessons
- `totalMaterials`: Total number of course materials
- `totalAssignments`: Total number of assignments
- `totalEnrollments`: Total number of enrollments
- `activeUsers`: Number of users with active enrollments
- `activeCourses`: Number of courses with active enrollments
- `totalMaterialsSize`: Total size of all materials in bytes
- `usersByRole`: Breakdown of users by role
- `coursesByStatus`: Breakdown of courses by status
- `systemOverview`: System metrics including averages and rates

---

### 2. Get All Course Analytics
Retrieve detailed analytics for all courses.

**Endpoint**: `GET /api/v1/admindashboard/courses/analytics`

**Authorization**: Required (Admin role)

**Response**:
```json
{
  "succeeded": true,
  "message": "Course analytics retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "Introduction to Python",
      "enrollmentCount": 150,
      "rating": 4.5,
      "status": "Active",
      "isPublished": true,
      "createdDate": "2024-01-01T08:00:00Z"
    },
    {
      "id": 2,
      "title": "Advanced JavaScript",
      "enrollmentCount": 89,
      "rating": 4.2,
      "status": "Active",
      "isPublished": true,
      "createdDate": "2024-01-05T08:00:00Z"
    }
  ],
  "code": 200
}
```

**Analytics Includes**:
- Course ID and title
- Number of enrolled students
- Average course rating
- Publication status
- Course creation date
- Course status

---

### 3. Get Top Courses by Enrollment
Retrieve the most enrolled courses.

**Endpoint**: `GET /api/v1/admindashboard/courses/top-by-enrollment`

**Authorization**: Required (Admin role)

**Query Parameters**:
- `limit` (optional, default: 10, max: 100): Number of courses to return

**Example**: `GET /api/v1/admindashboard/courses/top-by-enrollment?limit=5`

**Response**:
```json
{
  "succeeded": true,
  "message": "Top courses by enrollment retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "Introduction to Python",
      "enrollmentCount": 150,
      "rating": 4.5,
      "status": "Active",
      "isPublished": true,
      "createdDate": "2024-01-01T08:00:00Z"
    }
  ],
  "code": 200
}
```

---

### 4. Get Top Courses by Rating
Retrieve the highest-rated courses.

**Endpoint**: `GET /api/v1/admindashboard/courses/top-by-rating`

**Authorization**: Required (Admin role)

**Query Parameters**:
- `limit` (optional, default: 10, max: 100): Number of courses to return

**Example**: `GET /api/v1/admindashboard/courses/top-by-rating?limit=10`

**Response**:
```json
{
  "succeeded": true,
  "message": "Top courses by rating retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "Introduction to Python",
      "enrollmentCount": 150,
      "rating": 4.8,
      "status": "Active",
      "isPublished": true,
      "createdDate": "2024-01-01T08:00:00Z"
    }
  ],
  "code": 200
}
```

---

### 5. Get User Growth Statistics
Retrieve daily user growth statistics.

**Endpoint**: `GET /api/v1/admindashboard/users/growth-stats`

**Authorization**: Required (Admin role)

**Query Parameters**:
- `days` (optional, default: 30, max: 365): Number of days to include in the statistics

**Example**: `GET /api/v1/admindashboard/users/growth-stats?days=30`

**Response**:
```json
{
  "succeeded": true,
  "message": "User growth statistics retrieved successfully",
  "data": [
    {
      "date": "2024-01-01T00:00:00Z",
      "newUsers": 5,
      "totalUsers": 5,
      "activeUsers": 5
    },
    {
      "date": "2024-01-02T00:00:00Z",
      "newUsers": 8,
      "totalUsers": 13,
      "activeUsers": 8
    },
    {
      "date": "2024-01-03T00:00:00Z",
      "newUsers": 3,
      "totalUsers": 16,
      "activeUsers": 3
    }
  ],
  "code": 200
}
```

**Response Fields**:
- `date`: Date of the statistics
- `newUsers`: Number of new user enrollments on that day
- `totalUsers`: Cumulative total of users up to that date
- `activeUsers`: Number of users actively enrolling on that day

---

## Error Handling

### Common Error Responses

**401 Unauthorized**:
```json
{
  "succeeded": false,
  "message": "User is not authenticated",
  "code": 401
}
```

**403 Forbidden** (User doesn't have Admin role):
```json
{
  "succeeded": false,
  "message": "User does not have Admin role",
  "code": 403
}
```

**400 Bad Request**:
```json
{
  "succeeded": false,
  "message": "Limit must be between 1 and 100",
  "code": 400
}
```

**500 Internal Server Error**:
```json
{
  "succeeded": false,
  "message": "An error occurred while retrieving dashboard statistics",
  "code": 500
}
```

---

## Request/Response Examples

### Example 1: Get Dashboard Stats with cURL

```bash
curl -X GET "https://api.linguatech.com/api/v1/admindashboard/stats" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Example 2: Get Top 5 Courses by Enrollment with cURL

```bash
curl -X GET "https://api.linguatech.com/api/v1/admindashboard/courses/top-by-enrollment?limit=5" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Example 3: Using JavaScript/Fetch

```javascript
// Get dashboard statistics
fetch('/api/v1/admindashboard/stats', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => {
  if (data.succeeded) {
    console.log('Dashboard Stats:', data.data);
  } else {
    console.error('Error:', data.message);
  }
})
.catch(error => console.error('Request failed:', error));

// Get user growth statistics
fetch('/api/v1/admindashboard/users/growth-stats?days=30', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => {
  if (data.succeeded) {
    console.log('User Growth:', data.data);
  } else {
    console.error('Error:', data.message);
  }
})
.catch(error => console.error('Request failed:', error));
```

---

## Data Types

### AdminDashboardStatsDto
Complete system statistics and overview.

| Field | Type | Description |
|-------|------|-------------|
| totalUsers | int | Total number of registered users |
| totalCourses | int | Total number of courses |
| publishedCourses | int | Number of published courses |
| totalLessons | int | Total number of lessons |
| totalMaterials | int | Total number of materials |
| totalAssignments | int | Total number of assignments |
| totalEnrollments | int | Total enrollments |
| activeUsers | int | Users with active enrollments |
| activeCourses | int | Courses with active enrollments |
| totalMaterialsSize | long | Total size of materials in bytes |
| usersByRole | Dictionary | Users breakdown by role |
| coursesByStatus | Dictionary | Courses breakdown by status |
| systemOverview | SystemOverviewDto | System metrics |

### CourseAnalyticsDto
Analytics for individual courses.

| Field | Type | Description |
|-------|------|-------------|
| id | int | Course ID |
| title | string | Course title |
| enrollmentCount | int | Number of enrollments |
| rating | double | Course rating (0-5) |
| status | string | Course status |
| isPublished | bool | Publication status |
| createdDate | DateTime | Course creation date |

### UserGrowthStatsDto
Daily user growth statistics.

| Field | Type | Description |
|-------|------|-------------|
| date | DateTime | Date of the statistics |
| newUsers | int | New users on that day |
| totalUsers | int | Cumulative total users |
| activeUsers | int | Active users on that day |

### SystemOverviewDto
System overview metrics.

| Field | Type | Description |
|-------|------|-------------|
| lastUpdated | DateTime | Last update timestamp |
| averageEnrollmentPerCourse | decimal | Average enrollments per course |
| totalStorageUsedMB | decimal | Total storage in MB |
| courseCompletionRate | decimal | Completion rate percentage |
| averageCourseRating | double | Average rating across courses |

---

## Performance Considerations

1. **Dashboard Stats**: This endpoint aggregates data across multiple entities. For large databases, consider caching the results for 5-10 minutes.

2. **Course Analytics**: When retrieving analytics for many courses, the endpoint may take time. Consider implementing pagination or filtering by status/publication date.

3. **User Growth Stats**: For large date ranges (365+ days), the endpoint may consume significant memory. Recommend limiting to 90-day queries by default.

4. **Rate Limiting**: Implement rate limiting to prevent abuse:
   - 100 requests per minute for dashboard stats
   - 50 requests per minute for detailed analytics

---

## Best Practices

1. **Caching**: Cache dashboard statistics for 5-10 minutes to improve performance.
2. **Filtering**: Use date filters when querying growth statistics to reduce payload size.
3. **Error Handling**: Always check the `succeeded` field before accessing `data`.
4. **Token Management**: Ensure JWT tokens are refreshed before expiration.
5. **Logging**: Log all admin dashboard access for security auditing.

---

## Support

For issues or questions regarding the Admin Dashboard API, please contact the development team or refer to the main API documentation.
