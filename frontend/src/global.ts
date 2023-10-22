import axios, { AxiosError } from "axios";
import { AuthApi, Configuration } from "./api";
import { API_URL, APP_URL_Login } from "./constants";

console.log("global startup");

axios.defaults.withCredentials = true;

axios.interceptors.request.use(
  async (config) => {
    console.log("request interceptor");
    return config;
  },

  (error) => {
    Promise.reject(error);
  }
);

axios.interceptors.response.use(
  (response) => {
    console.log("response interceptor");
    return response;
  },
  async function (error: AxiosError) {
    if (error?.response?.status === 401) {

      if (document.location.pathname !== APP_URL_Login)
        document.location = APP_URL_Login;

      if (error.response.headers.length > 0) {
        // console.log('refreshing token')
      }
    }
  }
);

/**
 * @func authApi
 * @description api exported from AuthController.cs
 */
export const authApi = () => {
  const config = new Configuration();
  return new AuthApi(config, API_URL);
};
