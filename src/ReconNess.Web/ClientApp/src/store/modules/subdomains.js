import api from '../../api'

const state = {
    currentSubdomain: {}
}

const actions = {
    subdomain(context, { targetName, subdomain}) {
        return new Promise((resolve, reject) => {
            try {
                api.get('subdomains/' + targetName + '/' + subdomain)
                    .then((res) => {
                        context.commit('subdomain', res.data)
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    createSubdomain(context, { target, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('subdomains', { target: target, name: subdomain })
                    .then((res) => {
                        context.commit('createSubdomain', res.data)
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    updateSubdomain(context, subdomain) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains', subdomain.id, subdomain)
                    .then(() => {
                        //context.commit('updateSubdomain', subdomain)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    deleteSubdomain(context, { targetName, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('subdomains/' + targetName, subdomain.id)
                    .then(() => {
                        context.commit('deleteSubdomain', subdomain)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    labels() {
        return new Promise((resolve, reject) => {
            try {
                api.get('labels')
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    updateLabel(context, { subdomain, label}) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains/label', subdomain.id, { label: label })
                    .then((res) => {                     
                        const label = res.data
                        context.commit('updateLabel', { subdomain,  label})                        
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
    subdomain(state, subdomain) {
        state.currentSubdomain = subdomain
    },
    updateLabel(state, { subdomain, label }) {
        const s = this.state.targets.currentTarget.subdomains.find(sub => sub.name == subdomain.name)        
        if (!s.labels.some(l => l.name === label.name)) {
            s.labels.push(label)
        }       
    },
    createSubdomain(state, subdomain) {
        this.state.targets.currentTarget.subdomains.push(subdomain)
    },   
    deleteSubdomain(state, subdomina) {
        this.state.targets.currentTarget.subdomains = this.state.targets.currentTarget.subdomains.filter((s) => {
            return s.name !== subdomina.name;
        })
    }    
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}