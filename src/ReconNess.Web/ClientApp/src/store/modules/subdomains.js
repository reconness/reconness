import api from '../../api'

const state = {
    currentSubdomain: {}
}

const actions = {
    subdomain(context, { targetName, rootDomain, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.get('subdomains/' + targetName + '/' + rootDomain + '/' + subdomain)
                    .then((res) => {
                        context.commit('subdomain', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    updateSubdomain({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains', state.currentSubdomain.id, state.currentSubdomain)
                    .then(() => {
                        commit('updateSubdomain')
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    deleteSubdomain(context, { subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('subdomains', subdomain.id)
                    .then(() => {
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
    updateLabel(context, { subdomain, label }) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains/label', subdomain.id, { label: label })
                    .then((res) => {
                        const label = res.data
                        context.commit('updateLabel', { subdomain, label })
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
        const s = this.state.rootdomains.currentRootDomain.subdomains.find(sub => sub.name == subdomain.name)
        if (!s.labels.some(l => l.name === label.name)) {
            s.labels.push(label)
        }
    }
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}