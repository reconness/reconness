import api from '../../api'

const actions = {
    subdomain(context, { targetName, subdomain}) {
        return new Promise((resolve, reject) => {
            try {
                api.get('subdomains/' + targetName + '/' + subdomain)
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
    createSubdomain(context, { target, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('subdomains', { target: target, name: subdomain })
                    .then((res) => {
                        //context.commit('createSubdomain', subdomain)
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    updateSubdomain(context, subdomain) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains', subdomain.id, subdomain)
                    .then(() => {
                        // context.commit('updateSubdomain', subdomain)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    deleteSubdomain(context, { targetName, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('subdomains/' + targetName, subdomain.id)
                    .then(() => {
                        //context.commit('deleteSubdomain', subdomain)
                        resolve()
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
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
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    updateLabel(context, { subdomain, label}) {
        return new Promise((resolve, reject) => {
            try {
                api.update('subdomains/label', subdomain.id, { label: label })
                    .then((res) => {
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


export default {
    namespaced: true,
    actions
}