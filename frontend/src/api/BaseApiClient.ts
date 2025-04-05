/* tslint:disable */

// ReSharper disable InconsistentNaming

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
}

export function isPagedResult<T>(result: any): result is PagedResult<T> {
  return result && Array.isArray(result.items) && typeof result.totalCount === 'number'
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

  constructor(baseUrl: string = 'https://localhost:7275', instance?: AxiosInstance) {
    this.baseUrl = baseUrl
    this.instance =
      instance ||
      axios.create({
        baseURL: baseUrl,
        headers: {
          'Content-Type': 'application/json'
        }
      })

    this.instance.interceptors.request.use(
      (config) => {
        // Add auth token if needed
        // const token = localStorage.getItem("authToken");
        // if (token) config.headers.Authorization = `Bearer ${token}`;
        return config
      },
      (error) => Promise.reject(error)
    )
  }
}
