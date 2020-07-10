import api from '../../api'

const state = {
    references: []
}

const actions = {
    async categories() {
        return new Promise((resolve, reject) => {
            try {
                api.get('references/categories')
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
    async references(context) {
        return new Promise((resolve, reject) => {
            try {
                api.get('references')
                    .then((res) => {
                        context.commit('references', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))

            }
            catch (err) {
                reject(err)
            }
        })
    },
    createReference(context, reference) {
        return new Promise((resolve, reject) => {
            try {
                api.create('references', reference)
                    .then(() => {
                        context.commit('createReference', reference)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
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
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
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