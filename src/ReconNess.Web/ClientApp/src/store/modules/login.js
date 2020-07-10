import api from '../../api'

const actions = {
    login(context, { username, password }) {
        return new Promise((resolve, reject) => {
            try {
                api.login(username, password)
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    }
}


export default {
    namespaced: true,
    actions
}