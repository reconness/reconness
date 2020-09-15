﻿import api from '../../api'

const state = {
    currentRootDomain: {}
}

const actions = {    
    rootDomain(context, { targetName, rootDomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('rootdomains/' + targetName, rootDomain)
                    .then((res) => {
                        context.commit('rootDomain', res.data)
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },   
    deleteSubdomains({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('rootdomains/deleteSubdomians', this.state.targets.currentTarget.name + '/' + state.currentRootDomain.name)
                    .then(() => {
                        commit('deleteSubdomains')
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err) 
            }
        })
    },
    createSubdomain({ commit, state }, { subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('subdomains', { target: this.state.targets.currentTarget.name, rootDomain: state.currentRootDomain.name, name: subdomain })
                    .then((res) => {
                        commit('createSubdomain', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    uploadSubdomains({ commit, state }, { formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('rootdomains/uploadSubdomains', this.state.targets.currentTarget.name + '/' + state.currentRootDomain.name, formData)
                    .then((res) => {
                        commit('uploadSubdomains', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    upload({ commit, state }, { formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('rootdomains/upload', this.state.targets.currentTarget.name + '/' + state.currentRootDomain.name, formData)
                    .then((res) => {
                        commit('uploadSubdomains', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    export() {
        return new Promise((resolve, reject) => {
            try {
                api.download('rootdomains/export', this.state.targets.currentTarget.name + '/' + state.currentRootDomain.name)
                    .then((res) => {
                        var fileURL = window.URL.createObjectURL(new Blob([res.data]));
                        var fileLink = document.createElement('a');
                        fileLink.href = fileURL;
                        fileLink.setAttribute('download', 'rootdomain.json');
                        document.body.appendChild(fileLink);
                        fileLink.click();
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    exportSubdomains() {
        return new Promise((resolve, reject) => {
            try {
                api.download('rootdomains/exportSubdomains', this.state.targets.currentTarget.name + '/' + state.currentRootDomain.name)
                    .then((res) => {
                        var fileURL = window.URL.createObjectURL(new Blob([res.data]));
                        var fileLink = document.createElement('a');
                        fileLink.href = fileURL;
                        fileLink.setAttribute('download', 'subdomains.csv');
                        document.body.appendChild(fileLink);
                        fileLink.click();
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    }
}

const mutations = {
    rootDomain(state, target) {        
        this.state.targets.currentTarget = target
        state.currentRootDomain = target.rootDomains[0] || []
    },
    deleteSubdomains(state) {
        state.currentRootDomain.subdomains = []
    },
    createSubdomain(state, subdomain) {
        state.currentRootDomain.subdomains.push(subdomain)
    },
    uploadSubdomains(state, subdomains) {
        subdomains.map(sub => state.currentRootDomain.subdomains.push(sub))
    }
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}