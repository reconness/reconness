import api from '../../api'

const state = {
    targets: []
}

const actions = {
    target(context, targetName) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('targets', targetName)
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    async targets(context) {
        const targets = (await api.get('targets')).data
        context.commit('targets', targets)
    },
    createTarget(context,  target) {
        return new Promise((resolve, reject) => {
            try {
                api.create('targets', target)
                    .then(() => {
                        context.commit('createTarget', target)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    updateTarget(context, target) {
        return new Promise((resolve, reject) => {
            try {
                api.update('targets', target.id, target)
                    .then(() => {
                        context.commit('updateTarget', target)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    deleteTarget(context, target) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('targets', target.name)
                    .then(() => {
                        context.commit('deleteTarget', target)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    deleteAllSubdomains(context, { targetName }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('targets', targetName + '/subdomains')
                    .then(() => {
                        //context.commit('deleteAllSubdomains', targetName)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    uploadTargets(context, { targetName, formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('targets', targetName + '/subdomains', formData)
                    .then((res) => {
                        //context.commit('uploadTargets', targetName)
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    }
}

const mutations = {
    targets(state, targets) {
        state.targets = targets
    },
    createTarget(state, target) {
        state.targets.push(target)
    },
    updateTarget(state, target) {
        state.targets.forEach((t, i) => {
            if (t.id === target.id) {
                state.targets[i].name = target.name
                state.targets[i].rootDomain = target.rootDomain
                state.targets[i].rootDomain = target.rootDomain
                state.targets[i].bugBountyProgramUrl = target.bugBountyProgramUrl
                state.targets[i].isPrivate = target.isPrivate
                state.targets[i].inScope = target.inScope
                state.targets[i].outOfScope = target.outOfScope
            }
        });
    },
    deleteTarget(state, target) {
        state.targets = state.targets.filter((t) => {
            return t.name !== target.name;
        })
    }        
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}