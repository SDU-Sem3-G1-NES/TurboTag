/* tslint:disable */
/* eslint-disable */

// ReSharper disable InconsistentNaming
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export function isPagedResult<T>(result: unknown): result is PagedResult<T> {
  return (
    typeof result === 'object' &&
    result !== null &&
    Array.isArray((result as PagedResult<T>).items) &&
    typeof (result as PagedResult<T>).totalCount === 'number'
  )
}

export class BaseApiClient {
  protected instance: AxiosInstance
  protected baseUrl: string

  constructor(configuration: ApiConfiguration) {
    this.baseUrl = configuration.baseUrl
    this.instance = configuration.instance
  }
}

export class ApiConfiguration {
  public baseUrl: string
  public instance: AxiosInstance

  constructor(baseUrl: string = 'http://localhost:5088', instance?: AxiosInstance) {
    this.baseUrl = baseUrl
    this.instance =
      instance ||
      axios.create({
        baseURL: baseUrl,
        headers: {
          'Content-Type': 'application/json'
        }
      })

    // Request interceptor for authentication
    this.instance.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('authToken')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor for handling auth errors and token refresh
    this.instance.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config

        // Handle unauthorized errors (401) for token refresh
        if (error.response?.status === 401 && !originalRequest._retry && window.location.pathname !== '/login') {
          originalRequest._retry = true

          try {
            // Attempt to refresh the token
            const refreshToken = localStorage.getItem('refreshToken') || sessionStorage.getItem('refreshToken')
            const accessToken = localStorage.getItem('authToken')
            if (!refreshToken) {
              // No refresh token available, redirect to login
              this.redirectToLogin()
              return Promise.reject(error)
            }

            // Call your token refresh endpoint
            const response = await axios.post(`${baseUrl}/Login/refresh-token`, {
              accessToken, refreshToken
            })

            // If successful, update stored tokens
            if (localStorage.getItem('refreshToken')) {
              localStorage.setItem('refreshToken', response.data.refreshToken)
            } else {
              sessionStorage.setItem('refreshToken', response.data.refreshToken)
            }
            localStorage.setItem('authToken', response.data.accessToken)

            // Update the authorization header and retry
            originalRequest.headers.Authorization = `Bearer ${response.data.accessToken}`
            return axios(originalRequest)
          } catch (refreshError) {
            // If refresh fails, redirect to login
            this.redirectToLogin()
            return Promise.reject(refreshError)
          }
        }

        // Handle forbidden errors (403)
        if (error.response?.status === 403) {
          // Handle access denied - could redirect to forbidden page or show message
          window.location.href = '/forbidden'
          console.error('Access forbidden')
        }

        return Promise.reject(error)
      }
    )
  }

  private redirectToLogin(): void {
    // Clear authentication data
    localStorage.removeItem('authToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('userId')
    localStorage.removeItem('userName')
    sessionStorage.removeItem('refreshToken')
    localStorage.removeItem('userType')

    // Redirect to login page - adjust based on your routing setup
    window.location.href = '/login'
  }
}
export class FileResponse {
  data: Blob;

  constructor(data: Blob) {
    this.data = data;
  }

  static fromJS(data: any): FileResponse {
    return new FileResponse(data);
  }

  toJSON(): any {
    return this.data;
  }
}