import api from '../../api'

const state = {
    currentUser: {}
}

const actions = {
    user(context, id) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('users', id)
                    .then((res) => {
                        context.commit('user', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    users() {
        return new Promise((resolve, reject) => {
            try {
                api.get('users')
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
    createUser(context, user) {
        return new Promise((resolve, reject) => {
            try {
                api.create('users', user)
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
    updateUser(context, user) {
        return new Promise((resolve, reject) => {
            try {
                api.update('users', user.id, user)
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
    deleteUser(context, user) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('users', user.id)
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
    notification() {
        return new Promise((resolve, reject) => {
            try {
                api.get('accounts/notification')
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
    saveNotification(context, notification) {
        return new Promise((resolve, reject) => {
            try {
                api.create('accounts/saveNotification', notification)
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
    latestVersion() {
        return new Promise((resolve, reject) => {
            try {
                api.get('accounts/latestVersion')
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
    currentVersion() {
        return new Promise((resolve, reject) => {
            try {
                api.get('accounts/currentVersion')
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
    logfiles() {
        return new Promise((resolve, reject) => {
            try {
                api.get('accounts/logfiles')
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
    readLogfile(context, logFileSelected) {
        return new Promise((resolve, reject) => {
            try {
                api.get('accounts/readLogfile/' + logFileSelected)
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
    cleanLogfile(context, logFileSelected) {
        return new Promise((resolve, reject) => {
            try {
                api.create('accounts/cleanLogfile', { "logFileSelected": logFileSelected })
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
}
const mutations = {
    user(state, user) {
        state.currentUser = user
    }
}

export default {
    namespaced: true,
    state,
    actions,
    mutations
}