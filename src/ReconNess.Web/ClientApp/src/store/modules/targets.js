import api from '../../api'

const state = {
    targets: [],
    currentTarget: {
        subdomains: []
    },
    currentRootDomain: {}
}

const actions = {
    target(context, targetName) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('targets', targetName)
                    .then((res) => {
                        context.commit('target', res.data)
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    rootDomain(context, { targetName, rootDomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('targets/' + targetName, rootDomain)
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
    async targets(context) {
        return new Promise((resolve, reject) => {
            try {
                api.get('targets')
                    .then((res) => {
                        context.commit('targets', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    createTarget(context,  target) {
        return new Promise((resolve, reject) => {
            try {
                api.create('targets', target)
                    .then(() => {
                        context.commit('createTarget', target)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    updateTarget({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.update('targets', state.currentTarget.id, state.currentTarget)
                    .then(() => {
                        commit('updateTarget')
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    deleteTarget({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('targets', state.currentTarget.name)
                    .then(() => {
                        commit('deleteTarget')
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    deleteAllSubdomains({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('targets', state.currentTarget.name + '/' + state.currentRootDomain.name)
                    .then(() => {
                        commit('deleteAllSubdomains')
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    createSubdomain(context, { subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('subdomains', { target: state.currentTarget.name, rootDomain: state.currentRootDomain.name, name: subdomain })
                    .then((res) => {
                        context.commit('createSubdomain', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    uploadTargets({ commit, state }, { formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('targets', state.currentTarget.name + '/' + state.currentRootDomain.name, formData)
                    .then((res) => {
                        commit('uploadTargets', res.data)
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
    target(state, target) {
        state.currentTarget = target
    },
    targets(state, targets) {
        state.targets = targets
    },
    rootDomain(state, target) {
        state.currentTarget = target
        state.currentRootDomain = target.rootDomains[0] || []
    },
    createTarget(state, target) {
        state.targets.push(target)
    },
    updateTarget(state) {
        state.targets.forEach((t, i) => {
            if (t.id === state.currentTarget.id) {
                state.targets[i].name = state.currentTarget.name
                state.targets[i].rootDomain = state.currentTarget.rootDomain
                state.targets[i].rootDomain = state.currentTarget.rootDomain
                state.targets[i].bugBountyProgramUrl = state.currentTarget.bugBountyProgramUrl
                state.targets[i].isPrivate = state.currentTarget.isPrivate
                state.targets[i].inScope = state.currentTarget.inScope
                state.targets[i].outOfScope = state.currentTarget.outOfScope
            }
        });
    },
    deleteTarget(state) {
        state.targets = state.targets.filter((t) => {
            return t.name !== state.currentTarget.name;
        })
    },
    deleteAllSubdomains(state) {
        state.currentRootDomain.subdomains = []
    },
    createSubdomain(state, subdomain) {
        state.currentRootDomain.subdomains.push(subdomain)
    }, 
    uploadTargets(state, subdomains) {
        subdomains.map(sub => state.currentRootDomain.subdomains.push(sub))        
    }
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}