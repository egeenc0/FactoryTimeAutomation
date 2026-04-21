import axios from 'axios'

const baseURL = import.meta.env.VITE_API_BASE_URL?.trim() || ''

export const http = axios.create({
  baseURL: baseURL.length > 0 ? baseURL : undefined,
})
