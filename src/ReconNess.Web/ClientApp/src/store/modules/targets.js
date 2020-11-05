import api from '../../api'

const state = {
    targets: [],
    currentTarget: {}
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
    targets(context) {
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
    createTarget(context, target) {
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
    importRootDomain(context, { formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('targets/importRootDomain', this.state.targets.currentTarget.name, formData)
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
}

const mutations = {
    target(state, target) {
        state.currentTarget = target
        if (state.currentTarget.rootDomains.length === 0) {
            state.currentTarget.rootDomains = [{ name: '' }]
        }
    },
    targets(state, targets) {
        state.targets = targets
    },
    createTarget(state, target) {
        state.targets.push(target)
    },
    updateTarget(state) {
        state.targets.forEach((t, i) => {
            if (t.id === state.currentTarget.id) {
                state.targets[i].name = state.currentTarget.name
                state.targets[i].rootDomains = state.currentTarget.rootDomains
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
    }
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}