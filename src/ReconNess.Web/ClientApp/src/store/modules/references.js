import api from '../../api'

const state = {
    references: []
}

const actions = {
    async categories() {
        return new Promise((resolve, reject) => {
            api.get('references/categories')
                .then((res) => {
                    resolve(res.data)
                })
                .catch(error => reject(error))
        })
    },
    async references(context) {
        const references = (await api.get('references')).data
        context.commit('references', references)
    },
    createReference(context, reference) {
        return new Promise((resolve, reject) => {
            try {
                api.create('references', reference)
                    .then(() => {
                        context.commit('createReference', reference)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },    
    deleteReference(context, reference) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('references', reference.id)
                    .then(() => {
                        context.commit('deleteReference', reference)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
}

const mutations = {
    references(state, references) {
        state.references = references
    },
    createReference(state, reference) {
        state.references.push(reference)
    },
    deleteReference(state, reference) {
        state.references = state.references.filter((r) => {
            return r.id !== reference.id;
        })
    },
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}