/* tslint:disable */

// ReSharper disable InconsistentNaming

export class BaseApiClient {
  protected instance: AxiosInstance;
  protected baseUrl: string;

  constructor(configuration: ApiConfiguration) {
    this.baseUrl = configuration.baseUrl;
    this.instance = configuration.instance;
  }
}

export class ApiConfiguration {
  public baseUrl: string;
  public instance: AxiosInstance;

  constructor(baseUrl: string = 'http://localhost:5088', instance?: AxiosInstance) {
    this.baseUrl = baseUrl;
    this.instance =
      instance ||
      axios.create({
        baseURL: baseUrl,
        headers: {
          'Content-Type': 'application/json'
        }
      });

    this.instance.interceptors.request.use(
      (config) => {
        // Add auth token if needed
        // const token = localStorage.getItem("authToken");
        // if (token) config.headers.Authorization = `Bearer ${token}`;
        return config;
      },
      (error) => Promise.reject(error)
    );
  }
}
