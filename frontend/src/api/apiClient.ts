//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import axios, { AxiosError } from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, CancelToken } from 'axios';

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

export class BaseApiClient {
  protected instance: AxiosInstance;
  protected baseUrl: string;

  constructor(configuration: ApiConfiguration) {
    this.baseUrl = configuration.baseUrl;
    this.instance = configuration.instance;
  }
}

export interface ITestClient {
                    /**
             * @return OK
             */
            get(): Promise<string>;
        }

    export class TestClient extends BaseApiClient implements ITestClient {
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

        constructor(configuration: ApiConfiguration = new ApiConfiguration()) {

            super(configuration);

        }

    
    

        /**
         * @return OK
         */
        get( cancelToken?: CancelToken): Promise<string> {
        let url_ = this.baseUrl + "/Test";
        url_ = url_.replace(/[?&]$/, "");

                let options_: AxiosRequestConfig = {
                        method: "GET",
        url: url_,
        headers: {
                                    "Accept": "text/plain"
                },
            cancelToken
        };

                    return this.instance.request(options_).catch((_error: any) => {
                if (isAxiosError(_error) && _error.response) {
        return _error.response;
        } else {
        throw _error;
        }
        }).then((_response: AxiosResponse) => {
                    return this.processGet(_response);
                });
        }

        protected processGet(response: AxiosResponse): Promise<string> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (const k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
                result200 = resultData200 !== undefined ? resultData200 : <any>null;
    
            return Promise.resolve<string>(result200);

        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<string>(null as any);
        }
        }

export class SwaggerException extends Error {
    override message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new SwaggerException(message, status, response, headers, null);
}

function isAxiosError(obj: any): obj is AxiosError {
    return obj && obj.isAxiosError === true;
}

/* tslint:disable */

// ReSharper disable InconsistentNaming