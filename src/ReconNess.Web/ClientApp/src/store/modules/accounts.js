import api from '../../api'

const actions = {
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


export default {
    namespaced: true,
    actions
}