import api from '../../api'

const actions = {
    get(context) {
        return new Promise((resolve, reject) => {
            try {
                api.get('wordlists')
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
    getContent(context, wordlist) {
        return new Promise((resolve, reject) => {
            try {
                api.get('wordlists/content?type=' + wordlist.type + '&filename=' + wordlist.filename)
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
    save(context, wordlist) {
        return new Promise((resolve, reject) => {
            try {
                api.update('wordlists/' + wordlist.type, wordlist.filename, { data: wordlist.data } )
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
    delete(context, wordlist) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('wordlists/' + wordlist.type, wordlist.filename)
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
    upload({ commit, state }, { type, formData }) {
        return new Promise((resolve, reject) => {
            try {
                api.upload('wordlists/upload', type , formData)
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