import axios from 'axios'
import { authHeader } from './auth-header';

export default {
    login(username, password) {
        return axios.create({
            baseURL: '/api/',
            headers: { 'Content-Type': 'application/json' }
        })
            .post('auth/login', JSON.stringify({ username, password }))
            .catch(this.handleResponse)
    },
    get(resource) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .get(resource)
            .catch(this.handleResponse)
    },
    getById(resource, id) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .get(`${resource}/${id}`)
            .catch(this.handleResponse)
    },
    create(resource, data) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .post(resource, data)
            .catch(this.handleResponse)
    },
    update(resource, id, data) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .put(`${resource}/${id}`, data)
            .catch(this.handleResponse)
    },
    upload(resource, id, formData) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .post(`${resource}/${id}`, formData)
            .catch(this.handleResponse)
    },
    download(resource, id) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader(),
            responseType: 'blob'
        })
            .post(`${resource}/${id}`)
            .catch(this.handleResponse)
    },
    download(resource) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader(),
            responseType: 'blob'
        })
            .get(`${resource}`)
            .catch(this.handleResponse)
    },
    delete(resource, id) {
        return axios.create({
            baseURL: '/api/',
            headers: authHeader()
        })
            .delete(`${resource}/${id}`)
            .catch(this.handleResponse)
    },
    handleResponse(error) {
        if (error.response.status === 401) {
            // auto logout if 401 response returned from api
            localStorage.removeItem('user');
        }

        return Promise.reject(error.response.data);
    }
}