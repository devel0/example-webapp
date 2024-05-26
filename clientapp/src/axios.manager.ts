import { Configuration, WebApiServerApi } from "../api"
import { API_URL } from "./constants"

export const genWebApiServerApi = () => {
    const config = new Configuration()
    return new WebApiServerApi(config, API_URL())
}