export const API_URL = () => {
    // https://vitejs.dev/guide/env-and-mode
    if (import.meta.env.DEV)
        return "https://webapp-test.searchathing.com/api"
    else
        return "https://test.searchathing.com/api"
}